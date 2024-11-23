using System;
using System.Collections;
using System.Collections.Generic;
using Metrics;
using UnityEngine;
using AppsFlyerSDK;
using Firebase.Crashlytics;

public class AppsFlyerAnalytic : IAnalytics
{
    private AppsFlyerObjectScript appsFlyerObject;
    
    private const string AppsFlyerDevKey = "YBThmUqaiHZYpiSwZ3GQz4";

    private static readonly List<Events> AppsFlyerValidEventList = new List<Events>()
    {
        Events.Complete1stTutRace,
        Events.Complete2ndTutRace,
        Events.Complete3thTutRace,
        Events.CompleteRegTutRace,
        Events.CompleteCrewTutRace,
        Events.LevelUp,
        Events.FirstTimePurchase,
        Events.FirstRewardedAdWatched,
        Events.FirstInterstitialAdWatched,
        Events.RewardedAdWatched,
        Events.InterstitialAdWatched,
        Events.ClickOnShopItem,
        Events.First10Races,
        Events.TierCompleted,
        Events.HeyYou,
        Events.LevelReached_02,
        Events.LevelReached_03,
        Events.LevelReached_04,
        Events.LevelReached_05,
        Events.LevelReached_06,
        Events.LevelReached_07,
        Events.LevelReached_08,
        Events.LevelReached_09,
        Events.LevelReached_10,
        Events.AppsFlyerValidationStart,
        Events.AppsFlyerValidationSuccess,
        Events.AppsFlyerValidationFail,
        
        Events.OnClearFinesFreeButtonClicked,
        Events.PressForFuelAddWatchCompleted,
        Events.PressForFuelAddWatchCanceled,
        Events.ErrorToShowPressFuelVideo,
        Events.OnUpgradeFinesCapacityButtonClicked,
        Events.OnBuyUpgradeFinesCapacityButtonClicked,
        Events.OnClearFinesWithCoinButtonClicked,
        Events.ApproveClearFinesWithCoin,
        
        Events.FirstBeginnerRegulationRaceAttemped,
        Events.FirstAmateurRegulationRaceAttemped,
        Events.FirstHardRegulationRaceAttemped,
        
        Events.Tier1CrewMember1Race,
        Events.Tier1CrewMember2Race,
        Events.Tier1CrewMember3Race,
        Events.Tier1CrewMember3_2Race,
        Events.Tier1CrewMember4Race,
        Events.Tier1CrewMember4_2Race,
        Events.Tier1CrewMember4_3Race,
        
        Events.Tier1CrewMember1FirstRace,
        Events.Tier1CrewMember2FirstRace,
        Events.Tier1CrewMember3FirstRace,
        Events.Tier1CrewMember3_2FirstRace,
        Events.Tier1CrewMember4FirstRace,
        Events.Tier1CrewMember4_2FirstRace,
        Events.Tier1CrewMember4_3FirstRace,

        Events.AppsFlyerValidationFail,
        Events.OnlineRace,
        
        Events.TapToUpgrade,
        Events.IsFirstBodyUpgrade,
        Events.IsFirstEngineUpgrade,
        Events.IsFirstIntakeUpgrade,
        Events.IsFirstNitroUpgrade,
        Events.IsFirstTransmissionUpgrade,
        Events.IsFirstTurboUpgrade,
        Events.IsFirstTyresUpgrade,
        Events.FirstTapToUpgrade,
        
        Events.DeferredPurchase,
        
        Events.TestInsideCountry,
        Events.TestOutsideCountry

    };

    public event AnalyticsFinishedInitializationHandler InitializationFinished;

    public void Init()
    {
        string appID = "";
        
        appsFlyerObject = new GameObject().AddComponent<AppsFlyerObjectScript>();
        appsFlyerObject.gameObject.name = "AppsFlyerObject";
        GameObject.DontDestroyOnLoad(appsFlyerObject);
        
#if UNITY_IOS
            appID = "1458220954";
#endif
        
        appsFlyerObject.Init(AppsFlyerDevKey, appID, true, Debug.isDebugBuild);
        
        Debug.Log("Inited AppsFlyer SDK version: " + AppsFlyer.getSdkVersion());
    }

    public void SetUserID()
    {
        
    }

    public bool IsValidEventID(Events eventName)
    {
        return AppsFlyerValidEventList.Contains(eventName);
    }

    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        // send to appsflyer
        if (IsValidEventID(eventName) && !PurchasingModuleSelection.IsUDP)
        {
            Dictionary<string, string> appsflyerParameters = new Dictionary<string, string>(data.Count);
            foreach(var evnt in data.Keys)
            {
                appsflyerParameters.Add(evnt.ToString(), data[evnt]);
            }
            AppsFlyer.sendEvent(eventName.ToString(), appsflyerParameters);
        }
    }

    public void UserManager_LoggedInEvent()
    {
        SetUserID();
    }

    public void ValidateReceiptWithAppsFlyer(string prodID, string price, string currency, string receipt)
    {
        appsFlyerObject.ProcessPurchase(prodID, price, currency, receipt);
    }
    
    public void AddAppsFlyerPurchaseData(ref JsonDict jsonDict)
    {
        // https://support.appsflyer.com/hc/en-us/articles/207034486-Server-to-server-events-API-for-mobile-S2S-mobile-
        try
        {
            var appsFlyerId = AppsFlyer.getAppsFlyerId();
            
            jsonDict.Set("appsflyer_id", appsFlyerId);
            //if (!string.IsNullOrEmpty(GoogleAdId)) jsonDict.Set("advertising_id", GoogleAdId);
            //jsonDict.Set("customer_user_id",); => set from server side
            //jsonDict.Set("eventName", ); => set from server side -> af_purchase
            //jsonDict.Set("eventValue", ); => set from server side 
            //jsonDict.Set("app_version_name", ); => set from server side 
            //jsonDict.Set("app_store", ); => set from server side 
            //jsonDict.Set("bundleIdentifier", ); => set from server side -> com.myapp
            jsonDict.Set("af_devKey", AppsFlyerDevKey);
        }
        catch (Exception e)
        {
            GTDebug.LogError(GTLogChannel.Metrics, e.Message);
            //Crashlytics.LogException(e);
        }
    }
}
