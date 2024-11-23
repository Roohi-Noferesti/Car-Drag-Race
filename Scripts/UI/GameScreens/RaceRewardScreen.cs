using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using I2.Loc;
using Metrics;
using Objectives;
using UnityEngine;
using UnityEngine.Events;
using KingKodeStudio;
using AdSystem.Enums;

public class RaceRewardScreen : PostRaceScreen
{
    public static RaceRewardScreen Instance;
    
    public AnimationCurve RaceBarCurve;

    public static bool ScreenFinished;

    private bool haveGivenReward;

    private BubbleMessage tryAgainMessage;

    private bool hasUpdatedProfile;


    //private bool popupShown;

    public RaceRewards_BonusBlock BonusBlockPrefab_Default;

    public GameObject BonusBlockPrefab_RaceWorld;

    public GameObject BonusBlockPrefab_Friends;

    public RaceRewards_BonusBlock BonusBlockPrefab_DailyBattle;

    public GameObject BonusBlockPrefab_Relay;

    private RaceRewards_BonusBlock BonusBlock;

    public GameObject WinLosePrefab_Default;

    public GameObject WinLosePrefab_Multiplayer;

    public GameObject WinLosePrefab_Friends;

    public GameObject WinLosePrefab_Relay;

    private RaceRewards_WinLoseBase WinLoseBlock;

    public GameObject StatsContainerPrefab_Default;

    public GameObject StatsContainerPrefab_Relay;

    private RaceRewards_StatsContainer StatsContainer;

    public GameObject BackGround;

    public GameObject LoadingInfo;


    //private bool animationsArePaused;

    private bool bonusBlockSetup = false;

    private string mpConsumables = string.Empty;


    //private bool rpServerHasFakedResponse = true;

    private float rpServerResponseTimer;


    //private static bool _triggeredSeasonFlowManager;


    //private FlowConditionalBase screenConditionals;

    //private FlowConditionalBase worldTourScreenConditional;

    private Dictionary<int, Events> metricsStreakWin = new Dictionary<int, Events>
    {
        {
            0,
            Events.MPRace1Win
        },
        {
            1,
            Events.MPRace2Win
        },
        {
            2,
            Events.MPRace3Win
        },
        {
            3,
            Events.MPRace4Win
        },
        {
            4,
            Events.MPRace5Win
        },
        {
            5,
            Events.MPRace6Win
        }
    };

    private Dictionary<int, Events> metricsStreakLose = new Dictionary<int, Events>
    {
        {
            0,
            Events.MPRace1Lose
        },
        {
            1,
            Events.MPRace2Lose
        },
        {
            2,
            Events.MPRace3Lose
        },
        {
            3,
            Events.MPRace4Lose
        },
        {
            4,
            Events.MPRace5Lose
        },
        {
            5,
            Events.MPRace6Lose
        }
    };

    private int oldCullingMask;
    public static bool alreadyAwardedWinStreakThisRace;
    private SMPWinStreakReward CurrentWinStreakReward;


    public static bool DoNotUpdate { get; set; }

    public override ScreenID ID
    {
        get { return ScreenID.RaceRewards; }
    }

    protected override void Awake()
    {
        if (Instance != null)
        {
        }

        Instance = this;
        //this.LoadingInfo.SetActive(false);
        //this.screenConditionals = new RaceRewardScreenFlowConditional();
        //this.worldTourScreenConditional = new RaceRewardScreenWorldTourFlowConditional();
        ScreenFinished = false;
        base.Awake();
    }

