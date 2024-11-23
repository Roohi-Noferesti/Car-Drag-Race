using System;
using System.Collections.Generic;

public class RaceWithFriendConditionCombosMapper
{
	public static HashSet<KeyValuePair<int, string>> FromJson(JsonDict FriendCarCombos)
	{
		HashSet<KeyValuePair<int, string>> hashSet = new HashSet<KeyValuePair<int, string>>();
		if (FriendCarCombos == null)
		{
			return hashSet;
		}
		foreach (string current in FriendCarCombos.Keys)
		{
			string empty = string.Empty;
			if (FriendCarCombos.TryGetValue(current, out empty))
			{
				hashSet.Add(new KeyValuePair<int, string>(Convert.ToInt32(current), empty));
			}
		}
		return hashSet;
	}

	public static JsonDict ToJson(HashSet<KeyValuePair<int, string>> combos)
	{
		JsonDict jsonDict = new JsonDict();
		foreach (KeyValuePair<int, string> current in combos)
		{
			jsonDict.Set(current.Key.ToString(), current.Value);
		}
		return jsonDict;
	}
}
