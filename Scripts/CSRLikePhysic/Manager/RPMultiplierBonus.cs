using System;
using System.Collections.Generic;

[Serializable]
public class RPMultiplierBonus
{
	public class BoostScreenSettings
	{
		public string GroupDescription = string.Empty;

		public bool HiddenUnlessActive;
	}

	public RP_BONUS_TYPE Type;

	public string StartTime = string.Empty;

	public string EndTime = string.Empty;

	public string Name = string.Empty;

	public string CarDBKey = string.Empty;

	public float Bonus;

	public List<float> PerCarBonuses;

	public List<int> CumulativeNumCars;

	public RPMultiplierBonus.BoostScreenSettings RPBoostScreenSettings = new RPMultiplierBonus.BoostScreenSettings();
}
