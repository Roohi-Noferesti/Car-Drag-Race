using System;
using System.Collections.Generic;

public class GameCenterAchievement
{
	public string categoryId;

	public bool isHidden;

	public bool completed;

	public DateTime lastReportedDate;

	public float percentComplete;

	public GameCenterAchievement(JsonDict jd)
	{
		this.categoryId = jd.GetString("identifier");
		this.isHidden = jd.GetBool("hidden");
		this.completed = jd.GetBool("completed");
		this.percentComplete = jd.GetFloat("percentComplete");
		float @float = jd.GetFloat("lastReportedDate");
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		this.lastReportedDate = dateTime.AddSeconds((double)@float);
	}

	public static List<GameCenterAchievement> fromJSON(string json)
	{
		List<GameCenterAchievement> list = new List<GameCenterAchievement>();
		JsonList jsonList = new JsonList();
		if (!jsonList.Read(json))
		{
			return list;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			JsonDict jsonDict = jsonList.GetJsonDict(i);
			GameCenterAchievement item = new GameCenterAchievement(jsonDict);
			list.Add(item);
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Achievement> categoryId: {0}, hidden: {1}, completed: {2}, percentComplete: {3}, lastReported: {4}", new object[]
		{
			this.categoryId,
			this.isHidden,
			this.completed,
			this.percentComplete,
			this.lastReportedDate
		});
	}
}
