using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class DailyBattleEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    DailyBattleEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "DailyBattle";
    }

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Restrictions/Pin_battle";
	}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Pin_Brown";
	}

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#0095FFFF", out color);
        return color;
    }

    public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_DAILY_BATTLE");
	}

	public override Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0f);
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(100f, 120.6f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_NEW_DAILY_BATTLE");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.DailyBattleMultipliers;
	}
}
