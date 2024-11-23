using System;

public class MangledCashEarned : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return (int) PlayerProfileManager.Instance.ActiveProfile.CashEarned;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.CashEarnedSetAndMangle = value;
		}
	}
}
