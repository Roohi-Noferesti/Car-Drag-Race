using System;
using I2.Loc;

public class AwardFuelTankPrize : AwardPrizeBase
{
	public override void AwardPrize()
	{
		FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
	}

	public override string GetMetricsTypeString()
	{
		return "Fuel Tank";
	}

    public override string GetPrizeString()
    {
        return LocalizationManager.GetTranslation("TEXT_FILL_FUEL");
    }

    public override void TakePrizeAwayFromProfile()
	{
		PlayerProfileManager.Instance.ActiveProfile.NumberOfFuelRefillsRemaining--;
	}
}
