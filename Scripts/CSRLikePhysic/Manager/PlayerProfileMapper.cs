using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class PlayerProfileMapper
{
    private static void GetScoreData(JsonDict jsonDict, ref SocialGamePlatformSelector.ScoreData scoreData)
    {
        jsonDict.TryGetValue("sc", out scoreData.score);
        int @int = jsonDict.GetInt("c");
        int int2 = jsonDict.GetInt("s");
        scoreData.leaderboard = Leaderboards.GetByIdx(@int);
        scoreData.status = (SocialGamePlatformSelector.ScoreStatus)((!Enum.IsDefined(typeof(GameCenterController.ScoreStatus), int2)) ? 0 : int2);
    }

    private static void GetAchievementData(JsonDict jsonDict, ref SocialGamePlatformSelector.AchievementData achievementData)
    {
        int @int = jsonDict.GetInt("a");
        int int2 = jsonDict.GetInt("s");
        achievementData.achievement = Achievements.GetByIdx(@int);
        achievementData.status = (SocialGamePlatformSelector.AchievementStatus)((!Enum.IsDefined(typeof(SocialGamePlatformSelector.AchievementStatus), int2)) ? 0 : int2);
    }

    private static void GetArrival(JsonDict jsonDict, ref Arrival arrival)
    {
        jsonDict.TryGetValue("apno", out arrival.AssociatedPushNotification);
        jsonDict.TryGetValue("dlts", out arrival.deliveryTimeSecs);
        jsonDict.TryGetValue("duet", out arrival.dueTime);
        arrival.arrivalType = jsonDict.GetEnum<ArrivalType>("arvt");
        jsonDict.TryGetValue("cari", out arrival.carId);
        jsonDict.TryGetValue("coli", out arrival.ColourIndex);
        arrival.upgradeType = jsonDict.GetEnum<eUpgradeType>("upgt");
        jsonDict.TryGetValue("datd", out arrival.doesAutoTickDown);
    }

    private static void GetMultiplayerCarPrize(JsonDict jsonDict, ref MultiplayerCarPrize carPrize)
    {
        jsonDict.TryGetValue("wodb", out carPrize.CarDBKey);
        jsonDict.TryGetValue("ppwo", out carPrize.PiecesWon);
        jsonDict.TryGetValue("npto", out carPrize.NumPiecesTotal);
    }

    private static void GetCarUpgradeStatus(JsonDict jsonDict, ref CarUpgradeStatus upgradeStatus)
    {
        int num;
        jsonDict.TryGetValue("lvlo", out num);
        int l;
        jsonDict.TryGetValue("lvlf", out l, num);
        int l2;
        jsonDict.TryGetValue("lvle", out l2, 0);
        upgradeStatus.levelOwned = CarUpgradeStatus.Convert(num);
        upgradeStatus.levelFitted = CarUpgradeStatus.Convert(l);
        upgradeStatus.evoOwned = CarUpgradeStatus.Convert(l2);
    }

    private static void GetCarGarageInstance(JsonDict jsonDict, ref CarGarageInstance carOwned)
    {
        jsonDict.TryGetValue("crdb", out carOwned.CarDBKey);
        jsonDict.TryGetValue("apci", out carOwned.AppliedColourIndex);
        jsonDict.TryGetValue("apln", out carOwned.AppliedLiveryName);
        jsonDict.TryGetValue("bqmt", out carOwned.BestQuarterMileTime);
        jsonDict.TryGetValue("bhmt", out carOwned.BestHalfMileTime);
        jsonDict.TryGetValue("ditr", out carOwned.DistanceTravelled);
        jsonDict.TryGetValue("rcat", out carOwned.RacesAttempted);
        jsonDict.TryGetValue("rwon", out carOwned.RacesWon);
        jsonDict.TryGetValue("msou", out carOwned.MoneySpentOnUpgrades);
        jsonDict.TryGetValue("nuub", out carOwned.NumUpgradesBought);
        jsonDict.TryGetValue("tsat", out carOwned.TopSpeedAttained);
        jsonDict.TryGetValue("ccna", out carOwned.CustomCarNags);
        jsonDict.TryGetValue("cbht", out carOwned.BodyHeight);
        carOwned.UpgradeStatus = jsonDict.GetObjectArrayFromDictValues("upst", new GetObjectDelegate<CarUpgradeStatus>(GetCarUpgradeStatus), CarUpgrades.ValidUpgrades);
        carOwned.EquipItemCollection = new VirtualEquipItemCollection();
        carOwned.EquipItemCollection.AddRange(jsonDict.GetObjectList<VirtualEquipItem>("veic", GetEquipItemCollection));
        jsonDict.TryGetValue("owlv", out carOwned.OwnedLiveries);
        jsonDict.TryGetValue("cppi", out carOwned.CurrentPPIndex);
        carOwned.CurrentTier = jsonDict.GetEnum<eCarTier>("ctie");
        carOwned.NumberPlate = jsonDict.GetObject<NumberPlate>("nupl", GetNumberPlate);
        jsonDict.TryGetValue("elit", out carOwned.EliteCar);
        jsonDict.TryGetValue("spup", out carOwned.SportsUpgrade);
        jsonDict.TryGetValue("tlad", out carOwned.TightLoopQuarterMileTimeAdjust);
        if (!jsonDict.TryGetValue("ulvs", out carOwned.UnlockedLiveries))
        {
            carOwned.UnlockedLiveries = new List<string>();
        }
        if (!jsonDict.TryGetValue("nlvs", out carOwned.NewLiveries))
        {
            carOwned.NewLiveries = new List<string>();
        }
    }

    private static void GetEquipItemCollection(JsonDict jsonDict, ref VirtualEquipItem equipItem)
    {
        equipItem.CarID = jsonDict.GetString("crid");
        equipItem.Equiped = jsonDict.GetBool("eqpd");
        equipItem.VirtualItemID = jsonDict.GetString("viid");
        equipItem.ItemType = jsonDict.GetEnum<VirtualItemType>("ittp");
    }

    private static void GetNumberPlate(JsonDict jsonDict, ref NumberPlate numberPlate)
    {
        jsonDict.TryGetValue("txt", out numberPlate.Text, string.Empty);
        numberPlate.SetToDefaultColours();
    }

    private static void GetSeasonPrizesAwardedData(JsonDict jsonDict, ref SeasonPrizeIdentifier prize)
    {
        jsonDict.TryGetValue("spld", out prize.LeaderboardID);
        jsonDict.TryGetValue("sppd", out prize.PrizeID);
    }

    private static void GetVideoForFuelTimestamps(JsonDict jsonDict, ref DateTime ts)
    {
        jsonDict.TryGetValue("vfft", out ts);
    }

    private static void GetDeferredNarrativeSceneData(JsonDict jsonDict, out Dictionary<string, PlayerProfileData.DeferredNarrativeScene> dict)
    {
        dict = new Dictionary<string, PlayerProfileData.DeferredNarrativeScene>();
        List<string> list;
        jsonDict.TryGetValue("wtdk", out list);
        List<string> list2;
        jsonDict.TryGetValue("wtds", out list2);
        List<string> list3;
        jsonDict.TryGetValue("wtde", out list3);
        if (list != null && list2 != null && list3 != null)
        {
            int num = 0;
            while (num < list.Count && num < list2.Count && num < list3.Count)
            {
                dict.Add(list[num], new PlayerProfileData.DeferredNarrativeScene
                {
                    SceneID = list2[num],
                    SequenceID = list3[num]
                });
                num++;
            }
        }
    }

    private static void GetPlayerProfileData(JsonDict jsonDict, ref PlayerProfileData pp)
    {
        pp.CarePackageData.FromJson(ref jsonDict);
        pp.RestrictionRaceData.FromJson(jsonDict);
        pp.OfferWallData.FromJson(ref jsonDict);
        pp.PinSchedulerData.FromJson(ref jsonDict);
        pp.CarDeals.FromJson(jsonDict);
        pp.BundleOfferData.FromJson(ref jsonDict);
        pp.TutorialBubblesData.FromJson(ref jsonDict);
        pp.MultiplayerEventsData.FromJson(ref jsonDict);
        jsonDict.TryGetValue("name", out pp.Username);
        jsonDict.TryGetValue("dsnm", out pp.DisplayName);
        jsonDict.TryGetValue("avid", out pp.AvatarID);
        jsonDict.TryGetValue("plst", out pp.PlayerStar);
        jsonDict.TryGetValue("plls", out pp.PlayerLeagueStar);
        jsonDict.TryGetEnum("pllg", out pp.PlayerLeague);
        jsonDict.TryGetValue("new2", out pp.NewProfile);
        jsonDict.TryGetValue("date", out pp.dateTimeLastSaved);
        jsonDict.TryGetValue("prvr", out pp.ProductVersionLastSaved, "1.0.5");
        jsonDict.TryGetValue("boqt", out pp.BestOverallQuarterMileTime);
        jsonDict.TryGetValue("boht", out pp.BestOverallHalfMileTime);
        jsonDict.TryGetValue("tdtr", out pp.TotalDistanceTravelled);
        jsonDict.TryGetValue("tplt", out pp.TotalPlayTime);
        jsonDict.TryGetValue("tgrt", out pp.TotalGarageTime);
        jsonDict.TryGetValue("casp", out pp.CashSpent);
        jsonDict.TryGetValue("caea", out pp.CashEarned);
        jsonDict.TryGetValue("cabo", out pp.CashBought);
        jsonDict.TryGetValue("iacs", out pp.IAPCashSpent);
        jsonDict.TryGetValue("goea", out pp.GoldEarned);
        jsonDict.TryGetValue("gosp", out pp.GoldSpent);
        jsonDict.TryGetValue("gobo", out pp.GoldBought);
        jsonDict.TryGetValue("iags", out pp.IAPGoldSpent);
        jsonDict.TryGetValue("gtea", out pp.GachaTokensEarned);
        jsonDict.TryGetValue("gtsp", out pp.GachaTokensSpent);
        jsonDict.TryGetValue("gbke", out pp.GachaBronzeKeysEarned);
        jsonDict.TryGetValue("gbks", out pp.GachaBronzeKeysSpent);
        jsonDict.TryGetValue("igbk", out pp.IAPGachaBronzeKeysSpent);
        jsonDict.TryGetValue("gske", out pp.GachaSilverKeysEarned);
        jsonDict.TryGetValue("gsks", out pp.GachaSilverKeysSpent);
        jsonDict.TryGetValue("igsk", out pp.IAPGachaSilverKeysSpent);
        jsonDict.TryGetValue("ggke", out pp.GachaGoldKeysEarned);
        jsonDict.TryGetValue("ggks", out pp.GachaGoldKeysSpent);
        jsonDict.TryGetValue("iggk", out pp.IAPGachaGoldKeysSpent);
        jsonDict.TryGetValue("dbdt", out pp.DailyBattlesDoneToday);
        jsonDict.TryGetValue("dbcd", out pp.DailyBattlesConsecutiveDaysCount);
        jsonDict.TryGetValue("dble", out pp.DailyBattlesLastEventAt);
        jsonDict.TryGetValue("dbwl", out pp.DailyBattlesWonLast);
        jsonDict.TryGetValue("bnou", out pp.BoostNitrousUsed);
        jsonDict.TryGetValue("nful", out pp.FreeUpgradesLeft);
        pp.BossChallengeStateT1 = jsonDict.GetEnum<BossChallengeStateEnum>("bcs1");
        pp.BossChallengeStateT2 = jsonDict.GetEnum<BossChallengeStateEnum>("bcs2");
        pp.BossChallengeStateT3 = jsonDict.GetEnum<BossChallengeStateEnum>("bcs3");
        pp.BossChallengeStateT4 = jsonDict.GetEnum<BossChallengeStateEnum>("bcs4");
        pp.BossChallengeStateT5 = jsonDict.GetEnum<BossChallengeStateEnum>("bcs5");
        jsonDict.TryGetValue("mumu", out pp.OptionMusicMute);
        jsonDict.TryGetValue("somu", out pp.OptionSoundMute);
        jsonDict.TryGetValue("onot", out pp.OptionNotifications);
        jsonDict.TryGetValue("lmlm", out pp.HasSeenLowMemoryLanguageMessage);
        jsonDict.TryGetValue("raen", out pp.RacesEntered);
        jsonDict.TryGetValue("tutr", out pp.TutorialRacesAttempted);
        jsonDict.TryGetValue("mfer", out pp.MechanicFettledRaces);
        jsonDict.TryGetValue("rawo", out pp.RacesWon);
        jsonDict.TryGetValue("ralo", out pp.RacesLost);
        jsonDict.TryGetValue("ntwe", out pp.NumTweets);
        jsonDict.TryGetValue("nfbp", out pp.NumFBPosts);
        jsonDict.TryGetValue("ect1", out pp.EventsCompletedTier1);
        jsonDict.TryGetValue("ect2", out pp.EventsCompletedTier2);
        jsonDict.TryGetValue("ect3", out pp.EventsCompletedTier3);
        jsonDict.TryGetValue("ect4", out pp.EventsCompletedTier4);
        jsonDict.TryGetValue("ect5", out pp.EventsCompletedTier5);
        jsonDict.TryGetValue("t1cs", out pp.Tier1CarSpecificEventTarget);
        jsonDict.TryGetValue("t2cs", out pp.Tier2CarSpecificEventTarget);
        jsonDict.TryGetValue("t3cs", out pp.Tier3CarSpecificEventTarget);
        jsonDict.TryGetValue("t4cs", out pp.Tier4CarSpecificEventTarget);
        jsonDict.TryGetValue("t5cs", out pp.Tier5CarSpecificEventTarget);
        string tier1Target;
        string tier2Target;
        string tier3Target;
        string tier4Target;
        string tier5Target;
        jsonDict.TryGetValue("t1ms", out tier1Target);
        jsonDict.TryGetValue("t2ms", out tier2Target);
        jsonDict.TryGetValue("t3ms", out tier3Target);
        jsonDict.TryGetValue("t4ms", out tier4Target);
        jsonDict.TryGetValue("t5ms", out tier5Target);
        pp.Tier1ManufacturerSpecificEventTarget = tier1Target;
        pp.Tier2ManufacturerSpecificEventTarget = tier2Target;
        pp.Tier3ManufacturerSpecificEventTarget = tier3Target;
        pp.Tier4ManufacturerSpecificEventTarget = tier4Target;
        pp.Tier5ManufacturerSpecificEventTarget = tier5Target;
        jsonDict.TryGetValue("wanu", out pp.HaveWarnedAboutNitrousUpgrade);
        jsonDict.TryGetValue("hlof", out pp.HasLikedOnFacebook);
        jsonDict.TryGetValue("hfot", out pp.HasFollowedUsOnTwitter);
        jsonDict.TryGetValue("mtrr", out pp.MechanicTuningRacesRemaining);
        jsonDict.TryGetValue("sfmp", out pp.HaveShownFirstMechanicPopUp);
        jsonDict.TryGetValue("colo", out pp.ContiguousLosses);
        jsonDict.TryGetValue("cplo", out pp.ContiguousProgressionLosses);
        jsonDict.TryGetValue("cplt", out pp.ContiguousProgressionLossesTriggered);
        jsonDict.TryGetValue("hbfc", out pp.HasBoughtFirstCar);
        jsonDict.TryGetValue("hbfu", out pp.HasBoughtFirstUpgrade);
        jsonDict.TryGetValue("hcpn", out pp.HasChoosePlayerName);
        jsonDict.TryGetValue("lang", out pp.LastAgentNag);
        jsonDict.TryGetValue("lply", out pp.LastPlayedMultiplayer);
        jsonDict.TryGetValue("lpec", out pp.LastPlayedEliteClub);
        jsonDict.TryGetValue("lpwt", out pp.LastPlayedRaceTheWorldWorldTour);
        jsonDict.TryGetValue("lpld", out pp.MultiplayerDifficulty);
        jsonDict.TryGetValue("empd", out pp.EliteMultiplayerDifficulty);
        jsonDict.TryGetValue("wtmd", out pp.MultiplayerEventDifficulty);
        jsonDict.TryGetValue("uspl", out pp.UserStartedPlaying);
        jsonDict.TryGetValue("usls", out pp.UserStartedLastSession);
        jsonDict.TryGetValue("scdb", out pp.CurrentlySelectedCarDBKey);
        jsonDict.TryGetValue("plxp", out pp.PlayerXP);
        jsonDict.TryGetValue("pllv", out pp.PlayerLevel);
        pp.PlayerRP = -1;
        jsonDict.TryGetValue("plrk", out pp.WorldRank, 0);
        jsonDict.TryGetValue("plwr", out pp.PreviousWorldRank, 0);
        jsonDict.TryGetValue("ulcc", out pp.UTCLastClockChange);
        jsonDict.TryGetValue("ct2f", out pp.CanTrySecondFacebookNag);
        jsonDict.TryGetValue("relf", out pp.RacesEnteredAtLastFacebookNag);
        if (!jsonDict.TryGetObjectList<MultiplayerCarPrize>("mpcp", out pp.CarsWonInMultiplayer, new GetObjectDelegate<MultiplayerCarPrize>(PlayerProfileMapper.GetMultiplayerCarPrize)))
        {
            pp.CarsWonInMultiplayer = new List<MultiplayerCarPrize>();
        }
        pp.CarsOwned = jsonDict.GetObjectList<CarGarageInstance>("caow", new GetObjectDelegate<CarGarageInstance>(PlayerProfileMapper.GetCarGarageInstance));
        jsonDict.TryGetValue("nwcs", out pp.NewCars, new List<string>());
        jsonDict.TryGetValue("evcm", out pp.EventsCompleted);
        jsonDict.TryGetValue("scnt", out pp.SessionsCounter, 0);
        jsonDict.TryGetValue("obco", out pp.LegacyObjectivesCompleted, new List<int>());
        if (!jsonDict.TryGetValue("objc", out pp.ObjectivesCompleted))
        {
            pp.ObjectivesCompleted = new List<string>();
        }
        if (!jsonDict.TryGetValue("obca", out pp.ObjectivesCollected))
        {
            pp.ObjectivesCollected = new List<string>();
        }
        JsonDict jsonDict7 = jsonDict.GetJsonDict("obja");
        pp.ActiveObjectives = new Dictionary<string, JsonDict>();
        if (jsonDict7 != null)
        {
            foreach (string current5 in jsonDict7.Keys)
            {
                pp.ActiveObjectives.Add(current5, jsonDict7.GetJsonDict(current5));
            }
        }
        jsonDict.TryGetValue("doet", out pp.ObjectiveEndTime);
        pp.arrivalQueue = jsonDict.GetObjectList<Arrival>("arqu", PlayerProfileMapper.GetArrival);
        jsonDict.TryGetValue("pcap", out pp.PreferredCsrAvatarPic, AvatarPicture.FallbackCSRAvatar);
        pp.playerAchievements = jsonDict.GetObjectList<SocialGamePlatformSelector.AchievementData>("playerAchievements", PlayerProfileMapper.GetAchievementData);
        if (pp.playerAchievements == null)
        {
            pp.playerAchievements = new List<SocialGamePlatformSelector.AchievementData>();
        }
        pp.playerScores = jsonDict.GetObjectList<SocialGamePlatformSelector.ScoreData>("playerScores", PlayerProfileMapper.GetScoreData);
        if (pp.playerScores == null)
        {
            pp.playerScores = new List<SocialGamePlatformSelector.ScoreData>();
        }
        jsonDict.TryGetValue("fupi", out pp.FuelPips, 10);
        jsonDict.TryGetValue("fart", out pp.LastFuelAutoReplenishedTime, GTDateTime.Now);
        jsonDict.TryGetValue("llav", out pp.LastLegalAgreementVersion, 0);
        jsonDict.TryGetValue("ladt", out pp.LastAdvertDate, GTDateTime.Now);
        jsonDict.TryGetValue("adct", out pp.AdCount, 0);
        jsonDict.TryGetValue("gfrl", out pp.GoldFuelRefills);
        jsonDict.TryGetValue("cums", out pp.CumulativeSessions);
        jsonDict.TryGetValue("hsif", out pp.HasSignedIntoFacebookBefore, false);
        jsonDict.TryGetValue("hsig", out pp.HasSignedIntoGameCentreBefore, false);
        jsonDict.TryGetValue("hsip", out pp.HasSignedIntoGooglePlayGamesBefore, false);
        jsonDict.TryGetValue("ggsc", out pp.GPGSignInCancellations, 0);
        jsonDict.TryGetValue("cpip", out pp.CrewProgressionIntroductionPlayed);
        jsonDict.TryGetValue("hafc", out pp.HasAttemptedFirstCrew);
        jsonDict.TryGetValue("lsmd", out pp.LastServerMessageDisplayedID);
        jsonDict.TryGetValue("lsmc", out pp.LastServerMessageDisplayedCount);
        jsonDict.TryGetValue("fifc", out pp.FacebookInviteFuelRewardsCount);
        jsonDict.TryGetValue("fift", out pp.FacebookInviteFuelRewardsTime);
        jsonDict.TryGetValue("fsra", out pp.IsFacebookSSORewardAllowed);
        jsonDict.TryGetValue("fbid", out pp.FacebookID);
        jsonDict.TryGetValue("tifc", out pp.TwitterInviteFuelRewardsCount);
        jsonDict.TryGetValue("tift", out pp.TwitterInviteFuelRewardsTime);
        jsonDict.TryGetValue("tcfc", out pp.TwitterCashRewardsCount);
        jsonDict.TryGetValue("tcft", out pp.TwitterCashRewardsTime);
        jsonDict.TryGetValue("ratb", out pp.DoneRateAppTriggerBuyCar);
        jsonDict.TryGetValue("ratc", out pp.DoneRateAppTriggerCrewMember);
        jsonDict.TryGetValue("ludn", out pp.LastUpgradeDateTimeNag);
        jsonDict.TryGetValue("msmi", out pp.HasHadMechanicSlowMotionIntroduction);
        jsonDict.TryGetValue("hsnt", out pp.HasSeenNitrousTutorial);
        jsonDict.TryGetValue("btnq", out pp.BestTwitterNagTimeQtr);
        jsonDict.TryGetValue("sscp", out pp.ShouldShowSkipTo2ndCrewMemberPopup);
        jsonDict.TryGetValue("lbcr", out pp.LastBoughtCarRacesEntered);
        jsonDict.TryGetValue("trhi", out pp.TransactionHistory, new List<string>());
        jsonDict.TryGetValue("mdcb", out pp.MultiplayerDCBDifficulty);
        jsonDict.TryGetValue("mdcw", out pp.MultiplayerDCBConsecutiveWins);
        jsonDict.TryGetValue("mdcl", out pp.MultiplayerDCBConsecutiveLoses);
        jsonDict.TryGetValue("morw", out pp.OnlineRacesWon);
        jsonDict.TryGetValue("morl", out pp.OnlineRacesLost);
        jsonDict.TryGetValue("crld", out pp.ConsecutiveRacesWonAtLowDifficulty);
        jsonDict.TryGetValue("molr", out pp.LastOnlineRace);
        jsonDict.TryGetValue("mort", out pp.OnlineRacesWonToday);
        jsonDict.TryGetValue("mlrt", out pp.OnlineRacesLostToday);
        jsonDict.TryGetValue("cone", out pp.Consumables[eCarConsumables.EngineTune].Expires, DateTime.MinValue);
        jsonDict.TryGetValue("conp", out pp.Consumables[eCarConsumables.PRAgent].Expires, DateTime.MinValue);
        jsonDict.TryGetValue("conn", out pp.Consumables[eCarConsumables.Nitrous].Expires, DateTime.MinValue);
        jsonDict.TryGetValue("cont", out pp.Consumables[eCarConsumables.Tyre].Expires, DateTime.MinValue);
        jsonDict.TryGetValue("conw", out pp.Consumables[eCarConsumables.WholeTeam].Expires, DateTime.MinValue);
        jsonDict.TryGetValue("cnes", out pp.Consumables[eCarConsumables.EngineTune].ExpiresSync, DateTime.MinValue);
        jsonDict.TryGetValue("cnps", out pp.Consumables[eCarConsumables.PRAgent].ExpiresSync, DateTime.MinValue);
        jsonDict.TryGetValue("cnns", out pp.Consumables[eCarConsumables.Nitrous].ExpiresSync, DateTime.MinValue);
        jsonDict.TryGetValue("cnts", out pp.Consumables[eCarConsumables.Tyre].ExpiresSync, DateTime.MinValue);
        jsonDict.TryGetValue("cnws", out pp.Consumables[eCarConsumables.WholeTeam].ExpiresSync, DateTime.MinValue);
        jsonDict.TryGetValue("care", out pp.Consumables[eCarConsumables.EngineTune].AntiCheatRacesLeft, 0);
        jsonDict.TryGetValue("carp", out pp.Consumables[eCarConsumables.PRAgent].AntiCheatRacesLeft, 0);
        jsonDict.TryGetValue("carn", out pp.Consumables[eCarConsumables.Nitrous].AntiCheatRacesLeft, 0);
        jsonDict.TryGetValue("cart", out pp.Consumables[eCarConsumables.Tyre].AntiCheatRacesLeft, 0);
        jsonDict.TryGetValue("carw", out pp.Consumables[eCarConsumables.WholeTeam].AntiCheatRacesLeft, 0);
        jsonDict.TryGetValue("cnre", out pp.Consumables[eCarConsumables.EngineTune].RacesLeft, 0);
        jsonDict.TryGetValue("cnrp", out pp.Consumables[eCarConsumables.PRAgent].RacesLeft, 0);
        jsonDict.TryGetValue("cnrn", out pp.Consumables[eCarConsumables.Nitrous].RacesLeft, 0);
        jsonDict.TryGetValue("cnrt", out pp.Consumables[eCarConsumables.Tyre].RacesLeft, 0);
        jsonDict.TryGetValue("cnrw", out pp.Consumables[eCarConsumables.WholeTeam].RacesLeft, 0);
        jsonDict.TryGetValue("cnem", out pp.Consumables[eCarConsumables.EngineTune].CmlMinutesPurchased, 0);
        jsonDict.TryGetValue("cnpm", out pp.Consumables[eCarConsumables.PRAgent].CmlMinutesPurchased, 0);
        jsonDict.TryGetValue("cnnm", out pp.Consumables[eCarConsumables.Nitrous].CmlMinutesPurchased, 0);
        jsonDict.TryGetValue("cntm", out pp.Consumables[eCarConsumables.Tyre].CmlMinutesPurchased, 0);
        jsonDict.TryGetValue("cnwm", out pp.Consumables[eCarConsumables.WholeTeam].CmlMinutesPurchased, 0);
        jsonDict.TryGetValue("duwt", out pp.DoneUpgradeWarningOnNewTier);
        jsonDict.TryGetValue("bmws", out pp.BestEverMultiplayerWinStreak);
        jsonDict.TryGetValue("bmsb", out pp.BestEverMultiplayerWinStreakBanked);
        jsonDict.TryGetValue("tmsc", out pp.TotalMultiplayerStreaksCompleted);
        jsonDict.TryGetValue("tmsl", out pp.TotalMultiplayerStreaksLost);
        jsonDict.TryGetValue("dpcl", out pp.DailyPrizeCardLastEventAt);
        jsonDict.TryGetValue("attr", out pp.AppTuttiTimedRewardLastEventAt);
        jsonDict.TryGetValue("vstr", out pp.VasTimedRewardLastEventAt);
        jsonDict.TryGetValue("pcdr", out pp.NumberOfPrizeCardRemaining);
        jsonDict.TryGetValue("nsgm", out pp.NumberOfStargazerMoments);
        jsonDict.TryGetValue("sgsp", out pp.NumberOfSportsCarPiecesRemaining);
        jsonDict.TryGetValue("sgdp", out pp.NumberOfDesiribleCarPiecesRemaining);
        jsonDict.TryGetValue("sgcp", out pp.NumberOfCommonCarPiecesRemaining);
        jsonDict.TryGetValue("sgfu", out pp.NumberOfUpgradeRewardsRemaining);
        jsonDict.TryGetValue("tcrr", out pp.NumberOfTinyCashRewardsRemaining);
        jsonDict.TryGetValue("scrr", out pp.NumberOfSmallCashRewardsRemaining);
        jsonDict.TryGetValue("mcrr", out pp.NumberOfMediumCashRewardsRemaining);
        jsonDict.TryGetValue("lcrr", out pp.NumberOfLargeCashRewardsRemaining);
        jsonDict.TryGetValue("hcrr", out pp.NumberOfHugeCashRewardsRemaining);
        jsonDict.TryGetValue("tgrr", out pp.NumberOfTinyGoldRewardsRemaining);
        jsonDict.TryGetValue("sgrr", out pp.NumberOfSmallGoldRewardsRemaining);
        jsonDict.TryGetValue("mgrr", out pp.NumberOfMediumGoldRewardsRemaining);
        jsonDict.TryGetValue("lgrr", out pp.NumberOfLargeGoldRewardsRemaining);
        jsonDict.TryGetValue("hgrr", out pp.NumberOfHugeGoldRewardsRemaining);
        jsonDict.TryGetValue("tkrr", out pp.NumberOfTinyKeyRewardsRemaining);
        jsonDict.TryGetValue("skrr", out pp.NumberOfSmallKeyRewardsRemaining);
        jsonDict.TryGetValue("mkrr", out pp.NumberOfMediumKeyRewardsRemaining);
        jsonDict.TryGetValue("lkrr", out pp.NumberOfLargeKeyRewardsRemaining);
        jsonDict.TryGetValue("hkrr", out pp.NumberOfHugeKeyRewardsRemaining);
        jsonDict.TryGetValue("sgfp", out pp.NumberOfFuelPipsRewardsRemaining);
        jsonDict.TryGetValue("sgft", out pp.NumberOfFuelRefillsRemaining);
        jsonDict.TryGetValue("trrp", out pp.NumberOfTinyRPRewardsRemaining, 0);
        jsonDict.TryGetValue("srrp", out pp.NumberOfSmallRPRewardsRemaining, 0);
        jsonDict.TryGetValue("mrrp", out pp.NumberOfMediumRPRewardsRemaining, 0);
        jsonDict.TryGetValue("lrrp", out pp.NumberOfLargeRPRewardsRemaining, 0);
        jsonDict.TryGetValue("hrrp", out pp.NumberOfHugeRPRewardsRemaining, 0);
        jsonDict.TryGetValue("nptr", out pp.NumberOfProTunerRewardsRemaining, 0);
        jsonDict.TryGetValue("nnmr", out pp.NumberOfN20ManiacRewardsRemaining, 0);
        jsonDict.TryGetValue("ntcr", out pp.NumberOfTireCrewRewardsRemaining, 0);
        jsonDict.TryGetValue("mtss", out pp.MultiplayerTutorial_HasSeenModeSelectScreen);
        jsonDict.TryGetValue("mtse", out pp.MultiplayerTutorial_HasSeenAnyEvent);
        jsonDict.TryGetValue("mtvr", out pp.MultiplayerTutorial_VersusRaceTeamCompleted);
        jsonDict.TryGetValue("mscs", out pp.MultiplayerTutorial_SuccessfullyCompletedStreak);
        jsonDict.TryGetValue("mcmt", out pp.MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted);
        jsonDict.TryGetValue("mttu", out pp.MultiplayerTutorial_TracksideUpgradesCompleted);
        jsonDict.TryGetValue("mtfp", out pp.MultiplayerTutorial_FirstPrizeCompleted);
        jsonDict.TryGetValue("mtnc", out pp.MultiplayerTutorial_CardsButNoCarPartCompleted);
        jsonDict.TryGetValue("mtec", out pp.MultiplayerTutorial_EliteClubCompleted);
        jsonDict.TryGetValue("mtlw", out pp.MultiplayerTutorial_LostWinStreakCompleted);
        jsonDict.TryGetValue("mtmr", out pp.MultiplayerTutorial_MapRaceTeamCompleted);
        jsonDict.TryGetValue("mtfc", out pp.MultiplayerTutorial_FirstCarPartCompleted);
        jsonDict.TryGetValue("mtps", out pp.MultiplayerTutorial_PrizeScreenCompleted);
        jsonDict.TryGetValue("mtep", out pp.MultiplayerTutorial_ElitePassCompleted);
        jsonDict.TryGetValue("mtlc", out pp.MultiplayerTutorial_EliteCarCompleted);
        jsonDict.TryGetValue("mrps", out pp.MultiplayerTutorial_HasSeenRespectScreen);
        jsonDict.TryGetValue("mrpb", out pp.Multiplayer_HasSeenRPBonusPopup);
        jsonDict.TryGetValue("mcuc", out pp.MultiplayerConsumableUpsellCount);
        jsonDict.TryGetValue("mmuc", out pp.MultiplayerConsumableMapUpsellCount);
        jsonDict.TryGetValue("mcsl", out pp.MultiplayerConsumableMapUpsellStreaksLost);
        jsonDict.TryGetValue("ftsl", out pp.FriendTutorial_HasSupressedTutorial);
        jsonDict.TryGetValue("ftsp", out pp.FriendTutorial_HasSeenFriendsPane);
        jsonDict.TryGetValue("stvs", out pp.SharingTutorial_VideoShareCompleted);
        jsonDict.TryGetValue("wmid", out pp.WelcomeMessageId);
        jsonDict.TryGetValue("lpli", out pp.SeasonLastPlayedLeaderboardID, -1);
        jsonDict.TryGetValue("lpei", out pp.SeasonLastPlayedEventID, -1);
        pp.SeasonPrizesAwarded = jsonDict.GetObjectList<SeasonPrizeIdentifier>("spza", new GetObjectDelegate<SeasonPrizeIdentifier>(PlayerProfileMapper.GetSeasonPrizesAwardedData));
        if (pp.SeasonPrizesAwarded == null)
        {
            pp.SeasonPrizesAwarded = new List<SeasonPrizeIdentifier>();
        }
        pp.BestCarTimes = CarTimesMapper.FromJson(jsonDict.GetJsonDict("bctf"));
        pp.FriendsRewardCollectedForCars = FriendsRewardCollectedMapper.FromJson(jsonDict.GetJsonList("frcc"));
        jsonDict.TryGetValue("udid", out pp.UDID, string.Empty);
        jsonDict.TryGetValue("uuid", out pp.UUID, string.Empty);
        pp.VideoForFuelTimestamps = jsonDict.GetObjectList<DateTime>("vffs", new GetObjectDelegate<DateTime>(PlayerProfileMapper.GetVideoForFuelTimestamps));
        if (pp.VideoForFuelTimestamps == null)
        {
            pp.VideoForFuelTimestamps = new List<DateTime>();
        }
        jsonDict.TryGetValue("hcpt", out pp.HighestCrossPromotionTierAdvertSeen);
        pp.KnownFriendsPrizes = jsonDict.GetDictFromObject<int, int>("kfp_");
        jsonDict.TryGetValue("pf__", out pp.PeakFriends);
        jsonDict.TryGetValue("ssi_", out pp.SyncdServiceID, new List<string>());
        jsonDict.TryGetValue("ffrw", out pp.FriendsRacesWon);
        jsonDict.TryGetValue("ffrl", out pp.FriendsRacesLost);
        jsonDict.TryGetValue("fcrs", out pp.FriendsCarsWon);
        jsonDict.TryGetValue("fgld", out pp.FriendsGold);
        jsonDict.TryGetValue("ftfu", out pp.FirstTimeRYFUser, true);
        jsonDict.TryGetValue("f2sa", out pp.TwoStarTimeConditionShown);
        jsonDict.TryGetValue("f3sa", out pp.ThreeStarTimeConditionShown);
        jsonDict.TryGetValue("ftuc", out pp.LastTierUnlockConditionShown);
        pp.RaceWithFriendConditionCombos = RaceWithFriendConditionCombosMapper.FromJson(jsonDict.GetJsonDict("fccs"));
        jsonDict.TryGetValue("fbyb", out pp.BeatYourBestConditionShwonCount);
        jsonDict.TryGetValue("fwcp", out pp.HasSeenWonAllCarsConditionPopup);
        jsonDict.TryGetValue("ficc", out pp.InviteFriendsConditionShownCount);
        jsonDict.TryGetValue("frhc", out pp.RaceTooHardConditionShownCount);
        jsonDict.TryGetValue("finf", out pp.InitialNetworkFriends);
        jsonDict.TryGetValue("finv", out pp.FriendsInvited);
        jsonDict.TryGetValue("fict", out pp.InviteFriendsConditionLastTimeShown, DateTime.MinValue);
        jsonDict.TryGetValue("fbct", out pp.BuyACarLastConditionLastTimeShown, DateTime.MinValue);
        jsonDict.TryGetValue("frft", out pp.RaceWithFriendConditionLastTimeShown, DateTime.MinValue);
        jsonDict.TryGetValue("ferc", out pp.FriendsEasyRaceConditionLastTimeShown, DateTime.MinValue);
        jsonDict.TryGetValue("wtrc", out pp.HasSeenRenewWholeTeamIAPCondition);
        jsonDict.TryGetValue("arhc", out pp.AllHardRacesConditionLastTimeShown, DateTime.MinValue);
        jsonDict.TryGetValue("hatv", out pp.HasAwardTelegramVisit);
        jsonDict.TryGetValue("haiv", out pp.HasAwardInstagramVisit);
        jsonDict.TryGetValue("ugas", out pp.HasUpgradedFuelTank);
        jsonDict.TryGetValue("fgas", out pp.HasReceivedFuelTankUpgradeRefill);
        jsonDict.TryGetValue("lgas", out pp.GasTankReminderLastTimeShown);
        jsonDict.TryGetValue("ngas", out pp.GasTankReminderNumberOfTimesShown);
        jsonDict.TryGetValue("tgas", out pp.GasTankReminderIDShown);
        jsonDict.TryGetValue("ufet", out pp.UnlimitedFuel.Expires);
        jsonDict.TryGetValue("ufes", out pp.UnlimitedFuel.ExpiresSync);
        jsonDict.TryGetValue("ufcm", out pp.UnlimitedFuel.CmlMinutesPurchased);
        jsonDict.TryGetValue("ufrp", out pp.UnlimitedFuel.HasSeenRenewalPopup);
        jsonDict.TryGetValue("ufrt", out pp.UnlimitedFuel.RaceTeamPopupTimesSeen);
        jsonDict.TryGetValue("nrlc", out pp.NumberOfRelaysCompetetedIn, 1);
        string value;
        if (jsonDict.TryGetValue("ogso", out value))
        {
            pp.OptionGarageSortOrder = (GarageSortOrder)((int)Enum.Parse(typeof(GarageSortOrder), value));
        }
        jsonDict.TryGetValue("orso", out pp.OptionReverseSortOrder);
        pp.UnconfirmedDailyBattleResults = jsonDict.GetObjectList<DailyBattleCompletionRecord>("udbr", new GetObjectDelegate<DailyBattleCompletionRecord>(DailyBattleCompletionRecord.ReadFromJson));
        if (pp.UnconfirmedDailyBattleResults == null)
        {
            pp.UnconfirmedDailyBattleResults = new List<DailyBattleCompletionRecord>();
        }
        pp.DailyBattleGoldConfirmed = jsonDict.GetInt("dbcc");
        pp.DailyBattleCashConfirmed = jsonDict.GetInt("dbgc");
        pp.DailyBattleRewardsRemoved = jsonDict.GetInt("dbrr");
        jsonDict.TryGetValue("fapz", out pp.FuelAdPromptToggles);
        if (pp.FuelAdPromptToggles == null)
        {
            pp.FuelAdPromptToggles = new List<bool>();
        }
        jsonDict.TryGetValue("swti", out pp.HasSeenWorldTourIntroduction);
        jsonDict.TryGetValue("swtl", out pp.SeenWorldTourLockedIntroCount);
        jsonDict.TryGetValue("ofwt", out pp.OverrideRYFObjectives);
        jsonDict.TryGetValue("omwt", out pp.OverrideRTWObjectives);
        jsonDict.TryGetValue("lbts", out pp.LastBundleOfferTimeShown, DateTime.MinValue);
        pp.WorldTourEventIDsWithAnimitionCompleted = jsonDict.GetObjectList<WorldTourAnimationsCompleted>("wtas", new GetObjectDelegate<WorldTourAnimationsCompleted>(WorldTourAnimSeenMapper.ReadFromJson));
        if (pp.WorldTourEventIDsWithAnimitionCompleted == null)
        {
            pp.WorldTourEventIDsWithAnimitionCompleted = new List<WorldTourAnimationsCompleted>();
        }
        else
        {
            WorldTourAnimSeenMapper.RunTierItaliaFixMigration(ref pp.WorldTourEventIDsWithAnimitionCompleted);
        }
        jsonDict.TryGetValue("wtta", out pp.SeasonUnlockableThemesSeen, new List<string>());
        pp.WTBoostNitrousStatus = jsonDict.GetObject<WorldTourBoostNitrous>("wtbn", new GetObjectDelegate<WorldTourBoostNitrous>(WorldTourBoostNitrous.ReadFromJson));
        if (pp.WTBoostNitrousStatus == null)
        {
            pp.WTBoostNitrousStatus = new WorldTourBoostNitrous();
        }
        jsonDict.TryGetValue("hvms", out pp.HasVisitedMechanicScreen);
        if (!pp.HasVisitedMechanicScreen && pp.LegacyObjectivesCompleted.Contains(LegacyObjectivesManager.GetLegacyObjectiveID("IntroduceMechanic")))
        {
            pp.HasVisitedMechanicScreen = true;
        }
        jsonDict.TryGetValue("hvds", out pp.HasVisitedManufacturerScreen);
        if (!pp.HasVisitedManufacturerScreen && pp.LegacyObjectivesCompleted.Contains(LegacyObjectivesManager.GetLegacyObjectiveID("IntroduceCarDealer")))
        {
            pp.HasVisitedManufacturerScreen = true;
        }
        jsonDict.TryGetValue("hocr", out pp.HasOfferedCrewRacesCars);
        if (!pp.HasOfferedCrewRacesCars && pp.LegacyObjectivesCompleted.Contains(LegacyObjectivesManager.GetLegacyObjectiveID("CrewRaces")))
        {
            pp.HasOfferedCrewRacesCars = true;
        }
        jsonDict.TryGetValue("hfrt", out pp.HasFinishedRYFTutorial);
        jsonDict.TryGetValue("hsfn", out pp.HasSeenFacebookNag);
        jsonDict.TryGetValue("hsmi", out pp.HasSeenMultiplayerIntroScreen);
        jsonDict.TryGetValue("hsii", out pp.HasSeenInternationalIntroScreen);
        jsonDict.TryGetValue("wcau", out pp.WinCountAfterUpgrade);
        jsonDict.TryGetValue("hwrc", out pp.HasWeeklyRewardToClaim);
        jsonDict.TryGetValue("pwlc", out pp.PreviousWeeklyLeaderboardCheck);
        //if (pp.OnlineRacesLost > 0 || pp.OnlineRacesWon > 0 || pp.ObjectivesCompleted.Contains(LegacyObjectivesManager.GetLegacyObjectiveID("UnlockMultiplayer")))
        //{
        //    pp.HasSeenMultiplayerIntroScreen = true;
        //}
        pp.PopupsData.FromJson(ref jsonDict);
        jsonDict.TryGetValue("plli", out pp.LastPlayedMultiplayerLeaderboardID, 0);
        jsonDict.TryGetValue("mps", out pp.MapPaneSelected, 0);
        jsonDict.TryGetDictFromObject<string, string>("wtps", out pp.TierXPinSelections);
        PlayerProfileMapper.GetDeferredNarrativeSceneData(jsonDict, out pp.WTDeferredNarrativeScenes);

        jsonDict.TryGetValue("lwset", out pp.LastWinStreakExtendedTime);
        jsonDict.TryGetValue("smpchw", out pp.SMPChallengeWins);
        jsonDict.TryGetValue("smpcw", out pp.SMPConsecutiveWins);
        jsonDict.TryGetValue("smpcl", out pp.SMPConsecutiveLoses);
        jsonDict.TryGetValue("smptr", out pp.SMPTotalRaces);
        jsonDict.TryGetValue("smptrl", out pp.SMPTotalRacesLastSession);
        jsonDict.TryGetValue("smpwls", out pp.SMPWinsLastSession);
        jsonDict.TryGetValue("smpssd", out pp.SMPStartSessionDate);
        jsonDict.TryGetValue("smpwsid", out pp.SMPWinStreakID);
        jsonDict.TryGetValue("umau", out pp.UseMileAsUnit);
        int num20 = 0;
        //if (jsonDict.TryGetValue("smpsoc", out num20))
        //{
        //    pp.SMPLastSuccessfulSocketFamily = (short)num20;
        //}
        if (!jsonDict.TryGetValue("smpwca", out pp.IsSMPWinChallengeAvailable))
        {
            pp.IsSMPWinChallengeAvailable = true;
        }
        if (!jsonDict.TryGetValue("smpact", out pp.SMPWinChallengeActivationTime))
        {
            pp.SMPWinChallengeActivationTime = DateTime.MinValue;
        }
        if (!jsonDict.TryGetValue("smpbdt", out pp.SMPLastLobbyBotDisclaimerTime))
        {
            pp.SMPLastLobbyBotDisclaimerTime = DateTime.MinValue;
        }
        jsonDict.TryGetValue("ftpc", out pp.FirstPurchaseDone);
        jsonDict.TryGetValue("fraw", out pp.FirstRewardedAdWatched);
        jsonDict.TryGetValue("fiaw", out pp.FirstInterstitialAdWatched);
        jsonDict.TryGetValue("febr", out pp.FirstEnterBeginnerRegulation);
        jsonDict.TryGetValue("fenr", out pp.FirstEnterNormalRegulation);
        jsonDict.TryGetValue("fehr", out pp.FirstEnterHardRegulation);
        jsonDict.TryGetValue("nrw1", out pp.NumberOfRaceToWinCrew1);
        jsonDict.TryGetValue("nrw2", out pp.NumberOfRaceToWinCrew2);
        jsonDict.TryGetValue("nrw3", out pp.NumberOfRaceToWinCrew3);
        jsonDict.TryGetValue("nrw32", out pp.NumberOfRaceToWinCrew32);
        jsonDict.TryGetValue("nrw4", out pp.NumberOfRaceToWinCrew4);
        jsonDict.TryGetValue("nrw42", out pp.NumberOfRaceToWinCrew42);
        jsonDict.TryGetValue("nrw43", out pp.NumberOfRaceToWinCrew43);
        jsonDict.TryGetValue("fw1", out pp.FirstRace1);
        jsonDict.TryGetValue("fw2", out pp.FirstRace2);
        jsonDict.TryGetValue("fw3", out pp.FirstRace3);
        jsonDict.TryGetValue("fw32", out pp.FirstRace32);
        jsonDict.TryGetValue("fw4", out pp.FirstRace4);
        jsonDict.TryGetValue("fw42", out pp.FirstRace41);
        jsonDict.TryGetValue("fw43", out pp.FirstRace42);
        jsonDict.TryGetValue("ifbu", out pp.IsFirstBodyUpgrade);
        jsonDict.TryGetValue("ifeu", out pp.IsFirstEngineUpgrade);
        jsonDict.TryGetValue("ifiu", out pp.IsFirstIntakeUpgrade);
        jsonDict.TryGetValue("ifnu", out pp.IsFirstNitrousUpgrade);
        jsonDict.TryGetValue("iftu", out pp.IsFirstTransmissionUpgrade);
        jsonDict.TryGetValue("ifru", out pp.IsFirstTurboUpgrade);
        jsonDict.TryGetValue("ifyu", out pp.IsFirstTyresUpgrade);
        jsonDict.TryGetValue("iftfu", out pp.IsFirstTapToUpgrade);
        jsonDict.TryGetValue("isdb", out pp.IsDefaultBranch);
        jsonDict.TryGetValue("isab", out pp.IsA_Branch);
        jsonDict.TryGetValue("isbb", out pp.IsB_Branch);
        jsonDict.TryGetValue("dbst", out pp.DefaultBranch);
        jsonDict.TryGetValue("abst", out pp.A_Branch);
        jsonDict.TryGetValue("bbst", out pp.B_Branch);
       
    }

    public static PlayerProfileData FromJson(string json)
    {
        JsonDict jsonDict = new JsonDict();
        try
        {
            if (!jsonDict.Read(json))
            {
                PlayerProfileData result = null;
                return result;
            }
        }
        catch (Exception e)// var_1_1E)
        {
            Debug.LogError(e.Message);
            PlayerProfileData result = null;
            return result;
        }
        PlayerProfileData result2 = new PlayerProfileData();
        PlayerProfileMapper.GetPlayerProfileData(jsonDict, ref result2);
        return result2;
    }

    private static void SetScoreData(SocialGamePlatformSelector.ScoreData scoreData, ref JsonDict jsonDict)
    {
        jsonDict.Set("sc", scoreData.score);
        jsonDict.Set("c", scoreData.leaderboard.Idx);
        jsonDict.Set("s", (int)scoreData.status);
    }

    private static void SetAchievementData(SocialGamePlatformSelector.AchievementData achievementData, ref JsonDict jsonDict)
    {
        jsonDict.Set("a", achievementData.achievement.Idx);
        jsonDict.Set("s", (int)achievementData.status);
    }

    private static void SetSeasonPrizeAwarded(SeasonPrizeIdentifier prize, ref JsonDict jsonDict)
    {
        jsonDict.Set("spld", prize.LeaderboardID);
        jsonDict.Set("sppd", prize.PrizeID);
    }

    private static void SetVideoForFuelTimestamp(DateTime timestamp, ref JsonDict jsonDict)
    {
        jsonDict.Set("vfft", timestamp);
    }

    private static void SetPlayerReplay(PlayerReplay zPlayerReplay, ref JsonDict jsonDict)
    {
        string value = zPlayerReplay.ToJson();
        jsonDict.Set("prs", value);
    }

    private static void SetArrival(Arrival arrival, ref JsonDict jsonDict)
    {
        jsonDict.Set("apno", arrival.AssociatedPushNotification);
        jsonDict.Set("dlts", arrival.deliveryTimeSecs);
        jsonDict.Set("duet", arrival.dueTime);
        jsonDict.SetEnum<ArrivalType>("arvt", arrival.arrivalType);
        jsonDict.Set("cari", arrival.carId);
        jsonDict.Set("coli", arrival.ColourIndex);
        jsonDict.SetEnum<eUpgradeType>("upgt", arrival.upgradeType);
        jsonDict.Set("datd", arrival.doesAutoTickDown);
    }

    private static void SetUpgradeStatus(CarUpgradeStatus upgradeStatus, ref JsonDict jsonDict)
    {
        jsonDict.Set("lvlo", (int)upgradeStatus.levelOwned);
        if (upgradeStatus.levelFitted != upgradeStatus.levelOwned)
        {
            jsonDict.Set("lvlf", (int)upgradeStatus.levelFitted);
        }
        if (upgradeStatus.evoOwned != 0)
        {
            jsonDict.Set("lvle", (int)upgradeStatus.evoOwned);
        }
    }

    private static void SetCarGarageInstance(CarGarageInstance carOwned, ref JsonDict jsonDict)
    {
        jsonDict.Set("crdb", carOwned.CarDBKey);
        jsonDict.Set("apci", carOwned.AppliedColourIndex);
        jsonDict.Set("apln", carOwned.AppliedLiveryName);
        jsonDict.Set("bqmt", carOwned.BestQuarterMileTime);
        jsonDict.Set("bhmt", carOwned.BestHalfMileTime);
        jsonDict.Set("ditr", carOwned.DistanceTravelled);
        jsonDict.Set("rcat", carOwned.RacesAttempted);
        jsonDict.Set("rwon", carOwned.RacesWon);
        jsonDict.Set("msou", carOwned.MoneySpentOnUpgrades);
        jsonDict.Set("nuub", carOwned.NumUpgradesBought);
        jsonDict.Set("tsat", carOwned.TopSpeedAttained);
        jsonDict.Set("ccna", carOwned.CustomCarNags);
        jsonDict.Set("cbht", carOwned.BodyHeight);
        jsonDict.SetObjectArrayFromDictValues("upst", carOwned.UpgradeStatus, SetUpgradeStatus, CarUpgrades.ValidUpgrades);
        jsonDict.SetObjectList("veic", carOwned.EquipItemCollection.ItemList, SetEquipItemCollection);
        if (carOwned.OwnedLiveries == null)
        {
            carOwned.OwnedLiveries = new List<string>();
        }
        jsonDict.Set("owlv", carOwned.OwnedLiveries);
        jsonDict.Set("cppi", carOwned.CurrentPPIndex);
        jsonDict.SetEnum<eCarTier>("ctie", carOwned.CurrentTier);
        jsonDict.SetObject<NumberPlate>("nupl", carOwned.NumberPlate, SetNumberPlate);
        jsonDict.Set("elit", carOwned.EliteCar);
        jsonDict.Set("spup", carOwned.SportsUpgrade);
        jsonDict.Set("tlad", carOwned.TightLoopQuarterMileTimeAdjust);
        if (carOwned.UnlockedLiveries != null && carOwned.UnlockedLiveries.Count > 0)
        {
            jsonDict.Set("ulvs", carOwned.UnlockedLiveries);
        }
        if (carOwned.NewLiveries != null && carOwned.NewLiveries.Count > 0)
        {
            jsonDict.Set("nlvs", carOwned.NewLiveries);
        }
    }

    private static void SetEquipItemCollection(VirtualEquipItem equipItem, ref JsonDict jsonDict)
    {
        jsonDict.Set("crid", equipItem.CarID);
        jsonDict.Set("eqpd", equipItem.Equiped);
        jsonDict.Set("viid", equipItem.VirtualItemID);
        jsonDict.SetEnum("ittp", equipItem.ItemType);
    }

    private static void SetMultiplayerCarPrize(MultiplayerCarPrize mpCarPrize, ref JsonDict jsonDict)
    {
        jsonDict.Set("wodb", mpCarPrize.CarDBKey);
        jsonDict.Set("ppwo", mpCarPrize.PiecesWon);
        jsonDict.Set("npto", mpCarPrize.NumPiecesTotal);
    }

    private static void SetNumberPlate(NumberPlate numberPlate, ref JsonDict jsonDict)
    {
        if (numberPlate == null)
        {
            return;
        }
        if (!string.IsNullOrEmpty(numberPlate.Text))
        {
            jsonDict.Set("txt", numberPlate.Text);
        }
    }

    private static void SetFriendPrizes(Dictionary<int, int> friendPrizes, ref JsonDict jsonDict)
    {
        foreach (KeyValuePair<int, int> current in friendPrizes)
        {
            jsonDict.Set(current.Key.ToString(), current.Value);
        }
    }

    private static void SetTierXPinSelections(Dictionary<string, string> txp, ref JsonDict jsonDict)
    {
        foreach (KeyValuePair<string, string> current in txp)
        {
            jsonDict.Set(current.Key, current.Value);
        }
    }

    private static void SetPlayerProfileData(PlayerProfileData pp, ref JsonDict jsonDict)
    {
        jsonDict.Set("name", pp.Username);
        jsonDict.Set("dsnm", pp.DisplayName);
        jsonDict.Set("avid", pp.AvatarID);
        jsonDict.Set("plst", pp.PlayerStar);
        jsonDict.SetEnum("pllg", pp.PlayerLeague);
        jsonDict.Set("new2", true);
        jsonDict.Set("date", pp.dateTimeLastSaved);
        jsonDict.Set("prvr", pp.ProductVersionLastSaved);
        jsonDict.Set("boqt", pp.BestOverallQuarterMileTime);
        jsonDict.Set("boht", pp.BestOverallHalfMileTime);
        jsonDict.Set("tdtr", pp.TotalDistanceTravelled);
        jsonDict.Set("tplt", pp.TotalPlayTime);
        jsonDict.Set("tgrt", pp.TotalGarageTime);
        jsonDict.Set("casp", pp.CashSpent);
        jsonDict.Set("caea", pp.CashEarned);
        jsonDict.Set("cabo", pp.CashBought);
        jsonDict.Set("iacs", pp.IAPCashSpent);
        jsonDict.Set("goea", pp.GoldEarned);
        jsonDict.Set("gosp", pp.GoldSpent);
        jsonDict.Set("gobo", pp.GoldBought);
        jsonDict.Set("iags", pp.IAPGoldSpent);
        jsonDict.Set("gtea", pp.GachaTokensEarned);
        jsonDict.Set("gtsp", pp.GachaTokensSpent);
        jsonDict.Set("gbke", pp.GachaBronzeKeysEarned);
        jsonDict.Set("gbks", pp.GachaBronzeKeysSpent);
        jsonDict.Set("igbk", pp.IAPGachaBronzeKeysSpent);
        jsonDict.Set("gske", pp.GachaSilverKeysEarned);
        jsonDict.Set("gsks", pp.GachaSilverKeysSpent);
        jsonDict.Set("igsk", pp.IAPGachaSilverKeysSpent);
        jsonDict.Set("ggke", pp.GachaGoldKeysEarned);
        jsonDict.Set("ggks", pp.GachaGoldKeysSpent);
        jsonDict.Set("iggk", pp.IAPGachaGoldKeysSpent);
        jsonDict.Set("dbdt", pp.DailyBattlesDoneToday);
        jsonDict.Set("dbcd", pp.DailyBattlesConsecutiveDaysCount);
        jsonDict.Set("dble", pp.DailyBattlesLastEventAt);
        jsonDict.Set("dbwl", pp.DailyBattlesWonLast);
        jsonDict.Set("bnou", pp.BoostNitrousUsed);
        jsonDict.Set("nful", pp.FreeUpgradesLeft);
        jsonDict.SetEnum<BossChallengeStateEnum>("bcs1", pp.BossChallengeStateT1);
        jsonDict.SetEnum<BossChallengeStateEnum>("bcs2", pp.BossChallengeStateT2);
        jsonDict.SetEnum<BossChallengeStateEnum>("bcs3", pp.BossChallengeStateT3);
        jsonDict.SetEnum<BossChallengeStateEnum>("bcs4", pp.BossChallengeStateT4);
        jsonDict.SetEnum<BossChallengeStateEnum>("bcs5", pp.BossChallengeStateT5);
        jsonDict.Set("mumu", pp.OptionMusicMute);
        jsonDict.Set("somu", pp.OptionSoundMute);
        jsonDict.Set("onot", pp.OptionNotifications);
        jsonDict.Set("lmlm", pp.HasSeenLowMemoryLanguageMessage);
        jsonDict.Set("raen", pp.RacesEntered);
        jsonDict.Set("tutr", pp.TutorialRacesAttempted);
        jsonDict.Set("mfer", pp.MechanicFettledRaces);
        jsonDict.Set("rawo", pp.RacesWon);
        jsonDict.Set("ralo", pp.RacesLost);
        jsonDict.Set("ntwe", pp.NumTweets);
        jsonDict.Set("nfbp", pp.NumFBPosts);
        jsonDict.Set("ect1", pp.EventsCompletedTier1);
        jsonDict.Set("ect2", pp.EventsCompletedTier2);
        jsonDict.Set("ect3", pp.EventsCompletedTier3);
        jsonDict.Set("ect4", pp.EventsCompletedTier4);
        jsonDict.Set("ect5", pp.EventsCompletedTier5);
        jsonDict.Set("t1cs", pp.Tier1CarSpecificEventTarget);
        jsonDict.Set("t2cs", pp.Tier2CarSpecificEventTarget);
        jsonDict.Set("t3cs", pp.Tier3CarSpecificEventTarget);
        jsonDict.Set("t4cs", pp.Tier4CarSpecificEventTarget);
        jsonDict.Set("t5cs", pp.Tier5CarSpecificEventTarget);
        jsonDict.Set("t1ms", pp.Tier1ManufacturerSpecificEventTarget);
        jsonDict.Set("t2ms", pp.Tier2ManufacturerSpecificEventTarget);
        jsonDict.Set("t3ms", pp.Tier3ManufacturerSpecificEventTarget);
        jsonDict.Set("t4ms", pp.Tier4ManufacturerSpecificEventTarget);
        jsonDict.Set("t5ms", pp.Tier5ManufacturerSpecificEventTarget);
        jsonDict.Set("wanu", pp.HaveWarnedAboutNitrousUpgrade);
        jsonDict.Set("hsif", pp.HasSignedIntoFacebookBefore);
        jsonDict.Set("hsig", pp.HasSignedIntoGameCentreBefore);
        jsonDict.Set("hsip", pp.HasSignedIntoGooglePlayGamesBefore);
        jsonDict.Set("ggsc", pp.GPGSignInCancellations);
        jsonDict.Set("mtrr", pp.MechanicTuningRacesRemaining);
        jsonDict.Set("sfmp", pp.HaveShownFirstMechanicPopUp);
        jsonDict.Set("colo", pp.ContiguousLosses);
        jsonDict.Set("cplo", pp.ContiguousProgressionLosses);
        jsonDict.Set("cplt", pp.ContiguousProgressionLossesTriggered);
        jsonDict.Set("hbfc", pp.HasBoughtFirstCar);
        jsonDict.Set("hbfu", pp.HasBoughtFirstUpgrade);
        jsonDict.Set("hcpn", pp.HasChoosePlayerName);
        jsonDict.Set("lang", pp.LastAgentNag);
        jsonDict.Set("lply", pp.LastPlayedMultiplayer);
        jsonDict.Set("lpec", pp.LastPlayedEliteClub);
        jsonDict.Set("lpwt", pp.LastPlayedRaceTheWorldWorldTour);
        jsonDict.Set("lpld", pp.MultiplayerDifficulty);
        jsonDict.Set("empd", pp.EliteMultiplayerDifficulty);
        jsonDict.Set("wtmd", pp.MultiplayerEventDifficulty);
        jsonDict.Set("uspl", pp.UserStartedPlaying);
        jsonDict.Set("usls", pp.UserStartedLastSession);
        jsonDict.Set("scdb", pp.CurrentlySelectedCarDBKey);
        jsonDict.Set("plxp", pp.PlayerXP);
        jsonDict.Set("pllv", pp.PlayerLevel);
        jsonDict.Set("nrlc", pp.NumberOfRelaysCompetetedIn);
        jsonDict.Set("plrk", pp.WorldRank);
        jsonDict.Set("plwr", pp.PreviousWorldRank);
        jsonDict.Set("ulcc", pp.UTCLastClockChange);
        jsonDict.Set("ct2f", pp.CanTrySecondFacebookNag);
        jsonDict.Set("relf", pp.RacesEnteredAtLastFacebookNag);
        jsonDict.Set("scnt", pp.SessionsCounter);
        jsonDict.Set("hlof", pp.HasLikedOnFacebook);
        jsonDict.Set("hfot", pp.HasFollowedUsOnTwitter);
        jsonDict.Set("pcap", pp.PreferredCsrAvatarPic);
        jsonDict.SetObjectList<MultiplayerCarPrize>("mpcp", pp.CarsWonInMultiplayer, new SetObjectDelegate<MultiplayerCarPrize>(PlayerProfileMapper.SetMultiplayerCarPrize));
        jsonDict.SetObjectList<CarGarageInstance>("caow", pp.CarsOwned, PlayerProfileMapper.SetCarGarageInstance);
        jsonDict.Set("nwcs", pp.NewCars);
        jsonDict.Set("evcm", pp.EventsCompleted);
        jsonDict.Set("objc", pp.ObjectivesCompleted);
        jsonDict.Set("obco", pp.LegacyObjectivesCompleted);
        jsonDict.Set("obca", pp.ObjectivesCollected);
        JsonDict jsonDict7 = new JsonDict();
        foreach (KeyValuePair<string, JsonDict> current26 in pp.ActiveObjectives)
        {
            jsonDict7.Set(current26.Key, current26.Value);
        }
        jsonDict.Set("obja", jsonDict7);
        jsonDict.Set("doet", pp.ObjectiveEndTime);
        jsonDict.SetObjectList<Arrival>("arqu", pp.arrivalQueue, PlayerProfileMapper.SetArrival);
        jsonDict.SetObjectList("playerAchievements", pp.playerAchievements, PlayerProfileMapper.SetAchievementData);
        jsonDict.SetObjectList("playerScores", pp.playerScores, PlayerProfileMapper.SetScoreData);
        jsonDict.Set("fupi", pp.FuelPips);
        jsonDict.Set("fart", pp.LastFuelAutoReplenishedTime);
        jsonDict.Set("llav", pp.LastLegalAgreementVersion);
        jsonDict.Set("ladt", pp.LastAdvertDate);
        jsonDict.Set("adct", pp.AdCount);
        jsonDict.Set("gfrl", pp.GoldFuelRefills);
        jsonDict.Set("cums", pp.CumulativeSessions);
        jsonDict.Set("cpip", pp.CrewProgressionIntroductionPlayed);
        jsonDict.Set("hafc", pp.HasAttemptedFirstCrew);
        jsonDict.Set("lsmd", pp.LastServerMessageDisplayedID);
        jsonDict.Set("lsmc", pp.LastServerMessageDisplayedCount);
        jsonDict.Set("fifc", pp.FacebookInviteFuelRewardsCount);
        jsonDict.Set("fift", pp.FacebookInviteFuelRewardsTime);
        jsonDict.Set("fsra", pp.IsFacebookSSORewardAllowed);
        jsonDict.Set("fbid", pp.FacebookID);
        jsonDict.Set("tifc", pp.TwitterInviteFuelRewardsCount);
        jsonDict.Set("tift", pp.TwitterInviteFuelRewardsTime);
        jsonDict.Set("tcfc", pp.TwitterCashRewardsCount);
        jsonDict.Set("tcft", pp.TwitterCashRewardsTime);
        jsonDict.Set("ratb", pp.DoneRateAppTriggerBuyCar);
        jsonDict.Set("ratc", pp.DoneRateAppTriggerCrewMember);
        jsonDict.Set("ludn", pp.LastUpgradeDateTimeNag);
        jsonDict.Set("msmi", pp.HasHadMechanicSlowMotionIntroduction);
        jsonDict.Set("hsnt", pp.HasSeenNitrousTutorial);
        jsonDict.Set("btnq", pp.BestTwitterNagTimeQtr);
        jsonDict.Set("sscp", pp.ShouldShowSkipTo2ndCrewMemberPopup);
        jsonDict.Set("trhi", pp.TransactionHistory);
        jsonDict.Set("lbcr", pp.LastBoughtCarRacesEntered);
        jsonDict.Set("mdcb", pp.MultiplayerDCBDifficulty);
        jsonDict.Set("mdcw", pp.MultiplayerDCBConsecutiveWins);
        jsonDict.Set("mdcl", pp.MultiplayerDCBConsecutiveLoses);
        jsonDict.Set("morw", pp.OnlineRacesWon);
        jsonDict.Set("morl", pp.OnlineRacesLost);
        jsonDict.Set("crld", pp.ConsecutiveRacesWonAtLowDifficulty);
        jsonDict.Set("molr", pp.LastOnlineRace);
        jsonDict.Set("mort", pp.OnlineRacesWonToday);
        jsonDict.Set("mlrt", pp.OnlineRacesLostToday);
        jsonDict.Set("cone", pp.Consumables[eCarConsumables.EngineTune].Expires);
        jsonDict.Set("conp", pp.Consumables[eCarConsumables.PRAgent].Expires);
        jsonDict.Set("conn", pp.Consumables[eCarConsumables.Nitrous].Expires);
        jsonDict.Set("cont", pp.Consumables[eCarConsumables.Tyre].Expires);
        jsonDict.Set("conw", pp.Consumables[eCarConsumables.WholeTeam].Expires);
        jsonDict.Set("cnes", pp.Consumables[eCarConsumables.EngineTune].ExpiresSync);
        jsonDict.Set("cnps", pp.Consumables[eCarConsumables.PRAgent].ExpiresSync);
        jsonDict.Set("cnns", pp.Consumables[eCarConsumables.Nitrous].ExpiresSync);
        jsonDict.Set("cnts", pp.Consumables[eCarConsumables.Tyre].ExpiresSync);
        jsonDict.Set("cnws", pp.Consumables[eCarConsumables.WholeTeam].ExpiresSync);
        jsonDict.Set("care", pp.Consumables[eCarConsumables.EngineTune].AntiCheatRacesLeft);
        jsonDict.Set("carp", pp.Consumables[eCarConsumables.PRAgent].AntiCheatRacesLeft);
        jsonDict.Set("carn", pp.Consumables[eCarConsumables.Nitrous].AntiCheatRacesLeft);
        jsonDict.Set("cart", pp.Consumables[eCarConsumables.Tyre].AntiCheatRacesLeft);
        jsonDict.Set("carw", pp.Consumables[eCarConsumables.WholeTeam].AntiCheatRacesLeft);
        jsonDict.Set("cnre", pp.Consumables[eCarConsumables.EngineTune].RacesLeft);
        jsonDict.Set("cnrp", pp.Consumables[eCarConsumables.PRAgent].RacesLeft);
        jsonDict.Set("cnrn", pp.Consumables[eCarConsumables.Nitrous].RacesLeft);
        jsonDict.Set("cnrt", pp.Consumables[eCarConsumables.Tyre].RacesLeft);
        jsonDict.Set("cnrw", pp.Consumables[eCarConsumables.WholeTeam].RacesLeft);
        jsonDict.Set("cnem", pp.Consumables[eCarConsumables.EngineTune].CmlMinutesPurchased);
        jsonDict.Set("cnpm", pp.Consumables[eCarConsumables.PRAgent].CmlMinutesPurchased);
        jsonDict.Set("cnnm", pp.Consumables[eCarConsumables.Nitrous].CmlMinutesPurchased);
        jsonDict.Set("cntm", pp.Consumables[eCarConsumables.Tyre].CmlMinutesPurchased);
        jsonDict.Set("cnwm", pp.Consumables[eCarConsumables.WholeTeam].CmlMinutesPurchased);
        jsonDict.Set("duwt", pp.DoneUpgradeWarningOnNewTier);
        jsonDict.Set("bmws", pp.BestEverMultiplayerWinStreak);
        jsonDict.Set("bmsb", pp.BestEverMultiplayerWinStreakBanked);
        jsonDict.Set("tmsc", pp.TotalMultiplayerStreaksCompleted);
        jsonDict.Set("tmsl", pp.TotalMultiplayerStreaksLost);
        jsonDict.Set("nsgm", pp.NumberOfStargazerMoments);
        jsonDict.Set("mtss", pp.MultiplayerTutorial_HasSeenModeSelectScreen);
        jsonDict.Set("mtse", pp.MultiplayerTutorial_HasSeenAnyEvent);
        jsonDict.Set("mtvr", pp.MultiplayerTutorial_VersusRaceTeamCompleted);
        jsonDict.Set("mscs", pp.MultiplayerTutorial_SuccessfullyCompletedStreak);
        jsonDict.Set("mcmt", pp.MultiplayerTutorial_ConsumablesTimeRaceTeamCompleted);
        jsonDict.Set("mttu", pp.MultiplayerTutorial_TracksideUpgradesCompleted);
        jsonDict.Set("mtfp", pp.MultiplayerTutorial_FirstPrizeCompleted);
        jsonDict.Set("mtnc", pp.MultiplayerTutorial_CardsButNoCarPartCompleted);
        jsonDict.Set("mtec", pp.MultiplayerTutorial_EliteClubCompleted);
        jsonDict.Set("mtlw", pp.MultiplayerTutorial_LostWinStreakCompleted);
        jsonDict.Set("mtmr", pp.MultiplayerTutorial_MapRaceTeamCompleted);
        jsonDict.Set("mtfc", pp.MultiplayerTutorial_FirstCarPartCompleted);
        jsonDict.Set("mtps", pp.MultiplayerTutorial_PrizeScreenCompleted);
        jsonDict.Set("mtep", pp.MultiplayerTutorial_ElitePassCompleted);
        jsonDict.Set("mtlc", pp.MultiplayerTutorial_EliteCarCompleted);
        jsonDict.Set("mrps", pp.MultiplayerTutorial_HasSeenRespectScreen);
        jsonDict.Set("mrpb", pp.Multiplayer_HasSeenRPBonusPopup);
        jsonDict.Set("mcuc", pp.MultiplayerConsumableUpsellCount);
        jsonDict.Set("mmuc", pp.MultiplayerConsumableMapUpsellCount);
        jsonDict.Set("mcsl", pp.MultiplayerConsumableMapUpsellStreaksLost);
        jsonDict.Set("ftsl", pp.FriendTutorial_HasSupressedTutorial);
        jsonDict.Set("ftsp", pp.FriendTutorial_HasSeenFriendsPane);
        jsonDict.Set("stvs", pp.SharingTutorial_VideoShareCompleted);
        jsonDict.Set("dpcl", pp.DailyPrizeCardLastEventAt);
        jsonDict.Set("attr", pp.AppTuttiTimedRewardLastEventAt);
        jsonDict.Set("vstr", pp.VasTimedRewardLastEventAt);
        jsonDict.Set("pcdr", pp.NumberOfPrizeCardRemaining);
        jsonDict.Set("sgsp", pp.NumberOfSportsCarPiecesRemaining);
        jsonDict.Set("sgdp", pp.NumberOfDesiribleCarPiecesRemaining);
        jsonDict.Set("sgcp", pp.NumberOfCommonCarPiecesRemaining);
        jsonDict.Set("sgfu", pp.NumberOfUpgradeRewardsRemaining);
        jsonDict.Set("tgrr", pp.NumberOfTinyGoldRewardsRemaining);
        jsonDict.Set("sgrr", pp.NumberOfSmallGoldRewardsRemaining);
        jsonDict.Set("mgrr", pp.NumberOfMediumGoldRewardsRemaining);
        jsonDict.Set("lgrr", pp.NumberOfLargeGoldRewardsRemaining);
        jsonDict.Set("hgrr", pp.NumberOfHugeGoldRewardsRemaining);
        jsonDict.Set("tkrr", pp.NumberOfTinyKeyRewardsRemaining);
        jsonDict.Set("skrr", pp.NumberOfSmallKeyRewardsRemaining);
        jsonDict.Set("mkrr", pp.NumberOfMediumKeyRewardsRemaining);
        jsonDict.Set("lkrr", pp.NumberOfLargeKeyRewardsRemaining);
        jsonDict.Set("hkrr", pp.NumberOfHugeKeyRewardsRemaining);
        jsonDict.Set("tcrr", pp.NumberOfTinyCashRewardsRemaining);
        jsonDict.Set("scrr", pp.NumberOfSmallCashRewardsRemaining);
        jsonDict.Set("mcrr", pp.NumberOfMediumCashRewardsRemaining);
        jsonDict.Set("lcrr", pp.NumberOfLargeCashRewardsRemaining);
        jsonDict.Set("hcrr", pp.NumberOfHugeCashRewardsRemaining);
        jsonDict.Set("sgfp", pp.NumberOfFuelPipsRewardsRemaining);
        jsonDict.Set("sgft", pp.NumberOfFuelRefillsRemaining);
        jsonDict.Set("trrp", pp.NumberOfTinyRPRewardsRemaining);
        jsonDict.Set("srrp", pp.NumberOfSmallRPRewardsRemaining);
        jsonDict.Set("mrrp", pp.NumberOfMediumRPRewardsRemaining);
        jsonDict.Set("lrrp", pp.NumberOfLargeRPRewardsRemaining);
        jsonDict.Set("hrrp", pp.NumberOfHugeRPRewardsRemaining);
        jsonDict.Set("nptr", pp.NumberOfProTunerRewardsRemaining);
        jsonDict.Set("nnmr", pp.NumberOfN20ManiacRewardsRemaining);
        jsonDict.Set("ntcr", pp.NumberOfTireCrewRewardsRemaining);
        jsonDict.Set("wmid", pp.WelcomeMessageId);
        jsonDict.Set("lpli", pp.SeasonLastPlayedLeaderboardID);
        jsonDict.Set("lpei", pp.SeasonLastPlayedEventID);
        jsonDict.SetObjectList<SeasonPrizeIdentifier>("spza", pp.SeasonPrizesAwarded, new SetObjectDelegate<SeasonPrizeIdentifier>(PlayerProfileMapper.SetSeasonPrizeAwarded));
        jsonDict.Set("bctf", CarTimesMapper.ToJson(pp.BestCarTimes));
        jsonDict.Set("frcc", FriendsRewardCollectedMapper.ToJson(pp.FriendsRewardCollectedForCars));
        jsonDict.Set("udid", pp.UDID);
        jsonDict.Set("uuid", pp.UUID);
        jsonDict.SetObjectList<DateTime>("vffs", pp.VideoForFuelTimestamps, new SetObjectDelegate<DateTime>(PlayerProfileMapper.SetVideoForFuelTimestamp));
        jsonDict.SetObject<Dictionary<int, int>>("kfp_", pp.KnownFriendsPrizes, new SetObjectDelegate<Dictionary<int, int>>(PlayerProfileMapper.SetFriendPrizes));
        jsonDict.Set("pf__", pp.PeakFriends);
        jsonDict.Set("ssi_", pp.SyncdServiceID);
        jsonDict.Set("hcpt", pp.HighestCrossPromotionTierAdvertSeen);
        jsonDict.Set("ffrw", pp.FriendsRacesWon);
        jsonDict.Set("ffrl", pp.FriendsRacesLost);
        jsonDict.Set("fcrs", pp.FriendsCarsWon);
        jsonDict.Set("fgld", pp.FriendsGold);
        jsonDict.Set("f2sa", pp.TwoStarTimeConditionShown);
        jsonDict.Set("f3sa", pp.ThreeStarTimeConditionShown);
        jsonDict.Set("ftuc", pp.LastTierUnlockConditionShown);
        jsonDict.Set("fwcp", pp.HasSeenWonAllCarsConditionPopup);
        jsonDict.Set("fccs", RaceWithFriendConditionCombosMapper.ToJson(pp.RaceWithFriendConditionCombos));
        jsonDict.Set("fbyb", pp.BeatYourBestConditionShwonCount);
        jsonDict.Set("ficc", pp.InviteFriendsConditionShownCount);
        jsonDict.Set("frhc", pp.RaceTooHardConditionShownCount);
        jsonDict.Set("fict", pp.InviteFriendsConditionLastTimeShown);
        jsonDict.Set("fbct", pp.BuyACarLastConditionLastTimeShown);
        jsonDict.Set("frft", pp.RaceWithFriendConditionLastTimeShown);
        jsonDict.Set("ferc", pp.FriendsEasyRaceConditionLastTimeShown);
        jsonDict.Set("finf", pp.InitialNetworkFriends);
        jsonDict.Set("finv", pp.FriendsInvited);
        jsonDict.Set("ugas", pp.HasUpgradedFuelTank);
        jsonDict.Set("fgas", pp.HasReceivedFuelTankUpgradeRefill);
        jsonDict.Set("lgas", pp.GasTankReminderLastTimeShown);
        jsonDict.Set("ngas", pp.GasTankReminderNumberOfTimesShown);
        jsonDict.Set("tgas", pp.GasTankReminderIDShown);
        jsonDict.Set("ufet", pp.UnlimitedFuel.Expires);
        jsonDict.Set("ufes", pp.UnlimitedFuel.ExpiresSync);
        jsonDict.Set("ufcm", pp.UnlimitedFuel.CmlMinutesPurchased);
        jsonDict.Set("ufrp", pp.UnlimitedFuel.HasSeenRenewalPopup);
        jsonDict.Set("ufrt", pp.UnlimitedFuel.RaceTeamPopupTimesSeen);
        jsonDict.Set("ftfu", pp.FirstTimeRYFUser);
        jsonDict.Set("wtrc", pp.HasSeenRenewWholeTeamIAPCondition);
        jsonDict.Set("arhc", pp.AllHardRacesConditionLastTimeShown);
        jsonDict.Set("hatv", pp.HasAwardTelegramVisit);
        jsonDict.Set("haiv", pp.HasAwardInstagramVisit);
        pp.CarePackageData.ToJson(ref jsonDict);
        pp.OfferWallData.ToJson(ref jsonDict);
        pp.PinSchedulerData.ToJson(ref jsonDict);
        jsonDict.Set("ogso", pp.OptionGarageSortOrder.ToString());
        jsonDict.Set("orso", pp.OptionReverseSortOrder);
        jsonDict.Set("fapz", pp.FuelAdPromptToggles);
        jsonDict.SetObjectList<DailyBattleCompletionRecord>("udbr", pp.UnconfirmedDailyBattleResults, new SetObjectDelegate<DailyBattleCompletionRecord>(DailyBattleCompletionRecord.WriteToJson));
        jsonDict.Set("dbcc", pp.DailyBattleGoldConfirmed);
        jsonDict.Set("dbgc", pp.DailyBattleCashConfirmed);
        jsonDict.Set("dbrr", pp.DailyBattleRewardsRemoved);
        pp.RestrictionRaceData.ToJson(jsonDict);
        pp.CarDeals.ToJson(jsonDict);
        jsonDict.Set("swti", pp.HasSeenWorldTourIntroduction);
        jsonDict.Set("swtl", pp.SeenWorldTourLockedIntroCount);
        jsonDict.Set("ofwt", pp.OverrideRYFObjectives);
        jsonDict.Set("omwt", pp.OverrideRTWObjectives);
        jsonDict.SetObjectList<WorldTourAnimationsCompleted>("wtas", pp.WorldTourEventIDsWithAnimitionCompleted, new SetObjectDelegate<WorldTourAnimationsCompleted>(WorldTourAnimSeenMapper.WriteToJson));
        jsonDict.Set("wtta", pp.SeasonUnlockableThemesSeen);
        jsonDict.SetObject<WorldTourBoostNitrous>("wtbn", pp.WTBoostNitrousStatus, new SetObjectDelegate<WorldTourBoostNitrous>(WorldTourBoostNitrous.WriteToJson));
        jsonDict.Set("wtdk", new List<string>(pp.WTDeferredNarrativeScenes.Keys));
        jsonDict.Set("wtds", new List<string>(from v in pp.WTDeferredNarrativeScenes.Values
                                              select v.SceneID));
        jsonDict.Set("wtde", new List<string>(from v in pp.WTDeferredNarrativeScenes.Values
                                              select v.SequenceID));
        jsonDict.Set("hvms", pp.HasVisitedMechanicScreen);
        jsonDict.Set("hvds", pp.HasVisitedManufacturerScreen);
        jsonDict.Set("hocr", pp.HasOfferedCrewRacesCars);
        jsonDict.Set("hfrt", pp.HasFinishedRYFTutorial);
        jsonDict.Set("hsfn", pp.HasSeenFacebookNag);
        jsonDict.Set("hsmi", pp.HasSeenMultiplayerIntroScreen);
        jsonDict.Set("hsii", pp.HasSeenInternationalIntroScreen);
        jsonDict.Set("lbts", pp.LastBundleOfferTimeShown);
        jsonDict.Set("wcau", pp.WinCountAfterUpgrade);
        jsonDict.Set("hwrc", pp.HasWeeklyRewardToClaim);
        jsonDict.Set("pwlc", pp.PreviousWeeklyLeaderboardCheck);
        pp.PopupsData.ToJson(ref jsonDict);
        jsonDict.Set("plli", pp.LastPlayedMultiplayerLeaderboardID);
        jsonDict.Set("mps", pp.MapPaneSelected);
        jsonDict.SetObject<Dictionary<string, string>>("wtps", pp.TierXPinSelections, new SetObjectDelegate<Dictionary<string, string>>(PlayerProfileMapper.SetTierXPinSelections));
        pp.TutorialBubblesData.ToJson(ref jsonDict);
        pp.BundleOfferData.ToJson(ref jsonDict);
        pp.MultiplayerEventsData.ToJson(ref jsonDict);
        jsonDict.Set("smpchw", pp.SMPChallengeWins);
        jsonDict.Set("smpcl", pp.SMPConsecutiveLoses);
        jsonDict.Set("smpcw", pp.SMPConsecutiveWins);
        jsonDict.Set("smptr", pp.SMPTotalRaces);
        jsonDict.Set("smptrl", pp.SMPTotalRacesLastSession);
        jsonDict.Set("smpssd", pp.SMPStartSessionDate);
        jsonDict.Set("smpwls", pp.SMPWinsLastSession);
        jsonDict.Set("smpwsid", pp.SMPWinStreakID);
        jsonDict.Set("umau", pp.UseMileAsUnit);
        jsonDict.Set("smpwca", pp.IsSMPWinChallengeAvailable);
        jsonDict.Set("smpact", pp.SMPWinChallengeActivationTime);
        jsonDict.Set("smpbdt", pp.SMPLastLobbyBotDisclaimerTime);
        jsonDict.Set("cutz", pp.CurrentUserTimeZone);
        jsonDict.Set("cucl", pp.CurrentUserChosenLanguage);
        jsonDict.Set("ftpc", pp.FirstPurchaseDone);
        jsonDict.Set("fraw", pp.FirstRewardedAdWatched);
        jsonDict.Set("fiaw", pp.FirstInterstitialAdWatched);
        jsonDict.Set("febr", pp.FirstEnterBeginnerRegulation);
        jsonDict.Set("fenr", pp.FirstEnterNormalRegulation);
        jsonDict.Set("fehr", pp.FirstEnterHardRegulation);
        jsonDict.Set("nrw1", pp.NumberOfRaceToWinCrew1);
        jsonDict.Set("nrw2", pp.NumberOfRaceToWinCrew2);
        jsonDict.Set("nrw3", pp.NumberOfRaceToWinCrew3);
        jsonDict.Set("nrw32", pp.NumberOfRaceToWinCrew32);
        jsonDict.Set("nrw4", pp.NumberOfRaceToWinCrew4);
        jsonDict.Set("nrw42", pp.NumberOfRaceToWinCrew42);
        jsonDict.Set("nrw43", pp.NumberOfRaceToWinCrew43);
        jsonDict.Set("fw1", pp.FirstRace1);
        jsonDict.Set("fw2", pp.FirstRace2);
        jsonDict.Set("fw3", pp.FirstRace3);
        jsonDict.Set("fw32", pp.FirstRace32);
        jsonDict.Set("fw4", pp.FirstRace4);
        jsonDict.Set("fw42", pp.FirstRace41);
        jsonDict.Set("fw43", pp.FirstRace42);
        jsonDict.Set("ifbu", pp.IsFirstBodyUpgrade);
        jsonDict.Set("ifeu", pp.IsFirstEngineUpgrade);
        jsonDict.Set("ifiu", pp.IsFirstIntakeUpgrade);
        jsonDict.Set("ifnu", pp.IsFirstNitrousUpgrade);
        jsonDict.Set("iftu", pp.IsFirstTransmissionUpgrade);
        jsonDict.Set("ifru", pp.IsFirstTurboUpgrade);
        jsonDict.Set("ifyu", pp.IsFirstTyresUpgrade);
        jsonDict.Set("iftfu", pp.IsFirstTapToUpgrade);
        jsonDict.Set("isdb", pp.IsDefaultBranch);
        jsonDict.Set("isab", pp.IsA_Branch);
        jsonDict.Set("isbb", pp.IsB_Branch);
        jsonDict.Set("dbst", pp.DefaultBranch);
        jsonDict.Set("abst", pp.A_Branch);
        jsonDict.Set("bbst", pp.B_Branch);
        
    }

    public static string ToJson(PlayerProfileData pp)
    {
        JsonDict jsonDict = new JsonDict();
        PlayerProfileMapper.SetPlayerProfileData(pp, ref jsonDict);
        return jsonDict.ToString();
    }
}
