using Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Firebase.Analytics;
using Firebase.Crashlytics;
using GameAnalyticsSDK;
using ir.metrix.unity;
using KingKodeStudio;
using Unity.Services.Core;
using UnityEngine;
using AppsFlyerSDK;
using FlurrySDK;

public class MetricsIntegration : MonoBehaviour
{
    public static string GoogleAdId = null;
    
    private const long BUFFER_LIMIT = 2048L;

    private const long BUFFER_MAXOUT = 65536L;

    private const string BAMurl = "http://www.kingcodestudio.com:2505/user_data/";

    public static MetricsIntegration Instance;

    private static string NM_PORTAL_ADDRESS = "http://www.kingcodestudio.com:2505/fozzie/portal.php";

    private bool sessionActive;

    private bool metricsSystemActive = true;

    private bool logMetrics = false;

    private const string ADJUST_TRACKER_NAME_KEY = "adjust_trackerName";
    private const string ADJUST_TRACKER_CLICK_LABEL_KEY = "adjust_clickLabel";
    
    private Dictionary<Type, IAnalytics> analyticsPlugins = new Dictionary<Type, IAnalytics>();

    private bool isMetricsEnabled;


    private static Dictionary<GTPlatforms, string> NmgSessionKey = new Dictionary<GTPlatforms, string>
    {
        {
            GTPlatforms.ANDROID,
            "upha0ovoozeizoCeer3wa3baeLougeinieXidue6"
        },
        {
            GTPlatforms.iOS,
            "aec6ShahOi9AebieothoWas7Iveidae3"
        },
        {
            GTPlatforms.OSX,
            "oix5vai6aeNg8aez3ieyed8eesohyahb3r"
        }
    };

    private static Dictionary<GTPlatforms, string> NmgSessionIdent = new Dictionary<GTPlatforms, string>
    {
        {
            GTPlatforms.ANDROID,
            "csrandroid"
        },
        {
            GTPlatforms.iOS,
            "csr"
        },
        {
            GTPlatforms.OSX,
            "csrmac"
        }
    };
    
    private WebRequest bamWebRequest;

    public bool NmgSessionActive()
    {
        return sessionActive;
    }

    public static bool IsReady()
    {
        return Instance != null;
    }

    //private void LogEvent(Events eventName, List<KeyValuePair<Parameters, string>> metricsData)
    //{
    //	string text = this.listToString(metricsData);
    //       if (this.logMetrics)
    //       {
    //           if (GTPlatform.IsEditor)
    //           {
    //               this.DebugLogEventToFile(eventName.ToString(), metricsData);
    //           }
    //       }
    //	this.LogBAMEventToFile(string.Format("::{0}::{1}", eventName, text));
    //	if (!this.sessionActive && this.metricsSystemActive)
    //	{
    //		return;
    //	}
    //       //NmgBinding.logEventWithParameters(eventName.ToString(), text);
    //       //ApsalarBinding.logEventWithParameters(eventName, text);
    //}

    private string listToString(List<KeyValuePair<Parameters, string>> theList)
    {
        var list = new List<string>();
        var list2 = new List<string>();
        var list3 = new List<string>();
        //string empty = string.Empty;
        var validForNMOnly = ParametersLists.ValidForNMOnly;
        var validForFlurry = ParametersLists.ValidForFlurry;
        foreach (var current in theList)
        {
            var list4 = (!validForNMOnly.Contains(current.Key))
                ? ((!validForFlurry.Contains(current.Key)) ? list2 : list)
                : list3;
            list4.Add(string.Format("{0}||{1}", current.Key, current.Value));
        }

        var text = string.Join("|/|", list3.ToArray());
        var text2 = string.Join("|/|", list.ToArray());
        var text3 = string.Join("|/|", list2.ToArray());
        return string.Concat(new string[]
        {
            text2,
            "///",
            text3,
            "///",
            text
        });
    }

    private void LogEvent(Events eventName, List<KeyValuePair<Parameters, string>> metricsData)
    {
        // var text = listToString(metricsData);
        if (logMetrics)
        {
            if (GTPlatform.IsEditor)
            {
                DebugLogEventToFile(eventName.ToString(), metricsData);
            }
        }

        //LogBAMEventToFile(string.Format("::{0}::{1}", eventName, text));
        if (!sessionActive && metricsSystemActive)
        {
            return;
        }

        //NmgBinding.logEventWithParameters(eventName.ToString(), text);
        //ApsalarBinding.logEventWithParameters(eventName, text);
    }


