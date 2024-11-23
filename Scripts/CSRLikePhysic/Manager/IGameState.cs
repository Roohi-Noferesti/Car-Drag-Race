using System;
using System.Collections.Generic;
using DataSerialization;

public interface IGameState
{
    bool CanOfferSuperNitrous
    {
        get;
    }

    string CurrentWorldTourSequenceID
    {
        get;
    }

    string CurrentWorldTourThemeID
    {
        get;
    }

    string CurrentWorldTourThemeOption
    {
        get;
    }

    string PreviousWorldTourThemeID
    {
        get;
    }

    bool CurrentScreenAlreadyOnStack
    {
        get;
    }

    string DeviceOS
    {
        get;
    }

    List<string> GetAllThemeIDs();

    int ReceivedCarePackageRewardCount(string carePackageID);

    int TotalReceivedCarePackageCount(string carePackageID);

    int GetCurrentCash();

    void SetPinToRaced(string themeID, ScheduledPin pin, bool won);

    int LastWonEventSequenceLevel(string themeID, string sequenceID);

    int ChoiceSelection(string themeID, string sequenceID, string pinID);

    void SetPinToShown(string themeID, ScheduledPin pin);

    int LastShownEventSequenceLevel(string themeID, string sequenceID);

    int GetPinScheduleRacesWonSinceStateChange(string themeID);

    void IncrementPinScheduleRacesComplete(string themeID);

    int GetEventCountInSequenceFromProfile(string themeID, string sequenceID);

    bool IsSequenceComplete(string themeID, string id);

    bool IsPinWon(string themeID, ScheduledPin pin);

    string GetWorldTourLastSequenceRaced(string themeID);

    int GetWorldTourRaceResultCount(string themeID, string sequenceID, string pinID, bool didWin);

    bool HasRacedSpecificPinSchedulerPin(string themeID, string sequenceID, string scheduledPinID);

    int LastRacedEventSequenceLevel(string themeID, string sequenceID);

    int GetWorldTourThemeSeenCount(string themeID);

    void IncrementWorldTourThemeSeenCount(string themeID);

    ThemeCompletionLevel GetWorldTourThemeCompletionLevel(string themeID);

    void IncrementWorldTourThemeCompletionLevel(string themeID);

    void SetWorldTourThemeCompletionLevel(string themeID, ThemeCompletionLevel level);

    bool IsWorldTourNonBossCrewMembersAllDefeated();

    bool HasSeenAllWorldTourCrewMemebersAnims(string themeID);

    bool ShouldPlayWorldTourFinalAnimation(string themeID, string themePrizeCar, string outroAnimFlag);

    void ResetLifeCount(string themeID, List<ScheduledPin> filteredPins);

    ScheduledPinLifetimeData GetPinLifetimeData(string themeID, string lifetimeGroup);

    ScheduledPin GetCurrentEventScheduledPin();

    bool IsMechanicActive();

    bool IsCarOwned(string carKey);

    bool IsProCarOwned(string carKey);

    bool IsAvailableToBuyInShowroom(string carKey);

    bool IsCurrentCar(string carKey);

    bool IsCurrentCarFullyUpgraded();

    bool CurrentCarUsesEvoParts();

    int GetCurrentCarEvoPartsEarned(int upgradeLevel);

    int GetCurrentCarNumEvoPartsSpent();

    bool HasDismissedTutorialBubble(string name);

    int GetTutorialBubbleSeenCount(string name);

    bool HasSeenSeasonUnlockableTheme(string theme);

    bool IsRelayCarFullyUpgraded(int raceIndex);

    float GetCurrentRelayRaceDifficulty();

    bool GetPlayerProfileBoolean(string property);

    int GetPlayerProfileInteger(string property);

    DateTime GetPlayerProfileDate(string property);

    string GetCurrentGameMode();

    string GetCurrentScreenID();

    string GetVersusScreenMode();

    int GetCurrentSeasonNumber();

    string AppStore();

    bool IsMultiplayerEnabled();
    bool IsDailyBattleUnlocked();
    int GetPlayerLevel();
    bool IsEventCompleted(int unlockEventID);
}
