using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SeasonServerDatabase
{
	[Serializable]
	public class LeaderboardPrizes
	{
		public int prize_id = -1;

		public int requirement = -1;

		public string type = "percentile";
	}

	[Serializable]
	public class LeaderboardStatuses
	{
		public int leaderboard_id = -1;

		public int finishing_in = -1;

		public int event_id = -1;

		public int active;

		public int finished;

		public List<SeasonServerDatabase.LeaderboardPrizes> prizes = new List<SeasonServerDatabase.LeaderboardPrizes>();

		public bool isActive()
		{
			return this.active == 1;
		}

		public bool isFinished()
		{
			return this.finished == 1;
		}
	}

	[Serializable]
	public class LeaderboardStatus
	{
		public List<SeasonServerDatabase.LeaderboardStatuses> leaderboard_status = new List<SeasonServerDatabase.LeaderboardStatuses>();
	}

	[Serializable]
	public class LeaderboardPeerRanking
	{
		public int delta_rank;

		public int uid = -1;

		public string display_name = "PROXY_DISPLAY_NAME";

		public int rp = 2147483647;
	}

	[Serializable]
	public class LeaderboardStanding
	{
		public int leaderboard_id = -1;

		public int percentile = 100;

		public int rp;

		public int rank = 2147483647;

		public int did_compete;

		public List<int> prizes = new List<int>();

		public List<SeasonServerDatabase.LeaderboardPeerRanking> peers = new List<SeasonServerDatabase.LeaderboardPeerRanking>();

		public bool didCompete()
		{
			return this.did_compete == 1;
		}
	}

	[Serializable]
	public class LeaderboardStandings
	{
		public List<SeasonServerDatabase.LeaderboardStanding> standings = new List<SeasonServerDatabase.LeaderboardStanding>();
	}

	public delegate void SeasonServerDataUpdated(SeasonServerDatabase updatedDatabase);

	private static SeasonServerDatabase _instance;

	private RtwLeaderboardStatus leaderboardStatus;

	private RtwLeaderboardStandings leaderboardStandings;

    public event SeasonServerDatabase.SeasonServerDataUpdated ServerLeaderboardStandingsUpdated;

    public event SeasonServerDatabase.SeasonServerDataUpdated ServerLeaderboardStatusesUpdated;

	public static SeasonServerDatabase Instance
	{
		get
		{
			if (SeasonServerDatabase._instance == null)
			{
				SeasonServerDatabase._instance = new SeasonServerDatabase();
			}
			return SeasonServerDatabase._instance;
		}
	}

	private SeasonServerDatabase()
	{
		this.ClearAll();
	}

	public void Shutdown()
	{
		this.ClearAll();
	}

	public void ClearAll()
	{
		this.leaderboardStatus = null;
		this.leaderboardStandings = null;
	}

	public void SetLeaderboardStatus(RtwLeaderboardStatus newStatus)
	{
		this.leaderboardStatus = newStatus;
		if (this.ServerLeaderboardStatusesUpdated != null)
		{
			this.ServerLeaderboardStatusesUpdated(this);
		}
	}

	public void SetLeaderboardStandings(RtwLeaderboardStandings newStandings, RtwLeaderboardStatus newStatus)
	{
		this.SetLeaderboardStatus(newStatus);
		this.leaderboardStandings = newStandings;
		if (this.ServerLeaderboardStandingsUpdated != null)
		{
			this.ServerLeaderboardStandingsUpdated(this);
		}
	}

	public bool DoWeHaveStatusAndStandings()
	{
		return this.leaderboardStatus != null && this.leaderboardStandings != null;
	}

	public bool AreStandingsUpToDate()
	{
		if (!this.DoWeHaveStatusAndStandings())
		{
			return false;
		}
		List<RtwLeaderboardStatusItem> list = this.GetInactiveLeaderboards();
		list = list.FindAll((RtwLeaderboardStatusItem p) => p.finished);
		return list.TrueForAll((RtwLeaderboardStatusItem p) => this.GetStandingsForLeaderboard(p.leaderboard_id) != null);
	}

	public bool IsAnySeasonActive()
	{
		List<RtwLeaderboardStatusItem> activeLeaderboards = this.GetActiveLeaderboards();
		RtwLeaderboardStatusItem rtwLeaderboardStatusItem = activeLeaderboards.Find((RtwLeaderboardStatusItem p) => p.leaderboard_id != 0 && this.LeaderboardIDIsASeason(p.leaderboard_id));
		return rtwLeaderboardStatusItem != null;
	}

	private bool LeaderboardIDIsASeason(int leaderboardID)
	{
		SeasonEventMetadata eventForLeaderboard = this.GetEventForLeaderboard(leaderboardID);
		return eventForLeaderboard != null && eventForLeaderboard.EventType == SeasonEventType.Season;
	}

	private List<RtwLeaderboardStatusItem> GetActiveLeaderboards()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<RtwLeaderboardStatusItem>();
		}
		return this.leaderboardStatus.Items.FindAll((RtwLeaderboardStatusItem p) => p.active);
	}

	private List<RtwLeaderboardStatusItem> GetInactiveLeaderboards()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<RtwLeaderboardStatusItem>();
		}
		return this.leaderboardStatus.Items.FindAll((RtwLeaderboardStatusItem p) => !p.active);
	}

	public List<RtwLeaderboardStatusItem> GetAllLeaderbordsWithCountdowns()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<RtwLeaderboardStatusItem>();
		}
		return this.leaderboardStatus.Items.FindAll((RtwLeaderboardStatusItem p) => p.finishing_in != -1);
	}

	public List<RtwLeaderboardStanding> GetAllLeaderboardStandings()
	{
		if (this.leaderboardStandings == null)
		{
			return new List<RtwLeaderboardStanding>();
		}
		return this.leaderboardStandings.Items;
	}

	public List<int> GetAllEventIDs()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<int>();
		}
		IEnumerable<int> source = from p in this.leaderboardStatus.Items
		select p.event_id;
		return source.Distinct<int>().ToList<int>();
	}

	public List<int> GetAllPrizeIDs()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<int>();
		}
		return SeasonServerDatabase.GetUniquePrizeIDsForStatuses(this.leaderboardStatus.Items);
	}

	public List<int> GetAllFinishedEventPrizeIDs()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<int>();
		}
		List<RtwLeaderboardStatusItem> statuses = this.leaderboardStatus.Items.FindAll((RtwLeaderboardStatusItem p) => !p.active && p.finished);
		return SeasonServerDatabase.GetUniquePrizeIDsForStatuses(statuses);
	}

	private static List<int> GetUniquePrizeIDsForStatuses(List<RtwLeaderboardStatusItem> statuses)
	{
		List<int> prizedIDs = new List<int>();
		statuses.ForEach(delegate(RtwLeaderboardStatusItem p)
		{
			prizedIDs.AddRange(from q in p.prizes
			select q.prize_id);
		});
		return prizedIDs.Distinct<int>().ToList<int>();
	}

	public int GetMostRecentActiveSeasonEventID()
	{
		if (this.leaderboardStatus == null || this.leaderboardStatus.Items.Count <= 0)
		{
			return -1;
		}
		List<RtwLeaderboardStatusItem> list = this.GetAllSeasonStatusItems();
		list = list.FindAll((RtwLeaderboardStatusItem p) => p.active);
		if (list.Count <= 0)
		{
			return -1;
		}
		int max = 0;
		list.ForEach(delegate(RtwLeaderboardStatusItem p)
		{
			max = Mathf.Max(max, p.event_id);
		});
		return max;
	}

	public int GetMostRecentActiveSeasonLeaderboardID()
	{
		if (this.leaderboardStatus == null || this.leaderboardStatus.Items.Count <= 0)
		{
			return -1;
		}
		List<RtwLeaderboardStatusItem> list = this.GetAllSeasonStatusItems();
		list = list.FindAll((RtwLeaderboardStatusItem p) => p.active);
		if (list.Count <= 0)
		{
			return -1;
		}
		int max = 0;
		list.ForEach(delegate(RtwLeaderboardStatusItem p)
		{
			max = Mathf.Max(max, p.leaderboard_id);
		});
		return max;
	}

	public RtwLeaderboardStatusItem GetLeaderboardStatusForID(int leaderboardID)
	{
		if (this.leaderboardStatus == null)
		{
			return null;
		}
		List<RtwLeaderboardStatusItem> allSeasonStatusItems = this.GetAllSeasonStatusItems();
		return allSeasonStatusItems.Find((RtwLeaderboardStatusItem p) => p.leaderboard_id == leaderboardID);
	}

	public RtwLeaderboardStatusItem GetLeaderboardStatusForUpcommingSeason()
	{
		if (this.leaderboardStatus == null)
		{
			return null;
		}
		List<RtwLeaderboardStatusItem> allSeasonStatusItems = this.GetAllSeasonStatusItems();
		return allSeasonStatusItems.Find((RtwLeaderboardStatusItem p) => !p.active && !p.finished);
	}

	private List<RtwLeaderboardStatusItem> GetAllSeasonStatusItems()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<RtwLeaderboardStatusItem>();
		}
		return this.leaderboardStatus.Items.FindAll((RtwLeaderboardStatusItem p) => this.LeaderboardIDIsASeason(p.leaderboard_id));
	}

	public List<RtwLeaderboardStatusItem> GetAllLeaderboardStatuses()
	{
		if (this.leaderboardStatus == null)
		{
			return new List<RtwLeaderboardStatusItem>();
		}
		return this.leaderboardStatus.Items;
	}

	public RtwLeaderboardStanding GetStandingsForLeaderboard(int leaderboardID)
	{
		if (this.leaderboardStandings == null)
		{
			return null;
		}
		return this.leaderboardStandings.Items.Find((RtwLeaderboardStanding p) => p.leaderboard_id == leaderboardID);
	}

	public int GetUnlockRequirementForPrize(SeasonPrizeIdentifier prize)
	{
		RtwLeaderboardStatusItem rtwLeaderboardStatusItem = this.leaderboardStatus.Items.Find((RtwLeaderboardStatusItem p) => p.leaderboard_id == prize.LeaderboardID);
		if (rtwLeaderboardStatusItem == null)
		{
			return -1;
		}
		RtwLeaderboardPrizeData rtwLeaderboardPrizeData = rtwLeaderboardStatusItem.prizes.Find((RtwLeaderboardPrizeData p) => p.prize_id == prize.PrizeID);
		if (rtwLeaderboardPrizeData == null)
		{
			return -1;
		}
		return rtwLeaderboardPrizeData.requirement;
	}

	public SeasonEventMetadata GetEventForLeaderboard(int leaderboardID)
	{
		if (this.leaderboardStatus == null)
		{
			return null;
		}
		RtwLeaderboardStatusItem rtwLeaderboardStatusItem = this.leaderboardStatus.Items.Find((RtwLeaderboardStatusItem p) => p.leaderboard_id == leaderboardID);
		if (rtwLeaderboardStatusItem == null)
		{
			return null;
		}
		return GameDatabase.Instance.SeasonEvents.GetEvent(rtwLeaderboardStatusItem.event_id);
	}

	public int GetMostRecentSeasonStandingsLeaderboardID()
	{
		if (!this.AreStandingsUpToDate())
		{
			return -1;
		}
		List<RtwLeaderboardStanding> allLeaderboardStandings = this.GetAllLeaderboardStandings();
		if (allLeaderboardStandings.Count <= 0)
		{
			return -1;
		}
		int max = 0;
		allLeaderboardStandings.ForEach(delegate(RtwLeaderboardStanding p)
		{
			max = Mathf.Max(max, p.leaderboard_id);
		});
		return max;
	}

	public int getMostRecentFinishedSeasonStatusLeaderboardID()
	{
		if (this.leaderboardStatus == null)
		{
			return -1;
		}
		List<RtwLeaderboardStatusItem> list = this.GetAllSeasonStatusItems();
		list = list.FindAll((RtwLeaderboardStatusItem p) => !p.active && p.finished);
		if (list.Count <= 0)
		{
			return -1;
		}
		int max = 0;
		list.ForEach(delegate(RtwLeaderboardStatusItem p)
		{
			max = Mathf.Max(max, p.leaderboard_id);
		});
		return max;
	}

	public int GetMostRecentCompetedSeasonStandingsLeaderboardID()
	{
		if (!this.AreStandingsUpToDate())
		{
			return -1;
		}
		List<RtwLeaderboardStanding> list = this.GetAllLeaderboardStandings();
		list = list.FindAll((RtwLeaderboardStanding p) => p.did_compete == 1);
		if (list.Count <= 0)
		{
			return -1;
		}
		int max = 0;
		list.ForEach(delegate(RtwLeaderboardStanding p)
		{
			max = Mathf.Max(max, p.leaderboard_id);
		});
		return max;
	}
}
