using System;
using KingKodeStudio;

public class GasTankIAPCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	private enum PopupState
	{
		UpgradeTank,
		Unlimited
	}

	public const int NUM_POPUP_STATES = 2;

	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if ((!iAPs.IsGasTankUpgradeAvailable() || activeProfile.HasUpgradedFuelTank) && (!iAPs.IsUnlimitedFuelAvailable || UnlimitedFuelManager.IsActive))
		{
			return;
		}
		if (activeProfile.FuelRefillsBoughtWithGold == 0)
		{
			return;
		}
		if (FuelManager.Instance.GetFuel() > iAPs.GetGasTankReminderLowFuelAmount())
		{
			return;
		}
		if (activeProfile.GasTankMessageNumberOfTimesShown >= iAPs.GetGasTankReminderRepeatCount())
		{
			return;
		}
		if (activeProfile.GasTankMessageDurationSinceLastShownInMinutes() < iAPs.GetGasTankReminderFrequencyInMinutes())
		{
			return;
		}
		this.state = ConditionState.VALID;
	}

	private void OnUnlimitedPopupShownOK()
	{
        ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
	    GetMoreFuelScreen.RedirectToUnlimited = true;
        PlayerProfileManager.Instance.ActiveProfile.DisplayedGasTankReminder();
	}

	private void OnExtendedPopupShownOK()
	{
        FuelUpgradeShopScreen.PushWithProduct(FuelUpgradeShopScreen.ProductType.UpgradeTank);
        PlayerProfileManager.Instance.ActiveProfile.DisplayedGasTankReminder();
	}

	private void OnPopupShownCancel()
	{
		PlayerProfileManager.Instance.ActiveProfile.DisplayedGasTankReminder();
	}

	public override PopUp GetPopup()
	{
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
	    bool upgradeAvailable = false;//iAPs.IsGasTankUpgradeAvailable() && !activeProfile.HasUpgradedFuelTank;
		bool unlimitedAvailable = iAPs.IsUnlimitedFuelAvailable && !UnlimitedFuelManager.IsActive;
		bool isUnlimitedPopup = activeProfile.GasTankReminderIDShown == 1;
		if (upgradeAvailable != unlimitedAvailable)
		{
			isUnlimitedPopup = unlimitedAvailable;
		}
		PopUp result;
		if (isUnlimitedPopup)
		{
			result = new PopUp
			{
				Title = "TEXT_UNLIMITED_GAS_IAP_POPUP_TITLE",
				BodyText = "TEXT_UNLIMITED_GAS_IAP_POPUP_BODY",
				ConfirmAction = new PopUpButtonAction(this.OnUnlimitedPopupShownOK),
				CancelAction = new PopUpButtonAction(this.OnPopupShownCancel),
				ConfirmText = "TEXT_BUTTON_SHOW_ME",
				CancelText = "TEXT_BUTTON_NO_THANKS",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
		}
		else
		{
			result = new PopUp
			{
				Title = "TEXT_GAS_IAP_POPUP_TITLE",
				BodyText = "TEXT_GAS_IAP_POPUP_BODY",
				ConfirmAction = new PopUpButtonAction(this.OnExtendedPopupShownOK),
				CancelAction = new PopUpButtonAction(this.OnPopupShownCancel),
				ConfirmText = "TEXT_BUTTON_SHOW_ME",
				CancelText = "TEXT_BUTTON_NO_THANKS",
                GraphicPath = PopUpManager.Instance.graphics_mechanicPrefab,
				ImageCaption = "TEXT_NAME_MECHANIC"
			};
		}
		return result;
	}

	public override bool HasBubbleMessage()
	{
		return false;
	}

	public override string GetBubbleMessage()
	{
		return string.Empty;
	}
}