    private void LogEvent(Events events, Dictionary<Parameters, string> data)
    {
#if !UNITY_EDITOR
        Log.AnEvent(events, data);
#endif
    }
    public void PrepareAdForDefaultInterstitialFlow()
    {
        //   GTAdManager.Instance.TriggerAdInWorkshop = true;

        Debug.Log("UserHasPaidSomethingToDisableAds:" + GTAdManager.Instance.UserHasPaidSomethingToDisableAds());
        if (GTAdManager.Instance.UserHasPaidSomethingToDisableAds())
        {
            return;
        }

        Debug.Log("UseHoursInsteadOfSessions:" + GameDatabase.Instance.AdConfiguration.UseHoursInsteadOfSessions);
        Debug.Log("CheckShouldShowAdsBasedOnSessions:" + GTAdManager.Instance.CheckShouldShowAdsBasedOnSessions());
        if (GameDatabase.Instance.AdConfiguration.UseHoursInsteadOfSessions)
        {

            Debug.Log("CheckSessionShouldShowAd:" + GTAdManager.Instance.CheckSessionShouldShowAd());
            if (!GTAdManager.Instance.CheckSessionShouldShowAd())
            {
                return;
            }
        }
        else if (!GTAdManager.Instance.CheckShouldShowAdsBasedOnSessions())
        {
            return;
        }
        GTAdManager.Instance.AutoShowAd(AdSpace.Default, 3);
    }
    public override void OnCreated(bool zAlreadyOnStack)
    {
        var profile = PlayerProfileManager.Instance.ActiveProfile;
        Dictionary<int, int> eventIDRaceCount = new Dictionary<int, int>
        {
            {4101, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace4101},
            {4102, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace4102},
            {4103, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace4103},
            {41031, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace41031},
            {41041, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace41041},
            {41042, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace41042},
            {4105, PlayerProfileManager.Instance.ActiveProfile.NumberOfRace4105}
        };
        Dictionary<int, Events> eventMapper = new Dictionary<int, Events>
        {
            {4101, Events.Tier1CrewMember1Race},
            {4102, Events.Tier1CrewMember2Race},
            {4103, Events.Tier1CrewMember3Race},
            {41031, Events.Tier1CrewMember3_2Race},
            {41041, Events.Tier1CrewMember4Race},
            {41042, Events.Tier1CrewMember4_2Race},
            {4105, Events.Tier1CrewMember4_3Race}
        };
        Dictionary<int, Events> eventFirstRaceMapper = new Dictionary<int, Events>
        {
            {4101, Events.Tier1CrewMember1FirstRace},
            {4102, Events.Tier1CrewMember2FirstRace},
            {4103, Events.Tier1CrewMember3FirstRace},
            {41031, Events.Tier1CrewMember3_2FirstRace},
            {41041, Events.Tier1CrewMember4FirstRace},
            {41042, Events.Tier1CrewMember4_2FirstRace},
            {4105, Events.Tier1CrewMember4_3FirstRace}
        };
        Dictionary<int, Parameters> parameterMapper = new Dictionary<int, Parameters>
        {
            {4101, Parameters.RaceCountToWinCrewMember1},
            {4102, Parameters.RaceCountToWinCrewMember2},
            {4103, Parameters.RaceCountToWinCrewMember3},
            {41031, Parameters.RaceCountToWinCrewMember32},
            {41041, Parameters.RaceCountToWinCrewMember4},
            {41042, Parameters.RaceCountToWinCrewMember42},
            {4105, Parameters.RaceCountToWinCrewMember43}
        };
        Dictionary<int, Parameters> parameterFirstRaceMapper = new Dictionary<int, Parameters>
        {
            {4101, Parameters.FirstTier1Crew1Race},
            {4102, Parameters.FirstTier1Crew2Race},
            {4103, Parameters.FirstTier1Crew3Race},
            {41031, Parameters.FirstTier1Crew32Race},
            {41041, Parameters.FirstTier1Crew4Race},
            {41042, Parameters.FirstTier1Crew42Race},
            {4105, Parameters.FirstTier1Crew43Race}
        };
        var eventID = RaceEventInfo.Instance.CurrentEvent.EventID;
        
        if (eventIDRaceCount.ContainsKey(eventID))
        {
            
            if (!RaceResultsTracker.You.IsWinner)
            {
                PlayerProfileManager.Instance.ActiveProfile.AddEventCompletedCount(eventID);
                PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                if (!PlayerProfileManager.Instance.ActiveProfile.GetEventFirstRace(eventID))
                {
                    Dictionary<Parameters, string> dataFirst = new Dictionary<Parameters, string>
                    {
                        {parameterFirstRaceMapper[eventID], "1"}
                    };
                    LogEvent(eventFirstRaceMapper[eventID], dataFirst);
                    PlayerProfileManager.Instance.ActiveProfile.AddEventFirstRace(eventID);
                    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                }
                
            }
            else
            {
                if (!PlayerProfileManager.Instance.ActiveProfile.GetEventFirstRace(eventID))
                {
                    Dictionary<Parameters, string> dataFirst = new Dictionary<Parameters, string>
                    {
                        {parameterFirstRaceMapper[eventID], "1"}
                    };
                    PlayerProfileManager.Instance.ActiveProfile.AddEventFirstRace(eventID);
                    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                    LogEvent(eventFirstRaceMapper[eventID], dataFirst);
                }
                Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
                {
                    {parameterMapper[eventID], (PlayerProfileManager.Instance.ActiveProfile.AddEventCompletedCount(eventID)).ToString()}
                };
                PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                LogEvent(eventMapper[eventID], data);
            }
            
        }
        
        ShownResults = RaceResultsTracker.GetState();
        //_triggeredSeasonFlowManager = false;
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            UseFakeRaceData = true;
        }

        if (ShownResults.You == null)
        {
            ShownResults.You = new RaceResultsData();
            ShownResults.Them = new RaceResultsData();
            ShownResults.Best = new RaceResultsData();
        }

