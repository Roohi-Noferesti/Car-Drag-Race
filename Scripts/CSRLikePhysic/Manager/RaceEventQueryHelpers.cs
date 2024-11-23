using System;
using System.Collections.Generic;

public static class RaceEventQueryHelpers
{
    public static string GetManufacturerForManufacturerSpecificEvent(RaceEventGroup raceEventGroup)
	{
		if (!(raceEventGroup.Parent is ManufacturerSpecificEvents))
		{
			return "None";
		}
        var manufactureID = "None";
		foreach (RaceEventData current in raceEventGroup.RaceEvents)
		{
			var carManufacturerID = CarDatabase.Instance.GetCarManufacturerID(current.AICar);
            if (manufactureID != carManufacturerID && manufactureID != "None")
			{
                return "None";
			}
			manufactureID = carManufacturerID;
		}
		return manufactureID;
	}

	public static string GetCarForCarSpecificEventGroup(RaceEventGroup eventGroup)
	{
		if (!(eventGroup.Parent is CarSpecificEvents))
		{
			return string.Empty;
		}
		string text = string.Empty;
		foreach (RaceEventData current in eventGroup.RaceEvents)
		{
			if (text != current.AICar && text != string.Empty)
			{
				return string.Empty;
			}
			text = current.AICar;
		}
		return text;
	}

	public static RaceEventGroup GetFirstEventGroupForCarWeDontOwn(List<RaceEventGroup> raceEventGroups)
	{
		if (raceEventGroups == null || raceEventGroups.Count == 0)
		{
			return null;
		}
		if (!(raceEventGroups[0].Parent is CarSpecificEvents))
		{
			return null;
		}
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		foreach (RaceEventGroup current in raceEventGroups)
		{
			string carInThisEventGroupDBKey = RaceEventQueryHelpers.GetCarForCarSpecificEventGroup(current);
			string bossCarInThisEventGroupDBKey = carInThisEventGroupDBKey + "Boss";
			CarGarageInstance carGarageInstance = carsOwned.Find((CarGarageInstance x) => x.CarDBKey == carInThisEventGroupDBKey);
			CarGarageInstance carGarageInstance2 = carsOwned.Find((CarGarageInstance x) => x.CarDBKey == bossCarInThisEventGroupDBKey);
			if (carGarageInstance == null && carGarageInstance2 == null)
			{
				return current;
			}
		}
		return null;
	}

	public static RaceEventGroup GetFirstEventGroupForManufacturerWeDontOwn(List<RaceEventGroup> raceEventGroups)
	{
		if (raceEventGroups == null || raceEventGroups.Count == 0)
		{
			return null;
		}
		if (!(raceEventGroups[0].Parent is ManufacturerSpecificEvents))
		{
			return null;
		}
		RaceEventGroup result = null;
		foreach (RaceEventGroup current in raceEventGroups)
		{
			var manufacturerForManufacturerSpecificEvent = RaceEventQueryHelpers.GetManufacturerForManufacturerSpecificEvent(current);
			if (!RaceEventQueryHelpers.HaveCarFromThisManufacturer(manufacturerForManufacturerSpecificEvent))
			{
				result = current;
				break;
			}
		}
		return result;
	}

    private static bool HaveCarFromThisManufacturer(string manufacturerID)
	{
		List<CarInfo> allCarsFromManufacturer = CarDatabase.Instance.GetAllCarsFromManufacturer(manufacturerID);
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		foreach (CarGarageInstance car in carsOwned)
		{
			if (allCarsFromManufacturer.Find((CarInfo x) => x.Key == car.CarDBKey))
			{
				return true;
			}
		}
		return false;
	}
}
