using System;
using System.Collections;
using KingKodeStudio;
//using KingKodeStudio.IAB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : ZHUDScreen
{
    [SerializeField] private TextMeshProUGUI m_versionText;
    [SerializeField] private TextMeshProUGUI m_userIDText;
    [SerializeField] private TextMeshProUGUI m_deviceIDText;
    [SerializeField] private TextMeshProUGUI m_abVersionText;
    [SerializeField] private TextMeshProUGUI m_serverVersion;
    [SerializeField] private Toggle m_highQualityToggle;
    [SerializeField] private Toggle m_optimumQualityToggle;
    [SerializeField] private Toggle m_mileToggle;
    [SerializeField] private Toggle m_kilometerToggle;
    [SerializeField] private RuntimeButton m_sfxMuteButton;
    [SerializeField] private RuntimeButton m_musicMuteButton;
    [SerializeField] private RuntimeButton m_noticiationMuteButton;
    [SerializeField] private RuntimeButton m_clearProfileData;
    //[SerializeField] private RuntimeButton m_paymentMethodButton;
    [SerializeField] private RuntimeButton m_restorePurchaseButton;
    [SerializeField] private RuntimeButton m_selectLanguageButton;
    [SerializeField] private TextMeshProUGUI txtGameCenterAlias;
    private static bool _mutedmusic;
    private static bool _mutedsfx;
    private static bool _notificationsOn = true;
    public GameObject SoundMusicIcon;
    public GameObject SoundFXIcon;
    public GameObject NotificationIcon;
    
    [SerializeField]
    private RuntimeTextButton GooglePlayGamesLoginButton;
    [SerializeField]
    private RuntimeTextButton GooglePlayGamesLogoutButton;
    [SerializeField]
    private RuntimeTextButton AppTuttiPolicyButton;

    public override ScreenID ID
    {
        get { return ScreenID.Options; }
    }

    public static OptionsScreen Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        m_highQualityToggle.isOn = PlayerPrefs.GetInt("_optimumQuality") == 1;
        m_optimumQualityToggle.isOn = PlayerPrefs.GetInt("_optimumQuality") == 0;
        m_mileToggle.isOn = activeProfile.UseMileAsUnit;
        m_kilometerToggle.isOn = !activeProfile.UseMileAsUnit;

        m_sfxMuteButton.AddValueChangedDelegate(OnMuteSFX);
        m_musicMuteButton.AddValueChangedDelegate(OnMuteMusic);
        m_noticiationMuteButton.AddValueChangedDelegate(OnNotifications);
        m_clearProfileData.AddValueChangedDelegate(OnClearProfileData);
        m_restorePurchaseButton.AddValueChangedDelegate(OnRestorePurchase);
        m_highQualityToggle.onValueChanged.AddListener(ToggleHighQuality);
        m_mileToggle.onValueChanged.AddListener(ToggleUnitSetting);
        //m_paymentMethodButton.AddValueChangedDelegate(OnPaymentMethodButton);

//        this.m_paymentMethodButton.CurrentState = BaseRuntimeControl.State.Hidden;
            //KingIAB.Setting.IsGooglePlay && BasePlatform.ActivePlatform.InsideFortumoZone
            //    ? BaseRuntimeControl.State.Active
            //    : BaseRuntimeControl.State.Hidden;
        this.GooglePlayGamesLoginButton.SetText("TEXT_BUTTON_GOOGLE_SIGNIN", false, false);
        this.GooglePlayGamesLogoutButton.SetText("TEXT_BUTTON_GOOGLE_SIGNOUT", false, false);
        this.RefreshGooglePlayGamesButtons();

        m_versionText.text = "Version: " + Application.version;
        m_userIDText.text = "ID: " + UserManager.Instance.currentAccount.UserID;
        var deviceToken = UserManager.Instance.currentAccount.DeviceToken;
        if (string.IsNullOrEmpty(deviceToken))
        {
            deviceToken = NotificationManager.Active.GetUnityDeviceToken();
        }
        m_deviceIDText.text = "UDID: " + deviceToken;
        m_serverVersion.text = "Server V2 : " + Endpoint.UseServerV2;

        if (AssetDatabaseClient.Instance.IsReadyToUse)
        {
            m_abVersionText.text = "Asset Branch : "+AssetDatabaseClient.Instance.Data.GetBranch();
        }
        else
        {
            m_abVersionText.text = "Asset version : not ready";
        }

        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            _mutedmusic = PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute;
            _mutedsfx = PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute;
            _notificationsOn = PlayerProfileManager.Instance.ActiveProfile.OptionNotifications;
        }

        SoundMusicIcon.gameObject.SetActive(!_mutedmusic);
        SoundFXIcon.gameObject.SetActive(!_mutedsfx);
        SetNotifications();

