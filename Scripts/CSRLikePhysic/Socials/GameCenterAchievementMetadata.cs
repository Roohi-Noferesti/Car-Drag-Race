using System;
using System.Collections.Generic;

public class GameCenterAchievementMetadata
{
	public string identifier;

	public string description;

	public string unachievedDescription;

	public bool isHidden;

	public int maximumPoints;

	public string title;

	public GameCenterAchievementMetadata(JsonDict jd)
	{
		this.identifier = jd.GetString("identifier");
		this.description = jd.GetString("achievedDescription");
		this.unachievedDescription = jd.GetString("unachievedDescription");
		this.isHidden = jd.GetBool("hidden");
		this.maximumPoints = jd.GetInt("maximumPoints");
		this.title = jd.GetString("title");
	}

	public static List<GameCenterAchievementMetadata> fromJSON(string json)
	{
		List<GameCenterAchievementMetadata> list = new List<GameCenterAchievementMetadata>();
		JsonList jsonList = new JsonList();
		if (!jsonList.Read(json))
		{
			return list;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			GameCenterAchievementMetadata item = new GameCenterAchievementMetadata(jsonList.GetJsonDict(i));
			list.Add(item);
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<AchievementMetaData> identifier: {0}, hidden: {1}, maxPoints: {2}, title: {3} desc: {4}, unachDesc: {5}", new object[]
		{
			this.identifier,
			this.isHidden,
			this.maximumPoints,
			this.title,
			this.description,
			this.unachievedDescription
		});
	}
}
