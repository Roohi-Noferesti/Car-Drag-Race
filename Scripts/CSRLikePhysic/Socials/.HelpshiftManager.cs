using System.Collections;
using System.Collections.Generic;
//using Helpshift;
using UnityEngine;

public class HelpshiftManager : MonoBehaviour
{
	public static HelpshiftManager Instance { get; private set; }


#if !UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS
    private HelpshiftSdk _support;
    public void updateMetaData(string nothing)
    {
        Debug.Log("Update metadata ************************************************************");
        Dictionary<string, object> configMap = new Dictionary<string, object>();
        configMap.Add("user-level", "21");
        configMap.Add("hs-tags", new string[] { "Tag-1" });
        _support.updateMetaData(configMap);
    }

    public void helpshiftSessionBegan(string message)
    {
        Debug.Log("Helpshift Support Session Began ************************************************************");
    }

    public void helpshiftSessionEnded(string message)
    {
        Debug.Log("Helpshift Support Session ended ************************************************************");
    }

    public void alertToRateAppAction(string result)
    {
        Debug.Log("User action on alert :" + result);
    }

    public void didReceiveNotificationCount(string count)
    {
        Debug.Log("Notification async count : " + count);
    }

    public void didReceiveInAppNotificationCount(string count)
    {
        Debug.Log("In-app Notification count : " + count);
    }

    public void conversationEnded()
    {
        Debug.Log("Helpshift conversation ended.");
    }

    public void didReceiveUnreadMessagesCount(string count)
    {
        Debug.Log("Helpshift Unread message count : " + count);
    }

    public void didCheckIfConversationActive(string active)
    {
        Debug.Log("Helpshift conversation active status : " + active);
    }

    /// <summary>
    /// Conversation delegates
    /// </summary>

    public void newConversationStarted(string message)
    {
        Debug.Log("Helpshift conversation started.");
    }

    public void userRepliedToConversation(string newMessage)
    {
        Debug.Log("Helpshift user replied to conversation.");
    }

    public void userCompletedCustomerSatisfactionSurvey(string json)
    {
//        Dictionary<string, object> csatInfo = (Dictionary<string, object>)Json.Deserialize(json);
//        Debug.Log("Customer satisfaction information : " + csatInfo);
    }

    public void authenticationFailed(string serializedJSONUserData)
    {
        HelpshiftUser user = HelpshiftJSONUtility.getHelpshiftUser(serializedJSONUserData);
        HelpshiftAuthFailureReason reason = HelpshiftJSONUtility.getAuthFailureReason(serializedJSONUserData);
        Debug.Log("Authentication failed : " + user.identifier + " " + user.authToken + " , Reason : " + reason.ToString());
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _support = HelpshiftSdk.getInstance();
        UserManager.LoggedInEvent += User_LoggedIn;
        string apiKey = "a7129b863cb671696a06575359ae66ff";
        string domainName = "kingkode.helpshift.com";
        string appId;
#if UNITY_ANDROID
		appId = "kingkode_platform_20190723051340066-88d2bdbac7dbefc";
#elif UNITY_IOS
                appId = "your_ios_app_id";
#endif
        _support.install(apiKey, domainName, appId, getInstallConfig());
    }

