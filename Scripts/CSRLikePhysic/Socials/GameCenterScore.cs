using System;
using System.Collections.Generic;

public class GameCenterScore
{
	public string category;

	public string formattedValue;

	public int value;

	public DateTime date;

	public string playerId;

	public int rank;

	public bool isFriend;

	public string alias;

	public int maxRange;

	public GameCenterScore(JsonDict jd)
	{
		this.category = jd.GetString("category");
		this.formattedValue = jd.GetString("formattedValue");
		this.value = jd.GetInt("value");
		this.playerId = jd.GetString("playerId");
		this.rank = jd.GetInt("rank");
		this.isFriend = jd.GetBool("isFriend");
		this.alias = jd.GetString("alias");
		this.maxRange = jd.GetInt("maxRange");
		float @float = jd.GetFloat("date");
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		this.date = dateTime.AddSeconds((double)@float);
	}

	public static List<GameCenterScore> fromJSON(string json)
	{
		List<GameCenterScore> list = new List<GameCenterScore>();
		JsonList jsonList = new JsonList();
		if (!jsonList.Read(json))
		{
			return list;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			JsonDict jsonDict = jsonList.GetJsonDict(i);
			GameCenterScore item = new GameCenterScore(jsonDict);
			list.Add(item);
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Score> category: {0}, formattedValue: {1}, date: {2}, rank: {3}, alias: {4}", new object[]
		{
			this.category,
			this.formattedValue,
			this.date,
			this.rank,
			this.alias
		});
	}
}
