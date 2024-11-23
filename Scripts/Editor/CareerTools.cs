using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using NUnit.Framework.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;


public class CareerTools : Editor
{
    private static CarInfo[] _carInfos;
    private static JsonDict _unavailableCarsAtEvents;
    //private static Dictionary<string,string> _carUsedInEvents;
    private static JsonDict _carUsedInEvents;
    private static AIPlayersConfiguration _aiPlayers;

    [MenuItem("Tools/Career/Fix Ai Cars In Career")]
    static void FixAiCarsInEachEvent()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "Fix AI Cars",true,FixAICars);

        EditorUtility.DisplayDialog("Done", "Career Fix Done", "OK");
    }


    [MenuItem("Tools/Career/Fix F12 Ai")]
    static void FixF12AI()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        var events = career.CareerRaceEvents.TierX.WorldTourRaceEvents.RaceEventGroups[4].RaceEvents;
        foreach (var @event in events)
        {
            @event.AIDriver = @event.AIDriver.Replace("McLaren650SGT3", "");
        }

        EditorUtility.SetDirty(career);


        EditorUtility.DisplayDialog("Done", "Career Fix Done", "OK");
    }


    [MenuItem("Tools/Career/Fix C63_AMG_GT")]
    static void FixC63AMGGT()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "Fix C63 AMG",true,FixC63AMG);

        EditorUtility.DisplayDialog("Done", "Career Fix Done", "OK");
    }
    
    [MenuItem("Tools/Career/GoldReward10")]
    static void GoldRewardx10()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "GoldRewardx10",true,GoldRewardx10);

        EditorUtility.DisplayDialog("Done", "Career Fix Done", "OK");
    }

    [MenuItem("Tools/Career/Log Unavailable AI Driver")]
    static void LogUnavailableAIDriverButton()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "LogUnavailableAiDriver", true, LogUnavailableAIDriver);
    }


    [MenuItem("Tools/Career/Log All Manufacturer")]
    static void LogAllManufacturer()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "Log All Manufacturer", true, LogAllManufacturer);
    }


    [MenuItem("Tools/Career/Log Relay Event")]
    static void LogRelayEvent()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        DoAction(career, "LogRelayEvent", true, LogRelayEvent);
    }


    [MenuItem("Tools/Career/Log Unavailable Ai Cars")]
    static void LogUnavailableCarsOnEachEvent()
    {
        var career = Selection.activeObject as CareerConfiguration;
        if (career == null)
        {
            Debug.LogError("Career is not selected");
            return;
        }
        career.CareerRaceEvents.ProcessEvents();

        _carInfos = GetCarInfos();
        _unavailableCarsAtEvents = new JsonDict();
        _unavailableCarsAtEvents.Set("Cars",new JsonDict());
        _unavailableCarsAtEvents.Set("Events",new JsonDict());
        _carUsedInEvents = new JsonDict();
        foreach (var carInfo in _carInfos)
        {
            var carTier = carInfo.BaseCarTier.ToString();
            if (!_carUsedInEvents.ContainsKey(carTier))
            {
                _carUsedInEvents.Set(carTier,new JsonDict());
            }

            //if (!carInfo.Available)
            //{
            //    _unavailableCarsAtEvents.Set(carInfo.Key, new JsonDict());
            //}

            var tierDict = _carUsedInEvents.GetJsonDict(carTier);
            tierDict.Set(carInfo.Key, new JsonDict());
        }

        DoAction(career, "Log Unavailable Cars", false,LogUnavailableAICars);

        var path = EditorUtility.SaveFilePanel("Save Log File", Application.dataPath, "Unavailable Cars", "txt");

        if (!string.IsNullOrEmpty(path))
        {
            //var text = "";
            //foreach (var unavailableCar in _unavailableCarsAtEvents)
            //{
            //    text += unavailableCar.Key + " : ";
            //    foreach (var i in unavailableCar.Value)
            //    {
            //        text += i + ",";
            //    }

            //    text += "\n";
            //}
            System.IO.File.WriteAllText(path,_unavailableCarsAtEvents.ToString());
            EditorUtility.RevealInFinder(path);
        }


        path = EditorUtility.SaveFilePanel("Save used car File", Application.dataPath, "CarsInEvents", "txt");

        if (!string.IsNullOrEmpty(path))
        {
            //var text = "";
            //foreach (var usedCars in _carUsedInEvents)
            //{
            //    text += usedCars.Key + " : " + usedCars.Value;
            //    text += "\n";
            //}
            System.IO.File.WriteAllText(path, _carUsedInEvents.ToString());
            EditorUtility.RevealInFinder(path);
        }
    }



    private static void DoAction(CareerConfiguration career,string actionDesc,bool needDirty, Action<RaceEventTopLevelCategory, RaceEventData> action)
    {
        if (needDirty)
            Undo.RegisterCompleteObjectUndo(career, actionDesc);
        FixInTier(career.CareerRaceEvents.Tier1, action);
        FixInTier(career.CareerRaceEvents.Tier2, action);
        FixInTier(career.CareerRaceEvents.Tier3, action);
        FixInTier(career.CareerRaceEvents.Tier4, action);
        FixInTier(career.CareerRaceEvents.Tier5, action);
        FixInTier(career.CareerRaceEvents.TierX, action);
        FixInCategory(career.CareerRaceEvents.FriendRaceEvents, action);
        FixInCategory(career.CareerRaceEvents.OnlineClubRacingEvents, action);
        FixInCategory(career.CareerRaceEvents.RaceTheWorldEvents, action);
        FixInCategory(career.CareerRaceEvents.RaceTheWorldWorldTourEvents, action);
        FixInCategory(career.CareerRaceEvents.SMPRaceEvents, action);
        if (needDirty)
            EditorUtility.SetDirty(career);
    }


    private static void FixInTier(BaseCarTierEvents tierEvent, Action<RaceEventTopLevelCategory, RaceEventData> action)
    {
        FixInCategory(tierEvent.CrewBattleEvents, action);
        FixInCategory(tierEvent.RegulationRaceEvents, action);
        FixInCategory(tierEvent.DailyBattleEvents, action);
        FixInCategory(tierEvent.LadderEvents, action);
        FixInCategory(tierEvent.RestrictionEvents, action);
        FixInCategory(tierEvent.CarSpecificEvents, action);
        FixInCategory(tierEvent.ManufacturerSpecificEvents, action);
        FixInCategory(tierEvent.WorldTourRaceEvents, action);
    }


    private static void FixInCategory(RaceEventTopLevelCategory category,Action<RaceEventTopLevelCategory, RaceEventData> action)
    {
        foreach (var raceGroup in category.RaceEventGroups)
        {
            foreach (var raceEvent in raceGroup.RaceEvents)
            {
                if (action != null)
                {
                    action(category, raceEvent);
                }
            }
        }
    }

    private static void FixAICars(RaceEventTopLevelCategory category,RaceEventData raceEvent)
    {
        if (CSRToGTCars.CSR_TO_GT_Cars.ContainsKey(raceEvent.AICar))
        {
            raceEvent.AICar = CSRToGTCars.CSR_TO_GT_Cars[raceEvent.AICar];
        }

        foreach (var raceEventRestriction in raceEvent.Restrictions)
        {
            if (!string.IsNullOrEmpty(raceEventRestriction.Model))
            {
                if (CSRToGTCars.CSR_TO_GT_Cars.ContainsKey(raceEventRestriction.Model))
                {
                    raceEventRestriction.Model = CSRToGTCars.CSR_TO_GT_Cars[raceEventRestriction.Model];
                }
            }
        }
    }
    
    
    private static void FixC63AMG(RaceEventTopLevelCategory category,RaceEventData raceEvent)
    {
        if (raceEvent.AICar.Contains("C63"))
        {
            raceEvent.AICar = "car_benz_c63_amg";
        }
        foreach (var raceEventRestriction in raceEvent.Restrictions)
        {
            if (!string.IsNullOrEmpty(raceEventRestriction.Model) && raceEventRestriction.Model.Contains("C63"))
            {
                raceEventRestriction.Model = "car_benz_c63_amg";
            }
        }
    }
    
    
    private static void GoldRewardx10(RaceEventTopLevelCategory category,RaceEventData raceEvent)
    {
        raceEvent.RaceReward.GoldPrize *= 10;
    }


    private static void LogUnavailableAIDriver(RaceEventTopLevelCategory category, RaceEventData raceEvent)
    {
        var aiDriver = raceEvent.AIDriver;

        if (_aiPlayers == null)
        {
            _aiPlayers =
                AssetDatabase.LoadAssetAtPath<AIPlayersConfiguration>(
                    "Assets/configuration/AIPlayersConfiguration.asset");
        }

        var foundAiDriver = _aiPlayers.AIDrivers.FirstOrDefault(a => a.AIDriverDBKey == aiDriver);
        var tier = category.GetTierEvents().GetCarTier();
        if (aiDriver == null)
        {
            Debug.LogWarning("Ai driver not found : " + aiDriver + " at : " + tier + "-" + category.GetName() + " , " +
                      raceEvent.EventID);
        }
        else
        {
            Debug.Log("Ai driver found : " + aiDriver + " at : " + tier + "-" + category.GetName() + " , " +
                      raceEvent.EventID);
        }
    }


    private static void LogAllManufacturer(RaceEventTopLevelCategory category, RaceEventData raceEvent)
    {
        if (raceEvent.Restrictions.Count > 0)
        {
            var manufactuRestriction = raceEvent.Restrictions.FirstOrDefault(r => r.RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER);
            if (manufactuRestriction != null)
            {
                var tier = category.GetTierEvents().GetCarTier();

                Debug.Log("Manufacturer : " + manufactuRestriction + " at : " + tier + "-" + category.GetName() +
                                 " , " +
                                 raceEvent.EventID);
            }
        }
    }


    private static void LogRelayEvent(RaceEventTopLevelCategory category, RaceEventData raceEvent)
    {
        if (raceEvent.IsRelay)
        {
            var tier = category.GetTierEvents().GetCarTier();
            Debug.Log("EventID Is Relay " + " at : " + tier + "-" + category.GetName() +
                      " , " +
                      raceEvent.EventID);
        }
    }


    private static void LogUnavailableAICars(RaceEventTopLevelCategory category, RaceEventData raceEvent)
    {
        string carTier = "";
        var carInfo = _carInfos.FirstOrDefault(c => c.Key == raceEvent.AICar);
        if (carInfo != null)
        {
            carTier = carInfo.BaseCarTier.ToString();
        }
        if (!string.IsNullOrEmpty(raceEvent.AICar) && _carUsedInEvents.ContainsKey(carTier))
        {
            var eventTier = category.GetTierEvents().GetCarTier().ToString();
            var raceType = category.GetName();
            var carTierDict = _carUsedInEvents.GetJsonDict(carTier);
            var carDict = carTierDict.GetJsonDict(raceEvent.AICar);
            if (!carDict.ContainsKey(eventTier))
            {
                carDict.Set(eventTier,new JsonDict());
            }

            var tierDict = carDict.GetJsonDict(eventTier);
            if (!tierDict.ContainsKey(raceType))
            {
                tierDict.Set(raceType, new List<int>());
            }

            var eventList = tierDict.GetIntList(raceType);
            if (!eventList.Contains(raceEvent.EventID))
            {
                eventList.Add(raceEvent.EventID);
            }
            tierDict.Set(raceType, eventList);
        }
        var carByKey = _carInfos.FirstOrDefault(c => c.Key == raceEvent.AICar);
        if (carByKey != null && !carByKey.Available)
        {
            var byCarsDict = _unavailableCarsAtEvents.GetJsonDict("Cars");
            var byEventsDict = _unavailableCarsAtEvents.GetJsonDict("Events");
            var tier = category.GetTierEvents().GetCarTier().ToString();
            var categoryName = category.GetName();

            if (!byCarsDict.ContainsKey(carByKey.Key))
            {
                byCarsDict.Set(carByKey.Key,new JsonDict());
            }

            var carDict = byCarsDict.GetJsonDict(carByKey.Key);

            if (!carDict.ContainsKey(tier))
            {
                carDict.Set(tier, new JsonDict());
            }

            if (!byEventsDict.ContainsKey(tier))
            {
                byEventsDict.Set(tier, new JsonDict());
            }

            var tierDict = carDict.GetJsonDict(tier);
            var tierDict2 = byEventsDict.GetJsonDict(tier);

            if (!tierDict.ContainsKey(categoryName))
            {
                tierDict.Set(categoryName, new JsonDict());
            }

            if (!tierDict2.ContainsKey(categoryName))
            {
                tierDict2.Set(categoryName,new JsonDict());
            }


            var categoryDict = tierDict.GetJsonDict(categoryName);
            var categoryDict2 = tierDict2.GetJsonDict(categoryName);

            if (!categoryDict.ContainsKey(raceEvent.Group.EventGroupName))
            {
                categoryDict.Set(raceEvent.Group.EventGroupName,new JsonDict());
            }

            if (!categoryDict2.ContainsKey(raceEvent.Group.EventGroupName))
            {
                categoryDict2.Set(raceEvent.Group.EventGroupName, new JsonDict());
            }

            var groupDict = categoryDict.GetJsonDict(raceEvent.Group.EventGroupName);
            var groupDict2 = categoryDict2.GetJsonDict(raceEvent.Group.EventGroupName);


            groupDict.Set(raceEvent.EventID.ToString(), carByKey.Key);
            groupDict2.Set(raceEvent.EventID.ToString(), carByKey.Key);

            tierDict.Set(categoryName, categoryDict);
        }

        //foreach (var raceEventRestriction in raceEvent.Restrictions)
        //{
        //    if (!string.IsNullOrEmpty(raceEventRestriction.Model))
        //    {
        //        var carByKey2 = _carInfos.FirstOrDefault(c => c.Key == raceEventRestriction.Model);
        //        if (carByKey2 != null && !carByKey2.Available)
        //        {
        //            Debug.Log(raceEventRestriction.Model + " is not available for Restriction at : " + category.GetTierEvents().GetCarTier() + " , " + category.GetName() + " , " + raceEvent.EventID);
        //            if (!_unavailableCarsAtEvents.ContainsKey(carByKey2.Key))
        //            {
        //                _unavailableCarsAtEvents.Add(carByKey2.Key, new List<int>());
        //            }

        //            if (!_unavailableCarsAtEvents[carByKey2.Key].Contains(raceEvent.EventID))
        //                _unavailableCarsAtEvents[carByKey2.Key].Add(raceEvent.EventID);
        //        }
        //    }
        //}
    }

    private static CarInfo[] GetCarInfos()
    {
        return CarListWindow.GetGTCars();
    }


}
