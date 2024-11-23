using System;

[Serializable]
public class FriendsRaceEventReward : RaceEventRewardBase
{
	public eCarTier Tier;

	public StarType Star;

	public int CashPrize;

	public int FirstTimeAchievedCashPrize;

	public override int GetCashReward()
	{
		return this.CashPrize;
	}
}
