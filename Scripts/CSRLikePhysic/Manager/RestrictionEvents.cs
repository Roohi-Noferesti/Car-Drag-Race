using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class RestrictionEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    RestrictionEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "Restriction";
    }

    public override string GetOverlayTextureName(RaceEventData zEvent)
    {
        //return "Restrictions/Pin_restriction";
        switch (zEvent.Restrictions[0].RestrictionType)
        {
            case eRaceEventRestrictionType.CAR_MANUFACTURER:
                return "Manufacturers/rest_" + zEvent.Restrictions[0].Manufacturer;
            case eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN:
                return "Restrictions/star_weight";
            case eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN:
                return "Restrictions/star_weight";
            case eRaceEventRestrictionType.CAR_DRIVE_WHEELS:
                if (zEvent.Restrictions[0].DriveWheels == eDriveType.FWD)
                {
                    return "Restrictions/Star_rest_FWD";
                }
                if (zEvent.Restrictions[0].DriveWheels == eDriveType.RWD)
                {
                    return "Restrictions/star_rest_RWD";
                }
                return "Restrictions/star_rest_4WD";
            case eRaceEventRestrictionType.CAR_MODEL:
                return "Cars/pin_" + zEvent.Restrictions[0].GetCarModels()[0];
            case eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED:
                return "Restrictions/star_rest_NOS_strike";
            case eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED:
                return "Restrictions/star_rest_tyre_strike";
            case eRaceEventRestrictionType.CAR_NITROUS_NEEDED:
                return "Restrictions/star_rest_NOS";
            case eRaceEventRestrictionType.CAR_TYRES_NEEDED:
                return "Restrictions/star_rest_tyre";
        }
        return "Restrictions/star_weight";
    }

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Star_Orange";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_RESTRICTION");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(426f, 188.5f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_RESTRICTION");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.RestrictionRaceMultipliers;
	}

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#FF3B3B", out color);
        return color;
    }
}
