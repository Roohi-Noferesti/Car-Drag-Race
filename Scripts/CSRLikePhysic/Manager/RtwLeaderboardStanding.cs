using System;
using System.Collections.Generic;

public class RtwLeaderboardStanding
{
	public int leaderboard_id
	{
		get;
		private set;
	}

	public int percentile
	{
		get;
		private set;
	}

	public int rp
	{
		get;
		private set;
	}

	public int rank
	{
		get;
		private set;
	}

	public List<int> prizes
	{
		get;
		private set;
	}

	public int did_compete
	{
		get;
		private set;
	}

	public static RtwLeaderboardStanding FromDict(JsonDict jsonDict)
	{
		RtwLeaderboardStanding rtwLeaderboardStanding = new RtwLeaderboardStanding();
		rtwLeaderboardStanding.leaderboard_id = jsonDict.GetInt("leaderboard_id");
		if (jsonDict.Exists("percentile"))
		{
			rtwLeaderboardStanding.percentile = jsonDict.GetInt("percentile");
		}
		else
		{
			rtwLeaderboardStanding.percentile = 0;
		}
		if (jsonDict.Exists("rp"))
		{
			rtwLeaderboardStanding.rp = jsonDict.GetInt("rp");
		}
		else
		{
			rtwLeaderboardStanding.rp = 0;
		}
		if (jsonDict.Exists("rank"))
		{
			rtwLeaderboardStanding.rank = jsonDict.GetInt("rank");
		}
		else
		{
			rtwLeaderboardStanding.rank = 0;
		}
		rtwLeaderboardStanding.prizes = new List<int>();
		rtwLeaderboardStanding.did_compete = jsonDict.GetInt("did_compete");
		if (jsonDict.Exists("prizes"))
		{
			JsonList jsonList = jsonDict.GetJsonList("prizes");
			if (jsonList != null)
			{
				int count = jsonList.Count;
				for (int i = 0; i < count; i++)
				{
					rtwLeaderboardStanding.prizes.Add(jsonList.GetInt(i));
				}
			}
		}
		else
		{
			rtwLeaderboardStanding.prizes.Clear();
		}
		return rtwLeaderboardStanding;
	}
}
