using System.Collections;
using System.Collections.Generic;
using ir.metrix.unity;
using Metrics;
using UnityEngine;

public class MetrixAnalytics : IAnalytics
{
    private static readonly List<Events> MetrixValidEventList = new List<Events>()
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
    };

    private Dictionary<Events, string> metrixSlugs = new Dictionary<Events, string>()
    {
        {Events.Complete1stTutRace, "pnsvb"},
        {Events.Complete2ndTutRace, "cmoge"},
        {Events.Complete3thTutRace, "prqtm"},
        {Events.CompleteRegTutRace, "gzspz"},
        {Events.CompleteCrewTutRace, "ljhfz"},
        {Events.LevelUp, "lziph"},
        {Events.FirstTimePurchase, "jpulj"},
        {Events.FirstRewardedAdWatched, "jmdya"},
        {Events.FirstInterstitialAdWatched, "bjfyf"},
        {Events.RewardedAdWatched, "sadpk"},
        {Events.InterstitialAdWatched, "xgkyp"},
        {Events.ClickOnShopItem, "kovbn"},
        {Events.First10Races, "qinyf"},
        {Events.TierCompleted, "wqqks"},
        {Events.HeyYou, "bvztj"},
        {Events.LevelReached_02, "wfzix"},
        {Events.LevelReached_03, "adyaf"},
        {Events.LevelReached_04, "vbckt"},
        {Events.LevelReached_05, "omcwu"},
        {Events.LevelReached_06, "osnlg"},
        {Events.LevelReached_07, "qgiec"},
        {Events.LevelReached_08, "rgtss"},
        {Events.LevelReached_09, "oavjr"},
        {Events.LevelReached_10, "nmzua"}
    };

    

    public event AnalyticsFinishedInitializationHandler InitializationFinished;

    public void Init()
    {
#if UNITY_ANDROID
        //This part of code is not needed for new version of metrix (1.1.0 and above)
#endif
    }

    public void SetUserID()
    {
        
    }

    public bool IsValidEventID(Events eventName)
    {
        return MetrixValidEventList.Contains(eventName);
    }

    public void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data)
    {
        if (IsValidEventID(eventName) && BasePlatform.ActivePlatform.InsideCountry)
        {
            Dictionary<string, string> metrixParameters = new Dictionary<string, string>(data.Count);
            foreach (var evnt in data.Keys)
            {
                metrixParameters.Add(evnt.ToString(), data[evnt]);
            }
            Metrix.NewEvent(metrixSlugs[eventName], metrixParameters);
        }
    }

    public void UserManager_LoggedInEvent()
    {
        
    }
}
