using DataSerialization;
using System;
using System.Collections.Generic;

public class GoGetSpecificCarPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		ShowroomScreen.ShowScreenWithPresetCarList(new List<CarInfo>
		{
			CarDatabase.Instance.GetCar(details.StringValue)
		});
	}
}
