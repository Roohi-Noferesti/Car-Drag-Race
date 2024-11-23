using System;
using System.Collections.Generic;
using Firebase.Crashlytics;
using I2.Loc;
using KingKodeStudio;
//using KingKodeStudio.IAB;
using Metrics;
using PurchasableItems;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public enum RequestState
    {
        OK,
        WAIT,
        ERROR,
        ERROR_NO_NETWORK,
        ERROR_LOGIN_AUTHENTICATION,
        ERROR_LOGIN_BANNED,
        ERROR_SERVER_INVALID_REQUEST,
        ERROR_SERVER_INTERNAL_ERROR,
        ERROR_SERVICE_UNAVAILABLE
    }

    public WebClient webClient;

    private WebRequest webRequest;

    private string analyticsData;

    private bool loggedIn;

    private bool debugResetPurchasesOnNextLogin;

    private float timeSinceLastDeviceTokenCheck;

    private Account attemptingConnectAccount;

    public static event UserLoggedInDelegate LoggedInEvent;

    public static event UserLoggedInDelegate LoginFailedEvent;

    public static event UserChangedDelegate UserChangedEvent;

    private string notificationDeviceToken;
    private bool settingAdjustAttribution;
    private static bool serverChosen;

    public static UserManager Instance
    {
        get;
        private set;
    }

    public Accounts deviceAccounts
    {
        get;
        private set;
    }

    public string error
    {
        get;
        private set;
    }

    public bool isLoggedIn
    {
        get
        {
            return loggedIn;
        }
    }

    public RequestState requestState
    {
        get;
        private set;
    }

    public Account currentAccount
    {
        get;
        private set;
    }

    public bool IsWaiting
    {
        get
        {
            return requestState == RequestState.WAIT;
        }
    }

    public void DebugResetPurchasesOnNextLogin()
    {
        debugResetPurchasesOnNextLogin = true;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        KeychainFake.SetPersistentPath(Application.persistentDataPath);
        FileUtils.SetPersistentPath(Application.persistentDataPath);
        LoadAccounts();

        //Thiese line of code cause access exception error on some devices
        //if (DoWeNeedToRunProfileMigration())
        //{
        //    PlayerProfileMigration.RunProfileMigrationProcess();
        //}
        //if (!BaseIdentity.ActivePlatform.DoesUUIDExist())
        //{
        //    string zContents = BaseIdentity.ActivePlatform.GenerateIdentity();
        //    BaseIdentity.ActivePlatform.SaveUUID(zContents);
        //}

        //if (!BaseBackupUtils.ActivePlatform.SecretFileExists())
        //{
        //    BaseBackupUtils.ActivePlatform.CreateSecretFile();
        //}
        webClient = gameObject.AddComponent<WebClient>();
        webClient.Init();
        ResetState();
        if (BasePlatform.ActivePlatform.GetTargetAppStore() == GTAppStore.Amazon)
        {
            return;
        }
        if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
        {
            StartConnect();
        }
#if UNITY_ANDROID
        else if (AndroidSpecific.GetGPGSignInCount() >= 1)
        {
            StartConnect();
            
        }
#endif

    }

    private void LogTestEvent()
    {
        Log.AnEvent(Endpoint.ServerEndpointCollection.Current == Endpoint.ServerEndpointCollection.LocalEndPoint || Endpoint.ServerEndpointCollection.Current == Endpoint.ServerEndpointCollection.TestEndPoint
            ? Events.TestInsideCountry
            : Events.TestOutsideCountry);
    }
    private void LoadAccounts()
    {
        deviceAccounts = new Accounts();
    }

    private bool IsThisANewReInstall()
    {
        return !BaseIdentity.ActivePlatform.DoesUUIDExist();
    }

    private bool DoWeNeedToRunProfileMigration()
    {
        bool flag = BaseBackupUtils.ActivePlatform.SecretFileExists();
        bool flag2 = BaseIdentity.ActivePlatform.DoesUUIDExist();
        return !flag && !flag2;
    }

    public bool ChangeUser(Account zAccount)
    {
        //if (!serverChosen)
        //{
        //    ChooseUserServer(zAccount); //smk -- we just broke an structure
        //    serverChosen = true;
        //}

        if (UserChangedEvent != null && (currentAccount == null || currentAccount.Username != zAccount.Username))
        {
            SetCurrentAccount(zAccount);
            UserChangedEvent();
            return true;
        }
        SetCurrentAccount(zAccount);
        return false;
    }

    public void SetCurrentAccount(Account zAccount)
    {
        currentAccount = zAccount;
    }

    public void Logout()
    {
        loggedIn = false;
        if (webClient != null)
            webClient.EndSession();
    }

    public void StopConnect()
    {
        if (IsWaiting)
        {
            FinishRequest();
        }
    }

    private void ZeroAppStoreBalances()
    {
        currentAccount.IAPGold = 0;
        currentAccount.IAPCash = 0;
        currentAccount.SuperNitrous = 0;
        currentAccount.RaceCrew = 0;
        SaveCurrentAccount();
    }

    private void Start()
    {
        NotificationManager.Active.AskForNotificationPermission();
    }

    private void Update()
    {
        if (PolledNetworkState.CachedValue == BasePlatform.eReachability.OFFLINE)
        {
            SetNoNetworkState();
            return;
        }
        if (this.webRequest!=null && IsWaiting && this.webRequest.IsDone && Kickback.IsSafePlaceToKickback())
        {
            string error = string.Empty;
            int num = 0;
            JsonDict jsonDict = DecodeResponse(ref error, ref num);
            if (WebUtils.IsUsingWWWClass() && num == 200 && string.IsNullOrEmpty(this.webRequest.Content))
            {
                num = 503;
            }
            if (num == 0)
            {
                SetErrorState(RequestState.ERROR_NO_NETWORK, "TEXT_WEB_REQUEST_STATUS_CODE_0");
                OnLoginFailed();
            }
            else if (num == 403)
            {
                SetErrorState(RequestState.ERROR_LOGIN_AUTHENTICATION, "TEXT_WEB_REQUEST_STATUS_CODE_403");
                OnLoginFailed();
            }
            else if (num == 503)
            {
                SetErrorState(RequestState.ERROR_SERVICE_UNAVAILABLE, "TEXT_WEB_REQUEST_STATUS_CODE_503");
                OnLoginFailed();
            }
            else if (num >= 500)
            {
                SetErrorState(RequestState.ERROR_SERVER_INTERNAL_ERROR, "TEXT_WEB_REQUEST_STATUS_CODE_500");
                OnLoginFailed();
            }
            else if (num >= 400)
            {
                SetErrorState(RequestState.ERROR_SERVER_INVALID_REQUEST, "TEXT_WEB_REQUEST_STATUS_CODE_400");
                OnLoginFailed();
            }
            else if (!string.IsNullOrEmpty(error))
            {
                SetErrorState(error);
                OnLoginFailed();
            }
            else if (!UpdateUserAccountFromLogin(jsonDict))
            {
                SetErrorState(RequestState.ERROR_LOGIN_AUTHENTICATION, "TEXT_WEB_REQUEST_STATUS_CODE_403");
                OnLoginFailed();
            }
            else
            {
                if (PlayerProfileManager.Instance.ActiveProfile.Name == "temp")
                {
                    PlayerProfileManager.Instance.ActiveProfile.Rename(currentAccount.Username);
                }
                string text = jsonDict.GetString("profile_data");
                if (!string.IsNullOrEmpty(text))
                {
                    byte[] zBytes = Convert.FromBase64String(text);
                    FileUtils.WriteLocalStorage(currentAccount.Username, zBytes, false);
                    PlayerProfileWeb.SendProfileConfirmed(currentAccount.Username);
                }
                OnLoggedIn();
            }
            FinishRequest();
        }
        UpdateCurrentAccountDeviceToken();
    }

    private void FinishRequest()
    {
        if (webRequest != null)
        {
            webRequest.Release();
            webRequest = null;
        }
    }

    public bool UpdateUserAccountFromLogin(JsonDict data)
    {
        int errorLineIndex = 0;
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        string username = data.GetString("username");
        string uUID = BaseIdentity.ActivePlatform.GetUUID();
        string macAddress = BasePlatform.ActivePlatform.GetMacAddress();
        string gcid = data.GetString("gcid");
        analyticsData = data.GetString("analytics_event");
        Dictionary<Parameters, string> data2 = new Dictionary<Parameters, string>
        {
            {
                Parameters.baid,
                username
            },
            {
                Parameters.openUDID,
                uUID
            },
            {
                Parameters.mac,
                macAddress
            }
        };
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        Log.AnEvent(Events.UserLogin, data2);
        Account account = deviceAccounts.Find(username);
        if (account != currentAccount)
        {
            if (account == null)
            {
                account = deviceAccounts.NewAccount(username);
            }
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        if (attemptingConnectAccount != null && attemptingConnectAccount.Username != username)
        {
            MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
            if (currentAccount.IsTemporaryAccount)
            {
                MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
                int num = deviceAccounts.FindSlot(attemptingConnectAccount.Username);
                if (num >= 0)
                {
                    deviceAccounts.Clear(num);
                }
            }
            else if (BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
            {
                MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
                //Remove previous account with same gcid in case of migrating use to world server
                if (!string.IsNullOrEmpty(attemptingConnectAccount.GCid) && attemptingConnectAccount.GCid==gcid)
                {
                    MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
                    int num = deviceAccounts.FindSlot(attemptingConnectAccount.Username);
                    if (num >= 0)
                    {
                        GTDebug.Log(GTLogChannel.UserManager,"attemping account is cleared here");
                        deviceAccounts.Clear(num);
                    }
                }
                MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
                account.Username = username;
                PlayerProfileManager.Instance.ActiveProfile.Rename(username);
                MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
            }
            MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        account.Password = data.GetString("password");
        account.IAPGold = data.GetInt("gold");
        account.SuperNitrous = data.GetInt("supernitrous");
        account.RaceCrew = data.GetInt("racecrew");
        account.IAPCash = data.GetInt("cash");
        account.GCid = data.GetString("gcid");
        account.CollectUsageData = data.GetInt("collect_usage_data");
        account.ABTestCode = data.GetString("abtest_code");
        account.ABTestBucketName = data.GetString("abtest_bucket_name");
        account.AssetDatabaseBranch = data.GetString("abtest_branch_name");
        account.IPAddress = data.GetString("ip_address");
        account.IntroMessageJson = data.GetString("intro_message");
        account.BAM = data.GetInt("bam_v2");
        account.ServerLastSavedProfileProductVersion = data.GetString("profile_product_version");
        account.MPToken = data.GetString("mpt");
        account.RYFToken = data.GetString("ryft");
        account.IsBanned = (data.GetInt("banned") == 1);
        account.HasUpgradedFuelTank = (data.GetInt("has_upgraded_fuel_tank") == 1);
        account.ClientAction = data.GetInt("client_action");
        account.ClientActionTextBody = data.GetString("client_action_text");
        account.ClientActionTextTitle = data.GetString("client_action_text_title");
        account.AdjustAttributionSet = data.GetInt("adjust_attribution_set") == 1;
        account.CountryCode = data.GetString("country_code");
        account.BranchPostfix = data.GetString("BranchPostFix");
        account.PossibleStoresForIAPABTest = data.GetString("BranchStores");
        var freshChatID = data.GetString("fresh_id");
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        GTDebug.Log(GTLogChannel.UserManager,"FreshChat ID By Server : " + freshChatID);
        if (!string.IsNullOrEmpty(freshChatID))
        {
            account.FreschatRestoreID = freshChatID;
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);

        PlayerProfileWeb.SyncFrequencySeconds = data.GetInt("profile_sync_freq");
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        if (!MultiplayerUtils.DisableMultiplayer)
        {
            MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
            RTWStatusManager.Instance.PollFrequency = data.GetInt("rtw_status_poll_freq");
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        if (!data.ContainsKey("ryf_messages_enabled"))
        {
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        //RYFStatusManager.Instance.PushMessagesAvailable = data.GetBool("ryf_messages_enabled");
        if (data.ContainsKey("ryf_sync_friend_data_freq"))
        {
            //RYFStatusManager.Instance.PollFrequency = (float)data.GetInt("ryf_sync_friend_data_freq", 700);
        }
        if (data.ContainsKey("ryf_update_friend_network_freq"))
        {
            //SocialFriendsManager.Instance.PollFrequency = (float)data.GetInt("ryf_update_friend_network_freq", 600);
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        if (!debugResetPurchasesOnNextLogin && (currentAccount.Username == account.Username || currentAccount.IsTemporaryAccount))
        {
            MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
            if (currentAccount.IAPGold > account.IAPGold)
            {
                account.IAPGold = currentAccount.IAPGold;
            }
            if (currentAccount.IAPCash > account.IAPCash)
            {
                account.IAPCash = currentAccount.IAPCash;
            }
            if (currentAccount.SuperNitrous > account.SuperNitrous)
            {
                account.SuperNitrous = currentAccount.SuperNitrous;
            }
            if (currentAccount.RaceCrew > account.RaceCrew)
            {
                account.RaceCrew = currentAccount.RaceCrew;
            }
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        if (currentAccount.Username == account.Username)
        {
            MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
            account.FBAccessToken = currentAccount.FBAccessToken;
            account.FBExpirationDate = currentAccount.FBExpirationDate;
        }
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        SetCurrentAccount(account);
        SaveCurrentAccount();
        attemptingConnectAccount = null;
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        
        /*if(data.ContainsKey("islamic_countries")) 
            BasePlatform.ActivePlatform.UpdateInsideIslamicCountryValue(data.GetString("islamic_countries"));
        if(data.ContainsKey("adpriority_countries"))
            BasePlatform.ActivePlatform.UpdateAdPriorityCountryValue(data.GetString("adpriority_countries"));*/

        if(data.ContainsKey("InsideIslamicCountry")) 
            BasePlatform.ActivePlatform.UpdateInsideIslamicCountryValue(data.GetBool("InsideIslamicCountry"));
        if(data.ContainsKey("InsideAdPriorityCountry"))
            BasePlatform.ActivePlatform.UpdateShouldOverwriteAdPriorityValue(data.GetBool("InsideAdPriorityCountry"));
        if(data.ContainsKey("InsideIAPFocusedCountry"))
            BasePlatform.ActivePlatform.UpdateInsideIAPFocusedCountryValue(data.GetBool("InsideIAPFocusedCountry"));
        if(data.ContainsKey("InsideAdFocusedCountry"))
            BasePlatform.ActivePlatform.UpdateInsideAdFocusedCountryValue(data.GetBool("InsideAdFocusedCountry"));
        if(data.ContainsKey("RemoteAdPriorities"))
            BasePlatform.ActivePlatform.UpdateRemoteAdPriorities(data.GetString("RemoteAdPriorities"));
        if(data.ContainsKey("SuccessfulIAPAbTestAssigning"))
            BasePlatform.ActivePlatform.SuccessfullyIAPAbTestAssigning(data.GetString("SuccessfulIAPAbTestAssigning"));
        
        MetricsIntegration.Instance.LogCrash(String.Format("UpdateUserAccountFromLogin Error: {0}", errorLineIndex++), false);
        
        return true;
    }

    public void UpdateCurrentAccountDeviceToken()
    {
        if (currentAccount == null || IsWaiting)
        {
            return;
        }
        if (timeSinceLastDeviceTokenCheck > 0f)
        {
            timeSinceLastDeviceTokenCheck -= Time.deltaTime;
            return;
        }
        if (string.IsNullOrEmpty(currentAccount.DeviceToken))
        {
            currentAccount.DeviceToken = NotificationManager.Active.GetUnityDeviceToken();
            if (!string.IsNullOrEmpty(currentAccount.DeviceToken))
            {
                SaveCurrentAccount();
            }
            else
            {
                timeSinceLastDeviceTokenCheck = 60f;
            }
        }
    }

    public void UpdateAccountBalanceFromReceipt(JsonDict zAccount)
    {
        int serverGold = zAccount.GetInt("gold");
        int serverCash = zAccount.GetInt("cash");
        int superNitrous = zAccount.GetInt("supernitrous");
        string text = string.Empty;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        string format = LocalizationManager.GetTranslation("TEXT_USERMANAGER_BALANCE_RECOVERY_MESSAGE");
        if (currentAccount.IAPGold > serverGold)
        {
            int num3 = currentAccount.IAPGold - serverGold;
            string goldString = CurrencyUtils.GetGoldStringWithIcon(activeProfile.GetCurrentGold());
            string goldString2 = CurrencyUtils.GetGoldStringWithIcon(activeProfile.GetCurrentGold() - num3);
            text += string.Format(format, goldString, goldString2);
        }
        if (currentAccount.IAPCash > serverCash)
        {
            int num4 = currentAccount.IAPCash - serverCash;
            if (text != string.Empty)
            {
                text += Environment.NewLine;
            }
            string cashString = CurrencyUtils.GetCashString(activeProfile.GetCurrentCash());
            string cashString2 = CurrencyUtils.GetCashString(activeProfile.GetCurrentCash() - num4);
            text += string.Format(format, cashString, cashString2);
        }
        currentAccount.IAPGold = serverGold;
        currentAccount.IAPCash = serverCash;
        currentAccount.SuperNitrous = superNitrous;
        SaveCurrentAccount();
        if (text != string.Empty)
        {
            if (SceneLoadManager.Instance != null && SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
            {
                text += Environment.NewLine;
                text += currentAccount.Username;
                BalanceRecoveryScreen.SetAdjustmentText(text);
                ScreenManager.Instance.PushScreen(ScreenID.BalanceRecovery);
            }
        }
    }

    public void UpdateUserAccountFromReceipt(JsonDict zAccount)
    {
        int isValidReceipt = zAccount.GetInt("is_valid_receipt");//1 is valid
        string @string = zAccount.GetString("code");
        if (@string.Contains("raceteam") && isValidReceipt == 0)
        {
            ConsumablesManager.RevokeWholeTeamConsumable();
        }
        IAPDatabase iAPs = GameDatabase.Instance.IAPs;
        if (isValidReceipt == 0)
        {
            if (@string.Contains(iAPs.UnlimitedFuelProductCode))
            {
                UnlimitedFuelManager.Revoke();
            }
            foreach (PurchasableItem current in GameDatabase.Instance.IAPs.GetPurchasableItems())
            {
                if (@string.Contains(current.IAPCode) && current.IsOwned)
                {
                    current.Revoke();
                }
            }
        }
    }

    public void UpdateUserAccountFromPurchaseResult(PurchaseResult zPurchase)
    {
        string productID = zPurchase.ProductID;
        GTProduct productWithID = ProductManager.Instance.GetProductWithID(productID);
        if (productWithID == null)
        {
            return;
        }
        if (zPurchase.Result == PurchaseResult.eResult.SUCCEEDED)
        {
            currentAccount.IAPGold += productWithID.Gold + productWithID.BonusGold;
            currentAccount.IAPCash += productWithID.Cash + productWithID.BonusCash;
            currentAccount.SuperNitrous += productWithID.NumSuperNitros;
            currentAccount.RaceCrew += productWithID.NumRaceCrew;
        }
        if (productWithID.NumRaceCrew > 0)
        {
            ConsumablesManager.SetupWholeTeamConsumable(null);
        }
        IAPDatabase iAPs = GameDatabase.Instance.IAPs;
        if (!string.IsNullOrEmpty(iAPs.UnlimitedFuelProductCode))
        {
            if (productWithID.Code.Contains(iAPs.UnlimitedFuelProductCode))
            {
                UnlimitedFuelManager.Apply();
            }
        }
        foreach (PurchasableItem current in iAPs.GetPurchasableItems())
        {
            if (current != null && !string.IsNullOrEmpty(current.IAPCode))
            {
                if (productID.Contains(current.IAPCode))
                {
                    current.Apply();
                }
            }
        }
        SaveCurrentAccount();
    }

    public void SaveCurrentAccount()
    {
        if (PlayerProfileManager.Instance != null)
        {
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            if (activeProfile != null)
            {
            }
        }
        TimeSpan timeSpan = GTDateTime.Now - new DateTime(1970, 1, 1);
        currentAccount.LastUsed = (int)timeSpan.TotalSeconds;
        deviceAccounts.Save(currentAccount);
    }

    private void OnLoginFailed()
    {
        debugResetPurchasesOnNextLogin = false;
        loggedIn = false;
        if (LoginFailedEvent != null)
        {
            LoginFailedEvent();
        }
    }

    private void OnLoggedIn()
    {
        requestState = RequestState.OK;
        loggedIn = true;
        if (loggedIn)
        {
            ProductManager.Instance.enabled = true;
            ProductManager.Instance.Init();
        }
        debugResetPurchasesOnNextLogin = false;
        if (isLoggedIn && analyticsData != string.Empty)
        {
            LogTestEvent();
        }

        /*
        GTGTDebug.Log(GTLogChannel.UserManager,GTLogChannel.UserManager,"USer manager log in --  attChanged : "+ MetricsIntegration.Instance.HasAdjustAttributionChanged()
                  +"  ,  accAttChanged : "+ currentAccount.AdjustAttributionSet);
        if (MetricsIntegration.Instance.HasAdjustAttributionChanged() && !currentAccount.AdjustAttributionSet)
        {
            GTGTDebug.Log(GTLogChannel.UserManager,GTLogChannel.UserManager, "first condition ");
            long referralUserID = 0;
            currentAccount.AdjustTrackerName = MetricsIntegration.Instance.AdjustTrackerName;
            GTGTDebug.Log(GTLogChannel.UserManager,GTLogChannel.UserManager, "adjust click label : " + MetricsIntegration.Instance.HasAdjustClickLabel());
            if (MetricsIntegration.Instance.HasAdjustClickLabel())
            {
                GTGTDebug.Log(GTLogChannel.UserManager,GTLogChannel.UserManager, "second condition : " + MetricsIntegration.Instance.AdjustClickLabel);
                if (long.TryParse(MetricsIntegration.Instance.AdjustClickLabel, out referralUserID))
                {
                    GTGTDebug.Log(GTLogChannel.UserManager,GTLogChannel.UserManager, "third condition : " + referralUserID);
                    currentAccount.ReferralUserID = referralUserID;
                }
            }

            SendAdjustAttributionToServer(currentAccount.AdjustTrackerName, referralUserID);
        }
        */

        AppStore.Instance.ConnectToStore();

        if (LoggedInEvent != null)
        {
            LoggedInEvent();
        }
    }

    private void SendAdjustAttributionToServer(string trackerName, long referralUserID)
    {
        GTDebug.Log(GTLogChannel.UserManager, "setting adjust in server : " + settingAdjustAttribution + " , trackerName : " + trackerName
                  + "  ,  referralUserID: " + referralUserID);
        if (settingAdjustAttribution)
        {
            return;
        }
        JsonDict parameters = new JsonDict();
        parameters.Set("username", UserManager.Instance.currentAccount.Username);
        parameters.Set("adjust_tracker_name", trackerName);
        parameters.Set("referral_user_id", referralUserID.ToString());
        WebRequestQueue.Instance.StartCall("rtw_set_adjust_attribution", "set adjust attribution", parameters, AdjustAttributionResponse,
            null, null);
    }

    private void AdjustAttributionResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        settingAdjustAttribution = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting adjust attribution : " + zerror);
            return;
        }

        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(zhttpcontent))
        {
            currentAccount.AdjustAttributionSet = true;
            GTDebug.Log(GTLogChannel.Account, "adjust attribution updated in server");
            SaveCurrentAccount();
        }
    }

    private void SetState(RequestState zState)
    {
        requestState = zState;
    }

    private void SetErrorState(string zErrorMessage)
    {
        requestState = RequestState.ERROR;
        error = zErrorMessage;
    }

    private void SetErrorState(RequestState zState, string zErrorMessage)
    {
        requestState = zState;
        error = zErrorMessage;
    }

    private void SetNoNetworkState()
    {
        if (IsWaiting)
        {
            FinishRequest();
        }
        if (requestState != RequestState.ERROR_NO_NETWORK)
        {
            SetErrorState(RequestState.ERROR_NO_NETWORK, "TEXT_NO_INTERNET");
        }
    }

    public void ResetState()
    {
        if (IsWaiting)
        {
            FinishRequest();
        }
        SetState(RequestState.OK);
    }

    public void ResetTokens()
    {
        currentAccount.MPToken = string.Empty;
        currentAccount.RYFToken = string.Empty;
    }

    public void StartConnect(bool authenticateByGoogle = false)
    {
        //smk
        GTDebug.Log(GTLogChannel.UserManager,"enter startconnect method");
        if (PlayerProfileManager.Instance.ActiveProfile != null && Instance.currentAccount != null && Instance.currentAccount.Username != PlayerProfileManager.Instance.ActiveProfile.Name && PlayerProfileWeb.RecoveredProfileExists())
        {
            return;
        }
        if (IsWaiting)
        {
            FinishRequest();
        }
        string[] gmailAddresses = new string[0];
        string gc_alias = null;
        string gc_ID = null;
#if UNITY_EDITOR
        //gmailAddresses = new string[0];// { "mujpir@gmail.com" };
        //gc_ID = "g06266648569269381875";//    "g05550166309918938189"-->"mujpir";
#elif UNITY_ANDROID
        GTDebug.Log(GTLogChannel.UserManager, "GC_ID is : " + GameCenterController.Instance.currentID());
        //gmailAddresses = AndroidSpecific.GetGMailAddresses();

        if (BasePlatform.ActivePlatform.GetTargetAppStore() == GTAppStore.GooglePlay)
        {
            gc_ID = GameCenterController.Instance.currentID();
        }
        else if (GameCenterController.Instance.isPlayerAuthenticated())
        {
            gc_ID = GameCenterController.Instance.currentID();
            gc_alias = GameCenterController.Instance.currentAlias();
        }
        else
        {
            gc_ID = GameCenterController.Instance.currentID();
        }
#endif

        GTDebug.Log(GTLogChannel.UserManager,"gc_id : " + gc_ID);
        bool isGoogleAccount = false;
        Account account;
        if (!string.IsNullOrEmpty(gc_ID) || gmailAddresses.Length > 0)
        {
            account = FindGameCenterAccount(gc_ID, gmailAddresses);
            if (account == null)
            {
                account = FindMostRecentNonGameCenterAccount();
                if (account != null && BasePlatform.ActivePlatform.IsSupportedGooglePlayStore() && string.IsNullOrEmpty(gc_ID) && gmailAddresses.Length > 0)
                {
                    account.GCid = gmailAddresses[0];
                    isGoogleAccount = true;
                }
            }
        }
        else
        {
            account = FindDefaultAccount();
        }
        bool profileFileExists = false;
        if (account != null)
        {
            profileFileExists = PlayerProfileFile.ProfileFileExists(account.Username);
        }
        byte[] zippedProfileData = null;
        GTDebug.Log(GTLogChannel.UserManager,"finding user");
        if (profileFileExists)
        {
            ChangeUser(account);
            if (isGoogleAccount)
            {
                SaveCurrentAccount();
            }
            PlayerProfileFile.ReadBinaryFile(account.Username, ref zippedProfileData);
        }
        else
        {
            Account tempAccount = deviceAccounts.FindTemp();
            if (tempAccount == null)
            {
                tempAccount = deviceAccounts.NewTempAccount();
                tempAccount.Password = "password";
            }
            tempAccount.GCid = ((!string.IsNullOrEmpty(gc_ID)) ? gc_ID : string.Empty);
            ChangeUser(tempAccount);
            SaveCurrentAccount();
            if (!PlayerProfileFile.ProfileFileExists(tempAccount.Username))
            {
                ZeroAppStoreBalances();
            }
        }
        GTDebug.Log(GTLogChannel.UserManager,"current account null : " + (currentAccount == null));
        Logout();
        if (!authenticateByGoogle && PolledNetworkState.CachedValue == BasePlatform.eReachability.OFFLINE)
        {
            SetNoNetworkState();
            OnLoginFailed();
            return;
        }
        string username = null;
        string password = null;
        string deviceToken = null;
        if (account != null)
        {
            username = account.Username;
            password = account.Password;
            deviceToken = account.DeviceToken;
            if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore() && !string.IsNullOrEmpty(account.GCid))
            {
                gc_ID = account.GCid;
            }
            if (string.IsNullOrEmpty(deviceToken))
            {
                deviceToken = NotificationManager.Active.GetUnityDeviceToken();
            }
        }

        //We add this line here to integrate to our existing server api
        if (string.IsNullOrEmpty(deviceToken))
        {
            deviceToken = NotificationManager.Active.GetUnityDeviceToken();
        }

        //GTDebug.Log(GTLogChannel.UserManager,deviceToken+"      "+SystemInfo.deviceUniqueIdentifier);

        if (string.IsNullOrEmpty(notificationDeviceToken))
        {
            notificationDeviceToken = NotificationManager.Active.GetNotificationServicesDeviceToken();
            GTDebug.Log(GTLogChannel.Account, "notif Device token : " + notificationDeviceToken);
        }

        attemptingConnectAccount = account;

        string language = LocalizationManager.CurrentLanguage;

        if (webClient == null)
        {
            return;
        }
        this.webRequest = this.ServerConnect(gc_ID, username, password, gc_alias, zippedProfileData, this.debugResetPurchasesOnNextLogin, deviceToken, gmailAddresses, language);
        SetState(RequestState.WAIT);
    }

    private WebRequest ServerConnect(string gcid, string username, string password, string gc_alias,
        byte[] zippedProfileData, bool debugResetPurchasesOnNextLogin, string device_token, string[] alternative_ids,
        string language)
    {
        GTDebug.Log(GTLogChannel.Account, "trying to connect to server : gcid:" + gcid + "  ,  device token:" + device_token);
        JsonDict jsonDict = new JsonDict();
        jsonDict.Set("gc_id", (gcid == null) ? string.Empty : gcid);
        jsonDict.Set("gc_alias", (gc_alias == null) ? string.Empty : gc_alias);
        jsonDict.Set("username", (username == null) ? string.Empty : username);
        jsonDict.Set("password", (password == null) ? string.Empty : password);
        if (alternative_ids != null)
        {
            for (int i = 0; i < alternative_ids.Length; i++)
            {
                jsonDict.Set("alt_id_" + i, alternative_ids[i]);
            }
            jsonDict.Set("num_alt_ids", alternative_ids.Length.ToString());
        }

        string value = string.Empty;
        if (zippedProfileData != null)
        {
            value = Convert.ToBase64String(zippedProfileData);
        }

        var devicModel = BasePlatform.ActivePlatform.GetDeviceModel();

        var platform = BasePlatform.TargetPlatform.ToString();

        var appVersion = ApplicationVersion.Current;

        var storeName = BasePlatform.ActivePlatform.GetTargetAppStore().ToString();

        var freshChat = string.IsNullOrEmpty(currentAccount.FreschatRestoreID) ? "" : currentAccount.FreschatRestoreID;

        var assetVersion = "NA";
        if (AssetDatabaseClient.Instance != null && AssetDatabaseClient.Instance.IsReadyToUse)
        {
            assetVersion = AssetDatabaseClient.Instance.Data.GetVersion().ToString();
        }

        jsonDict.Set("profile_data", value);
        jsonDict.Set("debug_reset_purchases", (!debugResetPurchasesOnNextLogin) ? "0" : "1");
        jsonDict.Set("device_token", string.IsNullOrEmpty(device_token) ? string.Empty : device_token);
        jsonDict.Set("notification_device_token", string.IsNullOrEmpty(notificationDeviceToken) ? string.Empty : notificationDeviceToken);
        jsonDict.Set("device_model", devicModel);
        jsonDict.Set("platform", platform);
        jsonDict.Set("app_ver", appVersion);
        jsonDict.Set("app_store", storeName);
        jsonDict.Set("fresh_id", freshChat);
        jsonDict.Set("assetdb_ver", assetVersion);
        jsonDict.Set("language", language);
        jsonDict.Set("city", BasePlatform.ActivePlatform.GetCity().ToLower());
        WebRequestQueue.Instance.RemoveItems("acc_connect_127");
        return webClient.ServerFunction("acc_connect_127", jsonDict);
    }


    private Account FindDefaultAccount()
    {
        return deviceAccounts.Default();
    }

    private Account FindGameCenterAccount(string gcid, string[] alternatives)
    {
        return deviceAccounts.FindGameCenterAccount(gcid, alternatives);
    }

    private Account FindMostRecentNonGameCenterAccount()
    {
        return deviceAccounts.Default(true);
    }

    private JsonDict DecodeResponse(ref string errorString, ref int status)
    {
        string content = string.Empty;
        WebClient.ProcessObfuscatedResponse(this.webRequest, BasePlatform.eSigningType.Server_Accounts, ref content, ref errorString, ref status);
        if (!string.IsNullOrEmpty(errorString))
        {
            return null;
        }
        if (status != 200)
        {
            return null;
        }
        JsonDict jsonDict = new JsonDict();
        try
        {
            jsonDict.Read(content);
        }
        catch (Exception var_2_3E)
        {
            errorString = "Server sent malformed JSON in response";
            jsonDict = null;
            return jsonDict;
        }
        if (content == null)
        {
            errorString = "Server sent an empty response";
            return null;
        }
        return jsonDict;
    }



    public void ResetCurrentAccount()
    {
        throw new NotImplementedException();
    }

    public void EnsureIdentityFiles()
    {
        throw new NotImplementedException();
    }

    public static void ChooseUserServer(Account zAccount)
    {
//#if !LOCAL_HOST
//        //if (zAccount.UserID == 0 && (!AndroidSpecific.InsideIran))
//        //{
//        //    Endpoint.ServerEndpointCollection.Current = Endpoint.ServerEndpointCollection.WorldEndPoint;
//        //    zAccount.UseEndPointVersionV2 = true;
//        //}
//        if (zAccount.UseEndPointVersionV2)
//        {
//            Endpoint.ServerEndpointCollection.Current = Endpoint.ServerEndpointCollection.WorldEndPoint;
//        }
//        else
//        {
//            Endpoint.ServerEndpointCollection.Current = Endpoint.ServerEndpointCollection.IranEndPoint;
//        }
//#endif
    }


}
