using System;

public class MangledGoldEarned : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GoldEarned;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GoldEarnedSetAndMangle = (int)value;
		}
	}
}
