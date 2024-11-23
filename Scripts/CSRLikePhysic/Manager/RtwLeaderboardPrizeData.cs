using System;

public class RtwLeaderboardPrizeData
{
	public int prize_id
	{
		get;
		private set;
	}

	public string type
	{
		get;
		private set;
	}

	public int requirement
	{
		get;
		private set;
	}

	public static RtwLeaderboardPrizeData FromDict(JsonDict jsonDict)
	{
		return new RtwLeaderboardPrizeData
		{
			prize_id = jsonDict.GetInt("prize_id"),
			type = jsonDict.GetString("type"),
			requirement = jsonDict.GetInt("requirement")
		};
	}
}
