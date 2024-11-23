using System;

public class CachedFriendRaceDataMapper
{
	public static string ToJson(CachedFriendRaceData lump)
	{
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("C", CarTimesMapper.ToJson(lump.CarTimes));
		return jsonDict.ToString();
	}

	public static CachedFriendRaceData FromJson(JsonDict jsonDict)
	{
		CachedFriendRaceData cachedFriendRaceData = new CachedFriendRaceData();
		JsonDict carTimes = new JsonDict();
		jsonDict.TryGetValue("C", out carTimes);
		cachedFriendRaceData.CarTimes = CarTimesMapper.FromJson(carTimes);
		return cachedFriendRaceData;
	}
}
