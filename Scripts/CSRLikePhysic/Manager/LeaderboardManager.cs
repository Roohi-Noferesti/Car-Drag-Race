using System;
using System.Collections.Generic;

public class LeaderboardManager
{
	private NetworkReplayManager replayManager;

	private bool hadError;

	private Dictionary<int, EventLeaderboard> CurrentLeaderboards = new Dictionary<int, EventLeaderboard>();

	private Dictionary<string, int> CurrentRPBounds = new Dictionary<string, int>();

	public bool IsRequestingLeaderboards
	{
		get
		{
			return WebRequestQueueRTW.Instance.IsQueued("rtw_active_leaderboards") || WebRequestQueueRTW.Instance.IsQueued("rtw_past_leaderboard");
		}
	}

	public bool HadError
	{
		get
		{
			return this.hadError;
		}
	}

	public LeaderboardManager(NetworkReplayManager replayManager)
	{
		this.replayManager = replayManager;
	}

	public void RequestActiveLeaderboards(bool withTop10 = false, int maxRetries = 4)
	{
		if (WebRequestQueueRTW.Instance == null)
		{
			return;
		}
		if (this.IsRequestingLeaderboards)
		{
			if (WebRequestQueueRTW.Instance.RetryAttempts("rtw_active_leaderboards") >= maxRetries)
			{
				this.hadError = true;
			}
			return;
		}
		if (UserManager.Instance == null || UserManager.Instance.currentAccount == null || UserManager.Instance.currentAccount.UserID == 0)
		{
			this.hadError = true;
			return;
		}
		this.CurrentLeaderboards.Clear();
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
		jsonDict.Set("rp_overall", PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP().ToString());
		jsonDict.Set("rp_season", PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP().ToString());
		jsonDict.Set("display_name", PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback());
		jsonDict.Set("send_leaders", (!withTop10) ? "0" : "1");
		jsonDict.Set("mpt", UserManager.Instance.currentAccount.MPToken);
		WebRequestQueueRTW.Instance.StartCall("rtw_active_leaderboards", "Requesting leaderboard", jsonDict, new WebClientDelegate2(this.LeaderboardResult), UserManager.Instance.currentAccount.UserID, string.Empty, maxRetries);
	}

	public void RequestPastTopTenLeaderboard(int leaderboardID)
	{
		if (this.IsRequestingLeaderboards)
		{
			return;
		}
		this.CurrentLeaderboards.Clear();
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
		jsonDict.Set("leaderboard_id", leaderboardID.ToString());
		jsonDict.Set("mpt", UserManager.Instance.currentAccount.MPToken);
		WebRequestQueueRTW.Instance.StartCall("rtw_past_leaderboard", "Requesting Historic leaderboard", jsonDict, new WebClientDelegate2(this.LeaderboardResult), UserManager.Instance.currentAccount.UserID, string.Empty, 5);
	}

