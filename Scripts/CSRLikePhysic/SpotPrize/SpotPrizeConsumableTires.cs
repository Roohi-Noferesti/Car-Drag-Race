using System;
using I2.Loc;

public class SpotPrizeConsumableTires : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_TIRES_DESCRIPTION");
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		ConsumablesManager.SetupRaceTeamConsumablePrize(eCarConsumables.Tyre, details.Duration);
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_BODY");
		string arg = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_TYRES");
		return string.Format(format, arg, details.Duration);
	}
}
