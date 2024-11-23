using System;
using KingKodeStudio;

public class RenewUnlimitedFuelConditional : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	private PlayerProfile Profile;

	public RenewUnlimitedFuelConditional()
	{
		this.Profile = PlayerProfileManager.Instance.ActiveProfile;
	}

	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
		if (this.Profile.HasSeenUnlimitedFuelRenewalPopup)
		{
			return;
		}
		TimeSpan timeRemaining = UnlimitedFuelManager.TimeRemaining;
		if (timeRemaining <= TimeSpan.Zero || (int)timeRemaining.TotalMinutes >= GameDatabase.Instance.IAPs.UnlimitedFuelRenewalReminderMinutes)
		{
			return;
		}
		this.state = ConditionState.VALID;
	}

	private void OnPopupShownOK()
	{
        ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
        GetMoreFuelScreen.RedirectToUnlimited = true;
        this.Profile.HasSeenUnlimitedFuelRenewalPopup = true;
	}

	private void OnPopupShownCancel()
	{
		this.Profile.HasSeenUnlimitedFuelRenewalPopup = true;
	}

	public override PopUp GetPopup()
	{
		return new PopUp
		{
			Title = "TEXT_UNLIMITED_GAS_RENEW_POPUP_TITLE",
			BodyText = "TEXT_UNLIMITED_GAS_RENEW_POPUP_BODY",
			ConfirmAction = new PopUpButtonAction(this.OnPopupShownOK),
			CancelAction = new PopUpButtonAction(this.OnPopupShownCancel),
			ConfirmText = "TEXT_BUTTON_SHOW_ME",
			CancelText = "TEXT_BUTTON_NO_THANKS",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
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
