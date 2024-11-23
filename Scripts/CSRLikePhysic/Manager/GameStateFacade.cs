using System;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using KingKodeStudio;
using UnityEngine;

public class GameStateFacade : IGameState
{
	private bool canOfferSuperNitrous;

	private string currentWorldTourSequenceID = string.Empty;

	public bool CanOfferSuperNitrous
	{
		get
		{
			return this.canOfferSuperNitrous;
		}
		set
		{
			this.canOfferSuperNitrous = value;
		}
	}

	public string CurrentWorldTourSequenceID
	{
		get
		{
			return this.currentWorldTourSequenceID;
		}
		set
		{
			this.currentWorldTourSequenceID = value;
		}
	}

	public List<CarGarageInstance> CarsOwned
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		}
	}

	public string CurrentWorldTourThemeID
	{
		get
		{
            return TierXManager.Instance != null ? TierXManager.Instance.CurrentThemeName : string.Empty;
		}
	}

	public string CurrentWorldTourThemeOption
	{
		get
		{
            return (!(TierXManager.Instance == null)) ? TierXManager.Instance.CurrentThemeOption : string.Empty;
		}
	}

	public string PreviousWorldTourThemeID
	{
		get
		{
            return (!(TierXManager.Instance == null)) ? TierXManager.Instance.PreviousThemeName : string.Empty;
		}
	}

	public bool CurrentScreenAlreadyOnStack
	{
		get
		{
            return !(ScreenManager.Instance == null) && ScreenManager.Instance.CurrentScreenAlreadyOnStack;
		}
	}

	public string DeviceOS
	{
		get
		{
			return BasePlatform.ActivePlatform.GetDeviceOS();
		}
	}

	public List<string> GetAllThemeIDs()
	{
        List<PinSchedulerData.PinSchedulerThemeData> allThemes = PlayerProfileManager.Instance.ActiveProfile.GetAllThemes();
        return (from q in allThemes
                where q.ThemeID != "TierX_Overview"
                select q.ThemeID).ToList<string>();
	}

    public int ReceivedCarePackageRewardCount(string carePackageID)
    {
        return GameDatabase.Instance.CarePackages.ReceivedRewardCount(carePackageID);
    }

    public int TotalReceivedCarePackageCount(string carePackageID)
    {
        return GameDatabase.Instance.CarePackages.TotalReceivedCount(carePackageID);
    }

    public int GetCurrentCash()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
	}

	public void SetPinToRaced(string themeID, ScheduledPin pin, bool won)
	{
        PlayerProfileManager.Instance.ActiveProfile.SetPinAsRacedInPinScheduleSequence(themeID, pin, won);
        ScheduledPin referrerPin = pin.ReferrerPin;
        if (referrerPin != null)
        {
            this.SetPinToRaced(themeID, referrerPin, won);
        }
        else
        {
            this.IncrementPinScheduleRacesComplete(themeID);
        }
        if (!string.IsNullOrEmpty(pin.CarToAwardID) && won)
        {
            CarGarageInstance carGarageInstance = new CarGarageInstance();
            var carinfo = CarDatabase.Instance.GetCar(pin.CarToAwardID);
            carGarageInstance.SetupNewGarageInstance(carinfo);
            CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
            foreach (KeyValuePair<eUpgradeType, CarUpgradeStatus> current in carUpgradeSetup.UpgradeStatus)
            {
                CarUpgradeStatus value = current.Value;
                value.levelFitted = pin.CarToAwardUpgradeLevel;
                value.levelOwned = pin.CarToAwardUpgradeLevel;
            }
            bool awardAsElite = !CarDataDefaults.NonEliteBossCars.Contains(pin.CarToAwardID);
            BoostNitrous.AwardBossCar(carGarageInstance, carUpgradeSetup, awardAsElite);
            PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = pin.CarToAwardID;
        }
        if (pin.Narrative != null && pin.Narrative.IsPostRaceSceneEligible(this))
        {
            string text = (!won) ? pin.Narrative.GetPostRaceLoseSceneID() : pin.Narrative.GetPostRaceWinSceneID();
            NarrativeScene narrativeScene;
            if (!string.IsNullOrEmpty(text) && TierXManager.Instance.GetNarrativeScene(text, out narrativeScene) && narrativeScene.CanTriggerHighStakesRace())
            {
                Dictionary<string, PlayerProfileData.DeferredNarrativeScene> worldTourDeferredNarrativeScenes = PlayerProfileManager.Instance.ActiveProfile.WorldTourDeferredNarrativeScenes;
                worldTourDeferredNarrativeScenes[TierXManager.Instance.CurrentThemeName] = new PlayerProfileData.DeferredNarrativeScene
                {
                    SceneID = text,
                    SequenceID = pin.ParentSequence.ID
                };
            }
        }
    }

	public int LastWonEventSequenceLevel(string themeID, string sequenceID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetLastWonLevelInPinScheduleSequence(themeID, sequenceID);
	}

	public int ChoiceSelection(string themeID, string sequenceID, string pinID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetChoiceSelection(themeID, sequenceID, pinID);
	}

	public void SetPinToShown(string themeID, ScheduledPin pin)
	{
		if (pin.ParentSequence != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.SetLastSeenLevelInPinScheduleSequence(themeID, pin);
			ScheduledPin referrerPin = pin.ReferrerPin;
			if (referrerPin != null)
			{
				this.SetPinToShown(themeID, referrerPin);
			}
		}
	}

	public int LastShownEventSequenceLevel(string themeID, string sequenceID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetLastSeenLevelInPinScheduleSequence(themeID, sequenceID);
	}

	public int GetPinScheduleRacesWonSinceStateChange(string themeID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetPinScheduleRacesWonSinceStateChange(themeID);
	}

	public void IncrementPinScheduleRacesComplete(string themeID)
	{
		PlayerProfileManager.Instance.ActiveProfile.IncrementPinScheduleRacesComplete(themeID);
	}

	public int GetWorldTourThemeSeenCount(string themeID)
	{
        return PlayerProfileManager.Instance.ActiveProfile.GetWorldTourThemeSeenCount(themeID);
	}

    public void IncrementWorldTourThemeSeenCount(string themeID)
	{
        PlayerProfileManager.Instance.ActiveProfile.IncrementWorldTourThemeSeenCount(themeID);
	}

    public ThemeCompletionLevel GetWorldTourThemeCompletionLevel(string themeID)
    {
        return PlayerProfileManager.Instance.ActiveProfile.GetWorldTourThemeCompletionLevel(themeID);
    }

    public void IncrementWorldTourThemeCompletionLevel(string themeID)
	{
        PlayerProfileManager.Instance.ActiveProfile.IncrementWorldTourThemeCompletionLevel(themeID);
	    var level = GetWorldTourThemeCompletionLevel(themeID);
	    GTDebug.Log(GTLogChannel.Other,"increase themeID " + level);
    }

    public void SetWorldTourThemeCompletionLevel(string themeID, ThemeCompletionLevel level)
    {
        PlayerProfileManager.Instance.ActiveProfile.SetWorldTourThemeCompletionLevel(themeID, level);
    }

    public int GetEventCountInSequenceFromProfile(string themeID, string sequenceID)
	{
        return PlayerProfileManager.Instance.ActiveProfile.GetEventCountInSequence(themeID, sequenceID);
	}

    public bool HasRacedSpecificPinSchedulerPin(string themeID, string sequenceID, string scheduledPinID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.HasRacedSpecificPinSchedulerPin(themeID, sequenceID, scheduledPinID);
	}

	public int LastRacedEventSequenceLevel(string themeID, string sequenceID)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetLastRacedLevelInPinScheduleSequence(themeID, sequenceID);
	}

	public bool IsWorldTourNonBossCrewMembersAllDefeated()
	{
        string[] eventIDsForAnimation = TierXManager.Instance.ThemeDescriptor.EventIDsForAnimation;
        string currentThemeName = TierXManager.Instance.CurrentThemeName;
        string[] array = eventIDsForAnimation;
        for (int i = 0; i < array.Length; i++)
        {
            string sequenceID = array[i];
            if (!this.IsSequenceComplete(currentThemeName, sequenceID))
            {
                return false;
            }
        }
        return true;
	}

	public bool HasSeenAllWorldTourCrewMemebersAnims(string themeID)
	{
        string[] eventIDsForAnimation = TierXManager.Instance.ThemeDescriptor.EventIDsForAnimation;
        for (int i = 0; i < eventIDsForAnimation.Length; i++)
        {
            string eventID = eventIDsForAnimation[i];
            if (!PlayerProfileManager.Instance.ActiveProfile.IsAnimationCompletedForWorldTourEventID(themeID, eventID))
            {
                return false;
            }
        }
		return true;
	}

	public bool ShouldPlayWorldTourFinalAnimation(string themeID, string themePrizeCar, string outroAnimFlag)
	{
        return !TierXManager.Instance.IsOverviewThemeActive() && this.HasSeenAllWorldTourCrewMemebersAnims(themeID) &&
               (!PlayerProfileManager.Instance.ActiveProfile.IsAnimationCompletedForWorldTourEventID(themeID, outroAnimFlag) ||
                !this.IsCarOwned(themePrizeCar));
	}

    public bool IsSequenceComplete(string themeID, string sequenceID)
	{
		int num = this.LastWonEventSequenceLevel(themeID, sequenceID);
		int num2 = this.GetEventCountInSequenceFromProfile(themeID, sequenceID) - 1;
		return num >= num2;
	}

	public bool IsPinWon(string themeID, ScheduledPin pin)
	{
		string iD = pin.ParentSequence.ID;
		int num = this.LastWonEventSequenceLevel(themeID, iD);
		return num >= pin.Level;
	}

	public string GetWorldTourLastSequenceRaced(string themeID)
	{
        return PlayerProfileManager.Instance.ActiveProfile.GetWorldTourLastSequenceRaced(themeID);
	}

    public int GetWorldTourRaceResultCount(string themeID, string sequenceID, string pinID, bool didWin)
    {
        return PlayerProfileManager.Instance.ActiveProfile.GetWorldTourRaceResultCount(themeID, sequenceID, pinID,
            didWin);
    }

    public void ResetLifeCount(string themeID, List<ScheduledPin> filteredPins)
	{
        PlayerProfileManager.Instance.ActiveProfile.ResetLifeCount(themeID, filteredPins);
	}

    public ScheduledPinLifetimeData GetPinLifetimeData(string themeID, string lifetimeGroup)
    {
        return PlayerProfileManager.Instance.ActiveProfile.GetPinLifetimeData(themeID, lifetimeGroup);
    }

    public ScheduledPin GetCurrentEventScheduledPin()
	{
        RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        if (currentEvent != null && currentEvent.IsWorldTourRace())
        {
            PinDetail worldTourPinPinDetail = currentEvent.GetWorldTourPinPinDetail();
            return worldTourPinPinDetail.WorldTourScheduledPinInfo;
        }
		return null;
	}

	public int GetCurrentSeasonNumber()
	{
        int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
        if (mostRecentActiveSeasonEventID == -1)
        {
            return -1;
        }
        SeasonEventMetadata @event = GameDatabase.Instance.SeasonEvents.GetEvent(mostRecentActiveSeasonEventID);
        return @event.SeasonDisplayNumber;
	}

    public bool IsMechanicActive()
	{
		return PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining > 0;
	}

	public bool IsProCarOwned(string carKey)
	{
		return PlayerProfileManager.Instance.ActiveProfile.IsProCarOwned(carKey);
	}

	public bool IsCarOwned(string carKey)
	{
		return PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(carKey) || ArrivalManager.Instance.isCarOnOrder(carKey);
	}

	public bool IsAvailableToBuyInShowroom(string carKey)
	{
		CarInfo car = CarDatabase.Instance.GetCar(carKey);
		return car.IsAvailableToBuyInShowroom();
	}

	public bool IsCurrentCar(string carKey)
	{
		return PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey == carKey;
	}

	public bool IsCurrentCarFullyUpgraded()
	{
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		return currentCar != null && currentCar.GetIsFullyUpgraded() && currentCar.GetIsFullyFitted();
	}

	public bool CurrentCarUsesEvoParts()
	{
		CarInfo car = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
		return car.UsesEvoUpgrades();
	}

	public int GetCurrentCarEvoPartsEarned(int upgradeLevel)
	{
		CarInfo car = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
		return car.EvolutionUpgradesEarned(upgradeLevel);
	}

	public int GetCurrentCarNumEvoPartsSpent()
	{
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		return currentCar.UpgradeStatus.Values.Sum((CarUpgradeStatus upg) => (int)upg.evoOwned);
	}

	private RaceEventData GetCurrentRelayEvent()
	{
		if (!RelayManager.IsCurrentEventRelay())
		{
			return null;
		}
		return RaceEventInfo.Instance.CurrentEvent;
	}

	public bool IsRelayCarFullyUpgraded(int raceIndex)
	{
		RaceEventData currentRelayEvent = this.GetCurrentRelayEvent();
		if (currentRelayEvent == null)
		{
			return false;
		}
		int count = currentRelayEvent.Group.RaceEvents.Count;
		if (raceIndex < 0 || raceIndex >= count)
		{
			return false;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		RaceEventData raceEventData = currentRelayEvent.Group.RaceEvents[raceIndex];
		CarGarageInstance carFromID = activeProfile.GetCarFromID(raceEventData.GetHumanCar());
		return carFromID != null && carFromID.GetIsFullyUpgraded() && carFromID.GetIsFullyFitted();
	}

	public float GetCurrentRelayRaceDifficulty()
	{
		RaceEventData currentRelayEvent = this.GetCurrentRelayEvent();
		if (currentRelayEvent == null)
		{
			return 0f;
		}
		int racesDone = RelayManager.GetRacesDone();
		float timeDifference = RelayManager.CalculateExpectedTimeDifference(currentRelayEvent, racesDone);
		return RelayManager.ConvertTimeDifferenceToPercentage(timeDifference);
	}

	public bool GetPlayerProfileBoolean(string property)
	{
		return this.GetPlayerProfileProperty<bool>(property, false);
	}

	public int GetPlayerProfileInteger(string property)
	{
		return this.GetPlayerProfileProperty<int>(property, 0);
	}

	public bool HasDismissedTutorialBubble(string name)
	{
		int bubbleID = TutorialBubble.HashName(name);
		return PlayerProfileManager.Instance.ActiveProfile.HasDismissedTutorialBubble(bubbleID);
	}

	public int GetTutorialBubbleSeenCount(string name)
	{
		int bubbleID = TutorialBubble.HashName(name);
		return PlayerProfileManager.Instance.ActiveProfile.GetTutorialBubbleSeenCount(bubbleID);
	}

	public DateTime GetPlayerProfileDate(string property)
	{
		return this.GetPlayerProfileProperty<DateTime>(property, DateTime.MinValue);
	}

	public string GetCurrentGameMode()
	{
        return CareerModeMapScreen.CurrentGameMode.ToString();
	}

    public string GetCurrentScreenID()
    {
        return ScreenManager.Instance.CurrentScreen.ToString();
    }

    public string GetVersusScreenMode()
	{
		return VSDummy.VSMode.ToString();
	}

	private T GetPlayerProfileProperty<T>(string property, T defaultValue)
	{
		T result = defaultValue;
		try
		{
			result = (T)((object)GetPropValue(PlayerProfileManager.Instance.ActiveProfile, property));
		}
		catch (Exception)// var_1_1D)
		{
		}
		return result;
	}

	private static object GetPropValue(object src, string propName)
	{
		return src.GetType().GetProperty(propName).GetValue(src, null);
	}

	public string AppStore()
	{
		return BasePlatform.ActivePlatform.GetTargetAppStore().ToString();
	}

	public bool HasSeenSeasonUnlockableTheme(string theme)
	{
		return PlayerProfileManager.Instance.ActiveProfile.HasSeenSeasonUnlockableTheme(theme);
	}

	public bool IsMultiplayerEnabled()
	{
        return GameDatabase.Instance.OnlineConfiguration.MultiplayerEnabled;
	}

    public bool IsDailyBattleUnlocked()
    {
        return false;
    }

    public int GetPlayerLevel()
    {
        if (PlayerProfileManager.Instance != null)
            PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel();
        return 0;
    }

    public bool IsEventCompleted(int eventID)
    {
        return PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(eventID);
    }
}
