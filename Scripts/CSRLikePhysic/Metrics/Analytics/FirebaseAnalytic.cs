using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Metrics;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseAnalytic : IAnalytics
{
    private static readonly List<Events> FirebaseValidEventList = new List<Events>()
    {
        Events.TapToShop,
        Events.ShopProductsArrived,
        Events.Complete1stTutRace,
        Events.Complete2ndTutRace,
        Events.Complete3thTutRace,
        Events.CompleteRegTutRace,
        Events.CompleteCrewTutRace,
        Events.ChooseYourName,
        Events.BuyFirstCar,
        Events.BuyThisCar,
        Events.ThisIsNitrous,
        Events.HeyYou,
        Events.CompleteRace,
        Events.TapsWorld,
        Events.HomeScreenShowed,
        Events.AdStaticInterstitialDisplayed,
        Events.AdUserV4RComplete,
        Events.AdUserCancelV4R,
        Events.AdUserV4RStopEarly,
        Events.AdUserStartV4R,
        Events.LevelUp,

        Events.FirstTimePurchase,
        Events.FirstRewardedAdWatched,
        Events.FirstInterstitialAdWatched,
        Events.RewardedAdWatched,
        Events.InterstitialAdWatched,
		
        Events.ClickOnShopItem,
        Events.First10Races,
        Events.TierCompleted,
        Events.LevelReached_02,
        Events.LevelReached_03,
        Events.LevelReached_04,
        Events.LevelReached_05,
        Events.LevelReached_06,
        Events.LevelReached_07,
        Events.LevelReached_08,
        Events.LevelReached_09,
        Events.LevelReached_10,
        
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
        Events.LevelReached_10,
        Events.JumpInClearFuelTimer,
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
        // Initialize Firebase
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                var app = Firebase.FirebaseApp.DefaultInstance;


                // Set a flag here for indicating that your project is ready to use Firebase.
                GTDebug.Log(GTLogChannel.Metrics,"Firebase is Ready to Use");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void SetUserID()
    {
        if (!BuildType.CanCollectData())
            return;
        
        FirebaseAnalytics.SetUserProperty("abtest_branch_name", UserManager.Instance.currentAccount.AssetDatabaseBranch);
        FirebaseAnalytics.SetUserProperty("UserID", UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.UserID.ToString());
        FirebaseAnalytics.SetUserProperty("UserName", UserManager.Instance.currentAccount==null?"unknown":UserManager.Instance.currentAccount.Username);
        FirebaseAnalytics.SetUserProperty("Market", BasePlatform.ActivePlatform.GetTargetAppStore().ToString());
        FirebaseAnalytics.SetUserProperty("Timezone", BasePlatform.ActivePlatform.GetCity());
        RemoteConfigManager.Instance.ReportToFirebase();
    }

    public bool IsValidEventID(Events eventName)
    {
        return FirebaseValidEventList.Contains(eventName);
    }

    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        if (IsValidEventID(eventName))
        {
            var firebaseParameters = data.Select(x => new Parameter(x.Key.ToString(), x.Value)).ToArray();
            FirebaseAnalytics.LogEvent(eventName.ToString(), firebaseParameters);
        }
    }

    public void UserManager_LoggedInEvent()
    {
        SetUserID();
    }
}
