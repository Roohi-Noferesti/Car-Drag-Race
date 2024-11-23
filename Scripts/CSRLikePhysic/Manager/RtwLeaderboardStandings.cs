using System;
using System.Collections.Generic;

public class RtwLeaderboardStandings
{
	public List<RtwLeaderboardStanding> Items
	{
		get;
		private set;
	}

	public static RtwLeaderboardStandings FromDict(JsonDict statusDict)
	{
		JsonList jsonList = statusDict.GetJsonList("standings");
		if (jsonList == null)
		{
			return null;
		}
		RtwLeaderboardStandings rtwLeaderboardStandings = new RtwLeaderboardStandings();
		rtwLeaderboardStandings.Items = new List<RtwLeaderboardStanding>();
		int count = jsonList.Count;
		for (int i = 0; i < count; i++)
		{
			RtwLeaderboardStanding item = RtwLeaderboardStanding.FromDict(jsonList.GetJsonDict(i));
			rtwLeaderboardStandings.Items.Add(item);
		}
		return rtwLeaderboardStandings;
	}
}