	private bool HandleResults(string content, string error, int status, object userData)
	{
		if (!this.validateResponseHeaders(content, error, status, userData))
		{
			return false;
		}
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(content))
		{
			return false;
		}
		if (!jsonDict.ContainsKey("rankings"))
		{
			return false;
		}
		JsonDict jsonDict2 = jsonDict.GetJsonDict("rankings");
		if (!jsonDict2.ContainsKey("personal"))
		{
			return false;
		}
		JsonList jsonList = jsonDict2.GetJsonList("personal");
		if (this.AreRankingsOutOfDate(jsonList))
		{
			this.RequestActiveLeaderboards(jsonDict2.ContainsKey("leaders"), 1);
			return false;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			this.ParsePeerData(jsonList.GetJsonDict(i));
		}
		if (jsonDict2.ContainsKey("leaders"))
		{
			JsonList jsonList2 = jsonDict2.GetJsonList("leaders");
			for (int j = 0; j < jsonList2.Count; j++)
			{
				this.parseTopTenLeaderboard(jsonList2.GetJsonDict(j));
			}
		}
		if (jsonDict.ContainsKey("rp_bounds"))
		{
			JsonDict jsonDict3 = jsonDict.GetJsonDict("rp_bounds");
			this.CurrentRPBounds = JsonConverter.DeserializeObject<Dictionary<string, int>>(jsonDict3.ToString());
		}
		return true;
	}

	private bool validateResponseHeaders(string content, string error, int status, object userData)
	{
		return status == 200 && error == null && (int)userData == UserManager.Instance.currentAccount.UserID && !string.IsNullOrEmpty(content);
	}

	private void parseTopTenLeaderboard(JsonDict jsonTopTenLeaderboard)
	{
		int @int = jsonTopTenLeaderboard.GetInt("leaderboard_id");
		if (!this.CurrentLeaderboards.ContainsKey(@int))
		{
			this.CurrentLeaderboards.Add(@int, new EventLeaderboard());
		}
		JsonList jsonList = jsonTopTenLeaderboard.GetJsonList("top_ten");
		if (jsonList.Count != 10)
		{
			return;
		}
		List<EventPeerRanking> list = JsonConverter.DeserializeObject<List<EventPeerRanking>>(jsonTopTenLeaderboard.GetJsonList("top_ten").ToString());
		list.Sort(new Comparison<EventPeerRanking>(LeaderboardManager.CompareByRPDescending));
		this.CurrentLeaderboards[@int].topTenEntries = list;
	}

	private void LeaderboardResult(string content, string error, int status, object userData)
	{
		this.hadError = !this.HandleResults(content, error, status, userData);
	}

	private static int CompareByRPDescending(EventPeerRanking a, EventPeerRanking b)
	{
		return b.rp - a.rp;
	}

	private static int CompareByRankAscending(EventPeerRanking a, EventPeerRanking b)
	{
		return a.rank - b.rank;
	}

	public EventLeaderboard GetLeaderboardWithHighestID()
	{
		int max = 0;
		List<int> list = new List<int>(this.CurrentLeaderboards.Keys);
		list.ForEach(delegate(int p)
		{
			max = Math.Max(max, p);
		});
		if (max == 0)
		{
			return null;
		}
		return this.GetLeaderboard(max);
	}

	public EventLeaderboard GetLeaderboard(int leaderboardID)
	{
		if (this.CurrentLeaderboards.ContainsKey(leaderboardID))
		{
			return this.CurrentLeaderboards[leaderboardID];
		}
		return null;
	}

	public bool RPBoundsAvailable()
	{
		return this.CurrentRPBounds.Count > 0;
	}

	public int GetRPBound(int percentile)
	{
		string key = percentile.ToString();
		if (this.CurrentRPBounds.ContainsKey(key))
		{
			return this.CurrentRPBounds[key];
		}
		return -1;
	}

	private bool AreRankingsOutOfDate(JsonList leaderboardRankings)
	{
		int playerRP = PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP();
		if (playerRP == -1)
		{
			return false;
		}
		for (int i = 0; i < leaderboardRankings.Count; i++)
		{
			JsonDict jsonDict = leaderboardRankings.GetJsonDict(i);
			if (SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID() == jsonDict.GetInt("leaderboard_id"))
			{
				int num = (!jsonDict.ContainsKey("rp")) ? 0 : jsonDict.GetInt("rp");
				if (playerRP != num)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ParsePeerData(JsonDict jsonLeaderboard)
	{
		EventLeaderboard eventLeaderboard;
		if (this.CurrentLeaderboards.ContainsKey(jsonLeaderboard.GetInt("leaderboard_id")))
		{
			eventLeaderboard = this.CurrentLeaderboards[jsonLeaderboard.GetInt("leaderboard_id")];
		}
		else
		{
			eventLeaderboard = new EventLeaderboard();
		}
		eventLeaderboard.previousSeasonRank = eventLeaderboard.currentSeasonRank;
		eventLeaderboard.currentSeasonRank = jsonLeaderboard.GetInt("rank");
		eventLeaderboard.previousSeasonRankPercentile = eventLeaderboard.currentSeasonRankPercentile;
		eventLeaderboard.currentSeasonRankPercentile = jsonLeaderboard.GetInt("percentile");
		if (jsonLeaderboard.ContainsKey("rp"))
		{
			eventLeaderboard.currentSeasonRP = jsonLeaderboard.GetInt("rp");
		}
		else
		{
			eventLeaderboard.currentSeasonRP = 0;
		}
		eventLeaderboard.leaderboard_id = jsonLeaderboard.GetInt("leaderboard_id");
		if (eventLeaderboard.leaderboard_id == SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID())
		{
			NetworkReplayServerResponse response = this.replayManager.Response;
			response.previousSeasonRank = response.currentSeasonRank;
			response.previousSeasonRankPercentile = response.currentSeasonRankPercentile;
			response.currentSeasonRank = eventLeaderboard.currentSeasonRank;
			response.currentSeasonRankPercentile = eventLeaderboard.currentSeasonRankPercentile;
			PlayerProfileManager.Instance.ActiveProfile.RaceRewardNewRank(eventLeaderboard.currentSeasonRank);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			PlayerProfileManager.Instance.ActiveProfile.SetPlayerRP(eventLeaderboard.currentSeasonRP);
		}
		eventLeaderboard.peersBelow.Clear();
		eventLeaderboard.peersAbove.Clear();
		JsonList jsonList = jsonLeaderboard.GetJsonList("peers");
		if (jsonList == null)
		{
			return;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			JsonDict jsonDict = jsonList.GetJsonDict(i);
			EventPeerRanking eventPeerRanking = new EventPeerRanking();
			int @int = jsonDict.GetInt("delta_rank");
			eventPeerRanking.rank = eventLeaderboard.currentSeasonRank + @int;
			eventPeerRanking.dname = jsonDict.GetString("display_name");
			eventPeerRanking.rp = jsonDict.GetInt("rp");
			eventPeerRanking.uid = jsonDict.GetInt("uid");
			if (@int > 0)
			{
				eventLeaderboard.peersAbove.Add(eventPeerRanking);
			}
			else
			{
				eventLeaderboard.peersBelow.Add(eventPeerRanking);
			}
		}
		eventLeaderboard.peersAbove.Sort(new Comparison<EventPeerRanking>(LeaderboardManager.CompareByRankAscending));
		if (eventLeaderboard.peersAbove.Count > 10)
		{
			eventLeaderboard.peersAbove.RemoveRange(0, eventLeaderboard.peersAbove.Count - 10);
		}
		eventLeaderboard.peersBelow.Sort(new Comparison<EventPeerRanking>(LeaderboardManager.CompareByRankAscending));
		if (eventLeaderboard.peersBelow.Count > 10)
		{
			eventLeaderboard.peersBelow.RemoveRange(10, eventLeaderboard.peersBelow.Count - 10);
		}
		this.CurrentLeaderboards[eventLeaderboard.leaderboard_id] = eventLeaderboard;
	}

	public EventLeaderboard CreateDummyLeaderboardData(int leaderboardID)
	{
		if (leaderboardID == -1)
		{
			return null;
		}
		EventLeaderboard eventLeaderboard = new EventLeaderboard();
		eventLeaderboard.leaderboard_id = leaderboardID;
		eventLeaderboard.previousSeasonRank = 25;
		eventLeaderboard.previousSeasonRankPercentile = 25;
		eventLeaderboard.topTenEntries = new List<EventPeerRanking>();
		for (int i = 0; i < 10; i++)
		{
			EventPeerRanking eventPeerRanking = new EventPeerRanking();
			eventPeerRanking.dname = "Player #" + i;
			eventPeerRanking.rank = i + 1;
			eventPeerRanking.rp = 10000 - i * 10;
			eventPeerRanking.uid = 0;
			eventLeaderboard.topTenEntries.Add(eventPeerRanking);
		}
		eventLeaderboard.peersAbove = new List<EventPeerRanking>();
		for (int j = 0; j < 3; j++)
		{
			EventPeerRanking eventPeerRanking2 = new EventPeerRanking();
			eventPeerRanking2.dname = "Player #" + (25 - j - 1).ToString();
			eventPeerRanking2.rank = 25 - j - 1;
			eventPeerRanking2.rp = 200 - j - 1;
			eventPeerRanking2.uid = 0;
			eventLeaderboard.peersAbove.Add(eventPeerRanking2);
		}
		eventLeaderboard.peersBelow = new List<EventPeerRanking>();
		for (int k = 0; k < 3; k++)
		{
			EventPeerRanking eventPeerRanking3 = new EventPeerRanking();
			eventPeerRanking3.dname = "Player #" + (25 + k + 1).ToString();
			eventPeerRanking3.rank = 25 + k + 1;
			eventPeerRanking3.rp = 200 - k - 1;
			eventPeerRanking3.uid = 0;
			eventLeaderboard.peersBelow.Add(eventPeerRanking3);
		}
		return eventLeaderboard;
	}
}
