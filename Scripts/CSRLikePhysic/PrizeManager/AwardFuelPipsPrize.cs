using System;
using I2.Loc;

public class AwardFuelPipsPrize : AwardPrizeBase
{
	private int numOfFuelPipsToAward;

	public AwardFuelPipsPrize()
	{
		this.numOfFuelPipsToAward = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.PipsOfFuelReward;
	}

	public override void AwardPrize()
	{
		FuelManager.Instance.AddFuel(this.numOfFuelPipsToAward, FuelReplenishTimeUpdateAction.UPDATE, FuelAnimationLockAction.OBEY);
	}

	public override string GetMetricsTypeString()
	{
		return "Fuel Pips";
	}

    public override string GetPrizeString()
    {
        return string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_PIPS"), numOfFuelPipsToAward);
    }

    public override int GetMetricsFuelPipsToAward()
	{
		return this.numOfFuelPipsToAward;
	}

	public override void TakePrizeAwayFromProfile()
	{
		PlayerProfileManager.Instance.ActiveProfile.NumberOfFuelPipsRewardsRemaining--;
	}
}
