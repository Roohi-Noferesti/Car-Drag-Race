using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DataSerialization;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class MetricsCalculate
{
	private delegate string MetricsCalcDelegate();

	private const int SESSION_EVENT_HISTORY = 8;

	private static bool sessionLoggedIn;

	private static int sessionMPCardsStart;

    private static DateTime sessionTimeDeltaStart = GTDateTime.Now;

	private static Queue<Events> sessionHistoryEvents = new Queue<Events>();

	private static Dictionary<Parameters, MetricsCalcDelegate> calcs = new Dictionary<Parameters, MetricsCalcDelegate>
	{
		{
			Parameters.nmcoreid,
			MetricsIntegration.GetNMCoreIDSafe
		},
		{
			Parameters.bckt,
			CurrentTestBucket
		},
		{
			Parameters.brnch,
			CurrentTestBranch
		},
		{
			Parameters.tst,
			CurrentTestCode
		},
		{
			Parameters.OSVersion,
			OSVersion
		},
		{
			Parameters.BCsh,
			CurrentCash
		},
		{
			Parameters.BGld,
			CurrentGold
		},
		{
			Parameters.BXp,
			CurrentXP
		},
		{
			Parameters.Bfuel,
			CurrentFuel
		},
		{
			Parameters.Banned,
			IsBanned
		},
		{
			Parameters.RTier,
			RaceTier
		},
		{
			Parameters.Rname,
			RaceName
		},
		{
			Parameters.Rlength,
			RaceLength
		},
		{
			Parameters.Rematch,
			Rematch
		},
		{
			Parameters.RDiff,
			RaceDifficulty
		},
		{
			Parameters.OpCr,
			AICarName
		},
		{
			Parameters.Rwd,
			AICarName
		},
		{
			Parameters.OpCrPP,
			AIPP
		},
		{
			Parameters.Ver,
			GameVersion
		},
		{
			Parameters.Platform,
			GamePlatform
		},
		{
			Parameters.PPDelta,
			PPDelta
		},
		{
			Parameters.PFin,
			PlayerFinishTime
		},
		{
			Parameters.PResult,
			PlayerResult
		},
		{
			Parameters.Pntrs,
			NitrousUsed
		},
		{
			Parameters.OpFin,
			AIFinishTime
		},
		{
			Parameters.PErlShft,
			EarlyShifts
		},
		{
			Parameters.PGdShft,
			GoodShifts
		},
		{
			Parameters.PPrfShft,
			PerfectShifts
		},
		{
			Parameters.PLteShft,
			LateShifts
		},
		{
			Parameters.PGrtLnch,
			PerfectStart
		},
		{
			Parameters.PTrmSpd,
			ResultTerminalSpeed
		},
		{
			Parameters.PQck60ft,
			ResultBest0To60
		},
		{
			Parameters.PBstAwd,
			ResultNewPB
		},
		{
			Parameters.P0to60,
			ResultPlayer0To60
		},
		{
			Parameters.P0to100,
			ResultPlayer0To100
		},
		{
			Parameters.PMchR,
			MechanicActive
		},
		{
			Parameters.MlstRce,
			MilestoneRace
		},
		{
			Parameters.CmlMchR,
			CumulativeMechanicFettledRaces
		},
		{
			Parameters.Cmltwt,
			CumulativeTweets
		},
		{
			Parameters.Cmlfb,
			CumulativeFBPosts
		},
		{
			Parameters.CmlCr,
			CumulativeCarsOwned
		},
		{
			Parameters.CmlSns,
			CumulativeSessions
		},
		{
			Parameters.CmlCsh,
			CumulativeCash
		},
		{
			Parameters.CmlGld,
			CumulativeGold
		},
		{
			Parameters.CmlCshPur,
			CumulativeCashPurchased
		},
		{
			Parameters.CmlGldPur,
			CumulativeGoldPurchased
		},
		{
			Parameters.RP,
			CurrentRankPoints
		},
		{
			Parameters.SessionNum,
			CumulativeSessions
		},
		{
			Parameters.CmlUpg,
			CumulativeUpgradesBoughtForCurrentCar
		},
		{
			Parameters.CmlRCmp,
			CumulativeRacesCompleted
		},
		{
			Parameters.PLvl,
			CurrentLevel
		},
		{
			Parameters.MPRaceTotal,
			CurrentMPTotalRaces
		},
		{
			Parameters.MPConsecWin,
			ConsecutiveMPWins
		},
		{
			Parameters.MPDiff,
			MPDifficulty
		},
		{
			Parameters.PerfTime,
			PerfectTime
		},
		{
			Parameters.MPElDiff,
			MPEliteDifficulty
		},
		{
			Parameters.MPEvDiff,
			MPEventDifficulty
		},
		{
			Parameters.MPWinRate,
			CurrentMPWinRate
		},
		{
			Parameters.SPWinRate,
			CurrentSPWinRate
		},
		{
			Parameters.CardsWon,
			CurrentMPCardsWon
		},
		{
			Parameters.tme,
			CurrentTimeDelta
		},
		{
			Parameters.Upsl,
			IsUpSell
		},
		{
			Parameters.lgdin,
			LoggedIn
		},
		{
			Parameters.SessionCardCount,
			SessionPrizeCardCount
		},
		{
			Parameters.H1,
			GetHistoryEvent1
		},
		{
			Parameters.H2,
			GetHistoryEvent2
		},
		{
			Parameters.H3,
			GetHistoryEvent3
		},
		{
			Parameters.H4,
			GetHistoryEvent4
		},
		{
			Parameters.H5,
			GetHistoryEvent5
		},
		{
			Parameters.H6,
			GetHistoryEvent6
		},
		{
			Parameters.H7,
			GetHistoryEvent7
		},
		{
			Parameters.H8,
			GetHistoryEvent8
		},
		{
			Parameters.PCr,
			PlayerCar
		},
		{
			Parameters.PCrPP,
			PlayerCarPP
		},
		{
			Parameters.PCrPUpgdPP,
			PlayerCarPostUpgradePP
		},
		{
			Parameters.Eng,
			EngineLevel
		},
		{
			Parameters.Bdy,
			BodyLevel
		},
		{
			Parameters.Trns,
			GearBoxLevel
		},
		{
			Parameters.Trs,
			TyresLevel
		},
		{
			Parameters.Trb,
			TurboLevel
		},
		{
			Parameters.Nit,
			NitroLevel
		},
		{
			Parameters.Intk,
			IntakeLevel
		},
		{
			Parameters.Snapshot,
			SnapshotNumber
		},
		{
			Parameters.FriendsRaces,
			NumberFriendsRaces
		},
		{
			Parameters.FriendsRacesWon,
			NumberFriendsRacesWon
		},
		{
			Parameters.FriendsGold,
			FriendsGold
		},
		{
			Parameters.FriendsCarsWon,
			FriendsCarsWon
		},
		{
			Parameters.ThreeStarCars,
			ThreeStarCars
		},
		{
			Parameters.CmlStars,
			TotalStars
		},
		{
			Parameters.Friends,
			NumberFriends
		},
		{
			Parameters.CmlFriendsInv,
			TotalFriendsInvited
		},
		{
			Parameters.PeakFriends,
			PeakFriends
		},
		{
			Parameters.InitialFriends,
			InitialFriends
		},
		{
			Parameters.HasRecievedFriendSync,
			HasRecievedFriendSync
		},
		{
			Parameters.BuildVersion,
			BuildVersionNumber
		},
		{
			Parameters.StreakNo,
			StreakNumber
		},
		{
			Parameters.StreakRaceNo,
			StreakRaceNumber
		},
		{
			Parameters.StreakRescued,
			StreakRescued
		},
		{
			Parameters.ConsumableEN,
			ConsumableEN
		},
		{
			Parameters.ConsumableNI,
			ConsumableNI
		},
		{
			Parameters.ConsumableTY,
			ConsumableTY
		},
		{
			Parameters.ConsumableBL,
			ConsumableBL
		},
		{
			Parameters.ConsumableWhole,
			ConsumableWhole
		},
		{
			Parameters.RPBoost,
			RPBoost
		},
		{
			Parameters.Card1,
			PrizeomaticCard1
		},
		{
			Parameters.Card2,
			PrizeomaticCard2
		},
		{
			Parameters.Card3,
			PrizeomaticCard3
		},
		{
			Parameters.FromScreen,
			GetFromScreen
		},
		{
			Parameters.Store,
			GetStore
		},
		{
			Parameters.DealCar,
			GetCarDealCar
		},
		{
			Parameters.DealCarPrice,
			GetCarDealPrice
		},
		{
			Parameters.DealCashback,
			GetCarDealCashback
		},
		{
			Parameters.DealDiscount,
			GetCarDealDiscount
		},
		{
			Parameters.DealFreeUpgradeLevel,
			GetCarDealFreeUpgrades
		},
		{
			Parameters.MusicMuted,
			GetMusicMuted
		},
		{
			Parameters.SFXMuted,
			GetSFXMuted
		},
		{
			Parameters.SystemVolume,
			GetSystemVolume
		},
		{
			Parameters.HeadphonesPluggedIn,
			GetHeadphonesPluggedIn
		},
		{
			Parameters.OtherAudioPlaying,
			GetOtherAudioPlaying
		},
		{
			Parameters.CmlPacks,
			GetCarePackageTotalReceivedCount
		},
		{
			Parameters.DayNum,
			GetDailyRaceNum
		},
		{
			Parameters.PinProg,
			GetProfilePinScheduleMetricsData
		},
		{
			Parameters.StreakDiff,
			GetCurrentStreakDifficulty
		},
		{
			Parameters.RlyLngth,
			GetCurrentRelayLength
		},
		{
			Parameters.RlyResult,
			GetCurrentRelayResult
		},
		{
			Parameters.MPEvtRace,
			GetCurrentMPEventRaceCount
		}
	};

	private static Dictionary<Parameters, string> defaults = new Dictionary<Parameters, string>
	{
		{
			Parameters.DCsh,
			"0"
		},
		{
			Parameters.DGld,
			"0"
		},
		{
			Parameters.DXp,
			"0"
		},
		{
			Parameters.Dfuel,
			"0"
		},
		{
			Parameters.Type,
			"NA"
		},
		{
			Parameters.CostCash,
			"0"
		},
		{
			Parameters.CostGold,
			"0"
		},
		{
			Parameters.Duration,
			"0"
		},
		{
			Parameters.PieceID,
			"0"
		},
		{
			Parameters.TSU,
			"NA"
		},
		{
			Parameters.PreUpgradeDelta,
			"0"
		},
		{
			Parameters.EliteLivID,
			"NA"
		},
		{
			Parameters.Pltfrm,
			"UnknownSocialPlatform"
		},
		{
			Parameters.Intfrm,
			"UnknownSocialIntFrm"
		},
		{
			Parameters.ItmClss,
			string.Empty
		},
		{
			Parameters.Itm,
			string.Empty
		},
		{
			Parameters.Shortcut,
			"0"
		},
		{
			Parameters.RecVer,
			string.Empty
		},
		{
			Parameters.QNam,
			string.Empty
		},
		{
			Parameters.PaidPuzzlePieces,
			"0"
		},
		{
			Parameters.ModeSwitch,
			"Unknown"
		},
		{
			Parameters.frm,
			string.Empty
		},
		{
			Parameters.to,
			string.Empty
		},
		{
			Parameters.QStrt,
			string.Empty
		},
		{
			Parameters.QCmp,
			string.Empty
		},
		{
			Parameters.Anm,
			string.Empty
		},
		{
			Parameters.baid,
			string.Empty
		},
		{
			Parameters.openUDID,
			string.Empty
		},
		{
			Parameters.mac,
			string.Empty
		},
		{
			Parameters.Platform,
			"UnknownOSPlatform"
		},
		{
			Parameters.InvtTyp,
			string.Empty
		},
		{
			Parameters.RPDelta,
			string.Empty
		},
		{
			Parameters.P1PPDelta,
			"NA"
		},
		{
			Parameters.P2PPDelta,
			"NA"
		},
		{
			Parameters.P3PPDelta,
			"NA"
		},
		{
			Parameters.P4PPDelta,
			"NA"
		},
		{
			Parameters.P5PPDelta,
			"NA"
		},
		{
			Parameters.P6PPDelta,
			"NA"
		},
		{
			Parameters.H1,
			"NA"
		},
		{
			Parameters.H2,
			"NA"
		},
		{
			Parameters.H3,
			"NA"
		},
		{
			Parameters.H4,
			"NA"
		},
		{
			Parameters.H5,
			"NA"
		},
		{
			Parameters.H6,
			"NA"
		},
		{
			Parameters.H7,
			"NA"
		},
		{
			Parameters.H8,
			"NA"
		},
		{
			Parameters.CardType,
			string.Empty
		},
		{
			Parameters.CarPart,
			string.Empty
		},
		{
			Parameters.FuelPips,
			string.Empty
		},
		{
			Parameters.CarCompleted,
			"NA"
		},
		{
			Parameters.PurchaseSportsPack,
			"0"
		},
		{
			Parameters.StreakType,
			"NA"
		},
		{
			Parameters.Result,
			"NA"
		},
		{
			Parameters.RaceCount,
			"0"
		},
		{
			Parameters.PlayerCar,
			"NA"
		},
		{
			Parameters.RPStreakDelta,
			"0"
		},
		{
			Parameters.WinningsCash,
			"0"
		},
		{
			Parameters.WinningsCards,
			"0"
		},
		{
			Parameters.Race1PPDelta,
			"0"
		},
		{
			Parameters.Race2PPDelta,
			"0"
		},
		{
			Parameters.Race3PPDelta,
			"0"
		},
		{
			Parameters.Race4PPDelta,
			"0"
		},
		{
			Parameters.Race5PPDelta,
			"0"
		},
		{
			Parameters.Race6PPDelta,
			"0"
		},
		{
			Parameters.Race1RPDelta,
			"0"
		},
		{
			Parameters.Race2RPDelta,
			"0"
		},
		{
			Parameters.Race3RPDelta,
			"0"
		},
		{
			Parameters.Race4RPDelta,
			"0"
		},
		{
			Parameters.Race5RPDelta,
			"0"
		},
		{
			Parameters.Race6RPDelta,
			"0"
		},
		{
			Parameters.Race1Result,
			"0"
		},
		{
			Parameters.Race2Result,
			"0"
		},
		{
			Parameters.Race3Result,
			"0"
		},
		{
			Parameters.Race4Result,
			"0"
		},
		{
			Parameters.Race5Result,
			"0"
		},
		{
			Parameters.Race6Result,
			"0"
		},
		{
			Parameters.StreakRescued,
			"0"
		},
		{
			Parameters.StreakRaceNo,
			"0"
		},
		{
			Parameters.CostAd,
			"0"
		},
		{
			Parameters.Rescue,
			"0"
		},
		{
			Parameters.Consumables,
			"NA"
		},
		{
			Parameters.PLvl,
			"NA"
		},
		{
			Parameters.SsnPID,
			"NA"
		},
		{
			Parameters.SsnAwdT,
			"NA"
		},
		{
			Parameters.SsnAwded,
			"0"
		},
		{
			Parameters.OWPvr,
			string.Empty
		},
		{
			Parameters.AdCfg,
			string.Empty
		},
		{
			Parameters.AdNwk,
			string.Empty
		},
		{
			Parameters.AdSpace,
			"None"
		},
		{
			Parameters.AdRaces,
			"-1"
		},
		{
			Parameters.AdsPerSession,
			"-1"
		},
		{
			Parameters.AdLoc,
			string.Empty
		},
		{
			Parameters.AdRwType,
			string.Empty
		},
		{
			Parameters.AdRwAmt,
			"0"
		},
		{
			Parameters.CSRID,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.ProfileUDID,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.GeneratedUDID,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.DeviceModel,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.DeviceSerial,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.DeviceAndroidID,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.DeviceTelephonyID,
			"NOT_IMPLEMENTED"
		},
		{
			Parameters.ProfileRecoveryResult,
			"NA"
		},
		{
			Parameters.GraphicsAPI,
			SystemInfo.graphicsDeviceVersion
		},
		{
			Parameters.AssetQuality,
			BaseDevice.ActiveDevice.DeviceQuality.ToString()
		},
		{
			Parameters.ValueIdentifier,
			"NA"
		},
		{
			Parameters.ProfileValue,
			"NA"
		},
		{
			Parameters.MangledValue,
			"NA"
		},
		{
			Parameters.UnmangledValue,
			"NA"
		},
		{
			Parameters.InvokedBy,
			"NA"
		},
		{
			Parameters.ReplayUserID,
			"NA"
		},
		{
			Parameters.ReplayRaceTime,
			"0"
		},
		{
			Parameters.SimRaceTime,
			"0"
		},
		{
			Parameters.SimReplayRaceDeltaTime,
			"0"
		},
		{
			Parameters.ReplayType,
			"NA"
		},
		{
			Parameters.PurchaseData,
			"NA"
		},
		{
			Parameters.CmlStars,
			"NA"
		},
		{
			Parameters.CmlRly,
			"NA"
		},
		{
			Parameters.StarsWon,
			"NA"
		},
		{
			Parameters.CarStars,
			"NA"
		},
		{
			Parameters.LPos,
			"NA"
		},
		{
			Parameters.DLPos,
			"NA"
		},
		{
			Parameters.Friends,
			"NA"
		},
		{
			Parameters.FriendsInCar,
			"NA"
		},
		{
			Parameters.FriendRaced,
			"NA"
		},
		{
			Parameters.RewardType,
			"NA"
		},
		{
			Parameters.RewardAmount,
			"NA"
		},
		{
			Parameters.RewardFor,
			"NA"
		},
		{
			Parameters.FriendUpgrade,
			"0"
		},
		{
			Parameters.AllFBFriends,
			"NA"
		},
		{
			Parameters.FriendsInv,
			"NA"
		},
		{
			Parameters.SocialPlatform,
			"Facebook"
		},
		{
			Parameters.BuildVersion,
			"NA"
		},
		{
			Parameters.LaunchRPM,
			"0"
		},
		{
			Parameters.TotalShifts,
			"0"
		},
		{
			Parameters.Shift0,
			"NA"
		},
		{
			Parameters.Shift1,
			"NA"
		},
		{
			Parameters.Shift2,
			"NA"
		},
		{
			Parameters.Shift3,
			"NA"
		},
		{
			Parameters.Shift4,
			"NA"
		},
		{
			Parameters.Shift5,
			"NA"
		},
		{
			Parameters.Shift6,
			"NA"
		},
		{
			Parameters.Shift7,
			"NA"
		},
		{
			Parameters.Shift8,
			"NA"
		},
		{
			Parameters.Shift9,
			"NA"
		},
		{
			Parameters.Continue,
			"NA"
		},
		{
			Parameters.IAP,
			"NA"
		},
		{
			Parameters.Currency,
			"NA"
		},
		{
			Parameters.YesNo,
			"NA"
		},
		{
			Parameters.MusicMuted,
			"0"
		},
		{
			Parameters.SFXMuted,
			"0"
		},
		{
			Parameters.SystemVolume,
			"0"
		},
		{
			Parameters.HeadphonesPluggedIn,
			"0"
		},
		{
			Parameters.OtherAudioPlaying,
			"0"
		},
		{
			Parameters.DBRewardsRemoved,
			"0"
		},
		{
			Parameters.DBRewardId,
			"NA"
		},
		{
			Parameters.DBRewardTier,
			"0"
		},
		{
			Parameters.DBRewardDate,
			"NA"
		},
		{
			Parameters.PackageLevel,
			"NA"
		},
		{
			Parameters.LapseTime,
			"NA"
		},
		{
			Parameters.CmlLevelPacks,
			"0"
		},
		{
			Parameters.FreeUpgs,
			"0"
		},
		{
			Parameters.Notif,
			"NA"
		},
		{
			Parameters.Title,
			"NA"
		},
		{
			Parameters.SPTitle,
			string.Empty
		},
		{
			Parameters.SPTime,
			string.Empty
		},
		{
			Parameters.SPDiscount,
			string.Empty
		},
		{
			Parameters.SPPopupIndex,
			string.Empty
		},
		{
			Parameters.IAPBought,
			string.Empty
		},
		{
			Parameters.StoreType,
			string.Empty
		},
		{
			Parameters.StoreOption,
			string.Empty
		},
		{
			Parameters.SelCar,
			"NA"
		},
		{
			Parameters.Plc,
			"NA"
		},
		{
			Parameters.CChc,
			"NA"
		},
		{
			Parameters.MPEvtID,
			"-1"
		},
		{
			Parameters.LeadPos,
			"0"
		},
		{
			Parameters.PrizeLvl,
			"0"
		},
		{
			Parameters.MPEvtRace,
			"0"
		},
		{
			Parameters.SptPrize,
			string.Empty
		}
	};

	public static void BeginUserSession()
	{
		sessionLoggedIn = true;
		sessionMPCardsStart = PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments;
        sessionTimeDeltaStart = GTDateTime.Now;
	}

	public static void SetTimeToNow()
	{
        sessionTimeDeltaStart = GTDateTime.Now;
	}

	public static void SessionAddHistoryEvent(Events theEvent)
	{
		if (sessionHistoryEvents.Count < 8)
		{
			sessionHistoryEvents.Enqueue(theEvent);
		}
		else
		{
			sessionHistoryEvents.Dequeue();
			sessionHistoryEvents.Enqueue(theEvent);
		}
	}

	public static string Get(Parameters param, Dictionary<Parameters, string> data)
	{
		string result = string.Empty;
		if (defaults.ContainsKey(param))
		{
			result = defaults[param];
		}
		if (calcs.ContainsKey(param))
		{
			result = calcs[param]();
		}
		if (data.ContainsKey(param))
		{
			result = data[param];
		}
		return result;
	}

	private static string MilestoneRace()
	{
		if (GameDatabase.Instance == null || !GameDatabase.Instance.IsReady())
		{
			return string.Empty;
		}
		string result = string.Empty;
		eCarTier eCarTier = RaceEventQuery.Instance.getHighestUnlockedClass();
		BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier);
		int num = tierEvents.CrewBattleEvents.NumEventsComplete();
		if (num == 0 && eCarTier > eCarTier.TIER_1)
		{
			eCarTier--;
			tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier);
			num = tierEvents.CrewBattleEvents.NumEventsComplete();
		}
		if (num > 0)
		{
			result = "Tier" + CarTierHelper.TierToString[(int)eCarTier] + "_CrewBoss" + num.ToString(CultureInfo.InvariantCulture);
		}
		return result;
	}

	private static string CurrentTestBucket()
	{
        return UserManager.Instance.currentAccount.ABTestBucketName;
	}

    private static string CurrentTestBranch()
    {
        return UserManager.Instance.currentAccount.AssetDatabaseBranch;
    }

    private static string CurrentTestCode()
    {
        return UserManager.Instance.currentAccount.ABTestCode;
    }

    private static string OSVersion()
	{
		return BasePlatform.ActivePlatform.GetDeviceOSVersion();
	}

	public static string CurrentCash()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash().ToString(CultureInfo.InvariantCulture);
	}

	public static string CurrentGold()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold().ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentXP()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetPlayerXP().ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentFuel()
	{
        return PlayerProfileManager.Instance.ActiveProfile.FuelPips.ToString(CultureInfo.InvariantCulture);
	}

    private static string IsBanned()
    {
        return (!UserManager.Instance.currentAccount.IsBanned) ? "0" : "1";
    }

    private static string CumulativeMechanicFettledRaces()
	{
		return PlayerProfileManager.Instance.ActiveProfile.MechanicFettledRaces.ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeTweets()
	{
		return PlayerProfileManager.Instance.ActiveProfile.CumulativeTweets.ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeFBPosts()
	{
		return PlayerProfileManager.Instance.ActiveProfile.CumulativeFBPosts.ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeCarsOwned()
	{
		return PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Count.ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeCash()
	{
        return (UserManager.Instance.currentAccount.IAPCash + PlayerProfileManager.Instance.ActiveProfile.CashEarned).ToString(CultureInfo.InvariantCulture);
	}

    private static string CumulativeGold()
    {
        return (UserManager.Instance.currentAccount.IAPGold + PlayerProfileManager.Instance.ActiveProfile.GoldEarned).ToString(CultureInfo.InvariantCulture);
    }

    private static string CumulativeCashPurchased()
    {
        return UserManager.Instance.currentAccount.IAPCash.ToString(CultureInfo.InvariantCulture);
    }

    private static string CumulativeGoldPurchased()
    {
        return UserManager.Instance.currentAccount.IAPGold.ToString(CultureInfo.InvariantCulture);
    }

    private static string CumulativeSessions()
	{
		return PlayerProfileManager.Instance.ActiveProfile.CumulativeSessions.ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentRankPoints()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP().ToString(CultureInfo.InvariantCulture);
	}

	private static string MechanicActive()
	{
		return (!RaceReCommon.JustFettledEngines && PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining <= 0) ? "no" : "yes";
	}

	private static string CurrentMPTotalRaces()
	{
		return TotalOnlineRaces().ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsecutiveMPWins()
	{
		return PlayerProfileManager.Instance.ActiveProfile.ConsecutiveOnlineWins.ToString(CultureInfo.InvariantCulture);
	}

	private static string MPDifficulty()
	{
		return PlayerProfileManager.Instance.ActiveProfile.MultiplayerDifficulty.ToString(CultureInfo.InvariantCulture);
	}

	private static string PerfectTime()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().TightLoopQuarterMileTime.ToString(CultureInfo.InvariantCulture);
	}

	private static string MPEliteDifficulty()
	{
		return PlayerProfileManager.Instance.ActiveProfile.EliteMultiplayerDifficulty.ToString(CultureInfo.InvariantCulture);
	}

	private static string MPEventDifficulty()
	{
		return PlayerProfileManager.Instance.ActiveProfile.MultiplayerEventDifficulty.ToString(CultureInfo.InvariantCulture);
	}

	private static string GetCurrentStreakDifficulty()
	{
		return StreakManager.CurrentStreakDifficulty.ToString(CultureInfo.InvariantCulture);
	}

	private static string GetCurrentRelayLength()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null || !currentEvent.IsRelay)
		{
			return "NA";
		}
		return currentEvent.Group.RaceEvents.Count.ToString();
	}

	private static string GetCurrentRelayResult()
	{
		if (!RelayManager.IsCurrentEventRelay())
		{
			return "NA";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < RelayManager.GetRaceCount(); i++)
		{
			float humanRaceTime = RelayManager.GetHumanRaceTime(i);
			float aiRaceTime = RelayManager.GetAiRaceTime(i);
			stringBuilder.Append((humanRaceTime > aiRaceTime) ? "0" : "1");
		}
		return stringBuilder.ToString();
	}

	private static string CurrentMPWinRate()
	{
		int num = TotalOnlineRaces();
		if (num == 0)
		{
			return "0";
		}
		float num2 = PlayerProfileManager.Instance.ActiveProfile.OnlineRacesWon / (float)num;
		return Mathf.RoundToInt(num2 * 100f).ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentSPWinRate()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.RacesEntered == 0)
		{
			return "0";
		}
		float num = PlayerProfileManager.Instance.ActiveProfile.RacesWon / (float)PlayerProfileManager.Instance.ActiveProfile.RacesEntered;
		return Mathf.RoundToInt(num * 100f).ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentMPCardsWon()
	{
		return PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments.ToString(CultureInfo.InvariantCulture);
	}

	private static string SessionPrizeCardCount()
	{
		if (!sessionLoggedIn)
		{
			return "0";
		}
		return (PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments - sessionMPCardsStart).ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeUpgradesBoughtForCurrentCar()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().NumUpgradesBought.ToString(CultureInfo.InvariantCulture);
	}

	private static string CumulativeRacesCompleted()
	{
		return (PlayerProfileManager.Instance.ActiveProfile.RacesEntered + PlayerProfileManager.Instance.ActiveProfile.TutorialRacesEntered).ToString(CultureInfo.InvariantCulture);
	}

	private static string CurrentLevel()
	{
		if (PlayerProfileManager.Instance == null || PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return defaults[Parameters.PLvl];
		}
		return PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel().ToString(CultureInfo.InvariantCulture);
	}

	private static string PlayerResult()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return (!RaceResultsTracker.You.IsWinner) ? "lose" : "win";
	}

	private static string RaceTier()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		if (currentCar == null)
		{
			return string.Empty;
		}
		if (currentCar.UpgradeStatus == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.PeekGetCar(currentCar.CarDBKey))
		{
			return string.Empty;
		}
		CarInfo car = CarDatabase.Instance.GetCar(currentCar.CarDBKey);
		string text = car.BaseCarTier.ToString();
		if (RaceEventInfo.Instance != null)
		{
			RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
			if (currentEvent != null && currentEvent.Parent != null)
			{
				text = currentEvent.Parent.GetTierEvents().GetCarTier().ToString();
				if (text == eCarTier.TIER_X.ToString())
				{
					string currentThemeOption = TierXManager.Instance.CurrentThemeOption;
					string str = (currentThemeOption != null) ? ("!" + currentThemeOption) : string.Empty;
					return TierXManager.Instance.CurrentThemeName + str;
				}
			}
		}
		return text;
	}

	private static string RaceName()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance == null)
		{
			return string.Empty;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return string.Empty;
		}
		string text = currentEvent.EventName;
		if (currentEvent.IsCrewRace() || currentEvent.IsCrewBattle())
		{
			text = "CrewBoss" + (currentEvent.EventOrder + 1).ToString(CultureInfo.InvariantCulture);
		}
		if (currentEvent.IsWorldTourRace())
		{
			PinDetail worldTourPinPinDetail = currentEvent.GetWorldTourPinPinDetail();
			if (worldTourPinPinDetail != null)
			{
				text = text + "!" + worldTourPinPinDetail.WorldTourScheduledPinInfo.ID;
			}
		}
		if (currentEvent.IsRelay)
		{
			text = text + "!" + (currentEvent.GetRelayRaceIndex() + 1);
		}
		return text;
	}

	private static string RaceDifficulty()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance == null)
		{
			return string.Empty;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return string.Empty;
		}
		return RaceEventDifficulty.Instance.GetRating(currentEvent, false).ToString();
	}

	private static string RaceLength()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance == null)
		{
			return string.Empty;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return string.Empty;
		}
		return (!currentEvent.IsHalfMile) ? ".25" : ".5";
	}

	private static string Rematch()
	{
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance == null)
		{
			return string.Empty;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return string.Empty;
		}
		if (!currentEvent.IsRaceTheWorldOrClubRaceEvent())
		{
			return string.Empty;
		}
		return "0";
	}

	private static string PlayerCar()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return string.Empty;
		}
		if (CarDatabase.Instance == null)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return string.Empty;
		}
		if (!CarDatabase.Instance.PeekGetCar(activeProfile.GetCurrentCar().CarDBKey))
		{
			return string.Empty;
		}
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		if (currentCar == null)
		{
			return string.Empty;
		}
		if (currentCar.UpgradeStatus == null)
		{
			return string.Empty;
		}
		return currentCar.CarDBKey;
	}

	private static string AICarName()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance == null)
		{
			return string.Empty;
		}
		if (RaceEventInfo.Instance.CurrentEvent == null)
		{
			return string.Empty;
		}
		string result = RaceEventInfo.Instance.CurrentEvent.AICar;
		if ((RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent()) && CompetitorManager.Instance.OtherCompetitor != null)
		{
			RacePlayerInfoComponent component = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
			result = component.CarDBKey;
		}
		return result;
	}

	private static int AIPPInt()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return 0;
		}
		if (RaceEventInfo.Instance == null)
		{
			return 0;
		}
		if (RaceEventInfo.Instance.CurrentEvent == null)
		{
			return 0;
		}
		int result = RaceEventInfo.Instance.CurrentEvent.GetAIPerformancePotentialIndex();
		if ((RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent()) && CompetitorManager.Instance.OtherCompetitor != null)
		{
			RacePlayerInfoComponent component = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
			result = component.PPIndex;
		}
		return result;
	}

	private static string AIPP()
	{
		return AIPPInt().ToString(CultureInfo.InvariantCulture);
	}

	private static string GameVersion()
	{
		string result = "Notavailable";
		if (AssetDatabaseClient.Instance == null)
		{
			return result;
		}
		if (!AssetDatabaseClient.Instance.Data.IsValid)
		{
			return result;
		}
		return AssetDatabaseClient.Instance.Data.GetAppVersion();
	}

	private static string GamePlatform()
	{
		if (BasePlatform.ActivePlatform == null)
		{
			return string.Empty;
		}
		return BasePlatform.ActivePlatform.GetDeviceOS();
	}

	private static string PlayerFinishTime()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.RaceTime.ToString(CultureInfo.InvariantCulture);
	}

	private static string NitrousUsed()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		string text = (!RaceResultsTracker.You.UsedNitrous) ? "0" : "1";
		return (!RaceResultsTracker.You.UsedNitrous || !RaceResultsTracker.You.HadBoostNitrousAvailable) ? text : "2";
	}

	private static string AIFinishTime()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return (RaceResultsTracker.Them != null) ? RaceResultsTracker.Them.RaceTime.ToString(CultureInfo.InvariantCulture) : "0.0";
	}

	private static string EarlyShifts()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.NumberOfOptimalChanges.ToString(CultureInfo.InvariantCulture);
	}

	private static string GoodShifts()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.NumberOfGoodChanges.ToString(CultureInfo.InvariantCulture);
	}

	private static string PerfectShifts()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.NumberOfOptimalChanges.ToString(CultureInfo.InvariantCulture);
	}

	private static string LateShifts()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.NumberOfLateChanges.ToString(CultureInfo.InvariantCulture);
	}

	private static string PerfectStart()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return (!RaceResultsTracker.You.GreatLaunch) ? "no" : "yes";
	}

	private static string ResultBest0To60()
	{
		if (RaceResultsTracker.Best == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.Best.Nought60Time.ToString(CultureInfo.InvariantCulture);
	}

	private static string ResultTerminalSpeed()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.SpeedWhenCrossingFinishLine.ToString(CultureInfo.InvariantCulture);
	}

	private static string ResultPlayer0To60()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.Nought60Time.ToString(CultureInfo.InvariantCulture);
	}

	private static string ResultPlayer0To100()
	{
		if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
		return RaceResultsTracker.You.Nought100Time.ToString(CultureInfo.InvariantCulture);
	}

	private static string ResultNewPB()
	{
	    if (RaceResultsTracker.You == null)
		{
			return string.Empty;
		}
	    return "no";//(RaceResultsTracker.You.RaceTime >= GameCenterController.Instance.getBestScoreForEvent()) ? "no" : "yes";
	}

    private static string CurrentTimeDelta()
    {
        return (GTDateTime.Now - sessionTimeDeltaStart).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
    }

    private static string SnapshotNumber()
	{
		if (AssetDatabaseClient.Instance == null || !AssetDatabaseClient.Instance.Data.IsValid)
		{
			return "NotApplicable";
		}
		return AssetDatabaseClient.Instance.Data.GetVersion().ToString(CultureInfo.InvariantCulture);
	}

	private static string NumberFriendsRaces()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendRacesPlayed.ToString(CultureInfo.InvariantCulture);
	}

	private static string NumberFriendsRacesWon()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon.ToString(CultureInfo.InvariantCulture);
	}

	private static string FriendsCarsWon()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendsCarsWon.ToString(CultureInfo.InvariantCulture);
	}

	private static string FriendsGold()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendsGold.ToString(CultureInfo.InvariantCulture);
	}

	private static string ThreeStarCars()
	{
        int num = 0;
        //StarsManager.GetMyStarStats().NumStars.TryGetValue(StarType.GOLD, out num);
        return num.ToString(CultureInfo.InvariantCulture);
	}

    private static string TotalStars()
    {
        return "0";//StarsManager.GetMyStarStats().TotalStars.ToString(CultureInfo.InvariantCulture);
    }

    private static string NumberFriends()
    {
        return "0";//LumpManager.Instance.FriendLumps.Count.ToString(CultureInfo.InvariantCulture);
    }

    private static string TotalFriendsInvited()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendsInvited.ToString(CultureInfo.InvariantCulture);
	}

	private static string PeakFriends()
	{
		return PlayerProfileManager.Instance.ActiveProfile.PeakFriends.ToString(CultureInfo.InvariantCulture);
	}

	private static string InitialFriends()
	{
		return PlayerProfileManager.Instance.ActiveProfile.InitialNetworkFriends.ToString(CultureInfo.InvariantCulture);
	}

	private static string HasRecievedFriendSync()
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.GetFriendSyncComplete()) ? "FALSE" : "TRUE";
	}

	private static string BuildVersionNumber()
	{
		return "NA";
	}

	private static int TotalOnlineRaces()
	{
		return PlayerProfileManager.Instance.ActiveProfile.OnlineRacesWon + PlayerProfileManager.Instance.ActiveProfile.OnlineRacesLost;
	}

	private static string GetHistoryEvent1()
	{
		return GetHistoryEvent(0);
	}

	private static string GetHistoryEvent2()
	{
		return GetHistoryEvent(1);
	}

	private static string GetHistoryEvent3()
	{
		return GetHistoryEvent(2);
	}

	private static string GetHistoryEvent4()
	{
		return GetHistoryEvent(3);
	}

	private static string GetHistoryEvent5()
	{
		return GetHistoryEvent(4);
	}

	private static string GetHistoryEvent6()
	{
		return GetHistoryEvent(5);
	}

	private static string GetHistoryEvent7()
	{
		return GetHistoryEvent(6);
	}

	private static string GetHistoryEvent8()
	{
		return GetHistoryEvent(7);
	}

	private static string GetHistoryEvent(int idx)
	{
		if (sessionHistoryEvents.Count <= idx)
		{
			return "NA";
		}
		return sessionHistoryEvents.ToArray()[idx].ToString();
	}

	private static string LoggedIn()
	{
		return (!sessionLoggedIn) ? "no" : "yes";
	}

	private static int PlayerCarPPInt()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return 0;
		}
		return activeProfile.GetCurrentCar().CurrentPPIndex;
	}

	private static string PlayerCarPP()
	{
		return PlayerCarPPInt().ToString(CultureInfo.InvariantCulture);
	}

	private static string PlayerCarPostUpgradePP()
	{
		return PlayerCarPP();
	}

	private static string PPDelta()
	{
		return (PlayerCarPPInt() - AIPPInt()).ToString(CultureInfo.InvariantCulture);
	}

	private static string GetUpgradeLevel(eUpgradeType upgrade)
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().UpgradeStatus[upgrade].levelFitted.ToString(CultureInfo.InvariantCulture);
	}

	private static string EngineLevel()
	{
		return GetUpgradeLevel(eUpgradeType.ENGINE);
	}

	private static string BodyLevel()
	{
		return GetUpgradeLevel(eUpgradeType.BODY);
	}

	private static string GearBoxLevel()
	{
		return GetUpgradeLevel(eUpgradeType.TRANSMISSION);
	}

	private static string TyresLevel()
	{
		return GetUpgradeLevel(eUpgradeType.TYRES);
	}

	private static string TurboLevel()
	{
		return GetUpgradeLevel(eUpgradeType.TURBO);
	}

	private static string NitroLevel()
	{
		return GetUpgradeLevel(eUpgradeType.NITROUS);
	}

	private static string IntakeLevel()
	{
		return GetUpgradeLevel(eUpgradeType.INTAKE);
	}

	private static string IsUpSell()
	{
		bool flag = ShowroomScreen.Init.screenMode == ShowroomMode.SpecialOffer;
		return (!flag) ? "no" : "yes";
	}

	private static string StreakNumber()
	{
		return StreakManager.Chain.VisibleChainCount.ToString(CultureInfo.InvariantCulture);
	}

	private static string StreakRaceNumber()
	{
		return StreakManager.CurrentStreak().ToString(CultureInfo.InvariantCulture);
	}

	private static string StreakRescued()
	{
		return StreakManager.CurrentStreakRescueCount().ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsumableEN()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetConsumableExpireTimeTotalMinutes(eCarConsumables.EngineTune).ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsumableNI()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetConsumableExpireTimeTotalMinutes(eCarConsumables.Nitrous).ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsumableTY()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetConsumableExpireTimeTotalMinutes(eCarConsumables.Tyre).ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsumableBL()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetConsumableExpireTimeTotalMinutes(eCarConsumables.PRAgent).ToString(CultureInfo.InvariantCulture);
	}

	private static string ConsumableWhole()
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.IsConsumableActive(eCarConsumables.WholeTeam)) ? "0" : "1";
	}

	private static string RPBoost()
	{
	    //return RPBonusManager.NavBarValue().ToString(CultureInfo.InvariantCulture);
	    return "na";
	}

    private static string PrizeomaticCard1()
    {
        return (PrizeOMaticScreen.LastCardsAwarded.Count <= 0) ? "NA" : PrizeOMaticScreen.LastCardsAwarded[0].CardReward.ToString();
    }

    private static string PrizeomaticCard2()
    {
        return (PrizeOMaticScreen.LastCardsAwarded.Count <= 1) ? "NA" : PrizeOMaticScreen.LastCardsAwarded[1].CardReward.ToString();
    }

    private static string PrizeomaticCard3()
    {
        return (PrizeOMaticScreen.LastCardsAwarded.Count <= 2) ? "NA" : PrizeOMaticScreen.LastCardsAwarded[2].CardReward.ToString();
    }

    private static string GetFromScreen()
    {
        return ScreenManager.Instance.CurrentStackString();
    }

    private static string GetStore()
    {
        return ShopScreen.ItemTypeToShow.ToString();
    }

    private static bool IsCarDealActive()
    {
        return ScreenManager.Instance.IsScreenOnStack(ScreenID.Showroom) && ShowroomScreen.Init.screenMode == ShowroomMode.SpecialOffer;
    }

    private static string GetCarDealCar()
	{
		if (!IsCarDealActive())
		{
			return "NA";
		}
		return ShowroomScreen.Init.DealToApply.Car;
	}

	private static string GetCarDealPrice()
	{
		if (!IsCarDealActive())
		{
			return "NA";
		}
		return ShowroomScreen.Init.DealToApply.GetGoldPrice().ToString(CultureInfo.InvariantCulture);
	}

	private static string GetCarDealDiscount()
	{
		if (!IsCarDealActive())
		{
			return "NA";
		}
		return ShowroomScreen.Init.DealToApply.GetDiscountMetricParam();
	}

	private static string GetCarDealFreeUpgrades()
	{
		if (!IsCarDealActive())
		{
			return "NA";
		}
		return ShowroomScreen.Init.DealToApply.GetFreeUpgradesMetricParam();
	}

	private static string GetCarDealCashback()
	{
		if (!IsCarDealActive())
		{
			return "NA";
		}
		return ShowroomScreen.Init.DealToApply.GetCashbackMetricParam();
	}

	private static string GetMusicMuted()
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute) ? "0" : "1";
	}

	private static string GetSFXMuted()
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute) ? "0" : "1";
	}

	private static string GetSystemVolume()
	{
	    return "0";//((int)(SystemAudio.GetOutputVolume() * 100f)).ToString(CultureInfo.InvariantCulture);
	}

    private static string GetHeadphonesPluggedIn()
    {
        return "false";//(!SystemAudio.GetIsHeadphonesPluggedIn()) ? "0" : "1";
    }

    private static string GetOtherAudioPlaying()
    {
        return "false";//(!SystemAudio.GetIsOtherAudioPlaying()) ? "0" : "1";
    }

    private static string GetCarePackageTotalReceivedCount()
    {
        return PlayerProfileManager.Instance.ActiveProfile.CarePackageTotalReceivedCount().ToString(CultureInfo.InvariantCulture);
    }

    private static string GetDailyRaceNum()
	{
		return PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount.ToString(CultureInfo.InvariantCulture);
	}

	private static string GetProfilePinScheduleMetricsData()
	{
	    return "na";//PlayerProfileManager.Instance.ActiveProfile.GetPinScheduleMetricsData();
	}

    private static string GetCurrentMPEventRaceCount()
	{
        //MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        //if (data != null)
        //{
        //    return MultiplayerEvent.Saved.GetRacesCompleted().ToString(CultureInfo.InvariantCulture);
        //}
		return "0";
	}
}
