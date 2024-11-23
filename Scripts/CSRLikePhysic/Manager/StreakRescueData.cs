using System;
using System.Collections.Generic;

[Serializable]
public class StreakRescueData
{
	public bool IsActive;

	public bool UseBloggerDiscount;

	public List<StreakRescueCostData> RaceRescueCost = new List<StreakRescueCostData>();
}
