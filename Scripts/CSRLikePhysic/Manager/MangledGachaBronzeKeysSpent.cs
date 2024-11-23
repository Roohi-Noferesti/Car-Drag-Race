using System;

public class MangledGachaBronzeKeysSpent : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaBronzeKeysSpent;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaBronzeKeysSpentSetAndMangle = value;
		}
	}
}
