using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using KingKodeStudio.IAB;
using UnityEngine;

public abstract class BasePlatform
{
    private const GTAppStore APP_STORE_NAME = GTAppStore.None;

    protected bool? m_isAppTuttiBuild;
    protected bool? m_isVasBuild;
    protected bool? m_insideCountry;
    
    protected bool? m_insideIslamicCountry;
    
    protected bool? m_shouldOverwriteAdPriority;
    
    protected bool? m_insideIAPFocusedCountry;
    protected bool? m_insideAdFocusedCountry;

    protected string m_remoteAdPriorities = "";

    protected string m_BranchPostfix = "";

    protected string m_PossibleStores = "";
    
    protected bool m_IsBranchActive = false;
    
    protected string m_SuccessfullyIAPAbTestAssigning = "";

    protected string m_timeZone;

    private bool? m_insideFortumoZone;
    

    //private CurrenciesFortumo currencies;
    
    private string[] islamicCountries = new string[] {String.Format("{0}{1}{2}", "te", "h", "ran"), "baghdad", "riyadh", "aden", "dubai", "muscat", "kuwait", "qatar", "bahrain", "kabul", "karachi", "cairo", "khartoum", "juba", "amman", "damascus", "tripoli", "dhaka"};
    private string[] adpriorityCountries = new string[] {String.Format("{0}{1}{2}", "te", "h", "ran"), "baghdad", "riyadh", "aden", "dubai", "muscat", "kuwait", "qatar", "bahrain", "kabul", "karachi", "cairo", "khartoum", "juba", "amman", "damascus", "tripoli", "dhaka"};

    public enum eReachability
    {
        OFFLINE,
        WWAN,
        WIFI
    }

    public enum eSigningType
    {
        Client_Everything,
        Server_Accounts,
        Server_RTW,
        Server_RYF
    }

    public enum eAudioMode
    {
        iPodMusic,
        gameMusic
    }

    public static readonly BasePlatform ActivePlatform;
    protected string m_version;

    static BasePlatform()
    {
#if UNITY_EDITOR
        ActivePlatform = new StandalonePlatform();
#elif UNITY_ANDROID
        ActivePlatform = new AndroidPlatform();
#elif UNITY_IPHONE
	    ActivePlatform = new IOSPlatform();
#endif

    }

    public static GTPlatforms TargetPlatform
    {
        get { return GTPlatform.Target; }
    }

    public virtual string GetBundleIdentifier()
    {
        return "BASE GetAppBundleIdentifier";
    }

