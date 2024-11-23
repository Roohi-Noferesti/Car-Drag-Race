using System;
using KingKodeStudio;

public class AllHardRacesTutorialConditional : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
        //if (CareerModeMapScreen.mapPaneSelected == -2)
        //{
        //    return;
        //}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile.RacesEntered < 2)
		{
			return;
		}
		if (activeProfile.HasCompletedFirstCrewRace())
		{
			return;
		}
		if (FlowConditionBase.IsTooFrequent(activeProfile.AllHardRacesConditionLastTimeShown, GameDatabase.Instance.TutorialConfiguration.AllHardRacesConditionalFrequency))
		{
			return;
		}
	    var careerModeMapScreen = CareerModeMapScreen.Instance;
        if (careerModeMapScreen != null)
		{
            EventPin eventPinMatchingCondition = careerModeMapScreen.EventSelect.GetEventPinMatchingCondition(delegate(EventPin eventPin)
            {
                RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(eventPin.EventData, false);
                bool flag = rating == RaceEventDifficulty.Rating.Easy || rating == RaceEventDifficulty.Rating.Challenging;
                return flag & (!eventPin.EventData.IsFriendRaceEvent() && eventPin.GetState == EventPin.eState.Normal);
            });
            if (eventPinMatchingCondition != null)
            {
                return;
            }
            if (careerModeMapScreen.EventSelect.HighlightedPin == careerModeMapScreen.EventSelect.GetRRPin())
            {
                return;
            }
		}
		this.state = ConditionState.VALID;
	}

	public void OnPopupShownOK()
	{
		PlayerProfileManager.Instance.ActiveProfile.AllHardRacesConditionLastTimeShown = GTDateTime.Now;
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
	}

	public void OnPopupShownCancel()
	{
	}

	public override PopUp GetPopup()
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_HARD_RACES_TITLE",
			BodyText = "TEXT_POPUPS_HARD_RACES_BODY",
			ConfirmAction = new PopUpButtonAction(this.OnPopupShownOK),
			ConfirmText = "TEXT_BUTTON_OK",
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