#if UNITY_IOS
        this.txtGameCenterAlias.text = "Game Center ID: " + SocialGamePlatformSelector.Instance.GetCurrentAlias();
#elif UNITY_ANDROID
        this.txtGameCenterAlias.text = "Google Play Games: " + SocialGamePlatformSelector.Instance.GetCurrentAlias();
#endif
        m_selectLanguageButton.CurrentState = BaseRuntimeControl.State.Active;

        if (!BuildType.CanShowGooglePlay())
        {
            HideGooglePlayGamesButtons();
            txtGameCenterAlias.gameObject.SetActive(false);
        }
        
        if(AppTuttiPolicyButton!=null)
            AppTuttiPolicyButton.gameObject.SetActive(BuildType.IsAppTuttiBuild);
    }
    
    private void HideGooglePlayGamesButtons()
    {
        this.GooglePlayGamesLoginButton.gameObject.SetActive(false);
        this.GooglePlayGamesLogoutButton.gameObject.SetActive(false);
    }

    //private void OnPaymentMethodButton()
    //{
    //    PopUp popUp = new PopUp();
    //    popUp.Title = "TEXT_POPUP_FORTUMO_TITLE";
    //    popUp.BodyText = "TEXT_POPUP_FORTUMO_BODY";
    //    popUp.CancelText = "TEXT_POPUP_FORTUMO_CONFIRM";
    //    popUp.ConfirmText = "TEXT_POPUP_FORTUMO_CANCEL";
    //    popUp.CancelAction = () =>
    //    {
    //        AppStore.Instance.SwitchAppStoreToFortumo();
    //        UserManager.Instance.currentAccount.IsFortumo = true;
    //        SaveAppStoreSetting();
    //    };
    //    popUp.ConfirmAction = () =>
    //    {
    //        AppStore.Instance.SwitchAppStoreToGooglePlay();
    //        UserManager.Instance.currentAccount.IsFortumo = false;
    //        SaveAppStoreSetting();
    //    };

    //    PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Default, null);
    //}


    private void SaveAppStoreSetting()
    {
        var currentAccount = UserManager.Instance.currentAccount;
        currentAccount.HasChosenBaseStoreOrFortumo = true;
        UserManager.Instance.SaveCurrentAccount();
        JsonDict AccountParams = new JsonDict();
        AccountParams.Set("username", "user" + currentAccount.UserID);
        AccountParams.Set("is_fortumo", currentAccount.IsFortumo);
        AccountParams.Set("has_chosen_base_or_fortumo", currentAccount.HasChosenBaseStoreOrFortumo);
        WebRequestQueue.Instance.StartCall("save_fortumo_settings", "Save user Data", AccountParams, null, null, ProduceHashSource(AccountParams));
    }


    private string ProduceHashSource(JsonDict dict)
    {
        string text = string.Empty;
        foreach (string current in dict.Keys)
        {
            text += dict.GetString(current);
        }
        return text;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        m_sfxMuteButton.RemoveValueChangedDelegate(OnMuteSFX);
        m_musicMuteButton.RemoveValueChangedDelegate(OnMuteMusic);
        m_noticiationMuteButton.RemoveValueChangedDelegate(OnNotifications);
        m_clearProfileData.RemoveValueChangedDelegate(OnClearProfileData);
        m_highQualityToggle.onValueChanged.RemoveListener(ToggleHighQuality);
    }

    private void OnClearProfileData()
    {
        PopUp popUp = new PopUp();
        popUp.Title = "TEXT_POPUP_CLEAR_DATA_TITLE";
        popUp.BodyText = "TEXT_POPUP_CLEAR_DATA_BODY";
        popUp.CancelText = "TEXT_BUTTON_CANCEL";
        popUp.ConfirmText = "TEXT_CLEAR_DATA";
        popUp.ConfirmAction = delegate
        {
            FileUtils.EraseLocalStorage();
            if (!Caching.ClearCache())
            {
            }
            AssetSystemManager.Instance.KickBackToSafePlaceAndReload(AssetSystemManager.Reason.AssetDBChanged);
            AssetDatabaseVersionPoll.Instance.PollNow();
        };
        PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Default, null);
    }

    public void ToggleHighQuality(bool value)
    {
        if (value)
        {
            SetToHighQuality();
        }
        else
        {
            SetToOptimumQuality();
        }
    }


    public void ToggleUnitSetting(bool value)
    {
        PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit = value;
    }


    private void SetToHighQuality()
    {
        PlayerPrefs.SetInt("_optimumQuality", 1);
        BaseDevice.ActiveDevice.SetToHighQuality();
    }

    private void SetToOptimumQuality()
    {
        PlayerPrefs.SetInt("_optimumQuality", 0);
        BaseDevice.ActiveDevice.SetToOptimumQuality();
    }

    public void OnMuteMusic()
    {
        _mutedmusic = !_mutedmusic;
        MenuAudio.Instance.setMuteMusic(_mutedmusic);
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute = _mutedmusic;
        }
        //this.MuteMusicButton.SetText((!OptionsScreen._mutedmusic) ? "TEXT_BUTTON_MUTE_MUSIC" : "TEXT_BUTTON_UNMUTE_MUSIC", false, true);
        SoundMusicIcon.gameObject.SetActive(!_mutedmusic);
    }

    public void OnMuteSFX()
    {
        _mutedsfx = !_mutedsfx;
        AudioManager.Instance.SetMute("Audio_SFXAudio", _mutedsfx);
        if (PlayerProfileManager.Instance != null)
        {
            PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute = _mutedsfx;
        }
        //this.MuteSFXButton.SetText((!OptionsScreen._mutedsfx) ? "TEXT_BUTTON_MUTE_SOUND_EFFECTS" : "TEXT_BUTTON_UNMUTE_SOUND_EFFECTS", false, true);
        SoundFXIcon.gameObject.SetActive(!_mutedsfx);
    }


    public void OnNotifications()
    {
        _notificationsOn = !_notificationsOn;
        if (PlayerProfileManager.Instance != null)
        {
            PlayerProfileManager.Instance.ActiveProfile.OptionNotifications = _notificationsOn;
        }
        SetNotifications();
    }

    public void SetNotifications()
    {
        if (!_notificationsOn)
        {
            NotificationManager.Active.ClearAllNotifications();
        }
        NotificationIcon.gameObject.SetActive(_notificationsOn);
    }


    public void OnTwitterButton()
    {
        SocialController.Instance.SendInviteTweetAction(null);
    }
    
    public void OnRestorePurchase()
    {
        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Restore, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Automatic);
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
    }

    public void OnLanguageButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Language);
    }

    public void OnContactButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Contact);
    }
    
    public void OnAppTuttiPolicyButton()
    {
        WebViewManager.Instance.Show("https://www.apptutti.cn/cbc/agreement.html");
    }

    protected override void Update()
    {
        base.Update();
#if UNITY_ANDROID
        if (BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
        {
            if ((GooglePlayGamesController.Instance.IsPlayerAuthenticated() && this.GooglePlayGamesLoginButton.gameObject.activeSelf) ||
                (!GooglePlayGamesController.Instance.IsPlayerAuthenticated() && this.GooglePlayGamesLogoutButton.gameObject.activeSelf))
            {
                this.RefreshGooglePlayGamesButtons();
            }
        }
#endif
    }
    
    
    public void OnGooglePlayGamesSignIn()
    {
#if UNITY_ANDROID
        if (!GooglePlayGamesController.Instance.IsPlayerAuthenticated())
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
                //this.RefreshGooglePlayGamesButtons();
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
            this.GooglePlayGamesLogoutButton.gameObject.SetActive(flag);
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

    public override void RequestBackup()
    {
        base.RequestBackup();
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }
}
