using KingKodeStudio;
using UnityEngine;

public class WorkshopCarDealCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
    private AgentCarDeal dealToShow;

	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
        if (GameDatabase.Instance.DealConfiguration == null)
        {
            return;
        }
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        CarDeal cardeal = GameDatabase.Instance.DealConfiguration.Car;
        BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(cardeal.MilestoneBeforeShowingFirstTier);
        if (tierEvents.CrewBattleEvents.NumEventsComplete() < cardeal.MilestoneBeforeShowingFirstCrew)
        {
            return;
        }
        if (RaceResultsTracker.You == null)
        {
            return;
        }
        if (!RaceResultsTracker.You.IsWinner)
        {
            return;
        }
        if (activeProfile.RacesSinceLastCarDeal > 0 && cardeal.TransitionDealsEnabled && AgentCarDeal.TryGetTransitionDeal(out this.dealToShow, false, false))
        {
            this.state = ConditionState.VALID;
            return;
        }
        if (!cardeal.RegularDealsEnabled)
        {
            return;
        }
        if (PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey ==
            PlayerProfileManager.Instance.ActiveProfile.LastAcquiredCar)// ||
        //FriendsRewardManager.Instance.ShouldShowNewCarRibbon)
        {
            return;
        }
        if (PopUpManager.Instance.WaitingToShowPopup || PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }
        int highestUnlockedClass = Mathf.Max(0, (int)RaceEventQuery.Instance.getHighestUnlockedClass());
        int minRacesCurrentTier = Mathf.Min(highestUnlockedClass, cardeal.MinRacesBeforeOffer.Count - 1);
        int maxRacesCurrentTier = Mathf.Min(highestUnlockedClass, cardeal.MaxRacesBeforeOffer.Count - 1);
        float minRacesBeforeOffer = (float)((cardeal.MinRacesBeforeOffer.Count <= 0) ? 0 : cardeal.MinRacesBeforeOffer[minRacesCurrentTier]);
        float maxRacesBeforeOffer = (float)((cardeal.MaxRacesBeforeOffer.Count <= 0) ? 100 : cardeal.MaxRacesBeforeOffer[maxRacesCurrentTier]);
        float num3 = ((float)activeProfile.RacesSinceLastCarDeal - minRacesBeforeOffer) / (maxRacesBeforeOffer - minRacesBeforeOffer);
        if (Random.value > num3)
        {
            return;
        }
        if (!AgentCarDeal.TryGetNextAvailableDeal(out this.dealToShow))
        {
            return;
        }
        this.state = ConditionState.VALID;
	}

	public override PopUp GetPopup()
	{
        return GetPopup(this.dealToShow, delegate
        {
            OnDealPopupOk(this.dealToShow);
        });
	}

    public static PopUp GetPopup(AgentCarDeal dealToShow, PopUpButtonAction okButtonAction)
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_AGENT_CAR_DEAL_TITLE",
			BodyText = dealToShow.GetWorkshopScreenPopupBodyText(),
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = okButtonAction,
			ConfirmText = "TEXT_BUTTON_SHOWROOM",
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

	public static void OnDealPopupOk(AgentCarDeal dealToShow)
	{
		ShowroomScreen.SetupShowRoomForDeal(dealToShow);
		dealToShow.OnPopupShown();
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
        ScreenManager.Instance.PushScreen(ScreenID.Showroom);
	}
}
