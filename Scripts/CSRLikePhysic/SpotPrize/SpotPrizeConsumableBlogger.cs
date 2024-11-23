using System;
using I2.Loc;

public class SpotPrizeConsumableBlogger : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_BLOGGER_DESCRIPTION");
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		ConsumablesManager.SetupRaceTeamConsumablePrize(eCarConsumables.PRAgent, details.Duration);
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_CONSUMABLE_BODY");
		string arg = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_PRAGENT");
		return string.Format(format, arg, details.Duration);
	}
}
