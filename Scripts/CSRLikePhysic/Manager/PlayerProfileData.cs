using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerProfileData : ISerializationCallbackReceiver
{
    public class ConsumableInfo
    {
        public DateTime Expires;

        public DateTime ExpiresSync;

        public int RacesLeft;

        public int AntiCheatRacesLeft;

        public int CmlMinutesPurchased;
    }

    public struct UnlimitedFuelInfo
    {
        public DateTime Expires;

        public DateTime ExpiresSync;

        public int CmlMinutesPurchased;

        public bool HasSeenRenewalPopup;

        public int RaceTeamPopupTimesSeen;
    }

    public struct DeferredNarrativeScene
    {
        public string SceneID;

        public string SequenceID;
    }



    public const int UnsetSeasonValue = -1;

    public CarePackageData CarePackageData = new CarePackageData();

    public OfferWallData OfferWallData = new OfferWallData();

    public PinSchedulerData PinSchedulerData = new PinSchedulerData();

    public RestrictionRaceData RestrictionRaceData = new RestrictionRaceData();

    public CarDealsData CarDeals = new CarDealsData();

    public BundleOfferProfileData BundleOfferData = new BundleOfferProfileData();

    public string ProductVersionLastSaved;

    public string DisplayName = string.Empty;

    public string Username;

    public bool NewProfile;

    public DateTime dateTimeLastSaved;

    public float BestOverallQuarterMileTime;

    public float BestOverallHalfMileTime;

    public float BestReactionTime;

    public float TotalDistanceTravelled;

    public int TotalPlayTime;

    public int TotalGarageTime;

    public long CashSpent;

    public long CashEarned;

    public long CashBought;

    public int IAPCashSpent;

    public int GoldEarned;

    public int GoldSpent;

    public int GoldBought;

    public int IAPGoldSpent;

    public int GachaTokensEarned;

    public int GachaTokensSpent;

    public int GachaBronzeKeysEarned;

    public int GachaBronzeKeysSpent;

    public int IAPGachaBronzeKeysSpent;

    public int GachaSilverKeysEarned;

    public int GachaSilverKeysSpent;

    public int IAPGachaSilverKeysSpent;

    public int GachaGoldKeysEarned;

    public int GachaGoldKeysSpent;

    public int IAPGachaGoldKeysSpent;

    public int PlayerXP;

    public int PlayerLevel = 1;

    public LeagueData.LeagueName PlayerLeague;

    public int PlayerRP;

    public int WorldRank;

    public int PreviousWorldRank;

    public int FuelPips = 10;

    public int GoldFuelRefills;

    public DateTime LastFuelAutoReplenishedTime = GTDateTime.Now;

    public int LastLegalAgreementVersion;

    public int NumMPCardPrizesWon;

    public int FreeUpgradesLeft;

    public DateTime LastAdvertDate;

    public int AdCount;

    public int BoostNitrousUsed;

    public int PreferredCsrAvatarPic = 0; //UnityEngine.Random.Range(1, 15);

    public BossChallengeStateEnum BossChallengeStateT1;

    public BossChallengeStateEnum BossChallengeStateT2;

    public BossChallengeStateEnum BossChallengeStateT3;

    public BossChallengeStateEnum BossChallengeStateT4;

    public BossChallengeStateEnum BossChallengeStateT5;

    public int DailyBattlesDoneToday;

    public DateTime DailyBattlesLastEventAt;

    public int DailyBattlesConsecutiveDaysCount;

    public bool DailyBattlesWonLast = true;

    public bool OptionMusicMute;

    public bool OptionSoundMute;

    public bool HasLikedOnFacebook;

    public bool HasFollowedUsOnTwitter;

    public bool OptionNotifications = true;

    public bool HasSeenLowMemoryLanguageMessage;

    public bool HasSeenUnlockCarScreen;

    public GarageSortOrder OptionGarageSortOrder = GarageSortOrder.Alphabetic;

    public bool OptionReverseSortOrder = true;

    public int RacesEntered;

    public int TutorialRacesAttempted;

    public int MechanicFettledRaces;

    public int RacesWon;

    public int RacesLost;

    public int OnlineRacesWon;

    public int ConsecutiveRacesWonAtLowDifficulty;

    public int ConsecutiveOnlineWins;

    public int OnlineRacesLost;

    public DateTime LastOnlineRace;

    public int OnlineRacesWonToday;

    public int OnlineRacesLostToday;

    public int FriendsRacesWon;

    public int FriendsRacesLost;

    public int FriendsCarsWon;

    public int FriendsGold;

    public int FriendsInvited;

    public int InitialNetworkFriends;

    public int NumTweets;

    public int NumFBPosts;

    public int EventsCompletedTier1;

    public int EventsCompletedTier2;

    public int EventsCompletedTier3;

    public int EventsCompletedTier4;

    public int EventsCompletedTier5;

    public string Tier1CarSpecificEventTarget = "None";

    public string Tier2CarSpecificEventTarget = "None";

    public string Tier3CarSpecificEventTarget = "None";

    public string Tier4CarSpecificEventTarget = "None";

    public string Tier5CarSpecificEventTarget = "None";

    //This is for holding wich car specific group player is already on
    public int Tier1CarSpecificEventTargetIndex = 0;

    public int Tier2CarSpecificEventTargetIndex = 0;

    public int Tier3CarSpecificEventTargetIndex = 0;

    public int Tier4CarSpecificEventTargetIndex = 0;

    public int Tier5CarSpecificEventTargetIndex = 0;

    public string Tier1ManufacturerSpecificEventTarget = "None";

    public string Tier2ManufacturerSpecificEventTarget = "None";

    public string Tier3ManufacturerSpecificEventTarget = "None";

    public string Tier4ManufacturerSpecificEventTarget = "None";

    public string Tier5ManufacturerSpecificEventTarget = "None";

    public bool HaveWarnedAboutNitrousUpgrade;

    public bool HasSignedIntoFacebookBefore;

    public bool HasSignedIntoGameCentreBefore;

    public bool HasSignedIntoGooglePlayGamesBefore;

    public int MechanicTuningRacesRemaining;

    public bool HaveShownFirstMechanicPopUp;

    public bool FirstRace1;
    
    public bool FirstRace2;
    
    public bool FirstRace3;
    
    public bool FirstRace32;
    
    public bool FirstRace4;
    
    public bool FirstRace41;
    
    public bool FirstRace42;

    public int CumulativeSessions;

    public int SessionsCounter;

    public bool InRollingRoadSession;

    public int ContiguousLosses;

    public int ContiguousProgressionLosses;

    public int ContiguousProgressionLossesTriggered;

    public bool HasBoughtFirstCar;

    public bool HasBoughtFirstUpgrade;

    public bool HasBoughtFirstProperty;

    public bool HasChoosePlayerName;
    public bool HasSeenRealRacePopup;

    public DateTime LastAgentNag;

    public DateTime LastPlayedMultiplayer;

    public DateTime LastPlayedEliteClub;

    public DateTime LastPlayedRaceTheWorldWorldTour;

    public bool DifficultyNeedsReset;

    public float MultiplayerDifficulty;

    public float EliteMultiplayerDifficulty;

    public float MultiplayerEventDifficulty;

    public int GPGSignInCancellations;

    public DateTime UserStartedPlaying;

    public DateTime UserStartedLastSession;

    public string CurrentlySelectedCarDBKey;

    public List<CarGarageInstance> CarsOwned = new List<CarGarageInstance>();

    public List<MultiplayerCarPrize> CarsWonInMultiplayer = new List<MultiplayerCarPrize>();

    public List<string> NewCars = new List<string>();

    public List<int> EventsCompleted = new List<int>();

    public List<int> LegacyObjectivesCompleted = new List<int>();

    public List<string> ObjectivesCompleted = new List<string>();

    public List<Arrival> arrivalQueue = new List<Arrival>();

    public List<SocialGamePlatformSelector.AchievementData> playerAchievements = new List<SocialGamePlatformSelector.AchievementData>();

    public List<SocialGamePlatformSelector.ScoreData> playerScores = new List<SocialGamePlatformSelector.ScoreData>();

    public bool[] CrewProgressionIntroductionPlayed = new bool[6];

    public bool HasAttemptedFirstCrew;

    public int LastServerMessageDisplayedID = -1;

    public int LastServerMessageDisplayedCount;

    public int FacebookInviteFuelRewardsCount;

    public DateTime FacebookInviteFuelRewardsTime;

    public string FacebookID = string.Empty;

    public int TwitterInviteFuelRewardsCount;

    public DateTime TwitterInviteFuelRewardsTime;

    public int TwitterCashRewardsCount;

    public DateTime TwitterCashRewardsTime;

    public bool IsFacebookSSORewardAllowed = true;

    public bool DoneRateAppTriggerBuyCar;

    public bool DoneRateAppTriggerCrewMember;

    public float BestTwitterNagTimeQtr = 100f;

    public DateTime LastUpgradeDateTimeNag = GTDateTime.Now;

    public bool HasHadMechanicSlowMotionIntroduction;

    public bool HasSeenGarageIntro;

    public bool HasSeenNitrousTutorial;

    public bool ShouldShowSkipTo2ndCrewMemberPopup;

    public bool CanTrySecondFacebookNag = true;

    public int RacesEnteredAtLastFacebookNag;

    public DateTime UTCLastClockChange;

    public List<string> TransactionHistory = new List<string>();

    public int LastBoughtCarRacesEntered;

    public float MultiplayerDCBDifficulty;

    public int MultiplayerDCBConsecutiveWins;

    public int MultiplayerDCBConsecutiveLoses;

    public int NumberOfRelaysCompetetedIn = 1;

    public Dictionary<eCarConsumables, ConsumableInfo> Consumables = new Dictionary<eCarConsumables, ConsumableInfo>
    {
        {
            eCarConsumables.EngineTune,
            new ConsumableInfo()
        },
        {
            eCarConsumables.PRAgent,
            new ConsumableInfo()
        },
        {
            eCarConsumables.Nitrous,
            new ConsumableInfo()
        },
        {
            eCarConsumables.Tyre,
            new ConsumableInfo()
        },
        {
            eCarConsumables.WholeTeam,
            new ConsumableInfo()
        }
    };

    public UnlimitedFuelInfo UnlimitedFuel;

    public int SeasonLastPlayedLeaderboardID = -1;

    public int SeasonLastPlayedEventID = -1;

    public bool DoneUpgradeWarningOnNewTier;

    public int BestEverMultiplayerWinStreak;

    public int BestEverMultiplayerWinStreakBanked;

    public int TotalMultiplayerStreaksCompleted;

    public int TotalMultiplayerStreaksLost;

    public int SMPWins;

    public int SMPWinsLastSession;

    public int SMPLosses;

    public int SMPConsecutiveRaces;

    public int SMPConsecutiveLoses;

    public int SMPConsecutiveWins;

    public int SMPChallengeWins;

    public bool IsSMPWinChallengeAvailable;

    public DateTime SMPWinChallengeActivationTime;

    public DateTime SMPLastLobbyBotDisclaimerTime;

    public long LastWinStreakExtendedTime;

    public int SMPTotalRaces;

    public DateTime SMPStartSessionDate;

    public int SMPTotalRacesLastSession;

    public string SMPWinStreakID;

    public int NumberOfPrizeCardRemaining;

    public int NumberOfStargazerMoments;

    public int NumberOfSportsCarPiecesRemaining;

    public int NumberOfDesiribleCarPiecesRemaining;

    public int NumberOfCommonCarPiecesRemaining;

    public int NumberOfTinyCashRewardsRemaining;

    public int NumberOfSmallCashRewardsRemaining;

    public int NumberOfMediumCashRewardsRemaining;

    public int NumberOfLargeCashRewardsRemaining;

    public int NumberOfHugeCashRewardsRemaining;

    public int NumberOfTinyGoldRewardsRemaining;

    public int NumberOfSmallGoldRewardsRemaining;

    public int NumberOfMediumGoldRewardsRemaining;

    public int NumberOfLargeGoldRewardsRemaining;

    public int NumberOfHugeGoldRewardsRemaining;

    public int NumberOfTinyKeyRewardsRemaining;

    public int NumberOfSmallKeyRewardsRemaining;

    public int NumberOfMediumKeyRewardsRemaining;

    public int NumberOfLargeKeyRewardsRemaining;

    public int NumberOfHugeKeyRewardsRemaining;

    public int NumberOfUpgradeRewardsRemaining;

    public int NumberOfFuelRefillsRemaining;

    public int NumberOfFuelPipsRewardsRemaining;

    public int NumberOfTinyRPRewardsRemaining;

    public int NumberOfSmallRPRewardsRemaining;

    public int NumberOfMediumRPRewardsRemaining;

    public int NumberOfLargeRPRewardsRemaining;

    public int NumberOfHugeRPRewardsRemaining;

    public int NumberOfProTunerRewardsRemaining;

    public int NumberOfN20ManiacRewardsRemaining;

    public int NumberOfTireCrewRewardsRemaining;

    public bool MultiplayerTutorial_SuccessfullyCompletedStreak;

    public bool MultiplayerTutorial_HasSeenModeSelectScreen;

    public bool MultiplayerTutorial_HasSeenAnyEvent;

    public bool MultiplayerTutorial_VersusRaceTeamCompleted;

    public bool MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted;

    public bool MultiplayerTutorial_TracksideUpgradesCompleted;

    public bool MultiplayerTutorial_FirstPrizeCompleted;

    public bool MultiplayerTutorial_CardsButNoCarPartCompleted;

    public bool MultiplayerTutorial_EliteClubCompleted;

    public bool MultiplayerTutorial_LostWinStreakCompleted;

    public bool MultiplayerTutorial_MapRaceTeamCompleted;

    public bool MultiplayerTutorial_FirstCarPartCompleted;

    public bool MultiplayerTutorial_PrizeScreenCompleted;

    public bool MultiplayerTutorial_HasSeenRespectScreen;

    public bool MultiplayerTutorial_ElitePassCompleted;

    public bool MultiplayerTutorial_EliteCarCompleted;

    public bool Multiplayer_HasSeenRPBonusPopup;

    public bool FriendTutorial_HasSupressedTutorial;

    public bool FriendTutorial_HasSeenFriendsPane;

    public int MultiplayerConsumableUpsellCount;

    public int MultiplayerConsumableMapUpsellCount;

    public int MultiplayerConsumableMapUpsellStreaksLost;

    public bool SharingTutorial_VideoShareCompleted;

    public int WelcomeMessageId;

    public List<SeasonPrizeIdentifier> SeasonPrizesAwarded = new List<SeasonPrizeIdentifier>();

    public Dictionary<string, float> BestCarTimes = new Dictionary<string, float>();

    public HashSet<int> FriendsRewardCollectedForCars = new HashSet<int>();

    public string UDID = string.Empty;

    public string UUID = string.Empty;

    public List<DateTime> VideoForFuelTimestamps = new List<DateTime>();

    public Dictionary<int, int> KnownFriendsPrizes = new Dictionary<int, int>();

    public int PeakFriends;

    public List<string> SyncdServiceID = new List<string>();

    public int HighestCrossPromotionTierAdvertSeen;

    public bool FirstTimeRYFUser = true;

    public bool TwoStarTimeConditionShown;

    public bool ThreeStarTimeConditionShown;

    public int LastTierUnlockConditionShown;

    public bool HasSeenWonAllCarsConditionPopup;

    public HashSet<KeyValuePair<int, string>> RaceWithFriendConditionCombos = new HashSet<KeyValuePair<int, string>>();

    public int BeatYourBestConditionShwonCount;

    public int InviteFriendsConditionShownCount;

    public int RaceTooHardConditionShownCount;

    public DateTime DailyPrizeCardLastEventAt = GTDateTime.Now;
    
    public DateTime AppTuttiTimedRewardLastEventAt = DateTime.MinValue;
    
    public DateTime VasTimedRewardLastEventAt = DateTime.MinValue;

    public DateTime InviteFriendsConditionLastTimeShown;

    public DateTime BuyACarLastConditionLastTimeShown;

    public DateTime RaceWithFriendConditionLastTimeShown;

    public DateTime FriendsEasyRaceConditionLastTimeShown;

    public bool HasSeenRenewWholeTeamIAPCondition;

    public DateTime AllHardRacesConditionLastTimeShown;

    public bool HasUpgradedFuelTank;

    public bool HasReceivedFuelTankUpgradeRefill;

    public DateTime GasTankReminderLastTimeShown;

    public int GasTankReminderNumberOfTimesShown;

    public int GasTankReminderIDShown;

    public int RacesStartedThisSession;

    public List<DailyBattleCompletionRecord> UnconfirmedDailyBattleResults = new List<DailyBattleCompletionRecord>();

    public int DailyBattleGoldConfirmed;

    public int DailyBattleCashConfirmed;

    public int DailyBattleRewardsRemoved;

    public List<bool> FuelAdPromptToggles = new List<bool>();

    public bool HasSeenWorldTourIntroduction;

    public int SeenWorldTourLockedIntroCount;

    public bool OverrideRYFObjectives;

    public bool OverrideRTWObjectives;

    public List<WorldTourAnimationsCompleted> WorldTourEventIDsWithAnimitionCompleted = new List<WorldTourAnimationsCompleted>();

    public List<string> SeasonUnlockableThemesSeen = new List<string>();

    public bool DailyBattlePromptToggle = true;

    public WorldTourBoostNitrous WTBoostNitrousStatus;

    public Dictionary<string, DeferredNarrativeScene> WTDeferredNarrativeScenes =
        new Dictionary<string, DeferredNarrativeScene>();

    public bool HasVisitedMechanicScreen;

    public bool HasVisitedManufacturerScreen;

    public bool HasAwardInstagramVisit;

    public bool HasAwardTelegramVisit;

    public bool HasOfferedCrewRacesCars;

    public bool HasFinishedRYFTutorial;

    public bool HasSeenFacebookNag;

    public bool HasSeenMultiplayerIntroScreen;

    public bool HasSeenInternationalIntroScreen;

    public int LastPlayedMultiplayerLeaderboardID;

    public PopupsProfileData PopupsData = new PopupsProfileData();

    public int MapPaneSelected;

    public Dictionary<string, string> TierXPinSelections = new Dictionary<string, string>();

    public DateTime LastBundleOfferTimeShown;

    public TutorialBubblesProfileData TutorialBubblesData = new TutorialBubblesProfileData();

    public int WinCountAfterUpgrade;
    public int PlayerStar;
    public int PlayerLeagueStar;
    public DateTime LastEndOfWeekCheck;
    public string ImageUrl;
    public string AvatarID;
    public List<string> ObjectivesCollected = new List<string>();
    public Dictionary<string, JsonDict> ActiveObjectives = new Dictionary<string, JsonDict>();
    public DateTime ObjectiveEndTime;
    public DateTime PreviousWeeklyLeaderboardCheck;
    public bool HasWeeklyRewardToClaim;
    public MultiplayerEventsProfileData MultiplayerEventsData = new MultiplayerEventsProfileData();
    public string CurrentUserTimeZone;
    public string CurrentUserChosenLanguage;
    public bool UseMileAsUnit;

    public bool IsFirstBodyUpgrade;
    public bool IsFirstEngineUpgrade;
    public bool IsFirstIntakeUpgrade;
    public bool IsFirstNitrousUpgrade;
    public bool IsFirstTransmissionUpgrade;
    public bool IsFirstTurboUpgrade;
    public bool IsFirstTyresUpgrade;
    public bool IsFirstTapToUpgrade;

    public string DefaultBranch = string.Empty;
    public string A_Branch = string.Empty;
    public string B_Branch = string.Empty;

    public bool IsDefaultBranch;
    public bool IsA_Branch;
    public bool IsB_Branch;

    

    #region Serialization Helper Variable
    [SerializeField]
    private string m_dailyBattleLastEventAt;
    [SerializeField]
    private string m_previousWeeklyLeaderboardCheck;
    [SerializeField]
    private string m_sMPWinChallengeActivationTime;
    [SerializeField]
    private string m_lastBundleOfferTimeShown;
    [SerializeField] 
    private string m_dailyPrizeCardLastEventAt;
    [SerializeField] 
    private string m_appTuttiTimedRewardLastEventAt ;
    [SerializeField] 
    private string m_vasTimedRewardLastEventAt ;
    #endregion

    #region Logs
    public bool FirstPurchaseDone;
    public bool FirstEnterBeginnerRegulation;
    public bool FirstEnterNormalRegulation;
    public bool FirstEnterHardRegulation;
    public bool FirstRewardedAdWatched;
    public bool FirstInterstitialAdWatched;
    public int NumberOfRaceToWinCrew1;
    public int NumberOfRaceToWinCrew2;
    public int NumberOfRaceToWinCrew3;
    public int NumberOfRaceToWinCrew32;
    public int NumberOfRaceToWinCrew4;
    public int NumberOfRaceToWinCrew42;
    public int NumberOfRaceToWinCrew43;
    #endregion

    public void Reset()
    {
        this.DisplayName = String.Empty;
        this.BestOverallQuarterMileTime = 0f;
        this.BestOverallHalfMileTime = 0f;
        this.TotalDistanceTravelled = 0f;
        this.TotalPlayTime = 0;
        this.TotalGarageTime = 0;
        this.CashSpent = 0L;
        MemoryValidator.Instance.InitialiseMangle<MangledCashSpent>(this.CashSpent, MangleInvoker.ProfileReset);
        this.CashEarned = 0L;
        MemoryValidator.Instance.InitialiseMangle<MangledCashEarned>(this.CashEarned, MangleInvoker.ProfileReset);
        this.GoldEarned = 0;
        MemoryValidator.Instance.InitialiseMangle<MangledGoldEarned>((long)this.GoldEarned, MangleInvoker.ProfileReset);
        this.GoldSpent = 0;
        MemoryValidator.Instance.InitialiseMangle<MangledGoldSpent>((long)this.GoldSpent, MangleInvoker.ProfileReset);
        this.NumMPCardPrizesWon = 0;
        this.PlayerXP = 0;
        this.PlayerLevel = 1;
        this.PlayerLeague = LeagueData.LeagueName.None;
        this.PlayerRP = 0;
        this.WorldRank = 0;
        this.PreviousWorldRank = 0;
        this.BoostNitrousUsed = 0;
        this.BossChallengeStateT1 = BossChallengeStateEnum.BEGIN;
        this.BossChallengeStateT2 = BossChallengeStateEnum.BEGIN;
        this.BossChallengeStateT3 = BossChallengeStateEnum.BEGIN;
        this.BossChallengeStateT4 = BossChallengeStateEnum.BEGIN;
        this.BossChallengeStateT5 = BossChallengeStateEnum.BEGIN;
        this.DailyBattlesLastEventAt = DateTime.MinValue;
        this.DailyBattlesConsecutiveDaysCount = 0;
        this.HasBoughtFirstCar = false;
        this.UserStartedPlaying = GTDateTime.Now;
        this.UserStartedLastSession = GTDateTime.Now;
        this.LastAgentNag = GTDateTime.Now.AddSeconds(-128700.0);
        this.LastPlayedMultiplayer = DateTime.MinValue;
        this.LastPlayedEliteClub = DateTime.MinValue;
        this.LastPlayedRaceTheWorldWorldTour = DateTime.MinValue;
        this.DifficultyNeedsReset = true;
        this.MechanicTuningRacesRemaining = 0;
        this.HaveShownFirstMechanicPopUp = false;
        this.CumulativeSessions = 0;
        this.ContiguousLosses = 0;
        this.ContiguousProgressionLosses = 0;
        this.ContiguousProgressionLossesTriggered = 0;
        this.InRollingRoadSession = false;
        this.EventsCompleted.Clear();
        this.ObjectivesCompleted.Clear();
        LegacyObjectivesCompleted.Clear();
        this.CurrentlySelectedCarDBKey = "FordFocusST";
        this.arrivalQueue.Clear();
        if (ArrivalManager.Instance != null)
        {
            ArrivalManager.Instance.debugClearArrivals();
        }
        SocialGamePlatformSelector.Instance.ResetScores();
        SocialGamePlatformSelector.Instance.ResetAchievements();
        this.EventsCompletedTier1 = 0;
        this.EventsCompletedTier2 = 0;
        this.EventsCompletedTier3 = 0;
        this.EventsCompletedTier4 = 0;
        this.EventsCompletedTier5 = 0;
        this.Tier1CarSpecificEventTarget = "None";
        this.Tier2CarSpecificEventTarget = "None";
        this.Tier3CarSpecificEventTarget = "None";
        this.Tier4CarSpecificEventTarget = "None";
        this.Tier5CarSpecificEventTarget = "None";
        this.Tier1ManufacturerSpecificEventTarget = "None";
        this.Tier2ManufacturerSpecificEventTarget = "None";
        this.Tier3ManufacturerSpecificEventTarget = "None";
        this.Tier4ManufacturerSpecificEventTarget = "None";
        this.Tier5ManufacturerSpecificEventTarget = "None";
        this.OptionNotifications = true;
        this.OptionMusicMute = false;
        this.OptionSoundMute = false;
        this.HasSeenLowMemoryLanguageMessage = false;
        this.HasSeenUnlockCarScreen = false;
        this.FuelPips = 10;
        this.GoldFuelRefills = 0;
        this.LastFuelAutoReplenishedTime = new DateTime(2016, 1, 1);
        this.LastLegalAgreementVersion = 0;
        this.CrewProgressionIntroductionPlayed = new bool[6];
        this.HasAttemptedFirstCrew = false;
        this.LastServerMessageDisplayedID = -1;
        this.LastServerMessageDisplayedCount = 0;
        this.FacebookInviteFuelRewardsCount = 0;
        this.FacebookInviteFuelRewardsTime = GTDateTime.Now;
        this.FacebookID = string.Empty;
        this.TwitterInviteFuelRewardsCount = 0;
        this.TwitterInviteFuelRewardsTime = GTDateTime.Now;
        DailyPrizeCardLastEventAt = GTDateTime.Now;
        AppTuttiTimedRewardLastEventAt = DateTime.MinValue;
        VasTimedRewardLastEventAt = DateTime.MinValue;
        this.IsFacebookSSORewardAllowed = true;
        this.BestTwitterNagTimeQtr = 100f;
        this.DoneRateAppTriggerBuyCar = false;
        this.DoneRateAppTriggerCrewMember = false;
        this.LastUpgradeDateTimeNag = GTDateTime.Now;
        this.HasHadMechanicSlowMotionIntroduction = false;
        this.HasSeenNitrousTutorial = false;
        this.ShouldShowSkipTo2ndCrewMemberPopup = false;
        this.UTCLastClockChange = GTDateTime.Now;
        this.CanTrySecondFacebookNag = true;
        this.RacesEnteredAtLastFacebookNag = 0;
        this.PreferredCsrAvatarPic = 0;//Random.Range(1, 15);
        this.SessionsCounter = 0;
        this.LastBoughtCarRacesEntered = 0;
        this.LastAdvertDate = GTDateTime.Now;
        this.AdCount = 0;
        this.TransactionHistory.Clear();
        this.MultiplayerDCBDifficulty = 0f;
        this.BestCarTimes = new Dictionary<string, float>();
        this.MultiplayerDCBConsecutiveWins = 0;
        this.MultiplayerDCBConsecutiveLoses = 0;
        this.CarsWonInMultiplayer.Clear();
        this.LastOnlineRace = GTDateTime.Now;
        this.OnlineRacesWonToday = 0;
        this.ConsecutiveRacesWonAtLowDifficulty = 0;
        this.ConsecutiveOnlineWins = 0;
        this.OnlineRacesWon = 0;
        this.OnlineRacesLost = 0;
        this.OnlineRacesLostToday = 0;
        this.FriendsRacesWon = 0;
        this.FriendsRacesLost = 0;
        this.FriendsCarsWon = 0;
        this.FriendsGold = 0;
        IEnumerator enumerator = Enum.GetValues(typeof (eCarConsumables)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                eCarConsumables key = (eCarConsumables) ((int) enumerator.Current);
                this.Consumables[key].Expires = DateTime.MinValue;
                this.Consumables[key].ExpiresSync = DateTime.MinValue;
                this.Consumables[key].AntiCheatRacesLeft = 0;
                this.Consumables[key].RacesLeft = 0;
                this.Consumables[key].CmlMinutesPurchased = 0;
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        this.BestEverMultiplayerWinStreak = 0;
        this.BestEverMultiplayerWinStreakBanked = 0;
        this.TotalMultiplayerStreaksCompleted = 0;
        this.TotalMultiplayerStreaksLost = 0;
        this.NumberOfStargazerMoments = 0;
        this.NumberOfSportsCarPiecesRemaining = 0;
        this.NumberOfDesiribleCarPiecesRemaining = 0;
        this.NumberOfCommonCarPiecesRemaining = 0;
        this.NumberOfTinyCashRewardsRemaining = 0;
        this.NumberOfSmallCashRewardsRemaining = 0;
        this.NumberOfMediumCashRewardsRemaining = 0;
        this.NumberOfLargeCashRewardsRemaining = 0;
        this.NumberOfHugeCashRewardsRemaining = 0;
        this.NumberOfTinyGoldRewardsRemaining = 0;
        this.NumberOfSmallGoldRewardsRemaining = 0;
        this.NumberOfMediumGoldRewardsRemaining = 0;
        this.NumberOfLargeGoldRewardsRemaining = 0;
        this.NumberOfHugeGoldRewardsRemaining = 0;
        this.NumberOfUpgradeRewardsRemaining = 0;
        this.NumberOfFuelRefillsRemaining = 0;
        this.NumberOfFuelPipsRewardsRemaining = 0;
        this.MultiplayerTutorial_SuccessfullyCompletedStreak = false;
        this.MultiplayerTutorial_HasSeenModeSelectScreen = false;
        this.MultiplayerTutorial_HasSeenAnyEvent = false;
        this.MultiplayerTutorial_VersusRaceTeamCompleted = false;
        this.MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted = false;
        this.MultiplayerTutorial_TracksideUpgradesCompleted = false;
        this.MultiplayerTutorial_FirstPrizeCompleted = false;
        this.MultiplayerTutorial_CardsButNoCarPartCompleted = false;
        this.MultiplayerTutorial_EliteClubCompleted = false;
        this.MultiplayerTutorial_LostWinStreakCompleted = false;
        this.MultiplayerTutorial_MapRaceTeamCompleted = false;
        this.MultiplayerTutorial_FirstCarPartCompleted = false;
        this.MultiplayerTutorial_PrizeScreenCompleted = false;
        this.MultiplayerTutorial_ElitePassCompleted = false;
        this.MultiplayerTutorial_EliteCarCompleted = false;
        this.MultiplayerConsumableUpsellCount = 0;
        this.MultiplayerConsumableMapUpsellCount = 0;
        this.MultiplayerConsumableMapUpsellStreaksLost = 0;
        this.FriendTutorial_HasSupressedTutorial = false;
        this.FriendTutorial_HasSeenFriendsPane = false;
        this.SharingTutorial_VideoShareCompleted = false;
        this.WelcomeMessageId = 0;
        this.SeasonPrizesAwarded.Clear();
        this.SeasonLastPlayedLeaderboardID = -1;
        this.SeasonLastPlayedEventID = -1;
        this.UDID = BaseIdentity.ActivePlatform.GenerateUDID();
        this.UUID = BaseIdentity.ActivePlatform.GetUUID();
        this.VideoForFuelTimestamps.Clear();
        this.ResetFriends();
        this.TwoStarTimeConditionShown = false;
        this.ThreeStarTimeConditionShown = false;
        this.HasSeenWonAllCarsConditionPopup = false;
        this.RaceWithFriendConditionCombos = new HashSet<KeyValuePair<int, string>>();
        this.BeatYourBestConditionShwonCount = 0;
        this.InviteFriendsConditionShownCount = 0;
        this.RaceTooHardConditionShownCount = 0;
        this.InviteFriendsConditionLastTimeShown = DateTime.MinValue;
        this.BuyACarLastConditionLastTimeShown = DateTime.MinValue;
        this.RaceWithFriendConditionLastTimeShown = DateTime.MinValue;
        this.FriendsEasyRaceConditionLastTimeShown = DateTime.MinValue;
        this.AllHardRacesConditionLastTimeShown = DateTime.MinValue;
        this.HasSeenRenewWholeTeamIAPCondition = false;
        this.HasUpgradedFuelTank = false;
        this.HasReceivedFuelTankUpgradeRefill = false;
        this.GasTankReminderLastTimeShown = DateTime.MinValue;
        this.GasTankReminderNumberOfTimesShown = 0;
        this.GasTankReminderIDShown = 0;
        this.UnlimitedFuel.Expires = DateTime.MinValue;
        this.UnlimitedFuel.ExpiresSync = DateTime.MinValue;
        this.UnlimitedFuel.CmlMinutesPurchased = 0;
        this.UnlimitedFuel.HasSeenRenewalPopup = false;
        this.UnlimitedFuel.RaceTeamPopupTimesSeen = 0;
        this.UnconfirmedDailyBattleResults.Clear();
        this.DailyBattleGoldConfirmed = 0;
        this.DailyBattleCashConfirmed = 0;
        this.GPGSignInCancellations = 0;
        this.RacesStartedThisSession = 0;
        this.HasSeenWorldTourIntroduction = false;
        this.SeenWorldTourLockedIntroCount = 0;
        this.OverrideRYFObjectives = false;
        this.OverrideRTWObjectives = false;
        this.WorldTourEventIDsWithAnimitionCompleted = new List<WorldTourAnimationsCompleted>();
        this.SeasonUnlockableThemesSeen = new List<string>();
        this.WTBoostNitrousStatus = new WorldTourBoostNitrous();
        this.WTDeferredNarrativeScenes = new Dictionary<string, DeferredNarrativeScene>();
        this.HasVisitedMechanicScreen = false;
        this.HasVisitedManufacturerScreen = false;
        this.HasAwardInstagramVisit = false;
        this.HasAwardTelegramVisit = false;
        this.HasOfferedCrewRacesCars = false;
        this.HasFinishedRYFTutorial = false;
        this.HasSeenFacebookNag = false;
        this.HasSeenMultiplayerIntroScreen = false;
        this.HasSeenInternationalIntroScreen = false;
        this.NumberOfRelaysCompetetedIn = 1;
        this.LastBundleOfferTimeShown = DateTime.MinValue;
        this.MapPaneSelected = 0;
        this.TierXPinSelections = new Dictionary<string, string>();
        this.UseMileAsUnit = false;
        this.NewCars.Clear();
        FirstPurchaseDone = false;
        FirstRewardedAdWatched = false;
        FirstInterstitialAdWatched = false;
        FirstEnterBeginnerRegulation = false;
        FirstEnterNormalRegulation = false;
        FirstEnterHardRegulation = false;
        NumberOfRaceToWinCrew1 = 0;
        NumberOfRaceToWinCrew2 = 0;
        NumberOfRaceToWinCrew3 = 0;
        NumberOfRaceToWinCrew32 = 0;
        NumberOfRaceToWinCrew4 = 0;
        NumberOfRaceToWinCrew42 = 0;
        NumberOfRaceToWinCrew43 = 0;
        FirstRace1 = false;
        FirstRace2 = false;
        FirstRace3 = false;
        FirstRace32 = false;
        FirstRace4 = false;
        FirstRace41 = false;
        FirstRace42 = false;

    }

    public void ResetFriends()
    {
        this.KnownFriendsPrizes.Clear();
        this.PeakFriends = 0;
        this.FriendsInvited = 0;
        this.InitialNetworkFriends = 0;
        this.SyncdServiceID.Clear();
        this.HighestCrossPromotionTierAdvertSeen = 0;
        this.FirstTimeRYFUser = true;
    }

    public void UpdatePlayerLevel()
    {
        PlayerLevel = GameDatabase.Instance.XPEvents.CurrentLevelForXP(PlayerXP);
    }

    public void UpdatePlayerLeague()
    {
        PlayerLeague = GameDatabase.Instance.StarDatabase.CurrentLeagueForStar(PlayerStar);
    }

    public void OnBeforeSerialize()
    {
        m_dailyBattleLastEventAt = DailyBattlesLastEventAt.ToString(CultureInfo.InvariantCulture);
        m_sMPWinChallengeActivationTime = SMPWinChallengeActivationTime.ToString(CultureInfo.InvariantCulture);
        m_lastBundleOfferTimeShown = LastBundleOfferTimeShown.ToString(CultureInfo.InvariantCulture);
        m_previousWeeklyLeaderboardCheck = PreviousWeeklyLeaderboardCheck.ToString(CultureInfo.InvariantCulture);
        m_dailyPrizeCardLastEventAt = DailyPrizeCardLastEventAt.ToString(CultureInfo.InvariantCulture);
        m_appTuttiTimedRewardLastEventAt = AppTuttiTimedRewardLastEventAt.ToString(CultureInfo.InvariantCulture);
        m_vasTimedRewardLastEventAt = VasTimedRewardLastEventAt.ToString(CultureInfo.InvariantCulture);
    }

    public void OnAfterDeserialize()
    {
        DailyBattlesLastEventAt = Convert.ToDateTime(m_dailyBattleLastEventAt);
        SMPWinChallengeActivationTime = Convert.ToDateTime(m_sMPWinChallengeActivationTime);
        LastBundleOfferTimeShown = Convert.ToDateTime(m_lastBundleOfferTimeShown);
        PreviousWeeklyLeaderboardCheck = Convert.ToDateTime(m_previousWeeklyLeaderboardCheck);
        DailyPrizeCardLastEventAt = Convert.ToDateTime(m_dailyPrizeCardLastEventAt);
        AppTuttiTimedRewardLastEventAt  = Convert.ToDateTime(m_appTuttiTimedRewardLastEventAt);
        VasTimedRewardLastEventAt  = Convert.ToDateTime(m_vasTimedRewardLastEventAt);
    }
}
