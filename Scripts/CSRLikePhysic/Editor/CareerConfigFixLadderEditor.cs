using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CareerConfigFixLaddereEitor : ScriptableWizard
{
    public TextAsset ConfigToFix;
    public CareerConfiguration ReferenceConfig;
    public bool UpdateAllLadderRaces = false;
    public bool UpdateAllSpecificRaces = false;
    [MenuItem("Tools/FixLadderRacesWizard...")]
    private static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CareerConfigFixLaddereEitor>("Fix Career Ladder", "Apply");
    }

    void OnWizardCreate()
    {
        var configToChange = CopyConfigUtils.ReadFromText<CareerConfiguration>(ConfigToFix.text);

        if (UpdateAllLadderRaces)
        {
            configToChange.CareerRaceEvents.Tier1.LadderEvents = ReferenceConfig.CareerRaceEvents.Tier1.LadderEvents;
            configToChange.CareerRaceEvents.Tier2.LadderEvents = ReferenceConfig.CareerRaceEvents.Tier2.LadderEvents;
            configToChange.CareerRaceEvents.Tier3.LadderEvents = ReferenceConfig.CareerRaceEvents.Tier3.LadderEvents;
            configToChange.CareerRaceEvents.Tier4.LadderEvents = ReferenceConfig.CareerRaceEvents.Tier4.LadderEvents;
            configToChange.CareerRaceEvents.Tier5.LadderEvents = ReferenceConfig.CareerRaceEvents.Tier5.LadderEvents;
        }

        if (UpdateAllSpecificRaces)
        {
            configToChange.CareerRaceEvents.Tier1.CarSpecificEvents =
                ReferenceConfig.CareerRaceEvents.Tier1.CarSpecificEvents;
            configToChange.CareerRaceEvents.Tier2.CarSpecificEvents =
                ReferenceConfig.CareerRaceEvents.Tier2.CarSpecificEvents;
            configToChange.CareerRaceEvents.Tier3.CarSpecificEvents =
                ReferenceConfig.CareerRaceEvents.Tier3.CarSpecificEvents;
            configToChange.CareerRaceEvents.Tier4.CarSpecificEvents =
                ReferenceConfig.CareerRaceEvents.Tier4.CarSpecificEvents;
            configToChange.CareerRaceEvents.Tier5.CarSpecificEvents =
                ReferenceConfig.CareerRaceEvents.Tier5.CarSpecificEvents;
        }
        CopyConfigUtils.SaveToFile(configToChange);
    }
}
