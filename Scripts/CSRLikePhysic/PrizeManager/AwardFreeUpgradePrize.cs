using System;
using I2.Loc;

public class AwardFreeUpgradePrize : AwardPrizeBase
{
	public override void AwardPrize()
	{
		PlayerProfileManager.Instance.ActiveProfile.AddFreeUpgrade(1);
	}

	public override string GetMetricsTypeString()
	{
		return "Free Upgrade";
	}

    public override string GetPrizeString()
    {
        return LocalizationManager.GetTranslation("TEXT_FREE_UPGRADE");
    }

    public override void TakePrizeAwayFromProfile()
	{
		PlayerProfileManager.Instance.ActiveProfile.NumberOfUpgradeRewardsRemaining--;
	}
}
