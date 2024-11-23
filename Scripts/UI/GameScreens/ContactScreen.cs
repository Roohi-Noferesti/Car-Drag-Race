using I2.Loc;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class ContactScreen : ZHUDScreen
{
    [SerializeField] private RuntimeTextButton m_creditButton;
    [SerializeField] private RuntimeTextButton m_instaButton;
    [SerializeField] private RuntimeTextButton m_supportButton;
    [SerializeField] private RuntimeTextButton m_emailButton;
    [SerializeField] private RuntimeTextButton m_telegramButton;
    [SerializeField] private RuntimeTextButton m_whatsppButton;
    [SerializeField] private RuntimeTextButton m_helpshiftButton;
    [SerializeField] private RuntimeTextButton m_faqButton;
    [SerializeField] private GameObject appTuttiSupport;

    public override ScreenID ID
    {
        get { return ScreenID.Contact; }
    }


    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        m_creditButton.AddValueChangedDelegate(OnCreditButton);
        m_supportButton.AddValueChangedDelegate(OnSupportButton);
        m_instaButton.AddValueChangedDelegate(OnInstagramButton);
        m_emailButton.AddValueChangedDelegate(OnEmailButton);
        m_telegramButton.AddValueChangedDelegate(OnTelegramButtonPressed);
        m_whatsppButton.AddValueChangedDelegate(OnWhatsappButtonPressed);
        m_helpshiftButton.AddValueChangedDelegate(OnHelpshiftButtonPressed);
        m_faqButton.AddValueChangedDelegate(OnFAQButtonPressed);

        if (BasePlatform.ActivePlatform.InsideCountry) {
            m_faqButton.gameObject.SetActive(false);
        } else {
            m_faqButton.gameObject.SetActive(true);
        }
        
        appTuttiSupport.SetActive(false);

        //if (GameDatabase.Instance.SocialConfiguration.UseHelpshift)
        //{
        //    m_helpshiftButton.CurrentState = BaseRuntimeControl.State.Active;
        //    m_supportButton.CurrentState = BaseRuntimeControl.State.Hidden;
        //}
        //else
        //{
        //    m_helpshiftButton.CurrentState = BaseRuntimeControl.State.Hidden;
        //    m_supportButton.CurrentState = BaseRuntimeControl.State.Active;
        //}
    }

    public override void OnDeactivate()
    {
        m_creditButton.RemoveValueChangedDelegate(OnCreditButton);
        m_supportButton.RemoveValueChangedDelegate(OnSupportButton);
        m_instaButton.RemoveValueChangedDelegate(OnInstagramButton);
        m_emailButton.RemoveValueChangedDelegate(OnEmailButton);
        m_telegramButton.RemoveValueChangedDelegate(OnTelegramButtonPressed);
        m_whatsppButton.RemoveValueChangedDelegate(OnWhatsappButtonPressed);
        m_helpshiftButton.RemoveValueChangedDelegate(OnHelpshiftButtonPressed);
        m_faqButton.RemoveValueChangedDelegate(OnFAQButtonPressed);
        base.OnDeactivate();
    }

    public void OnSupportButton()
    {
        var url = GTPlatform.GetPlatformSupportURL();
        Application.OpenURL(url);
    }

    public void OnAppTuttiCloseButton()
    {
        appTuttiSupport.SetActive(false);
    }

    private void OnCreditButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Credits);
    }

    private void OnInstagramButton()
    {
        Log.AnEvent(Events.TapInstagram);
        var url = GTPlatform.GetPlatformInstagramURL();
        Application.OpenURL(url);
    }

    public void OnTelegramButtonPressed()
    {
        Log.AnEvent(Events.TapTelegram);
        var url = GTPlatform.GetPlatformTelegramURL();
        Application.OpenURL(url);
    }

    public void OnWhatsappButtonPressed()
    {
        Log.AnEvent(Events.TapWhatsapp);
        var url = GTPlatform.GetPlatformWhatsappURL();
        Application.OpenURL(url);
    }

    private void OnEmailButton()
    {
        if (!UserManager.Instance.isLoggedIn)
            return;
#if UNITY_IOS
        var myID = "ID=" + UserManager.Instance.currentAccount.UserID;
        string url = string.Format("mailto:gtclub@turnedondigital.com?subject={0}&body={1}"
            , "Help", myID);
        url = url.Replace(" ", "");
        Application.OpenURL(url);
#else
        var myID = string.Format(LocalizationManager.GetTranslation("TEXT_MY_ID"),
            UserManager.Instance.currentAccount.UserID);
        Application.OpenURL(string.Format("mailto:gtclub@turnedondigital.com?subject={0}&body={1}"
            , LocalizationManager.GetTranslation("TEXT_EMAIL_SUBJECT"), myID));
#endif
    }
    
    private void OnHelpshiftButtonPressed()
    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        //HelpshiftManager.Instance.onShowFAQsClick();
//#endif
        BasePlatform.ActivePlatform.InitializeFreshChat();
        var userid = UserManager.Instance.currentAccount.UserID.ToString();
        var username = PlayerProfileManager.Instance.ActiveProfile.DisplayName;
        if (BuildType.IsAppTuttiBuild) {
            appTuttiSupport.SetActive(true);
        } else {
            if (BasePlatform.ActivePlatform.InsideCountry)
            {
                var appVersion = ApplicationVersion.Current;
                //WebViewManager.Instance.Show("https://goftino.com/c/LSjVEn");
                WebViewManager.Instance.Show($"https://kingcodestudio.com/GoftinoGT.html?userName={username}&userId={userid}&appVersion={appVersion}--{BasePlatform.ActivePlatform.GetTargetAppStore()}&device={SystemInfo.deviceModel}--{SystemInfo.deviceName}".Replace(" ", ""));
            }
            else
            {
                var restoreID = UserManager.Instance.currentAccount.FreschatRestoreID;
                BasePlatform.ActivePlatform.ShowFreshChat(userid, username, restoreID);
            }
		}
    }
    
    private void OnFAQButtonPressed()
    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        //HelpshiftManager.Instance.onShowFAQsClick();
//#endif
        BasePlatform.ActivePlatform.InitializeFreshChat();
        if (!BasePlatform.ActivePlatform.InsideCountry)
        {
            var userid = UserManager.Instance.currentAccount.UserID.ToString();
            var restoreID = UserManager.Instance.currentAccount.FreschatRestoreID;
            var username = PlayerProfileManager.Instance.ActiveProfile.DisplayName;
            BasePlatform.ActivePlatform.ShowFreshChatFAQ(userid, username, restoreID);
        }
    }
}
