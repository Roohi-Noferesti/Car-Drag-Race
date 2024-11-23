using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Old-Class
//public static class AutoDifficulty
//{
//    public enum DifficultyRating
//    {
//        Easy = 0,
//        Challenging = 1,
//        Difficult = 2,
//        Extreme
//    }

//    public class AutoDifficultyCarUpgradeSetup
//    {
//        public CarUpgradeSetup UpgradeSet;

//        public int RepresentativePP;

//        public int MaximumPP;

//        public string AIDriver;
//    }

//    private const int PPDifferenceThreshhold = 3;

//    public static List<AutoDifficultyCarUpgradeSetup> GetOpponentsForCarAtDifficulty(int PPdiff, CarGarageInstance carGarageInstance, bool skipBodyMassModifier = false, bool skipPastMaxMassModifiers = false)
//    {
//        int currentPPIndex = carGarageInstance.CurrentPPIndex;
//        int num = currentPPIndex + PPdiff;
//        int num2 = 0;
//        List<AutoDifficultyCarUpgradeSetup> list = new List<AutoDifficultyCarUpgradeSetup>();
//        List<CarInfo> allCars = CarDatabase.Instance.GetAllCars();
//        allCars.RemoveAll((CarInfo x) => x.IsPrizeInCurrentSeason() || x.IsPrizeInFutureSeason() || (!x.IsAvailableToBuyInShowroom() && !x.IsUnlockedToBuy()));
//        allCars.RemoveAll((CarInfo x) => GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromMatchRaces.Contains(x.Key));
//        foreach (CarInfo current in allCars)
//        {
//            CarUpgradeSetup carUpgradeSetup = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(current, currentPPIndex, num, out num2, skipBodyMassModifier, skipPastMaxMassModifiers);
//            if (!string.IsNullOrEmpty(carUpgradeSetup.CarDBKey) && num2 == num)
//            {
//                list.Add(new AutoDifficultyCarUpgradeSetup
//                {
//                    UpgradeSet = carUpgradeSetup,
//                    RepresentativePP = num2,
//                    MaximumPP = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(current),
//                    AIDriver = "matchcar"
//                });
//            }
//        }
//        if (list.Count == 0)
//        {
//            string carDBKey = carGarageInstance.CarDBKey;
//            CarInfo car;
//            if (!CarDataDefaults.IsBossCar(carGarageInstance.CarDBKey))
//            {
//                car = CarDatabase.Instance.GetCar(carDBKey);
//            }
//            else
//            {
//                car = CarDatabase.Instance.GetCar(carDBKey.Substring(0, carDBKey.Length - 4));
//            }
//            AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = new AutoDifficultyCarUpgradeSetup();
//            CarUpgradeSetup upgradeSet;
//            if (num < currentPPIndex)//  currentPPIndex < 200)
//            {
//                upgradeSet = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(car, currentPPIndex, currentPPIndex, out num2, true, false);
//                autoDifficultyCarUpgradeSetup.AIDriver = "matchcar";
//                ;// "baddriver";
//            }
//            else
//            {
//                upgradeSet = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(car, currentPPIndex, num, out num2, false, false);
//                autoDifficultyCarUpgradeSetup.AIDriver = "matchcar";
//            }
//            autoDifficultyCarUpgradeSetup.UpgradeSet = upgradeSet;
//            autoDifficultyCarUpgradeSetup.RepresentativePP = num2;
//            autoDifficultyCarUpgradeSetup.MaximumPP = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(car);
//            list.Add(autoDifficultyCarUpgradeSetup);
//            if (string.IsNullOrEmpty(upgradeSet.CarDBKey))
//                upgradeSet.CarDBKey = car.Key;
//        }
//        return list;
//    }

//    public static List<AutoDifficultyCarUpgradeSetup> GetOpponentsForCarAtPP(int PP, CarGarageInstance carGarageInstance)
//    {
//        int pPdiff = PP - carGarageInstance.CurrentPPIndex;
//        return GetOpponentsForCarAtDifficulty(pPdiff, carGarageInstance, false, true);
//    }


