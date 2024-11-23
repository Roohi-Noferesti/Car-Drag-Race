using System;
using System.Collections;
using AdSystem.Enums;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using Objectives;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GarageScreen : ZHUDScreen
{
    [SerializeField] private UnityEvent m_onIntroStarted;
    [SerializeField] private UnityEvent m_onIntroFinished;
    [SerializeField] private RuntimeTextButton m_showRoomButton;
    [SerializeField] private RuntimeTextButton m_upgradeButton;
    [SerializeField] private RuntimeTextButton m_nextButton;
    [SerializeField] private RuntimeTextButton m_achivementButton;
    [SerializeField] private RuntimeTextButton m_customiseButton;
    [SerializeField] private RuntimeButton m_nativeAdButton;
    [SerializeField] private RawImage m_nativeAdIcon;
    [SerializeField] private Image m_topVignet;
    [SerializeField] private GameObject m_buttonsSide;
    [SerializeField] private GameObject m_buttonsSideLeft;
    [SerializeField] private GameObject m_buttonsPanel;
    [SerializeField] private RuntimeButton m_snapshotButton;
    [SerializeField] private TextMeshProUGUI m_snapshotText;
    [SerializeField] private GameObject m_snapshotLogo;
    [SerializeField] private GameObject m_bazaarLeaderboardButton;
    [SerializeField] private Button m_appTuttiTimedRewardButton;
    [SerializeField] private Button m_vasTimedRewardButton;
    [SerializeField] private TextMeshProUGUI m_appTuttiRewardWaitText;
    [SerializeField] private TextMeshProUGUI m_vasRewardWaitText;

    private bool nextScreenRequest;
    private FlowConditionalBase screenConditionals;
    private FlowConditionalBase worldTourScreenConditional;
    private BubbleMessage m_showRoomBubble;
    private BubbleMessage NextButtonBubbleMessage;
    private BubbleMessage UpgradeButtonBubbleMessage;
    private BubbleMessage ShopButtonBubbleMessage;
    private BubbleMessage CarDealerBubbleMessage;
    private BubbleMessage CarSelectBubbleMessage;
    private BubbleMessage GarageIntroBubbleMessage;
    private bool waitingForCameraUnZoom;
    //private HudScreenEventArgs m_screenEventArgs;
    public static bool HasShownCrewChatter;

    private static float m_lastNativeBannerRequest;

#if GT_DEBUG_LOGGING
    [SerializeField]
    private bool m_debugLeaderboard;
    [SerializeField]
    private bool m_debugCustomise;
#endif

    private ScreenID m_lastScreenID;
    private bool isProgressionPopupValid;
    private bool popupShown;
    private static bool _hasCheckAnyNativeBannerAfterRace;


    public static GarageScreen Instance { get; private set; }

    public override ScreenID ID
    {
        get
        {
            return ScreenID.Workshop;
        }
    }

    public bool IsCarNew { get; private set; }
    public bool ShowingIntro { get; private set; }
    public static bool WorldTourPopupEvaulated { get; set; }

    protected override void Awake()
    {
        base.Awake();
        EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
        SequenceManager.Instance.OnSequenceEnd += Instance_OnSequenceEnd;
        //GarageManager.CarSetupCompleted += Instance_CarSetupCompleted;
        m_snapshotButton.AddValueChangedDelegate(OnTwitterButton);

        m_showRoomButton.AddValueChangedDelegate(OnShowRoomButtonPressed);
        m_upgradeButton.AddValueChangedDelegate(OnUpgradeButtonPressed);
        m_nextButton.AddValueChangedDelegate(OnStartButton);
        m_customiseButton.AddValueChangedDelegate(OnCustomiseButtonPressed);
        m_achivementButton.AddValueChangedDelegate(OnAchivementButtonPressed);
        m_nativeAdButton.AddValueChangedDelegate(OnShowNativeAd);

        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnTwitterButton()
    {
        if (!TouchManager.AttemptToUseButton("TwitterScreenShot"))
        {
            return;
        }
        if (SocialController.TwitterIsDisabled)
        {
            BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_TWITTER_DISABLED"));
            return;
        }
        BubbleManager.Instance.FadeAllOut();
        this.PrepareUIForScreenshot();
        ScreenshotCapture.Instance.CaptureAndTweetIfPossible(string.Empty, new Action(this.ResetAfterScreenshot));
        if (!BasePlatform.ActivePlatform.UsesNativeSharing() && !BasePlatform.ActivePlatform.CanSendTweet())
        {
            SocialController.Instance.ClearSocialNagTrigger();
        }
    }

    private void PrepareUIForScreenshot()
    {
        //CommonUI.Instance.NavBar.HideBackButton();
        //this.tweetCameraButton.gameObject.SetActive(false);
        //this.normalCameraButton.gameObject.SetActive(false);
        //this.CSRLogo.gameObject.SetActive(true);
        GarageCameraManager.Instance.enabled = false;
        m_snapshotLogo.SetActive(true);
        m_snapshotText.gameObject.SetActive(true);
        m_snapshotButton.CurrentState = BaseRuntimeControl.State.Hidden;
    }


    private void ResetAfterScreenshot()
    {
        //CommonUI.Instance.NavBar.ShowBackButton();
        //if (BasePlatform.ActivePlatform.UsesNativeSharing())
        //{
        //    this.normalCameraButton.gameObject.SetActive(true);
        //}
        //else
        //{
        //    bool flag = BasePlatform.ActivePlatform.CanSendTweet();
        //    this.tweetCameraButton.gameObject.SetActive(flag);
        //    this.normalCameraButton.gameObject.SetActive(!flag);
        //}
        //this.CSRLogo.gameObject.SetActive(false);
        m_snapshotLogo.SetActive(false);
        m_snapshotText.gameObject.SetActive(false);
        //BaseDevice.ActiveDevice.ApplyInitialQuality();
        GarageCameraManager.Instance.enabled = true;
    }

    private IEnumerator m_snapshotCoroutine()
    {
        m_snapshotLogo.SetActive(true);
        m_snapshotText.gameObject.SetActive(true);
        m_snapshotButton.CurrentState = BaseRuntimeControl.State.Hidden;
        yield return new WaitForSeconds(0.5F);
        BaseDevice.ActiveDevice.SetToScreenShotQuality();
        ScreenShot.TakeGarageScreenShot(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey
            , true, tex =>
            {
#if UNITY_EDITOR
                didBecomeActiveEvent();
                //#else
                var title = LocalizationManager.GetTranslation("TEXT_SHARE_SNAPSHOT_TITLE");
                var url = GTPlatform.GetPlatformUpdateURL();
                var body = string.Format(LocalizationManager.GetTranslation("TEXT_SHARE_SNAPSHOT_BODY"),
                    url);
                BasePlatform.ActivePlatform.ShareImage(title, url, ScreenshotCapture.Instance.CurrentFilename);
#endif
            });
    }

    void Instance_CarSetupCompleted()
    {
        //if (m_screenEventArgs != null)
        //{
        //    m_screenEventArgs.Wait = false;
        //}
    }

    protected override void OnDestroy()
    {
        EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        //GarageManager.CarSetupCompleted -= Instance_CarSetupCompleted;
        if (Instance == this)
        {
            Instance = null;
        }
        base.OnDestroy();
    }

    void EasyTouch_On_SimpleTap(Gesture gesture)
    {
        if (ShowingIntro)
        {
            StopGarageIntro();
        }
    }

    void Instance_OnSequenceEnd(string zSequenceName)
    {
        if (zSequenceName == "GarageIntro_Sequence")
            AfterGarageIntro();
    }

    private void ShowGarageIntro()
    {
        if (!ShowingIntro)
        {
            ShowingIntro = true;
            m_onIntroStarted.Invoke();
            //GarageManager.Instance.GarageCam.enabled = false;
            SequenceManager.Instance.PlaySequence("GarageIntro_Sequence");
            if (m_topVignet != null)
                m_topVignet.enabled = false;
            GarageIntroBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_GARAGE_INTRO_BUUBLE_DESC", false, new Vector3(0, -5, 0),
                BubbleMessage.NippleDir.DOWN,
                0.5f, BubbleMessageConfig.Frontend, false, false);
        }
    }

    private void AfterGarageIntro()
    {
        if (ShowingIntro)
        {
            GarageIntroBubbleMessage.OnDestroyEvent += m =>
            {
                BringUpDialog();
            };
            GarageIntroBubbleMessage.Dismiss();
        }
    }

    private void BringUpDialog()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var playerGold = activeProfile.GetCurrentGold();
        var playerCash = activeProfile.GetCurrentCash();
        var goldReward = GameDatabase.Instance.TutorialConfiguration.InitialGoldReward;
        var cashReward = GameDatabase.Instance.TutorialConfiguration.InitialCashReward;
        if (playerGold < goldReward)
        {
            var gold = goldReward - playerGold;
            activeProfile.AddGold(gold, "reward", "BringUpDialog");
        }
        if (playerCash < cashReward)
        {
            var cash = cashReward - playerCash;
            activeProfile.AddCash(cash, "reward", "BringUpDialog");
        }

        var cashString = CurrencyUtils.GetCashString(cashReward);
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_TUTORIAL_BUY_FIRST_CAR_TITLE",
            BodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_TUTORIAL_BUY_FIRST_CAR_BODY_NEW"), cashString),
            IsBig = true,
            ConfirmAction = () =>
            {
                ShowRoomBubble();
                if (m_topVignet != null)
                    m_topVignet.enabled = true;
                ShowingIntro = false;
                m_onIntroFinished.Invoke();
                CommonUI.Instance.PlayAnimation(true);
            },
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
            ImageCaption = "TEXT_NAME_RACE_OFFICIAL",
            ShouldCoverNavBar = true,
            BodyAlreadyTranslated = true
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }


    private void BringUpRealRaceIntroDialog()
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_REAL_RACE_BEGIN",
            BodyText = "TUTORIAL_DIALOG_REAL_RACE_TEXT",
            IsBig = true,
            ConfirmAction = () =>
            {
                ShowNextRaceBubble();
            },
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT",
            ShouldCoverNavBar = true,
        };
        PlayerProfileManager.Instance.ActiveProfile.HasSeenRealRacePopup = true;
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }


    public void ShowMapBubble()
    {
        Vector3 position = m_nextButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
        NextButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_ENTER_MAP", false, position,
            BubbleMessage.NippleDir.DOWN, 0.2F, BubbleMessageConfig.Frontend
            , true, true, m_nextButton.rectTransform());
    }

    private void ShowRoomBubble()
    {
        Vector3 position = m_showRoomButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
        m_showRoomBubble = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_BUY_CAR", false, position,
            BubbleMessage.NippleDir.DOWN, 0.2F, BubbleMessageConfig.Frontend, true
            , true, m_showRoomButton.rectTransform());
    }

    private void ShowNextRaceBubble()
    {
        Vector3 position = m_nextButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
        NextButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_GOTO_MAP_AND_RACE", false, position,
            BubbleMessage.NippleDir.DOWN, 0.9F, BubbleMessageConfig.Frontend, true, true,
            m_nextButton.rectTransform());
    }

    public void OnShowRoomButtonPressed()
    {
        if (m_showRoomBubble != null)
        {
            m_showRoomBubble.OnDestroyEvent += (b) => { ScreenManager.Instance.PushScreen(ScreenID.CarClass); };
            m_showRoomBubble.Dismiss();
        }
        else
        {
            ScreenManager.Instance.PushScreen(ScreenID.CarClass);
        }
    }


    public void OnCarSelectButtonPressed()
    {
        if (CarSelectBubbleMessage != null)
        {
            CarSelectBubbleMessage.OnDestroyEvent += (b) => { ScreenManager.Instance.PushScreen(ScreenID.CarSelect); };
            CarSelectBubbleMessage.Dismiss();
        }
        else
        {
            var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            if (currentCar != null)
                MyCarScreen.OnLoadCar = currentCar.CarDBKey;
            ScreenManager.Instance.PushScreen(ScreenID.CarSelect);
        }
    }

    public void OnUpgradeButtonPressed()
    {
        if (UpgradeButtonBubbleMessage != null)
        {
            UpgradeButtonBubbleMessage.OnDestroyEvent += (b) => { ScreenManager.Instance.PushScreen(ScreenID.Tuning); };
            UpgradeButtonBubbleMessage.Dismiss();
        }
        else
        {
            ScreenManager.Instance.PushScreen(ScreenID.Tuning);
        }
    }


    public void OnCustomiseButtonPressed()
    {
        //if (UpgradeButtonBubbleMessage != null)
        //{
        //    UpgradeButtonBubbleMessage.OnDestroyEvent += (b) => { ScreenManager.Active.pushScreen(ScreenID.Tuning); };
        //    UpgradeButtonBubbleMessage.Dismiss();
        //}
        //else
        //{

#if GT_DEBUG_LOGGING
        if (m_debugCustomise)
        {
            m_lastScreenID = ScreenID.Customise;
            ScreenManager.Instance.PushScreen(ScreenID.Customise);
            return;
        }
#endif

        var unlockLevel = GameDatabase.Instance.CareerConfiguration.CustomiseUnlockLevel;
        if (PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel() >= unlockLevel)
        {
            m_lastScreenID = ScreenID.Customise;
            ScreenManager.Instance.PushScreen(ScreenID.Customise);
        }
        else
        {
            var bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_PAINTSHOP_LOCK_DESK"), unlockLevel);
            PopUpManager.Instance.TryShowPopUp(new PopUp
            {
                Title = "TEXT_PAINTSHOP_LOCK_TITLE",
                BodyText = bodyText,
                BodyAlreadyTranslated = true,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            }, PopUpManager.ePriority.Default, null);
        }

        //}
    }

    public void StopGarageIntro()
    {
        if (ShowingIntro)
        {
            SequenceManager.Instance.StopSequence();
            AfterGarageIntro();
        }
    }


    public override bool Wait(bool startingUp)
    {
        if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            return false;
        if (startingUp)
            return SceneManagerGarage.Instance == null || !SceneManagerGarage.Instance.CarIsLoaded;
        return false;
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        //we want to ensure our products are retrieved
        AppStore.Instance.StartProductRequestIfStillWaiting();
        m_nativeAdButton.CurrentState = BaseRuntimeControl.State.Hidden;

        if (ShowingIntro)
        {
            ShowingIntro = false;
            m_onIntroFinished.Invoke();
            CommonUI.Instance.PlayAnimation(true);
        }
        if (!zAlreadyOnStack || SceneManagerGarage.Instance.OnCarLoaded == null)
            SceneManagerGarage.Instance.OnCarLoaded += OnCarLoaded;
        //GarageManager.CarSetupCompleted += OnCarLoaded;
        if (PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
        {
            //Debug.Log(obj.FromScreen.GetScreenID() + "    " + obj.ToScreen.GetScreenID());
            if (ScreenManager.Instance.LastScreen != ScreenID.Customise)
                GarageCamBehaviourRemastered.Instance.ResetTransform();
            this.ShowCarNow(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
        }

        if (PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstTutorialRace())
        {
            AchievementChecks.CheckForFirstAchievement();
        }

        LeftSidePanelContainer.Instance.HideLeftSidePanel();

        m_snapshotButton.CurrentState = BaseRuntimeControl.State.Hidden;
        m_snapshotLogo.SetActive(false);
        m_snapshotText.gameObject.SetActive(false);
        if (!zAlreadyOnStack)
        {
            GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
            //if (!GarageManager.Instance.CarIsLoaded)
            //{
            //    GarageCameraManager.Instance.SuppressAutoCams = true;
            //}

            if (!SceneManagerGarage.Instance.CarIsLoaded)
            {
                GarageCameraManager.Instance.SuppressAutoCams = true;
            }
            //SceneManagerGarage.Instance.OnCarLoaded += OnCarLoaded;
        }
        this.nextScreenRequest = false;
        if (RaceEventQuery.Instance.getHighestUnlockedClass() != eCarTier.TIER_1)
        {
            //this.uiTapToZoom.gameObject.SetActive(false);
        }
        //this.UpdateModeSwitchIcons();
        AchievementChecks.AchievementsCheck();


        if (GTAdManager.Instance.ShouldTriggerInterstitialBannerInWorkshop())
        {
            GTAdManager.Instance.AutoShowAd(AdSpace.InterstitialBannerAfterRace, 3);
        }
        else if (GTAdManager.Instance.ShouldTriggerAdInWorkshop())
        {
            GTAdManager.Instance.AutoShowAd(AdSpace.Default, 3);
        }

        if ((Time.time - m_lastNativeBannerRequest > GameDatabase.Instance.AdConfiguration.NativeBannerFreqTime
             || !_hasCheckAnyNativeBannerAfterRace))
        {
            m_nativeAdButton.CurrentState = BaseRuntimeControl.State.Hidden;
            GTDebug.Log(GTLogChannel.Adverts, "Request native banner from garagescreen");

            GTAdManager.Instance.ShowNativeBanner(AdSpace.Default,
                OnLoadBest: (space, mes, banner) =>
                {
                    if (banner != null && m_nativeAdButton != null)
                    {
                        var image = m_nativeAdButton.GetComponentInChildren<RawImage>();
                        if (image != null)
                        {
                            image.texture = banner.Icon;
                            m_lastNativeBannerRequest = Time.time;
                            _hasCheckAnyNativeBannerAfterRace = true;
                            GTDebug.Log(GTLogChannel.Adverts, "native banner cached in garagescreen");
                            m_nativeAdButton.CurrentState = BaseRuntimeControl.State.Active;
                        }
                    }
                },
                OnError: (space, mes) =>
                {


                });

        }


        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        this.screenConditionals = new WorkshopScreenConditional();

        this.worldTourScreenConditional = new RaceRewardScreenWorldTourFlowConditional();
        //SeasonFlowManager.Instance.OnEnteredWorkshop();
        this.screenConditionals.EvaluateAll();//note:This function is performance killer , It take 450 ms to evaluate in PC
        this.worldTourScreenConditional.EvaluateAll();
        if (activeProfile.IsCarNew(activeProfile.CurrentlySelectedCarDBKey))
        {
            IsCarNew = true;
            this.waitingForCameraUnZoom = true;
            GarageCameraManager.Instance.UnZoomEvent += new OnUnZoom(this.CameraUnZoomed);
            m_buttonsPanel.SetActive(false);
            m_buttonsSide.SetActive(false);
            m_buttonsSideLeft.SetActive(false);
            CommonUI.Instance.PlayAnimation(false);
            GarageCameraManager.Instance.TriggerNewCarCameraSequence(activeProfile.CurrentlySelectedCarDBKey);
        }
        ApplicationManager.DidBecomeActiveEvent += this.didBecomeActiveEvent;
        MapScreenCache.Map.SetActive(false);
        if (!activeProfile.HasBoughtFirstCar)
        {
            ShowroomScreen.Init.CurrentManufacturer = "None";
            ShowroomScreen.Init.screenMode = ShowroomMode.Tutorial_BuyCar;
            //ScreenManager.Instance.PushScreen(ScreenID.Showroom);
            //ScreenManager.Instance.UpdateImmediately();
            ShowGarageIntro();
        }


        //achivement button is disabled for abtest
        if (activeProfile.GetPlayerLevel() < 2 || ObjectiveManager.Instance.ActiveObjectives.Count == 0) //Crew Member 1
        {
            m_achivementButton.CurrentState = BaseRuntimeControl.State.Hidden;
        }
        else
        {
            m_achivementButton.CurrentState = BaseRuntimeControl.State.Active;
        }

        //if (activeProfile.GetPlayerLevel() < 3 && !m_debugCustomise)
        //{
        //    m_customiseButton.CurrentState = BaseRuntimeControl.State.Disabled;
        //}
        //else
        //{
        //    m_customiseButton.CurrentState = BaseRuntimeControl.State.Active;
        //}

        isProgressionPopupValid = !waitingForCameraUnZoom;
        //if (!HasShownCrewChatter && RaceEventInfo.Instance.CurrentEvent != null && RaceResultsTracker.You != null &&
        //    Chatter.PostRace(/*RaceController.Instance.OnTauntDismiss*/null,
        //    RaceEventInfo.Instance.CurrentEvent, RaceResultsTracker.You.IsWinner))
        //{
        //    HasShownCrewChatter = true;
        //}
        if (!activeProfile.HasBoughtFirstCar)
        {
            //ShowroomScreen.Init.CurrentManufacturer = null;
            //ShowroomScreen.Init.screenMode = ShowroomMode.Tutorial_BuyCar;
            ////ScreenManager.Instance.PushScreen(ScreenID.Showroom);
            ////ScreenManager.Instance.UpdateImmediately();
            //ShowGarageIntro();
        }
        else if (!activeProfile.HasChoosePlayerName && activeProfile.RacesWon >= 1 && activeProfile.RacesEntered >= 1
            && UserManager.Instance.isLoggedIn && PolledNetworkState.IsNetworkConnected
            && ServerSynchronisedTime.Instance.ServerTimeValid)
        {
            //Debug.Log("try to open chooseName");
            isProgressionPopupValid = false;
            ScreenManager.Instance.PushScreen(ScreenID.ChooseName);
        }

        activeProfile.CarSeen(activeProfile.CurrentlySelectedCarDBKey);

        //OnAfterActivate();

        m_bazaarLeaderboardButton.SetActive(BazaarGameHubManager.Instance.IsActive);
        BazaarGameHubManager.Instance.bazaarLeaderBoardButton = m_bazaarLeaderboardButton;
        BazaarGameHubManager.Instance.SetButtonsVisibility();

        if (BuildType.IsAppTuttiBuild && !GTAdManager.Instance.ShouldHideAdInterface)
        {
            m_appTuttiTimedRewardButton.gameObject.SetActive(true);
            m_appTuttiRewardWaitText.gameObject.SetActive(true);
            SetAppTuttiButtonVisibility(AppTuttiTimedRewardManager.Instance.IsTimeForReward());
        }
        else
        {
            m_appTuttiTimedRewardButton.gameObject.SetActive(false);
            m_appTuttiRewardWaitText.gameObject.SetActive(false);
        }

        if (BuildType.IsVasBuild)
        {
            m_vasTimedRewardButton.gameObject.SetActive(true);
            m_vasRewardWaitText.gameObject.SetActive(true);
            SetVasButtonVisibility(VasTimedRewardManager.Instance.IsTimeForReward());
        }
        else
        {
            m_vasTimedRewardButton.gameObject.SetActive(false);
            m_vasTimedRewardButton.gameObject.SetActive(false);
        }
    }

    public override void OnAfterActivate()
    {
        //var isProgressionPopupValid = !waitingForCameraUnZoom;
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //if (!m_hasShownCrewChatter && RaceEventInfo.Instance.CurrentEvent != null && RaceResultsTracker.You != null &&
        //    Chatter.PostRace(/*RaceController.Instance.OnTauntDismiss*/null,
        //    RaceEventInfo.Instance.CurrentEvent, RaceResultsTracker.You.IsWinner))
        //{
        //    m_hasShownCrewChatter = true;
        //}
        //else if (!activeProfile.HasBoughtFirstCar)
        //{
        //    //ShowroomScreen.Init.CurrentManufacturer = null;
        //    //ShowroomScreen.Init.screenMode = ShowroomMode.Tutorial_BuyCar;
        //    ////ScreenManager.Instance.PushScreen(ScreenID.Showroom);
        //    ////ScreenManager.Instance.UpdateImmediately();
        //    //ShowGarageIntro();
        //}
        //else if (!activeProfile.HasChoosePlayerName && activeProfile.HasCompletedFirstThreeTutorialRaces())
        //{
        //    Debug.Log("try to open chooseName");
        //    isProgressionPopupValid = false;
        //    ScreenManager.Instance.PushScreen(ScreenID.ChooseName);
        //}
        //if (!activeProfile.HasBoughtFirstUpgrade || !activeProfile.IsEventCompleted(1003))
        //{

        //}
        if (!activeProfile.HasBoughtFirstCar)
        {

        }
        else if (activeProfile.IsCarNew(activeProfile.CurrentlySelectedCarDBKey))
        {

        }
        else if (activeProfile.RacesWon == 0)
        {
            if (waitingForCameraUnZoom)
                return;
            Log.AnEvent(Events.TheseAreRegulationRaces);
            //if (!PlayerProfileManager.Instance.ActiveProfile.HasSeenRealRacePopup)
            //    BringUpRealRaceIntroDialog();
            //else
            //{
            ShowNextRaceBubble();
            //}
        }
        else if (PlayerProfileManager.Instance.ActiveProfile.RacesEntered == 1)
        {
            Log.AnEvent(Events.TheseAreCrewRaces);
            var tutorialConfig = GameDatabase.Instance.TutorialConfiguration;
            Vector3 position = m_nextButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
            NextButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_GOTO_MAP_AND_RACE", false, position,
                BubbleMessage.NippleDir.DOWN, 0.9F, BubbleMessageConfig.Frontend, tutorialConfig.IsOn, true
                , m_nextButton.rectTransform());
        }
        else if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 0 &&
                 !activeProfile.HasCompletedFirstCrewRace())
        {
            var tutorialConfig = GameDatabase.Instance.TutorialConfiguration;
            if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade)
            {
                Vector3 position = m_upgradeButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
                UpgradeButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_UPGRADE", false, position,
                    BubbleMessage.NippleDir.DOWN, 0.9F, BubbleMessageConfig.Frontend, tutorialConfig.IsOn, true
                    , m_upgradeButton.rectTransform());
            }
            else
            {
                Vector3 position = m_nextButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
                NextButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_GOTO_MAP_AND_RACE", false, position,
                    BubbleMessage.NippleDir.DOWN, 0.9F, BubbleMessageConfig.Frontend, false, true
                    , m_nextButton.rectTransform());
            }

            if (!AppStore.Instance.ShouldHideIAPInterface)
            {
                var shopButtonPos = CommonUI.Instance.ShopButtonPoint.GetBottomPoint() + new Vector3(0, -0.01f, 0);
                if (!tutorialConfig.IsOn)
                    this.ShopButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_SHOP", false, shopButtonPos,
                        BubbleMessage.NippleDir.UP, 1f);
            }
        }
        else if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 2 &&
                 PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade &&
                 PlayerProfileManager.Instance.ActiveProfile.HasSeenFacebookNag &&
                 PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt > DateTime.MinValue &&
                 !PlayerProfileManager.Instance.ActiveProfile.HasVisitedManufacturerScreen)
        {
            Vector3 vector4 = new Vector3(); //base.CarouselList.GetItem(0).transform.position;
            vector4 += new Vector3(0f, 0.38f, 0f);
            this.CarDealerBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_CARDEALER", false, vector4,
                BubbleMessage.NippleDir.DOWN, 0.2F);
        }
        //else if (DoUnlockTierCheck())
        //{
        //    isProgressionPopupValid = false;
        //    ScreenManager.Active.pushScreen(ScreenID.TierUnlocked);
        //}
        //else if (SocialController.Instance.isLoggedIntoFacebook && PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon == 1)
        //{
        //    Vector3 vector5 = base.CarouselList.GetItem(4).transform.position;
        //    vector5 += new Vector3(0f, 0.38f, 0f);
        //    this.NextButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_RETURN_TO_CITY_MAP", false, vector5, BubbleMessage.NippleDir.DOWN, 1f, BubbleMessageConfig.ThemeStyle.SMALL, BubbleMessageConfig.PositionType.BOX_RELATIVE, 0.16f);
        //}
        //IGameState gameState = new GameStateFacade();
        //if (gameState.IsCurrentCar("FerrariFXXK"))
        //{
        //    ListItem item = base.CarouselList.GetItem(1);
        //    if (gameState.GetWorldTourThemeSeenCount("TierX_International_Finals") == 0)
        //    {
        //        item.GreyOutThisItem(true);
        //        item.GreyedOutTap += new ShopListItem.TapEventHandler(this.ShowMechanicMissingPopup);
        //    }
        //    else if (gameState.GetCurrentCarEvoPartsEarned(0) < 1)
        //    {
        //        item.GreyOutThisItem(true);
        //        item.GreyedOutTap += new ShopListItem.TapEventHandler(this.ShowEvolutionNudgePopup);
        //    }
        //}

        if (!ShowingIntro && isProgressionPopupValid && !GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null))
        {
            GameDatabase.Instance.BundleOffers.UpdateOffers();
            BundleOfferController.Instance.TryShowIAPBundleForScreen(false);
        }

        //MultiplayerUtils.SelectedMultiplayerMode = MultiplayerMode.NONE;
        //if (activeProfile.HasNewCarsOtherThanCurrentlySelected())
        //{
        //    base.AddCarouselNotification(3);
        //}
        //if (activeProfile.CurrentCarHasNewLiveries)
        //{
        //    base.AddCarouselNotification(2);
        //}
    }

    private void CameraUnZoomed()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        IsCarNew = false;
        m_buttonsPanel.SetActive(true);
        m_buttonsSide.SetActive(true);
        m_buttonsSideLeft.SetActive(true);
        if (activeProfile.RacesWon == 0)
        {
            Log.AnEvent(Events.TheseAreRegulationRaces);
            ShowNextRaceBubble();
        }
        else if (!GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null))
        {
            GameDatabase.Instance.BundleOffers.UpdateOffers();
            BundleOfferController.Instance.TryShowIAPBundleForScreen(false);
        }
        this.waitingForCameraUnZoom = false;
        GarageCameraManager.Instance.UnZoomEvent -= new OnUnZoom(this.CameraUnZoomed);
    }

    //private void ShowMechanicMissingPopup(ListItem zItem)
    //{
    //    PopUpManager.Instance.TryShowPopUp(new PopUp
    //    {
    //        Title = "TEXT_POPUPS_MECHANIC_MISSING_TITLE",
    //        BodyText = "TEXT_POPUPS_MECHANIC_MISSING_BODY",
    //        IsBig = true,
    //        ConfirmText = "TEXT_BUTTON_OK",
    //        BundledGraphicPath = PopUpManager.Instance.graphics_agentPrefab,
    //        ImageCaption = "TEXT_NAME_AGENT"
    //    }, PopUpManager.ePriority.Default, null);
    //}

    //private void ShowEvolutionNudgePopup(ListItem zItem)
    //{
    //    PopUpManager.Instance.TryShowPopUp(new PopUp
    //    {
    //        Title = "TEXT_POPUPS_EVOLUTION_NUDGE_TITLE",
    //        BodyText = "TEXT_POPUPS_EVOLUTION_NUDGE_BODY",
    //        ConfirmText = "TEXT_BUTTON_OK",
    //        BundledGraphicPath = "InternationalPortraits.mechanic",
    //        ImageCaption = "TEXT_NAME_MECHANIC",
    //        IsCrewLeader = true,
    //        UseImageCaptionForCrewLeader = true
    //    }, PopUpManager.ePriority.Default, null);
    //}


    private void ShowTestYourCarPopup()
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "TEXT_POPUPS_EVOLUTION_NUDGE_TITLE",
            BodyText = "TEXT_POPUPS_EVOLUTION_NUDGE_BODY",
            ConfirmText = "TEXT_BUTTON_OK",
            ConfirmAction = () => { },
            GraphicPath = "InternationalPortraits.mechanic",
            ImageCaption = "TEXT_NAME_MECHANIC",
            UseImageCaptionForCrewLeader = true
        }, PopUpManager.ePriority.Default, null);
    }

    private void didBecomeActiveEvent()
    {
        //if (GarageCameraManager.Instance.IsZoomedIn)
        //    m_snapshotButton.CurrentState = BaseRuntimeControl.State.Active;
        //m_snapshotLogo.SetActive(false);
        //m_snapshotText.gameObject.SetActive(false);
        BaseDevice.ActiveDevice.ApplyInitialQuality();
        GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
        //this.screenConditionals = new WorkshopScreenConditional();
        //this.screenConditionals.EvaluateAll();
    }

    public void ShowCarNow(string carKey)
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        activeProfile.CurrentlySelectedCarDBKey = carKey;
        activeProfile.UpdateCurrentCarSetup();
        var currentCar = activeProfile.GetCurrentCar();
        if (currentCar != null)
        {
            //var carInfo = CarDatabase.Instance.GetCar(carKey);
            CarInfoUI.Instance.SetCurrentCarIDKey(carKey);
            //m_carNameText.text = LocalizationManager.GetTranslation(carInfo.ShortName);
            //m_carRateText.text = currentCar.CurrentPPIndex.ToString();
            //GarageManager.Instance.SwitchCar(currentCar.CarDBKey);
        }
        //CarInfoUI.Instance.SetCurrentCarIDKey(carKey);
    }

    public void OnStartButton()
    {
        this.nextScreenRequest = true;
    }

    protected override void OnEnterPressed()
    {
        //if (this.CurrentAnimState == CSRScreen.AnimState.IN)
        //{
        //    this.OnStartButton();
        //}
    }

    public override void OnDeactivate()
    {
        m_nativeAdButton.RemoveValueChangedDelegate(OnShowNativeAd);
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        activeProfile.CarSeen(activeProfile.CurrentlySelectedCarDBKey);
        LoadingScreenManager.Instance.ClearLoadingPanel();
        //GarageManager.CarSetupCompleted -= OnCarLoad;
        if (SceneManagerGarage.Instance != null)
            SceneManagerGarage.Instance.OnCarLoaded -= OnCarLoaded;
        ApplicationManager.DidBecomeActiveEvent -= this.didBecomeActiveEvent;
    }

    private void OnRacePress()
    {
        this.nextScreenRequest = true;
    }

    private void OnCarLoaded()
    {
        GarageCameraManager.Instance.SuppressAutoCams = false;

        if (m_lastScreenID != ScreenID.Customise)
        {
            GarageCamBehaviourRemastered.Instance.ResetTransform();
        }
        m_lastScreenID = ScreenID.Invalid;
    }

    private void OnPrizesAwardedPopupDismiss()
    {
        //ScreenManager.Instance.PushScreen(ScreenID.Showroom);
    }

    public override void StartAnimIn()
    {
        m_animator.Play("Open");
        CommonUI.Instance.PlayAnimation(true);
        m_snapshotButton.CurrentState = BaseRuntimeControl.State.Hidden;
        m_snapshotLogo.SetActive(false);
        m_snapshotText.gameObject.SetActive(false);
    }

    public override void StartAnimOut()
    {
        m_animator.Play("Close");
        CommonUI.Instance.PlayAnimation(false);
        if (BuildType.CanShowShareButton())
            m_snapshotButton.CurrentState = BaseRuntimeControl.State.Active;
        else
            m_snapshotButton.CurrentState = BaseRuntimeControl.State.Hidden;
        m_snapshotLogo.SetActive(false);
        m_snapshotText.gameObject.SetActive(false);
    }

    public bool DoUnlockTierCheck()
    {
        //var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //var playerLevel = activeProfile.GetPlayerLevel();
        //if (!activeProfile.HasSeenUnlockCarScreen)
        //{
        //    activeProfile.HasSeenUnlockCarScreen = true;
        //    var newcarUnlocked = GameDatabase.Instance.Cars.GetTierUnlockAtLevel(playerLevel);
        //    if (newcarUnlocked != eCarTier.BASE_EVENT_TIER && newcarUnlocked!=eCarTier.TIER_1)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }

    protected override void Update()
    {
        base.Update();
        if (CarStatsCalculator.Instance.IsCalculatingPerformance)
        {
            return;
        }
        if (this.nextScreenRequest)
        {
            MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
            this.nextScreenRequest = false;
            ScreenManager.Instance.PushScreen(ScreenID.CareerModeMap);
            CleanDownManager.Instance.OnEnterMap();
            return;
        }
        //bool flag = RaceEventQuery.Instance.getHighestUnlockedClass() == eCarTier.TIER_1;
        //this.uiTapToZoom.gameObject.SetActive(flag && !GarageCameraManager.Instance.IsZoomedIn);
        if (RaceEventInfo.Instance.CurrentEvent.IsWorldTourRace() && !WorldTourPopupEvaulated)
        {
            WorldTourPopupEvaulated = true;
            this.TryConditionalPopup(this.worldTourScreenConditional.GetNextValidCondition());
        }
        if (screenConditionals != null && !PopUpManager.Instance.isShowingPopUp && !this.waitingForCameraUnZoom && !GarageCameraManager.Instance.IsZoomedIn && !GTAdManager.Instance.isShowing)
        {
            FlowConditionBase nextValidCondition = this.screenConditionals.GetNextValidCondition();
            if (nextValidCondition != null)
            {
                PopUpManager.Instance.TryShowPopUp(nextValidCondition.GetPopup(), PopUpManager.ePriority.Default, null);
            }
        }
        //this.ModeSwitchButton.gameObject.SetActive(!GarageCameraManager.Instance.IsZoomedIn);
        //this.UpdateModeSwitchIcons();
        if (BuildType.IsAppTuttiBuild)
            SetAppTuttiRewardTimerText();

        if (BuildType.IsVasBuild)
            SetVasRewardTimerText();
    }

    private void TryConditionalPopup(FlowConditionBase validCondition)
    {
        if (validCondition != null && !this.popupShown)
        {
            PopUpManager.Instance.TryShowPopUp(validCondition.GetPopup(), PopUpManager.ePriority.Default, null);
            this.popupShown = true;
        }
    }

    public void BringUpgardeBubble()
    {
        Vector3 position = m_upgradeButton.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
        UpgradeButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_UPGRADE", false, position,
            BubbleMessage.NippleDir.DOWN, 0.2F, BubbleMessageConfig.Frontend, true, true
            , m_upgradeButton.rectTransform());
        //vector3 = CommonUI.Instance.NavBar.ShopButton.transform.position;
        //vector3 += new Vector3(-0.16f, -0.4f, 0f);
        //this.ShopButtonBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_SHOP", false, vector3,
        //    BubbleMessage.NippleDir.UP, 0.5f, BubbleMessageConfig.ThemeStyle.SMALL,
        //    BubbleMessageConfig.PositionType.BOX_RELATIVE, 0.16f);
    }

    public void OnSceneChanged()
    {
        HasShownCrewChatter = false;
    }

    public void OnAppTuttiTimedRewardButtonPressed()
    {
        if (AppTuttiTimedRewardManager.Instance.IsTimeForReward())
        {
            AppTuttiTimedRewardManager.Instance.StartFlow();
            SetAppTuttiButtonVisibility(false);
        }
        else
        {
            AppTuttiTimedRewardManager.Instance.ShowTimeRemainingpopup();
        }
    }

    public void OnVasTimedRewardButtonPressed()
    {
        if (VasTimedRewardManager.Instance.IsTimeForReward())
        {
            VasTimedRewardManager.Instance.StartFlow();
            SetVasButtonVisibility(false);
        }
        else
        {
            VasTimedRewardManager.Instance.ShowTimeRemainingpopup();
        }
    }

    public void OnLeaderboardButtonPressed()
    {
#if GT_DEBUG_LOGGING
        if (m_debugLeaderboard)
        {
            ScreenManager.Instance.PushScreen(ScreenID.Leaderboards);
            return;
        }
#endif
        if (!PolledNetworkState.IsNetworkConnected)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup();
        }
        else if (!ServerSynchronisedTime.Instance.IsServerTimeMatchClient)
        {
            PopUpDatabase.Common.ShowServerTimeMismatch();
        }
        else if (LegacyLeaderboardManager.Instance.IsUnlocked())
        {
            ScreenManager.Instance.PushScreen(ScreenID.Leaderboards);
        }
        else
        {
            var unlockLevel = GameDatabase.Instance.CareerConfiguration.LeaderboardUnlockLevel;
            var bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_LEADERBOARD_HIGHER_LEVEL_NEEDED_BODY"),
                unlockLevel);
            PopUpManager.Instance.TryShowPopUp(new PopUp
            {
                Title = "TEXT_POPUPS_LEADERBOARD_HIGHER_LEVEL_NEEDED_TITLE",
                BodyText = bodyText,
                BodyAlreadyTranslated = true,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            }, PopUpManager.ePriority.Default, null);
        }
    }

    public void OnBazaarLeaderboardButtonPressed()
    {
        BazaarGameHubManager.Instance.ShowLastTournamentRanking();
    }

    public override void OnBeforeDeactivate()
    {
        base.OnBeforeDeactivate();
        if (this.NextButtonBubbleMessage != null)
        {
            this.NextButtonBubbleMessage.Dismiss();
            this.NextButtonBubbleMessage = null;
        }
        if (this.UpgradeButtonBubbleMessage != null)
        {
            this.UpgradeButtonBubbleMessage.Dismiss();
            this.UpgradeButtonBubbleMessage = null;
        }
        if (this.ShopButtonBubbleMessage != null)
        {
            this.ShopButtonBubbleMessage.Dismiss();
            this.ShopButtonBubbleMessage = null;
        }
        if (this.CarDealerBubbleMessage != null)
        {
            this.CarDealerBubbleMessage.Dismiss();
            this.CarDealerBubbleMessage = null;
        }
    }


    public void OnAchivementButtonPressed()
    {
        LeftSidePanelContainer.Instance.ShowLeftSidePanel();
    }

    public override bool IgnoreHardwareBackButton()
    {
        var flag = base.IgnoreHardwareBackButton();
        return ShowingIntro || flag;
    }

    public override void RequestBackup()
    {
        if (CommonUI.Instance.IsIn)
        {
            if (LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
            {
                LeftSidePanelContainer.Instance.HideLeftSidePanel();
            }
            else
            {
                base.RequestBackup();
            }
        }
        else
        {
            GarageCameraManager.Instance.OnBackPressed();
        }
    }

    public void OnShowNativeAd()
    {

        if (GTAdManager.Instance.tryGetNativeBanner(out var banner))
        {
            NativeBannerScreen.NativeBannerObject = new NativeBannerBase
            {
                callToActionText = banner.callToActionText,
                Description = banner.Description,
                Icon = banner.Icon,
                landscapeBannerImage = banner.landscapeBannerImage,
                Title = banner.Title,
            };
            ScreenManager.Instance.PushScreen(ScreenID.NativeBanner);
        }

    }

    public static void ResetNativeBannerTime()
    {
        m_lastNativeBannerRequest = Time.time;
    }

    public static void ResetNativeBannerCheck()
    {
        _hasCheckAnyNativeBannerAfterRace = false;
        NativeBannerScreen.NativeBannerObject = null;
        //   GTAdManager.Instance.ClearNativeBannerCache();
    }

    private void SetAppTuttiButtonVisibility(bool active)
    {
        Image[] images = m_appTuttiTimedRewardButton.transform.Find("Button").GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            img.color = new Color(1f, 1f, 1f, active ? 1f : 0.6f);
        }
    }

    private void SetVasButtonVisibility(bool active)
    {
        Image[] images = m_vasTimedRewardButton.transform.Find("Button").GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            img.color = new Color(1f, 1f, 1f, active ? 1f : 0.6f);
        }
    }

    public void SetAppTuttiRewardTimerText()
    {
        string text;
        if (AppTuttiTimedRewardManager.Instance.IsTimeForReward())
        {
            text = "";
        }
        else
        {
            int timetoRerward = AppTuttiTimedRewardManager.Instance.TimeUntilNextReward();
            int minutes = timetoRerward / 60;
            int seconds = timetoRerward % 60;
            text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_MINUTES_AND_SECONDS_WAIT_TIME"), minutes, seconds);
        }

        if (m_appTuttiRewardWaitText.text != text)
        {
            m_appTuttiRewardWaitText.text = text.ToUpper();
        }
    }

    public void SetVasRewardTimerText()
    {
        string text;
        if (VasTimedRewardManager.Instance.IsTimeForReward())
        {
            text = "";
        }
        else
        {
            int timetoRerward = VasTimedRewardManager.Instance.TimeUntilNextReward();
            int minutes = timetoRerward / 60;
            int seconds = timetoRerward % 60;
            text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_MINUTES_AND_SECONDS_WAIT_TIME"), minutes, seconds);
        }

        if (m_vasRewardWaitText.text != text)
        {
            m_vasRewardWaitText.text = text.ToUpper();
        }
    }
}
