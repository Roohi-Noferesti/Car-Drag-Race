using System;
using System.Collections.Generic;

public class CarTimesMapper
{
	public static Dictionary<string, float> FromJson(JsonDict CarTimes)
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		if (CarTimes == null)
		{
			return dictionary;
		}
		foreach (string current in CarTimes.Keys)
		{
			float value = 0f;
			if (CarTimes.TryGetValue(current, out value))
			{
				dictionary[current] = value;
			}
		}
		return dictionary;
	}

	public static JsonDict ToJson(Dictionary<string, float> times)
	{
		JsonDict jsonDict = new JsonDict();
		jsonDict.FloatFormatString = "{0:0.00000}";
		foreach (KeyValuePair<string, float> current in times)
		{
			jsonDict.Set(current.Key, current.Value);
		}
		return jsonDict;
	}
}
