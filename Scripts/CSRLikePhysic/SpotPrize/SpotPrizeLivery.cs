using System;
using I2.Loc;
using KingKodeStudio;

public class SpotPrizeLivery : SpotPrize
{
	public override string GetPinDescription(SpotPrizeDetails details)
	{
		return LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_LIVERY_DESCRIPTION");
	}

	public override void AwardPrize(SpotPrizeDetails details)
	{
        //LiveryAwardScreen.PrepareScreen(details.Car, details.Livery);
		ScreenManager.Instance.PushScreen(ScreenID.LiveryAward);
	}

	public override string GetPopupBody(SpotPrizeDetails details)
	{
		string format = LocalizationManager.GetTranslation("TEXT_SPOT_PRIZE_LIVERY_BODY");
		string arg = string.Empty;
		CarInfo car = CarDatabase.Instance.GetCar(details.Car);
		if (car != null)
		{
			arg = LocalizationManager.GetTranslation(car.ShortName);
		}
		return string.Format(format, arg);
	}
}
