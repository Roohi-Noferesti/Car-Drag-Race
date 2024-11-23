using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KingKodeStudio;
using UnityEngine;

public class NetworkReplayManager : MonoBehaviour
{
	public class Ranking
	{
		public int rp
		{
			get;
			private set;
		}

		public int percentile
		{
			get;
			private set;
		}

		public int leaderboard_id
		{
			get;
			private set;
		}

		private Ranking()
		{
		}

		public static Ranking FromJsonDict(JsonDict jsonDict)
		{
			return new Ranking
			{
				rp = jsonDict.GetInt("rp"),
				percentile = jsonDict.GetInt("percentile"),
				leaderboard_id = jsonDict.GetInt("leaderboard_id")
			};
		}

		public static List<Ranking> FromJsonList(JsonList jsonList)
		{
			List<Ranking> list = new List<Ranking>(jsonList.Count);
			for (int i = 0; i < jsonList.Count; i++)
			{
				list.Add(FromJsonDict(jsonList.GetJsonDict(i)));
			}
			return list;
		}
	}

	private delegate NetworkReplayWorkItem NetworkReplayWorkItemFactory(JsonDict json);

	private const float TimeBetweenFailedWebClientRequests = 60f;

	private const string Filename = "nrm_wq_v2.txt";

	private const string ReplayName = "SavedReplay.txt";

    private bool uploading;

    public LeaderboardManager Leaderboard;

	private Queue<NetworkReplayWorkItem> workQueue = new Queue<NetworkReplayWorkItem>();

	private float nextWebClientRequestTimer;

	private Dictionary<ReplayType, NetworkReplayWorkItemFactory> ReplayTypeCreator;

	public static NetworkReplayManager Instance
	{
		get;
		private set;
	}

	public NetworkReplayServerResponse Response
	{
		get;
		private set;
	}

	public bool rankIsUpToDate
	{
		get
		{
			return this.workQueue.Count == 0;
		}
	}

	public NetworkReplayManager()
	{
		Dictionary<ReplayType, NetworkReplayWorkItemFactory> dictionary = new Dictionary<ReplayType, NetworkReplayWorkItemFactory>();
		dictionary.Add(ReplayType.RaceTheWorld, (JsonDict dict) => new NetworkReplayWorkItemRTW(dict, Instance.Response));
		dictionary.Add(ReplayType.RaceYourFriends, (JsonDict dict) => new NetworkReplayWorkItemFriends(dict));
		this.ReplayTypeCreator = dictionary;
	}

	public void OnProfileChanged()
	{
		this.LoadWorkQueueFromLocalStorage();
        RTWStatusManager.Instance.ForcePollServer();
	}

	private void Awake()
	{
		Instance = this;
        this.Leaderboard = new LeaderboardManager(this);
		this.Response = new NetworkReplayServerResponse();
		ApplicationManager.DidBecomeActiveEvent += new ApplicationEvent_Delegate(this.ApplicationDidBecomeActiveEvent);
		ApplicationManager.WillResignActiveEvent += new ApplicationEvent_Delegate(this.ApplicationWillResignActiveEvent);

	}

    private void OnDestroy()
	{
		ApplicationManager.DidBecomeActiveEvent -= new ApplicationEvent_Delegate(this.ApplicationDidBecomeActiveEvent);
		ApplicationManager.WillResignActiveEvent -= new ApplicationEvent_Delegate(this.ApplicationWillResignActiveEvent);

	}

	private void ApplicationDidBecomeActiveEvent()
	{
		//this.LoadWorkQueueFromLocalStorage();
	}

	private void ApplicationWillResignActiveEvent()
	{
		//this.SaveWorkQueueToLocalStorage();
	}

	public PlayerReplay LoadReplay()
	{
		string text = FileUtils.DecompressFromLocalStorage("SavedReplay.txt", true, false);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return PlayerReplay.CreateFromJson(BasePlatform.ActivePlatform.FX(text), null);
	}

	public void SaveReplay(PlayerReplay playerReplay, string fileName)
	{
		string zContent = playerReplay.ToJson();
		FileUtils.WriteLocalStorage(fileName, zContent, false, false);
	}

