using System;

public class SubtractCashBalanceCarePackageRewardCalculation : CarePackageRewardCalculationBase
{
	public override void PerformCalculation(CarePackageRewardDetails details)
	{
		details.Cash -= PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
	}
}