    public virtual eReachability GetReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                return eReachability.OFFLINE;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return eReachability.WWAN;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                return eReachability.WIFI;
        }

        return eReachability.OFFLINE;
    }

    public virtual string GetMacAddress()
    {
        return String.Empty;
    }

    public virtual void Initialise()
    {
        m_version = Application.version;
        // if (!InsideCountry)
        // {
        //     InitializeFreshChat();
        // }
    }

    public virtual string GetLanguages(int number)
    {
        return "fa";
    }

    public virtual string GetCurrentLocale()
    {
        return "fa-IR";
    }

    public virtual GTAppStore GetTargetAppStore()
    {
        return GTAppStore.None;
    }

    public virtual bool IsSupportedGooglePlayStore()
    {
        if (!BuildType.CanShowGooglePlay())
            return false;
        
        return true;
    }

    public virtual string GetTargetAppStoreFriendlyName()
    {
        return String.Empty;
    }

    public virtual bool HasZLIB()
    {
        return false;
    }

    public virtual bool ZLIBCompressTextToFileX(string destinationPath, string text)
    {
        return false;
    }

    public virtual string ZLIBDecompressTextFromFileX(string path)
    {
        return null;
    }

    public virtual string DecompressTextFromCompressedBase64Data(string compressedData)
    {
        return null;
    }

    public virtual string GetSecretName()
    {
        //return string.Empty;
        return "secret_name";
    }
    
    public virtual string HMACSHA1_Hash(string zSource, eSigningType type)
    {
        byte[] key = Encoding.ASCII.GetBytes(type.ToString());
        HMACSHA1 myhmacsha1 = new HMACSHA1(key);
        byte[] byteArray = Encoding.ASCII.GetBytes(zSource);
        MemoryStream stream = new MemoryStream(byteArray);
        return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
    }

    protected static string ByteToString(byte[] buff)
    {
        string text = String.Empty;
        for (int i = 0; i < buff.Length; i++)
        {
            text += buff[i].ToString("x2");
        }

        return text;
    }

    public virtual int GetOSUpTime()
    {
        return 0;
    }

    public virtual string GetApplicationVersion()
    {
        if (string.IsNullOrEmpty(m_version))
        {
            m_version = Application.version;
        }
        return m_version; //Application.version;
    }

    public virtual bool KeychainIsPluginEnabled()
    {
        return true;
    }

    public virtual void KeychainCreateWrapper(string ident)
    {
        KeychainFake.CreateWrapper(ident);
    }

    public virtual string KeychainReadString(eKeychain key)
    {
        return KeychainFake.ReadString(key);
    }

    public virtual void KeychainWriteString(eKeychain key, string value)
    {
        KeychainFake.WriteString(key, value);
    }

    public virtual void KeychainDestroyWrapper()
    {
        KeychainFake.DestroyWrapper();
    }

    public virtual bool AddSkipBackupAttributeToItem(string filePath)
    {
        return true;
    }

    public virtual string TX(string inputString)
    {
        if (string.IsNullOrEmpty(inputString))
            return "";
        
        UTF8Encoding uTF8Encoding = new UTF8Encoding();
        byte[] bytes = uTF8Encoding.GetBytes(inputString);
        int num = bytes.Length;
        for (int i = 0; i < num; i++)
        {
            if (i == 0)
            {
                bytes[i] ^= 116;
            }
            else
            {
                bytes[i] ^= bytes[i - 1];
            }
        }

        return Convert.ToBase64String(bytes);
    }

    public virtual string FX(string base64)
    {
        byte[] array = Convert.FromBase64String(base64);
        int num = array.Length;
        for (int i = num - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                array[i] ^= 116;
            }
            else
            {
                array[i] ^= array[i - 1];
            }
        }

        UTF8Encoding uTF8Encoding = new UTF8Encoding();
        return uTF8Encoding.GetString(array, 0, array.Length);
    }

    public virtual void Alert(string message)
    {
    }

    public virtual bool CanSendEmail()
    {
        return false;
    }

    public virtual bool SendEmail(string contents, string subject)
    {
        return true;
    }

    public virtual bool GetSendEmailSuccessful()
    {
        return false;
    }

    public virtual void GetDoesLikeUsOnFacebook()
    {
    }

    public virtual void PerformLikeUsOnFacebook()
    {
    }

    public virtual void SendInvitationRequestsToFacebook(string title, string message)
    {
    }

    public virtual bool PerformFacebookSSO(string accessToken, string expirationDate, List<string> userPermissions)
    {
        return true;
    }

    public virtual bool TokensAreValid(string accessToken, string expiryDate)
    {
        return true;
    }

    public virtual void FacebookLogout()
    {
    }

    public virtual bool PostToFacebook(string name, string caption, string description, string link, string picture)
    {
        return true;
    }

    public virtual void GetFacebookUserInfo()
    {
    }

    public virtual void GetFacebookTokenForBusiness()
    {
    }

    public virtual void GetFacebookUserPermissions()
    {
    }

    public virtual void RequestFacebookUserPermissions(List<string> permissions)
    {
    }

    public virtual void RequestFacebookUserFriendsPermissions(List<string> permissions)
    {
    }

    public virtual void RevokeFacebookUserPermission(string permission)
    {
    }

    public virtual void GetFacebookFriendsList(int limit)
    {
    }

    public virtual void GetFacebookFriendProfilePic(string friendID)
    {
    }

    public virtual bool CanSendSMS()
    {
        return false;
    }

    public virtual bool SendSMS(string contents)
    {
        return true;
    }

    public virtual bool GetSendSMSSuccessful()
    {
        return false;
    }

    public virtual bool UsesNativeSharing()
    {
        return false;
    }

    public virtual bool ShareImage(string caption, string contents, string imageUrl, string message = null)
    {
        return false;
    }

    public virtual bool ShareText(string caption, string contents, string url, string image)
    {
        return false;
    }

    public virtual void FollowUsOnTwitter()
    {
    }

    public virtual void HasFollowedUsOnTwitter()
    {
    }

    public virtual bool CanSendTweet()
    {
        return false;
    }

    public virtual bool SendTweet(string contents, string url, string image)
    {
        return true;
    }

    public virtual int TweetGetResolution()
    {
        return -1;
    }

    public virtual bool CopyImageToCameraRoll(string image)
    {
        return true;
    }

    public virtual string GetApplicationName()
    {
        return String.Empty;
    }

    public virtual string GetDeviceOS()
    {
        return String.Empty;
    }

    public virtual string GetDeviceOSVersion()
    {
        return String.Empty;
    }

    public virtual string GetDeviceType()
    {
        return String.Empty;
    }

    public virtual string GetCurrentCurrencySymbol()
    {
        return String.Empty;
    }

    public virtual string GetDeviceSerial()
    {
        return "NOT_IMPLEMENTED";
    }

    public virtual string GetDeviceModel()
    {
        return SystemInfo.deviceModel;
    }

    public virtual string GetTelephonyID()
    {
        return "NOT_IMPLEMENTED";
    }

    public virtual string GetSecureAndroidID()
    {
        return "NOT_IMPLEMENTED";
    }

    public virtual void ShowPickerWheel()
    {
    }

    public virtual void HidePickerWheel()
    {
    }

    public virtual void BeginPopulatePickerWheel(int itemCount)
    {
    }

    public virtual void EndPopulatePickerWheel()
    {
    }

    public virtual void SetPickerWheelItem(int index, string name)
    {
    }

    public virtual void SetPickerWheelLocalisedStrings(string next, string prev, string done)
    {
    }

    public virtual void SetPickerWheelSelection(int selection)
    {
    }

    public virtual void PrepareRenderTexture(RenderTexture rt)
    {
    }

    public virtual string GetDefaultBranchName()
    {
#if UNITY_ANDROID
        return "Default_Android";
#elif UNITY_IOS
			return "Default_IOS";
#endif
    }

    public virtual void InitialisationPreProcess()
    {
    }

    public virtual void CallOnApplicationFocus(bool isFocussed)
    {
    }

    public virtual void CallOnApplicationQuit()
    {
    }

    public virtual void SetMusicMode(eAudioMode mode)
    {
    }


    public virtual string GetTimeZone()
    {
        return "india";
    }


    public virtual void InitializeFreshChat()
    {

    }

    public virtual bool ShouldShowFemaleHair
    {
        get
        {
            return !InsideIslamicCountry;
        }
    }

    public virtual bool InsideCountry
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            if (!m_insideCountry.HasValue)
            {
                //bool isDebugBuild = Debug.isDebugBuild;
                bool isDebugBuild = false;
                if (isDebugBuild)
                {
                    m_insideCountry = DebugManager.Instance.IsInsideCountry();
                }
                else
                {
                    if (String.IsNullOrEmpty(m_timeZone))
                    {
                        m_timeZone = GetTimeZone().ToLower();
                        Debug.Log("Time zone is : " + m_timeZone);
                    }

                    if (String.IsNullOrEmpty(m_timeZone))
                    {
                        Debug.Log("Inside country return false");
                        return false;
                    }

                    var myCountryName = String.Format("{0}{1}{2}", "i", "r", "an");
                    var myCountryLocalName = String.Format("{0}{1}{2}", "ای", "ر", "ان");
                    var myCountryArabicName = String.Format("{0}{1}{2}", "إي", "ر", "ان");
                    var myCityName = String.Format("{0}{1}{2}", "te", "h", "ran");
                    if (m_timeZone.Contains(myCountryName) || m_timeZone.Contains(myCountryLocalName) || m_timeZone.Contains(myCountryArabicName) ||
                        m_timeZone.Contains(myCityName))
                    {
                        Debug.Log("Inside country : true");
                        m_insideCountry = true;
                    }
                    else
                    {
                        m_insideCountry = false;
                    }
                }
            }

            return m_insideCountry.Value;
