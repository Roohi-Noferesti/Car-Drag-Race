using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class CareerToCareerCarTools : EditorWindow
{
    private static CareerConfiguration _careerSource;
    private static CareerConfiguration _careerDestination;
    private static bool m_includeTierX = true;
    private static bool m_includeNonTirEvents = true;
    private static bool m_includeCarSpecificEvents = true;
    private static bool m_includeManufacturerEvents = true;

    [MenuItem("Tools/Career/CareerToCareerWindow")]
    static void ShowWindow()
    {
        var window = GetWindow<CareerToCareerCarTools>();
        window.Show();
    }

    void OnGUI()
    {
        _careerSource =
            (CareerConfiguration) EditorGUILayout.ObjectField("Source", _careerSource, typeof(CareerConfiguration),
                false);
        _careerDestination = (CareerConfiguration) EditorGUILayout.ObjectField("Destination", _careerDestination,
            typeof(CareerConfiguration), false);


        m_includeTierX = EditorGUILayout.Toggle("Include TierX", m_includeTierX);
        m_includeNonTirEvents = EditorGUILayout.Toggle("Include None Tier Events", m_includeNonTirEvents);
        m_includeCarSpecificEvents = EditorGUILayout.Toggle("Include Car Specific", m_includeCarSpecificEvents);
        m_includeManufacturerEvents = EditorGUILayout.Toggle("Include Manufacturer", m_includeManufacturerEvents);


        if (GUILayout.Button("Copy Cars"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy Cars");
            DoAction(CopyCarID);
            EditorUtility.SetDirty(_careerDestination);
        }

        if (GUILayout.Button("Copy EventID in each Event"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy EventIDs");
            DoAction(CopyEventID);
            EditorUtility.SetDirty(_careerDestination);
        }


        if (GUILayout.Button("Copy AI Driver in each Event"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy Ai Driver");
            DoAction(CopyAiDriver);
            EditorUtility.SetDirty(_careerDestination);
        }

        if (GUILayout.Button("Copy SMP Races"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy SMP Races");
            _careerDestination.CareerRaceEvents.SMPRaceEvents.RaceEventGroups.AddRange(_careerSource.CareerRaceEvents.SMPRaceEvents.RaceEventGroups);
            EditorUtility.SetDirty(_careerDestination);
        }


        if (GUILayout.Button("Copy Star Reward"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy Star Reward");
            DoAction(CopyStarReward);
            EditorUtility.SetDirty(_careerDestination);
        }


        if (GUILayout.Button("Copy Cosmetic Data"))
        {
            Undo.RegisterCompleteObjectUndo(_careerDestination, "Copy Cosmetic Data");
            DoAction(CopyCosmeticData);
            CopyCosmeticInTier(_careerDestination.CareerRaceEvents.Tier1);
            CopyCosmeticInTier(_careerDestination.CareerRaceEvents.Tier2);
            CopyCosmeticInTier(_careerDestination.CareerRaceEvents.Tier3);
            CopyCosmeticInTier(_careerDestination.CareerRaceEvents.Tier4);
            CopyCosmeticInTier(_careerDestination.CareerRaceEvents.Tier5);
            EditorUtility.SetDirty(_careerDestination);
        }
    }


    private void CopyCosmeticInTier(BaseCarTierEvents source)
    {
        CopyCosmeticData(
            source.CrewBattleEvents.RaceEventGroups[0].RaceEvents[2],
            source.CrewBattleEvents.RaceEventGroups[0].RaceEvents[3]);
        CopyCosmeticData(
            source.CrewBattleEvents.RaceEventGroups[0].RaceEvents[5],
            source.CrewBattleEvents.RaceEventGroups[0].RaceEvents[6]);
    }

    private void CopyEventID(RaceEventData source, RaceEventData destination)
    {
        destination.EventID = source.EventID;
    }


    private static void CopyCarID(RaceEventData source,RaceEventData destination)
    {
        destination.AICar = source.AICar;
        for (var i = 0; i < source.Restrictions.Count; i++)
        {
            destination.Restrictions[i].Model = source.Restrictions[i].Model;
        }
    }

    private void CopyAiDriver(RaceEventData source, RaceEventData destination)
    {
        destination.AIDriver = source.AIDriver;
    }

    private void CopyStarReward(RaceEventData source, RaceEventData destination)
    {
        if (destination.IsRestrictionEvent() || destination.IsManufacturerSpecificEvent() || destination.IsCarSpecificEvent())
        {
            CopyStarRewardDefault( destination.RaceReward.RaceStarReward.BronzeLeagueReward);
            CopyStarRewardDefault( destination.RaceReward.RaceStarReward.DiamondLeagueReward);
            CopyStarRewardDefault( destination.RaceReward.RaceStarReward.GoldenLeagueReward);
            CopyStarRewardDefault( destination.RaceReward.RaceStarReward.RegularLeagueReward);
            CopyStarRewardDefault( destination.RaceReward.RaceStarReward.SilverLeagueReward);
        }
        CopyStarRewardInternal(source.RaceReward.RaceStarReward.BronzeLeagueReward,destination.RaceReward.RaceStarReward.BronzeLeagueReward);
        CopyStarRewardInternal(source.RaceReward.RaceStarReward.DiamondLeagueReward,destination.RaceReward.RaceStarReward.DiamondLeagueReward);
        CopyStarRewardInternal(source.RaceReward.RaceStarReward.GoldenLeagueReward,destination.RaceReward.RaceStarReward.GoldenLeagueReward);
        CopyStarRewardInternal(source.RaceReward.RaceStarReward.RegularLeagueReward,destination.RaceReward.RaceStarReward.RegularLeagueReward);
        CopyStarRewardInternal(source.RaceReward.RaceStarReward.SilverLeagueReward,destination.RaceReward.RaceStarReward.SilverLeagueReward);
    }

    private void CopyStarRewardInternal(LeagueStarReward source,LeagueStarReward destination)
    {
        destination.WinStar = source.WinStar;
        destination.LoseStar = source.LoseStar;
    }

    private void CopyStarRewardDefault(LeagueStarReward destination)
    {
        destination.WinStar = 30;
        destination.LoseStar = -15;
    }


    private void CopyCosmeticData(RaceEventData source, RaceEventData destination)
    {
        if (!string.IsNullOrEmpty(source.BodyShader))
            destination.BodyShader = source.BodyShader;

        if (!string.IsNullOrEmpty(source.HeadLightShader))
            destination.HeadLightShader = source.HeadLightShader;

        if (!string.IsNullOrEmpty(source.RingShader))
            destination.RingShader = source.RingShader;

        destination.UseCustomShader = source.UseCustomShader;

        if (!string.IsNullOrEmpty(source.Spoiler))
            destination.Spoiler = source.Spoiler;

        if (!string.IsNullOrEmpty(source.Sticker))
            destination.Sticker = source.Sticker;
    }

    static void DoAction(Action<RaceEventData,RaceEventData> eventAction)
    {
        if (_careerSource == null || _careerDestination==null)
        {
            Debug.LogError("Career is not selected");
            return;
        }

        DoActionInTier(_careerSource.CareerRaceEvents.Tier1, _careerDestination.CareerRaceEvents.Tier1, eventAction);
        DoActionInTier(_careerSource.CareerRaceEvents.Tier2, _careerDestination.CareerRaceEvents.Tier2, eventAction);
        DoActionInTier(_careerSource.CareerRaceEvents.Tier3, _careerDestination.CareerRaceEvents.Tier3, eventAction);
        DoActionInTier(_careerSource.CareerRaceEvents.Tier4, _careerDestination.CareerRaceEvents.Tier4, eventAction);
        DoActionInTier(_careerSource.CareerRaceEvents.Tier5, _careerDestination.CareerRaceEvents.Tier5, eventAction);
        if (m_includeTierX)
            DoActionInTier(_careerSource.CareerRaceEvents.TierX, _careerDestination.CareerRaceEvents.TierX,
                eventAction);

        if (m_includeNonTirEvents)
        {
            DoActionInCategory(_careerSource.CareerRaceEvents.FriendRaceEvents,
                _careerDestination.CareerRaceEvents.FriendRaceEvents, eventAction);
            DoActionInCategory(_careerSource.CareerRaceEvents.OnlineClubRacingEvents,
                _careerDestination.CareerRaceEvents.OnlineClubRacingEvents, eventAction);
            DoActionInCategory(_careerSource.CareerRaceEvents.RaceTheWorldEvents,
                _careerDestination.CareerRaceEvents.RaceTheWorldEvents, eventAction);
            DoActionInCategory(_careerSource.CareerRaceEvents.RaceTheWorldWorldTourEvents,
                _careerDestination.CareerRaceEvents.RaceTheWorldWorldTourEvents, eventAction);
            DoActionInCategory(_careerSource.CareerRaceEvents.SMPRaceEvents,
                _careerDestination.CareerRaceEvents.SMPRaceEvents, eventAction);
        }

        EditorUtility.SetDirty(_careerDestination);

        EditorUtility.DisplayDialog("Done", "Career Copy Done", "OK");
    }


    private static void DoActionInTier(BaseCarTierEvents tierEventSource,BaseCarTierEvents tierEventDestination, Action<RaceEventData, RaceEventData> eventDataAction)
    {
        DoActionInCategory(tierEventSource.CrewBattleEvents, tierEventDestination.CrewBattleEvents, eventDataAction);
        DoActionInCategory(tierEventSource.RegulationRaceEvents, tierEventDestination.RegulationRaceEvents, eventDataAction);
        DoActionInCategory(tierEventSource.DailyBattleEvents, tierEventDestination.DailyBattleEvents, eventDataAction);
        DoActionInCategory(tierEventSource.LadderEvents, tierEventDestination.LadderEvents, eventDataAction);
        DoActionInCategory(tierEventSource.RestrictionEvents, tierEventDestination.RestrictionEvents, eventDataAction);
        if (m_includeCarSpecificEvents)
            DoActionInCategory(tierEventSource.CarSpecificEvents, tierEventDestination.CarSpecificEvents,
                eventDataAction);
        if (m_includeManufacturerEvents)
            DoActionInCategory(tierEventSource.ManufacturerSpecificEvents,
                tierEventDestination.ManufacturerSpecificEvents, eventDataAction);
        DoActionInCategory(tierEventSource.WorldTourRaceEvents, tierEventDestination.WorldTourRaceEvents, eventDataAction);
    }


    private static void DoActionInCategory(RaceEventTopLevelCategory categorySource,
        RaceEventTopLevelCategory categoryDestination, Action<RaceEventData, RaceEventData> eventDataAction)
    {
        for (var i = 0; i < categorySource.RaceEventGroups.Count; i++)
        {
            if (categoryDestination.RaceEventGroups.Count <= i)
            {
                Debug.Log("out of index at : " + categorySource.GetTierEvents().GetCarTier() + "," +
                          categorySource.GetName() + ",group " + i);
                break;
            }
            var raceGroupSource = categorySource.RaceEventGroups[i];
            var raceGroupDestination = categoryDestination.RaceEventGroups[i];
            for (var j = 0; j < raceGroupSource.RaceEvents.Count; j++)
            {
                if (raceGroupDestination.RaceEvents.Count <= j)
                {
                    Debug.Log("out of index at : " + categorySource.GetTierEvents().GetCarTier() + "," +
                              categorySource.GetName() + ",group " + i + ",event " + j);
                    break;
                }
                var raceEventSource = raceGroupSource.RaceEvents[j];
                var raceEventDestination = raceGroupDestination.RaceEvents[j];
                DoActionInEvent(raceEventSource, raceEventDestination, eventDataAction);
            }
        }
    }

    private static void DoActionInEvent(RaceEventData raceEventSource, RaceEventData raceEventDestination, Action<RaceEventData, RaceEventData> eventDataAction)
    {
        if (eventDataAction != null)
        {
            eventDataAction(raceEventSource, raceEventDestination);
        }

    }
}