    private void listToJsonDic(List<KeyValuePair<Parameters, string>> theList, ref JsonDict jsonDict)
    {
        foreach (var current in theList)
        {
            jsonDict.Set(current.Key.ToString(), current.Value);
        }
    }

    public void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        SetGoogleAdId();
        Instance = this;
        
        
    }
    
    private void SetGoogleAdId()
    {
        Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    GTDebug.LogError(GTLogChannel.Metrics, error);
                    return;
                }
                GoogleAdId = advertisingId;
                GTDebug.Log(GTLogChannel.Metrics,"advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
            }
        );
    }

    public void Initialize()
    {
        GetAnalytics(out analyticsPlugins);
        
        if (BuildType.CanCollectData())
        {
            //if (AgeVerificationManager.Instance.HasPrivacyPolicyVerified)
            //{
            //StartAdjust();
            //StartFlurry();
            foreach (var analytic in analyticsPlugins.Values)
            {
                analytic.Init();
            }

            isMetricsEnabled = true;
            
        }

        ApplicationManager.WillResignActiveEvent +=
            new ApplicationEvent_Delegate(Instance.OnApplicationWillResignActive);
        ApplicationManager.DidBecomeActiveEvent +=
            new ApplicationEvent_Delegate(Instance.OnApplicationWillEnterForeground);
        UserManager.LoggedInEvent += UserManager_LoggedInEvent;
        UserManager.UserChangedEvent += User_Changed;
    }
    

    private void GetAnalytics(out Dictionary<Type, IAnalytics> analyticsPlugins)
    {
        
        var enumerable =
            from x in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetTypes())
            where typeof(IAnalytics).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(IAnalytics))
            select x;
        analyticsPlugins = new Dictionary<Type, IAnalytics>();
        foreach (var current in enumerable)
        {
            IAnalytics item;
            if (typeof(MonoBehaviour).IsAssignableFrom(current))
            {
                GameObject go = new GameObject();
                //MethodInfo[] method1 =
                //    typeof(GameObject).GetMethods().Where(m => m.Name == "AddComponent").ToArray();
                //Debug.Log(method1.Length);
                MethodInfo method =
                    typeof(GameObject).GetMethod("AddComponent", new Type[1] { typeof(Type) });
                method.Invoke(go, new object[]
                {
                    current
                });
                go.name = current.Name;
                UnityEngine.Object.DontDestroyOnLoad(go);
                MethodInfo method2 =
                    typeof(GameObject).GetMethod("GetComponent", new Type[1] { typeof(Type) });
                item = (method2.Invoke(go, new object[]
                {
                    current
                }) as IAnalytics);
            }
            else
            {
                item = (Activator.CreateInstance(current) as IAnalytics);
            }
            analyticsPlugins.Add(item.GetType() ,item);
        }
    }
    private void User_Changed()
    {
        //error occured because it is called before initializing crashlytics
        //SetCrashlyticsUserID();
    }


    private void StartFlurry()
    {
        /*FlurryAnalytics.Instance.SetDebugLogEnabled(false);
        FlurryAnalytics.Instance.StartSession("TDQBJQ2BDYWD6YQSH4QF", "HXGDRDYJ6B9NK8DMVN98", true);*/
    }
    

    /*void SetBugSnagUserID()
    {
        if (!BuildType.CanCollectData())
            return;
        
        var config = BugsnagSettingsObject.LoadConfiguration();
        config.AddOnSendError(@event => {
            @event.AddMetadata("user", "ID", UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.UserID.ToString());
            @event.AddMetadata("user", "ABTestCode", UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.ABTestCode);
            @event.AddMetadata("user", "Username", UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.Username);
            @event.AddMetadata("user", "Market", BasePlatform.ActivePlatform.GetTargetAppStore().ToString());
            @event.AddMetadata("user", "Timezone", BasePlatform.ActivePlatform.GetCity());
            @event.AddMetadata("AppState", "UIScreenID", ScreenManager.Instance.ActiveScreen.ID.ToString());

            // Return `false` if you'd like to stop this error being reported
            return true;
        });
        Bugsnag.Start(config);
    }*/


    void SetAppLovinUserID()
    {
        //MaxSdk.SetUserId(UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.UserID.ToString());
    }

    public void LogCrash(string str, bool logToConsole = false)
    {
        try
        {
            if (!BuildType.CanCollectData())
                return;
            #if !UNITY_EDITOR
                Crashlytics.Log(str);
                Flurry.LogBreadcrumb(str);
                //Bugsnag.Notify(new System.InvalidOperationException(str));
            #endif
                if(logToConsole && Debug.isDebugBuild)
                    Debug.LogError("[SelfReportedCrash] " + str);
        }
        catch
        {
        }
    }

    /*
    private void StartAdjust()
    {
#if GT_DEBUG_LOGGING
        AdjustConfig adjustConfig = new AdjustConfig("fkzyvr087qbk", AdjustEnvironment.Sandbox);
        adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
#else
        AdjustConfig adjustConfig = new AdjustConfig("fkzyvr087qbk", AdjustEnvironment.Production);
        adjustConfig.setLogLevel(AdjustLogLevel.Error);
#endif
        adjustConfig.setLogDelegate(msg => GTDebug.Log(GTLogChannel.Android, msg));
        adjustConfig.setSendInBackground(false);
        adjustConfig.setLaunchDeferredDeeplink(true);
        //adjustConfig.setEventSuccessDelegate(EventSuccessCallback);
        //adjustConfig.setEventFailureDelegate(EventFailureCallback);
        //adjustConfig.setSessionSuccessDelegate(SessionSuccessCallback);
        //adjustConfig.setSessionFailureDelegate(SessionFailureCallback);
        //adjustConfig.setDeferredDeeplinkDelegate(DeferredDeeplinkCallback);
        adjustConfig.setAttributionChangedDelegate(AttributionChangedCallback);
        Adjust.start(adjustConfig);

        //AttributionChangedCallback(new AdjustAttribution(new Dictionary<string, string>()
        //{
        //    {AdjustUtils.KeyClickLabel, "1188690"},
        //    {AdjustUtils.KeyTrackerName, "GooglePlay"}
        //}));
    }
    */

    /*
    private void AttributionChangedCallback(AdjustAttribution obj)
    {
        GTDebug.Log(GTLogChannel.Metrics,"AttributionChangedCallback");
        if (!PlayerPrefs.HasKey(ADJUST_TRACKER_NAME_KEY))
        {
            //PlayerPrefs.SetString(ADJUST_TRACKER_NAME_KEY, obj.trackerName);
            //PlayerPrefs.SetString(ADJUST_TRACKER_CLICK_LABEL_KEY, obj.clickLabel);
            WritePref(obj.trackerName, obj.clickLabel);
            GTDebug.Log(GTLogChannel.Metrics,"AttributionChangedCallback : " + obj.trackerName + " , " + obj.clickLabel);
        }
    }

    private static string[] ReadPref()
    {
        string[] values = new string[2];
        string filePath = Application.persistentDataPath + "/pref.txt";
        if (File.Exists(filePath))
        {
            var data = File.ReadAllText(filePath);
            JsonDict jsonDict = new JsonDict();
            if (jsonDict.Read(data))
            {
                values[0] = jsonDict.GetString(ADJUST_TRACKER_NAME_KEY);
                values[1] = jsonDict.GetString(ADJUST_TRACKER_CLICK_LABEL_KEY);
            }
        }

        return values;
    }


    private static void WritePref(string trackerName,string clickLabel)
    {
        JsonDict jsonDict = new JsonDict();
        jsonDict.Set(ADJUST_TRACKER_NAME_KEY, trackerName);
        jsonDict.Set(ADJUST_TRACKER_CLICK_LABEL_KEY, clickLabel);
        File.WriteAllText(Application.persistentDataPath + "/pref.txt", jsonDict.ToString());
    }

    public bool HasAdjustAttributionChanged()
    {
        var data = ReadPref();
        return !string.IsNullOrEmpty(data[0]);
    }

    public bool HasAdjustClickLabel()
    {
        var data = ReadPref();
        return !string.IsNullOrEmpty(data[1]);
    }

    public string AdjustTrackerName
    {
        get
        {
            var data = ReadPref();
            return data[0];
        }
    }

    public string AdjustClickLabel
    {
        get
        {
            var data = ReadPref();
            return data[1];
        }
    }
    */

    void UserManager_LoggedInEvent()
    {
        if (isMetricsEnabled && BuildType.CanCollectData())
        {
            Debug.Log("UserManager_LoggedInEvent: Inited Crashlytics & BugSnag");
            Flurry.SetUserId(UserManager.Instance.currentAccount.UserID.ToString());
            foreach (var analytic in analyticsPlugins.Values)
            {
                analytic.SetUserID();
            }
            SetAppLovinUserID();
        }
    }

    public void ValidateReceiptWithAppsFlyer(string prodID, string price, string currency, string receipt)
    {
        (analyticsPlugins[typeof(AppsFlyerAnalytic)] as AppsFlyerAnalytic).ValidateReceiptWithAppsFlyer(prodID, price, currency, receipt);
    }
    
    public void AddAppsFlyerPurchaseData(ref JsonDict jsonDict)
    {
        try
        {
            if (!string.IsNullOrEmpty(GoogleAdId)) jsonDict.Set("advertising_id", GoogleAdId);
            (analyticsPlugins[typeof(AppsFlyerAnalytic)] as AppsFlyerAnalytic).AddAppsFlyerPurchaseData(ref jsonDict);
        }
        catch (Exception e)
        {
            GTDebug.LogError(GTLogChannel.Metrics, e.Message);
            //Crashlytics.LogException(e);
        }
    }

    public void Update()
    {
        //this.UpdateNMMetrics();
        updateBAMService();
    }

    private void ConnectToNMServices()
    {
        var productDesc = "notset";
        if (AssetDatabaseClient.Instance != null && AssetDatabaseClient.Instance.IsReadyToUse)
        {
            productDesc = AssetDatabaseClient.Instance.Data.GetAppVersion();
        }

        var preSharedKey = NmgSessionKey[BasePlatform.TargetPlatform];
        var productKey = NmgSessionIdent[BasePlatform.TargetPlatform];
        sessionActive = NmgBinding.NmgServicesConnect(NM_PORTAL_ADDRESS, preSharedKey,
            productKey, productDesc);
        //NmgServices.SetAppBackgroundTimeout(1);
    }

    public void StartMetricsSessions()
    {
        if (metricsSystemActive)
        {
            if (!sessionActive)
            {
                //this.ConnectToNMServices();
            }

            if (PlayerProfileManager.Instance.ActiveProfile != null)
            {
                PlayerProfileManager.Instance.ActiveProfile.CumulativeSessions++;
                PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            }
        }

        Log.AnEvent(Events.StartGT);
        //Log.AnEvent(Events.PlayerStat);
    }

    private void UpdateNMMetrics()
    {
        if (!metricsSystemActive)
        {
            return;
        }

        if (!sessionActive)
        {
            ConnectToNMServices();
            return;
        }

        NmgBinding.NmgServicesUpdate();
    }


    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        if (!isMetricsEnabled || !BuildType.CanCollectData())
            return;
        
        try
        {
            if(!data.ContainsKey(Parameters.UserID))
                data.Add(Parameters.UserID, UserManager.Instance.currentAccount != null? UserManager.Instance.currentAccount.UserID.ToString(): "");
            if(!data.ContainsKey(Parameters.AppVersion))
                data.Add(Parameters.AppVersion, BasePlatform.ActivePlatform.GetApplicationVersion());
            if(!data.ContainsKey(Parameters.UserCountry))
                data.Add(Parameters.UserCountry, BasePlatform.ActivePlatform.GetCity());
            
            ////Send to flurry
            var parameters = data.ToDictionary(s => s.Key.ToString(), s => s.Value);
            Flurry.LogEvent(eventName.ToString(), parameters);
            foreach (var analytics in analyticsPlugins.Values)
            {
                analytics.LogAnEvent_Internal(eventName, data);
            }

            //Analyse and send to server
            HandleSpecialCaseEvents(eventName, data);
            if (!EventsLists.ParametersForEvent.ContainsKey(eventName))
            {
                return;
            }

            var enumerable = EventsLists.ParametersForEvent[eventName].Union(EventsLists.Defaults);
            var list = new List<KeyValuePair<Parameters, string>>();
            foreach (var current in enumerable)
            {
                var value = MetricsCalculate.Get(current, data);
                var item = new KeyValuePair<Parameters, string>(current, value);
                list.Add(item);
            }

            LogEvent(eventName, list);
            HandleSpecialCaseEventsPost(eventName, data);
        }
        catch
        {
            Debug.LogError("An Error Occurred In LogAnEvent_Internal");
        }
    }


    public static string GetNMCoreIDSafe()
    {
        var result = string.Empty;
        //if (MetricsIntegration.Instance.NmgSessionActive())
        //{
        //    result = NmgBinding.GetCoreID();
        //}
        return result;
    }
    private void HandleSpecialCaseEvents(Events eventName, Dictionary<Parameters, string> data)
    {
        if (eventName != Events.UserLogin)
        {
            return;
        }

        MetricsCalculate.BeginUserSession();
        Flurry.SetUserId(data[Parameters.baid]);
    }

    private void HandleSpecialCaseEventsPost(Events eventName, Dictionary<Parameters, string> data)
    {
        if (EventsLists.EventsForSessionHistory.Contains(eventName))
        {
            MetricsCalculate.SessionAddHistoryEvent(eventName);
        }
    }

    private void DebugLogEventToFile(string eventName, List<KeyValuePair<Parameters, string>> metricsData)
    {
        var text = Path.Combine(FileUtils.temporaryCachePath, "MetricsOutput");
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }

        var path = Path.Combine(text, "MetricsData.txt");
        File.AppendAllText(path, "-=( " + eventName.ToString() + " )=--------------------------------------\n");
        foreach (var current in metricsData)
        {
            File.AppendAllText(path, string.Format("{0,-20}{1}\n", current.Key.ToString(), current.Value));
        }
    }

    // private string GetBAMFilename()
    // {
    //     var localStorageFilePath = FileUtils.GetLocalStorageFilePath("BAM");
    //     return Path.Combine(localStorageFilePath, UserManager.Instance.currentAccount.Username + ".bam");
    // }

    private void OnApplicationWillResignActive()
    {
        //var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //var dictionary = new Dictionary<Parameters, string>
        //{
        //    {
        //        Parameters.DCsh,
        //        activeProfile.GetCurrentCash().ToString()
        //    },
        //    {
        //        Parameters.DGld,
        //        activeProfile.GetCurrentGold().ToString()
        //    },
        //    {
        //        Parameters.DXp,
        //        activeProfile.GetPlayerXP().ToString()
        //    },
        //    {
        //        Parameters.Dfuel,
        //        FuelManager.Instance.GetFuel().ToString()
        //    },
        //};
        //Log.AnEvent(Events.EndGT, dictionary);
        //SendBAMEventsToServer();
    }

    private void OnApplicationWillEnterForeground()
    {
        StartMetricsSessions();
    }

    private static string AllEventData = "";
    
    private void LogBAMEventToFile(string eventData)
    {
        if (UserManager.Instance == null || UserManager.Instance.currentAccount == null ||
            UserManager.Instance.currentAccount.IsTemporaryAccount) // || UserManager.Instance.currentAccount.BAM == 0)
        {
            return;
        }

        var localStorageFilePath = FileUtils.GetLocalStorageFilePath("BAM");
        if (!Directory.Exists(localStorageFilePath))
        {
            Directory.CreateDirectory(localStorageFilePath);
        }

        //var bamFilename = GetBAMFilename();
        // var num = 0L;
        // if (File.Exists(bamFilename))
        // {
        //     var fileInfo = new FileInfo(bamFilename);
        //     num = fileInfo.Length;
        // }
        var num = AllEventData.Length;
        if (num < 65536L)
        {
            //File.AppendAllText(bamFilename, eventData + "\n");
            AllEventData += eventData;
        }

        if (num > 2048L)
        {
            // var go  = new GameObject("SendBamEventsToServerCoroutine");
            // var coroutineComponent = go.AddComponent<CoroutineComponent>();
            // coroutineComponent.StartCoroutine()
            SendBAMEventsToServer();
        }
    }


    private void SendBAMEventsToServer()
    {
        if (!PolledNetworkState.IsNetworkConnected)
        {
            return;
        }

        // var bamFilename = GetBAMFilename();
        // var fileInfo = new FileInfo(bamFilename);
        // if (!fileInfo.Exists || fileInfo.Length == 0L)
        // {
        //     return;
        // }

        //var data = File.ReadAllText(bamFilename);
        if (string.IsNullOrEmpty(AllEventData))
            return;
        
        bamWebRequest?.Release();

        if (bamWebRequest == null)
            bamWebRequest = new WebRequest(Endpoint.GetBamURL(), null, "POST");
        else
            bamWebRequest.ClearFormData();
        
        bamWebRequest.AddFormData("user_name", UserManager.Instance.currentAccount.Username);
        bamWebRequest.AddFormData("data", AllEventData);
        bamWebRequest.AddFormData("coreid", GetNMCoreIDSafe());
        bamWebRequest.Send();
       // ThreadPool.QueueUserWorkItem(DeleteBamFile);
    }
    //
    // private static void DeleteBamFile(object bamFilename)
    // {
    //     File.Delete((string)bamFilename);
    // }

    private void updateBAMService()
    {
        if (bamWebRequest != null && bamWebRequest.IsDone)
        {
            bamWebRequest.Release();
            bamWebRequest = null;
        }
    }
    
}