        GameObject original = BonusBlockPrefab_Default.gameObject;
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
        {
            original = BonusBlockPrefab_RaceWorld;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
        {
            original = BonusBlockPrefab_Friends;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsDailyBattle())
        {
            original = BonusBlockPrefab_DailyBattle.gameObject;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
        {
            original = BonusBlockPrefab_Relay;
        }

        BonusBlock = RaceEventInfo.Instance.CurrentEvent.IsDailyBattle()
            ? BonusBlockPrefab_DailyBattle
            : BonusBlockPrefab_Default; // original.GetComponent<RaceRewards_BonusBlock>();
        //this.BonusBlock.transform.parent = this.BonusBlockHolder.gameObject.transform;
        original = WinLosePrefab_Default;
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
        {
            original = WinLosePrefab_Multiplayer;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
        {
            original = WinLosePrefab_Friends;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
        {
            original = WinLosePrefab_Relay;
        }

        WinLoseBlock = original.GetComponent<RaceRewards_WinLoseBase>();
        WinLoseBlock.OnAnimationEnd = () => { StartCoroutine(PlayBonusAnimations()); };
        original = StatsContainerPrefab_Default;
        if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
        {
            original = StatsContainerPrefab_Relay;
        }

        StatsContainer = original.GetComponent<RaceRewards_StatsContainer>();
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
        {
        }

        //this.LoadingInfo.SetActive(false);
        //this.animationsArePaused = false;
        StatsContainer.Setup(ShownResults, UseFakeRaceData);
        if (!zAlreadyOnStack)
        {
            if (!IngameTutorial.IsInTutorial)
            {
                PrepareAdForDefaultInterstitialFlow();
            }

            haveGivenReward = false;
            WinLoseBlock.Setup(ShownResults);
            MenuAudio.Instance.fadeMusic(0.4f, 0.6f);
            if (!DoNotUpdate)
            {
                if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
                {
                    //AnimationUtils.PlayAnim(base.animation, "Rewards_ScreenFlow_Relay");
                }
                else
                {
                    //AnimationUtils.PlayAnim(base.animation, "Rewards_ScreenFlow");
                }
            }

            ScreenFinished = false;
            //if (PhilsFlag.Instance != null)
            //{
            //    PhilsFlag.Instance.Hide();
            //}
            if (RaceEventInfo.Instance.ShouldCurrentEventUseConsumables())
            {
                PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                mpConsumables = string.Concat(new string[]
                {
                    "C",
                    (!activeProfile.IsConsumableActive(eCarConsumables.EngineTune)) ? "0" : "1",
                    (!activeProfile.IsConsumableActive(eCarConsumables.PRAgent)) ? "0" : "1",
                    (!activeProfile.IsConsumableActive(eCarConsumables.Nitrous)) ? "0" : "1",
                    (!activeProfile.IsConsumableActive(eCarConsumables.Tyre)) ? "0" : "1"
                });
            }

            PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
            ScheduledPin worldTourPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
            if (worldTourPinInfo != null)
            {
                IGameState gameState = new GameStateFacade();
                string currentThemeName = TierXManager.Instance.CurrentThemeName;
                bool isWinner = RaceResultsTracker.You.IsWinner;
                gameState.SetPinToRaced(currentThemeName, worldTourPinInfo, isWinner);
                if (isWinner && worldTourPinInfo.ParentSequence.TypeEnum == PinSequence.eSequenceType.Choice)
                {
                    ScheduledPin referrerPin = worldTourPinInfo.ReferrerPin;
                    List<ScheduledPin> eligiblePins = referrerPin.ChoiceScreen.GetEligiblePins(gameState, referrerPin);
                    IEnumerable<string> source = from pin in eligiblePins
                        select ChoicePinStateToMetricsString(pin, worldTourPinInfo, gameState);
                    string value = "[" + string.Join(",", source.ToArray<string>()) + "]";
                    Log.AnEvent(Events.CarPicked, new Dictionary<Parameters, string>
                    {
                        {
                            Parameters.Plc,
                            referrerPin.ChoiceScreen.SequenceID
                        },
                        {
                            Parameters.CChc,
                            value
                        }
                    });
                }
            }

            if (!RaceEventInfo.Instance.CurrentEvent.IsRelay)
            {
                PlayerProfileManager.Instance.ActiveProfile.RaceCompleted(RaceResultsTracker.You,
                    RaceResultsTracker.Them);
            }

            //if (!IngameTutorial.IsInTutorial && ShownResults.Them != null)
            //{
            //    AchievementChecks.CheckForRaceEndAchievementsAndLeaderboards();
            //}
        }
        else
        {
            if (!RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
            {
                //this.popupShown = true;
            }

            hasUpdatedProfile = true;
            haveGivenReward = true;
            Hide(false);
            WinLoseBlock.Setup(ShownResults);
            BonusBlock.SetPlayerWon(ShownResults.You.IsWinner);
            BonusBlock.Finish();
            ScreenFinished = true;
        }

        if (!UseFakeRaceData)
        {
            ResetNags();
            //this.oldCullingMask = Camera.main.cullingMask;
            //Camera.main.cullingMask = 0;
        }

        if (!zAlreadyOnStack)
        {
            bonusBlockSetup = false;
            rpServerResponseTimer = 0f;
            //this.rpServerHasFakedResponse = false;
        }

        if (!zAlreadyOnStack && RaceEventInfo.Instance.CurrentEvent != null && RaceResultsTracker.You.IsWinner &&
            RaceEventInfo.Instance.IsCrewRaceEvent && RaceEventInfo.Instance.IsFirstCrewRaceEvent())
        {
            PlayerProfileManager.Instance.ActiveProfile.ShouldShowSkipTo2ndCrewMemberPopup = true;
        }

        var onePlayerResultsOnly = RaceResultsTracker.Them == null;
        if (!IngameTutorial.IsInTutorial && !onePlayerResultsOnly)
        {
            var didDisconnectSMPRace = ((RaceEventInfo.Instance.CurrentEvent != null) &&
                                        RaceEventInfo.Instance.CurrentEvent
                                            .IsSMPRaceEvent()); // && SMPNetworkManager.Instance.SMPYouLeftRace;
            if (!didDisconnectSMPRace)
            {
                AchievementChecks.CheckForRaceEndAchievementsAndLeaderboards();
            }
        }

        if ((RaceEventInfo.Instance.CurrentEvent != null) && RaceEventInfo.Instance.CurrentEvent.IsSMPRaceEvent()
                                                          && !zAlreadyOnStack)
        {
            //var voidedReason = "opponent_lost_connection";
            if (SMPNetworkManager.Instance.SMPYouLeftRace)
            {
                //voidedReason = "disconnect";
                ShowInvalidSMPRacePopup("TEXT_SMP_RACE_YOU_LEFT_HEADER", "TEXT_SMP_RACE_YOU_LEFT_BODY",
                    "TEXT_BUTTON_OK", OnInvalidSMPOK);
            }
            else if (SMPNetworkManager.Instance.SMPOpponentLeftRace)
            {
                //voidedReason = "opponent_disconnect";
                ShowInvalidSMPRacePopup("TEXT_SMP_RACE_OPPONENT_LEFT_HEADER", "TEXT_SMP_RACE_OPPONENT_LEFT_BODY",
                    "TEXT_BUTTON_OK", OnInvalidSMPOK);
            }

            if (SMPNetworkManager.Instance.SMPRaceInvalidated)
            {
                //if (SMPNetworkManager.Instance.SMPRaceDiscrepancy)
                //{
                //    voidedReason = "results_discrepancy";
                //}
                ShowInvalidSMPRacePopup("TEXT_SMP_RACE_INVALID_HEADER", "TEXT_SMP_RACE_INVALID_BODY", "TEXT_BUTTON_OK",
                    OnInvalidSMPOK);
                var FuelCost = 1; //RaceEventInfo.Instance.CurrentEvent.FuelCost;
                FuelManager.Instance.AddFuel(FuelCost, FuelReplenishTimeUpdateAction.UPDATE,
                    FuelAnimationLockAction.OBEY);

                //Give back stake to player
                var currentEvent = RaceEventInfo.Instance.CurrentEvent.EventOrder;
                var carDbKey = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey;
                var tier = CarDatabase.Instance.GetCar(carDbKey).BaseCarTier;
                var stake = GameDatabase.Instance.Online.GetStake(currentEvent, tier);
                PlayerProfileManager.Instance.ActiveProfile.AddCash(stake,"stake","stake");
                //ZTrackMetricsHelper.LogSMPRaceVoided(voidedReason, RaceEventInfo.Instance);
            }
        }

        if ((RaceEventInfo.Instance.IsSMPEvent && !zAlreadyOnStack)
        ) // && (!SMPNetworkManager.Instance.SMPRaceInvalidated && !TutorialQuery.IsStoryActive("PlayerOnline")))
        {
            var pp = PlayerProfileManager.Instance.ActiveProfile;
            pp.SMPConsecutiveRaces++;
            if (RaceResultsTracker.You.IsWinner)
            {
                pp.SMPWins++;
                pp.SMPWinsLastSession++;

                if ((ServerSynchronisedTime.Instance.GetDateTime() - pp.SMPStartSessionDate).TotalHours >
                    SMPConfigManager.SMPSessionHoursToReset)
                {
                    GTDebug.Log(GTLogChannel.AutoDifficulty,
                        "SMP Total Win Reset Here : startSession : " + pp.SMPStartSessionDate + "   now:" +
                        ServerSynchronisedTime.Instance.GetDateTime() + "  reset after : " +
                        SMPConfigManager.SMPSessionHoursToReset);
                    pp.SMPStartSessionDate = ServerSynchronisedTime.Instance.GetDateTime();
                    pp.SMPWinsLastSession = 0;
                    pp.SMPTotalRacesLastSession = 0;
                }

                if (pp.SMPChallengeWins == 0)
                {
                    pp.SMPWinStreakID = new MetricsTrackingID().ToString();
                }

                if (pp.IsSMPWinChallengeAvailable)
                {
                    pp.SMPChallengeWins++;
                    if (pp.SMPChallengeWins == 1)
                    {
                        pp.SMPWinChallengeActivationTime =
                            ServerSynchronisedTime.Instance.GetDateTime(); //To be added next version
                    }

                    if ((pp.SMPChallengeWins == SMPConfigManager.WinStreak.SMPWinStreakMaxValue) &&
                        !pp.IsNextSMPChallengeAvailable())
                    {
                        var remainingTime = pp.GetNextSMPWinChallengeRemainingTime();
                        //NotificationManager.Active.UpdateSMPWinStreakReminder(remainingTime);
                    }
                }
                else if (pp.SMPChallengeWins >= SMPConfigManager.WinStreak.SMPWinStreakMaxValue)
                {
                    pp.ResetSMPChallengeWins();
                }

                pp.SMPConsecutiveLoses = 0;
                pp.SMPConsecutiveWins++;
                pp.LastWinStreakExtendedTime = ServerSynchronisedTime.GetAbsoluteTimeNow(false);
            }
            else
            {
                pp.SMPLosses++;
                pp.SMPConsecutiveLoses++;
                pp.SMPConsecutiveWins = 0;
            }

            if (RaceResultsTracker.You.IsWinner)
            {
                UpdateWinStreakPrizeSMP();
            }

            //var currentCar = pp.GetCurrentCar();
            //if ((currentCar != null) && pp.CarSMPMatchMakingData.ContainsKey(currentCar.UniqueID))
            //{
            //    var yourRaceTime = RaceResultsTracker.You.RaceTime;
            //    var defaultRaceTime = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.TestTimeTuned;
            //    var EPIndex = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.EPIndex;
            //    var raceDelta = defaultRaceTime - yourRaceTime;
            //    var carMMData = pp.CarSMPMatchMakingData[currentCar.UniqueID];
            //    carMMData.UpdateAverageTimeDeltaAfterRace(raceDelta, defaultRaceTime, currentCar.CarTier);
            //    carMMData.UpdateLobbySwitchParameters(yourRaceTime, defaultRaceTime, EPIndex, currentCar.CarTier);
            //    //ZTrackMetricsHelper.LogSMPRaceERT(RaceEventInfo.Instance, carMMData);
            //    //ZTrackMetricsHelper.LogSMPRacePlayerSkill(RaceEventInfo.Instance, carMMData);
            //}
            //SMPRematchPaneController.Instance.ResetController();
        }

        //IGameState gameState2 = new GameStateFacade();
        //if (CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey).UsesEvoUpgrades())
        //{
        //    ListItem item = base.CarouselList.GetItem(3);
        //    item.GreyOutThisItem(true);
        //    if (gameState2.GetCurrentCarEvoPartsEarned(0) < 1)
        //    {
        //        item.GreyedOutTap += new TapEventHandler(this.ShowEvoNudgePopup);
        //    }
        //}
        //this.BackGround.SetActive(false);
        //if (!ScreenFinished)
        //{
        //    this.StatsContainer.gameObject.SetActive(false);
        //}
        GameDatabase.Instance.BundleOffers.UpdateOffers();
        GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null);
        DoNotUpdate = false;
        
        base.OnCreated(zAlreadyOnStack);
        SendMetricsEndOfOnlineRacesEvent();
    }

    private string ChoicePinStateToMetricsString(ScheduledPin pin, ScheduledPin currentPin, IGameState gameState)
    {
        PinDetail pin2 = TierXManager.Instance.GetPin(pin);
        RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(pin2.EventID);
        string str = "A";
        if (pin == currentPin)
        {
            str = "C";
        }
        else if (gameState.GetWorldTourRaceResultCount(TierXManager.Instance.CurrentThemeName, pin.ParentSequence.ID,
                     pin.ID, true) > 0)
        {
            str = "D";
        }
        else if (WorldTourChoiceItem.IsRestrictedChoice(eventByEventIndex))
        {
            str = "L";
        }

        CarInfo car = CarDatabase.Instance.GetCar(eventByEventIndex.AICar);
        return car.Key + ":" + str;
    }

    private void OnInvalidSMPOK()
    {
    }


    private void ShowInvalidSMPRacePopup(string SMPTitle, string SMPBodyText, string SMPButtonText,
        PopUpButtonAction ButtonCallback)
    {
        var popup = new PopUp()
        {
            Title = SMPTitle,
            BodyText = SMPBodyText,
            ConfirmText = SMPButtonText,
            ConfirmAction = ButtonCallback
        };
        PopUpManager.Instance.TryShowPopUp(popup);
    }



    private void FireMPTutorialMetricsEvent(bool playerWins)
    {
        int num = StreakManager.CurrentStreak();
        if (num > 5)
        {
            return;
        }

        Events theEvent = (!playerWins) ? metricsStreakLose[num] : metricsStreakWin[num];
        Log.AnEvent(theEvent);
    }

    private void SetupBonusBlock()
    {
        BonusBlock.Setup(ShownResults);
        string text = string.Format(LocalizationManager.GetTranslation("TEXT_BUTTON_BONUS_RACE_REWARD"), CurrencyUtils.GetCashString(GetbonusRaceReward()));
        ExtraRewardButton.SetText(text, true, false);
        UpdateProfile();
        bonusBlockSetup = true;
    }

    protected override void Update()
    {
        base.Update();
        if (DoNotUpdate)
        {
            return;
        }

        rpServerResponseTimer += Time.deltaTime;
        if (!bonusBlockSetup)
        {
            SetupBonusBlock();
            if (!UseFakeRaceData)
            {
                FirstCrewNag();
            }
        }
    }

    private IEnumerator PlayBonusAnimations()
    {
        BonusBlock.Anim_Bonus_1();
        yield return new WaitForSeconds(0.3F);
        BonusBlock.Anim_Bonus_2();
        yield return new WaitForSeconds(0.3F);
        BonusBlock.Anim_Bonus_3();
        if (RemoteConfigABTest.CheckRemoteConfigValue())
        {
            yield return new WaitForSeconds(0.3f);
            BonusBlock.Anim_Bonus_5();
        }
        yield return new WaitForSeconds(0.3F);
        BonusBlock.Anim_Bonus_4();
        if (BonusBlock.TotalGold > 0)
        {
            yield return new WaitForSeconds(0.3F);
            BonusBlock.Anim_Totals_Final();
        }

        yield return new WaitForSeconds(0.3F);
        CommonUI.Instance.FuelStats.DoFuelSpendGlow();
        yield return new WaitForSeconds(1);
        FinishScreen(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TriggerRaceReward();
        CurrentWinStreakReward = null;
        MenuAudio.Instance.fadeMusic(1f, 0.6f);
        //Camera.main.cullingMask = this.oldCullingMask;
        //if (!XPStats.DetectedLevelUp)
        //{
        //    this.CollectFriendsRewardCar();
        //}
    }

    public void ResetNags()
    {
        if (ShownResults.You.IsWinner)
        {
            PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLosses = 0;
            PlayerProfileManager.Instance.ActiveProfile.ContiguousLosses = 0;
            RaceReCommon.RepeatedEventLoseCount = 0;
            RaceReCommon.RepeatedEventId = -1;
        }
    }

    private void DismissRYFTryAgainMessage()
    {
        if (tryAgainMessage != null)
        {
            tryAgainMessage.Dismiss();
            tryAgainMessage = null;
        }
    }

    private void FirstCrewNag()
    {
        if (RaceEventInfo.Instance.CurrentEvent != null && ShownResults.You.IsWinner &&
            !PopUpManager.Instance.isShowingPopUp && RaceEventInfo.Instance.CurrentEvent.IsFirstCrewMemberRace())
        {
            RateTheAppNagger.TryShowPrompt(RateTheAppTrigger.BEATCREWMEMBER);
        }
    }

    private void Hide(bool hide)
    {
        WinLoseBlock.Hide(hide);
        BonusBlock.Hide(hide);
    }

    private void UpdateProfile()
    {
        if (hasUpdatedProfile)
        {
            return;
        }

        hasUpdatedProfile = true;
        CommonUI.Instance.CashStats.CashLockedState(true);
        CommonUI.Instance.CashStats.GoldLockedState(true);
        CommonUI.Instance.XPStats.XPLockedState(true);
        CommonUI.Instance.XPStats.LevelUpLockedState(true);
        CommonUI.Instance.StarLeagueStats.StarLockedState(true);
        CommonUI.Instance.StarStats.StarLockedState(true);
        CommonUI.Instance.StarStats.NewLeagueLockedState(true);
        CommonUI.Instance.FuelStats.FuelLockedState(true);

        //CommonUI.Instance.RPStats.RPLockedState(true);
        if (RelayManager.IsCurrentEventRelay())
        {
            SendMetricsEndOfRelayEvent();
        }
        else
        {
            SendMetricsEndOfRaceEvent();
        }

        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (RaceEventInfo.Instance.IsDailyBattleEvent)
        {
            DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(
                PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount,
                RaceEventQuery.Instance.getHighestUnlockedClass(), RaceResultsTracker.You.IsWinner);
            reward.ApplyToProfile(PlayerProfileManager.Instance.ActiveProfile);
            PlayerProfileManager.Instance.ActiveProfile.LogDailyBattleReward(
                PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount,
                RaceResultsTracker.You.IsWinner);
            int totalCash = BonusBlock.TotalCash;
            int totalGold = BonusBlock.TotalGold;
            if (reward.RewardType == DailyBattleRewardType.Cash)
            {
                totalCash -= reward.RewardValue;
            }
            else if (reward.RewardType == DailyBattleRewardType.Gold)
            {
                totalGold -= reward.RewardValue;
            }

            activeProfile.RaceReward(totalCash, totalGold, BonusBlock.TotalXP, BonusBlock.TotalStar);
        }
        else
        {
            activeProfile.RaceReward(BonusBlock.TotalCash, BonusBlock.TotalGold, BonusBlock.TotalXP
                , BonusBlock.TotalStar);
        }

        var raceResult = RaceResultsTracker.You;
        var isDailyBattle = RaceEventInfo.Instance.IsDailyBattleEvent;
        //ClientConnectionManager.EndRace(activeProfile.ID, BonusBlock.TotalStar, activeProfile.GetPlayerLevel(),
        //    RaceEventInfo.Instance.CurrentEvent.IsHalfMile, !isDailyBattle ? raceResult.RaceTime : 0, raceResult.Nought100TimeKPH
        //    ,activeProfile.CurrentlySelectedCarDBKey);

        if (UserManager.Instance.isLoggedIn)
        {
            LegacyLeaderboardManager.Instance.SubmitRaceReward(UserManager.Instance.currentAccount.Username,
                BonusBlock.TotalStar, activeProfile.GetPlayerLevel()
                , RaceEventInfo.Instance.CurrentEvent.IsHalfMile, !isDailyBattle ? raceResult.RaceTime : 0,
                raceResult.Nought100TimeKPH, activeProfile.CurrentlySelectedCarDBKey);
        }

        //PPEManager.CheckEndOfRaceEngagements();
        RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        if (currentEvent.IsHighStakesEvent())
        {
            BoostNitrous.DoRaceReward(RaceResultsTracker.You.IsWinner, currentEvent);
        }

        ObjectiveCommand.Execute(new FinishdRaceWithEventInfo(RaceEventInfo.Instance, RaceResultsTracker.You.IsWinner),
            true);
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }

    private void SendMetricsEndOfRelayEvent()
    {
        float num = 0f;
        for (int i = 0; i < RelayManager.GetRaceCount(); i++)
        {
            num += RelayManager.GetHumanRaceTime(i);
        }

        Log.AnEvent(Events.RelayRaceComplete, new Dictionary<Parameters, string>
        {
            {
                Parameters.DCsh,
                BonusBlock.TotalCash.ToString()
            },
            {
                Parameters.DGld,
                BonusBlock.TotalGold.ToString()
            },
            {
                Parameters.DXp,
                BonusBlock.TotalXP.ToString()
            },
            {
                Parameters.PFin,
                num.ToString()
            },
            {
                Parameters.CmlRly,
                PlayerProfileManager.Instance.ActiveProfile.NumberOfRelaysCompetetedIn.ToString()
            }
        });
        PlayerProfileManager.Instance.ActiveProfile.NumberOfRelaysCompetetedIn++;
        PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
    }

    private void SendMetricsEndOfRaceEvent()
    {
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
        {
            return;
        }

        Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
        {
            {
                Parameters.DCsh,
                BonusBlock.TotalCash.ToString()
            },
            {
                Parameters.DGld,
                BonusBlock.TotalGold.ToString()
            },
            {
                Parameters.DXp,
                BonusBlock.TotalXP.ToString()
            },
            {
                Parameters.Dfuel,
                GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent).ToString()
            },
            {
                Parameters.EventType,RaceEventInfo.Instance.CurrentEvent.EventName
            },
            {
                Parameters.IsWon, RaceResultsTracker.You.IsWinner.ToString()
            }
        };
        if (RaceEventInfo.Instance.CurrentEvent.IsLocalCarLoaned())
        {
            RacePlayerInfoComponent component = CompetitorManager.Instance.LocalCompetitor.PlayerInfo
                .GetComponent<RacePlayerInfoComponent>();
            CarUpgradeSetup localPlayerCarUpgradeSetup = RaceEventInfo.Instance.LocalPlayerCarUpgradeSetup;
            dictionary.Add(Parameters.Eng,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted.ToString());
            dictionary.Add(Parameters.Trb,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted.ToString());
            dictionary.Add(Parameters.Intk,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted.ToString());
            dictionary.Add(Parameters.Bdy,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted.ToString());
            dictionary.Add(Parameters.Trns,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted.ToString());
            dictionary.Add(Parameters.Trs,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted.ToString());
            dictionary.Add(Parameters.Nit,
                localPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted.ToString());
            dictionary.Add(Parameters.PCr, localPlayerCarUpgradeSetup.CarDBKey);
            dictionary.Add(Parameters.PCrPP, component.PPIndex.ToString());
            dictionary.Add(Parameters.OpCr, localPlayerCarUpgradeSetup.CarDBKey);
            dictionary.Add(Parameters.OpCrPP, component.PPIndex.ToString());
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
        {
            string carDBKey = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey;
            StarStats myStarStats = StarsManager.GetMyStarStats();
            int starForTime = (int) StarsManager.GetStarForTime(carDBKey, RaceResultsTracker.You.PrevBestForFriendsCar);
            int starForTime2 = (int) StarsManager.GetStarForTime(carDBKey, RaceResultsTracker.You.RaceTime);
            int num = Mathf.Max(starForTime2 - starForTime, 0);
            int num2 = 0;
            myStarStats.NumStars.TryGetValue(StarType.GOLD, out num2);
            num2 -= ((starForTime2 != 3 || num <= 0) ? 0 : 1);
            dictionary.Add(Parameters.CmlStars, (myStarStats.TotalStars - num).ToString());
            dictionary.Add(Parameters.CarStars, starForTime.ToString());
            dictionary.Add(Parameters.StarsWon, num.ToString());
            dictionary.Add(Parameters.ThreeStarCars, num2.ToString());
            dictionary.Add(Parameters.Friends, LumpManager.Instance.FriendLumps.Count.ToString());
            int leaderboardPositionForTimeInCar =
                LumpManager.Instance.GetLeaderboardPositionForTimeInCar(carDBKey,
                    RaceResultsTracker.You.PrevBestForFriendsCar);
            int
                num3 = 0; //LumpManager.Instance.GetLeaderboardPositionForTimeInCar(carDBKey, RaceResultsTracker.You.RaceTime) - leaderboardPositionForTimeInCar;
            int
                num4 = 0; //LumpManager.Instance.FriendLumps.Values.Count((CachedFriendRaceData q) => q.FriendHasCarTime(carDBKey));
            dictionary.Add(Parameters.LPos, leaderboardPositionForTimeInCar.ToString());
            dictionary.Add(Parameters.DLPos, num3.ToString());
            dictionary.Add(Parameters.FriendsInCar, num4.ToString());
            if (CompetitorManager.Instance.OtherCompetitor.PlayerInfo.IsStar)
            {
                dictionary.Add(Parameters.FriendRaced,
                    CompetitorManager.Instance.OtherCompetitor.PlayerInfo.CsrUserName);
            }
            else if (CompetitorManager.Instance.OtherCompetitor.PlayerInfo.CsrUserName.Equals(CompetitorManager.Instance
                .LocalCompetitor.PlayerInfo.CsrUserName))
            {
                dictionary.Add(Parameters.FriendRaced, "PB");
            }
            else
            {
                dictionary.Add(Parameters.FriendRaced,
                    CompetitorManager.Instance.OtherCompetitor.PlayerInfo.CsrUserName);
            }
        }

        Log.AnEvent(Events.CompleteRace, dictionary);
    }

    private void SendMetricsEndOfOnlineRacesEvent()
    {
        if (RaceEventInfo.Instance.CurrentEvent.IsSMPRaceEvent())
        {
            Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
            {
                {Parameters.OnlineRaceNumber, PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins.ToString()},
                {Parameters.OnlineRaceLosesCount, PlayerProfileManager.Instance.ActiveProfile.SMPLosses.ToString()}
            };
            Log.AnEvent(Events.OnlineRace, dictionary);
        }
    }

    public override void OnScreenClick()
    {
        base.OnScreenClick();
        FinishScreen(true);
    }
    
    public void OnGetExtraRewardClick()
    {
        if (!IsButtonValid())
            return;
        
        VideoForRewardsManager.Instance.SetExtraRewardResult(new VideoForRewardsManager.ExtraRewardResult()
        {
            ActionOnVideoFail = () =>
            {
                ExtraRewardButton.CurrentState = BaseRuntimeControl.State.Disabled;
            },
            ActionOnVideoOfferReject = () => {},
            ActionOnVideoSuccess = () =>
            {
                PlayerProfileManager.Instance.ActiveProfile.RaceReward(GetbonusRaceReward(), 0, 0, 0);
                ExtraRewardButton.CurrentState = BaseRuntimeControl.State.Disabled;
            },
            VideoFailRewardText = "",
            VideoSuccessRewardText = CurrencyUtils.GetCashString(GetbonusRaceReward())
        });
        VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize);
    }

    private int GetbonusRaceReward()
    {
        VideoForRewardConfiguration configuration = GameDatabase.Instance.Ad.GetConfiguration(VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize);
        return VideoForRewardsManager.GetRewardAmount(configuration, RaceEventInfo.Instance.CurrentEventTier);
    }

    public override void FinishScreen(bool finishAnims)
    {
        base.FinishScreen(finishAnims);
        if (!ScreenFinished)
        {
            if (finishAnims)
            {
                Hide(false);
                StatsContainer.gameObject.SetActive(true);
                BonusBlock.Bonus_2_Content.SetActive(true);
                if (!string.IsNullOrEmpty(BonusBlock.Bonus_3_Prize.text))
                {
                    BonusBlock.Bonus_3_Content.SetActive(true);
                }

                if (BonusBlock.Bonus_4_Content != null)
                {
                    BonusBlock.Bonus_4_Content.SetActive(true);
                }

                if (BonusBlock.Bonus_5_Content != null)
                {
                    BonusBlock.Bonus_5_Content.SetActive(true);
                }

                if (BonusBlock.Bonus_Gold != null)
                {
                    //BonusBlock.Bonus_Gold.SetActive(true);
                }

                BonusBlock.Finish();
            }

            TriggerRaceReward();
            MenuAudio.Instance.fadeMusic(1f, 0.6f);
            ScreenFinished = true;
        }
    }


    private void UpdateWinStreakPrizeSMP()
    {
        if (!alreadyAwardedWinStreakThisRace)
        {
            CurrentWinStreakReward = null;
            int sMPWinStreakMaxValue = 9;
            if (SMPConfigManager.WinStreak.SMPWinStreakMaxValue > 0)
            {
                sMPWinStreakMaxValue = SMPConfigManager.WinStreak.SMPWinStreakMaxValue;
            }

            int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
            int numWins = 0;
            if (SMPConfigManager.WinStreak.SMPWinStreakData != null)
            {
                foreach (SMPWinStreakReward reward in SMPConfigManager.WinStreak.SMPWinStreakData)
                {
                    if (reward.StreakCount > numWins)
                    {
                        numWins = reward.StreakCount;
                    }

                    if (((sMPChallengeWins > 0) && (sMPChallengeWins <= sMPWinStreakMaxValue)) &&
                        (sMPChallengeWins == reward.StreakCount))
                    {
                        CurrentWinStreakReward = reward;
                        if (CurrentWinStreakReward.WinReward.rewardType == ERewardType.RP)
                        {
                            //this.localRPResult.rpDelta += this.CurrentWinStreakReward.Amount;
                        }

                        break;
                    }
                }
            }

            if (((CurrentWinStreakReward != null) &&
                 (CurrentWinStreakReward.WinReward.rewardType == ERewardType.Invalid)) &&
                ((CurrentWinStreakReward.WinReward.name.ToLower() == "achievement") &&
                 (CurrentWinStreakReward.AchievementIndex != -1)))
            {
                //AchievementChecks.ReportSMPWinStreakAchievement(this.CurrentWinStreakReward.AchievementIndex);
                CurrentWinStreakReward = null;
            }

            if ((numWins < sMPChallengeWins) && (PlayerProfileManager.Instance.ActiveProfile.SMPWinStreakID != null))
            {
                //ZTrackMetricsHelper.LogSMPWinStreakEnded(PlayerProfileManager.Instance.ActiveProfile.SMPWinStreakID, numWins, true);
                PlayerProfileManager.Instance.ActiveProfile.SMPWinStreakID = null;
            }
        }
    }

    private void TriggerRaceReward()
    {
        bool lockStat = false;
        if (!haveGivenReward)
        {
            if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
            {
                int deltaRP = NetworkReplayManager.Instance.Response.deltaRP;
                Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
                {
                    {
                        Parameters.RPDelta,
                        deltaRP.ToString()
                    },
                    {
                        Parameters.Consumables,
                        mpConsumables
                    },
                    {
                        Parameters.StreakType, ""
                        //MultiplayerUtils.GetMultiplayerStreakType()
                    }
                };
                //if (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.EVENT)
                //{
                //    MultiplayerEventData data = MultiplayerEvent.Saved.Data;
                //    if (data != null)
                //    {
                //        dictionary.Add(Parameters.MPEvtID, data.ID.ToString());
                //        dictionary.Add(Parameters.SsnAwded, SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID().ToString());
                //        dictionary.Add(Parameters.LeadPos, NetworkReplayManager.Instance.Response.currentSeasonRankPercentile.ToString());
                //    }
                //}
                Log.AnEvent(Events.MultiplayerRaceComplete, dictionary);
                Log.AnEvent(Events.MultiplayerRaceComplete, dictionary);
            }

            if ((RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldEvent() ||
                 RaceEventInfo.Instance.CurrentEvent.IsOnlineClubRacingEvent()) &&
                !PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
            {
                FireMPTutorialMetricsEvent(RaceResultsTracker.You.IsWinner);
            }

            if (RaceResultsTracker.You != null && RaceResultsTracker.You.IsWinner)
            {
                if (RaceEventInfo.Instance.IsSMPEvent)
                {
                    lockStat = true;
                    ShowPopupforSMPWinStreakReward(CurrentWinStreakReward);
                    //metrics.StreakReward = this.CurrentWinStreakReward;
                }
                else if (RaceEventInfo.Instance.IsDailyBattleEvent)
                {
                    //if (DailyBattleRewardSystem.Instance != null)
                    //{
                    //    DailyBattleReward dBReward = DailyBattleRewardSystem.Instance.GetReward();
                    //    if (dBReward != null)
                    //    {
                    //        metrics.DBReward = dBReward;
                    //        this.ShowPopupforDailyBattleReward(dBReward);
                    //    }
                    //}
                }
                else if (RaceEventInfo.Instance.CurrentEvent != null)
                {
                    //if (!RaceEventInfo.Instance.CurrentEvent.IsCrewEvent() && (RaceEventInfo.Instance.CurrentEvent.GetEventPrizes() != null))
                    //{
                    //    int count = 0;
                    //    if (RaceEventInfo.Instance.CurrentEvent.EventMode == ERaceEventMode.ManufacturerLadder)
                    //    {
                    //        count = PlayerProfileManager.Instance.ActiveProfile.ManufacturerLadderRacesCompleted.Count;
                    //    }
                    //    else if (RaceEventInfo.Instance.CurrentEvent.IsECBEvent() || RaceEventInfo.Instance.CurrentEvent.IsFranchiseLadderEvent())
                    //    {
                    //        count = RaceEventInfo.Instance.CurrentEvent.Group.NumEventsComplete();
                    //    }
                    //    else
                    //    {
                    //        PlayerProfileManager.Instance.ActiveProfile.EventsCompleted.TryGetValue(RaceEventInfo.Instance.CurrentEvent.EventID, out count);
                    //    }
                    //    if (this.applyableRewardResults.Count > 0)
                    //    {
                    //        this.m_RewardPopupsToClear = this.applyableRewardResults.Count;
                    //        this.ShowPopUpForEventReward(this.applyableRewardResults[0], RaceEventInfo.instance.CurrentEvent);
                    //    }
                    //}
                    //if (RaceEventInfo.Instance.CurrentEvent.IsBossRace() && RaceEventInfo.Instance.CurrentEvent.IsFinalRaceInGroup())
                    //{
                    //    PushVars vars2 = new PushVars();
                    //    vars2.Add("bossName", RaceEventInfo.Instance.AIDriverData.Name);
                    //    PushVars vars = vars2;
                    //    NotificationManager.Active.SendPushToFriends("BeatTierBoss", vars, null);
                    //}
                }
            }

            CommonUI.Instance.CashStats.CashLockedState(false);
            if (!lockStat)
                CommonUI.Instance.CashStats.GoldLockedState(false);
            CommonUI.Instance.XPStats.XPLockedState(false);
            CommonUI.Instance.XPStats.LevelUpLockedState(false);
            CommonUI.Instance.StarLeagueStats.StarLockedState(false);
            CommonUI.Instance.StarStats.StarLockedState(false);
            CommonUI.Instance.StarStats.NewLeagueLockedState(false);
            CommonUI.Instance.FuelStats.FuelLockedState(false);
            //CommonUI.Instance.RPStats.RPLockedState(false);
            //FriendsRewardManager.Instance.ShowRewardsLockedState(false);
            haveGivenReward = true;
        }
    }


    private void ShowPopupforSMPWinStreakReward(SMPWinStreakReward SMPReward)
    {
        if (SMPReward != null)
        {
            if (CommonUI.Instance != null)
            {
                CommonUI.Instance.CashStats.GoldLockedState(true);
                //CommonUI.Instance.RPStats.IsLocked_RP = true;
            }

            if ((FuelManager.Instance != null) && (SMPReward.WinReward.rewardType == ERewardType.FuelPip))
            {
                FuelManager.Instance.FuelLockedState(true);
            }

            CSR2ApplyableReward applyableReward = new CSR2ApplyableReward(SMPReward.WinReward, SMPReward.Amount);
            AwardWinStreakPrizeSMP();
            string body = string.Format(LocalizationManager.GetTranslation("TEXT_SMP_WIN_STREAK_REWARD_BODY"),
                applyableReward.GetRewardText());
            var popup = new PopUp()
            {
                Title = "TEXT_SMP_WIN_STREAK_REWARD_TITLE",
                BodyText = body,
                BodyAlreadyTranslated = true,
                ConfirmText = "TEXT_BUTTON_COLLECT",
                ConfirmAction = OnClickSMPRewardCollect,
                ImageCaption = "TEXT_NAME_AGENT",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                IsBig = true

            };
            PopUpManager.Instance.TryShowPopUp(popup);
        }
    }

    private void AwardWinStreakPrizeSMP()
    {
        if (CurrentWinStreakReward != null)
        {
            SMPWinStreakReward currentWinStreakReward = CurrentWinStreakReward;
            if (currentWinStreakReward.WinReward.rewardType != ERewardType.Invalid)
            {
                CSR2ApplyableReward reward2 = new CSR2ApplyableReward();
                reward2.FillAnyRandomPrizes(currentWinStreakReward.WinReward);
                reward2.amount = currentWinStreakReward.Amount;
                reward2.ApplyAwardToPlayerProfile(false, true);
            }
        }
    }


    private void OnClickSMPRewardCollect()
    {
        CurrentWinStreakReward = null;
        alreadyAwardedWinStreakThisRace = true;
        CommonUI.Instance.CashStats.GoldLockedState(false);
        //CommonUI.Instance.RPStats.IsLocked_RP = false;
    }

    protected override void OnEnterPressed()
    {
        if (!ScreenFinished)
        {
            FinishScreen(true);
        }
        else
        {
            if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent() &&
                !LegacyObjectivesManager.IsLegacyObjectiveCompleted("FirstFriendRace"))
            {
                return;
            }

            MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
            OnNextButton();
        }
    }

    public override void RequestBackup()
    {
        if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent() &&
            !LegacyObjectivesManager.IsLegacyObjectiveCompleted("FirstFriendRace"))
        {
            return;
        }

        MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
        OnNextButton();
    }
}
