using System;
using UnityEngine;

public class RTWStatusManager : MonoBehaviour
{
	public const string MPTokenName = "mpt";

	public float PollFrequency = 600f;

	private float pollTimer;

	public static RTWStatusManager Instance
	{
		get;
		private set;
	}

	public int LastStatus
	{
		get;
		private set;
	}

	public float TimeLeftTillNextPollSeconds
	{
		get
		{
			return this.pollTimer;
		}
	}

	private static bool WaitingForWebRequests
	{
		get
		{
			return !(WebRequestQueueRTW.Instance == null) && WebRequestQueueRTW.Instance.IsQueued("rtw_status");
		}
	}

	private static bool ShouldPoll
	{
	    get
	    {
	        return (SceneLoadManager.Instance != null) &&
	               SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend &&
	               !string.IsNullOrEmpty(UserManager.Instance.currentAccount.MPToken) &&
	               !RTWStatusManager.WaitingForWebRequests && !PopUpManager.Instance.isShowingPopUp &&
	               UserManager.Instance.isLoggedIn && !UserManager.Instance.currentAccount.IsBanned;
	    }
	}

	private void ResetPollTimer()
	{
		this.pollTimer = this.PollFrequency;
	}

	private void Awake()
	{
		RTWStatusManager.Instance = this;
		this.ResetPollTimer();
		UserManager.LoggedInEvent += new UserLoggedInDelegate(this.ForcePollServer);
		UserManager.LoginFailedEvent += new UserLoggedInDelegate(this.ForcePollServer);
		ApplicationManager.DidBecomeActiveEvent += new ApplicationEvent_Delegate(this.ForcePollServer);
		UserManager.UserChangedEvent += new UserChangedDelegate(this.OnUserChanged);
		this.LastStatus = 200;
	}

	private void OnDestroy()
	{
		ApplicationManager.DidBecomeActiveEvent -= new ApplicationEvent_Delegate(this.ForcePollServer);
		UserManager.LoginFailedEvent -= new UserLoggedInDelegate(this.ForcePollServer);
		UserManager.LoggedInEvent -= new UserLoggedInDelegate(this.ForcePollServer);
		UserManager.UserChangedEvent -= new UserChangedDelegate(this.OnUserChanged);
	}

	private void OnUserChanged()
	{
	}

	public void ForcePollServer()
	{
		this.pollTimer = 1f;
	}

	private void Update()
	{
        if (RTWStatusManager.ShouldPoll && this.pollTimer > 0f)
        {
            this.pollTimer -= Time.deltaTime;
            //if (this.pollTimer <= 0f)
            //{
            //    JsonDict jsonDict = new JsonDict();
            //    jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
            //    jsonDict.Set("mpt", UserManager.Instance.currentAccount.MPToken);
            //    WebRequestQueueRTW.Instance.StartCall("rtw_status", "rtw_status", jsonDict,
            //        new WebClientDelegate2(this.HandleStatusResult), UserManager.Instance.currentAccount.UserID,
            //        string.Empty, 1);
            //}
        }
	}

	private void HandleStatusResult(string content, string error, int status, object userData)
	{
        this.LastStatus = status;
        int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
        if (status == 200 && error == null && !string.IsNullOrEmpty(content))
        {
            int num = (int)userData;
            if (num != UserManager.Instance.currentAccount.UserID)
            {
                return;
            }
            RtwStatusResult rtwStatusResult = RtwStatusResult.FromContent(content, num);
            SeasonServerDatabase.Instance.SetLeaderboardStandings(rtwStatusResult.LeaderboardStandings, rtwStatusResult.LeaderboardStatus);
        }
        int mostRecentActiveSeasonLeaderboardID2 = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
        if (mostRecentActiveSeasonLeaderboardID2 != mostRecentActiveSeasonLeaderboardID)
        {
            EventLeaderboard leaderboard = NetworkReplayManager.Instance.Leaderboard.GetLeaderboard(mostRecentActiveSeasonLeaderboardID2);
            if (leaderboard != null)
            {
                PlayerProfileManager.Instance.ActiveProfile.SetPlayerRP(leaderboard.currentSeasonRP);
            }
        }
        this.ResetPollTimer();
	}

    public static bool NetworkStateValidToEnterRTW()
    {
        return !string.IsNullOrEmpty(UserManager.Instance.currentAccount.MPToken) &&
               BasePlatform.ActivePlatform.GetReachability() != BasePlatform.eReachability.OFFLINE &&
               RTWStatusManager.Instance.LastStatus == 200;
    }
}
