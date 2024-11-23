using System;

public class RtwStatusResult
{
	public int UserId
	{
		get;
		private set;
	}

	public RtwLeaderboardStatus LeaderboardStatus
	{
		get;
		private set;
	}

	public RtwLeaderboardStandings LeaderboardStandings
	{
		get;
		private set;
	}

	public static RtwStatusResult FromContent(string json, int userId)
	{
		JsonDict jsonDict = new JsonDict();
		RtwStatusResult rtwStatusResult = new RtwStatusResult();
		if (jsonDict.Read(json))
		{
			rtwStatusResult.UserId = userId;
			rtwStatusResult.LeaderboardStatus = RtwLeaderboardStatus.FromDict(jsonDict);
			rtwStatusResult.LeaderboardStandings = RtwLeaderboardStandings.FromDict(jsonDict);
			return rtwStatusResult;
		}
		return rtwStatusResult;
	}
}
