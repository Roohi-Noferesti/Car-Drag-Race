using DataSerialization;
using System;

public class GoGetCarForTierPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		ShowroomScreen.ShowScreenWithTierCarList((eCarTier)(details.Tier - 1), false);
	}
}
