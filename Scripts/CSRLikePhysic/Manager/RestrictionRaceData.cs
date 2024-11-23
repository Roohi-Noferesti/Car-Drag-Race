using System;
using System.Collections.Generic;

public class RestrictionRaceData
{
	private Dictionary<eCarTier, int> lastShownRestrictions = new Dictionary<eCarTier, int>();

	public RestrictionRaceData()
	{
		for (int num = 0; num != 6; num++)
		{
			this.lastShownRestrictions.Add((eCarTier)num, 0);
		}
	}

	public void SetLastShownEventID(eCarTier tier, int eventID)
	{
		this.lastShownRestrictions[tier] = eventID;
	}

	public int GetLastShownEventID(eCarTier tier)
	{
		return this.lastShownRestrictions[tier];
	}

	public void ToJson(JsonDict dict)
	{
		JsonDict jsonDict = new JsonDict();
		foreach (KeyValuePair<eCarTier, int> current in this.lastShownRestrictions)
		{
			jsonDict.Set(current.Key.ToString(), current.Value);
		}
		dict.Set("lsrs", jsonDict);
	}

	public void FromJson(JsonDict dict)
	{
		JsonDict jsonDict = dict.GetJsonDict("lsrs");
		if (jsonDict != null)
		{
			for (int num = 0; num != 6; num++)
			{
				eCarTier eCarTier = (eCarTier)num;
				int eventID;
				if (jsonDict.TryGetValue(eCarTier.ToString(), out eventID))
				{
					this.SetLastShownEventID(eCarTier, eventID);
				}
			}
		}
	}
}
