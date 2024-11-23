using System;

public class MangledGachaGoldKeysSpent : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaGoldKeysSpent;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaGoldKeysSpentSetAndMangle = value;
		}
	}
}
