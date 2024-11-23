using System;
using System.Collections.Generic;

public class GameCenterLeaderboard
{
	public string categoryId;

	public string title;

	public GameCenterLeaderboard(string categoryId, string title)
	{
		this.categoryId = categoryId;
		this.title = title;
	}

	public static List<GameCenterLeaderboard> fromJSON(string json)
	{
		List<GameCenterLeaderboard> list = new List<GameCenterLeaderboard>();
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(json))
		{
			return list;
		}
		foreach (string current in jsonDict.Keys)
		{
			GameCenterLeaderboard item = new GameCenterLeaderboard(current, jsonDict.GetString(current));
			list.Add(item);
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Leaderboard> categoryId: {0}, title: {1}", this.categoryId, this.title);
	}
}
