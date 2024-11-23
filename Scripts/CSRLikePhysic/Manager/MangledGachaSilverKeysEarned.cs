using System;

public class MangledGachaSilverKeysEarned : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GachaSilverKeysEarned;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GachaSilverKeysEarnedSetAndMangle = value;
		}
	}
}
