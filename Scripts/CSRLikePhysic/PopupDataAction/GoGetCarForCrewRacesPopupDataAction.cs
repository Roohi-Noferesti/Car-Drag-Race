using DataSerialization;
using Metrics;
using System;
using KingKodeStudio;

public class GoGetCarForCrewRacesPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		Log.AnEvent(Events.YouShouldBuyBetterCar);
		//ShowroomScreen.Init.CurrentManufacturer = "id_mini";
		//ShowroomScreen.Init.LastSelectedManufacturer  = "id_mini";
		ShowroomScreen.Init.LastSelectedModelIndex = 3;//2 is index 0 because 2 dummy item is exists before actual items
        var cars = CarDatabase.Instance.GetAllCars(c => c.Key == "car_toyota_corolla" || c.Key == "car_toyota_yaris");
        ShowroomScreen.Init.PresetCarList = cars;

        ShowroomScreen.Init.screenMode = ShowroomMode.Preset_List;
		ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Showroom, new ScreenID[]
		{
			ScreenID.CarSelect
		});
	}
}
