using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CareerConfiguration))]
public class CareerConfigurationEditor : Editor
{
    private List<CarInfo> m_cars = new List<CarInfo>();
    private int m_eventCount;
    private int m_totalEvents;
    private int m_totalFix;
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<CareerConfiguration>(target);
        }
        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((CareerConfiguration)target);
        }
        base.OnInspectorGUI();
        if (GUILayout.Button("Fix lost upgrades"))
        {
            FixLostUpgradesInEvents();
        }
    }

    private void FixLostUpgradesInEvents()
    {
        var career = target as CareerConfiguration;

        if (career != null)
        {
            m_cars.Clear();
            m_eventCount = 0;
            m_totalFix = 0;
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar("Fixing upgrades", "Calculating...", 0);
            FixUpgradesInTier(career.CareerRaceEvents.Tier1,true);
            FixUpgradesInTier(career.CareerRaceEvents.Tier2, true);
            FixUpgradesInTier(career.CareerRaceEvents.Tier3, true);
            FixUpgradesInTier(career.CareerRaceEvents.Tier4, true);
            FixUpgradesInTier(career.CareerRaceEvents.Tier5, true);
            m_totalEvents = m_eventCount;
            m_eventCount = 0;

            //Fixing
            FixUpgradesInTier(career.CareerRaceEvents.Tier1,false);
            FixUpgradesInTier(career.CareerRaceEvents.Tier2, false);
            FixUpgradesInTier(career.CareerRaceEvents.Tier3, false);
            FixUpgradesInTier(career.CareerRaceEvents.Tier4, false);
            FixUpgradesInTier(career.CareerRaceEvents.Tier5, false);
            FixLostUpgradeForEvent(career.CareerRaceEvents.Tutorial);
            FixLostUpgradeForEvent(career.CareerRaceEvents.Tutorial2);
            FixLostUpgradeForEvent(career.CareerRaceEvents.Tutorial3);
        }

        EditorUtility.DisplayProgressBar("Fixing upgrades", "Applying...", 100);
        foreach (var carInfo in m_cars)
        {
            EditorUtility.SetDirty(carInfo);
        }
        EditorUtility.ClearProgressBar();

        EditorUtility.DisplayDialog("Fixing Upgrades", "Fixing Upgrades comleted\n"
                                                       + "Total Fix : " + m_totalFix + "\n"
                                                       + "Total cars : " + m_cars.Count, "Ok");
    }

    private void FixUpgradesInTier(BaseCarTierEvents baseCarTierEvents,bool calcuateCount)
    {
        //FixUpgradesInCategory(baseCarTierEvents.RegulationRaceEvents);
        //FixUpgradesInCategory(baseCarTierEvents.DailyBattleEvents);
        FixUpgradesInCategory(baseCarTierEvents.CarSpecificEvents, calcuateCount);
        FixUpgradesInCategory(baseCarTierEvents.CrewBattleEvents, calcuateCount);
        FixUpgradesInCategory(baseCarTierEvents.LadderEvents, calcuateCount);
        FixUpgradesInCategory(baseCarTierEvents.RestrictionEvents, calcuateCount);
        FixUpgradesInCategory(baseCarTierEvents.ManufacturerSpecificEvents, calcuateCount);
        FixUpgradesInCategory(baseCarTierEvents.WorldTourRaceEvents, calcuateCount);
    }

    private void FixUpgradesInCategory(RaceEventTopLevelCategory topLevelCategory,bool calculateCount)
    {
        foreach (var raceEventGroup in topLevelCategory.RaceEventGroups)
        {
            foreach (var raceEventData in raceEventGroup.RaceEvents)
            {
                if (calculateCount)
                {
                    m_eventCount++;
                }
                else
                {
                    FixLostUpgradeForEvent(raceEventData);
                }
            }
        }
    }

    private void FixLostUpgradeForEvent(RaceEventData raceEventData)
    {
        EditorUtility.DisplayProgressBar("Fixing upgrades", "Fixing,Pleaase wait... " + m_eventCount+"/"+ m_totalEvents, (float)m_eventCount / m_totalEvents);

        var carKey = raceEventData.AICar;
        var car = ResourceManager.GetCarAsset<CarInfo>(carKey, ServerItemBase.AssetType.spec);

        if (car == null)
        {
            Debug.Log(carKey + "  not found , you may change resources folder name");
            return;
        }


        var upgradeSetup = raceEventData.GetAICarUpgradeSetup(car);

        PredefinedUpgradeSetsData predefinedUpgrade = new PredefinedUpgradeSetsData();
        predefinedUpgrade.SetFromUpgradeSetup(raceEventData.AIPerformancePotentialIndex, upgradeSetup);

        if(car.PredefinedUpgradeSets.All(p => p.UpgradeData != predefinedUpgrade.UpgradeData))
        {
            Array.Resize(ref car.PredefinedUpgradeSets, car.PredefinedUpgradeSets.Length + 1);
            car.PredefinedUpgradeSets[car.PredefinedUpgradeSets.Length - 1] = predefinedUpgrade;
            Debug.Log("upgrade set '" + predefinedUpgrade.UpgradeData + "' not found in " + carKey + " for event : "
                +raceEventData.EventID+","+raceEventData.Parent);

            if (!m_cars.Contains(car))
            {
                m_cars.Add(car);
            }
            m_totalFix++;
        }

        m_eventCount++;
    }
}
