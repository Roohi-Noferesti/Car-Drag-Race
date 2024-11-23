using System;
using System.Collections.Generic;

public class FriendsRewardCollectedMapper
{
	public static JsonList ToJson(HashSet<int> friendRewardCollectedForCars)
	{
		JsonList jsonList = new JsonList();
		foreach (int current in friendRewardCollectedForCars)
		{
			jsonList.Add(current);
		}
		return jsonList;
	}

	public static HashSet<int> FromJson(JsonList list)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (list == null)
		{
			return hashSet;
		}
		for (int i = 0; i < list.Count; i++)
		{
			hashSet.Add(list.GetInt(i));
		}
		return hashSet;
	}
}
