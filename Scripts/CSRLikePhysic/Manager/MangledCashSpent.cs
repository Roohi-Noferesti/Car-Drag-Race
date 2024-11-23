using System;

public class MangledCashSpent : MangledPlayerProfileInteger
{
	protected override int PlayerProfileDataItem
	{
		get
		{
			return (int) PlayerProfileManager.Instance.ActiveProfile.CashSpent;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.CashSpentSetAndMangle = value;
		}
	}
}
