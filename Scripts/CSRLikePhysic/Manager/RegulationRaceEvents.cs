using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class RegulationRaceEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    RegulationRaceEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "Regulation";
    }

    public override Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0.01f);
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
        return "Restrictions/Pin_regulationrace";
	}

    public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Star_Green";
	}

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#00FFBFFF", out color);
        return color;
    }

    public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_REGULATION_RACE");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(175f, 151f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_REGULATION_RACE");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.RegulationRaceMultipliers;
	}
}
