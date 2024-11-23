using System;
using System.Collections.Generic;

public class GameCenterPlayer
{
	public string playerId;

	public string alias;

	public bool isFriend;

	public GameCenterPlayer(JsonDict jd)
	{
		this.playerId = jd.GetString("playerId");
		this.alias = jd.GetString("alias");
		this.isFriend = jd.GetBool("isFriend");
	}

	public static List<GameCenterPlayer> fromJSON(string json)
	{
		List<GameCenterPlayer> list = new List<GameCenterPlayer>();
		JsonList jsonList = new JsonList();
		if (!jsonList.Read(json))
		{
			return list;
		}
		for (int i = 0; i < jsonList.Count; i++)
		{
			GameCenterPlayer item = new GameCenterPlayer(jsonList.GetJsonDict(i));
			list.Add(item);
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Player> playerId: {0}, alias: {1}, isFriend: {2}", this.playerId, this.alias, this.isFriend);
	}
}
