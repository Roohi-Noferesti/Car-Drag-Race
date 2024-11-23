using System;
using System.Collections.Generic;

public class RtwLeaderboardStatus
{
	public List<RtwLeaderboardStatusItem> Items
	{
		get;
		private set;
	}

	public static RtwLeaderboardStatus FromDict(JsonDict statusDict)
	{
		JsonList jsonList = statusDict.GetJsonList("leaderboard_status");
		if (jsonList == null)
		{
			return null;
		}
		RtwLeaderboardStatus rtwLeaderboardStatus = new RtwLeaderboardStatus();
		rtwLeaderboardStatus.Items = new List<RtwLeaderboardStatusItem>();
		int count = jsonList.Count;
		for (int i = 0; i < count; i++)
		{
			RtwLeaderboardStatusItem item = RtwLeaderboardStatusItem.FromDict(jsonList.GetJsonDict(i));
			rtwLeaderboardStatus.Items.Add(item);
		}
		return rtwLeaderboardStatus;
	}
}
