using System;
using I2.Loc;

public class SpotPrizeGold : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return CurrencyUtils.GetGoldString(details.Quantity);
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		PlayerProfileManager.Instance.ActiveProfile.AddGold(details.Quantity,"reward","SpotPrize");
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_GOLD_BODY");
		return string.Format(format, CurrencyUtils.GetColouredGoldString(details.Quantity));
	}
}
