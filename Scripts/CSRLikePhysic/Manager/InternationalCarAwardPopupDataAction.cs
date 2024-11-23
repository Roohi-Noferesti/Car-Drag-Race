using DataSerialization;
using System;
using System.Linq;
using KingKodeStudio;

public class InternationalCarAwardPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		string carID = details.StringValue;
		bool won = details.Won;
		if (!CarDatabase.Instance.GetAllCars((CarInfo c) => c.Key == carID).Any<CarInfo>())
		{
			return;
		}
		//InternationalCarAwardScreen.PrepareScreen(carID, won);
		ScreenManager.Instance.PushScreen(ScreenID.InternationalCarAward);
	}
}
