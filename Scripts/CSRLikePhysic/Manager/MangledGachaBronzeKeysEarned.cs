using System;

public class MangledGachaBronzeKeysEarned : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaBronzeKeysEarned;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaBronzeKeysEarnedSetAndMangle = value;
		}
	}
}
