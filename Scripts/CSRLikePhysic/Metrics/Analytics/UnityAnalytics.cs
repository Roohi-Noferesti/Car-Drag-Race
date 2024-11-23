using System.Collections;
using System.Collections.Generic;
using Metrics;
using Unity.Services.Core;
using UnityEngine;

public class UnityAnalytics : IAnalytics
{
    private static readonly List<Events> UnityAnalyticsValidEventList = new List<Events>()
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
        UnityServices.InitializeAsync();
    }

    public void SetUserID()
    {
        
    }

    public bool IsValidEventID(Events eventName)
    {
        return UnityAnalyticsValidEventList.Contains(eventName);
    }

    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        if (IsValidEventID(eventName))
        {
            // Send custom event
            Dictionary<string, object> unityAnalyticsParameters = new Dictionary<string, object>(data.Count);
            foreach(var evnt in data.Keys)
            {
                unityAnalyticsParameters.Add(evnt.ToString(), data[evnt]);
            }
            // The ‘myEvent’ event will get queued up and sent every minute
            Unity.Services.Analytics.Events.CustomData(eventName.ToString(), unityAnalyticsParameters); 

            // Optional - You can call Events.Flush() to send the event immediately
            Unity.Services.Analytics.Events.Flush();
        }
    }

    public void UserManager_LoggedInEvent()
    {
        
    }
}
