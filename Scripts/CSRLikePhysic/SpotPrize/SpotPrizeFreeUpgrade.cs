using System;
using I2.Loc;

public class SpotPrizeFreeUpgrade : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		if (details.Quantity == 1)
		{
			return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_FREE_UPGRADE_DESCRIPTION");
		}
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_FREE_UPGRADES_DESCRIPTION");
		return string.Format(format, details.Quantity);
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		PlayerProfileManager.Instance.ActiveProfile.AddFreeUpgrade(details.Quantity);
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		if (details.Quantity == 1)
		{
			return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_FREE_UPGRADE_BODY");
		}
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_FREE_UPGRADES_BODY");
		return string.Format(format, details.Quantity);
	}
}