#endif
        }
    }
    
    public virtual bool InsideIslamicCountry
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            //bool isDebugBuild = Debug.isDebugBuild;
            bool isDebugBuild = false;
            if (isDebugBuild)
            {
                m_insideIslamicCountry = DebugManager.Instance.IsInsideCountry();
            }
            else
            {
                if (!m_insideIslamicCountry.HasValue)
                {
                    string city = GetCity().ToLower();
                    
                    if (InsideCountry || islamicCountries.Contains(city)) {
                        Debug.Log("Inside islamic country local set: true");
                        m_insideIslamicCountry = true;
                    } else {
                        m_insideIslamicCountry = false;
                    }
                }
            }
            return m_insideIslamicCountry.Value;
#endif
        }
    }
    
    public virtual bool ShouldOverwriteAdPriority
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            if (!m_shouldOverwriteAdPriority.HasValue)
            {
                string city = GetCity().ToLower();
                
                if (adpriorityCountries.Contains(city)) {
                    Debug.Log("ShouldOverwriteAdPriority local set: true");
                    m_shouldOverwriteAdPriority = true;
                } else {
                    m_shouldOverwriteAdPriority = false;
                }
            }
            return m_shouldOverwriteAdPriority.Value;
#endif
        }
    }
    
    public virtual bool InsideIAPFocusedCountry
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            if (!m_insideIAPFocusedCountry.HasValue)
            {
                m_insideIAPFocusedCountry = false;
            }
            return m_insideIAPFocusedCountry.Value;
