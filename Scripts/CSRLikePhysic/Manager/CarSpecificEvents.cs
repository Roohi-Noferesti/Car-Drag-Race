using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class CarSpecificEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    CarSpecificEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "CarSpecific";
    }

    public override string GetOverlayTextureName(RaceEventData zEvent)
	{
	    return "Cars/pin_" + zEvent.Restrictions[0].GetCarModels()[0];
	}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Pin_Purple";
	}

	public override Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0.08f);
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_CAR_SPECIFIC");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(54.5f, 191.3f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
	    var carname = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCar(zEvent.Restrictions[0].GetCarModels()[0]).ShortName);
	    return string.Format(LocalizationManager.GetTranslation("TEXT_RACE_PIN_CAR_SPECIFIC"),
	        carname);
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.CarSpecificRaceMultipliers;
	}
}
