using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class ManufacturerSpecificEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    ManufacturerSpecific\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "ManufacturerSpecific";
    }

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
        if (zEvent.Restrictions[0].RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER)
        {
            return "Manufacturers/rest_" + zEvent.Restrictions[0].Manufacturer;
        }
        if (zEvent.Restrictions[1].RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER)
        {
            return "Manufacturers/rest_" + zEvent.Restrictions[1].Manufacturer;
        }
        return string.Empty;
        //return "Pin_manufacturer";
    }

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Star_Orange";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_MANUFACTURER");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(120f, 220f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_MANUFACTURER_SPECIFIC");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.ManufacturerSpecificRaceMultipliers;
	}

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#FFD900", out color);
        return color;
    }
}
