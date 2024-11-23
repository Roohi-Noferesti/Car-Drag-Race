using System;

public class CashForTierCarCarePackageRewardCalculation : CarePackageRewardCalculationBase
{
	private eCarTier tier;

	public CashForTierCarCarePackageRewardCalculation(eCarTier tier)
	{
		this.tier = tier;
	}

	public override void PerformCalculation(CarePackageRewardDetails details)
	{
		details.Cash += CarDatabase.Instance.GetLowestCashPriceInTier(this.tier);
	}
}