#endif
        }
    }
    
    public virtual bool InsideAdFocusedCountry
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            if (!m_insideAdFocusedCountry.HasValue)
            {
                m_insideAdFocusedCountry = false;
            }
            return m_insideAdFocusedCountry.Value;
#endif
        }
    }

    public bool HasRemoteAdPriorityBeenSet()
    {
        return !string.IsNullOrEmpty(m_remoteAdPriorities);
    }
    
    public bool HasBranchPostfixSet()
    {
        return !string.IsNullOrEmpty(m_BranchPostfix);
    }
    
    public bool HasPossibleStoresSet()
    {
        return !string.IsNullOrEmpty(m_PossibleStores);
    }
    
    public bool HasAssignedIAPAbTestSuccessfully()
    {
        return !string.IsNullOrEmpty(m_SuccessfullyIAPAbTestAssigning);
    }
    
    public string GetPostFixBranch
    {
        get => m_BranchPostfix;
        set => m_BranchPostfix = value;
    }
    
    public string GetPossibleStore
    {
        get => m_PossibleStores;
        set => m_PossibleStores = value;
    }
    
    public void GetBranchPostfix(string value)
    {
        m_BranchPostfix = value;
    }
    
    public void GetPossibleStores(string value)
    {
        m_PossibleStores = value;
    }
    
    public void GetBranchActivity(bool value)
    {
        m_IsBranchActive = value;
    }
    
    public void SuccessfullyIAPAbTestAssigning(string value)
    {
        m_SuccessfullyIAPAbTestAssigning = value;
    }

    public int GetRemoteAdPriority(string name)
    {
        int i = 1;
        var adProviders = m_remoteAdPriorities.ToLower().Split(',');
        foreach (var adProvider in adProviders)
        {
            if (adProvider == name.ToLower())
                return i;
            i++;
        }
        return 0;
    }

    public virtual string GetCountryCode()
    {
        return "NOT SET";
    }
    
    public virtual string GetCity()
    {
        return "NOT SET";
    }
    
    public virtual void ShowFreshChatFAQ(string userid,string name, string restoreID)
    {

    }

    public virtual void ShowFreshChat(string userid,string name, string restoreID)
    {

    }

    public void UpdateInsideIslamicCountryValue(bool value)
    {
        Debug.Log("InsideIslamicCountry : " + value);
        m_insideIslamicCountry = value;
    }
    
    public void UpdateShouldOverwriteAdPriorityValue(bool value)
    {
        Debug.Log("ShouldOverwriteAdPriority : " + value);
        m_shouldOverwriteAdPriority = value;
    }
    
    public void UpdateInsideIAPFocusedCountryValue(bool value)
    {
        Debug.Log("InsideIAPFocusedCountry : " + value);
        m_insideIAPFocusedCountry = value;
    }
    
    
    public void UpdateInsideAdFocusedCountryValue(bool value)
    {
        Debug.Log("InsideAdFocusedCountry : " + value);
        m_insideAdFocusedCountry = value;
    }
    
    public void UpdateRemoteAdPriorities(string value)
    {
        Debug.Log("RemoteAdPriorities : " + value);
        m_remoteAdPriorities = value.ToLower().Replace(" ", "");
    }
    
    /*public void UpdateInsideIslamicCountryValue(string value)
    {
        islamicCountries = value.ToLower().Split(',');
        string city = GetCity().ToLower();
        if (islamicCountries.Contains(city)) {
            Debug.Log("InsideIslamicCountry : true");
            m_insideIslamicCountry = true;
        } else {
            Debug.Log("InsideIslamicCountry : false");
            m_insideIslamicCountry = false;
        }
    }
    
    public void UpdateAdPriorityCountryValue(string value)
    {
        adpriorityCountries = value.ToLower().Split(',');
        string city = GetCity().ToLower();
        if (adpriorityCountries.Contains(city)) {
            Debug.Log("ShouldOverwriteAdPriority : true");
            m_shouldOverwriteAdPriority = true;
        } else {
            Debug.Log("ShouldOverwriteAdPriority : false");
            m_shouldOverwriteAdPriority = false;
        }
    }*/
}
