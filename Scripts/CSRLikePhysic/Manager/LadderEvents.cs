using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class LadderEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    LadderEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "Ladder";
    }

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Restrictions/Pin_ladder";
	}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Star_Blue";
	}

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#AF30FFFF", out color);
        return color;
    }

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_LADDER");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(342.4f, 137f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_LADDER");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.LadderRaceMultipliers;
	}
}