//    public static void GetRandomOpponentForCarAtDifficulty(int pPdiff, ref RaceEventData eventData, CarGarageInstance carGarageInstance)
//    {
//        List<AutoDifficultyCarUpgradeSetup> opponentsForCarAtDifficulty = GetOpponentsForCarAtDifficulty(pPdiff, carGarageInstance, false, false);
//        int index = Random.Range(0, opponentsForCarAtDifficulty.Count - 1);
//        AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = opponentsForCarAtDifficulty[index];
//        eventData.AICar = autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey;
//        eventData.AIDriver = autoDifficultyCarUpgradeSetup.AIDriver;
//        eventData.BodyUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.BODY].levelFitted;
//        eventData.EngineUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
//        eventData.IntakeUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
//        eventData.NitrousUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
//        eventData.TransmissionUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
//        eventData.TurboUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
//        eventData.TyreUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
//        eventData.AIPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.RepresentativePP;
//        eventData.MaxPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.MaximumPP;
//        eventData.ModifiedCarMass = autoDifficultyCarUpgradeSetup.UpgradeSet.ModifiedCarMass;
//        CarInfo car = CarDatabase.Instance.GetCar(autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey);
//    }

//    public static void GetRandomOpponentForCarAtDifficulty(DifficultyRating difficulty,
//        ref RaceEventData eventData, CarGarageInstance carGarageInstance)
//    {
//        switch (difficulty)
//        {
//            case DifficultyRating.Easy:
//                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Easy;
//                break;
//            case DifficultyRating.Challenging:
//                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Easy;
//                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Challenging;
//                break;
//            case DifficultyRating.Difficult:
//                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Easy;
//                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Difficult;
//                break;
//        }

//        var pPdiff = GameDatabase.Instance.Z2HDifficultyDatabase.GetPPDelta(difficulty);
//        GetRandomOpponentForCarAtDifficulty(pPdiff, ref eventData, carGarageInstance);
//    }

//    public static void GetRandomOpponent(ref RaceEventData RaceEventData)
//    {
//        ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
//        CarGarageInstance carGarageInstance = new CarGarageInstance();
//        CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
//        chooseRandomCars.ChooseCompletelyRandomCar(out carGarageInstance, out carUpgradeSetup);
//        RaceEventData.AICar = carGarageInstance.CarDBKey;
//        RaceEventData.AIDriver = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("CarSpecificDriver_" + carGarageInstance.CarDBKey).AIDriverDBKey;
//        RaceEventData.BodyUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted;
//        RaceEventData.EngineUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
//        RaceEventData.IntakeUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
//        RaceEventData.NitrousUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
//        RaceEventData.TransmissionUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
//        RaceEventData.TurboUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
//        RaceEventData.TyreUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
//        RaceEventData.AIPerformancePotentialIndex = carGarageInstance.CurrentPPIndex;
//        RaceEventData.MaxPerformancePotentialIndex = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey));
//    }

//    public static void GetRandomOpponentForCarAtCertainTime(float quartermileTime, ref RaceEventData eventData, CarGarageInstance carGarageInstance)
//    {
//        int pP = CarPerformanceIndexCalculator.CalculatePerformanceIndexWorker(quartermileTime);
//        List<AutoDifficultyCarUpgradeSetup> opponentsForCarAtPP = GetOpponentsForCarAtPP(pP, carGarageInstance);
//        int index = Random.Range(0, opponentsForCarAtPP.Count - 1);
//        AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = opponentsForCarAtPP[index];
//        eventData.AICar = autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey;
//        eventData.AIDriver = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("CarSpecificDriver_" + autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey).AIDriverDBKey;
//        eventData.BodyUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.BODY].levelFitted;
//        eventData.EngineUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
//        eventData.IntakeUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
//        eventData.NitrousUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
//        eventData.TransmissionUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
//        eventData.TurboUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
//        eventData.TyreUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
//        eventData.AIPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.RepresentativePP;
//        eventData.MaxPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.MaximumPP;
//        eventData.ModifiedCarMass = autoDifficultyCarUpgradeSetup.UpgradeSet.ModifiedCarMass;
//    }
//}
#endregion

public static class AutoDifficulty
{
    public enum DifficultyRating
    {
        Easy,
        Challenging,
        Difficult,
        Extreme
    }

    public class AutoDifficultyCarUpgradeSetup
    {
        public CarUpgradeSetup UpgradeSet;

        public int RepresentativePP;

        public int MaximumPP;

        public string AIDriver;
    }

    private const int PPDifferenceThreshhold = 3;

