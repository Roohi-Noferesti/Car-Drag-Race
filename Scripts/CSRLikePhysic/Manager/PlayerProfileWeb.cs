using System;
using System.Collections.Generic;
using System.IO;
using KingKodeStudio;
using UnityEngine;

public class PlayerProfileWeb
{
    private static DateTime _lastSyncTime = GTDateTime.Now;

    private static DateTime _lastRecoveredProfileExistsTime = GTDateTime.Now;

    private static float _syncFrequencySeconds = 60f;

    private static float _lastRecoveredProfileExistsFrequency = 1f;

    private static readonly HashSet<ScreenID> _safeProfileRestoreScreens = new HashSet<ScreenID>
	{
		ScreenID.Splash,
		ScreenID.Home,
		ScreenID.Workshop,
		ScreenID.CareerModeMap,
        ScreenID.Options
	};

    private static bool _queueSync;

    private static bool _syncing;

    //private static string last_fbEmail = string.Empty;

    public static event AskOverwriteProfileDelegate AskOverwriteProfileEvent;
    public static bool Syncing
    {
        get
        {
            return _syncing;
        }
    }

    public static float SyncFrequencySeconds
    {
        get
        {
            return _syncFrequencySeconds;
        }
        set
        {
            _syncFrequencySeconds = value;
        }
    }

    public static void QueueSync()
    {
        _queueSync = true;
    }

    public static bool RecoveredProfileExists()
    {
        return File.Exists(FileUtils.GetLocalStorageFilePath(UserManager.Instance.currentAccount.Username));
    }

    private static void PollRecoveredProfileExists()
    {
        if (ScreenManager.Instance.CurrentScreen == ScreenID.Intro || ScreenManager.Instance.CurrentScreen == ScreenID.Splash || ScreenManager.Instance.CurrentScreen == ScreenID.Home)
        {
            _lastRecoveredProfileExistsFrequency = 1f;
        }
        else
        {
            _lastRecoveredProfileExistsFrequency = 10f;
        }
        if (_safeProfileRestoreScreens.Contains(ScreenManager.Instance.CurrentScreen) && 
            SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend &&
            !ScreenManager.Instance.IsTranslating &&
            !PopUpManager.Instance.isShowingPopUp &&
            /*PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstTutorialRace() &&*/
            (GarageCameraManager.Instance==null || !GarageCameraManager.Instance.IsZoomedIn) &&
            AskOverwriteProfileEvent != null)
        {
            double totalSeconds = (GTDateTime.Now - _lastRecoveredProfileExistsTime).TotalSeconds;
            if (totalSeconds > _lastRecoveredProfileExistsFrequency)
            {
                _lastRecoveredProfileExistsTime = GTDateTime.Now;
                if (RecoveredProfileExists())
                {
                    AskOverwriteProfileEvent();
                }
            }
        }
    }

    public static void Update()
    {
        double totalSeconds = (GTDateTime.Now - _lastSyncTime).TotalSeconds;
        //We dont want to sync profile in option screen because user may want to restore its data from server and it may create race condition
        if (totalSeconds > SyncFrequencySeconds && ScreenManager.Instance.CurrentScreen != ScreenID.Options)
        {
            Sync();
        }
        PollRecoveredProfileExists();
    }

    public static void Sync()
    {
        if (_queueSync && !RecoveredProfileExists())
        {
            byte[] profileData = null;
            if (PlayerProfileFile.ReadBinaryFile(UserManager.Instance.currentAccount.Username, ref profileData))
            {
                startWebSync(profileData, UserManager.Instance.currentAccount.Username,
                    UserManager.Instance.currentAccount.GCAlias,
                    UserManager.Instance.currentAccount.DeviceToken);
                _queueSync = false;
                _lastSyncTime = GTDateTime.Now;
            }
        }
    }

    public static void SendProfileConfirmed(string userName)
    {
        JsonDict jsonDict = new JsonDict();
        jsonDict.Set("username", userName);
        WebRequestQueue.Instance.StartCall("profile_confirmed", "profile_confirmed", jsonDict, null, null, string.Empty);
    }

    private static void startWebSync(byte[] profileData, string userId, string gcAlias, string deviceToken)
    {
        JsonDict jsonDict = new JsonDict();
        string profileDataString = Convert.ToBase64String(profileData);
        jsonDict.Set("profile_data", profileDataString);
        jsonDict.Set("gc_alias", gcAlias);
        jsonDict.Set("coreid", MetricsIntegration.GetNMCoreIDSafe());
        jsonDict.Set("username", userId);
        //if (gmail != last_fbEmail)
        //{
        //    if (gmail != null)
        //    {
        //        jsonDict.Set("fb_email", gmail);
        //    }
        //    last_fbEmail = gmail;
        //}
        jsonDict.Set("device_token", deviceToken);
        //Debug.Log("Start syncing at "+Time.unscaledTime);
        WebRequestQueue.Instance.StartCall("profile", "Syncing player profile", jsonDict, webSyncFinished, userId, profileDataString);
        _syncing = true;
    }

    private static void webSyncFinished(string content, string error, int status, object userData)
    {
        //Debug.Log("Sync finished at : "+Time.unscaledTime+" , error : "+ error+" , "+content);
        _syncing = false;
        if (status == 200 && !string.IsNullOrEmpty(content) && error == null)
        {
            JsonDict jsonDict = new JsonDict();
            if (jsonDict.Read(content))
            {
                int num;
                if (jsonDict.TryGetValue("profile_sync_freq", out num))
                {
                    SyncFrequencySeconds = num;
                }
                string s;
                if (jsonDict.Exists("profile_data") && jsonDict.TryGetValue("profile_data", out s))
                {
                    byte[] zBytes = Convert.FromBase64String(s);
                    FileUtils.WriteLocalStorage((string)userData, zBytes, false);
                    SendProfileConfirmed(UserManager.Instance.currentAccount.Username);
                }
            }
        }
    }

    public static void GetPlayerProfileData(string username,bool isSyncing, WebClientDelegate2 ProfileDataResponseCallback)
    {
        JsonDict parameters = new JsonDict();
        parameters.Set("username", username);
        parameters.Set("is_syncing", isSyncing);
        WebRequestQueue.Instance.StartCall("get_profile_data", "get Profile Data", parameters, ProfileDataResponseCallback,
            null, UsernameHashCode(username));
    }

    private static string UsernameHashCode(string username)
    {
        return BasePlatform.ActivePlatform.HMACSHA1_Hash(username,BasePlatform.eSigningType.Server_Accounts);
    }
}