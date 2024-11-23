using System;
using System.IO;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using Objectives;
using UnityEngine;
using RTLTMPro;
using UnityEngine.UI;

public class HomeScreen : ZHUDScreen
{
    //public InputField achievement_input;
    
    private const string m_MOTDAdSpace = "PH_MOTD_SPACE";

    public bool ShouldTriggerStoryOnAnimationCompleted;

    private static bool _hasShownMOTDSession = false;

    private static DateTime _shownMOTDWhen=GTDateTime.Now;

    private bool HasShownProgress;

    public static bool CheckForDailyPrize;


    [SerializeField]
    private RuntimeTextButton GooglePlayGamesLoginButton;
    [SerializeField]
    private RuntimeTextButton GooglePlayGamesLogoutButton;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.Home;
        }
    }

    //public void achievementButtonClicked()
    //{
    //    SocialGamePlatformSelector.Instance.ReportAchievement((Achievements.GetAchievement(int.Parse(achievement_input.text))));
    //}

    void Start()
    {
        _shownMOTDWhen = GTDateTime.Now;
        ObjectiveCommand.Execute(new LoggedOn(), true);
    }

    private bool ShouldShowMOTDAdSpace()
    {
        if (_hasShownMOTDSession && _shownMOTDWhen == GTDateTime.Now)
        {
            return false;
        }
        if (PlayerProfileManager.Instance == null)
        {
            return false;
        }
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        return activeProfile != null &&
               (activeProfile.HasCompletedFirstThreeTutorialRaces() || activeProfile.EventsCompleted.Count != 0 ||
                activeProfile.HasBoughtFirstCar);
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);

        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile.LegacyObjectivesCompleted.Count > 0)
        {
            string arg = string.Empty;
            foreach (int current in PlayerProfileManager.Instance.ActiveProfile.LegacyObjectivesCompleted)
            {
                arg = arg + current + ", ";
            }
        }
        //AudioManager.SetMute("Audio_SFXAudio", PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute);
        this.ShouldTriggerStoryOnAnimationCompleted = true;
        CommonUI.Instance.XPStats.SetToCurrentKnown();
        CommonUI.Instance.TriggerShowFirstTime();
        activeProfile.ForcePlayerIntoValidOwnedCar();
        if (AppStore.Instance.IsProcessingTransaction)
        {
            AppStore.Instance.ProcessExistingTransactions();
        }
        this.GooglePlayGamesLoginButton.SetText("TEXT_BUTTON_GOOGLE_SIGNIN", false, false);
        this.GooglePlayGamesLogoutButton.SetText("TEXT_BUTTON_GOOGLE_SIGNOUT", false, false);
        this.RefreshGooglePlayGamesButtons();
        this.Update();
        this.ShouldShowWelcomeMessage();
        //OfferWallManager.FetchContent();
        //OfferWallManager.CheckForReward();
        GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
        NotificationManager.Active.RemoveOldLocalNotifications();
        //CSRAdManager.Instance.Precache(AdManager.AdSpace.MOTD);
        if (MapScreenCache.Map != null)
            MapScreenCache.Map.gameObject.SetActive(false);

        //Disable daily reward for ab test
        if (!PopUpManager.Instance.isShowingPopUp && activeProfile.HasCompletedSecondTutorialRace() && activeProfile.NumberOfPrizeCardRemaining > 0 &&
            !CheckForDailyPrize && !BuildType.IsAppTuttiBuild)
        {
            ScreenManager.Instance.PushScreen(ScreenID.PrizeOMatic);
        }
        else
        {
            GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null);
        }

        //ScreenManager.Instance.PushScreen(ScreenID.UserRatingGame);


        //TriggerShowProgress();
        if (PlayerProfileManager.Instance.ActiveProfile != null)
        {
            PlayerProfileManager.Instance.ActiveProfile.CurrentUserChosenLanguage = LocalizationManager.CurrentLanguage;
        }

        if (!BuildType.CanShowGooglePlay())
            HideGooglePlayGamesButtons();
        
        Log.AnEvent(Events.HomeScreenShowed);
        
        if (File.Exists(Application.persistentDataPath + "log.txt"))
            File.Delete(Application.persistentDataPath + "log.txt");
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "log.txt", false);
        writer.WriteLine("OK");
        writer.Close();
    }

    private void ShouldShowWelcomeMessage()
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        int welcomeMessageId = activeProfile.WelcomeMessageId;
        int welcomeMessageId2 = GameDatabase.Instance.CareerConfiguration.WelcomeMessageId;
        if (welcomeMessageId2 == 0)
        {
            return;
        }
        if (welcomeMessageId2 > welcomeMessageId)
        {
            string welcomeMessage = GameDatabase.Instance.CareerConfiguration.WelcomeMessage;
            string welcomeMessageTitle = GameDatabase.Instance.CareerConfiguration.WelcomeMessageTitle;
            string welcomeMessageDynamicImageAsset = GameDatabase.Instance.CareerConfiguration.WelcomeMessageDynamicImageAsset;
            PopUp popUp = new PopUp
            {
                Title = welcomeMessageTitle,
                BodyText = welcomeMessage,
                ConfirmAction = new PopUpButtonAction(this.WelcomeMessageOk),
                ConfirmText = "TEXT_BUTTON_OK",
                ID = PopUpID.WelcomeMessageFromData,
                IsBig = true,
                ImageCaption = "TEXT_NAME_AGENT",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab
            };
            if (!string.IsNullOrEmpty(welcomeMessageDynamicImageAsset))
            {
                popUp.ImageCaption = "TEXT_NAME_FRANKIE";
                popUp.IsDynamicImage = true;
                popUp.DynamicImageAssetID = welcomeMessageDynamicImageAsset;
            }
            PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Default, null);
        }
    }

    private void WelcomeMessageOk()
    {
        int welcomeMessageId = GameDatabase.Instance.CareerConfiguration.WelcomeMessageId;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        activeProfile.WelcomeMessageId = welcomeMessageId;
    }

    private bool OnTryCanLeaveScreen()
    {
        if (PlayerProfileManager.Instance.ActiveProfile == null)
        {
            return false;
        }
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
        CommonUI.Instance.XPStats.XPLockedState(false);
        CommonUI.Instance.StarStats.NewLeagueLockedState(false);
        CommonUI.Instance.StarStats.StarLockedState(false);
        CommonUI.Instance.StarLeagueStats.StarLockedState(false);
        return true;
    }

    public void OnStartButton()
    {
        if (!OnTryCanLeaveScreen())
        {
            return;
        }
        //if (BoostNitrous.CheckAndPushBossChallenge())
        //{
        //    return;
        //}
        PlayerProfileManager.Instance.ActiveProfile.DebugWelcomeDismissed = false;
        if (GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null))
        {
            return;
        }
        ScreenManager.Instance.PushScreen(ScreenID.Workshop);
    }


    public void OnProfileButton()
    {
        //if (BoostNitrous.CheckAndPushBossChallenge())
        //{
        //    return;
        //}
        PlayerProfileManager.Instance.ActiveProfile.DebugWelcomeDismissed = false;
        if (GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null))
        {
            return;
        }

        ProfileScreen.SetUser(PlayerProfileManager.Instance.ActiveProfile.GetProfileData());
        ScreenManager.Instance.PushScreen(ScreenID.Profile);
    }


    public void OnOptionButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Options);
    }

    public void OnContactButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Contact);
    }

    protected override void Update()
    {
        base.Update();
        this.UpdateForCarouselAnim();
        //if (!PopUpManager.Instance.isShowingPopUp && this.ShouldShowMOTDAdSpace() && CSRAdManager.Instance.HasAdvertToDisplay(AdManager.AdSpace.MOTD))
        //{
        //    CSRAdManager.Instance.ForceTriggerAdvert(AdManager.AdSpace.MOTD, null, null);
        //    HomeScreen._hasShownMOTDSession = true;
        //    HomeScreen._shownMOTDWhen = GTDateTime.Now;
        //}

#if UNITY_ANDROID
        if ((GooglePlayGamesController.Instance.IsPlayerAuthenticated() && this.GooglePlayGamesLoginButton.gameObject.activeSelf) || 
            (!GooglePlayGamesController.Instance.IsPlayerAuthenticated() && this.GooglePlayGamesLogoutButton.gameObject.activeSelf))
        {
            this.RefreshGooglePlayGamesButtons();
        }
#endif
    }

    private void UpdateForCarouselAnim()
    {
        if (/*this.CurrentAnimState == CSRScreen.AnimState.IN &&*/ this.ShouldTriggerStoryOnAnimationCompleted)
        {
            this.ShouldTriggerStoryOnAnimationCompleted = false;
            this.TriggerShowProgress();
        }
        //if (CSRCarouselScreen.CarouselInterpolating)
        //{
        //    this.OptionsContainer.transform.localPosition = this.OptionButtonContainerOffset * CSRCarouselScreen.CarouselInterpolator;
        //}
    }

    public void OnGooglePlayGamesSignIn()
    {
#if UNITY_ANDROID
        if (BasePlatform.ActivePlatform.IsSupportedGooglePlayStore() && !GooglePlayGamesController.Instance.IsPlayerAuthenticated())
        {
            GooglePlayGamesController.Instance.AuthenticatePlayer(new Action<bool>(this.Callback_OnGooglePlayGamesAuthentication), false);
        }
#endif
    }

    private void Callback_OnGooglePlayGamesAuthentication(bool success)
    {
#if UNITY_ANDROID
        if (success)
        {
            PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations = 0;
            if (GooglePlayGamesController.Instance.GetPlayerID() != UserManager.Instance.currentAccount.GCid)
            {
                GameCenterController.Instance.DoDeferredIDChange();
            }
            else
            {
                this.RefreshGooglePlayGamesButtons();
                GooglePlayGamesController.Instance.InitialLogin();
            }
        }
#endif
    }

    private void RefreshGooglePlayGamesButtons()
    {
        if (!BuildType.CanShowGooglePlay())
            return;
        
#if UNITY_ANDROID
        if (BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
        {
            bool flag = GooglePlayGamesController.Instance.IsPlayerAuthenticated();
            this.GooglePlayGamesLoginButton.gameObject.SetActive(!flag);
            this.GooglePlayGamesLogoutButton.gameObject.SetActive(false);
        }
        else
        {
            this.GooglePlayGamesLoginButton.gameObject.SetActive(false);
            this.GooglePlayGamesLogoutButton.gameObject.SetActive(false);
        }

#elif UNITY_IOS
        this.GooglePlayGamesLoginButton.gameObject.SetActive(false);
        this.GooglePlayGamesLogoutButton.gameObject.SetActive(false);
#endif
    }

    private void HideGooglePlayGamesButtons()
    {
        this.GooglePlayGamesLoginButton.gameObject.SetActive(false);
        this.GooglePlayGamesLogoutButton.gameObject.SetActive(false);
    }


    public void OnGooglePlayGamesSignOut()
    {
#if UNITY_ANDROID
        if (GooglePlayGamesController.Instance.IsPlayerAuthenticated())
        {
            Color buttonColor;
            ColorUtility.TryParseHtmlString("#FF8400FF", out buttonColor);

            PopUp popUp = new PopUp
            {
                Title = "TEXT_POPUP_GOOGLE_SIGNOUT_TITLE",
                BodyText = "TEXT_POPUP_GOOGLE_SIGNOUT_BODY",
                ConfirmAction = new PopUpButtonAction(this.OnConfirmGooglePlayGamesSignOut),
                ConfirmText = "TEXT_BUTTON_DISCONNECT",
                ConfirmColor = buttonColor,
                UseConfirmColor = true,
                CancelText = "TEXT_BUTTON_CANCEL",
                IsBig = false
            };
            PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Default, null);
        }
#endif
    }

    public void OnConfirmGooglePlayGamesSignOut()
    {
#if UNITY_ANDROID
        GooglePlayGamesController.Instance.SignOut();
        this.RefreshGooglePlayGamesButtons();
#endif
    }


    private void TriggerShowProgress()
    {
        if (this.HasShownProgress)
        {
            return;
        }
        this.HasShownProgress = true;
        ServerIntroMessage.TryShowMessage();
    }
}