	public void SaveWorkQueueToLocalStorage()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.workQueue.Count.ToString());
		while (this.workQueue.Count > 0)
		{
			NetworkReplayWorkItem networkReplayWorkItem = this.workQueue.Dequeue();
			stringBuilder.AppendLine(networkReplayWorkItem.Type().ToString());
			JsonDict jsonDict = new JsonDict();
			networkReplayWorkItem.ToJson(jsonDict);
			stringBuilder.AppendLine(jsonDict.ToString());
		}
		FileUtils.CompressToLocalStorage("nrm_wq_v2.txt", BasePlatform.ActivePlatform.TX(stringBuilder.ToString()), true, false);
		this.workQueue.Clear();
	}

	public void LoadWorkQueueFromLocalStorage()
	{
		this.workQueue.Clear();
		string text = FileUtils.DecompressFromLocalStorage("nrm_wq_v2.txt", true, false);
		if (!string.IsNullOrEmpty(text))
		{
			text = BasePlatform.ActivePlatform.FX(text);
			using (StringReader stringReader = new StringReader(text))
			{
				int i = int.Parse(stringReader.ReadLine());
				while (i > 0)
				{
					ReplayType key = (ReplayType)((int)Enum.Parse(typeof(ReplayType), stringReader.ReadLine()));
					string json = stringReader.ReadLine();
					NetworkReplayWorkItemFactory networkReplayWorkItemFactory;
					if (this.ReplayTypeCreator.TryGetValue(key, out networkReplayWorkItemFactory))
					{
						JsonDict jsonDict = new JsonDict();
						jsonDict.Read(json);
						NetworkReplayWorkItem item = networkReplayWorkItemFactory(jsonDict);
						this.workQueue.Enqueue(item);
						i--;
					}
				}
			}
			FileUtils.EraseLocalStorageFile("nrm_wq_v2.txt", false);
		}
	}

	private NetworkReplayWorkItem CreateWorkItem(PlayerReplay zLocalPlayerReplay, PlayerReplay zOpponentPlayerReplay, RaceEventData zEventData, ReplayType zReplayType)
	{
		if (zReplayType == ReplayType.RaceYourFriends)
		{
			return new NetworkReplayWorkItemFriends(zLocalPlayerReplay);
		}
		NetworkReplayWorkItemRTW networkReplayWorkItemRTW = new NetworkReplayWorkItemRTW(zLocalPlayerReplay, zOpponentPlayerReplay, zEventData, this.Response);
        this.FakeServerResponse(networkReplayWorkItemRTW);
		return networkReplayWorkItemRTW;
	}

	public bool UnitTestLoadAndSave()
	{
		List<NetworkReplayWorkItem> copyQueue = new List<NetworkReplayWorkItem>();
		this.workQueue.ToList<NetworkReplayWorkItem>().ForEach(delegate(NetworkReplayWorkItem item)
		{
			copyQueue.Add(item);
		});
		this.SaveWorkQueueToLocalStorage();
		this.LoadWorkQueueFromLocalStorage();
		bool result = true;
		List<NetworkReplayWorkItem> list = this.workQueue.ToList<NetworkReplayWorkItem>();
		for (int i = 0; i < copyQueue.Count; i++)
		{
			if (!list[i].Equals(copyQueue[i]))
			{
				result = false;
			}
		}
		return result;
	}

	public void AddReplayToUploadQueue(PlayerReplay zLocalPlayerReplay, PlayerReplay zOpponentPlayerReplay, RaceEventData zEventData, ReplayType zReplayType)
	{
		if (RaceEventInfo.Instance.CurrentEvent.IsTutorial() || RaceEventInfo.Instance.CurrentEvent.IsTestDrive())
			return;
		
		NetworkReplayWorkItem item = this.CreateWorkItem(zLocalPlayerReplay, zOpponentPlayerReplay, zEventData, zReplayType);
		this.workQueue.Enqueue(item);
	}

	private void Update()
	{
		if (this.nextWebClientRequestTimer > 0f)
		{
			this.nextWebClientRequestTimer -= Time.deltaTime;
		}
		bool flag = SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend || ScreenManager.Instance.CurrentScreen == ScreenID.RaceResults || ScreenManager.Instance.CurrentScreen == ScreenID.RaceRewards || ScreenManager.Instance.CurrentScreen == ScreenID.LevelUp;
		if (!flag || WebRequestQueueRTW.Instance.QueueCount > 0 || this.workQueue.Count < 1 || this.nextWebClientRequestTimer > 0f || !PolledNetworkState.IsNetworkConnected)
		{
			return;
		}
		NetworkReplayWorkItem networkReplayWorkItem = this.workQueue.Peek();
		networkReplayWorkItem.Upload(new WebClientDelegate2(this.ReplayUploadFinished));
	    uploading = true;
	}


    private void OnlineRaceGameEvents_AddUserReplayCompleted()
    {
        if (this.workQueue != null && this.workQueue.Count>0)
            this.workQueue.Dequeue();
        uploading = false;
    }

	private void ReplayUploadFinished(string content, string error, int status, object userData)
	{
		NetworkReplayWorkItem networkReplayWorkItem = this.workQueue.Peek();
		bool flag = false;
		if (status == 200 && error == null)
		{
            if ((int)userData != UserManager.Instance.currentAccount.UserID)
            {
                this.workQueue.Dequeue();
                return;
            }
			if (networkReplayWorkItem.ProcessContent(content))
			{
				flag = true;
			}
		}
		else if (status >= 400 && status <= 499)
		{
			flag = true;
		}
		else
		{
			this.nextWebClientRequestTimer = 60f;
		}
		if (flag)
		{
			this.workQueue.Dequeue();
		}
	}

	private void FakeServerResponse(NetworkReplayWorkItemRTW workItem)
	{
        RacePlayerInfoComponent component = workItem.PlayerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
        int playerRP = PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP();
        int pPIndex = component.PPIndex;
	    float deltaT = 0;//workItem.OpponentReplay.replayData.finishTime - workItem.PlayerReplay.replayData.finishTime;
        MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        float eventMultiplier = (data == null) ? 1f : data.RPMultiplier;
        RPCalculator.CalculateRP(deltaT, pPIndex, playerRP, workItem.IsEliteRace, workItem.IsEvent, workItem.IsBlogger, eventMultiplier, out this.Response.raceBonus, out this.Response.leadBonus, out this.Response.eliteBonus, out this.Response.worldTourBonus, out this.Response.deltaRP);
		this.Response.bloggerBonus = 0;
		this.Response.currentSeasonRP = Math.Max(0, PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP() + this.Response.deltaRP);
	}

	public void FakeServerResponseSetRP()
	{
		PlayerProfileManager.Instance.ActiveProfile.SetPlayerRP(this.Response.currentSeasonRP);
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
	}
}
