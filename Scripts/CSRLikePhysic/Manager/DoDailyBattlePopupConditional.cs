using System;
using UnityEngine;

public class DoDailyBattlePopupConditional : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!activeProfile.DailyBattlePromptEnabled)
		{
			return;
		}
		if (FuelManager.Instance.GetFuel() == 0)
		{
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue)
		{
			return;
		}
		if (activeProfile.GetTimeUntilNextDailyBattle() >= TimeSpan.Zero)
		{
			return;
		}
        //Debug.Log(activeProfile.SessionRacesCompleted);
        if (activeProfile.SessionRacesCompleted < DailyBattleRewardManager.Instance.SessionRacesReminderThreshold)
        {
            return;
        }
		if (activeProfile.SessionDailyBattleRaces > 0)
		{
			return;
		}
        //var careerModeMapScreen = ScreenManager.Active.ActiveScreen as CareerModeMapScreen;
        //if (careerModeMapScreen == null) // && MapScreen.carTierSelected >= eCarTier.TIER_1 && MapScreen.carTierSelected <= eCarTier.TIER_5)
        //{
        //    return;
        //}
        EventPin eventPinMatchingCondition = CareerModeMapScreen.Instance.EventSelect.GetEventPinMatchingCondition((EventPin e) => e.EventData.IsDailyBattle());
        if (eventPinMatchingCondition == null)
        {
            return;
        }
		this.state = ConditionState.VALID;
	}

	public void OnPopupShown()
	{
		PlayerProfileManager.Instance.ActiveProfile.DisableDailyBattlePrompt();
	}

	public override PopUp GetPopup()
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_DAILY_BATTLES_TITLE",
			BodyText = "TEXT_DAILYBATTLEPROMPT_BODY",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.OnPopupShown),
			ConfirmAction = delegate
			{
				this.OnPopupShown();
			    CareerModeMapScreen careerModeMapScreen = CareerModeMapScreen.Instance;
                if (careerModeMapScreen != null)
                {
                    EventPin eventPinMatchingCondition = careerModeMapScreen.EventSelect.GetEventPinMatchingCondition((EventPin e) => e.EventData.IsDailyBattle());
                    if (eventPinMatchingCondition != null)
                    {
                        careerModeMapScreen.OnEventSelected(eventPinMatchingCondition.EventData, false);
                        //careerModeMapScreen.OnEventStart(eventPinMatchingCondition.EventData);
                    }
                }
			},
			CancelText = "TEXT_BUTTON_LATER",
			ConfirmText = "TEXT_BUTTON_RACE",
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
