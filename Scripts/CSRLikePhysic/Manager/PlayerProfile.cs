using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataSerialization;
using GameAnalyticsSDK;
using I2.Loc;
using Metrics;
using Objectives;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerProfile
{
	private const int MaxRacesPerMinuteRTW = 2;

	public int SessionRacesCompleted;

	public int SessionDailyBattleRaces;

	private CarUpgradeSetup mCurrentCarUpgradeSetup;
    public int IAPCashSpent;
    public int IAPGoldSpent;
    public int IAPGachaBronzeKeysSpent;
    public int IAPGachaSilverKeysSpent;
    public int IAPGachaGoldKeysSpent;

    private PlayerProfileData ProfileData
	{
		get;
		set;
	}

	public string Name { get; private set; }

    public string DisplayName
    {
        get { return ProfileData.DisplayName; }
        set { ProfileData.DisplayName = value; }
    }

	public CarPhysicsSetupCreator PlayerPhysicsSetup
	{
		get;
		set;
	}

	public string CarSelectedBeforePrizeomaticWonCar
	{
		get;
		set;
	}

	public DateTime LastSaveDate
	{
		get
		{
			return ProfileData.dateTimeLastSaved;
		}
		set
		{
			ProfileData.dateTimeLastSaved = value;
		}
	}

	public DateTime LastAdvertDate
	{
		get
		{
			return ProfileData.LastAdvertDate;
		}
		set
		{
			ProfileData.LastAdvertDate = value;
		}
	}

	public int RacesWon
	{
		get
		{
#if LOCAL_STORAGE
		    return LocalStorageExtension.GetInt("RacesWon");
#endif
			return ProfileData.RacesWon;
		}
		set
		{
#if LOCAL_STORAGE
            LocalStorageExtension.SetInt("RacesWon",value);
#endif
			ProfileData.RacesWon = value;
		}
	}

    public string CurrentUserTimeZone
    {
        get { return ProfileData.CurrentUserTimeZone; }
        set { ProfileData.CurrentUserTimeZone = value; }
    }

    public string CurrentUserChosenLanguage
    {
        get { return ProfileData.CurrentUserChosenLanguage; }
        set { ProfileData.CurrentUserChosenLanguage = value; }
    } 
    

    public int RacesLost
    {
        get
        {
#if LOCAL_STORAGE
		    return LocalStorageExtension.GetInt("RacesLost");
#endif
            return ProfileData.RacesLost;
        }
        set
        {
#if LOCAL_STORAGE
            LocalStorageExtension.SetInt("RacesLost",value);
#endif
            ProfileData.RacesLost = value;
        }
    }

	public int FriendRacesPlayed
	{
		get
		{
			return ProfileData.FriendsRacesWon + ProfileData.FriendsRacesLost;
		}
	}

	public bool FirstTimeFriendsUser
	{
		get
		{
			return ProfileData.FirstTimeRYFUser;
		}
		set
		{
			ProfileData.FirstTimeRYFUser = value;
		}
	}

	public int FriendRacesWon
	{
		get
		{
			return ProfileData.FriendsRacesWon;
		}
		set
		{
			ProfileData.FriendsRacesWon = value;
		}
	}

	public int FriendRacesLost
	{
		get
		{
			return ProfileData.FriendsRacesLost;
		}
		set
		{
			ProfileData.FriendsRacesLost = value;
		}
	}

	public int FriendsCarsWon
	{
		get
		{
			return ProfileData.FriendsCarsWon;
		}
		set
		{
			ProfileData.FriendsCarsWon = value;
		}
	}

	public int FriendsGold
	{
		get
		{
			return ProfileData.FriendsGold;
		}
		set
		{
			ProfileData.FriendsGold = value;
		}
	}

	public int FriendsInvited
	{
		get
		{
			return ProfileData.FriendsInvited;
		}
		set
		{
			ProfileData.FriendsInvited = value;
		}
	}

	public int InitialNetworkFriends
	{
		get
		{
			return ProfileData.InitialNetworkFriends;
		}
	}

	public int OnlineRacesWon
	{
		get
		{
			return ProfileData.OnlineRacesWon;
		}
		set
		{
			ProfileData.OnlineRacesWon = value;
		}
	}

	public int ConsecutiveRacesWonAtLowDifficulty
	{
		get
		{
			return ProfileData.ConsecutiveRacesWonAtLowDifficulty;
		}
		set
		{
			ProfileData.ConsecutiveRacesWonAtLowDifficulty = value;
		}
	}

	public int ConsecutiveOnlineWins
	{
		get
		{
			return ProfileData.ConsecutiveOnlineWins;
		}
		set
		{
			ProfileData.ConsecutiveOnlineWins = value;
		}
	}

	public int OnlineRacesWonToday
	{
		get
		{
			return ProfileData.OnlineRacesWonToday;
		}
		set
		{
			ProfileData.OnlineRacesWonToday = value;
		}
	}

	public int OnlineRacesLostToday
	{
		get
		{
			return ProfileData.OnlineRacesLostToday;
		}
		set
		{
			ProfileData.OnlineRacesLostToday = value;
		}
	}

	public bool HasPlayedMultiplayer
	{
		get
		{
			return OnlineRacesLost > 0 || OnlineRacesWon > 0;
		}
	}

    public bool IsDailyBattleUnlocked
    {
        get
        {
            var firstCrewRace =
                GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups[0]
                    .RaceEvents[0].EventID;
            return PlayerProfileManager.Instance.ActiveProfile.EventsCompleted.Contains(firstCrewRace);
        }
    }

	public int OnlineRacesLost
	{
		get
		{
			return ProfileData.OnlineRacesLost;
		}
		set
		{
			ProfileData.OnlineRacesLost = value;
		}
	}

	public int NumberOfRelaysCompetetedIn
	{
		get
		{
			return ProfileData.NumberOfRelaysCompetetedIn;
		}
		set
		{
			ProfileData.NumberOfRelaysCompetetedIn = value;
		}
	}

	public bool HasBoughtFirstUpgrade
	{
#if LOCAL_STORAGE
	    get { return LocalStorageExtension.GetBool("HasBoughtFirstUpgrade"); }
        set { LocalStorageExtension.SetBool("HasBoughtFirstUpgrade", value); }
#else
		get
		{
			return ProfileData.HasBoughtFirstUpgrade;
		}
		set
		{
			ProfileData.HasBoughtFirstUpgrade = value;
		}
#endif
	}


    public bool HasBoughtFirstProperty
    {
#if LOCAL_STORAGE
        get { return LocalStorageExtension.GetBool("HasBoughtFirstProperty"); }
        set { LocalStorageExtension.SetBool("HasBoughtFirstProperty", value); }
#else
		get
		{
			return ProfileData.HasBoughtFirstProperty;
		}
		set
		{
			ProfileData.HasBoughtFirstProperty = value;
		}
#endif
    }

    public bool HasSeenGarageIntro
    {
        get
        {
            return ProfileData.HasSeenGarageIntro;
        }
        set
        {
            ProfileData.HasSeenGarageIntro = value;
        }
    }



    public int BoostNitrousUsed
	{
		get
		{
			return ProfileData.BoostNitrousUsed;
		}
		set
		{
			ProfileData.BoostNitrousUsed = value;
		}
	}

	public int FreeUpgradesLeft
	{
		get
		{
			return ProfileData.FreeUpgradesLeft;
		}
		set
		{
			ProfileData.FreeUpgradesLeft = value;
		}
	}

	public int WelcomeMessageId
	{
		get
		{
			return ProfileData.WelcomeMessageId;
		}
		set
		{
			ProfileData.WelcomeMessageId = value;
		}
	}

	public bool HasLikedOnFacebook
	{
		get
		{
			return ProfileData.HasLikedOnFacebook;
		}
		set
		{
			ProfileData.HasLikedOnFacebook = value;
		}
	}

	public bool HasFollowedUsOnTwitter
	{
		get
		{
			return ProfileData.HasFollowedUsOnTwitter;
		}
		set
		{
			ProfileData.HasFollowedUsOnTwitter = value;
		}
	}

	public Dictionary<string, float> BestCarTimes
	{
		get
		{
			return ProfileData.BestCarTimes;
		}
		set
		{
			ProfileData.BestCarTimes = value;
		}
	}

	public bool InRollingRoadSession
	{
		get
		{
			return ProfileData.InRollingRoadSession;
		}
		set
		{
			ProfileData.InRollingRoadSession = value;
		}
	}

	public int MechanicTuningRacesRemaining
	{
		get
		{
			return ProfileData.MechanicTuningRacesRemaining;
		}
		set
		{
			ProfileData.MechanicTuningRacesRemaining = value;
		}
	}

	public bool MechanicIsUnlocked
	{
		get
		{
			return HasVisitedMechanicScreen || HasHadMechanicSlowMotionIntroduction;
		}
	}

    public bool HasAwardInstagramVisit
    {
        get { return ProfileData.HasAwardInstagramVisit; }
        set { ProfileData.HasAwardInstagramVisit = value; }
    }

    public bool HasAwardTelegramVisit
    {
        get { return ProfileData.HasAwardTelegramVisit; }
        set { ProfileData.HasAwardTelegramVisit = value; }
    }

	public int CumulativeSessions
	{
		get
		{
			return ProfileData.CumulativeSessions;
		}
		set
		{
			ProfileData.CumulativeSessions = value;
		}
	}

	public int NumMPCardPrizesWon
	{
		get
		{
			return ProfileData.NumMPCardPrizesWon;
		}
		set
		{
			ProfileData.NumMPCardPrizesWon = value;
		}
	}

	public int CumulativeTweets
	{
		get
		{
			return ProfileData.NumTweets;
		}
		set
		{
			ProfileData.NumTweets = value;
		}
	}

	public int CumulativeFBPosts
	{
		get
		{
			return ProfileData.NumFBPosts;
		}
		set
		{
			ProfileData.NumFBPosts = value;
		}
	}

	public int SessionCounter
	{
		get
		{
			return ProfileData.SessionsCounter;
		}
		set
		{
			ProfileData.SessionsCounter = value;
		}
	}

	public int ContiguousLosses
	{
		get
		{
			return ProfileData.ContiguousLosses;
		}
		set
		{
			ProfileData.ContiguousLosses = value;
		}
	}

	public bool OptionSoundMute
	{
		get
		{
			return ProfileData.OptionSoundMute;
		}
		set
		{
			ProfileData.OptionSoundMute = value;
		}
	}

	public bool OptionMusicMute
	{
		get
		{
			return ProfileData.OptionMusicMute;
		}
		set
		{
			ProfileData.OptionMusicMute = value;
		}
	}

	public bool OptionNotifications
	{
		get
		{
			return ProfileData.OptionNotifications;
		}
		set
		{
			ProfileData.OptionNotifications = value;
		}
	}

	public bool HasSeenLowMemoryLanguageMessage
	{
		get
		{
			return ProfileData.HasSeenLowMemoryLanguageMessage;
		}
		set
		{
			ProfileData.HasSeenLowMemoryLanguageMessage = value;
		}
	}

	public GarageSortOrder OptionGarageSortOrder
	{
		get
		{
			return ProfileData.OptionGarageSortOrder;
		}
		set
		{
			ProfileData.OptionGarageSortOrder = value;
		}
	}

	public bool OptionReverseSortOrder
	{
		get
		{
			return ProfileData.OptionReverseSortOrder;
		}
		set
		{
			ProfileData.OptionReverseSortOrder = value;
		}
	}

	public DateTime UserStartedPlaying
	{
		get
		{
			return ProfileData.UserStartedPlaying;
		}
		set
		{
			ProfileData.UserStartedPlaying = value;
		}
	}

	public DateTime UserStartedLastSession
	{
		get
		{
			return ProfileData.UserStartedLastSession;
		}
		set
		{
			ProfileData.UserStartedLastSession = value;
		}
	}

    public List<SocialGamePlatformSelector.AchievementData> PlayerAchievements { get
        {
            return ProfileData.playerAchievements;
        }
        set
        {
            ProfileData.playerAchievements = value;
        }
    }

    public List<SocialGamePlatformSelector.ScoreData> PlayerScores
    {
        get { return ProfileData.playerScores; }
        set { ProfileData.playerScores = value; }
    }

    public List<Arrival> ArrivalQueue
	{
		get
		{
			return ProfileData.arrivalQueue;
		}
		set
		{
			ProfileData.arrivalQueue = value;
		}
	}

	public List<int> EventsCompleted
	{
		get
		{
			return ProfileData.EventsCompleted;
		}
		set
		{
			ProfileData.EventsCompleted = value;
		}
	}

	public int NumberOfRace4101
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew1;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew1 = value;
		}
	}
	
	public int NumberOfRace4102
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew2;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew2 = value;
		}
	}
	
	public int NumberOfRace4103
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew3;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew3= value;
		}
	}
	
	public int NumberOfRace41031
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew32;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew32 = value;
		}
	}
	
	public int NumberOfRace41041
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew4;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew4 = value;
		}
	}
	
	public int NumberOfRace41042
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew42;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew42 = value;
		}
	}
	
	public int NumberOfRace4105
	{
		get
		{
			return ProfileData.NumberOfRaceToWinCrew43;
		}
		set
		{
			ProfileData.NumberOfRaceToWinCrew43 = value;
		}
	}
	
	public bool IsFirstBodyUpgrade
	{
		get
		{
			return ProfileData.IsFirstBodyUpgrade;
		}
		set
		{
			ProfileData.IsFirstBodyUpgrade = value;
		}
	}

	public bool IsFirstEngineUpgrade
	{
		get
		{
			return ProfileData.IsFirstEngineUpgrade;
		}
		set
		{
			ProfileData.IsFirstEngineUpgrade = value;
		}
	}
	
	public bool IsFirstIntakeUpgrade
	{
		get
		{
			return ProfileData.IsFirstIntakeUpgrade;
		}
		set
		{
			ProfileData.IsFirstIntakeUpgrade = value;
		}
	}
	
	public bool IsFirstNitrousUpgrade
	{
		get
		{
			return ProfileData.IsFirstNitrousUpgrade;
		}
		set
		{
			ProfileData.IsFirstBodyUpgrade = value;
		}
	}
	
	public bool IsFirstTransmissionUpgrade
	{
		get
		{
			return ProfileData.IsFirstTransmissionUpgrade;
		}
		set
		{
			ProfileData.IsFirstTransmissionUpgrade = value;
		}
	}
	
	public bool IsFirstTurboUpgrade
	{
		get
		{
			return ProfileData.IsFirstTurboUpgrade;
		}
		set
		{
			ProfileData.IsFirstTurboUpgrade = value;
		}
	}
	
	public bool IsFirstTyresUpgrade
	{
		get
		{
			return ProfileData.IsFirstTyresUpgrade;
		}
		set
		{
			ProfileData.IsFirstTyresUpgrade = value;
		}
	}

	public bool IsFirstTapToUpgrade
	{
		get
		{
			return ProfileData.IsFirstTapToUpgrade;
		}
		set
		{
			ProfileData.IsFirstTapToUpgrade = value;
		}
	}
	
    public bool IsEventCompleted(int eventID)
    {
#if UNITY_EDITOR
        if (GameDatabase.Instance != null && GameDatabase.Instance.DebugDatabase.Configuration!=null && GameDatabase.Instance.DebugDatabase.Configuration.UseDebugEvents)
            return GameDatabase.Instance.DebugDatabase.ContainsEventID(eventID);
        else
            return ProfileData.EventsCompleted.Contains(eventID);
#else
        return ProfileData.EventsCompleted.Contains(eventID);
#endif
    }

    public List<int> LegacyObjectivesCompleted
    {
        get
        {
            return ProfileData.LegacyObjectivesCompleted;
        }
        set
        {
            ProfileData.LegacyObjectivesCompleted = value;
        }
    }

    public List<string> ObjectivesCompleted
	{
		get
		{
			return ProfileData.ObjectivesCompleted;
		}
		set
		{
			ProfileData.ObjectivesCompleted = value;
		}
	}

    //public List<string> TutorialObjectivesCompleted
    //{
    //    get
    //    {
    //        return this.ProfileData.TutorialObjectivesCompleted;
    //    }
    //    set
    //    {
    //        this.ProfileData.TutorialObjectivesCompleted = value;
    //    }
    //}

    public List<string> ObjectivesCollected
    {
        get
        {
            return this.ProfileData.ObjectivesCollected;
        }
        set
        {
            this.ProfileData.ObjectivesCollected = value;
        }
    }

    public Dictionary<string, JsonDict> ActiveObjectives
    {
        get
        {
            return this.ProfileData.ActiveObjectives;
        }
        set
        {
            this.ProfileData.ActiveObjectives = value;
        }
    }

    public DateTime ObjectiveEndTime
    {
        get
        {
            return this.ProfileData.ObjectiveEndTime;
        }
        set
        {
            this.ProfileData.ObjectiveEndTime = value;
        }
    }

	public string CurrentlySelectedCarDBKey
	{
        get
		{
			return ProfileData.CurrentlySelectedCarDBKey;
		}
		set
		{
			ProfileData.CurrentlySelectedCarDBKey = value;
			UpdateCurrentCarSetup();
		}
    }

	public List<CarGarageInstance> CarsOwned
	{
		get
		{
			return ProfileData.CarsOwned;
		}
		set
		{
			ProfileData.CarsOwned = value;
		}
	}

	public int TotalPlayTime
	{
		get
		{
			return ProfileData.TotalPlayTime;
		}
		set
		{
			ProfileData.TotalPlayTime = value;
		}
	}

	public int TotalGarageTime
	{
		get
		{
			return ProfileData.TotalGarageTime;
		}
		set
		{
			ProfileData.TotalGarageTime = value;
		}
	}

	public DateTime LastAgentNag
	{
		get
		{
			return ProfileData.LastAgentNag;
		}
		set
		{
			ProfileData.LastAgentNag = value;
		}
	}

	public DateTime LastPlayedMultiplayer
	{
		get
		{
			return ProfileData.LastPlayedMultiplayer;
		}
		set
		{
			ProfileData.LastPlayedMultiplayer = value;
		}
	}

	public DateTime LastPlayedEliteClub
	{
		get
		{
			return ProfileData.LastPlayedEliteClub;
		}
		set
		{
			ProfileData.LastPlayedEliteClub = value;
		}
	}

	public DateTime LastPlayedRaceTheWorldWorldTour
	{
		get
		{
			return ProfileData.LastPlayedRaceTheWorldWorldTour;
		}
		set
		{
			ProfileData.LastPlayedRaceTheWorldWorldTour = value;
		}
	}

	public bool DifficultyNeedsReset
	{
		get
		{
			return ProfileData.DifficultyNeedsReset;
		}
		set
		{
			ProfileData.DifficultyNeedsReset = value;
		}
	}

	public float MultiplayerDifficulty
	{
		get
		{
			return ProfileData.MultiplayerDifficulty;
		}
		set
		{
			ProfileData.MultiplayerDifficulty = value;
		}
	}

	public float EliteMultiplayerDifficulty
	{
		get
		{
			return ProfileData.EliteMultiplayerDifficulty;
		}
		set
		{
			ProfileData.EliteMultiplayerDifficulty = value;
		}
	}

	public float MultiplayerEventDifficulty
	{
		get
		{
			return ProfileData.MultiplayerEventDifficulty;
		}
		set
		{
			ProfileData.MultiplayerEventDifficulty = value;
		}
	}

	public float BestOverallQuarterMileTime
	{
		get
		{
			return ProfileData.BestOverallQuarterMileTime;
		}
		set
		{
			ProfileData.BestOverallQuarterMileTime = value;
		}
	}

	public float TotalDistanceTravelled
	{
		get
		{
			return ProfileData.TotalDistanceTravelled;
		}
		set
		{
			ProfileData.TotalDistanceTravelled = value;
		}
	}

	public int EventsCompletedTier1
	{
		get
		{
			return ProfileData.EventsCompletedTier1;
		}
		set
		{
			ProfileData.EventsCompletedTier1 = value;
		}
	}

	public int EventsCompletedTier2
	{
		get
		{
			return ProfileData.EventsCompletedTier2;
		}
		set
		{
			ProfileData.EventsCompletedTier2 = value;
		}
	}

	public int EventsCompletedTier3
	{
		get
		{
			return ProfileData.EventsCompletedTier3;
		}
		set
		{
			ProfileData.EventsCompletedTier3 = value;
		}
	}

	public int EventsCompletedTier4
	{
		get
		{
			return ProfileData.EventsCompletedTier4;
		}
		set
		{
			ProfileData.EventsCompletedTier4 = value;
		}
	}

	public int EventsCompletedTier5
	{
		get
		{
			return ProfileData.EventsCompletedTier5;
		}
		set
		{
			ProfileData.EventsCompletedTier5 = value;
		}
	}

	public int AdCount
	{
		get
		{
			return ProfileData.AdCount;
		}
		set
		{
			ProfileData.AdCount = value;
		}
	}

	public int RacesEntered
	{
        get
        {
#if LOCAL_STORAGE
            return LocalStorageExtension.GetInt("RacesEntered");
#endif
            return ProfileData.RacesEntered;
        }
        set
        {
#if LOCAL_STORAGE
            LocalStorageExtension.SetInt("RacesEntered", value);
#endif
            ProfileData.RacesEntered = value;
        }
	}

	public int TutorialRacesEntered
	{
		get
		{
			return ProfileData.TutorialRacesAttempted;
		}
		set
		{
			ProfileData.TutorialRacesAttempted = value;
		}
	}

	public int MechanicFettledRaces
	{
		get
		{
			return ProfileData.MechanicFettledRaces;
		}
		set
		{
			ProfileData.MechanicFettledRaces = value;
		}
	}

	public int ContiguousProgressionLosses
	{
		get
		{
			return ProfileData.ContiguousProgressionLosses;
		}
		set
		{
			ProfileData.ContiguousProgressionLosses = value;
		}
	}

	public int ContiguousProgressionLossesTriggered
	{
		get
		{
			return ProfileData.ContiguousProgressionLossesTriggered;
		}
		set
		{
			ProfileData.ContiguousProgressionLossesTriggered = value;
		}
	}

	public bool HaveShownFirstMechanicPopUp
	{
		get
		{
			return ProfileData.HaveShownFirstMechanicPopUp;
		}
		set
		{
			ProfileData.HaveShownFirstMechanicPopUp = value;
		}
	}

	public bool FirstWinInCrew4101
	{
		get
		{
			return ProfileData.FirstRace1;
		}
		set
		{
			ProfileData.FirstRace1 = value;
		}
	}
	
	public bool FirstWinInCrew4102
	{
		get
		{
			return ProfileData.FirstRace2;
		}
		set
		{
			ProfileData.FirstRace2 = value;
		}
	}
	
	public bool FirstWinInCrew4103
	{
		get
		{
			return ProfileData.FirstRace3;
		}
		set
		{
			ProfileData.FirstRace3 = value;
		}
	}
	
	public bool FirstWinInCrew41031
	{
		get
		{
			return ProfileData.FirstRace32;
		}
		set
		{
			ProfileData.FirstRace32 = value;
		}
	}
	
	public bool FirstWinInCrew41041
	{
		get
		{
			return ProfileData.FirstRace4;
		}
		set
		{
			ProfileData.FirstRace4 = value;
		}
	}
	
	public bool FirstWinInCrew41042
	{
		get
		{
			return ProfileData.FirstRace41;
		}
		set
		{
			ProfileData.FirstRace41 = value;
		}
	}
	
	public bool FirstWinInCrew4105
	{
		get
		{
			return ProfileData.FirstRace42;
		}
		set
		{
			ProfileData.FirstRace42 = value;
		}
	}

	public bool HasSignedIntoFacebookBefore
	{
		get
		{
			return ProfileData.HasSignedIntoFacebookBefore;
		}
		set
		{
			ProfileData.HasSignedIntoFacebookBefore = value;
		}
	}

	public bool HasSignedIntoGameCentreBefore
	{
		get
		{
			return ProfileData.HasSignedIntoGameCentreBefore;
		}
		set
		{
			ProfileData.HasSignedIntoGameCentreBefore = value;
		}
	}
	public bool HasSignedIntoGooglePlayGamesBefore
	{
		get
		{
			return ProfileData.HasSignedIntoGooglePlayGamesBefore;
		}
		set
		{
			ProfileData.HasSignedIntoGooglePlayGamesBefore = value;
		}
	}

	public long CashEarned
	{
		get
		{
            if (ProfileData == null)
            {
                return 0;
            }
			return ProfileData.CashEarned;
		}
	}

	public long CashSpent
	{
		get
		{
            if (ProfileData == null)
            {
                return 0;
            }
            return ProfileData.CashSpent;
		}
	}


    public long CashBought
    {
        get
        {
            return ProfileData.CashBought;
        }
        set { ProfileData.CashBought = value; }
    }

    public int GoldEarned
	{
		get
		{
            if (ProfileData == null)
            {
                return 0;
            }
            return ProfileData.GoldEarned;
		}
	}

    public int GoldSpent
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return ProfileData.GoldSpent;
        }
    }


    public int GoldBought
    {
        get { return ProfileData.GoldBought; }
        set { ProfileData.GoldBought = value; }
    }


    public int GachaGoldKeysEarned
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaGoldKeysEarned;
        }
    }

    public int GachaSilverKeysEarned
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaSilverKeysEarned;
        }
    }

    public int GachaBronzeKeysEarned
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaBronzeKeysEarned;
        }
    }

    public int GachaGoldKeysEarnedSetAndMangle
    {
        set
        {
            this.ProfileData.GachaGoldKeysEarned = value;
            MemoryValidator.Instance.Mangle<MangledGachaGoldKeysEarned>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GachaSilverKeysEarnedSetAndMangle
    {
        set
        {
            this.ProfileData.GachaSilverKeysEarned = value;
            MemoryValidator.Instance.Mangle<MangledGachaSilverKeysEarned>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GachaBronzeKeysEarnedSetAndMangle
    {
        set
        {
            this.ProfileData.GachaBronzeKeysEarned = value;
            MemoryValidator.Instance.Mangle<MangledGachaBronzeKeysEarned>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public long CashEarnedSetAndMangle
    {
        set
        {
            this.ProfileData.CashEarned = value;
            MemoryValidator.Instance.Mangle<MangledCashEarned>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public long CashSpentSetAndMangle
    {
        set
        {
            this.ProfileData.CashSpent = value;
            MemoryValidator.Instance.Mangle<MangledCashSpent>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GoldEarnedSetAndMangle
    {
        set
        {
            this.ProfileData.GoldEarned = value;
            MemoryValidator.Instance.Mangle<MangledGoldEarned>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GoldSpentSetAndMangle
    {
        set
        {
            this.ProfileData.GoldSpent = value;
            MemoryValidator.Instance.Mangle<MangledGoldSpent>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int CashEarnedDisplayCapped
	{
		get
		{
			return (ProfileData.CashEarned > 2147483647L) ? 2147483647 : ((int)ProfileData.CashEarned);
		}
	}

	public int CashSpentDisplayCapped
	{
		get
		{
			return (ProfileData.CashSpent > 2147483647L) ? 2147483647 : ((int)ProfileData.CashSpent);
		}
	}

	public bool HasBoughtFirstCar
	{
        get
		{
			return ProfileData.HasBoughtFirstCar;
		}
		set
		{
			ProfileData.HasBoughtFirstCar = value;
		}
    }


    public bool HasChoosePlayerName
    {
        get
		{
			return ProfileData.HasChoosePlayerName;
		}
		set
		{
			ProfileData.HasChoosePlayerName = value;
		}
    }

	public float BestOverallHalfMileTime
	{
		get
		{
			return ProfileData.BestOverallHalfMileTime;
		}
		set
		{
			ProfileData.BestOverallHalfMileTime = value;
		}
	}

	public int FuelRefillsBoughtWithGold
	{
		get
		{
			return ProfileData.GoldFuelRefills;
		}
		set
		{
			if (ProfileData.GoldFuelRefills < value)
			{
                ProfileData.LastFuelAutoReplenishedTime = GTDateTime.Now;
			}
			ProfileData.GoldFuelRefills = value;
		}
	}

	public bool[] CrewProgressionIntroductionPlayed
	{
		get
		{
			return ProfileData.CrewProgressionIntroductionPlayed;
		}
		set
		{
			ProfileData.CrewProgressionIntroductionPlayed = value;
		}
	}

	public bool AttemptedFirstCrewRace
	{
		get
		{
			return ProfileData.HasAttemptedFirstCrew;
		}
		set
		{
			ProfileData.HasAttemptedFirstCrew = value;
		}
	}

	public bool HasHadMechanicSlowMotionIntroduction
	{
		get
		{
			return ProfileData.HasHadMechanicSlowMotionIntroduction;
		}
		set
		{
			ProfileData.HasHadMechanicSlowMotionIntroduction = value;
		}
	}

	public bool HasSeenNitrousTutorial
	{
		get
		{
			return ProfileData.HasSeenNitrousTutorial;
		}
		set
		{
			ProfileData.HasSeenNitrousTutorial = value;
		}
	}

	public float BestTwitterNagTimeQtr
	{
		get
		{
			return ProfileData.BestTwitterNagTimeQtr;
		}
		set
		{
			ProfileData.BestTwitterNagTimeQtr = value;
		}
	}

	public bool CanTrySecondFacebookNag
	{
		get
		{
			return ProfileData.CanTrySecondFacebookNag;
		}
		set
		{
			ProfileData.CanTrySecondFacebookNag = value;
		}
	}

	public int RacesEnteredAtLastFacebookNag
	{
		get
		{
			return ProfileData.RacesEnteredAtLastFacebookNag;
		}
		set
		{
			ProfileData.RacesEnteredAtLastFacebookNag = value;
		}
	}

	public int PlayerWorldRank
	{
		get
		{
			return ProfileData.WorldRank;
		}
		set
		{
			ProfileData.WorldRank = value;
		}
	}

	public int PreviousPlayerWorldRank
	{
		get
		{
			return ProfileData.PreviousWorldRank;
		}
		set
		{
			ProfileData.PreviousWorldRank = value;
		}
	}

    public bool SeenOfferWallButton
    {
        get
        {
            return this.ProfileData.OfferWallData.SeenOfferWallButton;
        }
        set
        {
            this.ProfileData.OfferWallData.SeenOfferWallButton = value;
        }
    }

    public DateTime CarePackageUpdateTime
    {
        get
        {
            return this.ProfileData.CarePackageData.LastUpdateTime;
        }
    }

    public string CarePackageId
    {
        get
        {
            return this.ProfileData.CarePackageData.CarePackageId;
        }
    }

    public bool CarePackageDisplayed
    {
        get
        {
            return this.ProfileData.CarePackageData.CarePackageDisplayed;
        }
    }

    public WorldTourBoostNitrous WorldTourBoostNitrous
    {
        get
        {
            return this.ProfileData.WTBoostNitrousStatus;
        }
    }

    public Dictionary<string, PlayerProfileData.DeferredNarrativeScene> WorldTourDeferredNarrativeScenes
	{
		get
		{
			return ProfileData.WTDeferredNarrativeScenes;
		}
		set
		{
			ProfileData.WTDeferredNarrativeScenes = value;
		}
	}

	public bool ShouldUseOldLiveryCalc
	{
		get
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			DateTime value = dateTime.AddSeconds(GameDatabase.Instance.CurrenciesConfiguration.RewardsMultipliers.LiveryBonusTypeDateBoundary);
			return UserStartedPlaying.CompareTo(value) <= 0;
		}
	}

	public int RacesStartedThisSession
	{
		get
		{
			return ProfileData.RacesStartedThisSession;
		}
		set
		{
			ProfileData.RacesStartedThisSession = value;
		}
	}

	public string LastAcquiredCar
	{
		get;
		private set;
	}

	public int LastServerMessageDisplayedID
	{
		get
		{
			return ProfileData.LastServerMessageDisplayedID;
		}
		set
		{
			ProfileData.LastServerMessageDisplayedID = value;
		}
	}

	public int LastServerMessageDisplayedCount
	{
		get
		{
			return ProfileData.LastServerMessageDisplayedCount;
		}
		set
		{
			ProfileData.LastServerMessageDisplayedCount = value;
		}
	}

	public DateTime TwitterCashRewardsTime
	{
		get
		{
			return ProfileData.TwitterCashRewardsTime;
		}
		set
		{
			ProfileData.TwitterCashRewardsTime = value;
		}
	}

	public int TwitterCashRewardsCount
	{
		get
		{
			return ProfileData.TwitterCashRewardsCount;
		}
		set
		{
			ProfileData.TwitterCashRewardsCount = value;
		}
	}

	public bool IsFacebookSSORewardAllowed
	{
		get
		{
			return ProfileData.IsFacebookSSORewardAllowed;
		}
		set
		{
			ProfileData.IsFacebookSSORewardAllowed = value;
		}
	}

	public DateTime FacebookInviteFuelRewardsTime
	{
		get
		{
			return ProfileData.FacebookInviteFuelRewardsTime;
		}
		set
		{
			ProfileData.FacebookInviteFuelRewardsTime = value;
		}
	}

	public int FacebookInviteFuelRewardsCount
	{
		get
		{
			return ProfileData.FacebookInviteFuelRewardsCount;
		}
		set
		{
			ProfileData.FacebookInviteFuelRewardsCount = value;
		}
	}

	public DateTime TwitterInviteFuelRewardsTime
	{
		get
		{
			return ProfileData.TwitterInviteFuelRewardsTime;
		}
		set
		{
			ProfileData.TwitterInviteFuelRewardsTime = value;
		}
	}

	public int TwitterInviteFuelRewardsCount
	{
		get
		{
			return ProfileData.TwitterInviteFuelRewardsCount;
		}
		set
		{
			ProfileData.TwitterInviteFuelRewardsCount = value;
		}
	}

	public string FacebookID
	{
		get
		{
			return ProfileData.FacebookID;
		}
		set
		{
			ProfileData.FacebookID = value;
		}
	}

	public bool DoneRateAppTriggerBuyCar
	{
		get
		{
			return ProfileData.DoneRateAppTriggerBuyCar;
		}
		set
		{
			ProfileData.DoneRateAppTriggerBuyCar = value;
		}
	}

	public bool DoneRateAppTriggerCrewMember
	{
		get
		{
			return ProfileData.DoneRateAppTriggerCrewMember;
		}
		set
		{
			ProfileData.DoneRateAppTriggerCrewMember = value;
		}
	}

	public DateTime LastUpgradeDateTimeNag
	{
		get
		{
			return ProfileData.LastUpgradeDateTimeNag;
		}
		set
		{
			ProfileData.LastUpgradeDateTimeNag = value;
		}
	}

	public bool ShouldShowSkipTo2ndCrewMemberPopup
	{
		get
		{
			return ProfileData.ShouldShowSkipTo2ndCrewMemberPopup;
		}
		set
		{
			ProfileData.ShouldShowSkipTo2ndCrewMemberPopup = value;
		}
	}

	public DateTime UTCLastClockChange
	{
		get
		{
			return ProfileData.UTCLastClockChange;
		}
		set
		{
			ProfileData.UTCLastClockChange = value;
		}
	}

	public int LastBoughtCarRacesEntered
	{
		get
		{
			return ProfileData.LastBoughtCarRacesEntered;
		}
		set
		{
			ProfileData.LastBoughtCarRacesEntered = value;
		}
	}

	public float MultiplayerDCBDifficulty
	{
		get
		{
			return ProfileData.MultiplayerDCBDifficulty;
		}
		set
		{
			ProfileData.MultiplayerDCBDifficulty = value;
		}
	}

	public int MultiplayerDCBConsecutiveWins
	{
		get
		{
			return ProfileData.MultiplayerDCBConsecutiveWins;
		}
		set
		{
			ProfileData.MultiplayerDCBConsecutiveWins = value;
		}
	}

	public int MultiplayerDCBConsecutiveLoses
	{
		get
		{
			return ProfileData.MultiplayerDCBConsecutiveLoses;
		}
		set
		{
			ProfileData.MultiplayerDCBConsecutiveLoses = value;
		}
	}

	public int PreferredCsrAvatarPicture
	{
		get
		{
			return ProfileData.PreferredCsrAvatarPic;
		}
		set
		{
			ProfileData.PreferredCsrAvatarPic = value;
		}
	}

	public DateTime UnlimitedFuelExpires
	{
		get
		{
			return ProfileData.UnlimitedFuel.Expires;
		}
		set
		{
			ProfileData.UnlimitedFuel.Expires = value;
		}
	}

	public DateTime UnlimitedFuelExpiresSync
	{
		get
		{
			return ProfileData.UnlimitedFuel.ExpiresSync;
		}
		set
		{
			ProfileData.UnlimitedFuel.ExpiresSync = value;
		}
	}

	public int UnlimitedFuelMinutesPurchased
	{
		get
		{
			return ProfileData.UnlimitedFuel.CmlMinutesPurchased;
		}
		set
		{
			ProfileData.UnlimitedFuel.CmlMinutesPurchased = value;
		}
	}

	public bool HasSeenUnlimitedFuelRenewalPopup
	{
		get
		{
			return ProfileData.UnlimitedFuel.HasSeenRenewalPopup;
		}
		set
		{
			ProfileData.UnlimitedFuel.HasSeenRenewalPopup = value;
		}
	}

	public int UnlimitedFuelRaceTeamPopupTimesSeen
	{
		get
		{
			return ProfileData.UnlimitedFuel.RaceTeamPopupTimesSeen;
		}
	}

	public bool DoneUpgradeWarningOnNewTier
	{
		get
		{
			return ProfileData.DoneUpgradeWarningOnNewTier;
		}
		set
		{
			ProfileData.DoneUpgradeWarningOnNewTier = value;
		}
	}

	public int BestEverMultiplayerWinStreak
	{
		get
		{
			return ProfileData.BestEverMultiplayerWinStreak;
		}
		set
		{
			ProfileData.BestEverMultiplayerWinStreak = value;
		}
	}

	public int BestEverMultiplayerWinStreakBanked
	{
		get
		{
			return ProfileData.BestEverMultiplayerWinStreakBanked;
		}
		set
		{
			ProfileData.BestEverMultiplayerWinStreakBanked = value;
		}
	}

	public int TotalMultiplayerStreaksCompleted
	{
		get
		{
			return ProfileData.TotalMultiplayerStreaksCompleted;
		}
		set
		{
			ProfileData.TotalMultiplayerStreaksCompleted = value;
		}
	}

	public int TotalMultiplayerStreaksLost
	{
		get
		{
			return ProfileData.TotalMultiplayerStreaksLost;
		}
		set
		{
			ProfileData.TotalMultiplayerStreaksLost = value;
		}
	}

    public int SMPWins
    {
        get
        {
            return this.ProfileData.SMPWins;
        }
        set
        {
            this.ProfileData.SMPWins = value;
        }
    }

    public int SMPWinsLastSession
    {
        get
        {
            return this.ProfileData.SMPWinsLastSession;
        }
        set
        {
            this.ProfileData.SMPWinsLastSession = value;
        }
    }

    public int SMPLosses
    {
        get
        {
            return this.ProfileData.SMPLosses;
        }
        set
        {
            this.ProfileData.SMPLosses = value;
        }
    }

    public int SMPConsecutiveRaces
    {
        get
        {
            return this.ProfileData.SMPConsecutiveRaces;
        }
        set
        {
            this.ProfileData.SMPConsecutiveRaces = value;
        }
    }


    public int SMPChallengeWins
    {
        get
        {
            return ProfileData.SMPChallengeWins;
        }
        set
        {
            ProfileData.SMPChallengeWins = value;
        }
    }

    public string SMPWinStreakID
    {
        get
        {
            return this.ProfileData.SMPWinStreakID;
        }
        set
        {
            this.ProfileData.SMPWinStreakID = value;
        }
    }


    public int SMPTotalRaces
    {
        get
        {
            return ProfileData.SMPTotalRaces;
        }
        set
        {
            ProfileData.SMPTotalRaces = value;
        }
    }


    public DateTime SMPStartSessionDate
    {
        get
        {
            return ProfileData.SMPStartSessionDate;
        }
        set
        {
            ProfileData.SMPStartSessionDate = value;
        }
    }


    public int SMPTotalRacesLastSession
    {
        get
        {
            return ProfileData.SMPTotalRacesLastSession;
        }
        set
        {
            ProfileData.SMPTotalRacesLastSession = value;
        }
    }

    public int SMPConsecutiveLoses
    {
        get
        {
            return this.ProfileData.SMPConsecutiveLoses;
        }
        set
        {
            this.ProfileData.SMPConsecutiveLoses = value;
        }
    }

    public int SMPConsecutiveWins
    {
        get
        {
            return this.ProfileData.SMPConsecutiveWins;
        }
        set
        {
            this.ProfileData.SMPConsecutiveWins = value;
        }
    }

    public long LastWinStreakExtendedTime
    {
        get
        {
            return this.ProfileData.LastWinStreakExtendedTime;
        }
        set
        {
            this.ProfileData.LastWinStreakExtendedTime = value;
        }
    }

    public bool IsSMPWinChallengeAvailable
    {
        get
        {
            return this.ProfileData.IsSMPWinChallengeAvailable;
        }
        set
        {
            this.ProfileData.IsSMPWinChallengeAvailable = value;
        }
    }


    public DateTime SMPWinChallengeActivationTime
    {
        get
        {
            return this.ProfileData.SMPWinChallengeActivationTime;
        }
        set
        {
            this.ProfileData.SMPWinChallengeActivationTime = value;
        }
    }

    public int NumberOfPrizeCardRemaining
    {
        get
        {
            return ProfileData.NumberOfPrizeCardRemaining;
        }
        set
        {
            ProfileData.NumberOfPrizeCardRemaining = value;
        }
    }


	public int NumberOfStargazerMoments
	{
		get
		{
			return ProfileData.NumberOfStargazerMoments;
		}
		set
		{
			ProfileData.NumberOfStargazerMoments = value;
		}
	}

	public bool MultiplayerTutorial_HasSeenModeSelectScreen
	{
		get
		{
			return ProfileData.MultiplayerTutorial_HasSeenModeSelectScreen;
		}
		set
		{
			ProfileData.MultiplayerTutorial_HasSeenModeSelectScreen = value;
		}
	}

	public bool MultiplayerTutorial_HasSeenAnyEvent
	{
		get
		{
			return ProfileData.MultiplayerTutorial_HasSeenAnyEvent;
		}
		set
		{
			ProfileData.MultiplayerTutorial_HasSeenAnyEvent = value;
		}
	}

	public bool MultiplayerTutorial_VersusRaceTeamCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_VersusRaceTeamCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_VersusRaceTeamCompleted = value;
		}
	}

	public bool MultiplayerTutorial_SuccessfullyCompletedStreak
	{
		get
		{
			return ProfileData.MultiplayerTutorial_SuccessfullyCompletedStreak;
		}
		set
		{
			ProfileData.MultiplayerTutorial_SuccessfullyCompletedStreak = value;
		}
	}

	public bool MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted = value;
		}
	}

	public bool MultiplayerTutorial_TracksideUpgradesCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_TracksideUpgradesCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_TracksideUpgradesCompleted = value;
		}
	}

	public bool MultiplayerTutorial_FirstPrizeCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_FirstPrizeCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_FirstPrizeCompleted = value;
		}
	}

	public bool MultiplayerTutorial_CardsButNoCarPartCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_CardsButNoCarPartCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_CardsButNoCarPartCompleted = value;
		}
	}

	public bool MultiplayerTutorial_EliteClubCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_EliteClubCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_EliteClubCompleted = value;
		}
	}

	public bool MultiplayerTutorial_LostWinStreakCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_LostWinStreakCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_LostWinStreakCompleted = value;
		}
	}

	public bool MultiplayerTutorial_MapRaceTeamCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_MapRaceTeamCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_MapRaceTeamCompleted = value;
		}
	}

	public bool MultiplayerTutorial_FirstCarPartCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_FirstCarPartCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_FirstCarPartCompleted = value;
		}
	}

	public bool MultiplayerTutorial_PrizeScreenCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_PrizeScreenCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_PrizeScreenCompleted = value;
		}
	}

	public bool MultiplayerTutorial_HasSeenRespectScreen
	{
		get
		{
			return ProfileData.MultiplayerTutorial_HasSeenRespectScreen;
		}
		set
		{
			ProfileData.MultiplayerTutorial_HasSeenRespectScreen = value;
		}
	}

	public bool MultiplayerTutorial_ElitePassCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_ElitePassCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_ElitePassCompleted = value;
		}
	}

	public bool MultiplayerTutorial_EliteCarCompleted
	{
		get
		{
			return ProfileData.MultiplayerTutorial_EliteCarCompleted;
		}
		set
		{
			ProfileData.MultiplayerTutorial_EliteCarCompleted = value;
		}
	}

	public int MultiplayerConsumableUpsellCount
	{
		get
		{
			return ProfileData.MultiplayerConsumableUpsellCount;
		}
		set
		{
			ProfileData.MultiplayerConsumableUpsellCount = value;
		}
	}

	public bool FriendTutorial_HasSupressedTutorial
	{
		get
		{
			return ProfileData.FriendTutorial_HasSupressedTutorial;
		}
		set
		{
			ProfileData.FriendTutorial_HasSupressedTutorial = value;
		}
	}

	public bool FriendTutorial_HasSeenFriendsPane
	{
		get
		{
			return ProfileData.FriendTutorial_HasSeenFriendsPane;
		}
		set
		{
			ProfileData.FriendTutorial_HasSeenFriendsPane = value;
		}
	}

	public bool SharingTutorial_VideoShareCompleted
	{
		get
		{
			return ProfileData.SharingTutorial_VideoShareCompleted;
		}
		set
		{
			ProfileData.SharingTutorial_VideoShareCompleted = value;
		}
	}

	public int NumberOfSportsCarPiecesRemaining
	{
		get
		{
			return ProfileData.NumberOfSportsCarPiecesRemaining;
		}
		set
		{
			ProfileData.NumberOfSportsCarPiecesRemaining = value;
		}
	}

	public int NumberOfDesiribleCarPiecesRemaining
	{
		get
		{
			return ProfileData.NumberOfDesiribleCarPiecesRemaining;
		}
		set
		{
			ProfileData.NumberOfDesiribleCarPiecesRemaining = value;
		}
	}

	public int NumberOfCommonCarPiecesRemaining
	{
		get
		{
			return ProfileData.NumberOfCommonCarPiecesRemaining;
		}
		set
		{
			ProfileData.NumberOfCommonCarPiecesRemaining = value;
		}
	}

	public int NumberOfTinyCashRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfTinyCashRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfTinyCashRewardsRemaining = value;
		}
	}

	public int NumberOfSmallCashRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfSmallCashRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfSmallCashRewardsRemaining = value;
		}
	}

	public int NumberOfMediumCashRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfMediumCashRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfMediumCashRewardsRemaining = value;
		}
	}

	public int NumberOfLargeCashRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfLargeCashRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfLargeCashRewardsRemaining = value;
		}
	}

	public int NumberOfHugeCashRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfHugeCashRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfHugeCashRewardsRemaining = value;
		}
	}

	public int NumberOfTinyGoldRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfTinyGoldRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfTinyGoldRewardsRemaining = value;
		}
	}

	public int NumberOfSmallGoldRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfSmallGoldRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfSmallGoldRewardsRemaining = value;
		}
	}

	public int NumberOfMediumGoldRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfMediumGoldRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfMediumGoldRewardsRemaining = value;
		}
	}

	public int NumberOfLargeGoldRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfLargeGoldRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfLargeGoldRewardsRemaining = value;
		}
	}

	public int NumberOfHugeGoldRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfHugeGoldRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfHugeGoldRewardsRemaining = value;
		}
	}


    public int NumberOfTinyKeyRewardsRemaining
    {
        get
        {
            return ProfileData.NumberOfTinyKeyRewardsRemaining;
        }
        set
        {
            ProfileData.NumberOfTinyKeyRewardsRemaining = value;
        }
    }

    public int NumberOfSmallKeyRewardsRemaining
    {
        get
        {
            return ProfileData.NumberOfSmallKeyRewardsRemaining;
        }
        set
        {
            ProfileData.NumberOfSmallKeyRewardsRemaining = value;
        }
    }

    public int NumberOfMediumKeyRewardsRemaining
    {
        get
        {
            return ProfileData.NumberOfMediumKeyRewardsRemaining;
        }
        set
        {
            ProfileData.NumberOfMediumKeyRewardsRemaining = value;
        }
    }

    public int NumberOfLargeKeyRewardsRemaining
    {
        get
        {
            return ProfileData.NumberOfLargeKeyRewardsRemaining;
        }
        set
        {
            ProfileData.NumberOfLargeKeyRewardsRemaining = value;
        }
    }

    public int NumberOfHugeKeyRewardsRemaining
    {
        get
        {
            return ProfileData.NumberOfHugeKeyRewardsRemaining;
        }
        set
        {
            ProfileData.NumberOfHugeKeyRewardsRemaining = value;
        }
    }

	public int NumberOfUpgradeRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfUpgradeRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfUpgradeRewardsRemaining = value;
		}
	}

	public int NumberOfFuelRefillsRemaining
	{
		get
		{
			return ProfileData.NumberOfFuelRefillsRemaining;
		}
		set
		{
			ProfileData.NumberOfFuelRefillsRemaining = value;
		}
	}

	public int NumberOfFuelPipsRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfFuelPipsRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfFuelPipsRewardsRemaining = value;
		}
	}

	public int NumberOfTinyRPRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfTinyRPRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfTinyRPRewardsRemaining = value;
		}
	}

	public int NumberOfSmallRPRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfSmallRPRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfSmallRPRewardsRemaining = value;
		}
	}

	public int NumberOfMediumRPRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfMediumRPRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfMediumRPRewardsRemaining = value;
		}
	}

	public int NumberOfLargeRPRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfLargeRPRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfLargeRPRewardsRemaining = value;
		}
	}

	public int NumberOfHugeRPRewardsReamining
	{
		get
		{
			return ProfileData.NumberOfHugeRPRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfHugeRPRewardsRemaining = value;
		}
	}

	public int NumberOfProTunerRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfProTunerRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfProTunerRewardsRemaining = value;
		}
	}

	public int NumberOfN20ManiacRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfN20ManiacRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfN20ManiacRewardsRemaining = value;
		}
	}

	public int NumberOfTireCrewRewardsRemaining
	{
		get
		{
			return ProfileData.NumberOfTireCrewRewardsRemaining;
		}
		set
		{
			ProfileData.NumberOfTireCrewRewardsRemaining = value;
		}
	}

	public int MultiplayerConsumableMapUpsellCount
	{
		get
		{
			return ProfileData.MultiplayerConsumableMapUpsellCount;
		}
		set
		{
			ProfileData.MultiplayerConsumableMapUpsellCount = value;
		}
	}

	public int MultiplayerConsumableMapUpsellStreaksLost
	{
		get
		{
			return ProfileData.MultiplayerConsumableMapUpsellStreaksLost;
		}
		set
		{
			ProfileData.MultiplayerConsumableMapUpsellStreaksLost = value;
		}
	}

	public int SeasonLastPlayedLeaderboardID
	{
		get
		{
			return ProfileData.SeasonLastPlayedLeaderboardID;
		}
		set
		{
			ProfileData.SeasonLastPlayedLeaderboardID = value;
		}
	}

	public int SeasonLastPlayedEventID
	{
		get
		{
			return ProfileData.SeasonLastPlayedEventID;
		}
		set
		{
			ProfileData.SeasonLastPlayedEventID = value;
		}
	}

	public string UDID
	{
		get
		{
			return ProfileData.UDID;
		}
		set
		{
			ProfileData.UDID = value;
		}
	}

	public string UUID
	{
		get
		{
			return ProfileData.UUID;
		}
		set
		{
			ProfileData.UUID = value;
		}
	}

	public bool HasSeenRPBonusPopup
	{
		get
		{
			return ProfileData.Multiplayer_HasSeenRPBonusPopup;
		}
		set
		{
			ProfileData.Multiplayer_HasSeenRPBonusPopup = value;
		}
	}

	public int PeakFriends
	{
		get
		{
			return ProfileData.PeakFriends;
		}
	}

	public eCarTier HighestCrossPromotionTierAdvertSeen
	{
		get
		{
			return (eCarTier)ProfileData.HighestCrossPromotionTierAdvertSeen;
		}
		set
		{
			ProfileData.HighestCrossPromotionTierAdvertSeen = (int)value;
		}
	}

	public bool TwoStarTimeConditionShown
	{
		get
		{
			return ProfileData.TwoStarTimeConditionShown;
		}
		set
		{
			ProfileData.TwoStarTimeConditionShown = value;
		}
	}

	public bool ThreeStarTimeConditionShown
	{
		get
		{
			return ProfileData.ThreeStarTimeConditionShown;
		}
		set
		{
			ProfileData.ThreeStarTimeConditionShown = value;
		}
	}

	public bool HasSeenFriendsWonAllCarsConditionPopup
	{
		get
		{
			return ProfileData.HasSeenWonAllCarsConditionPopup;
		}
		set
		{
			ProfileData.HasSeenWonAllCarsConditionPopup = value;
		}
	}

	public int LastTierUnlockConditionShown
	{
		get
		{
			return ProfileData.LastTierUnlockConditionShown;
		}
		set
		{
			ProfileData.LastTierUnlockConditionShown = value;
		}
	}

	public HashSet<KeyValuePair<int, string>> RaceWithFriendConditionCombos
	{
		get
		{
			return ProfileData.RaceWithFriendConditionCombos;
		}
		set
		{
			ProfileData.RaceWithFriendConditionCombos = value;
		}
	}

	public int BeatYourBestConditionShwonCount
	{
		get
		{
			return ProfileData.BeatYourBestConditionShwonCount;
		}
		set
		{
			ProfileData.BeatYourBestConditionShwonCount = value;
		}
	}

	public int InviteFriendsConditionShownCount
	{
		get
		{
			return ProfileData.InviteFriendsConditionShownCount;
		}
		set
		{
			ProfileData.InviteFriendsConditionShownCount = value;
		}
	}

	public int RaceTooHardConditionShownCount
	{
		get
		{
			return ProfileData.RaceTooHardConditionShownCount;
		}
		set
		{
			ProfileData.RaceTooHardConditionShownCount = value;
		}
	}

    public DateTime DailyPrizeCardLastEventAt
    {
        get
        {
            return ProfileData.DailyPrizeCardLastEventAt;
        }
        set
        {
            ProfileData.DailyPrizeCardLastEventAt = value;
        }
    }
    
    public DateTime AppTuttiTimedRewardLastEventAt 
    {
        get
        {
            return ProfileData.AppTuttiTimedRewardLastEventAt;
        }
        set
        {
            ProfileData.AppTuttiTimedRewardLastEventAt = value;
        }
    }
    
    public DateTime VasTimedRewardLastEventAt 
    {
	    get
	    {
		    return ProfileData.VasTimedRewardLastEventAt;
	    }
	    set
	    {
		    ProfileData.VasTimedRewardLastEventAt = value;
	    }
    }

	public DateTime InviteFriendsConditionLastTimeShown
	{
		get
		{
			return ProfileData.InviteFriendsConditionLastTimeShown;
		}
		set
		{
			ProfileData.InviteFriendsConditionLastTimeShown = value;
		}
	}

	public DateTime BuyACarLastConditionLastTimeShown
	{
		get
		{
			return ProfileData.BuyACarLastConditionLastTimeShown;
		}
		set
		{
			ProfileData.BuyACarLastConditionLastTimeShown = value;
		}
	}

	public DateTime RaceWithFriendConditionLastTimeShown
	{
		get
		{
			return ProfileData.RaceWithFriendConditionLastTimeShown;
		}
		set
		{
			ProfileData.RaceWithFriendConditionLastTimeShown = value;
		}
	}

	public DateTime FriendsEasyRaceConditionLastTimeShown
	{
		get
		{
			return ProfileData.FriendsEasyRaceConditionLastTimeShown;
		}
		set
		{
			ProfileData.FriendsEasyRaceConditionLastTimeShown = value;
		}
	}

	public bool HasSeenRenewWholeTeamIAPCondition
	{
		get
		{
			return ProfileData.HasSeenRenewWholeTeamIAPCondition;
		}
		set
		{
			ProfileData.HasSeenRenewWholeTeamIAPCondition = value;
		}
	}

	public DateTime AllHardRacesConditionLastTimeShown
	{
		get
		{
			return ProfileData.AllHardRacesConditionLastTimeShown;
		}
		set
		{
			ProfileData.AllHardRacesConditionLastTimeShown = value;
		}
	}

    public bool HasUpgradedFuelTank
    {
        get
        {
            return ProfileData.HasUpgradedFuelTank ||
                   (UserManager.Instance != null && UserManager.Instance.currentAccount != null
                                                 && UserManager.Instance.currentAccount.HasUpgradedFuelTank);
        }
        set
        {
            ProfileData.HasUpgradedFuelTank = value;
            if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
                UserManager.Instance.currentAccount.HasUpgradedFuelTank = value;

        }
    }

    public bool HasReceivedFuelTankUpgradeRefill
	{
		get
		{
			return ProfileData.HasReceivedFuelTankUpgradeRefill;
		}
		set
		{
			ProfileData.HasReceivedFuelTankUpgradeRefill = value;
		}
	}

	public int GasTankMessageNumberOfTimesShown
	{
		get
		{
			return ProfileData.GasTankReminderNumberOfTimesShown;
		}
	}

	public int GasTankReminderIDShown
	{
		get
		{
			return ProfileData.GasTankReminderIDShown;
		}
	}

	public bool HasPaidForSomething
	{
		get
		{
		    return HasUpgradedFuelTank || UnlimitedFuelMinutesPurchased > 0 || PlayerBoughtConsumable ||
            (UserManager.Instance != null && UserManager.Instance.currentAccount != null && UserManager.Instance.currentAccount.UserBoughtConsumables);
		}
	}

    public bool PlayerBoughtConsumable
    {
        get { return ProfileData.GoldBought>0 || ProfileData.CashBought>0; }
    }

    public bool DailyBattlePromptEnabled
	{
		get
		{
			return ProfileData.DailyBattlePromptToggle;
		}
	}

	public int RacesSinceLastCarDeal
	{
		get
		{
			return ProfileData.CarDeals.RacesSinceLastDeal;
		}
	}

	public string LastCarDealCarOffered
	{
		get
		{
			return ProfileData.CarDeals.LastCarOffered;
		}
	}

	public int LastCarDealDiscount
	{
		get
		{
			return ProfileData.CarDeals.LastDiscount;
		}
	}

	public int LastCarDealDiscountRepeatCount
	{
		get
		{
			return ProfileData.CarDeals.LastDiscountRepeatCount;
		}
	}

	public bool LastCarDealWasCashback
	{
		get
		{
			return ProfileData.CarDeals.LastDealWasCashback;
		}
	}

	public DateTime DailyBattlesLastEventAt
	{
		get
		{
			return ProfileData.DailyBattlesLastEventAt;
		}
		set
		{
			ProfileData.DailyBattlesLastEventAt = value;
		}
	}

	public int DailyBattlesConsecutiveDaysCount
	{
		get
		{
			return ProfileData.DailyBattlesConsecutiveDaysCount;
		}
		set
		{
			ProfileData.DailyBattlesConsecutiveDaysCount = value;
		}
	}

	public bool DailyBattlesWonLast
	{
		get
		{
			return ProfileData.DailyBattlesWonLast;
		}
		set
		{
			ProfileData.DailyBattlesWonLast = value;
		}
	}

	public int DailyBattlesDoneToday
	{
		get
		{
			return ProfileData.DailyBattlesDoneToday;
		}
	}

	public DateTime LastBundleOfferTimeShown
	{
		get
		{
			return ProfileData.LastBundleOfferTimeShown;
		}
		set
		{
			ProfileData.LastBundleOfferTimeShown = value;
		}
	}

	public string DefaultBranch
	{
		get
		{
			return ProfileData.DefaultBranch;
		}
		set
		{
			ProfileData.DefaultBranch = value;
		}
	}
	
	public string A_Branch
	{
		get
		{
			return ProfileData.A_Branch;
		}
		set
		{
			ProfileData.A_Branch = value;
		}
	}
	
	public string B_Branch
	{
		get
		{
			return ProfileData.B_Branch;
		}
		set
		{
			ProfileData.B_Branch = value;
		}
	}
	
	public bool IsDefaultBranch
	{
		get
		{
			return ProfileData.IsDefaultBranch;
		}
		set
		{
			ProfileData.IsDefaultBranch = value;
		}
	}
	
	public bool IsA_Branch
	{
		get
		{
			return ProfileData.IsA_Branch;
		}
		set
		{
			ProfileData.IsA_Branch = value;
		}
	}
	
	public bool IsB_Branch
	{
		get
		{
			return ProfileData.IsB_Branch;
		}
		set
		{
			ProfileData.IsB_Branch = value;
		}
	}

	public int GPGSignInCancellations
	{
		get
		{
 #if UNITY_EDITOR
			return PlayerPrefs.GetInt("getGPGSignInCount");
#elif UNITY_ANDROID
            return AndroidSpecific.GetGPGSignInCount();
#else
			return PlayerPrefs.GetInt("getGPGSignInCount");
#endif
		}
		set
		{
#if UNITY_EDITOR
			PlayerPrefs.SetInt("getGPGSignInCount", value);
#elif UNITY_ANDROID
            AndroidSpecific.SetGPGSignInCount(value);
#else
			PlayerPrefs.SetInt("getGPGSignInCount", value); 
#endif

		}
	}

	public bool HasSeenWorldTourIntro
	{
		get
		{
			return ProfileData.HasSeenWorldTourIntroduction;
		}
		set
		{
			ProfileData.HasSeenWorldTourIntroduction = value;
		}
	}

	public int SeenWorldTourLockedIntroCount
	{
		get
		{
			return ProfileData.SeenWorldTourLockedIntroCount;
		}
		set
		{
			ProfileData.SeenWorldTourLockedIntroCount = value;
		}
	}

	public bool OverrideRYFObjectives
	{
		get
		{
			return ProfileData.OverrideRYFObjectives;
		}
		set
		{
			ProfileData.OverrideRYFObjectives = value;
		}
	}

	public bool OverrideRTWObjectives
	{
		get
		{
			return ProfileData.OverrideRTWObjectives;
		}
		set
		{
			ProfileData.OverrideRTWObjectives = value;
		}
	}

	public int LastPlayedMultiplayerLeaderboardID
	{
		get
		{
			return ProfileData.LastPlayedMultiplayerLeaderboardID;
		}
		set
		{
			ProfileData.LastPlayedMultiplayerLeaderboardID = value;
		}
	}

	public bool HasVisitedMechanicScreen
	{
		get
		{
			return ProfileData.HasVisitedMechanicScreen || LegacyObjectivesCompleted.Contains(10521);
		}
		set
		{
			ProfileData.HasVisitedMechanicScreen = value;
		}
	}

	public bool HasVisitedManufacturerScreen
	{
		get
		{
			return ProfileData.HasVisitedManufacturerScreen;
		}
		set
		{
			ProfileData.HasVisitedManufacturerScreen = value;
		}
	}

	public bool HasOfferedCrewRacesCars
	{
		get
		{
			return ProfileData.HasOfferedCrewRacesCars;
		}
		set
		{
			ProfileData.HasOfferedCrewRacesCars = value;
		}
	}

	public bool HasSeenFacebookNag
	{
		get
		{
			return ProfileData.HasSeenFacebookNag;
		}
		set
		{
			ProfileData.HasSeenFacebookNag = value;
		}
	}

	public bool HasFinishedRYFTutorial
	{
		get
		{
			return ProfileData.HasFinishedRYFTutorial;
		}
		set
		{
			ProfileData.HasFinishedRYFTutorial = value;
		}
	}

	public bool HasSeenMultiplayerIntroScreen
	{
		get
		{
			return ProfileData.HasSeenMultiplayerIntroScreen;
		}
		set
		{
			ProfileData.HasSeenMultiplayerIntroScreen = value;
		}
	}

	public bool HasSeenInternationalIntroScreen
	{
		get
		{
			return ProfileData.HasSeenInternationalIntroScreen;
		}
		set
		{
			ProfileData.HasSeenInternationalIntroScreen = value;
		}
	}

	public int MapPaneSelected
	{
		get
		{
			return ProfileData.MapPaneSelected;
		}
		set
		{
			ProfileData.MapPaneSelected = value;
		}
	}

    public bool UseMileAsUnit
    {
        get { return ProfileData.UseMileAsUnit; }
        set { ProfileData.UseMileAsUnit = value; }
    }


    public string ImageUrl
    {
        get { return ProfileData.ImageUrl; }
        set { ProfileData.ImageUrl = value; }
    }

    public int LastBundleOfferPopupID
    {
        get
        {
            return this.ProfileData.BundleOfferData.LastBundleOfferedPopupID;
        }
        set
        {
            this.ProfileData.BundleOfferData.LastBundleOfferedPopupID = value;
        }
    }

    public List<int> QueuedOffersIDs
    {
        get
        {
            return this.ProfileData.BundleOfferData.QueuedOffersIDs;
        }
        set
        {
            this.ProfileData.BundleOfferData.QueuedOffersIDs = value;
        }
    }

    public IEnumerable<MultiplayerEventRPBonus> MultiplayerEventRPBonuses
    {
        get
        {
            return this.ProfileData.MultiplayerEventsData.GetRPBonuses();
        }
    }

	public bool CurrentCarHasNewLiveries
	{
		get
		{
			CarGarageInstance currentCar = GetCurrentCar();
			return currentCar != null && currentCar.NewLiveries.Count > 0;
		}
	}

	public bool DebugWelcomeDismissed
	{
		get;
		set;
	}

	public bool FirstPurchaseDone
	{
		get { return ProfileData.FirstPurchaseDone; }
		set { ProfileData.FirstPurchaseDone = value; }
	}
	
	public bool FirstEnterBeginnerRegulation
	{
		get { return this.ProfileData.FirstEnterBeginnerRegulation; }
		set {  this.ProfileData.FirstEnterBeginnerRegulation = value; }
	}
	
	public bool FirstEnterNormalRegulation
	{
		get { return  this.ProfileData.FirstEnterNormalRegulation; }
		set {  this.ProfileData.FirstEnterNormalRegulation = value; }
	}
	
	public bool FirstEnterHardRegulation
	{
		get { return ProfileData.FirstEnterHardRegulation; }
		set { ProfileData.FirstEnterHardRegulation = value; }
	}

	public bool FirstRewardedAdWatched
	{
		get { return ProfileData.FirstRewardedAdWatched; }
		set { ProfileData.FirstRewardedAdWatched = value; }
	}

	public bool FirstInterstitialAdWatched
	{
		get { return ProfileData.FirstInterstitialAdWatched; }
		set { ProfileData.FirstInterstitialAdWatched = value; }
	}

	public PlayerProfile(string name)
	{
		Name = name;
	}

	public void CreateDefault()
	{
		ProfileData = new PlayerProfileData();
		ProfileData.Reset();
		SessionRacesCompleted = 0;
		SessionDailyBattleRaces = 0;
		UseMileAsUnit = ShouldUseMileAsUnit();
	}

	private bool ShouldUseMileAsUnit()
	{
		var imperialCountries = new[] { "US","GB", "MM", "LR"  };
		var countryCode = UserManager.Instance.currentAccount.CountryCode;
		return imperialCountries.Contains(countryCode);
	}

	public string DisplayNameWithYOUFallback()
	{
		return NameValidater.ReplaceUnsupportedCharacters(GetDisplayName(LocalizationManager.GetTranslation("TEXT_REWARDS_YOU")));
	}

	public string DisplayNameWithUserNameFallback()
	{
		return NameValidater.ReplaceUnsupportedCharacters(GetDisplayName(PlayerProfileManager.Instance.ActiveProfile.DisplayName));
	}

	public AvatarPicture.eAvatarType GetAvatarTypeToDisplayForDefault()
	{
        //if (this.CanUseFacebookProfile() && !SocialController.Instance.facebookPictureIsSilhouette)
        //{
        //    return AvatarPicture.eAvatarType.FACEBOOK_AVATAR;
        //}
		if (CanUseGamecenterProfile())
		{
			return AvatarPicture.eAvatarType.GAME_CENTER_AVATAR;
		}
		if (CanUseGooglePlayProfile())
		{
			return AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR;
		}
		return AvatarPicture.eAvatarType.CSR_AVATAR;
	}

	public UserNameType GetNameTypeToDisplayForDefault()
	{
		if (CanUseFacebookProfile())
		{
			return UserNameType.Facebook;
		}
		if (CanUseGamecenterProfile())
		{
			return UserNameType.Gamecenter;
		}
		if (CanUseGooglePlayProfile())
		{
			return UserNameType.GooglePlay;
		}
		return UserNameType.CSR;
	}

	private bool CanUseFacebookProfile()
	{
	    return false;//SocialController.Instance.isLoggedIntoFacebook && !string.IsNullOrEmpty(SocialController.Instance.GetFacebookName());
	}

	private bool TryUseFacebookProfileName(out string name)
	{
		name = string.Empty;
		if (CanUseFacebookProfile())
		{
		    string facebookNameWithUnsupportedCharacters = null;//SocialController.Instance.GetFacebookNameWithUnsupportedCharacters();
			if (NameValidater.CanNameBeDisplayedInCurrentLanguage(facebookNameWithUnsupportedCharacters))
			{
				name = facebookNameWithUnsupportedCharacters;
				return true;
			}
		}
		return false;
	}

	private bool CanUseGamecenterProfile()
	{
	    return false;//GameCenterController.Instance.isPlayerLoggedIn();
	}

	private bool CanUseGooglePlayProfile()
	{
	    return false;// GooglePlayGamesController.Instance.IsPlayerAuthenticated();
	}

	private bool TryUseGamecenterProfileName(out string name)
	{
		name = string.Empty;
        //if (this.CanUseGamecenterProfile() && GameCenterController.Instance.currentAliasCanBeDisplayed())
        //{
        //    name = GameCenterController.Instance.currentAlias();
        //    return true;
        //}
		return false;
	}

	private bool TryUseGooglePlayProfileName(out string name)
	{
		name = string.Empty;
		if (CanUseGooglePlayProfile())
		{
            //name = NameValidater.ReplaceUnsupportedCharacters(GooglePlayGamesController.Instance.GetCurrentAlias());
			return true;
		}
		return false;
	}

	private string GetDisplayName(string defaultName)
	{
		string result;
		if (TryUseFacebookProfileName(out result))
		{
			return result;
		}
		if (TryUseGamecenterProfileName(out result))
		{
			return result;
		}
		if (TryUseGooglePlayProfileName(out result))
		{
			return result;
		}
		return defaultName;
	}

	public void Rename(string name)
	{
		Name = name;
		Save();
	}

    public void Save()
    {
        this.ProfileData.Username = this.Name;
        this.DataToSetOnSave();
        string json = PlayerProfileMapper.ToJson(this.ProfileData);
        PlayerProfileFile.WriteFile(this.Name, json,EProfileFileType.account);
    }
    
    public string GetProfileJson()
	{
	    return PlayerProfileMapper.ToJson(this.ProfileData);
	}

	private void DataToSetOnSave()
	{
		if (ProfileData.UnconfirmedDailyBattleResults.Any())
		{
			bool flag = UpdateDailyBattleRewardsFromNetworkTime();
			if (flag)
			{
				PopUp pop = new PopUp
				{
					Title = "TEXT_DAILYBATTLE_CLOCK_CHEAT_MESSAGE_TITLE",
					BodyText = "TEXT_DAILYBATTLE_CLOCK_CHEAT_MESSAGE",
					IsBig = true,
					ConfirmText = "TEXT_BUTTON_OK",
                    ImageCaption = "TEXT_NAME_AGENT",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab
                };
				PopUpManager.Instance.SchedulePopUp(pop, new HashSet<ScreenID>
				{
					ScreenID.Home,
					ScreenID.CareerModeMap,
					ScreenID.Workshop
				});
				ProfileData.DailyBattlesConsecutiveDaysCount = 1;
                ProfileData.DailyBattlesLastEventAt = GTDateTime.Now;
			}
		}

        if (Application.isPlaying)
        {
            ProfileData.dateTimeLastSaved = GTDateTime.Now;
            ProfileData.ProductVersionLastSaved = ApplicationVersion.Current;
        }

	}

	public bool Load(EProfileFileType fileType)
	{
		string empty = string.Empty;
		if (!PlayerProfileFile.ReadFile(Name, ref empty, fileType))
		{
			return false;
		}
		if (empty.Length == 0)
		{
			return false;
		}
        this.ProfileData = PlayerProfileMapper.FromJson(empty);
		if (ProfileData == null)
		{
			return false;
		}
        //CheckProfileHasUpdated();
        if (!ProfileData.NewProfile)
		{
			CreateDefault();
		}
		if (ProfileData.Username != Name)
		{
			CreateDefault();
		}
		EnableDailyBattlePrompt();
		return true;
	}

	public bool LoadFromProfileData(PlayerProfileData profileData)
	{
	    this.ProfileData = profileData;
	    ValidateServerData();


#if GIVE_ALL_CARS
	    GivePlayerAllCars();
#endif

        profileData.UpdatePlayerLevel();

        //We should store player league and read it from database next time
        //Because player league maybe downgraded.Note:For further info look at StarStats Class
        //profileData.UpdatePlayerLeague();//We never use this method

        //if (ProfileData == null)
        //{
        //    return false;
        //}
        //CheckProfileHasUpdated();
        //if (!ProfileData.NewProfile)
        //{
        //    CreateDefault();
        //}
        //ProfileData.Username = Name;
        EnableDailyBattlePrompt();
		return true;
	}


    private void ValidateServerData()
    {
        if (ProfileData.CarsOwned.Count == 0)
        {
            ProfileData.CurrentlySelectedCarDBKey = null;
            ProfileData.HasBoughtFirstCar = false;
        }
    }

    private void GivePlayerAllCars()
    {
        CarsOwned = new List<CarGarageInstance>();
        var cars = CarDatabase.Instance.GetAllCars();
        for (int i = 0; i < cars.Count; i++)
        {
            var car = new CarGarageInstance()
            {
                CarDBKey = cars[i].Key,
                CurrentPPIndex = cars[i].BasePerformanceIndex
            };
            CarsOwned.Add(car);
        }
    }

    public void CheckProfileHasUpdated()
	{
        if (ProfileHasUpdatedOverVersion("1.8.3.194"))
        {
            UpdateProfile();
        }
	}


    private void UpdateProfile()
    {
        if (!Application.isPlaying)
            return;
        if (ProfileData.EventsCompleted.Contains(41041) && !ProfileData.EventsCompleted.Contains(41031))
        {
            ProfileData.EventsCompleted.Add(41031);
        }
        if (ProfileData.EventsCompleted.Contains(42041) && !ProfileData.EventsCompleted.Contains(42031))
        {
            ProfileData.EventsCompleted.Add(42031);
        }
        if (ProfileData.EventsCompleted.Contains(43041) && !ProfileData.EventsCompleted.Contains(43031))
        {
            ProfileData.EventsCompleted.Add(43031);
        }
        if (ProfileData.EventsCompleted.Contains(44041) && !ProfileData.EventsCompleted.Contains(44031))
        {
            ProfileData.EventsCompleted.Add(44031);
        }
        if (ProfileData.EventsCompleted.Contains(45041) && !ProfileData.EventsCompleted.Contains(45031))
        {
            ProfileData.EventsCompleted.Add(45031);
        }

        var carsOwned = ProfileData.CarsOwned;

        var newCarList = new List<CarGarageInstance>();
        foreach (var carGarageInstance in carsOwned)
        {
            if (CarMigrationMapper.CarMapperDictionary.ContainsKey(carGarageInstance.CarDBKey))
            {
                var mappedCar = CarMigrationMapper.CarMapperDictionary[carGarageInstance.CarDBKey];
                if (carsOwned.All( c => c.CarDBKey != mappedCar))
                {
                    var carOwnedInstance = carsOwned.FirstOrDefault(c => c.CarDBKey == carGarageInstance.CarDBKey);
                    var car = CarDatabase.Instance.GetCar(mappedCar);
                    var newCar = new CarGarageInstance()
                    {
                        CarDBKey = mappedCar,
                    };
                    newCar.SetupNewGarageInstance(car);
                    foreach (var carUpgradeStatuse in carOwnedInstance.UpgradeStatus)
                    {
                        newCar.UpgradeStatus[carUpgradeStatuse.Key].levelFitted = carUpgradeStatuse.Value.levelFitted;
                        newCar.UpgradeStatus[carUpgradeStatuse.Key].levelOwned = carUpgradeStatuse.Value.levelOwned;
                    }
                    newCarList.Add(newCar);
                }
            }
        }

        ProfileData.CarsOwned.AddRange(newCarList);
    }


    private bool ProfileHasUpdatedOverVersion(string criticalVersion)
	{
		string productVersionLastSaved = ProfileData.ProductVersionLastSaved;
		string applicationVersion = BasePlatform.ActivePlatform.GetApplicationVersion();
		return ApplicationVersion.Compare(productVersionLastSaved, criticalVersion) == -1 && ApplicationVersion.Compare(applicationVersion, criticalVersion) >= 0;
	}

	private void SetProfileTimeDateStampsToUTC()
	{
        DateTime dateTime = GTDateTime.Now.AddDays(-1.0);
        TimeSpan value = (GTDateTime.Now - GTDateTime.Now).Duration();
		ProfileData.FacebookInviteFuelRewardsTime = dateTime;
		ProfileData.LastAgentNag = dateTime;
		ProfileData.LastFuelAutoReplenishedTime = dateTime;
		ProfileData.LastUpgradeDateTimeNag = dateTime;
		ProfileData.UserStartedPlaying = ProfileData.UserStartedPlaying.Add(value);
		ProfileData.TwitterCashRewardsTime = dateTime;
		ProfileData.TwitterInviteFuelRewardsTime = dateTime;
		for (int i = 0; i < ProfileData.arrivalQueue.Count; i++)
		{
			ProfileData.arrivalQueue[i].dueTime = dateTime;
		}
		NotificationManager.Active.ClearAllNotifications();
	}

	public void SetLegalAgreementVersion(int version)
	{
		ProfileData.LastLegalAgreementVersion = version;
	}

	public override string ToString()
	{
		string str = string.Empty;
		str = str + "Name: " + Name + "\n";
		return str + ProfileData;
	}

	public bool HasSetTimeInCar(string carDBKey)
	{
		return ProfileData.BestCarTimes.ContainsKey(carDBKey);
	}

	public bool HasCollectedFriendRewardForCarThreeStar(string carDBKey)
	{
		int carNumberID = CarDatabase.Instance.GetCar(carDBKey).CarNumberID;
		return ProfileData.FriendsRewardCollectedForCars.Contains(carNumberID);
	}

	public void CollectedFriendRewardForCarThreeStar(string carDBKey)
	{
		int carNumberID = CarDatabase.Instance.GetCar(carDBKey).CarNumberID;
		ProfileData.FriendsRewardCollectedForCars.Add(carNumberID);
	}

    public BossChallengeStateEnum GetBossChallengeState(eCarTier Tier)
    {
        switch (Tier)
        {
            case eCarTier.TIER_1:
                return this.ProfileData.BossChallengeStateT1;
            case eCarTier.TIER_2:
                return this.ProfileData.BossChallengeStateT2;
            case eCarTier.TIER_3:
                return this.ProfileData.BossChallengeStateT3;
            case eCarTier.TIER_4:
                return this.ProfileData.BossChallengeStateT4;
            case eCarTier.TIER_5:
                return this.ProfileData.BossChallengeStateT5;
            default:
                return this.ProfileData.BossChallengeStateT1;
        }
    }

    public void SetBossChallengeState(eCarTier Tier, BossChallengeStateEnum state)
    {
        switch (Tier)
        {
            case eCarTier.TIER_1:
                this.ProfileData.BossChallengeStateT1 = state;
                return;
            case eCarTier.TIER_2:
                this.ProfileData.BossChallengeStateT2 = state;
                return;
            case eCarTier.TIER_3:
                this.ProfileData.BossChallengeStateT3 = state;
                return;
            case eCarTier.TIER_4:
                this.ProfileData.BossChallengeStateT4 = state;
                return;
            case eCarTier.TIER_5:
                this.ProfileData.BossChallengeStateT5 = state;
                return;
            default:
                return;
        }
    }

	public bool HasCompletedAnOnlineRace()
	{
		return ProfileData.OnlineRacesLost > 0 || ProfileData.OnlineRacesWon > 0;
	}

    public int GetEventsCompletedInTier(eCarTier tier)
    {
        switch (tier)
        {
            case eCarTier.TIER_1:
                return EventsCompletedTier1;
            case eCarTier.TIER_2:
                return EventsCompletedTier2;
            case eCarTier.TIER_3:
                return EventsCompletedTier3;
            case eCarTier.TIER_4:
                return EventsCompletedTier4;
            case eCarTier.TIER_5:
                return EventsCompletedTier5;
            default:
                return 0;
        }
    }

    public string GetTargetCarModelForCarSpecificEvents(eCarTier carTier)
	{
	    switch (carTier)
	    {
	        case eCarTier.TIER_1:
	            return ProfileData.Tier1CarSpecificEventTarget;
	        case eCarTier.TIER_2:
	            return ProfileData.Tier2CarSpecificEventTarget;
	        case eCarTier.TIER_3:
	            return ProfileData.Tier3CarSpecificEventTarget;
	        case eCarTier.TIER_4:
	            return ProfileData.Tier4CarSpecificEventTarget;
	        case eCarTier.TIER_5:
	            return ProfileData.Tier5CarSpecificEventTarget;
	        default:
	            return "None";
	    }
	}


    public int GetTargetGroupForCarSpecificEvents(eCarTier carTier)
    {
        switch (carTier)
        {
            case eCarTier.TIER_1:
                return ProfileData.Tier1CarSpecificEventTargetIndex;
            case eCarTier.TIER_2:
                return ProfileData.Tier2CarSpecificEventTargetIndex;
            case eCarTier.TIER_3:
                return ProfileData.Tier3CarSpecificEventTargetIndex;
            case eCarTier.TIER_4:
                return ProfileData.Tier4CarSpecificEventTargetIndex;
            case eCarTier.TIER_5:
                return ProfileData.Tier5CarSpecificEventTargetIndex;
            default:
                return 0;
        }
    }

	public void SetTargetCarModelForCarSpecificEvents(eCarTier carTier, string carDBKey)
	{
		switch (carTier)
		{
		case eCarTier.TIER_1:
			ProfileData.Tier1CarSpecificEventTarget = carDBKey;
			break;
		case eCarTier.TIER_2:
			ProfileData.Tier2CarSpecificEventTarget = carDBKey;
			break;
		case eCarTier.TIER_3:
			ProfileData.Tier3CarSpecificEventTarget = carDBKey;
			break;
		case eCarTier.TIER_4:
			ProfileData.Tier4CarSpecificEventTarget = carDBKey;
			break;
		case eCarTier.TIER_5:
			ProfileData.Tier5CarSpecificEventTarget = carDBKey;
			break;
		}
	}


    public void SetTargetGroupForCarSpecificEvents(eCarTier carTier, int group)
    {
        switch (carTier)
        {
            case eCarTier.TIER_1:
                ProfileData.Tier1CarSpecificEventTargetIndex = group;
                break;
            case eCarTier.TIER_2:
                ProfileData.Tier2CarSpecificEventTargetIndex = group;
                break;
            case eCarTier.TIER_3:
                ProfileData.Tier3CarSpecificEventTargetIndex = group;
                break;
            case eCarTier.TIER_4:
                ProfileData.Tier4CarSpecificEventTargetIndex = group;
                break;
            case eCarTier.TIER_5:
                ProfileData.Tier5CarSpecificEventTargetIndex = group;
                break;
        }
    }

    public string GetTargetManufacturerForManufacturerEvents(eCarTier carTier)
    {
        switch (carTier)
        {
            case eCarTier.TIER_1:
                return ProfileData.Tier1ManufacturerSpecificEventTarget;
            case eCarTier.TIER_2:
                return ProfileData.Tier2ManufacturerSpecificEventTarget;
            case eCarTier.TIER_3:
                return ProfileData.Tier3ManufacturerSpecificEventTarget;
            case eCarTier.TIER_4:
                return ProfileData.Tier4ManufacturerSpecificEventTarget;
            case eCarTier.TIER_5:
                return ProfileData.Tier5ManufacturerSpecificEventTarget;
            default:
                return "None";
        }
    }

    public void SetTargetManufacturerForManufacturerEvents(eCarTier carTier, string manufacturerID)
	{
		switch (carTier)
		{
		case eCarTier.TIER_1:
			ProfileData.Tier1ManufacturerSpecificEventTarget = manufacturerID;
			break;
		case eCarTier.TIER_2:
			ProfileData.Tier2ManufacturerSpecificEventTarget = manufacturerID;
			break;
		case eCarTier.TIER_3:
			ProfileData.Tier3ManufacturerSpecificEventTarget = manufacturerID;
			break;
		case eCarTier.TIER_4:
			ProfileData.Tier4ManufacturerSpecificEventTarget = manufacturerID;
			break;
		case eCarTier.TIER_5:
			ProfileData.Tier5ManufacturerSpecificEventTarget = manufacturerID;
			break;
		}
	}

    public int CarePackageTotalReceivedCount()
    {
        return this.ProfileData.CarePackageData.TotalReceivedCount();
    }

    public int CarePackageTotalReceivedLevelCount(string levelID)
    {
        return this.ProfileData.CarePackageData.TotalReceivedLevelCount(levelID);
    }

    public List<CarePackageReceivedLevelData> GetCarePackageLevelsReceived()
    {
        return this.ProfileData.CarePackageData.GetLevelsReceived();
    }

    public void SetCarePackageInfo(DateTime time, string id, bool displayed)
	{
        this.ProfileData.CarePackageData.Set(time, id, displayed);
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public void IncrementCarePackageReceivedCount(string ID)
	{
        this.ProfileData.CarePackageData.IncrementReceivedCount(ID);
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public void FilterUnusedReceivedCarePackageLevels(HashSet<string> levelIDs)
	{
        this.ProfileData.CarePackageData.FilterUnusedReceivedLevels(levelIDs);
    }

	public bool isYesterday(int zDayOfYear, int currentDayOfYear)
	{
		return currentDayOfYear == zDayOfYear + 1 || (zDayOfYear == 1 && currentDayOfYear >= 365);
	}

    private bool ShouldHaveOneDailyBattlePerDay()
    {
        if (!DailyBattleRewardManager.Instance.CheatsBattleOncePerDay)
        {
            return false;
        }
        return
            ProfileData.UnconfirmedDailyBattleResults.Any(
                (DailyBattleCompletionRecord x) =>
                    GTDateTime.Now - x.When > DailyBattleRewardManager.Instance.TimeBeforeOneRacePerDay);
    }

    public TimeSpan GetTimeUntilNextDailyBattle()
    {

        if (!ServerSynchronisedTime.Instance.ServerTimeValid)
            return TimeSpan.Zero;


        if (ProfileData.DailyBattlesConsecutiveDaysCount == 0)
        {
            return TimeSpan.Zero;
        }

        var now = GTDateTime.Now;
        var reward =
            DailyBattleRewardManager.Instance.GetReward(ProfileData.DailyBattlesConsecutiveDaysCount,
                RaceEventQuery.Instance.getHighestUnlockedClass(), ProfileData.DailyBattlesWonLast);
        var dateTime = ProfileData.DailyBattlesLastEventAt + reward.CooldownTime.TimeSpan;
        var nextDayTime =
            new DateTime(ProfileData.DailyBattlesLastEventAt.Year, ProfileData.DailyBattlesLastEventAt.Month,
                ProfileData.DailyBattlesLastEventAt.Day, 0, 0, 0).AddDays(1.0);
        if (ShouldHaveOneDailyBattlePerDay())
        {
            return nextDayTime - now;
        }
        var timeSpan = (dateTime > nextDayTime) ? (nextDayTime - now) : (dateTime - now);
        //if (timeSpan <= TimeSpan.Zero)
        //{
        //    return TimeSpan.Zero;
        //}
        return timeSpan;
    }

    public bool GetIsNextDailyBattleAfterMidnight()
	{
		if (ProfileData.DailyBattlesConsecutiveDaysCount == 0)
		{
			return false;
		}
		if (ShouldHaveOneDailyBattlePerDay())
		{
			return true;
		}
		DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(ProfileData.DailyBattlesConsecutiveDaysCount, RaceEventQuery.Instance.getHighestUnlockedClass(), ProfileData.DailyBattlesWonLast);
        TimeSpan cooldownTime = reward.CooldownTime.TimeSpan;
		DateTime t = ProfileData.DailyBattlesLastEventAt + cooldownTime;
		DateTime t2 = new DateTime(ProfileData.DailyBattlesLastEventAt.Year, ProfileData.DailyBattlesLastEventAt.Month, ProfileData.DailyBattlesLastEventAt.Day, 0, 0, 0);
		t2 = t2.AddDays(1.0);
		return !(t < t2);
	}

	public int GetDaysSinceLastDailyBattle()
	{
		DateTime now = GTDateTime.Now;
		DateTime d = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
		DateTime d2 = new DateTime(ProfileData.DailyBattlesLastEventAt.Year, ProfileData.DailyBattlesLastEventAt.Month, ProfileData.DailyBattlesLastEventAt.Day, 12, 0, 0);
		return (d - d2).Days;
	}

	public void ResetDailyBattleDaysIfRequired()
	{
		int daysSinceLastDailyBattle = GetDaysSinceLastDailyBattle();
		if (daysSinceLastDailyBattle > 1)
		{
			ProfileData.DailyBattlesConsecutiveDaysCount = 1;
		}
	}

	public void RaceDailyBattle()
	{
	    int daysSinceLastDailyBattle = GetDaysSinceLastDailyBattle();
		if (daysSinceLastDailyBattle == 0)
		{
            //Debug.Log("UpdateDailyBattlesNextRace");
			ProfileData.DailyBattlesDoneToday++;
			DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(ProfileData.DailyBattlesConsecutiveDaysCount, RaceEventQuery.Instance.getHighestUnlockedClass(), ProfileData.DailyBattlesWonLast);
			NotificationManager.Active.UpdateDailyBattlesNextRace(reward.CooldownTime.TimeSpan);
		}
		else if (daysSinceLastDailyBattle == 1)
		{
            //Debug.Log("UpdateDailyBattlesFirstRaceTomorrow");
			ProfileData.DailyBattlesConsecutiveDaysCount++;
			ProfileData.DailyBattlesDoneToday = 1;
			NotificationManager.Active.UpdateDailyBattlesFirstRaceTomorrow();
		}
		else if (daysSinceLastDailyBattle > 1)
		{
			ProfileData.DailyBattlesConsecutiveDaysCount = 1;
			ProfileData.DailyBattlesDoneToday = 1;
		}
		ProfileData.DailyBattlesLastEventAt = GTDateTime.Now;
	}

	private void RegisterQuarterMileTime(float zQuarterMileTime)
	{
		if (zQuarterMileTime < ProfileData.BestOverallQuarterMileTime || ProfileData.BestOverallQuarterMileTime == 0f)
		{
			ProfileData.BestOverallQuarterMileTime = zQuarterMileTime;
		}
		CarGarageInstance currentCar = GetCurrentCar();
		if (zQuarterMileTime < currentCar.BestQuarterMileTime || currentCar.BestQuarterMileTime == 0f)
		{
			currentCar.BestQuarterMileTime = zQuarterMileTime;
		}
	}

	public void AddOfferWallEvent(OfferWallConfiguration.eProvider provider, int amount)
	{
        this.ProfileData.OfferWallData.AddAwardEvents(provider, amount);
	}

    public void AddOfferWallGold(OfferWallConfiguration.eProvider provider, int amount)
    {
        this.ProfileData.OfferWallData.AddGold(provider, amount);
    }

    public void DuplicateWorldTourThemeDataForDebugging(string themeIDToCopy, string newThemeID)
    {
        this.ProfileData.PinSchedulerData.DuplicateWorldTourThemeDataForDebugging(themeIDToCopy, newThemeID);
    }

    public List<PinSchedulerData.PinSchedulerThemeData> GetAllThemes()
    {
        return this.ProfileData.PinSchedulerData.GetAllThemes();
    }

    public int GetEventCountInSequence(string themeID, string sequenceID)
    {
        return this.ProfileData.PinSchedulerData.GetEventCountInSequence(themeID, sequenceID);
    }

    public string GetWorldTourLastSequenceRaced(string themeID)
    {
        return this.ProfileData.PinSchedulerData.GetLastSequenceRaced(themeID);
    }

    public int GetWorldTourRaceResultCount(string themeID, string sequenceID, string pinID, bool didWin)
    {
        return this.ProfileData.PinSchedulerData.GetWorldTourRaceResultCount(themeID, sequenceID, pinID, didWin);
    }

    public void ResetLifeCount(string themeID, List<ScheduledPin> filteredPins)
    {
        this.ProfileData.PinSchedulerData.ResetLifeCount(themeID, filteredPins);
    }

    public ScheduledPinLifetimeData GetPinLifetimeData(string themeID, string lifetimeGroup)
    {
        return this.ProfileData.PinSchedulerData.GetPinLifetimeData(themeID, lifetimeGroup);
    }

    public void SetPinAsRacedInPinScheduleSequence(string themeID, ScheduledPin pin, bool won)
    {
        this.ProfileData.PinSchedulerData.SetPinAsRaced(themeID, pin, won);
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }

	public void SetLastSeenLevelInPinScheduleSequence(string themeID, ScheduledPin pin)
	{
        this.ProfileData.PinSchedulerData.SetLastSeenLevelInSequence(themeID, pin);
	}

	public int GetLastWonLevelInPinScheduleSequence(string themeID, string sequenceID)
	{
	    return this.ProfileData.PinSchedulerData.GetLastWonLevelInSequence(themeID, sequenceID);
	}

	public int GetChoiceSelection(string themeID, string sequenceID, string pinID)
	{
        return this.ProfileData.PinSchedulerData.GetChoiceSelection(themeID, sequenceID, pinID);
	}

	public int GetLastSeenLevelInPinScheduleSequence(string themeID, string sequenceID)
	{
        return this.ProfileData.PinSchedulerData.GetLastSeenLevelInSequence(themeID, sequenceID);
	}

	public int GetPinScheduleRacesWonSinceStateChange(string themeID)
	{
        return this.ProfileData.PinSchedulerData.GetRacesWonSinceStateChange(themeID);
	}

	public bool HasRacedSpecificPinSchedulerPin(string themeID, string sequenceID, string scheduledPinID)
	{
        return this.ProfileData.PinSchedulerData.HasRacedSpecificPinSchedulerPin(themeID, sequenceID, scheduledPinID);
	}

	public int GetLastRacedLevelInPinScheduleSequence(string themeID, string sequenceID)
	{
        return this.ProfileData.PinSchedulerData.GetLastRacedLevelInPinScheduleSequence(themeID, sequenceID);
	}

	public void IncrementPinScheduleRacesComplete(string themeID)
	{
        this.ProfileData.PinSchedulerData.IncrementRacesComplete(themeID);
	}

    public int GetWorldTourThemeSeenCount(string themeID)
    {
        return this.ProfileData.PinSchedulerData.GetThemeSeenCount(themeID);
    }

    public void IncrementWorldTourThemeSeenCount(string themeID)
    {
        this.ProfileData.PinSchedulerData.IncrementThemeSeenCount(themeID);
    }

    public ThemeCompletionLevel GetWorldTourThemeCompletionLevel(string themeID)
    {
        return this.ProfileData.PinSchedulerData.GetThemeCompletionLevel(themeID);
    }

    public void IncrementWorldTourThemeCompletionLevel(string themeID)
    {
        this.ProfileData.PinSchedulerData.IncrementThemeCompletionLevel(themeID);
    }

    public void SetWorldTourThemeCompletionLevel(string themeID, ThemeCompletionLevel level)
    {
        this.ProfileData.PinSchedulerData.SetThemeCompletionLevel(themeID, level);
    }

    public void ResetPinScheduleData()
    {
        this.ProfileData.PinSchedulerData.Reset();
    }

    public string GetPinScheduleMetricsData()
    {
        return this.ProfileData.PinSchedulerData.AsProgressMetricsParameter();
    }

    public int GetCurrentGold()
    {
        if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
        {
            int currentGold = UserManager.Instance.currentAccount.IAPGold +
                this.GoldBought + this.GoldEarned - this.GoldSpent;
            if (currentGold < 0)
            {
                currentGold = 0;
            }
            return currentGold;
        }
        return 0;
    }

    public int GetCurrentCash()
    {
        if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
        {
            long currrentCash = (long)UserManager.Instance.currentAccount.IAPCash + 
                + this.CashBought+ this.CashEarned - this.CashSpent;
            int nonZeroCash;
            if (currrentCash < 2147483647L)
            {
                nonZeroCash = (int)currrentCash;
            }
            else
            {
                nonZeroCash = 2147483647;
            }
            if (nonZeroCash < 0)
            {
                nonZeroCash = 0;
            }
            return nonZeroCash;
        }
        return 0;
    }

    public int PlayerStar
    {
	    get
	    {
		    return ProfileData.PlayerStar;
	    }
	    set { ProfileData.PlayerStar = value; }
    }

    public int PlayerLeagueStar
    {
        get { return ProfileData.PlayerLeagueStar; }
        set { ProfileData.PlayerLeagueStar = value; }
    }

    public int WinCountAfterUpgrade
    {
        get { return ProfileData.WinCountAfterUpgrade; }
        set { ProfileData.WinCountAfterUpgrade = value; }
    }

    public bool HasSeenUnlockCarScreen
    {
        get { return ProfileData.HasSeenUnlockCarScreen; }
        set { ProfileData.HasSeenUnlockCarScreen = value; }
    }


    public int GetPlayerXP()
	{
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			return ProfileData.PlayerXP;
		}
		return 0;
	}

    public int GetPlayerStar()
    {
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            return ProfileData.PlayerStar;
        }
        return 0;
    }

    public int GetPlayerLeagueStar()
    {
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            return ProfileData.PlayerLeagueStar;
        }
        return 0;
    }

	public void AddPlayerXP(int XPAmount)
	{
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			ProfileData.PlayerXP += XPAmount;
		}
	}

	public int GetPlayerLevel()
	{
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			if (ProfileData.PlayerLevel == 0)
			{
				ProfileData.PlayerLevel = 1;
			}
			return ProfileData.PlayerLevel;
		}
		return 1;
	}

    public LeagueData.LeagueName GetPlayerLeague()
    {
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            if (ProfileData.PlayerLeague == LeagueData.LeagueName.None)
            {
                ProfileData.PlayerLeague = LeagueData.LeagueName.Regular;
            }
            return ProfileData.PlayerLeague;
        }
        return LeagueData.LeagueName.None;
    }

	public void IncrementPlayerLevel()
	{
        if (ProfileData!=null)
		{
			ProfileData.PlayerLevel++;
		}
	}

    public void ChangePlayerLeague(LeagueData.LeagueName leagueName)
    {
        if (ProfileData != null)
        {
            ProfileData.PlayerLeague = leagueName;
        }
    }

	public int GetPlayerRP()
	{
        if (ProfileData != null)
		{
			return ProfileData.PlayerRP;
		}
		return 0;
	}

	public void SetBestTimeForCar(string carDBKey, float time)
	{
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			BestCarTimes[carDBKey] = time;
            //LumpManager.Instance.OnSetBestTime();
		}
	}

	public float GetBestTimeForCar(string carDBKey)
	{
        if (ProfileData != null)
		{
			float result = 0f;
			if (BestCarTimes.TryGetValue(carDBKey, out result))
			{
				return result;
			}
		}
		return 0f;
	}

	public bool HasSetAFriendTimeInAnyCar()
	{
		return BestCarTimes.Any((KeyValuePair<string, float> q) => q.Value != 0f);
	}

	public void SetPlayerRP(int value)
	{
        if (ProfileData != null)
		{
			ProfileData.PlayerRP = value;
		}
	}

	public void SubtractFreeUpgrade()
	{
        if (ProfileData != null)
		{
			if (FreeUpgradesLeft <= 0)
			{
				return;
			}
			FreeUpgradesLeft--;
		}
	}


    public void SubtractFreePropertyItem()
    {
        //if (ProfileData != null)
        //{
        //    if (FreeUpgradesLeft <= 0)
        //    {
        //        return;
        //    }
        //    FreeUpgradesLeft--;
        //}
    }

	public int AddFuel(int zFuelPipsToAdd)
	{
        if (ProfileData == null)
		{
			return 0;
		}
		ProfileData.FuelPips += zFuelPipsToAdd;
		for (int num = 0; num != GameDatabase.Instance.AdConfiguration.FuelAdPromptResetThreshold.Count; num++)
		{
			if (ProfileData.FuelPips > GameDatabase.Instance.AdConfiguration.FuelAdPromptResetThreshold[num])
			{
				EnableFuelAdPrompt(num);
			}
		}
		return zFuelPipsToAdd;
	}

	public void SetFuelAutoReplenishTimestampToNow()
	{
        if (ProfileData == null)
		{
			return;
		}
		if (GTDateTime.Now > ProfileData.LastFuelAutoReplenishedTime || GameDatabase.Instance.CareerConfiguration.FuelCheatPrevention == 0)
		{
			ProfileData.LastFuelAutoReplenishedTime = GTDateTime.Now;
		}
	}

	public bool SpendFuel(int pips)
	{
        if (ProfileData == null)
		{
			return false;
		}
		if (ProfileData.FuelPips - pips < 0)
		{
			return false;
		}
		ProfileData.FuelPips -= pips;
		return true;
	}

	public int FuelPips
	{
	    get
	    {
	        if (ProfileData == null)
	        {
	            return 0;
	        }
	        return ProfileData.FuelPips;
	    }
	}

    public bool HasSeenRealRacePopup
    {
        get
        {
#if LOCAL_STORAGE
            return LocalStorageExtension.GetBool("HasSeenRealRacePopup");
#endif
            return ProfileData.HasSeenRealRacePopup;
        }
        set
        {
#if LOCAL_STORAGE
            LocalStorageExtension.SetBool("HasSeenRealRacePopup", value);
#endif
            ProfileData.HasSeenRealRacePopup = value;
        }
    }

    public string AvatarID
    {
        get { return ProfileData.AvatarID; }
        set { ProfileData.AvatarID = value; }
    }

    public bool HasWeeklyRewardToClaim
    {
        get { return ProfileData.HasWeeklyRewardToClaim; }
        set { ProfileData.HasWeeklyRewardToClaim = value; }
    }

    public DateTime PreviousWeeklyLeaderboardCheck
    {
        get { return ProfileData.PreviousWeeklyLeaderboardCheck; }
        set { ProfileData.PreviousWeeklyLeaderboardCheck = value; }
    }

    public DateTime LastEndOfWeekCheck
    {
        get { return ProfileData.LastEndOfWeekCheck; }
        set { ProfileData.LastEndOfWeekCheck = value; }
    }

    public int GachaBronzeKeysSpent
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaBronzeKeysSpent;
        }
    }

    public int GachaBronzeKeysSpentSetAndMangle
    {
        set
        {
            this.ProfileData.GachaBronzeKeysSpent = value;
            MemoryValidator.Instance.Mangle<MangledGachaBronzeKeysSpent>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GachaGoldKeysSpent
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaGoldKeysSpent;
        }
    }

    public int GachaGoldKeysSpentSetAndMangle
    {
        set
        {
            this.ProfileData.GachaGoldKeysSpent = value;
            MemoryValidator.Instance.Mangle<MangledGachaGoldKeysSpent>(MangleInvoker.PlayerProfileAccessor);
        }
    }

    public int GachaSilverKeysSpent
    {
        get
        {
            if (ProfileData == null)
            {
                return 0;
            }
            return this.ProfileData.GachaSilverKeysSpent;
        }
    }

    public int GachaSilverKeysSpentSetAndMangle
    {
        set
        {
            this.ProfileData.GachaSilverKeysSpent = value;
            MemoryValidator.Instance.Mangle<MangledGachaSilverKeysSpent>(MangleInvoker.PlayerProfileAccessor);
        }
    }


    public DateTime LastFuelAutoReplenishedTime()
	{
        if (ProfileData == null)
		{
			return DateTime.MinValue;
		}
		return ProfileData.LastFuelAutoReplenishedTime;
	}
    
    public DateTime LastVasRewardTime()
    {
	    if (ProfileData == null)
	    {
		    return DateTime.MinValue;
	    }
	    return ProfileData.VasTimedRewardLastEventAt;
    }
    
    public DateTime LastAppTuttiRewardTime()
    {
	    if (ProfileData == null)
	    {
		    return DateTime.MinValue;
	    }
	    return ProfileData.AppTuttiTimedRewardLastEventAt;
    }

	public int LastLegalAgreementVersion()
	{
		return ProfileData.LastLegalAgreementVersion;
	}

	public int GetNumEventsCompleted(eCarTier zTier)
	{
		switch (zTier)
		{
		case eCarTier.TIER_1:
			return ProfileData.EventsCompletedTier1;
		case eCarTier.TIER_2:
			return ProfileData.EventsCompletedTier2;
		case eCarTier.TIER_3:
			return ProfileData.EventsCompletedTier3;
		case eCarTier.TIER_4:
			return ProfileData.EventsCompletedTier4;
		case eCarTier.TIER_5:
			return ProfileData.EventsCompletedTier5;
		default:
			return 0;
		}
	}

	public void AddGold(int gold,string category,string itemId)
	{
        this.GoldEarnedSetAndMangle = this.GoldEarned + gold;
        if (gold > 0 && category!=null && itemId!=null) LogSource(CurrencyType.gold, gold, category, itemId);

	}

    public void SpendGold(int gold, string category, string itemId)
    {
        if (gold > 0)
        {
            this.GoldSpentSetAndMangle = this.GoldSpent + gold;
            if(category != null && itemId != null)
				LogSink(CurrencyType.gold, gold,category,itemId);
        }
        if (this.GetCurrentGold() < 0)
        {
        }
        AchievementChecks.CheckForGoldSpentAchievement(this.GoldSpent);
    }

    private void LogSink(CurrencyType currencyType, int amount, string category, string itemId)
    {
	    if (Debug.isDebugBuild)
		    return;
		    
		eCarTier zCarTier = GetLastTier();
		itemId += "_T" + (((int)zCarTier) + 1).ToString() + "_C" + GetLastCrewEvent(zCarTier);
		GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, currencyType.ToString(), amount, category, itemId);
    }
    private void LogSource(CurrencyType currencyType, int amount, string category, string itemId)
    {
	    if (Debug.isDebugBuild)
		    return;
	    
	    if (currencyType == CurrencyType.cash && amount > 2000000)
		    return;
	    
	    if (currencyType == CurrencyType.gold && amount > 5000)
		    return;
	    
		eCarTier zCarTier = GetLastTier();
		itemId += "_T" + (((int)zCarTier)+1).ToString() + "_C" + GetLastCrewEvent(zCarTier);
		GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currencyType.ToString(), amount, category, itemId);
    }

	private eCarTier GetLastTier()
    {
		return RaceEventQuery.Instance.getHighestUnlockedClass();
	}
	
	private List<int> _allCrewBattleEvents;
	public int GetCrewBattleCompletedCount()
	{
		var careerRaceEvents = GameDatabase.Instance.Career.Configuration.CareerRaceEvents;
		if (_allCrewBattleEvents == null)
			_allCrewBattleEvents = GetAllCrewBattleEvents();
		
		var remainingCrewRaces =  _allCrewBattleEvents.Except(EventsCompleted).Count();
		var completedCreRaces = _allCrewBattleEvents.Count - remainingCrewRaces;
		return completedCreRaces;

		List<int> GetAllCrewBattleEvents()
		{
			return careerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups.First().RaceEvents.Select(x => x.EventID).Concat(
				careerRaceEvents.Tier2.CrewBattleEvents.RaceEventGroups.First().RaceEvents.Select(x => x.EventID).Concat(
					careerRaceEvents.Tier3.CrewBattleEvents.RaceEventGroups.First().RaceEvents.Select(x => x.EventID).Concat(
						careerRaceEvents.Tier4.CrewBattleEvents.RaceEventGroups.First().RaceEvents.Select(x => x.EventID).Concat(
							careerRaceEvents.Tier5.CrewBattleEvents.RaceEventGroups.First().RaceEvents.Select(x => x.EventID)
						)))).ToList();
		}
	}

	private int GetLastCrewEvent(eCarTier zCarTier)
    {
		RaceEventData crewBattleEvent =
			RaceEventQuery.Instance.GetCrewBattleEvent(
				GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		bool isCompleted = false;
		if (crewBattleEvent == null)
		{
			crewBattleEvent =
				RaceEventQuery.Instance.GetCrewBattleEvent(
					GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
			isCompleted = true;
		}
		return crewBattleEvent.EventOrder;
	}



    private enum CurrencyType
    {
		gold,
		cash
    }
    
    public void AddStar(int star)
    {
        PlayerStar += star;
        // if (PlayerStar < 0)
        // {
	       //  PlayerStar = 0;
        // }
    }

    public void AddLeagueStar(int star)
    {
        if (GTDateTime.Now > ProfileData.LastEndOfWeekCheck)
        {
            ServerSynchronisedTime.UpdateLeagueDate();
            PlayerLeagueStar = 0;
            ProfileData.LastEndOfWeekCheck = ServerSynchronisedTime.EndDateOfLeague;
        }
        PlayerLeagueStar += star;
        
    }

    public void AddCash(int cash, string category, string itemId)
    {
	    this.CashEarnedSetAndMangle = this.CashEarned + (long) cash;
	    if (cash > 0 && category!=null && itemId!=null) LogSource(CurrencyType.cash, cash, category, itemId);
    }

    public void SpendCash(int cash, string category, string itemId)
    {
	    if (cash > 0)
	    {
		    this.CashSpentSetAndMangle = this.CashSpent + (long) cash;
		    ObjectiveCommand.Execute(new SpendSC(cash), true);
		    if(category != null && itemId != null)
				LogSink(CurrencyType.cash, cash, category, itemId);

	    }

	    AchievementChecks.CheckForCashSpentAchievement(this.CashSpent);
    }

    public void AddFreeUpgrade(int upgrade)
	{
		ProfileData.FreeUpgradesLeft += upgrade;

	}

	private void RegisterHalfMileTime(float zHalfMileTime)
	{
		if (zHalfMileTime < ProfileData.BestOverallHalfMileTime || ProfileData.BestOverallHalfMileTime == 0f)
		{
			ProfileData.BestOverallHalfMileTime = zHalfMileTime;
		}
		CarGarageInstance currentCar = GetCurrentCar();
		if (zHalfMileTime < currentCar.BestHalfMileTime || currentCar.BestHalfMileTime == 0f)
		{
			currentCar.BestHalfMileTime = zHalfMileTime;
		}
	}

	private void RegisterTime(RaceEventData currentRaceEventData, float timeRecordedInRace)
	{
		if (currentRaceEventData.IsHalfMile)
		{
			RegisterHalfMileTime(timeRecordedInRace);
		}
		else
		{
			RegisterQuarterMileTime(timeRecordedInRace);
		}
	}

	public void RaceReward(int cashAwarded, int goldAwarded, int XPAwarded,int starReward)
	{
		AddCash(cashAwarded, "reward", "RaceReward");
        AddGold(goldAwarded,"reward", "RaceReward");
	    if (PolledNetworkState.IsNetworkConnected && LegacyLeaderboardManager.Instance.UserStarFetched)
	    {
	        AddStar(starReward);
            if (LegacyLeaderboardManager.Instance.IsUnlocked())
	            AddLeagueStar(starReward);
	    }
	    GameDatabase.Instance.XPEvents.AddPlayerXP(XPAwarded);
        //int num = Mathf.CeilToInt((float)rpResult.rpDelta * this.CurrentCarCustomisationRPBonusPercent);
        //ObjectiveCommand.Execute(new CounterFreshnessRPEarned(num), true);
	}

	public void RaceRewardNewRank(int newRank)
	{
		if (ProfileData.WorldRank == 0)
		{
			ProfileData.WorldRank = newRank;
			ProfileData.PreviousWorldRank = 0;
		}
		else if (ProfileData.WorldRank != newRank)
		{
			ProfileData.PreviousWorldRank = ProfileData.WorldRank;
			ProfileData.WorldRank = newRank;
		}
	}

	public void RaceCompleted(RaceResultsData humanResultsData, RaceResultsData aiResultsData, RaceEventData currentEvent = null)
	{
	    var serverUserValues = new Dictionary<string, string>();
	    if(currentEvent==null) {
		    currentEvent = RaceEventInfo.Instance.CurrentEvent;
	    }
		if (currentEvent.RaceReward == null)
		{
			return;
		}
		CarGarageInstance currentCar = GetCurrentCar();
		if (!IngameTutorial.IsInTutorial && !RaceEventInfo.Instance.IsDailyBattleEvent && !currentEvent.IsHighStakesEvent())
		{
			if (GetCurrentCar().TopSpeedAttained < humanResultsData.TopRaceSpeedMPH)
			{
				GetCurrentCar().TopSpeedAttained = humanResultsData.TopRaceSpeedMPH;
			}
			RegisterTime(currentEvent, humanResultsData.RaceTime);
		}
		if (currentEvent.IsRelay)
		{
			int relayRaceIndex = currentEvent.GetRelayRaceIndex();
			RelayManager.SetRaceResult(relayRaceIndex, humanResultsData, aiResultsData);
		}
		if (RaceEventInfo.Instance.IsDailyBattleEvent)
		{
			ProfileData.DailyBattlesWonLast = humanResultsData.IsWinner;
			SessionDailyBattleRaces++;
            serverUserValues.Add("DailyBattlesWonLast", humanResultsData.IsWinner.ToString());
		}
		if (ProfileData.MechanicTuningRacesRemaining > 0 && RaceEventInfo.Instance.ShouldCurrentEventUseMechanic())
		{
            //RaceReCommon.JustFettledEngines = true;
			ProfileData.MechanicTuningRacesRemaining--;
			ProfileData.MechanicFettledRaces++;
            serverUserValues.Add("MechanicTuningRacesRemaining", MechanicTuningRacesRemaining.ToString());
            serverUserValues.Add("MechanicFettledRaces", MechanicFettledRaces.ToString());
		}
	    if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
		{
			ConsumeRacesLeft(1);
			UpdateOnlineRacesWonToday();
			if (humanResultsData.IsWinner)
			{
				ProfileData.OnlineRacesWon++;
				ProfileData.OnlineRacesWonToday++;
				ProfileData.ConsecutiveOnlineWins++;
			}
			else
			{
				ProfileData.OnlineRacesLost++;
				ProfileData.OnlineRacesLostToday++;
				ProfileData.ConsecutiveOnlineWins = 0;
			}
		}
		else if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		{
			if (humanResultsData.IsWinner)
			{
				ProfileData.FriendsRacesWon++;
			}
			else
			{
				ProfileData.FriendsRacesLost++;
			}
		}
		else
		{
			if (!IngameTutorial.IsInTutorial)
			{
				RacesEntered++;
                serverUserValues.Add("RacesEntered", RacesEntered.ToString());
                if (!RaceEventInfo.Instance.IsDailyBattleEvent)
				{
					currentCar.RacesAttempted++;
				}
			}
			if (humanResultsData.IsWinner)
			{
				if (!IngameTutorial.IsInTutorial)
				{
					RacesWon++;
                    serverUserValues.Add("RacesWon", RacesWon.ToString());
					if (!RaceEventInfo.Instance.IsDailyBattleEvent)
					{
						currentCar.RacesWon++;
					    WinCountAfterUpgrade++;
                        serverUserValues.Add("WinCountAfterUpgrade", WinCountAfterUpgrade.ToString());
					}
				}
				AddEventCompleted(currentEvent.EventID);
			}
			else
			{
                if (!IngameTutorial.IsInTutorial)
                {
                    RacesLost++;
                    serverUserValues.Add("RacesLost", RacesLost.ToString());
                }
			}
		}
		ProfileData.CarDeals.RacesSinceLastDeal++;
		SessionRacesCompleted++;

	}

	public void AddEventCompleted(int zEventID)
	{
		
		if (ProfileData.EventsCompleted.Contains(zEventID))
		{
			return;
		}

		//Logging
		/*switch(zEventID)
        {
			case 1001:
				Log.AnEvent(Events.EndOfFirstTutorialRace);
				break;
			case 1002:
				Log.AnEvent(Events.EndOfSecondTutorialRace);
				break;
			case 602:
				if(!HasChoosePlayerName)
					Log.AnEvent(Events.EndOfFirstRegulationRace);
				break;
			case 4101:
				Log.AnEvent(Events.EndOfFirstCrewRace);
				break;
		}*/

		if (!IngameTutorial.IsInTutorial && !RaceEventInfo.Instance.IsHighStakesEvent)
		{
			RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventID);
			eCarTier carTier = eventByEventIndex.Parent.GetTierEvents().GetCarTier();
			if (eventByEventIndex.IsRepeatableEvent())
			{
				return;
			}
			switch (carTier)
			{
				
			case eCarTier.TIER_1:
				ProfileData.EventsCompletedTier1++;
				break;
			case eCarTier.TIER_2:
				ProfileData.EventsCompletedTier2++;
				break;
			case eCarTier.TIER_3:
				ProfileData.EventsCompletedTier3++;
				break;
			case eCarTier.TIER_4:
				ProfileData.EventsCompletedTier4++;
				break;
			case eCarTier.TIER_5:
				ProfileData.EventsCompletedTier5++;
			        break;
			}
		}
        ProfileData.EventsCompleted.Add(zEventID);
		ProfileData.EventsCompleted.Sort();

		if(EventsCompleted.Count==10)
        {
			Log.AnEvent(Events.First10Races);
		}
	}
	
	public int AddEventCompletedCount(int zEventID)
	{
		List<int> eventIDs = new List<int>
		{
			4101,
			4102,
			4103,
			41031,
			41041,
			41042,
			4105
		};
		// if (ProfileData.EventsCompleted.Contains(zEventID))
		// {
		// 	return;
		// }

		//Logging
		/*switch(zEventID)
        {
			case 1001:
				Log.AnEvent(Events.EndOfFirstTutorialRace);
				break;
			case 1002:
				Log.AnEvent(Events.EndOfSecondTutorialRace);
				break;
			case 602:
				if(!HasChoosePlayerName)
					Log.AnEvent(Events.EndOfFirstRegulationRace);
				break;
			case 4101:
				Log.AnEvent(Events.EndOfFirstCrewRace);
				break;
		}*/

		
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventID);
		if (eventIDs.Contains(zEventID))
		{
			switch (zEventID)
			{
				case 4101:
					return ProfileData.NumberOfRaceToWinCrew1++;
				case 4102:
					return ProfileData.NumberOfRaceToWinCrew2++;
				case 4103:
					return ProfileData.NumberOfRaceToWinCrew3++;
				case 41031:
					return ProfileData.NumberOfRaceToWinCrew32++;
				case 41041:
					return ProfileData.NumberOfRaceToWinCrew4++;
				case 41042:
					return ProfileData.NumberOfRaceToWinCrew42++;
				case 4105:
					return ProfileData.NumberOfRaceToWinCrew43++;
			}

		}

		return 0;
	}
	
	public bool AddEventFirstRace(int zEventID)
	{
		List<int> eventIDs = new List<int>
		{
			4101,
			4102,
			4103,
			41031,
			41041,
			41042,
			4105
		};
		// if (ProfileData.EventsCompleted.Contains(zEventID))
		// {
		// 	return;
		// }

		//Logging
		/*switch(zEventID)
        {
			case 1001:
				Log.AnEvent(Events.EndOfFirstTutorialRace);
				break;
			case 1002:
				Log.AnEvent(Events.EndOfSecondTutorialRace);
				break;
			case 602:
				if(!HasChoosePlayerName)
					Log.AnEvent(Events.EndOfFirstRegulationRace);
				break;
			case 4101:
				Log.AnEvent(Events.EndOfFirstCrewRace);
				break;
		}*/

		
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventID);
		bool isFirst = false;
		if (eventIDs.Contains(zEventID))
		{
			
			switch (zEventID)
			{
				case 4101:
					isFirst = ProfileData.FirstRace1 = true;
					break;
				case 4102:
					isFirst = ProfileData.FirstRace2 = true;
					break;
				case 4103:
					isFirst = ProfileData.FirstRace3 = true;
					break;
				case 41031:
					isFirst = ProfileData.FirstRace32 = true;
					break;
				case 41041:
					isFirst = ProfileData.FirstRace4 = true;
					break;
				case 41042:
					isFirst = ProfileData.FirstRace41 = true;
					break;
				case 4105:
					isFirst = ProfileData.FirstRace42 = true;
					break;
			}

		}

		return isFirst;
	}
	
	public bool GetEventFirstRace(int zEventID)
	{
		List<int> eventIDs = new List<int>
		{
			4101,
			4102,
			4103,
			41031,
			41041,
			41042,
			4105
		};
		
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventID);
		bool isFirst = false;
		if (eventIDs.Contains(zEventID))
		{
			
			switch (zEventID)
			{
				case 4101:
					return ProfileData.FirstRace1;
				case 4102:
					return ProfileData.FirstRace2;
				case 4103:
					return ProfileData.FirstRace3;
				case 41031:
					return ProfileData.FirstRace32;
				case 41041:
					return ProfileData.FirstRace4;
				case 41042:
					return ProfileData.FirstRace41;
				case 4105:
					return ProfileData.FirstRace42;
			}

		}

		return isFirst;
	}
	
	public void RemoveEventCompleted(int zEventID)
	{
		if (!ProfileData.EventsCompleted.Contains(zEventID))
		{
			return;
		}
		if (!IngameTutorial.IsInTutorial && !RaceEventInfo.Instance.IsHighStakesEvent)
		{
			RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventID);
			eCarTier carTier = eventByEventIndex.Parent.GetTierEvents().GetCarTier();
			switch (carTier)
			{
				case eCarTier.TIER_1:
					ProfileData.EventsCompletedTier1--;
					break;
				case eCarTier.TIER_2:
					ProfileData.EventsCompletedTier2--;
					break;
				case eCarTier.TIER_3:
					ProfileData.EventsCompletedTier3--;
					break;
				case eCarTier.TIER_4:
					ProfileData.EventsCompletedTier4--;
					break;
				case eCarTier.TIER_5:
					ProfileData.EventsCompletedTier5--;
					break;
			}
		}
		ProfileData.EventsCompleted.Remove(zEventID);
		ProfileData.EventsCompleted.Sort();
	}

	public bool GiveInitialCar()
	{
		if (ProfileData.CarsOwned.Count == 0)
		{
			ProfileData.CurrentlySelectedCarDBKey = "car_toyota_camry";
			CarGarageInstance carGarageInstance = new CarGarageInstance();
		    var car = CarDatabase.Instance.GetCar(ProfileData.CurrentlySelectedCarDBKey);
            carGarageInstance.SetupNewGarageInstance(car);
			ProfileData.CarsOwned.Add(carGarageInstance);
			return true;
		}
		return false;
	}

	public float BestTime()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsHalfMile)
		{
			return ProfileData.BestOverallHalfMileTime;
		}
		return ProfileData.BestOverallQuarterMileTime;
	}

	public bool PlayerOwnsCurrentCar()
	{
		return ProfileData.CarsOwned.Find((CarGarageInstance x) => x.CarDBKey == ProfileData.CurrentlySelectedCarDBKey) != null;
	}

	public bool PlayerCarInvalidRTW(string carDBKey = "")
	{
		//CarInfo car;
		//if (string.IsNullOrEmpty(carDBKey))
		//{
		//	car = CarDatabase.Instance.GetCar(CurrentlySelectedCarDBKey);
		//}
		//else
		//{
		//	car = CarDatabase.Instance.GetCar(carDBKey);
		//}
	    return false;//SeasonServerDatabase.Instance.DoWeHaveStatusAndStandings() && (car.IsPrizeInFutureSeason() || car.IsPrizeInCurrentSeason());
	}

	public CarGarageInstance GetCurrentCar()
	{
		CarGarageInstance carGarageInstance = ProfileData.CarsOwned.Find(x => x.CarDBKey == ProfileData.CurrentlySelectedCarDBKey);
		if (carGarageInstance == null)
		{
            carGarageInstance = new CarGarageInstance();
		    var car = CarDatabase.Instance.GetCar(ProfileData.CurrentlySelectedCarDBKey);
            carGarageInstance.SetupNewGarageInstance(car);
            return carGarageInstance;
		}
		return carGarageInstance;
	}

	public void UpdateOnlineRacesWonToday()
	{
		if (ProfileData.LastOnlineRace.DayOfYear != GTDateTime.Now.DayOfYear)
		{
			ProfileData.OnlineRacesWonToday = 0;
			ProfileData.OnlineRacesLostToday = 0;
		}
		ProfileData.LastOnlineRace = GTDateTime.Now;
	}

	public CarGarageInstance GetCarFromID(string carid)
	{
		return ProfileData.CarsOwned.Find((CarGarageInstance p) => p.CarDBKey == carid);
	}

	public bool FuelTankIsFull()
	{
        return FuelPips == FuelManager.Instance.CurrentMaxFuel();
	}

	private CarGarageInstance GetOwnedCar(string carID)
	{
		List<CarGarageInstance> carsOwned = ProfileData.CarsOwned;
		if (carsOwned == null)
		{
			return null;
		}
		return carsOwned.Find((CarGarageInstance ownedCar) => ownedCar.CarDBKey == carID);
	}

	public bool IsProCarOwned(string carID)
	{
		CarGarageInstance ownedCar = GetOwnedCar(carID);
		return ownedCar != null && ownedCar.EliteCar;
	}

	public bool IsCarOwned(string carID)
	{
		return GetOwnedCar(carID) != null;
	}

	public bool IsCarNew(string carKey)
	{
		return ProfileData.NewCars.Contains(carKey);
	}

	public bool HasNewCars()
	{
		return ProfileData.NewCars.Count > 0;
	}

	public bool HasNewCarsOtherThanCurrentlySelected()
	{
		return ProfileData.NewCars.Any((string c) => c != CurrentlySelectedCarDBKey);
	}

	public void CarSeen(string carKey)
	{
		ProfileData.NewCars.Remove(carKey);
	}
	
	public bool HasSeenCar(string carKey)
	{
		return ProfileData.NewCars.Contains(carKey);
	}
	
	public int UnseenCarCount()
	{
		return ProfileData.NewCars.Count();
	}

	public void SetUpgradeLevelFitted(eUpgradeType zUpgradeType, int zLevel)
	{
		CarGarageInstance currentCar = GetCurrentCar();
		currentCar.UpgradeStatus[zUpgradeType].levelFitted = CarUpgradeStatus.Convert(zLevel);
		UpdateCurrentCarSetup();
	}

	public void SetUpgradeLevelOwned(eUpgradeType zUpgradeType, int zLevel)
	{
		CarGarageInstance currentCar = GetCurrentCar();
		currentCar.UpgradeStatus[zUpgradeType].levelOwned = CarUpgradeStatus.Convert(zLevel);
		UpdateCurrentCarSetup();
	}

	public void UpdateCurrentCarSetup()
	{
		CarGarageInstance currentCar = GetCurrentCar();

	    if (currentCar != null)
	        FillInUpgradeSetup(currentCar);
	}

	private void FillInUpgradeSetup(CarGarageInstance garageCar)
	{
		mCurrentCarUpgradeSetup = GetUpgradeSetupForCar(garageCar);
	}

	public CarUpgradeSetup GetUpgradeSetupForCar(CarGarageInstance garageCar)
	{
		CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
		carUpgradeSetup.CarDBKey = garageCar.CarDBKey;
		foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
		{
			carUpgradeSetup.UpgradeStatus[current].levelFitted = garageCar.UpgradeStatus[current].levelFitted;
			carUpgradeSetup.UpgradeStatus[current].levelOwned = garageCar.UpgradeStatus[current].levelOwned;
		}
		return carUpgradeSetup;
	}

	public void UpdateCurrentPhysicsSetup()
	{
        //Was before
        //if (string.IsNullOrEmpty(CurrentlySelectedCarDBKey))
        //    return;
        //UpdateCurrentCarSetup();
        //var carInfo = CarDatabase.Instance.GetCar(CurrentlySelectedCarDBKey);
        //var ppData = GameDatabase.Instance.CarsConfiguration.CarPPData;
        //var carStatCalculator = new CarStatsCalculator(new CarPhysics(), carInfo, ppData);
        //carStatCalculator.CalculateStatsForHumanCarWithUpgradeSetup(this.GetCurrentCarUpgradeSetup());
        //this.PlayerPhysicsSetup = carStatCalculator.playerCarPhysicsSetup;
        //carStatCalculator.SetOutStats(eCarStatsType.PLAYER_SETUP_CAR);

        this.UpdateCurrentCarSetup();
        CarStatsCalculator.Instance.CalculateStatsForHumanCarWithUpgradeSetup(this.GetCurrentCarUpgradeSetup());
        this.PlayerPhysicsSetup = CarStatsCalculator.Instance.playerCarPhysicsSetup;
        CarStatsCalculator.Instance.SetOutStats(eCarStatsType.PLAYER_SETUP_CAR);
	}

	public int GetUpgradeLevelOwned(eUpgradeType zUpgradeType)
	{
		CarGarageInstance currentCar = GetCurrentCar();
		return currentCar.UpgradeStatus[zUpgradeType].levelOwned;
	}

	public int GetUpgradeLevelFitted(eUpgradeType zUpgradeType)
	{
		CarGarageInstance currentCar = GetCurrentCar();
		return currentCar.UpgradeStatus[zUpgradeType].levelFitted;
	}

	public CarUpgradeSetup GetCurrentCarUpgradeSetup()
	{
		return mCurrentCarUpgradeSetup;
	}

	public eBuyCarResult CanBuyCar(CarInfo carInfo, string carDBKey, AgentCarDeal deal, CostType costtype)
	{
		if (costtype == CostType.OFFERPACK)
		{
			return eBuyCarResult.SUCCESS;
		}
		int cashPrice;
        int goldPrice;
		if (carInfo != null)
		{
			cashPrice = carInfo.BuyPrice;
            goldPrice = carInfo.GoldPrice;
		}
		else
		{
			cashPrice = 0;
			goldPrice = 0;
		}
		if (deal != null)
		{
			cashPrice = 0;
			goldPrice = deal.GetGoldPrice();
		}
		if (GetCurrentCash() < cashPrice && costtype == CostType.CASH)
		{
			return eBuyCarResult.NOT_ENOUGH_CASH;
		}
		if (GetCurrentGold() < goldPrice && costtype == CostType.GOLD)
		{
			return eBuyCarResult.NOT_ENOUGH_GOLD;
		}
		if (!ProfileData.HasBoughtFirstCar)
		{
			return eBuyCarResult.SUCCESS;
		}
		if (ProfileData.CarsOwned.Find((CarGarageInstance x) => x.CarDBKey == carDBKey) != null)
		{
			return eBuyCarResult.ALREADY_OWN_THIS_CAR;
		}
		return eBuyCarResult.SUCCESS;
	}

	public eBuyCarResult OrderCar(CarInfo carInfo, string carDBKey, AgentCarDeal deal, CostType costtype)
	{
		int cash = carInfo.BuyPrice;
        int goldPrice = carInfo.GoldPrice;
		if (deal != null)
		{
			cash = 0;
			goldPrice = deal.GetGoldPrice();
		}
		LastBoughtCarRacesEntered = RacesEntered;
		if (costtype == CostType.CASH)
		{
			SpendCash(cash,"OrderCar", "Car");
		}
		if (costtype == CostType.GOLD)
		{
			SpendGold(goldPrice,"OrderCar", "Car");
		}
		return eBuyCarResult.SUCCESS;
	}

    public void SpendGachaKeys(int keys, GachaType eGachaType, EGachaKeysSpentReason reason, MetricsCurrencySpend spendData = null)
    {
        if (eGachaType != GachaType.Invalid && eGachaType < GachaType.MaxGachaTypes)
        {
            if (spendData != null)
            {
                switch (eGachaType)
                {
                    case GachaType.Bronze:
                        spendData.Setup(keys, EMetricsCurrencyType.Keys_Bronze);
                        break;
                    case GachaType.Silver:
                        spendData.Setup(keys, EMetricsCurrencyType.Keys_Silver);
                        break;
                    case GachaType.Gold:
                        spendData.Setup(keys, EMetricsCurrencyType.Keys_Gold);
                        break;
                    default:
                        spendData.Setup(keys, EMetricsCurrencyType.Keys_Bronze);
                        break;
                }
            }
            if (keys > 0)
            {
                GachaKeysSpentTransaction transaction = new GachaKeysSpentTransaction(keys, eGachaType, reason);
                PlayerProfileManager.Instance.PendingTransactions.AddTransaction(transaction);
                switch (eGachaType)
                {
                    case GachaType.Bronze:
                        this.GachaBronzeKeysSpentSetAndMangle = this.GachaBronzeKeysSpent + keys;
                        break;
                    case GachaType.Silver:
                        this.GachaSilverKeysSpentSetAndMangle = this.GachaSilverKeysSpent + keys;
                        break;
                    case GachaType.Gold:
                        this.GachaGoldKeysSpentSetAndMangle = this.GachaGoldKeysSpent + keys;
                        break;
                }
                //if (this.OnGachaKeysSpent != null)
                //{
                //    this.OnGachaKeysSpent(keys);
                //}
            }
        }
    }

    public void AddToTransactionHistory(string zItem, int zDeltaCash, int zDeltaGold)
	{
        ProfileData.TransactionHistory.Add(string.Concat(zItem, ",", zDeltaCash, ",", zDeltaGold));
	}

	public void ClearLastAcquiredCar()
	{
		LastAcquiredCar = string.Empty;
	}

	public void GiveCar(string carDBKey, int colourIndex, bool alreadyOwn = false)
	{
		if (!HasBoughtFirstCar)
		{
			PlayerProfileManager.Instance.ActiveProfile.ProfileData.CarsOwned.Clear();
			PlayerProfileManager.Instance.ActiveProfile.ProfileData.CurrentlySelectedCarDBKey = string.Empty;
		    HasBoughtFirstCar = true;
		}
        if (true)//carSource != eCarSource.Temp)
        {
            ObjectiveCommand.Execute(new CounterObtainCar(), true);
        }
		if (ProfileData.CurrentlySelectedCarDBKey == string.Empty)
		{
			ProfileData.CurrentlySelectedCarDBKey = carDBKey;
		}
		if (!alreadyOwn)
		{
			CarGarageInstance carGarageInstance = new CarGarageInstance();
            var car = CarDatabase.Instance.GetCar(carDBKey);
            carGarageInstance.SetupNewGarageInstance(car);
            //carGarageInstance.AppliedColourIndex = colourIndex;
			ProfileData.CarsOwned.Add(carGarageInstance);
			UpdateCurrentCarSetup();
		    if (!ProfileData.NewCars.Contains(carDBKey))
		        ProfileData.NewCars.Add(carDBKey);
			LastAcquiredCar = carDBKey;
		}
	}

	public void AwardFriendStarCar(string carDBKey, int colourIndex)
	{
		FriendsCarsWon++;
		GiveCar(carDBKey, colourIndex, IsCarOwned(carDBKey));
	}

	public void SelectCar(string carDBKey)
	{
		if (CurrentlySelectedCarDBKey != carDBKey)
		{
			CurrentlySelectedCarDBKey = carDBKey;
			UpdateCurrentPhysicsSetup();
		}
	}

	public bool DeliverCarNow(string carDBKey)
	{
		Arrival arrivalForCar = ArrivalManager.Instance.GetArrivalForCar(carDBKey);
		if (arrivalForCar == null)
		{
			return true;
		}
	    var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
        int goldCostForSkip = arrivalForCar.GetGoldCostForSkip(secondsPerGold);
		if (GetCurrentGold() - goldCostForSkip < 0)
		{
			return false;
		}
		SpendGold(goldCostForSkip,"OrderCar", "DeliverNow");
		return true;
	}

	public int GetUpgradeDeliveryCost(string carDBKey, eUpgradeType CurrentUpgradeType)
	{
		Arrival arrivalForUpgrade = ArrivalManager.Instance.GetArrivalForUpgrade(carDBKey, CurrentUpgradeType);
		if (arrivalForUpgrade != null)
		{
            var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
            return arrivalForUpgrade.GetGoldCostForSkip(secondsPerGold);
		}
		return 0;
	}

	public int GetDeliveryCost(string carDBKey)
	{
		Arrival arrivalForCar = ArrivalManager.Instance.GetArrivalForCar(carDBKey);
		if (arrivalForCar != null)
		{
            var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
            return arrivalForCar.GetGoldCostForSkip(secondsPerGold);
		}
		return 0;
	}

	public bool DeliverUpgradeNow(string carDBKey, eUpgradeType upgrade)
	{
		Arrival arrivalForUpgrade = ArrivalManager.Instance.GetArrivalForUpgrade(carDBKey, upgrade);
		if (arrivalForUpgrade == null)
		{
			return true;
		}
        var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
        int goldCostForSkip = arrivalForUpgrade.GetGoldCostForSkip(secondsPerGold);
		if (GetCurrentGold() - goldCostForSkip < 0)
		{
			return false;
		}
		SpendGold(goldCostForSkip,"UpgradeCar", "DeliverNow");
		Save();
		return true;
	}

	public bool CanBuyUpgrade(CarGarageInstance garageInstance, int CashCost, int GoldCost, CostType costing)
	{
		return costing == CostType.FREE || (((costing != CostType.CASH && costing != CostType.CASHANDGOLD) || GetCurrentCash() >= CashCost) && ((costing != CostType.GOLD && costing != CostType.CASHANDGOLD) || GetCurrentGold() >= GoldCost));
	}

	public bool OrderUpgrade(CarGarageInstance garageInstance, CostType costtype, int CashCost, int GoldCost)
	{
		if (costtype == CostType.FREE)
		{
			SubtractFreeUpgrade();
		}
		else
		{
			if (costtype == CostType.CASH || costtype == CostType.CASHANDGOLD)
			{
				SpendCash(CashCost,"UpgradeCar", "OrderUpgrade");
				GetCurrentCar().MoneySpentOnUpgrades += CashCost;
			}
			if (costtype == CostType.GOLD || costtype == CostType.CASHANDGOLD)
			{
				SpendGold(GoldCost,"UpgradeCar", "OrderUpgrade");
			}
		}
		if (GetCurrentCar().NumUpgradesBought == 0)
		{
            //Log.AnEvent(Events.Bought1stUpgrade);
		}
		GetCurrentCar().NumUpgradesBought++;
        ObjectiveCommand.Execute(new CounterUpgradeItem(), true);
	    if (!HasBoughtFirstUpgrade)
	    {
            Log.AnEvent(Events.Bought1stUpgrade);
	    }
	        
        HasBoughtFirstUpgrade = true;
		return true;
	}


    public bool OrderPropertyItem(CarGarageInstance garageInstance, CostType costtype, int CashCost, int GoldCost)
    {
        if (costtype == CostType.FREE)
        {
            SubtractFreePropertyItem();
        }
        else
        {
            if (costtype == CostType.CASH || costtype == CostType.CASHANDGOLD)
            {
                SpendCash(CashCost,"OrderPropertyItem", "OrderUpgrade");
                GetCurrentCar().MoneySpentOnPropertyItem += CashCost;
            }
            if (costtype == CostType.GOLD || costtype == CostType.CASHANDGOLD)
            {
                SpendGold(GoldCost,"OrderPropertyItem", "OrderUpgrade");
            }
        }
        if (GetCurrentCar().NumPropertyBought == 0)
        {
            Log.AnEvent(Events.Bought1stProperty);
        }
        GetCurrentCar().NumPropertyBought++;
        HasBoughtFirstProperty = true;
        return true;
    }

	public bool HasWonAnyMultiplayerCarPieces()
	{
	    return this.ProfileData.CarsWonInMultiplayer.Count > 0;
	}

    public List<MultiplayerCarPrize> GetFullyWonMultiplayerCars()
    {
        return this.ProfileData.CarsWonInMultiplayer.FindAll((MultiplayerCarPrize x) => x.PiecesWon.Count == x.NumPiecesTotal);
    }

    public List<MultiplayerCarPrize> GetPartiallyWonMultiplayerCars()
    {
        return this.ProfileData.CarsWonInMultiplayer.FindAll((MultiplayerCarPrize x) => x.PiecesWon.Count > 0 && x.PiecesWon.Count < x.NumPiecesTotal);
    }

    public bool CarIsAFullyWonMultiplayerCar(string carDBKey)
    {
        List<MultiplayerCarPrize> list = this.GetFullyWonMultiplayerCars();
        list = list.FindAll((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        return list.Count > 0;
    }

    public bool CarIsAPartiallyWonMultiplayerCar(string carDBKey)
    {
        List<MultiplayerCarPrize> list = this.GetPartiallyWonMultiplayerCars();
        list = list.FindAll((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        return list.Count > 0;
    }

    public int GetNumPiecesOfMultiplyerCarWon(string carDBKey)
	{
        List<int> piecesOfMultiplayerCarWon = this.GetPiecesOfMultiplayerCarWon(carDBKey);
        if (piecesOfMultiplayerCarWon == null)
        {
            return 0;
        }
        return piecesOfMultiplayerCarWon.Count;
	}

    public List<int> GetPiecesOfMultiplayerCarWon(string carDBKey)
    {
        List<MultiplayerCarPrize> partiallyWonMultiplayerCars = this.GetPartiallyWonMultiplayerCars();
        partiallyWonMultiplayerCars.AddRange(this.GetFullyWonMultiplayerCars());
        MultiplayerCarPrize multiplayerCarPrize = partiallyWonMultiplayerCars.Find((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        if (multiplayerCarPrize == null)
        {
            return null;
        }
        return new List<int>(multiplayerCarPrize.PiecesWon);
    }

    public bool AwardPrizeomaticCarPart(string carDBKey)
    {
        MultiplayerCarPrize multiplayerCarPrize = this.ProfileData.CarsWonInMultiplayer.Find((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        if (multiplayerCarPrize == null)
        {
            MultiplayerCarPrize multiplayerCarPrize2 = new MultiplayerCarPrize();
            multiplayerCarPrize2.CarDBKey = carDBKey;
            int num = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(carDBKey);
            this.AwardRandomCarPart(multiplayerCarPrize2, num);
            multiplayerCarPrize2.NumPiecesTotal = num;
            this.ProfileData.CarsWonInMultiplayer.Add(multiplayerCarPrize2);
            return false;
        }
        this.AwardRandomCarPart(multiplayerCarPrize, multiplayerCarPrize.NumPiecesTotal);
        return this.PostAwardPart(multiplayerCarPrize, carDBKey);
    }

    public void AddPrizeToWonMPWonCars(string carDBKey)
    {
        MultiplayerCarPrize multiplayerCarPrize = new MultiplayerCarPrize();
        multiplayerCarPrize.CarDBKey = carDBKey;
        multiplayerCarPrize.PiecesWon = new List<int>();
        multiplayerCarPrize.NumPiecesTotal = 0;
    }

    public bool AwardSpecificPrizeomaticCarPart(int part, string carDBKey)
    {
        MultiplayerCarPrize multiplayerCarPrize = this.ProfileData.CarsWonInMultiplayer.Find((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        if (multiplayerCarPrize == null)
        {
            MultiplayerCarPrize multiplayerCarPrize2 = new MultiplayerCarPrize();
            multiplayerCarPrize2.CarDBKey = carDBKey;
            multiplayerCarPrize2.PiecesWon.Add(part);
            int numPiecesTotal = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(carDBKey);
            multiplayerCarPrize2.NumPiecesTotal = numPiecesTotal;
            this.ProfileData.CarsWonInMultiplayer.Add(multiplayerCarPrize2);
            return false;
        }
        multiplayerCarPrize.PiecesWon.Add(part);
        return this.PostAwardPart(multiplayerCarPrize, carDBKey);
    }

    private void AwardRemainingPrizeomaticCarParts(string carDBKey, MultiplayerCarPrize carWon)
    {
        for (int i = 0; i < carWon.NumPiecesTotal; i++)
        {
            if (!carWon.PiecesWon.Contains(i))
            {
                carWon.PiecesWon.Add(i);
            }
        }
        this.PostAwardPart(carWon, carDBKey);
    }

    public void BuyPrizeomaticCar(string carDBKey)
    {
        MultiplayerCarPrize multiplayerCarPrize = this.ProfileData.CarsWonInMultiplayer.Find((MultiplayerCarPrize x) => x.CarDBKey == carDBKey);
        if (multiplayerCarPrize != null)
        {
            int num = GameDatabase.Instance.Online.GoldCostToCompleteCar(carDBKey, multiplayerCarPrize.PiecesWon.Count);
            if (num > 0)
            {
                if (this.GetCurrentGold() >= num)
                {
                    this.SpendGold(num,"BuyPrizeomaticCar","PrizeomaticCar");
                    this.AwardRemainingPrizeomaticCarParts(carDBKey, multiplayerCarPrize);
                    this.SendMetricsEvent(0, num, multiplayerCarPrize.PiecesWon.Count + 1);
                }
            }
            else
            {
                int num2 = GameDatabase.Instance.Online.CashCostToCompleteCar(carDBKey, multiplayerCarPrize.PiecesWon.Count);
                if (this.GetCurrentCash() >= num2)
                {
                    this.SpendCash(num2,"BuyPrizeomaticCar","PrizeomaticCar");
                    this.AwardRemainingPrizeomaticCarParts(carDBKey, multiplayerCarPrize);
                    this.SendMetricsEvent(num2, 0, multiplayerCarPrize.PiecesWon.Count + 1);
                }
            }
        }
    }

	private void SendMetricsEvent(int cashPrice, int goldPrice, int pieceID)
	{
        string multiplayerStreakType = MultiplayerUtils.GetMultiplayerStreakType();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Type,
				"Puzzle"
			},
			{
				Parameters.PieceID,
				pieceID.ToString()
			},
			{
				Parameters.CostGold,
				goldPrice.ToString()
			},
			{
				Parameters.CostCash,
				cashPrice.ToString()
			},
			{
				Parameters.DGld,
				(-goldPrice).ToString()
			},
			{
				Parameters.DCsh,
				(-cashPrice).ToString()
			},
            {
                Parameters.StreakType,
                multiplayerStreakType
            }
		};
		Log.AnEvent(Events.MultiplayerPurchase, data);
	}

    private bool PostAwardPart(MultiplayerCarPrize carWon, string carDBKey)
    {
        if (carWon.NumPiecesTotal == carWon.PiecesWon.Count)
        {
            if (this.IsCarOwned(carDBKey))
            {
                SportsPackScreen.CarWonAlreadyOwned = true;
                CarGarageInstance carFromID = this.GetCarFromID(carDBKey);
                SportsPackScreen.CarWonAlreadyOwnedPP = carFromID.CurrentPPIndex;
                this.GiveCar(carDBKey, 0, true);
            }
            else
            {
                this.GiveCar(carDBKey, 0, false);
                SportsPackScreen.CarWonAlreadyOwned = false;
            }
            SportsPackScreen.CarToAward = carDBKey;
            this.GetCarFromID(carDBKey).EliteCar = true;
            this.ApplyEliteLiveryToCar(carDBKey);
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            return true;
        }
        return false;
    }

    private void AwardRandomCarPart(MultiplayerCarPrize prize, int totalPieces)
    {
        if (prize.PiecesWon.Count == 0)
        {
            prize.PiecesWon.Add(UnityEngine.Random.Range(0, totalPieces - 1));
        }
        else
        {
            List<int> list = new List<int>();
            for (int i = 0; i < totalPieces; i++)
            {
                if (!prize.PiecesWon.Contains(i))
                {
                    list.Add(i);
                }
            }
            if (list.Count <= 0)
            {
                return;
            }
            int num = totalPieces - prize.PiecesWon.Count;
            int index = 0;
            if (num != 1)
            {
                index = UnityEngine.Random.Range(0, list.Count - 1);
            }
            prize.PiecesWon.Add(list[index]);
        }
    }

	public bool GiveUpgrade(string carDBKey, eUpgradeType upgradeType)
	{
		CarGarageInstance carGarageInstance = ProfileData.CarsOwned.Find((CarGarageInstance car) => car.CarDBKey == carDBKey);
		if (carGarageInstance == null)
		{
			return false;
		}
		CarUpgradeStatus expr_3E = carGarageInstance.UpgradeStatus[upgradeType];
		expr_3E.levelOwned += 1;
	    WinCountAfterUpgrade = 0;
		return true;
	}

	public bool OwnsEliteCar()
	{
		return ProfileData.CarsOwned.Any((CarGarageInstance car) => car.EliteCar);
	}

	public bool GetMechanicServicesAvailable()
	{
		return ProfileData.MechanicTuningRacesRemaining > 0;
	}

	public eCarTier GetHighestCarTierOwned()
	{
		eCarTier ecarTier = eCarTier.TIER_1;
		foreach (CarGarageInstance current in ProfileData.CarsOwned)
		{
			CarInfo car = CarDatabase.Instance.GetCar(current.CarDBKey);
            if (car.BaseCarTier > ecarTier)
			{
                ecarTier = car.BaseCarTier;
			}
		}
        return ecarTier;
	}

	public int GetMaxPPOfCarOwnedInThisTier(eCarTier tier, out bool ownAnyCarsInThisTier)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<CarGarageInstance> list = activeProfile.CarsOwned.FindAll((CarGarageInstance x) => x.CurrentTier == tier);
		ownAnyCarsInThisTier = false;
		int num = -1;
		if (list.Count > 0)
		{
			ownAnyCarsInThisTier = true;
			foreach (CarGarageInstance current in list)
			{
				if (current.CurrentPPIndex > num)
				{
					num = current.CurrentPPIndex;
				}
			}
			return num;
		}
		return num;
	}

	public int NumTimedConsumablesActive()
	{
		int num = 0;
		if (IsConsumableActive(eCarConsumables.EngineTune))
		{
			num++;
		}
		if (IsConsumableActive(eCarConsumables.Nitrous))
		{
			num++;
		}
		if (IsConsumableActive(eCarConsumables.Tyre))
		{
			num++;
		}
		return num;
	}

	public void UpdateConsumablesFromNetworkTime()
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return;
		}
		UpdateConsumableFromNetworkTime(eCarConsumables.EngineTune);
		UpdateConsumableFromNetworkTime(eCarConsumables.Nitrous);
		UpdateConsumableFromNetworkTime(eCarConsumables.PRAgent);
		UpdateConsumableFromNetworkTime(eCarConsumables.Tyre);
		UpdateConsumableFromNetworkTime(eCarConsumables.WholeTeam);
	}

	private void UpdateConsumableFromNetworkTime(eCarConsumables type)
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return;
		}
		if (GetConsumableRacesLeft(type) > 0)
		{
			return;
		}
		if (GetConsumableExpireTimeSync(type) == DateTime.MinValue && IsConsumableActive(type))
		{
			int num = GetConsumableNumMinutesLastPurchased(type) * 60;
			TimeSpan consumableTimeRemaining = GetConsumableTimeRemaining(type);
			if (num > consumableTimeRemaining.TotalSeconds || num == 0)
			{
				num = (int)consumableTimeRemaining.TotalSeconds;
			}
			if (num > 0)
			{
                DateTime time = ServerSynchronisedTime.Instance.GetDateTime().AddSeconds(num);
				SetConsumableExpireTimeSync(type, time);
			}
		}
		else if (!IsConsumableActive(type))
		{
			SetConsumableExpireTime(type, DateTime.MinValue);
			SetConsumableExpireTimeSync(type, DateTime.MinValue);
			SetConsumableAntiCheatRacesLeft(type, 0);
		}
	}

	public void ResetConsumable(eCarConsumables type)
	{
		SetConsumableExpireTime(type, DateTime.MinValue);
		SetConsumableExpireTimeSync(type, DateTime.MinValue);
		SetConsumableAntiCheatRacesLeft(type, 0);
		SetConsumableRacesLeft(type, 0);
	}

	public bool IsConsumableActive(eCarConsumables type)
	{
		if (GetConsumableRacesLeft(type) > 0)
		{
			return true;
		}
		DateTime t = GetConsumableExpireTime(type);
		DateTime consumableExpireTimeSync = GetConsumableExpireTimeSync(type);
		DateTime t2 = GTDateTime.Now;
        if (ServerSynchronisedTime.Instance.ServerTimeValid && consumableExpireTimeSync != DateTime.MinValue)
		{
			t = consumableExpireTimeSync;
            t2 = ServerSynchronisedTime.Instance.GetDateTime();
		}
		return t > t2 && GetConsumableAntiCheatRacesLeft(type) > 0;
	}

	public int GetConsumableExprireTimeHours(eCarConsumables type)
	{
		if (!IsConsumableActive(type))
		{
			return 0;
		}
		TimeSpan consumableTimeRemaining = GetConsumableTimeRemaining(type);
		return Mathf.Max(0, consumableTimeRemaining.Hours + consumableTimeRemaining.Days * 24);
	}

	public int GetConsumableExprireTimeMinutes(eCarConsumables type)
	{
		if (!IsConsumableActive(type))
		{
			return 0;
		}
		return Mathf.Max(0, GetConsumableTimeRemaining(type).Minutes);
	}

	public int GetConsumableExprireTimeSeconds(eCarConsumables type)
	{
		if (!IsConsumableActive(type))
		{
			return 0;
		}
		return Mathf.Max(0, GetConsumableTimeRemaining(type).Seconds);
	}

	public int GetConsumableExpireTimeTotalMinutes(eCarConsumables type)
	{
		if (!IsConsumableActive(type))
		{
			return 0;
		}
		return Mathf.Max(0, (int)GetConsumableTimeRemaining(type).TotalMinutes);
	}

	private TimeSpan GetConsumableTimeRemaining(eCarConsumables type)
	{
		TimeSpan zero = TimeSpan.Zero;
		if (!IsConsumableActive(type))
		{
			return zero;
		}
		DateTime d = GetConsumableExpireTime(type);
		DateTime consumableExpireTimeSync = GetConsumableExpireTimeSync(type);
		DateTime d2 = GTDateTime.Now;
        if (ServerSynchronisedTime.Instance.ServerTimeValid && consumableExpireTimeSync != DateTime.MinValue)
		{
			d = consumableExpireTimeSync;
            d2 = ServerSynchronisedTime.Instance.GetDateTime();
		}
		return d - d2;
	}

	public void ExtendConsumableDuration(eCarConsumables type, int duration)
	{
		int num = GetConsumableExpireTimeTotalMinutes(type);
		num += duration;
		SetConsumableExpireTime(type, num);
	}

	public int GetConsumableAntiCheatRacesLeft(eCarConsumables type)
	{
		return ProfileData.Consumables[type].AntiCheatRacesLeft;
	}

	private void SetConsumableAntiCheatRacesLeft(eCarConsumables type, int val)
	{
		ProfileData.Consumables[type].AntiCheatRacesLeft = val;
	}

	public int GetConsumableRacesLeft(eCarConsumables type)
	{
		return ProfileData.Consumables[type].RacesLeft;
	}

	public void ConsumeRacesLeft(int numRaces)
	{
		IEnumerator enumerator = Enum.GetValues(typeof(eCarConsumables)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				eCarConsumables key = (eCarConsumables)((int)enumerator.Current);
				ProfileData.Consumables[key].RacesLeft = Mathf.Max(0, ProfileData.Consumables[key].RacesLeft - numRaces);
				ProfileData.Consumables[key].AntiCheatRacesLeft = Mathf.Max(0, ProfileData.Consumables[key].AntiCheatRacesLeft - numRaces);
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
	}

	public DateTime GetConsumableExpireTime(eCarConsumables type)
	{
		return (GetConsumableAntiCheatRacesLeft(type) > 0) ? ProfileData.Consumables[type].Expires : DateTime.MinValue;
	}

	private void SetConsumableExpireTime(eCarConsumables type, DateTime time)
	{
		ProfileData.Consumables[type].Expires = time;
	}

	public DateTime GetConsumableExpireTimeSync(eCarConsumables type)
	{
		return ProfileData.Consumables[type].ExpiresSync;
	}

	private void SetConsumableExpireTimeSync(eCarConsumables type, DateTime time)
	{
		ProfileData.Consumables[type].ExpiresSync = time;
	}

	public void IncrementConsumableExpireTime(eCarConsumables type, int MinutesActive)
	{
		DateTime time = GetConsumableExpireTime(type).AddMinutes(MinutesActive);
		int val = GetConsumableAntiCheatRacesLeft(type) + MinutesActive * 2;
		int numMinutes = GetConsumableExpireTimeTotalMinutes(type) + MinutesActive;
		DateTime time2 = DateTime.MinValue;
		DateTime consumableExpireTimeSync = GetConsumableExpireTimeSync(type);
        if (ServerSynchronisedTime.Instance.ServerTimeValid && consumableExpireTimeSync != DateTime.MinValue)
		{
			time2 = consumableExpireTimeSync.AddMinutes(MinutesActive);
		}
		SetConsumableExpireTime(type, time);
		SetConsumableExpireTimeSync(type, time2);
		SetConsumableAntiCheatRacesLeft(type, val);
		SetConsumableCumulativeMinutesPurchased(type, numMinutes);
	}

	public void SetConsumableExpireTime(eCarConsumables type, int MinutesActive)
	{
		DateTime time = GTDateTime.Now.AddMinutes(MinutesActive);
		int val = MinutesActive * 2;
		DateTime time2 = DateTime.MinValue;
        if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
            time2 = ServerSynchronisedTime.Instance.GetDateTime().AddMinutes(MinutesActive);
		}
		SetConsumableExpireTime(type, time);
		SetConsumableExpireTimeSync(type, time2);
		SetConsumableAntiCheatRacesLeft(type, val);
		SetConsumableCumulativeMinutesPurchased(type, MinutesActive);
	}

	private int GetConsumableNumMinutesLastPurchased(eCarConsumables type)
	{
		return ProfileData.Consumables[type].CmlMinutesPurchased;
	}

	private void SetConsumableCumulativeMinutesPurchased(eCarConsumables type, int numMinutes)
	{
		ProfileData.Consumables[type].CmlMinutesPurchased = numMinutes;
	}

	public void SetConsumableRacesLeft(eCarConsumables type, int RacesLeft)
	{
		SetConsumableCumulativeMinutesPurchased(type, 0);
		ProfileData.Consumables[type].RacesLeft = RacesLeft;
	}

	public bool AnyConsumablesAvailableToBuy()
	{
		return !IsConsumableActive(eCarConsumables.EngineTune) || !IsConsumableActive(eCarConsumables.PRAgent) || !IsConsumableActive(eCarConsumables.Nitrous) || !IsConsumableActive(eCarConsumables.Tyre);
	}

	public bool AnyConsumablesActive()
	{
		return IsConsumableActive(eCarConsumables.EngineTune) || IsConsumableActive(eCarConsumables.PRAgent) || IsConsumableActive(eCarConsumables.Nitrous) || IsConsumableActive(eCarConsumables.Tyre);
	}

	public void DisplayedUnlimitedFuelRaceTeamPopup()
	{
		PlayerProfileData expr_0B_cp_0 = ProfileData;
		expr_0B_cp_0.UnlimitedFuel.RaceTeamPopupTimesSeen = expr_0B_cp_0.UnlimitedFuel.RaceTeamPopupTimesSeen + 1;
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
	}

    public bool HasSeasonPrizeBeenAwarded(SeasonPrizeIdentifier toCheck)
    {
        for (int i = 0; i < this.ProfileData.SeasonPrizesAwarded.Count; i++)
        {
            if (this.ProfileData.SeasonPrizesAwarded[i].Equals(toCheck))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasSeasonPrizeBeenAwarded(int leaderBoardID)
    {
        foreach (SeasonPrizeIdentifier current in this.ProfileData.SeasonPrizesAwarded)
        {
            if (current.LeaderboardID == leaderBoardID)
            {
                return true;
            }
        }
        return false;
    }

    public void AwardSeasonPrize(SeasonPrizeIdentifier toCheck, SeasonPrizeMetadata prizeData)
    {
        if (this.HasSeasonPrizeBeenAwarded(toCheck))
        {
            return;
        }
        if (prizeData == null)
        {
            return;
        }
        this.ProfileData.SeasonPrizesAwarded.Add(toCheck);
        switch (prizeData.Type)
        {
            case SeasonPrizeMetadata.ePrizeType.Car:
                {
                    string numberPlate = string.Empty;
                    SeasonEventMetadata eventForLeaderboard = SeasonServerDatabase.Instance.GetEventForLeaderboard(toCheck.LeaderboardID);
                    if (eventForLeaderboard != null)
                    {
                        numberPlate = string.Format(LocalizationManager.GetTranslation("TEXT_SEASON_SHORT"), eventForLeaderboard.SeasonDisplayNumber);
                    }
                    this.AwardCar(prizeData.Data, prizeData.AwardedCarIsPro, numberPlate);
                    break;
                }
            case SeasonPrizeMetadata.ePrizeType.Gold:
                {
                    int num = 0;
                    if (prizeData.GetPrizeDataAsInt(out num) && num > 0)
                    {
                        this.AddGold(num,"reward","AwardSeasonPrize");
                    }
                    break;
                }
            case SeasonPrizeMetadata.ePrizeType.Cash:
                {
                    int num2 = 0;
                    if (prizeData.GetPrizeDataAsInt(out num2) && num2 > 0)
                    {
                        this.AddCash(num2,"reward", "AwardSeasonPrize");
                    }
                    break;
                }
        }
    }

	public void AwardCar(string carDBKey, bool asPro, string numberPlate)
	{
		CarInfo car = CarDatabase.Instance.GetCar(carDBKey);
		if (car == CarDatabase.Instance.GetDefaultCar())
		{
			return;
		}
		if (!IsCarOwned(carDBKey))
		{
			GiveCar(carDBKey, 0, false);
		}
        //if (asPro)
        //{
        //    this.AddPrizeToWonMPWonCars(carDBKey);
        //}
		CurrentlySelectedCarDBKey = carDBKey;
		GetCurrentCar().EliteCar = asPro;
		GetCurrentCar().NumberPlate.Text = numberPlate;
		PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
		if (GetCurrentCar().EliteCar)
		{
			ApplyEliteLiveryToCurrentCar();
		}
	}

	public List<int> GetPrizesAwardedForSeason(int leaderboardID)
	{
		List<int> list = new List<int>();
        //foreach (SeasonPrizeIdentifier current in this.ProfileData.SeasonPrizesAwarded)
        //{
        //    if (current.LeaderboardID == leaderboardID)
        //    {
        //        list.Add(current.PrizeID);
        //    }
        //}
		return list;
	}

	private void ApplyEliteLiveryToCurrentCar()
	{
		string value = CurrentlySelectedCarDBKey + "_Livery";
		List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(GTAssetTypes.livery);
		string appliedLiveryName = string.Empty;
		foreach (AssetDatabaseAsset current in assetsOfType)
		{
			string code = current.code;
			if (code.StartsWith(value))
			{
				if (code.ToLower().Contains("elite"))
				{
					appliedLiveryName = code;
				}
			}
		}
		GetCurrentCar().AppliedLiveryName = appliedLiveryName;
	}

	private void ApplyEliteLiveryToCar(string key)
	{
		string value = key + "_Livery";
		List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(GTAssetTypes.livery);
		string appliedLiveryName = string.Empty;
		foreach (AssetDatabaseAsset current in assetsOfType)
		{
			string code = current.code;
			if (code.StartsWith(value))
			{
				if (code.ToLower().Contains("elite"))
				{
					appliedLiveryName = code;
				}
			}
		}
		GetCarFromID(key).AppliedLiveryName = appliedLiveryName;
	}

	public void ApplyUnlockableLiveryToCurrentCar(string liveryID)
	{
		List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(GTAssetTypes.livery);
		string text = CurrentlySelectedCarDBKey + "_Livery_Unlockable_" + liveryID;
		foreach (AssetDatabaseAsset current in assetsOfType)
		{
			string code = current.code;
			if (code.Equals(text))
			{
				GetCurrentCar().AppliedLiveryName = text;
				break;
			}
		}
	}

	public int GetTotalGaragePP()
	{
		PlayerProfileData profileData = ProfileData;
		int num = 0;
		foreach (CarGarageInstance current in profileData.CarsOwned)
		{
			num += current.CurrentPPIndex;
		}
		return num;
	}

	public void VideoForFuelTimestamp()
	{
		RemoveVideoForFuelTimestampsOlderThan24H();
		DateTime utcNow = GTDateTime.Now;
		ProfileData.VideoForFuelTimestamps.Add(utcNow);
	}

	public bool CanWatchVideoForFuel(int limitLast24H)
	{
		RemoveVideoForFuelTimestampsOlderThan24H();
		return ProfileData.VideoForFuelTimestamps.Count < limitLast24H;
	}

	private void RemoveVideoForFuelTimestampsOlderThan24H()
	{
	    var timeSpan = new TimeSpan(1, 0, 0, 0);
	    var time = GTDateTime.Now;
        DateTime timestamp24HAgo = time - timeSpan;
		ProfileData.VideoForFuelTimestamps.RemoveAll((DateTime p) => p < timestamp24HAgo);
	}

	public List<DateTime> GetVideoForFuelTimestamps()
	{
		return ProfileData.VideoForFuelTimestamps;
	}

    private int DetermineNextPrize(Dictionary<int, CachedFriendRaceData> friends, List<KeyValuePair<int, int>> orphanedPrizes)
    {
        int result = 0;
        if (orphanedPrizes.Count > 0)
        {
            int key = orphanedPrizes.First<KeyValuePair<int, int>>().Key;
            result = orphanedPrizes.First<KeyValuePair<int, int>>().Value;
            this.ProfileData.KnownFriendsPrizes[key] = 0;
            orphanedPrizes.RemoveAt(0);
        }
        else if (friends.Count > this.ProfileData.PeakFriends)
        {
            int num = ++this.ProfileData.PeakFriends;
            if (GameDatabase.Instance.Friends.FriendCountHasPrize(num))
            {
                result = num;
            }
        }
        return result;
    }

    public void UpdateFriends(Dictionary<int, CachedFriendRaceData> friends, CachedFriendRaceData playerFriendLump)
    {
        bool flag = false;
        List<string> newNetworks = playerFriendLump.ServiceID.FindAll((string id) => !this.ProfileData.SyncdServiceID.Contains(id));
        newNetworks = (from id in newNetworks
                       select SocialFriendsManager.Instance.GetNetworkIDPrefix(id)).ToList<string>();
        if (newNetworks.Count > 0)
        {
        }
        List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
        foreach (KeyValuePair<int, int> current in this.ProfileData.KnownFriendsPrizes)
        {
            if (current.Value > 0 && !friends.ContainsKey(current.Key))
            {
                list.Add(current);
            }
        }
        foreach (int current2 in friends.Keys)
        {
            if (!this.ProfileData.KnownFriendsPrizes.ContainsKey(current2))
            {
                int num = 0;
                if (!friends[current2].ServiceID.All(delegate(string friend_id)
                {
                    string friendNetworkId = SocialFriendsManager.Instance.GetNetworkIDPrefix(friend_id);
                    return newNetworks.Any((string newNetwork_id) => newNetwork_id == friendNetworkId);
                }))
                {
                    num = this.DetermineNextPrize(friends, list);
                }
                else
                {
                    this.ProfileData.PeakFriends++;
                    this.ProfileData.InitialNetworkFriends++;
                }
                this.ProfileData.KnownFriendsPrizes.Add(current2, num);
                if (num > 0)
                {
                    flag = true;
                }
            }
        }
        while (this.ProfileData.PeakFriends < friends.Count)
        {
            int key = this.ProfileData.KnownFriendsPrizes.FirstOrDefault((KeyValuePair<int, int> x) => x.Value == 0 && friends.ContainsKey(x.Key)).Key;
            int num2 = this.DetermineNextPrize(friends, list);
            this.ProfileData.KnownFriendsPrizes[key] = num2;
            if (num2 != 0)
            {
                flag = true;
            }
        }
        this.ProfileData.SyncdServiceID = playerFriendLump.ServiceID;
        if (flag)
        {
            this.Save();
        }
    }

    public bool HasWonAllFriendsCars()
    {
        List<string> list = GameDatabase.Instance.Friends.AllRewardableCars();
        foreach (string current in list)
        {
            if (!this.IsCarOwned(current))
            {
                return false;
            }
        }
        return true;
    }

	public int GetUncollectedPrizeForFriend(int uid)
	{
		int result = 0;
		if (ProfileData.KnownFriendsPrizes.ContainsKey(uid))
		{
			result = Math.Max(0, ProfileData.KnownFriendsPrizes[uid]);
		}
		return result;
	}

	public bool GetFriendSyncComplete()
	{
		return ProfileData.SyncdServiceID.Count > 0;
	}

	public void SetFriendPrizeCollected(int prize_index)
	{
		int key = ProfileData.KnownFriendsPrizes.FirstOrDefault((KeyValuePair<int, int> x) => x.Value == prize_index).Key;
		if (key != 0)
		{
			ProfileData.KnownFriendsPrizes[key] = -ProfileData.KnownFriendsPrizes[key];
		}
	}

	public void ResetFriends()
	{
		ProfileData.ResetFriends();
		Save();
	}

	public bool HasSeenRaceWithFriendConditionCombo(int friendID, string carDBKey)
	{
		return ProfileData.RaceWithFriendConditionCombos.Contains(new KeyValuePair<int, string>(friendID, carDBKey));
	}

	public void DisplayedGasTankReminder()
	{
		ProfileData.GasTankReminderNumberOfTimesShown++;
		ProfileData.GasTankReminderLastTimeShown = GTDateTime.Now;
		ProfileData.GasTankReminderIDShown = (ProfileData.GasTankReminderIDShown + 1) % 2;
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
	}

	public int GasTankMessageDurationSinceLastShownInMinutes()
	{
		return (int)(GTDateTime.Now - ProfileData.GasTankReminderLastTimeShown).TotalMinutes;
	}

	public void SetLastShownRestrictionEventID(eCarTier tier, int eventID)
	{
        this.ProfileData.RestrictionRaceData.SetLastShownEventID(tier, eventID);
	}

	public int GetLastShownRestrictionEventID(eCarTier tier)
	{
	    return this.ProfileData.RestrictionRaceData.GetLastShownEventID(tier);
	}

	private void EnableFuelAdPrompt(int i)
	{
		while (ProfileData.FuelAdPromptToggles.Count <= i)
		{
			ProfileData.FuelAdPromptToggles.Add(true);
		}
		ProfileData.FuelAdPromptToggles[i] = true;
	}

	public void DisableFuelAdPrompt(int i)
	{
		while (ProfileData.FuelAdPromptToggles.Count <= i)
		{
			ProfileData.FuelAdPromptToggles.Add(true);
		}
		ProfileData.FuelAdPromptToggles[i] = false;
	}

	public bool FuelAdPromptEnabled(int i)
	{
		return ProfileData.FuelAdPromptToggles.Count <= i || ProfileData.FuelAdPromptToggles[i];
	}

	private void EnableDailyBattlePrompt()
	{
		ProfileData.DailyBattlePromptToggle = true;
	}

	public void DisableDailyBattlePrompt()
	{
		ProfileData.DailyBattlePromptToggle = false;
	}

	public void CarCashbackDealShown(string car)
	{
		ProfileData.CarDeals.RacesSinceLastDeal = 0;
		ProfileData.CarDeals.LastCarOffered = car;
		ProfileData.CarDeals.LastDealWasCashback = true;
	}

	public void CarDiscountDealShown(string car, int discount)
	{
		ProfileData.CarDeals.RacesSinceLastDeal = 0;
		ProfileData.CarDeals.LastCarOffered = car;
		ProfileData.CarDeals.LastDealWasCashback = false;
		if (discount == ProfileData.CarDeals.LastDiscount)
		{
			ProfileData.CarDeals.LastDiscountRepeatCount++;
		}
		else
		{
			ProfileData.CarDeals.LastDiscount = discount;
			ProfileData.CarDeals.LastDiscountRepeatCount = 0;
		}
	}

	public void CarFreeUpgradeDealShown()
	{
		ProfileData.CarDeals.RacesSinceLastDeal = 0;
	}

	public void ResetCarDealsDiscount()
	{
		ProfileData.CarDeals.LastDiscount = 0;
	}

	public void CarDealCompleted()
	{
		ResetCarDealsDiscount();
	}

	public eCarTier LastCarDealTier()
	{
		eCarTier result = eCarTier.MAX_CAR_TIERS;
		if (!string.IsNullOrEmpty(LastCarDealCarOffered))
		{
			CarInfo car = CarDatabase.Instance.GetCar(LastCarDealCarOffered);
			if (car != null)
			{
				result = car.BaseCarTier;
			}
		}
		return result;
	}

	public void ConfirmDailyBattleGoldAwarded(int amount)
	{
		ProfileData.DailyBattleGoldConfirmed += amount;
	}

	public void ConfirmDailyBattleCashAwarded(int amount)
	{
		ProfileData.DailyBattleCashConfirmed += amount;
	}

	public void LogDailyBattleReward(int consecutiveDays, bool playerWon)
	{
		DailyBattleCompletionRecord dailyBattleCompletionRecord = new DailyBattleCompletionRecord();
		dailyBattleCompletionRecord.RewardId = consecutiveDays;
		dailyBattleCompletionRecord.Tier = RaceEventQuery.Instance.getHighestUnlockedClass();
        dailyBattleCompletionRecord.When = GTDateTime.Now;
		dailyBattleCompletionRecord.PlayerWon = playerWon;
		ProfileData.UnconfirmedDailyBattleResults.Add(dailyBattleCompletionRecord);
	}

	private bool UpdateDailyBattleRewardsFromNetworkTime()
	{
	    if (!Application.isPlaying)
	        return false;
		bool result = false;
        if (ServerSynchronisedTime.Instance!=null && ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			List<DailyBattleCompletionRecord> list = new List<DailyBattleCompletionRecord>();
            DateTime dateTime = GTDateTime.Now;
			TimeSpan t = TimeSpan.FromDays(1.0);
			foreach (DailyBattleCompletionRecord current in ProfileData.UnconfirmedDailyBattleResults)
			{
				TimeSpan t2 = dateTime - current.When;
				if (t2 >= t)
				{
                    //DailyBattleRewardManager.Instance.GetReward(current.RewardId, current.Tier, current.PlayerWon).ConfirmInProfile(this);
				}
				else if (t2 <= -TimeSpan.FromDays(1.0))
				{
                    //DailyBattleRewardManager.Instance.GetReward(current.RewardId, current.Tier, current.PlayerWon).RemoveFromProfile(this, DailyBattleRewardManager.Instance.AllowSubZeroPrizeDeductions);
					ProfileData.DailyBattleRewardsRemoved++;
					Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
					{
						{
							Parameters.DBRewardsRemoved,
							ProfileData.DailyBattleRewardsRemoved.ToString()
						},
						{
							Parameters.DBRewardId,
							current.RewardId.ToString()
						},
						{
							Parameters.DBRewardTier,
							current.Tier.ToString()
						},
						{
							Parameters.DBRewardDate,
							current.When.ToString("yyyy-MM-dd")
						}
					};
					Log.AnEvent(Events.DailyBattleRewardsRemoved, data);
					result = true;
				}
				else
				{
					list.Add(current);
				}
				ProfileData.UnconfirmedDailyBattleResults = list;
			}
		}
		return result;
	}

	public bool IsAnimationCompletedForWorldTourEventID(string ThemeID, string eventID)
	{
        WorldTourAnimationsCompleted worldTourAnimationsCompleted = this.ProfileData.WorldTourEventIDsWithAnimitionCompleted.FirstOrDefault((WorldTourAnimationsCompleted q) => q.ThemeID == ThemeID);
        return worldTourAnimationsCompleted != null && worldTourAnimationsCompleted.AmimCompletedForTheme.Contains(eventID);
	}

    public void SetAnimationCompletedForWorldTourEventID(string ThemeID, string eventID)
	{
        WorldTourAnimationsCompleted worldTourAnimationsCompleted = this.ProfileData.WorldTourEventIDsWithAnimitionCompleted.FirstOrDefault((WorldTourAnimationsCompleted t) => t.ThemeID == ThemeID);
        if (worldTourAnimationsCompleted == null)
        {
            WorldTourAnimationsCompleted worldTourAnimationsCompleted2 = new WorldTourAnimationsCompleted();
            worldTourAnimationsCompleted2.ThemeID = ThemeID;
            worldTourAnimationsCompleted2.AmimCompletedForTheme.Add(eventID);
            this.ProfileData.WorldTourEventIDsWithAnimitionCompleted.Add(worldTourAnimationsCompleted2);
        }
        else
        {
            worldTourAnimationsCompleted.AmimCompletedForTheme.Add(eventID);
        }
	}

	public void ClearWorldTourAnimationsCompleteList()
	{
        this.ProfileData.WorldTourEventIDsWithAnimitionCompleted.Clear();
	}

	public bool HasSeenSeasonUnlockableTheme(string themeID)
	{
		return ProfileData.SeasonUnlockableThemesSeen.Contains(themeID);
	}

	public void SetHasSeenSeasonUnlockableTheme(string themeID)
	{
		if (!ProfileData.SeasonUnlockableThemesSeen.Contains(themeID))
		{
			ProfileData.SeasonUnlockableThemesSeen.Add(themeID);
		}
	}

	public int GetPopupSeenCount(int popupID)
	{
		return ProfileData.PopupsData.GetPopupSeenCount(popupID);
	}
	
	public void ResetPopupSeenCount(int popupID)
	{
		ProfileData.PopupsData.ResetPopupSeenCount(popupID);
	}

	public void IncreasePopupSeenCount(int popupID)
	{
		ProfileData.PopupsData.IncreasePopupSeenCount(popupID);
	}

	public DateTime GetPopupFirstSeenTime(int popupID)
	{
		return ProfileData.PopupsData.GetPopupFirstSeenTime(popupID);
	}

	public void SetPopupFirstSeenTime(int popupID)
	{
		ProfileData.PopupsData.SetPopupFirstSeenTime(popupID);
	}

	public bool IsPopupValid(int popupID)
	{
		return ProfileData.PopupsData.IsPopupValid(popupID);
	}

	public void SetPopupIsValid(int popupID, bool valid)
	{
		ProfileData.PopupsData.SetPopupIsValid(popupID, valid);
	}

	public void ResetPopupData(int popupID)
	{
		ProfileData.PopupsData.ResetPopupData(popupID);
	}

	public bool HasCompletedFirstCrewRace()
	{
		RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
		RaceEventData raceEventData = careerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups[0].RaceEvents[0];
		return EventsCompleted.Contains(raceEventData.EventID);
	}

	public bool HasCompletedFirstThreeTutorialRaces()
	{
	    return IsEventCompleted(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial.EventID)
	           && IsEventCompleted(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2.EventID)
	           && IsEventCompleted(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial3.EventID);
	}

    public bool HasCompletedFirstTutorialRace()
    {
        return IsEventCompleted(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial.EventID);
    }

    public bool HasCompletedSecondTutorialRace()
    {
        return IsEventCompleted(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2.EventID);
    }

	public int GetTutorialBubbleSeenCount(int bubbleID)
	{
		return ProfileData.TutorialBubblesData.GetTutorialBubbleSeenCount(bubbleID);
	}

	public void IncrementTutorialBubbleSeenCount(int bubbleID)
	{
		ProfileData.TutorialBubblesData.IncrementTutorialBubbleSeenCount(bubbleID);
	}

	public bool HasDismissedTutorialBubble(int bubbleID)
	{
		return ProfileData.TutorialBubblesData.HasDismissedTutorialBubble(bubbleID);
	}

	public void SetTutorialBubbleDismissed(int bubbleID)
	{
		ProfileData.TutorialBubblesData.SetTutorialBubbleDismissed(bubbleID);
	}

	[Conditional("CSR_DEBUGMENU")]
	public void ResetTutorialBubblesData()
	{
	}

	public void ForcePlayerIntoValidOwnedCar()
	{
		if (!HasBoughtFirstCar)
		{
			return;
		}
		if (!CarsOwned.Any((CarGarageInstance car) => car.CarDBKey == CurrentlySelectedCarDBKey) || !CarDatabase.Instance.GetAllCars().Any((CarInfo car) => car.Key == CurrentlySelectedCarDBKey))
		{
			GetIntoFastestCar();
		}
	}

	public void GetIntoFastestCar()
	{
		CarGarageInstance carGarageInstance = (from car in CarsOwned
		orderby car.CurrentPPIndex descending
		select car).FirstOrDefault<CarGarageInstance>();
		if (carGarageInstance != null)
		{
			CurrentlySelectedCarDBKey = carGarageInstance.CarDBKey;
			CarInfoUI.Instance.SetCurrentCarIDKey(carGarageInstance.CarDBKey);
		}
	}

	public void RecordSelectedPinID(string theme, string pin)
	{
		ProfileData.TierXPinSelections[theme] = pin;
	}

	public string GetLastSelectedPinID(string theme)
	{
		string empty = string.Empty;
		ProfileData.TierXPinSelections.TryGetValue(theme, out empty);
		return empty;
	}

    public MultiplayerEventsProfileData GetMultiplayerEventsProfileData()
    {
        return this.ProfileData.MultiplayerEventsData;
    }

    public int GetMultiplayerEventID()
    {
        return this.ProfileData.MultiplayerEventsData.GetMultiplayerEventProfileData().ID;
    }

	public void SetMultiplayerEventEntered()
	{
        this.ProfileData.MultiplayerEventsData.SetEntered();
	}

	public bool HasMultiplayerEventBeenEntered()
	{
	    return this.ProfileData.MultiplayerEventsData.HasBeenEntered();
	}

	public void AddMultiplayerEventProgression(float quantity)
	{
        this.ProfileData.MultiplayerEventsData.AddProgression(quantity);
	}

	public void SetMultiplayerEventProgression(float quantity)
	{
        this.ProfileData.MultiplayerEventsData.SetProgression(quantity);
	}

	public float GetMultiplayerEventProgression()
	{
	    return this.ProfileData.MultiplayerEventsData.GetProgression();
	}

	public float GetLastSeenMultiplayerEventProgression()
	{
	    return this.ProfileData.MultiplayerEventsData.GetLastSeenProgression();
	}

	public void SetSeenMultiplayerEventProgression()
	{
        this.ProfileData.MultiplayerEventsData.SetSeenProgression();
	}

	public void SetMultiplayerEventSpotPrizeAwarded(int spotPrizeID)
	{
        this.ProfileData.MultiplayerEventsData.SetSpotPrizeAwarded(spotPrizeID);
	}

	public bool GetMultiplayerEventSpotPrizeAwarded(int spotPrizeID)
	{
	    return this.ProfileData.MultiplayerEventsData.GetSpotPrizeAwarded(spotPrizeID);
	}

	public void AddMultiplayerEventRacesCompleted(int raceCount)
	{
        this.ProfileData.MultiplayerEventsData.AddRacesCompleted(raceCount);
	}

	public int GetMultiplayerEventRacesCompleted()
	{
	    return  this.ProfileData.MultiplayerEventsData.GetRacesCompleted();
	}

	public void UpdateMultiplayerEventProfileData(int ID)
	{
        this.ProfileData.MultiplayerEventsData.UpdateMultiplayerEventProfileData(ID);
	}

	public void AddMultiplayerEventRPBonus(int eventID, int spotPrizeID, DateTime startTime)
	{
        this.ProfileData.MultiplayerEventsData.AddRPBonus(eventID, spotPrizeID, startTime);
	}

	public void AwardUnlockableLivery(string carID, string liveryID)
	{
		CarGarageInstance carFromID = GetCarFromID(carID);
		if (carFromID != null && !carFromID.UnlockedLiveries.Contains(liveryID))
		{
			string item = carID + "_Livery_Unlockable_" + liveryID;
			carFromID.OwnedLiveries.Add(item);
			carFromID.NewLiveries.Add(item);
			carFromID.UnlockedLiveries.Add(liveryID);
			CurrentlySelectedCarDBKey = carID;
		}
	}

    public PlayerProfileData GetProfileData()
    {
        return ProfileData;
    }

    public void SetProfileData(PlayerProfileData playerProfileData)
    {
        ProfileData = playerProfileData;
    }

    public int GetTotalPurchasePriceOfCarsOwned()
    {
        if (!this.HasBoughtFirstCar)
        {
            return 0;
        }
        int num = 0;
        foreach (CarGarageInstance current in this.ProfileData.CarsOwned)
        {
            num += current.GarageBaseCashValue;
            num += current.CalculateCashValueUpgradeBonus();
            //num += current.CalculateCashValueFusionBonus();
        }
        return num;
    }

    public bool IsNextSMPChallengeAvailable()
    {
        TimeSpan nextSMPWinChallengeRemainingTime = this.GetNextSMPWinChallengeRemainingTime();
        return nextSMPWinChallengeRemainingTime <= TimeSpan.Zero;
    }

    public TimeSpan GetNextSMPWinChallengeRemainingTime()
    {
        TimeSpan timeSpan = SMPConfigManager.WinStreak.WinChallengeCooldown - this.GetTimeSinceSMPWinChallengeActivation();
        if (timeSpan < TimeSpan.Zero)
        {
            timeSpan = TimeSpan.Zero;
        }
        return timeSpan;
    }

    public TimeSpan GetTimeSinceSMPWinChallengeActivation()
    {
        return ServerSynchronisedTime.Instance.GetDateTime() - this.SMPWinChallengeActivationTime;
    }

    public void ResetSMPChallengeWins()
    {
        this.SMPChallengeWins = 0;
    }

    public void AddGachaKeys(int keys, GachaType eGachaType, EGachaKeysEarnedReason reason)
    {
        if (eGachaType != GachaType.Invalid && eGachaType < GachaType.MaxGachaTypes)
        {
            if (keys > 0)
            {
                GachaKeysEarnedTransaction transaction = new GachaKeysEarnedTransaction(keys, eGachaType, reason);
                PlayerProfileManager.Instance.PendingTransactions.AddTransaction(transaction);
                switch (eGachaType)
                {
                    case GachaType.Bronze:
                        this.GachaBronzeKeysEarnedSetAndMangle = this.GachaBronzeKeysEarned + keys;
                        break;
                    case GachaType.Silver:
                        this.GachaSilverKeysEarnedSetAndMangle = this.GachaSilverKeysEarned + keys;
                        break;
                    case GachaType.Gold:
                        this.GachaGoldKeysEarnedSetAndMangle = this.GachaGoldKeysEarned + keys;
                        break;
                }
                //if (this.OnGachaKeysEarned != null)
                //{
                //    this.OnGachaKeysEarned(keys);
                //}
            }
        }
    }

    public int GetCurrentGachaKeys(GachaType eGachaType)
    {
        if (eGachaType != GachaType.Invalid && eGachaType < GachaType.MaxGachaTypes)
        {
            int num = 0;
            if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
            {
                switch (eGachaType)
                {
                    case GachaType.Bronze:
                        num = UserManager.Instance.currentAccount.IAPKeys_Bronze + (this.GachaBronzeKeysEarned - this.GachaBronzeKeysSpent);
                        break;
                    case GachaType.Silver:
                        num = UserManager.Instance.currentAccount.IAPKeys_Silver + (this.GachaSilverKeysEarned - this.GachaSilverKeysSpent);
                        break;
                    case GachaType.Gold:
                        num = UserManager.Instance.currentAccount.IAPKeys_Gold + (this.GachaGoldKeysEarned - this.GachaGoldKeysSpent);
                        break;
                }
            }
            if (num < 0)
            {
                num = 0;
            }
            return num;
        }
        return 0;
    }

    public void AddIAPSpentCash(int value)
    {
        //this.IAPCashSpentAndMangle = this.IAPCashSpent + value;
    }

    public void AddIAPSpentGold(int value)
    {
        //this.IAPGoldSpentSetAndMangle = this.IAPGoldSpent + value;
    }

    public void AddIAPSpentGachaKeys(EMetricsCurrencyType currencyType, int paidSpend)
    {
        switch (currencyType)
        {
            case EMetricsCurrencyType.Keys_Bronze:
                //this.IAPGachaBronzeKeysSpentSetAndMangle = this.IAPGachaBronzeKeysSpent + paidSpend;
                break;
            case EMetricsCurrencyType.Keys_Silver:
                //this.IAPGachaSilverKeysSpentSetAndMangle = this.IAPGachaSilverKeysSpent + paidSpend;
                break;
            case EMetricsCurrencyType.Keys_Gold:
                //this.IAPGachaGoldKeysSpentSetAndMangle = this.IAPGachaGoldKeysSpent + paidSpend;
                break;
        }
    }
    
}