    private void User_LoggedIn()
    {
        var account = UserManager.Instance.currentAccount;
        if(_support!=null && account.UserID!=0)
        {
            _support.requestUnreadMessagesCount(true);
            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            if (activeProfile != null)
            {
                HelpshiftUser user = new HelpshiftUser.Builder (account.UserID.ToString(),"" )
                    .setAuthToken ("")
                    .setName (!string.IsNullOrEmpty(activeProfile.DisplayName)?activeProfile.DisplayName:activeProfile.Name)
                    .build ();

                _support.login (user);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID
        _support.registerDelegates();
#endif

    }

    public void onShowFAQsClick()
    {
        Debug.Log("Show FAQs clicked !!");
        _support.showFAQs();
    }
    public void onCustomContactUsClick()
    {
        Dictionary<string, object>[] flows = getDynamicFlows();
        Dictionary<string, object> faqConfig = new Dictionary<string, object>();
        faqConfig.Add(HelpshiftSdk.HsCustomContactUsFlows, flows);
        _support.showFAQs(faqConfig);
    }

    protected Dictionary<string, object>[] getDynamicFlows()
    {
        Dictionary<string, object> conversationFlow = new Dictionary<string, object>();
        conversationFlow.Add(HelpshiftSdk.HsFlowType, HelpshiftSdk.HsFlowTypeConversation);
        conversationFlow.Add(HelpshiftSdk.HsFlowTitle, "Converse");
        Dictionary<string, object> conversationConfig = new Dictionary<string, object>();
        conversationConfig.Add("conversationPrefillText", "This is from dynamic");
        conversationConfig.Add("hideNameAndEmail", "yes");
        conversationConfig.Add("showSearchOnNewConversation", "yes");
        conversationFlow.Add(HelpshiftSdk.HsFlowConfig, conversationConfig);

        Dictionary<string, object> faqsFlow = new Dictionary<string, object>();
        faqsFlow.Add(HelpshiftSdk.HsFlowType, HelpshiftSdk.HsFlowTypeFaqs);
        faqsFlow.Add(HelpshiftSdk.HsFlowTitle, "FAQs");

        Dictionary<string, object> faqSectionFlow = new Dictionary<string, object>();
        faqSectionFlow.Add(HelpshiftSdk.HsFlowType, HelpshiftSdk.HsFlowTypeFaqSection);
        faqSectionFlow.Add(HelpshiftSdk.HsFlowTitle, "FAQ section");
        faqSectionFlow.Add(HelpshiftSdk.HsFlowData, "1509");

        Dictionary<string, object> faqFlow = new Dictionary<string, object>();
        faqFlow.Add(HelpshiftSdk.HsFlowType, HelpshiftSdk.HsFlowTypeSingleFaq);
        faqFlow.Add(HelpshiftSdk.HsFlowTitle, "FAQ");
        faqFlow.Add(HelpshiftSdk.HsFlowData, "2998");

        Dictionary<string, object> nestedFlow = new Dictionary<string, object>();
        nestedFlow.Add(HelpshiftSdk.HsFlowType, HelpshiftSdk.HsFlowTypeNested);
        nestedFlow.Add(HelpshiftSdk.HsFlowTitle, "Next form");
        nestedFlow.Add(HelpshiftSdk.HsFlowData, new Dictionary<string, object>[]
        {
            conversationFlow,
            faqsFlow,
            faqSectionFlow,
            faqFlow
        });

        Dictionary<string, object>[] flows = new Dictionary<string, object>[] {
            conversationFlow,
            faqsFlow,
            faqSectionFlow,
            faqFlow,
            nestedFlow
        };

        return flows;
    }

    public void onShowDynamicClick()
    {
        _support.showDynamicForm("This is a dynamic form", getDynamicFlows());
    }

    public void onShowConversationClick()
    {
        Debug.Log("Show Conversation clicked !!");
#if UNITY_ANDROID
        _support.showConversation(getApiConfig());
#elif UNITY_IOS
        _support.showConversation();
#endif
    }

    public void onShowFAQSectionClick()
    {
//        GameObject inputFieldGo = GameObject.FindGameObjectWithTag("faq_section_id");
//        InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
//        try
//        {
//            Convert.ToInt16(inputFieldCo.text);
//            _support.showFAQSection(inputFieldCo.text);
//        }
//        catch (FormatException e)
//        {
//            Debug.Log("Input string is not a sequence of digits : " + e);
//        }
    }

    public void onShowFAQClick()
    {
//        GameObject inputFieldGo = GameObject.FindGameObjectWithTag("faq_id");
//        InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
//        try
//        {
//            Convert.ToInt16(inputFieldCo.text);
//            _support.showSingleFAQ(inputFieldCo.text);
//        }
//        catch (FormatException e)
//        {
//            Debug.Log("Input string is not a sequence of digits : " + e);
//        }
    }

    public void onShowReviewReminderClick()
    {
#if UNITY_IOS
        _support.showAlertToRateAppWithURL("itms-apps://itunes.apple.com/app/id460171653");
#elif UNITY_ANDROID
        _support.showAlertToRateAppWithURL("market://details?id=com.kingkodestudio.z2h");
#endif
    }

    /**
        Helper method to build config dictionary for Helpshift Unity SDK FAQ/Conversation APIs
     */
    private Dictionary<string, object> getApiConfig()
    {
        Dictionary<string, object> configDictionary = new Dictionary<string, object>();
        /*
        Possible values:
        CONTACT_US_ALWAYS, CONTACT_US_NEVER, CONTACT_US_AFTER_VIEWING_FAQS, CONTACT_US_AFTER_MARKING_ANSWER_UNHELPFUL
         */
        configDictionary.Add("enableContactUs", HelpshiftSdk.CONTACT_US_AFTER_VIEWING_FAQS);
        configDictionary.Add("gotoConversationAfterContactUs", "no"); // Possible options:  "yes", "no"
        configDictionary.Add("requireEmail", "no"); // Possible options:  "yes", "no"
        configDictionary.Add("hideNameAndEmail", "no"); // Possible options:  "yes", "no"
        configDictionary.Add("enableFullPrivacy", "no"); // Possible options:  "yes", "no"
        configDictionary.Add("showSearchOnNewConversation", "no"); // Possible options:  "yes", "no"
        configDictionary.Add("showConversationResolutionQuestion", "yes"); // Possible options:  "yes", "no"
        configDictionary.Add("enableTypingIndicator", "yes"); // Possible options:  "yes", "no"
        configDictionary.Add("showConversationInfoScreen", "yes"); // Possible options:  "yes", "no"
        return configDictionary;
    }

    /**
        Helper method to build config dictionary for Helpshift Unity SDK Install API
     */
    private Dictionary<string, object> getInstallConfig()
    {
        Dictionary<string, object> installDictionary = new Dictionary<string, object>();
        installDictionary.Add("unityGameObject", "background_image");
        installDictionary.Add("enableInAppNotification", "yes"); // Possible options:  "yes", "no"
        installDictionary.Add("enableDefaultFallbackLanguage", "yes"); // Possible options:  "yes", "no"
        installDictionary.Add("disableEntryExitAnimations", "no"); // Possible options:  "yes", "no"
        installDictionary.Add("enableInboxPolling", "no"); // Possible options:  "yes", "no"
        installDictionary.Add("enableLogging", "yes"); // Possible options:  "yes", "no"
        installDictionary.Add("screenOrientation", -1); // Possible options:  SCREEN_ORIENTATION_LANDSCAPE=0, SCREEN_ORIENTATION_PORTRAIT=1, SCREEN_ORIENTATION_UNSPECIFIED = -1
        return installDictionary;
    }
#endif
}
