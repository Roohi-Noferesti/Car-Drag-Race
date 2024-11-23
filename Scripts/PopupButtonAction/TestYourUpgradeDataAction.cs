using DataSerialization;
using Metrics;

public class TestYourUpgradeDataAction : PopupDataActionBase 
{
    public override void Execute(EligibilityConditionDetails details)
    {
        CarInfoUI.Instance.ShowCarStats(false);
        var ppdif = GameDatabase.Instance.TutorialConfiguration.Tutorial3PPDifference;
        var playerNitrousLevel = GameDatabase.Instance.TutorialConfiguration.Tutorial3PlayerNitrousLevel;
        Log.AnEvent(Events.TestUpgrade);
        var raceEventData = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial3;
        var playerCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        AutoDifficulty.GetRandomOpponentForCarAtPP(ppdif, ref raceEventData,
            playerCar);
        //raceEventData.NitrousUpgradeLevel = 1;
        //var upgradeSetup = new CarUpgradeSetup();
        //upgradeSetup.UpgradeStatus = playerCar.UpgradeStatus;
        //raceEventData.AIDriver = "Tutorial_3_Driver";
        var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        currentCar.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = playerNitrousLevel;
        currentCar.UpgradeStatus[eUpgradeType.NITROUS].levelOwned = playerNitrousLevel;
        if (currentCar.CarDBKey == "car_peugeot_206")
        {
            raceEventData.AIDriver = "tutorial_3_driver";
            raceEventData.EngineUpgradeLevel = 0;
        }
        RaceEventInfo.Instance.PopulateForTutorial(raceEventData, playerCar.CarDBKey);
        IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial3);
        SceneManagerFrontend.ButtonStart();
    }
}
