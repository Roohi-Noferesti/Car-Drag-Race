using System.Collections.Generic;
using DataSerialization;

public static class PopupDataButtonActionExtensions
{
	public enum PopupDataButtonActionType
	{
        OPEN_SCREEN,
		NEW_PLAYER,
        TEST_YOUR_CAR,
        TEST_UPGRADE,
		SEND_METRIC_EVENT,
		BUY_FIRST_UPGRADE,
		FACEBOOK_SIGNIN_OK,
		FACEBOOK_SIGNIN_CANCEL,
		FACEBOOK_USER_PERMISSION_REQUEST,
		GO_GET_CAR,
		INTRO_FRIENDS,
		INTRO_FRIENDS_FIRST_STAR,
		INTRO_FRIENDS_SECOND_RACE,
		INTRO_FRIENDS_COMPLETE,
		ENTER_TIER_2,
		GO_GET_CAR_FOR_TIER,
		GO_GET_SPECIFIC_CAR,
		ENTER_TIER_3,
		ENTER_TIER_4,
		ENTER_TIER_5,
		INTRO_WORLD_TOUR,
		FIRE_WORLD_TOUR_TUTORIAL_METRIC_EVENT,
		INCREASE_WORLD_TOUR_THEME_COMPLETION_LEVEL,
		REFRESH_WORLD_TOUR_THEME,
		REFRESH_CURRENT_SCREEN,
		BUY_STARTER_PACK,
		REJECT_STARTER_PACK,
		DEBUG_WELCOME_DISMISS,
		BUY_BUNDLE_OFFER,
		REJECT_BUNDLE_OFFER,
		RERATE_APP,
		OPEN_URL,
		INTERNATIONAL_CAR_AWARD,
        FOCUS_ON_MAP,
        DISABLE_PIN_EXCEPT,
	}

	private static readonly Dictionary<PopupDataButtonActionType, PopupDataActionBase> actionMapping = new Dictionary<PopupDataButtonActionType, PopupDataActionBase>
	{
        {
			PopupDataButtonActionType.OPEN_SCREEN,
			new GoToScreenPopupDataAction()
		},
		{
			PopupDataButtonActionType.NEW_PLAYER,
			new NewPlayerPopupDataAction()
		},
	    {
			PopupDataButtonActionType.TEST_YOUR_CAR,
            new TestYourCarPopupDataAction()
	    },
        {
			PopupDataButtonActionType.TEST_UPGRADE,
            new TestYourUpgradeDataAction()
	    },
		{
			PopupDataButtonActionType.SEND_METRIC_EVENT,
			new SendMetricEventPopupDataAction()
		},
		{
			PopupDataButtonActionType.BUY_FIRST_UPGRADE,
			new BuyFirstUpgradePopupDataAction()
		},
		{
			PopupDataButtonActionType.FACEBOOK_SIGNIN_OK,
			null//new FacebookSigninOkPopupDataAction()
		},
		{
			PopupDataButtonActionType.FACEBOOK_SIGNIN_CANCEL,
			null//new FacebookSigninCancelPopupDataAction()
		},
		{
			PopupDataButtonActionType.FACEBOOK_USER_PERMISSION_REQUEST,
			null//new FacebookUserPermissionRequest()
		},
		{
			PopupDataButtonActionType.GO_GET_CAR,
			new GoGetCarForCrewRacesPopupDataAction()
		},
		{
			PopupDataButtonActionType.INTRO_FRIENDS,
			null//new IntroFriendsPopupDataAction()
		},
		{
			PopupDataButtonActionType.INTRO_FRIENDS_FIRST_STAR,
			null//new IntroFriendsFirstStarPopupDataAction()
		},
		{
			PopupDataButtonActionType.INTRO_FRIENDS_COMPLETE,
			null//new IntroFriendsCompletePopupDataAction()
		},
		{
			PopupDataButtonActionType.INTRO_FRIENDS_SECOND_RACE,
			null//new IntroFriendsSecondRacePopupDataAction()
		},
		{
			PopupDataButtonActionType.ENTER_TIER_2,
			new EnterTierPopupDataAction(eCarTier.TIER_2)
		},
		{
			PopupDataButtonActionType.GO_GET_CAR_FOR_TIER,
			new GoGetCarForTierPopupDataAction()
		},
		{
			PopupDataButtonActionType.GO_GET_SPECIFIC_CAR,
			new GoGetSpecificCarPopupDataAction()
		},
		{
			PopupDataButtonActionType.ENTER_TIER_3,
			new EnterTierPopupDataAction(eCarTier.TIER_3)
		},
		{
			PopupDataButtonActionType.ENTER_TIER_4,
			new EnterTierPopupDataAction(eCarTier.TIER_4)
		},
		{
			PopupDataButtonActionType.ENTER_TIER_5,
			new EnterTierPopupDataAction(eCarTier.TIER_5)
		},
		{
			PopupDataButtonActionType.INTRO_WORLD_TOUR,
			new IntroWorldTourPopupDataAction()
		},
		{
			PopupDataButtonActionType.FIRE_WORLD_TOUR_TUTORIAL_METRIC_EVENT,
			new FireWTTutorialMetricsPopupDataAction()
		},
		{
			PopupDataButtonActionType.INCREASE_WORLD_TOUR_THEME_COMPLETION_LEVEL,
			new IncreaseWTThemeCompletionLevelPopupDataAction()
		},
		{
			PopupDataButtonActionType.REFRESH_WORLD_TOUR_THEME,
			new RefreshWorldTourThemePopupdataAction()
		},
		{
			PopupDataButtonActionType.REFRESH_CURRENT_SCREEN,
			new RefreshCurrentScreenPopupDataAction()
		},
		{
			PopupDataButtonActionType.DEBUG_WELCOME_DISMISS,
			new DebugWelcomeDismissAction()
		},
		{
			PopupDataButtonActionType.BUY_BUNDLE_OFFER,
			new BuyBundleOfferPopupDataAction()
		},
		{
			PopupDataButtonActionType.REJECT_BUNDLE_OFFER,
			new RejectBundleOfferDataAction()
		},
		{
			PopupDataButtonActionType.RERATE_APP,
			new ReRateAppPopupDataAction()
		},
		{
			PopupDataButtonActionType.OPEN_URL,
			new OpenURLPopupDataAction()
		},
		{
			PopupDataButtonActionType.INTERNATIONAL_CAR_AWARD,
			new InternationalCarAwardPopupDataAction()
		},
	    {
	        PopupDataButtonActionType.FOCUS_ON_MAP,
            new FocusOnEventPopupDataAction()
	    },
        {
	        PopupDataButtonActionType.DISABLE_PIN_EXCEPT,
            new DisableAllMapPinEventPopupDataAction()
	    }
	};

	public static void Initialise(this PopupDataButtonAction pdba)
	{
        pdba.Details.Initialise();
	}

	public static void Execute(this PopupDataButtonAction pdba)
	{
		if (actionMapping.ContainsKey(pdba.ActionType))
		{
			PopupDataActionBase popupDataActionBase = actionMapping[pdba.ActionType];
			if (popupDataActionBase != null)
			{
				popupDataActionBase.Execute(pdba.Details);
			}
		}
	}

    public static void Execute(this PopupDataButtonActionType pdbat)
    {
        if (actionMapping.ContainsKey(pdbat))
        {
            actionMapping[pdbat].Execute(null);
        }
    }
}
