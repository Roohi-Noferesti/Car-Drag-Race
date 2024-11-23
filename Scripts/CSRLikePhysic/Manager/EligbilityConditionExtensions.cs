using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using Objectives.Impl;

public static class EligbilityConditionExtensions
{
    private static readonly IDictionary<EligbilityConditionType, EligibilityConditionBase> conditionMapping = new Dictionary<EligbilityConditionType, EligibilityConditionBase>
	{
		{
			EligbilityConditionType.HasSeenSeasonUnlockableTheme,
			new HasSeenSeasonUnlockableThemeCondition()
		},
		{
			EligbilityConditionType.CarOwnedInTier,
			new CarOwnedInTierCondition()
		},
		{
			EligbilityConditionType.CarePackageRewardCount,
			new CarePackageRewardsReceivedCondition()
		},
		{
			EligbilityConditionType.CarePackageCount,
			new CarePackagesReceivedCountCondition()
		},
		{
			EligbilityConditionType.CashAmount,
			new CashAmountCondition()
		},
		{
			EligbilityConditionType.GoldAmount,
			new GoldAmountCondition()
		},
		{
			EligbilityConditionType.CrewEventsUncompletedInTier,
			new CrewEventsUncompletedInTier()
		},
		{
			EligbilityConditionType.ObjectiveComplete,
			new ObjectiveCompleteCondition()
		},
		{
			EligbilityConditionType.PlayedCurrentSeason,
			new PlayedCurrentSeasonCondition()
		},
		{
			EligbilityConditionType.StartGame,
			new AlwaysTrueCondition()
		},
		{
			EligbilityConditionType.TimeSinceMultiplayerRace,
			new TimeSinceMultiplayerRaceCondition()
		},
		{
			EligbilityConditionType.DoesNotHaveCashForTierCar,
			new DoesNotHaveCashForTierCarCondition()
		},
		{
			EligbilityConditionType.HasPaid,
			new HasPaidCondition()
		},
		{
			EligbilityConditionType.NotShowingEventTypeInSequence,
			new NotShowingEventTypeInSequenceCondition()
		},
		{
			EligbilityConditionType.WorldTourLastRaceInSequenceWasFinalPin,
			new WorldTourLastRaceInSequenceWasFinalPinCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceLevelRaced,
			new WorldTourSequenceLevelRacedCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceLevelWon,
			new WorldTourSequenceLevelWonCondition()
		},
		{
			EligbilityConditionType.WorldTourRacesWonSinceStateChange,
			new WorldTourRacesWonSinceStateChangeCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceLevel,
			new WorldTourSequenceLevelCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceComplete,
			new WorldTourSequenceCompleteCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceCompleteCount,
			new WorldTourSequenceCompleteCountCondition()
		},
		{
			EligbilityConditionType.WorldTourRaceResultCount,
			new WorldTourRaceResultCountCondition()
		},
		{
			EligbilityConditionType.WorldTourScheduledPinAge,
			new WorldTourScheduledPinAgeCondition()
		},
		{
			EligbilityConditionType.WorldTourRacesSincePinRaced,
			new WorldTourRacesSincePinRacedCondition()
		},
		{
			EligbilityConditionType.WorldTourThemeCompletionLevel,
			new WorldTourThemeCompletionLevelCondition()
		},
		{
			EligbilityConditionType.WorldTourThemeOption,
			new WorldTourThemeOptionCondition()
		},
		{
			EligbilityConditionType.WorldTourChoice,
			new WorldTourChoiceCondition()
		},
		{
			EligbilityConditionType.WorldTourBundlesEligibleCount,
			new WorldTourBundlesEligibleCount()
		},
		{
			EligbilityConditionType.RandomRangeCondition,
			new RandomRangeCondition()
		},
		{
			EligbilityConditionType.CanOfferSuperNitrous,
			new CanOfferSuperNitrousCondition()
		},
		{
			EligbilityConditionType.CurrentWorldTourSequence,
			new CurrentWorldTourSequenceCondition()
		},
		{
			EligbilityConditionType.IsCarOwned,
			new IsCarOwnedCondition()
		},
		{
			EligbilityConditionType.IsProCarOwned,
			new IsProCarOwnedCondition()
		},
		{
			EligbilityConditionType.IsMechanicActive,
			new IsMechanicActiveCondition()
		},
		{
			EligbilityConditionType.IsAvailableToBuyInShowroom,
			new IsAvailableToBuyInShowroomCondition()
		},
		{
			EligbilityConditionType.IsCurrentCar,
			new IsCurrentCarCondition()
		},
		{
			EligbilityConditionType.IsCurrentScreenInList,
			new IsCurrentScreenInListCondition()
		},
		{
			EligbilityConditionType.AreAnimationsCompleted,
			new AreAnimationsCompletedCondition()
		},
		{
			EligbilityConditionType.HasSeenPopupCount,
			new HasSeenPopupCountCondition()
		},
		{
			EligbilityConditionType.CareerEventsCompleteAny,
			new CareerEventsCompleteAnyCondition()
		},
		{
			EligbilityConditionType.CareerEventsCompleteCount,
			new CareerEventsCompleteCountCondition()
		},
		{
			EligbilityConditionType.CarOwnedCount,
			new CarOwnedCountCondition()
		},
		{
			EligbilityConditionType.CheckProfileProperty,
			new PlayerProfileBooleanCondition()
		},
        {
			EligbilityConditionType.CheckProfileIntegerProperty,
			new PlayerProfileIntegerCondition()
		},
		{
			EligbilityConditionType.RacesWonCount,
			new RacesWonCountCondition()
		},
		{
			EligbilityConditionType.EventComplete,
			new EventCompleteCondition()
		},
		{
			EligbilityConditionType.RacesEnteredCount,
			new RacesEnteredCountCondition()
		},
		{
			EligbilityConditionType.LadderRacesCompleteCount,
			new LadderRacesCompleteCountCondition()
		},
		{
			EligbilityConditionType.HasVisitedMechanicScreen,
			new HasVisitedMechanicScreenCondition()
		},
		{
			EligbilityConditionType.HasPlayedDailyBattles,
			new HasPlayedDailyBattlesCondition()
		},
		{
			EligbilityConditionType.IsLoggedIntoFacebook,
			new IsLoggedIntoFacebookCondition()
		},
		{
			EligbilityConditionType.FacebookUserPermissionStatus,
			new FacebookUserPermissionStatus()
		},
		{
			EligbilityConditionType.AlwaysTrue,
			new AlwaysTrueCondition()
		},
		{
			EligbilityConditionType.CurrentGameMode,
			new CurrentGameModeCondition()
		},
		{
			EligbilityConditionType.FriendsCarHasWonStar,
			new FriendsCarHasWonStarCondition()
		},
		{
			EligbilityConditionType.HasWonLastRace,
			new HasWonLastRaceCondition()
		},
		{
			EligbilityConditionType.IsWaitingForRYFReward,
			new IsWaitingForRYFRewardCondition()
		},
		{
			EligbilityConditionType.RYFStarTypeCount,
			new RYFStarTypeCountCondition()
		},
		{
			EligbilityConditionType.RYFStarTotalCount,
			new RYFStarTotalCountCondition()
		},
		{
			EligbilityConditionType.RYFRacesEnteredCount,
			new RYFRacesEnteredCountCondition()
		},
		{
			EligbilityConditionType.RYFRacesWonCount,
			new RYFRacesWonCountCondition()
		},
		{
			EligbilityConditionType.RYFRacesLoseCount,
			new RYFRacesLoseCountCondition()
		},
		{
			EligbilityConditionType.IsMultiplayerUnlocked,
			new IsMultiplayerUnlockedCondition()
		},
		{
			EligbilityConditionType.HasWonMultiplayerOnlineRace,
			new HasWonMultiplayerOnlineRaceCondition()
		},
		{
			EligbilityConditionType.HighestUnlockedClass,
			new HighestUnlockedClassCondition()
		},
		{
			EligbilityConditionType.MinHighestUnlockedClass,
			new MinHighestUnlockedClassCondition()
		},
		{
			EligbilityConditionType.TierBossChallengeFinished,
			new TierBossChallengeFinishedCondition()
		},
		{
			EligbilityConditionType.CompletedEventsPerTier,
			new CompletedEventsPerTierCondition()
		},
		{
			EligbilityConditionType.CompletedRestrictedEventsPerTier,
			new CompletedRestrictedEventsPerTierCondition()
		},
		{
			EligbilityConditionType.CompletedCarSpecificEventsPerTier,
			new CompletedCarSpecificEventsPerTierCondition()
		},
		{
			EligbilityConditionType.CompletedManufacturerSpecificEventsPerTier,
			new CompletedManufacturerSpecificEventsPerTierCondition()
		},
		{
			EligbilityConditionType.IsWorldTourAvailable,
			new AlwaysTrueCondition()
		},
		{
			EligbilityConditionType.IsWorldTourUnlocked,
			new IsWorldTourUnlockedCondition()
		},
		{
			EligbilityConditionType.IsCertainDeviceTime,
			new IsCertainDeviceTimeCondition()
		},
		{
			EligbilityConditionType.IsCertainServerTime,
			new IsCertainServerTimeCondition()
		},
		{
			EligbilityConditionType.IsCurrentSeason,
			new IsCurrentSeasonCondition()
		},
		{
			EligbilityConditionType.IsServerTimeValid,
			new IsServerTimeValidCondition()
		},
		{
			EligbilityConditionType.AlwaysFalse,
			new AlwaysFalseCondition()
		},
		{
			EligbilityConditionType.CurrentMapPaneSelected,
			new CurrentMapPaneSelectedCondition()
		},
		{
			EligbilityConditionType.CurrentWorldTourTheme,
			new CurrentWorldTourThemeCondition()
		},
		{
			EligbilityConditionType.PreviousWorldTourTheme,
			new PreviousWorldTourThemeCondition()
		},
		{
			EligbilityConditionType.TimePastSincePopUpSeen,
			new TimePastSincePopupSeenCondition()
		},
		{
			EligbilityConditionType.NumberOfCarsInListOwned,
			new NumberOfCarsInListOwned()
		},
		{
			EligbilityConditionType.TimePastSinceLastBundleOffer,
			new TimePastSinceLastBundleOfferCondition()
		},
		{
			EligbilityConditionType.HasSeenTutorialBubbleCount,
			new HasSeenTutorialBubbleCountCondition()
		},
		{
			EligbilityConditionType.HasDismissedTutorialBubble,
			new HasDismissedTutorialBubbleCondition()
		},
		{
			EligbilityConditionType.IsRelayPlayerCarFullyUpgraded,
			new IsRelayPlayerCarFullyUpgradedCondition()
		},
		{
			EligbilityConditionType.CurrentRelayRaceDifficulty,
			new CurrentRelayRaceDifficultyCondition()
		},
		{
			EligbilityConditionType.CurrentRelayRaceTimeDifference,
			new CurrentRelayRaceTimeDifferenceCondition()
		},
		{
			EligbilityConditionType.RacesDoneInCurrentRelayRace,
			new RacesDoneInCurrentRelayRaceCondition()
		},
		{
			EligbilityConditionType.VersusScreenMode,
			new VersusScreenModeCondition()
		},
		{
			EligbilityConditionType.IsCurrentCarFullyUpgraded,
			new IsCurrentCarFullyUpgradedCondition()
		},
		{
			EligbilityConditionType.AppStore,
			new AppStoreCondition()
		},
		{
			EligbilityConditionType.CurrentScreenAlreadyOnStack,
			new CurrentScreenAlreadyOnStackCondition()
		},
		{
			EligbilityConditionType.HasJustFinishedWorldTourSequence,
			new HasJustFinishedWorldTourSequenceCondition()
		},
		{
			EligbilityConditionType.WorldTourLastSequenceWon,
			new WorldTourLastSequenceWonCondition()
		},
		{
			EligbilityConditionType.WorldTourSequenceWonLastShownEvent,
			new WorldTourSequenceWonLastShownEventCondition()
		},
		{
			EligbilityConditionType.WorldTourSeenCount,
			new WorldTourSeenCountCondition()
		},
		{
			EligbilityConditionType.CurrentCarUsesEvoParts,
			new CurrentCarUsesEvoPartsCondition()
		},
		{
			EligbilityConditionType.HasCurrentCarGotUnspentEvoTokens,
			new HasCurrentCarGotUnspentEvoTokensCondition()
		},
		{
			EligbilityConditionType.IsMultiplayerEnabled,
			new IsMultiplayerEnabledCondition()
		},
		{
			EligbilityConditionType.DeviceOS,
			new DeviceOSCondition()
		},
		{
			EligbilityConditionType.IsDebugMenuActive,
			new AlwaysFalseCondition()
		},
        {
			EligbilityConditionType.CheckProfileIntegerGreaterOrEqualProperty,
			new PlayerProfileIntegerGreaterOrEqualCondition()
		},
        {
	        EligbilityConditionType.InsideCountry,
	        new InsideCountryCondition()
        },
        {
	        EligbilityConditionType.NotInsideCountry,
	        new NotInsideCountryCondition()
        },
        {
	        EligbilityConditionType.RemoteConfigCondition,
	        new RemoteConfigCondition()
        },
        {
	        EligbilityConditionType.TutorialConfigorationIsOn,
	        new TutorialConfigorationIsOnCondition()
        }
	};

	public static void Initialise(this EligibilityCondition ec)
	{
	    if (!string.IsNullOrEmpty(ec.Type))
	    {
	        var conditionType = EnumHelper.FromString<EligbilityConditionType>(ec.Type);
	        if (conditionMapping.ContainsKey(conditionType))
	        {
	            var type = conditionMapping.Keys.First((key) => key == conditionType);
	            ec.ConditionType = type;
	        }
        }
	    else
	    {
	        if (conditionMapping.ContainsKey(ec.ConditionType))
	        {
	            var type = conditionMapping.Keys.First((key) => key == ec.ConditionType);
	            ec.ConditionType = type;
	        }
        }
		ec.Details.Initialise();
	}

	public static bool IsValid(this EligibilityCondition ec, IGameState gameState)
	{
        if (conditionMapping.ContainsKey(ec.ConditionType))
		{
            EligibilityConditionBase eligibilityConditionBase = conditionMapping[ec.ConditionType];
			if (eligibilityConditionBase != null)
			{
				return eligibilityConditionBase.IsValid(gameState, ec.Details);
			}
		}
		return false;
	}
}