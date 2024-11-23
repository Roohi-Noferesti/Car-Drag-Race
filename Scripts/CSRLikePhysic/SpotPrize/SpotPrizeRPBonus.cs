using System;
using I2.Loc;

public class SpotPrizeRPBonus : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_RPBONUS_DESCRIPTION");
		return string.Format(format, details.FloatQuantity * 100f, details.Duration);
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
		DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
		//DateTime inFinish = dateTime.AddMinutes((double)details.Duration);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.AddMultiplayerEventRPBonus(details.EventID, details.SpotPrizeID, dateTime);
        //RPBonusManager.AddBonus(new RPBonusMultiplayerEvent(details.FloatQuantity, new RPBonusWindow(dateTime, inFinish)));
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_RPBONUS_BODY");
		return string.Format(format, (int)(details.FloatQuantity * 100f), details.Duration);
	}
}
