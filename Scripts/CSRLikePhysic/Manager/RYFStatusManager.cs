using System;
using System.Collections.Generic;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class RYFStatusManager : MonoBehaviour
{
	public const int DefaultPollFrequency = 700;

	public const int MaxPollFrequency = 5;

	public bool hasReceivedResponse;

	public float PollFrequency = 700f;

	public bool PushMessagesAvailable;

	private DateTime nextPollTime;

    public event RYFStatusManager_Delegate OnFriendsLumpUpdate;

	public static RYFStatusManager Instance
	{
		get;
		private set;
	}

	public string UnixTimeStampOfCachedData
	{
		get;
		private set;
	}

	public int LastStatus
	{
		get;
		private set;
	}

	public static bool HasBeenProcessed
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GetFriendSyncComplete();
		}
	}

	private static bool DebugNoCachedFriendsData
	{
		get
		{
			return false;
		}
	}

	private static bool WaitingForWebRequests
	{
		get
		{
			return !(WebRequestQueueRTW.Instance == null) && WebRequestQueueRTW.Instance.IsQueued(FriendWebRequests.SyncFriendDataRequest);
		}
	}

    private static bool ShouldPoll
    {
        get
        {
            return UserManager.Instance.currentAccount != null && !(SceneLoadManager.Instance == null) &&
                   SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend &&
                   !string.IsNullOrEmpty(UserManager.Instance.currentAccount.RYFToken) && !WaitingForWebRequests &&
                   !PopUpManager.Instance.isShowingPopUp &&
                   !PlayerProfileManager.Instance.ActiveProfile.FirstTimeFriendsUser && UserManager.Instance.isLoggedIn &&
                   GTSystemOrder.SystemsReady && false;//!(ScreenManager.Instance.CurrentScreen is PlayerListScreen);
        }
    }

    public RYFStatusManager()
	{

	}

	private void ResetPollTimer()
	{
		this.nextPollTime = GTDateTime.Now.AddSeconds((double)this.PollFrequency);
	}

	private void Awake()
	{
		Instance = this;
		this.UnixTimeStampOfCachedData = "0";
		this.hasReceivedResponse = false;
		UserManager.LoggedInEvent += new UserLoggedInDelegate(this.ForcePollServer);
		UserManager.LoginFailedEvent += new UserLoggedInDelegate(this.ForcePollServer);
		this.LastStatus = 200;
	}

	public void StartUp()
	{
        this.nextPollTime = GTDateTime.Now;
	}

	public void Shutdown()
	{
	}

	private void OnDestroy()
	{
		UserManager.LoginFailedEvent -= new UserLoggedInDelegate(this.ForcePollServer);
		UserManager.LoggedInEvent -= new UserLoggedInDelegate(this.ForcePollServer);
	}

	public void DoPollServerIn(float when)
	{
        this.nextPollTime = GTDateTime.Now.AddSeconds((double)when);
	}

	public void ForcePollServer()
	{
        this.nextPollTime = GTDateTime.Now;
	}

	public float GetTimeUntilSyncRequest()
	{
        return (float)(this.nextPollTime - GTDateTime.Now).TotalSeconds;
	}

	private void Update()
	{
        if (ShouldPoll && this.nextPollTime <= GTDateTime.Now)
		{
			FriendWebRequests.SyncFriendData(new WebClientDelegate2(this.OnSyncFriendResponse));
            this.nextPollTime = GTDateTime.Now.AddSeconds((double)Math.Max(5f, this.PollFrequency));
		}
	}

	public void OnSyncFriendResponse(string zHTTPContent, string zError, int zStatus, object zUserData)
	{
		this.LastStatus = zStatus;
		if (zStatus == 200 && zError == null)
		{
			if (string.IsNullOrEmpty(zHTTPContent))
			{
				return;
			}
			this.hasReceivedResponse = true;
			JsonDict jsonDict = new JsonDict();
			jsonDict.Read(zHTTPContent);
			this.SetupFriendsList(jsonDict);
			this.SetupLastPollDate(jsonDict);
		}
		Log.AnEvent(Events.SyncFriendsRecieved);
	}

	private void SetupFriendsList(JsonDict dict)
	{
		if (dict.ContainsKey("cached_friend_data"))
		{
			string @string = dict.GetString("cached_friend_data");
			if (string.IsNullOrEmpty(@string) || DebugNoCachedFriendsData)
			{
				return;
			}
			string json = BasePlatform.ActivePlatform.DecompressTextFromCompressedBase64Data(@string);
			JsonList jsonList = new JsonList();
			jsonList.Read(json);
			List<ServerCachedFriendRaceData> serverLump = JsonConverter.DeserializeObject<List<ServerCachedFriendRaceData>>(json);
			LumpManager.Instance.HandleLumpResponse(serverLump);
			this.OnFriendsLumpUpdate();
		}
	}

	private void SetupLastPollDate(JsonDict dict)
	{
		if (dict.ContainsKey("cache_update_time"))
		{
			this.UnixTimeStampOfCachedData = dict.GetString("cache_update_time");
		}
	}

	public static bool NetworkStateValidToEnterRYF()
	{
		return !string.IsNullOrEmpty(UserManager.Instance.currentAccount.RYFToken) && BasePlatform.ActivePlatform.GetReachability() != BasePlatform.eReachability.OFFLINE && Instance.LastStatus == 200;
	}
}
