using System;
using System.Collections.Generic;
using System.Diagnostics;

public class WorldTourBoostNitrous
{
	private class NitrousInfo
	{
		public int EventID = -1;

		public string PinID = string.Empty;

		public BossChallengeStateEnum State;
	}

	private Dictionary<string, WorldTourBoostNitrous.NitrousInfo> m_SuperNitrousStatus = new Dictionary<string, WorldTourBoostNitrous.NitrousInfo>();

	public static void ReadFromJson(JsonDict jsonDict, ref WorldTourBoostNitrous result)
	{
		result = new WorldTourBoostNitrous();
		result.ReadJson(jsonDict);
	}

	private void ReadJson(JsonDict jsonDict)
	{
		this.m_SuperNitrousStatus.Clear();
		foreach (string current in jsonDict.Keys)
		{
			JsonDict jsonDict2 = jsonDict.GetJsonDict(current);
			WorldTourBoostNitrous.NitrousInfo nitrousInfo = new WorldTourBoostNitrous.NitrousInfo();
			nitrousInfo.EventID = jsonDict2.GetInt("e");
			nitrousInfo.PinID = jsonDict2.GetString("p");
			nitrousInfo.State = (BossChallengeStateEnum)jsonDict2.GetInt("s");
			this.m_SuperNitrousStatus.Add(this.GetKey(nitrousInfo.EventID, nitrousInfo.PinID), nitrousInfo);
		}
	}

	public static void WriteToJson(WorldTourBoostNitrous record, ref JsonDict jsonDict)
	{
		foreach (KeyValuePair<string, WorldTourBoostNitrous.NitrousInfo> current in record.m_SuperNitrousStatus)
		{
			JsonDict jsonDict2 = new JsonDict();
			jsonDict2.Set("e", current.Value.EventID);
			jsonDict2.Set("p", current.Value.PinID);
			jsonDict2.Set("s", (int)current.Value.State);
			jsonDict.Set(current.Key, jsonDict2);
		}
	}

	private string GetKey(int eventID, string pinID)
	{
		return eventID.ToString() + ":" + pinID;
	}

	public void SetRaceBegun(int eventID, string pinID)
	{
		string key = this.GetKey(eventID, pinID);
		if (this.m_SuperNitrousStatus.ContainsKey(key))
		{
			return;
		}
		WorldTourBoostNitrous.NitrousInfo value = new WorldTourBoostNitrous.NitrousInfo
		{
			EventID = eventID,
			PinID = pinID
		};
		this.m_SuperNitrousStatus.Add(key, value);
	}

	public void SetRaceInProgress(int eventID, string pinID)
	{
		string key = this.GetKey(eventID, pinID);
		if (!this.m_SuperNitrousStatus.ContainsKey(key))
		{
			return;
		}
		this.m_SuperNitrousStatus[key].State = BossChallengeStateEnum.INPROGRESS;
	}

	public void SetRaceFinished(int eventID, string pinID)
	{
		string key = this.GetKey(eventID, pinID);
		if (!this.m_SuperNitrousStatus.ContainsKey(key))
		{
			return;
		}
		this.m_SuperNitrousStatus.Remove(key);
	}

	public IEnumerable<KeyValuePair<int, string>> ChallengeIDsToShow(BossChallengeStateEnum state)
	{
	    bool flag = false;
	    var enumerator = m_SuperNitrousStatus.GetEnumerator();
	    try
	    {
	        while (enumerator.MoveNext())
	        {
	            var current = enumerator.Current;
	            if (current.Value.State == state)
	            {
	                yield return new KeyValuePair<int, string>(current.Value.EventID, current.Value.PinID);
	                flag = true;
	            }
	        }
	    }
	    finally
	    {
	        if (!flag)
	        {
	        }
	        enumerator.Dispose();
	    }
    }
}
