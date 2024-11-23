using System;
using System.Collections.Generic;

public class RtwLeaderboardStatusItem
{
	public int leaderboard_id
	{
		get;
		private set;
	}

	public int finishing_in
	{
		get;
		private set;
	}

	public int event_id
	{
		get;
		private set;
	}

	public bool active
	{
		get;
		private set;
	}

	public bool finished
	{
		get;
		private set;
	}

	public List<RtwLeaderboardPrizeData> prizes
	{
		get;
		private set;
	}

	public static RtwLeaderboardStatusItem FromDict(JsonDict jsonDict)
	{
		RtwLeaderboardStatusItem rtwLeaderboardStatusItem = new RtwLeaderboardStatusItem();
		rtwLeaderboardStatusItem.leaderboard_id = jsonDict.GetInt("leaderboard_id");
		rtwLeaderboardStatusItem.event_id = jsonDict.GetInt("event_id");
		rtwLeaderboardStatusItem.active = (jsonDict.GetInt("active") == 1);
		rtwLeaderboardStatusItem.finished = (jsonDict.GetInt("finished") == 1);
		int finishing_in;
		if (jsonDict.Exists("finishing_in"))
		{
			jsonDict.TryGetValue("finishing_in", out finishing_in);
		}
		else
		{
			finishing_in = -1;
		}
		rtwLeaderboardStatusItem.finishing_in = finishing_in;
		rtwLeaderboardStatusItem.prizes = new List<RtwLeaderboardPrizeData>();
		JsonList jsonList = jsonDict.GetJsonList("prizes");
		int count = jsonList.Count;
		for (int i = 0; i < count; i++)
		{
			RtwLeaderboardPrizeData item = RtwLeaderboardPrizeData.FromDict(jsonList.GetJsonDict(i));
			rtwLeaderboardStatusItem.prizes.Add(item);
		}
		return rtwLeaderboardStatusItem;
	}
}
