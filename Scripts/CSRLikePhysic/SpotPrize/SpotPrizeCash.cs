using System;
using I2.Loc;

public class SpotPrizeCash : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return CurrencyUtils.GetCashString(details.Quantity);
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		PlayerProfileManager.Instance.ActiveProfile.AddCash(details.Quantity,"reward","SpotPrize");
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CASH_BODY");
		return string.Format(format, CurrencyUtils.GetColouredCashString(details.Quantity));
	}
}
