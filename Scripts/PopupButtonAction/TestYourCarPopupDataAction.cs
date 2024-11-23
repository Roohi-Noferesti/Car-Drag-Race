using DataSerialization;
using Metrics;
using UnityEngine;

public class TestYourCarPopupDataAction : PopupDataActionBase 
{
    public override void Execute(EligibilityConditionDetails details)
    {
        var ppdif = GameDatabase.Instance.TutorialConfiguration.Tutorial2PPDifference;
        Log.AnEvent(Events.NowDo2ndRace);
        var raceEventData = (RaceEventData)GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2;//.Clone();
        var playerCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        AutoDifficulty.GetRandomOpponentForCarAtPP(ppdif, ref raceEventData,
            playerCar);
        raceEventData.AIDriver = "matchcar";
        raceEventData.NitrousUpgradeLevel += 5;
        raceEventData.NitrousUpgradeLevel = (byte) Mathf.Clamp(raceEventData.NitrousUpgradeLevel, 0, 5);

        if (playerCar.CarDBKey == "car_mazda_3")
        {
            raceEventData.EngineUpgradeLevel += 2;
            raceEventData.EngineUpgradeLevel = (byte)Mathf.Clamp(raceEventData.EngineUpgradeLevel, 0, 5);
        }
        RaceEventInfo.Instance.PopulateForTutorial(raceEventData, playerCar.CarDBKey);
        IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial2);
        SceneManagerFrontend.ButtonStart();
    }
}
