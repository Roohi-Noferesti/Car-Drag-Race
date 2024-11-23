using System;

public class MangledGachaGoldKeysEarned : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaGoldKeysEarned;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaGoldKeysEarnedSetAndMangle = value;
		}
	}
}
