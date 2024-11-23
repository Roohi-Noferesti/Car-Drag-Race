using System;

public class MangledGachaSilverKeysSpent : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaSilverKeysSpent;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaSilverKeysSpentSetAndMangle = value;
		}
	}
}
