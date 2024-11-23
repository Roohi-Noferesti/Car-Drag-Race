using System;

public class MangledGoldSpent : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.GoldSpent;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.GoldSpentSetAndMangle = (int)value;
		}
	}
}
