using System;
using UnityEngine;

public class ServerSynchronisedTime
{
    public delegate void RequestCallback(bool requestSuccessfull, DateTime serverTime);

    private const string _rtwCall = "rtw_utc";

    private DateTime _serverTimeUTC = DateTime.MinValue;

    private float _appTime;

    private RequestCallback _requestCallbacks;

    private static ServerSynchronisedTime _instance;
    private static DateTime m_startDateOfLeague;
    private static DateTime m_endDateOfLeague;

    private bool _serverTimeValid;

    public bool RequestInProgress
    {
        get;
        private set;
    }

    public bool ServerTimeValid
    {
        get
        {
            if (!_serverTimeValid)
            {
                return false;
            }
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float num = realtimeSinceStartup - _appTime;
            return num >= 0f;
        }
        private set
        {
            _serverTimeValid = value;
        }
    }

    public static DateTime StartDateOfLeague
    {
        get
        {
            if (Instance.GetDateTime() > m_endDateOfLeague)
            {
                UpdateLeagueDate();
            }
            return m_startDateOfLeague;
        }
    }

    public static DateTime EndDateOfLeague
    {
        get
        {
            if (Instance.GetDateTime() > m_endDateOfLeague)
            {
                UpdateLeagueDate();
            }
            return m_endDateOfLeague;
        }
    }

    public bool IsServerTimeMatchClient
    {
        get
        {
            return ServerTimeValid &&
                   Mathf.Abs((int)(GTDateTime.Now - GetDateTime()).TotalHours) <
                   GameDatabase.Instance.OnlineConfiguration.ServerTimeMaxHourDifference;
        }
    }

    public static void UpdateLeagueDate()
    {
        m_startDateOfLeague = Instance.GetDateTime().GetEndOfweek();
        m_endDateOfLeague = Instance.GetDateTime().GetEndOfweek();
    }

    private void OnUserLogin()
    {
        if (!ServerTimeValid)
        {
            RequestServerTime(null);
        }
    }

    public static ServerSynchronisedTime Instance
    {
        get
        {
            return _instance;
        }
    }

    public static void Create()
    {
        _instance = new ServerSynchronisedTime();
        ApplicationManager.WillResignActiveEvent += _instance.OnSuspendApp;
        ApplicationManager.DidBecomeActiveEvent += _instance.OnResumeApp;
        UserManager.LoggedInEvent += _instance.OnUserLogin;
    }

    public void OnResumeApp()
    {
        RequestInProgress = false;
        WebRequestQueueRTW.Instance.RemoveItems(_rtwCall);
        ServerTimeValid = false;
        RequestServerTime(null);
        CallCallbacks();
    }

    public void OnSuspendApp()
    {
        CallCallbacks();
    }

    public static long AddAbsoluteTimeSeconds(long absoluteTimeS, long seconds)
    {
        return absoluteTimeS + seconds * 1000;
    }

    public static long GetAbsoluteTimeNow(bool value)
    {
        return (long)(Instance.GetDateTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public static TimeSpan GetAbsoluteRemaingTimeUntil(long timeMS)
    {
        return GetAbsoluteTimeDifference(timeMS, GetAbsoluteTimeNow(false));
    }


    public static TimeSpan GetAbsoluteTimeDifference(long timeMS1, long timeMS2)
    {
        return new TimeSpan(0, 0, 0, 0, (int)(timeMS1 - timeMS2));
    }

    public DateTime GetDateTime()
    {
        if (!ServerTimeValid)
        {
            return GTDateTime.Now;
        }
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        float num = realtimeSinceStartup - _appTime;
        if (num < 0f)
        {
            return GTDateTime.Now;
        }
        DateTime result = _serverTimeUTC;
        result = result.AddSeconds(num);
        if (GTDateTime.AdjustServerTime)
        {
            result = result.Add(GTDateTime.LocalTimeDelta);
        }
        return result;
    }

    public bool RequestServerTime(RequestCallback onFinished)
    {
        if (RequestInProgress || WebRequestQueueRTW.Instance.IsBusyWith(_rtwCall) || WebRequestQueueRTW.Instance.IsQueued(_rtwCall))
        {
            WebRequestQueueRTW.Instance.RemoveItems(_rtwCall);
            RequestInProgress = false;
        }
        if (ServerTimeValid)
        {
            return false;
        }
        if (!PolledNetworkState.IsNetworkConnected)
        {
            return false;
        }
        _requestCallbacks = onFinished;
        RequestInProgress = true;
        JsonDict jsonDict = new JsonDict();
        jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
        jsonDict.Set("mpt", UserManager.Instance.currentAccount.MPToken);
        WebRequestQueueRTW.Instance.StartCall(_rtwCall, "Sync Server Time", jsonDict, OnRequestFinished, null, string.Empty, 5);
        return true;
    }

    private void OnRequestFinished(string content, string error, int status, object userData)
    {
        if (!RequestInProgress)
        {
            return;
        }
        RequestInProgress = false;
        if (status != 200)
        {
            ServerTimeValid = false;
            CallCallbacks();
            return;
        }
        if (error != null)
        {
            ServerTimeValid = false;
            CallCallbacks();
            return;
        }
        JsonDict jsonDict = new JsonDict();
        jsonDict.Read(content);
        int num = 0;
        if (!jsonDict.TryGetValue("utc", out num))
        {
            ServerTimeValid = false;
            CallCallbacks();
            return;
        }
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
        _serverTimeUTC = dateTime.AddSeconds(num);
        _appTime = Time.realtimeSinceStartup;
        ServerTimeValid = true;
        CallCallbacks();

        GTDebug.Log(GTLogChannel.ServerSynchronisedTime, "Server Time is : " + _serverTimeUTC);
    }

    private void CallCallbacks()
    {
        DateTime serverTime = GTDateTime.Now;
        if (ServerTimeValid)
        {
            serverTime = GetDateTime();
        }
        if (_requestCallbacks != null)
        {
            _requestCallbacks(ServerTimeValid, serverTime);
            _requestCallbacks = null;
        }
    }

}
