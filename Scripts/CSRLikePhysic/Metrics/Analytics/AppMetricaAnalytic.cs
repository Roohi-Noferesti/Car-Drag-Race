using System.Collections;
using System.Collections.Generic;
using Metrics;
using UnityEngine;

public class AppMetricaAnalytic : IAnalytics
{

    private static readonly List<Events> AppMetricaValidEventList = new List<Events>()
    {
        Events.FirstTimePurchase,
        Events.LevelReached_10,
        Events.PurchaseStart,
        Events.PurchaseSuccess,
        Events.PurchaseFail,
        Events.PurchaseCancel,
        Events.PurchaseShopOpen,
        Events.PurchaseShopCategoryClick,
        Events.PurchaseShopItemClick,
        
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

        Events.PurchaseShopItemClick,
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
        GameObject.Instantiate(Resources.Load<GameObject>("AppMetrica/AppMetrica"));
    }

    public void SetUserID()
    {
        
    }

    public bool IsValidEventID(Events eventName)
    {
        return AppMetricaValidEventList.Contains(eventName);
    }

    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        if (IsValidEventID(eventName))
        {
            AppMetrica.Instance.SetUserProfileID(UserManager.Instance.currentAccount.UserID.ToString());
            Dictionary<string, object> appMtricaParameters = new Dictionary<string, object>(data.Count);
            foreach (var evnt in data.Keys)
            {
                appMtricaParameters.Add(evnt.ToString(), data[evnt]);
            }
            AppMetrica.Instance.ReportEvent(eventName.ToString(), appMtricaParameters);
        }
    }

    public void UserManager_LoggedInEvent()
    {
        
    }
}
