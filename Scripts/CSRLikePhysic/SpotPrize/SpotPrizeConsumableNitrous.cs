using System;
using I2.Loc;

public class SpotPrizeConsumableNitrous : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_NITROUS_DESCRIPTION");
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		ConsumablesManager.SetupRaceTeamConsumablePrize(eCarConsumables.Nitrous, details.Duration);
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_BODY");
		string arg = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_NITROUS");
		return string.Format(format, arg, details.Duration);
	}
}