    public static List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> GetOpponentsForCarAtDifficulty(int PPdiff, CarGarageInstance carGarageInstance, bool skipBodyMassModifier = false, bool skipPastMaxMassModifiers = false)
    {
        int currentPPIndex = carGarageInstance.CurrentPPIndex;
        int targetPPIndex = currentPPIndex + PPdiff;
        int closestPPIndexFound = 0;
        List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> list = new List<AutoDifficulty.AutoDifficultyCarUpgradeSetup>();
        List<CarInfo> allCars = CarDatabase.Instance.GetAllCars();
        allCars.RemoveAll((CarInfo x) => x.IsPrizeInCurrentSeason() || x.IsPrizeInFutureSeason() || (!x.IsAvailableToBuyInShowroom() && !x.IsUnlockedToBuy()));
        allCars.RemoveAll((CarInfo x) => GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromMatchRaces.Contains(x.Key));
        var carTier = CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey).BaseCarTier;
        //if (GameDatabase.Instance.Z2HDifficultyConfiguration.UseEqualOrLowerTierForMatchMaking)
        //{
        //    allCars.RemoveAll(c => c.BaseCarTier > carTier);
        //}
        foreach (CarInfo current in allCars)
        {
            CarUpgradeSetup carUpgradeSetup = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(current, currentPPIndex, targetPPIndex, out closestPPIndexFound, skipBodyMassModifier, skipPastMaxMassModifiers);
            //Debug.Log("check condition : " + closestPPIndexFound + "   " + targetPPIndex + "   " + currentPPIndex);
            if (!string.IsNullOrEmpty(carUpgradeSetup.CarDBKey) && closestPPIndexFound==targetPPIndex)//closestPPIndexFound <= targetPPIndex && closestPPIndexFound > targetPPIndex - 3)
            {
                //Debug.Log("meet condition : " + closestPPIndexFound + "   " + targetPPIndex + "   " + currentPPIndex);
                list.Add(new AutoDifficulty.AutoDifficultyCarUpgradeSetup
                {
                    UpgradeSet = carUpgradeSetup,
                    RepresentativePP = closestPPIndexFound,
                    MaximumPP = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(current),
                    AIDriver = "matchcar"
                });
            }
        }
        if (list.Count == 0)
        {
            string carDBKey = carGarageInstance.CarDBKey;
            CarInfo car;

            if (!CarDataDefaults.IsBossCar(carDBKey))
            {
                car = CarDatabase.Instance.GetCar(carDBKey);
            }
            else
            {
                car = CarDatabase.Instance.GetCar(carDBKey.Substring(0, carDBKey.Length - 4));
            }
            AutoDifficulty.AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = new AutoDifficulty.AutoDifficultyCarUpgradeSetup();
            CarUpgradeSetup upgradeSet;
            var rating = GameDatabase.Instance.Z2HDifficultyDatabase.GetDifficulty(PPdiff, car.BaseCarTier);
            if (currentPPIndex < 200)
            {
                upgradeSet = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(car, currentPPIndex, currentPPIndex, out closestPPIndexFound, true, false);
                autoDifficultyCarUpgradeSetup.AIDriver = "baddriver";
            }
            else
            {
                upgradeSet = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(car, currentPPIndex, targetPPIndex, out closestPPIndexFound, false, false);
                autoDifficultyCarUpgradeSetup.AIDriver = "matchcar";//rating == DifficultyRating.Easy || relativePP ? "baddriver" : "matchcar";
            }
            //if (autoDifficultyCarUpgradeSetup.AIDriver == "baddriver")
            //{
            //    GTDebug.LogWarning(GTLogChannel.AutoDifficulty, "bad driver set for opponent at requested ppindex : " + targetPPIndex
            //                          + "for car : " + car.Key);
            //}
            upgradeSet.CarDBKey = car.Key;
            autoDifficultyCarUpgradeSetup.UpgradeSet = upgradeSet;
            autoDifficultyCarUpgradeSetup.RepresentativePP = closestPPIndexFound;
            autoDifficultyCarUpgradeSetup.MaximumPP = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(car);
            list.Add(autoDifficultyCarUpgradeSetup);
        }
        return list;
    }

    public static List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> GetOpponentsForCarAtPP(int PP, CarGarageInstance carGarageInstance)
    {
        int pPdiff = PP - carGarageInstance.CurrentPPIndex;
        return AutoDifficulty.GetOpponentsForCarAtDifficulty(pPdiff, carGarageInstance, false, true);
    }


	public static void GetRandomOpponentForCarAtPP(int pPdiff, ref RaceEventData eventData, CarGarageInstance carGarageInstance)
	{
		List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> opponentsForCarAtDifficulty = AutoDifficulty.GetOpponentsForCarAtDifficulty(pPdiff, carGarageInstance, false, false);
		int index = UnityEngine.Random.Range(0, opponentsForCarAtDifficulty.Count - 1);
		AutoDifficulty.AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = opponentsForCarAtDifficulty[index];
        //LogUtility.Log("found : "+eventData.AICar+","+autoDifficultyCarUpgradeSetup.RepresentativePP + "   player : " + carGarageInstance.CurrentPPIndex+"   diff : "+pPdiff);
		eventData.AICar = autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey;
		eventData.AIDriver = autoDifficultyCarUpgradeSetup.AIDriver;
		eventData.BodyUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.BODY].levelFitted;
		eventData.EngineUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
		eventData.IntakeUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
		eventData.NitrousUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
		eventData.TransmissionUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
		eventData.TurboUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
		eventData.TyreUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
		eventData.AIPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.RepresentativePP;
		eventData.MaxPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.MaximumPP;
		eventData.ModifiedCarMass = autoDifficultyCarUpgradeSetup.UpgradeSet.ModifiedCarMass;
        GTDebug.Log(GTLogChannel.AutoDifficulty, "Searchinf for ppdiff:" + pPdiff + ",found : " + autoDifficultyCarUpgradeSetup.RepresentativePP);
		//CarInfo car = CarDatabase.Instance.GetCar(autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey);
	}

    public static void GetRandomOpponentForCarAtDifficulty(AutoDifficulty.DifficultyRating difficulty,
        ref RaceEventData eventData, CarGarageInstance carGarageInstance)
    {
        int pPdiff = 0;
        switch (difficulty)
        {
            case AutoDifficulty.DifficultyRating.Easy:
                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Easy;
                break;
            case AutoDifficulty.DifficultyRating.Challenging:
                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Challenging;
                break;
            case AutoDifficulty.DifficultyRating.Difficult:
                pPdiff = TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Difficult;
                break;
        }
        GetRandomOpponentForCarAtPP(pPdiff, ref eventData, carGarageInstance);
    }

    public static void GetRandomOpponentForCarAtDifficultyOnline(OnlineRace onlineRace, ref RaceEventData eventData, CarGarageInstance carGarageInstance)
    {
        var carTier = CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey).BaseCarTier;
        var pPdiff = GameDatabase.Instance.Online.GetPPDeltaOnline(onlineRace, carTier);
        GetRandomOpponentForCarAtPP(pPdiff, ref eventData, carGarageInstance);
    }

    public static void GetRandomOpponent(ref RaceEventData RaceEventData)
    {
        ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
        CarGarageInstance carGarageInstance = new CarGarageInstance();
        CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
        chooseRandomCars.ChooseCompletelyRandomCar(out carGarageInstance, out carUpgradeSetup);
        RaceEventData.AICar = carGarageInstance.CarDBKey;
        RaceEventData.AIDriver = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("CarSpecificDriver_" + carGarageInstance.CarDBKey).AIDriverDBKey;
        RaceEventData.BodyUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted;
        RaceEventData.EngineUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
        RaceEventData.IntakeUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
        RaceEventData.NitrousUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
        RaceEventData.TransmissionUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
        RaceEventData.TurboUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
        RaceEventData.TyreUpgradeLevel = carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
        RaceEventData.AIPerformancePotentialIndex = carGarageInstance.CurrentPPIndex;
        RaceEventData.MaxPerformancePotentialIndex = PredefinedUpgradeSetsData.MaximumPPFromUpgrades(CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey));
    }

    public static void GetRandomOpponentForCarAtCertainTime(float quartermileTime, ref RaceEventData eventData, CarGarageInstance carGarageInstance)
    {
        int pP = CarPerformanceIndexCalculator.CalculatePerformanceIndexWorker(quartermileTime);
        List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> opponentsForCarAtPP = AutoDifficulty.GetOpponentsForCarAtPP(pP, carGarageInstance);
        int index = UnityEngine.Random.Range(0, opponentsForCarAtPP.Count - 1);
        AutoDifficulty.AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = opponentsForCarAtPP[index];
        eventData.AICar = autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey;
        eventData.AIDriver = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("CarSpecificDriver_" + autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey).AIDriverDBKey;
        eventData.BodyUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.BODY].levelFitted;
        eventData.EngineUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
        eventData.IntakeUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
        eventData.NitrousUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
        eventData.TransmissionUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
        eventData.TurboUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
        eventData.TyreUpgradeLevel = autoDifficultyCarUpgradeSetup.UpgradeSet.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
        eventData.AIPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.RepresentativePP;
        eventData.MaxPerformancePotentialIndex = autoDifficultyCarUpgradeSetup.MaximumPP;
        eventData.ModifiedCarMass = autoDifficultyCarUpgradeSetup.UpgradeSet.ModifiedCarMass;
    }
}




