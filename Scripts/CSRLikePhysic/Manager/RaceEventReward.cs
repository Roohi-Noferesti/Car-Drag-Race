using System;

[Serializable]
public class RaceEventReward : RaceEventRewardBase
{
    //[HideInInspector]
	public int GoldPrize;

    //[HideInInspector]
    public int CashPrize;

    //[HideInInspector]
	public int MinCashPrize;

    //[HideInInspector]
	public int MaxCashPrize;

    //[HideInInspector]
    public int XPPrize = -1;

    public RaceStarReward RaceStarReward;

	public override int GetCashReward()
	{
		if (CashPrize!=0)
		{
			return this.CashPrize;
		}
        if (this.MinCashPrize == 0 && this.MaxCashPrize == 0)
        {
            return 0;
        }
        float pPIndexRatioWithinTierIgnoreTierBoundaries = CarPerformanceIndexCalculator.GetPPIndexRatioWithinTierIgnoreTierBoundaries(CarStatsCalculator.Instance.playerCarPhysicsSetup.NewPerformanceIndex, CarStatsCalculator.Instance.playerCarPhysicsSetup.BaseCarTier);
        return (int)(this.MinCashPrize * (1f - pPIndexRatioWithinTierIgnoreTierBoundaries) + (float)this.MaxCashPrize * pPIndexRatioWithinTierIgnoreTierBoundaries);
	}
}
