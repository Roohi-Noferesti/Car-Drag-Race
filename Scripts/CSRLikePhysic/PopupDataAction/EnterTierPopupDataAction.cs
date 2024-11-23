using DataSerialization;
using System;
using KingKodeStudio;

public class EnterTierPopupDataAction : PopupDataActionBase
{
	private eCarTier Tier;

	public EnterTierPopupDataAction(eCarTier tier)
	{
		this.Tier = tier;
	}

	public override void Execute(EligibilityConditionDetails details)
	{
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen == null)
		{
			return;
		}
		careerModeMapScreen.OnTierSelected(this.Tier);
	}
}
