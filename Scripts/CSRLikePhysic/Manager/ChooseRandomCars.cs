using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseRandomCars
{
	private const int DRIVER_BATTLE_CAR_CHOICE_MEMORY = 3;

	private Queue<string> LastChosenDriverBattleCars = new Queue<string>();

	public void ChoosePlayerLoneCar(out CarGarageInstance carGarageInstance, out CarUpgradeSetup setup)
	{
		List<eCarTier> carTiers = new List<eCarTier>
		{
			eCarTier.TIER_1,
			eCarTier.TIER_2,
			eCarTier.TIER_3,
			eCarTier.TIER_4,
			eCarTier.TIER_5
		};
		List<CarInfo> availableCars = this.getAvailableCars(carTiers);
		List<CarInfo> list = this.filterCarsWithPredefinedUpgradeSets(availableCars);
		list.RemoveAll((CarInfo q) => !q.IsAvailableInShowroomAndUpgrade);
		List<CarInfo> list2 = this.filterOwnedCars(list);
		if (list2.Count == 0)
		{
			list2 = list;
		}
		CarInfo randomDriverBattleCarExcludingLastTime = this.GetRandomDriverBattleCarExcludingLastTime(list2);
		PredefinedUpgradeSetsData randomUpgradeDataSet = this.GetRandomUpgradeDataSet(randomDriverBattleCarExcludingLastTime.PredefinedUpgradeSets.ToList<PredefinedUpgradeSetsData>());
		setup = new CarUpgradeSetup();
		setup.CarDBKey = randomDriverBattleCarExcludingLastTime.Key;
		randomUpgradeDataSet.FillUpgradeSetup(randomDriverBattleCarExcludingLastTime, ref setup);
		carGarageInstance = new CarGarageInstance();
	    var car = CarDatabase.Instance.GetCar(randomDriverBattleCarExcludingLastTime.Key);
		carGarageInstance.SetupNewGarageInstance(car);
		carGarageInstance.CurrentPPIndex = randomUpgradeDataSet.PPIndex;
	}

	public void ChooseCompletelyRandomCar(out CarGarageInstance carGarageInstance, out CarUpgradeSetup setup)
	{
		List<eCarTier> carTiers = new List<eCarTier>
		{
			eCarTier.TIER_1,
			eCarTier.TIER_2,
			eCarTier.TIER_3,
			eCarTier.TIER_4,
			eCarTier.TIER_5
		};
		List<CarInfo> availableCars = this.getAvailableCars(carTiers);
		List<CarInfo> list = this.filterCarsWithPredefinedUpgradeSets(availableCars);
		list.RemoveAll((CarInfo q) => !q.IsAvailableInShowroomAndUpgrade);
		int index = Random.Range(0, list.Count);
		CarInfo carInfo = list[index];
		PredefinedUpgradeSetsData randomUpgradeDataSet = this.GetRandomUpgradeDataSet(carInfo.PredefinedUpgradeSets.ToList<PredefinedUpgradeSetsData>());
		setup = new CarUpgradeSetup();
		setup.CarDBKey = carInfo.Key;
		randomUpgradeDataSet.FillUpgradeSetup(carInfo, ref setup);
		carGarageInstance = new CarGarageInstance();
		carGarageInstance.SetupNewGarageInstance(carInfo);
		carGarageInstance.CurrentPPIndex = randomUpgradeDataSet.PPIndex;
	}

	public void GetSwitchBackOpponentCar(ref CarGarageInstance carGarageInstance, out CarUpgradeSetup setup)
	{
		CarInfo car = CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey);
		PredefinedUpgradeSetsData fullyUpgradedSet = car.GetFullyUpgradedSet();
		setup = new CarUpgradeSetup();
		setup.CarDBKey = car.Key;
		fullyUpgradedSet.FillUpgradeSetup(car, ref setup);
		carGarageInstance.CurrentPPIndex = fullyUpgradedSet.PPIndex;
	}

	public void ChooseRandomOwnedCar(out CarGarageInstance carGarageInstance)
	{
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		int index = Random.Range(0, carsOwned.Count - 1);
		carGarageInstance = carsOwned[index];
	}

	private List<CarInfo> getAvailableCars(List<eCarTier> carTiers)
	{
		List<CarInfo> list = new List<CarInfo>();
		foreach (eCarTier current in carTiers)
		{
			list.AddRange(CarDatabase.Instance.GetCarsOfTier(current));
		}
		list.RemoveAll((CarInfo x) => !x.IsAvailableInShowroomAndUpgrade);
        //list.RemoveAll((CarInfo x) => !SeasonUtilities.CanCarBeUnlockedBySeason(x));
		list.RemoveAll((CarInfo x) => GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromRegulationRaces.Contains(x.Key));
		return list;
	}

	private List<CarInfo> filterOwnedCars(List<CarInfo> cars)
	{
		List<CarInfo> list = new List<CarInfo>(cars);
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		foreach (CarInfo car in cars)
		{
			if (carsOwned.Find((CarGarageInstance x) => x.CarDBKey == car.Key) != null)
			{
				list.RemoveAll((CarInfo x) => x.Key == car.Key);
			}
		}
		return list;
	}

	private List<CarInfo> filterCarsWithPredefinedUpgradeSets(List<CarInfo> cars)
	{
		List<CarInfo> list = new List<CarInfo>(cars);
		foreach (CarInfo car in cars)
		{
			if (car.PredefinedUpgradeSets.Count<PredefinedUpgradeSetsData>() == 0)
			{
				list.RemoveAll((CarInfo c) => c.Key == car.Key);
			}
		}
		return list;
	}

	private PredefinedUpgradeSetsData GetRandomUpgradeDataSet(List<PredefinedUpgradeSetsData> upgradeSets)
	{
		List<PredefinedUpgradeSetsData> list = new List<PredefinedUpgradeSetsData>(upgradeSets);
		list.RemoveAll((PredefinedUpgradeSetsData x) => x.UpgradeData.Contains(":"));
		if (list.Count == 0)
		{
			return upgradeSets.ElementAt(Random.Range(0, upgradeSets.Count));
		}
		return list.ElementAt(Random.Range(0, list.Count));
	}

	public string ChooseDriverBattleCar(eCarTier carTier)
	{
		if (carTier == eCarTier.TIER_X)
		{
			carTier = eCarTier.TIER_5;
		}
	    List<CarInfo> carsOfTier = carTier == eCarTier.TIER_5
	        ? CarDatabase.Instance.GetCarsOfTier(carTier)
	        : new List<CarInfo>();
		carsOfTier.AddRange(CarDatabase.Instance.GetCarsOfTier(carTier + 1));
		carsOfTier.AddRange(CarDatabase.Instance.GetCarsOfTier(carTier + 2));
		carsOfTier.RemoveAll((CarInfo x) => !x.IsAvailableInShowroomAndUpgrade);
		carsOfTier.RemoveAll((CarInfo x) => CarDataDefaults.IsBossCar(x.Key));
        //carsOfTier.RemoveAll((CarInfo x) => !SeasonUtilities.CanCarBeUnlockedBySeason(x));
		List<CarInfo> list = new List<CarInfo>(carsOfTier);
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		foreach (CarInfo availableCar in carsOfTier)
		{
			if (carsOwned.Find((CarGarageInstance x) => x.CarDBKey == availableCar.Key) != null)
			{
				list.RemoveAll((CarInfo x) => x.Key == availableCar.Key);
			}
		}
		CarInfo randomDriverBattleCarExcludingLastTime;
		if (list.Count == 0)
		{
			if (carsOfTier.Count == 0)
			{
				return this.ChooseDriverBattleCar(carTier - 1);
			}
			randomDriverBattleCarExcludingLastTime = this.GetRandomDriverBattleCarExcludingLastTime(carsOfTier);
		}
		else
		{
			randomDriverBattleCarExcludingLastTime = this.GetRandomDriverBattleCarExcludingLastTime(list);
		}
		return randomDriverBattleCarExcludingLastTime.Key;
	}

	public string ChooseRegulationRaceCar(eCarTier carTier, bool halfMile)
	{
		CarInfo randomRegulationRaceCar = CarDatabase.Instance.GetRandomRegulationRaceCar(carTier, halfMile);
		return randomRegulationRaceCar.Key;
	}

	private CarInfo GetRandomDriverBattleCarExcludingLastTime(List<CarInfo> listToChooseFrom)
	{
		CarInfo carInfo = null;
		int num = 0;
		while (num++ < 20)
		{
			int index = Random.Range(0, listToChooseFrom.Count);
			carInfo = listToChooseFrom[index];
			if (!this.LastChosenDriverBattleCars.Contains(carInfo.Key))
			{
				break;
			}
		}
		if (this.LastChosenDriverBattleCars.Count == 3)
		{
			this.LastChosenDriverBattleCars.Dequeue();
		}
		this.LastChosenDriverBattleCars.Enqueue(carInfo.Key);
		return carInfo;
	}
}
