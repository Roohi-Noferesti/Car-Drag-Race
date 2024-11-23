using System;
using UnityEngine;
using System.Collections;
using I2.Loc;

[Serializable]
public class TournamentRaceEvents : RaceEventTopLevelCategory 
{
    public override string ToString()
    {
        string str = "    TournamentRaceEvents\n";
        return str + base.ToString();
    }

    public override Vector2 GetOverlayOffset()
    {
        return new Vector2(0f, 0.01f);
    }

    public override string GetOverlayTextureName(RaceEventData zEvent)
    {
        return "Restrictions/Pin_tournament";
    }

    public override string GetBackgroundTextureName(RaceEventData zEvent)
    {
        return "Backgrounds/Star_Green";
    }

    public override Color GetBackgroundColor(RaceEventData zEvent)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#FF9900FF", out color);
        return color;
    }

    public override string GetProgressBarText(RaceEventData zEvent)
    {
        return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_TOURNAMENT_RACE");
    }

    public override Vector2 GetPinPosition()
    {
        return new Vector2(175f, 151f);
    }

    public override string GetPinString(RaceEventData zEvent)
    {
        return LocalizationManager.GetTranslation("TEXT_RACE_PIN_TOURNAMENT_RACE");
    }

    public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
    {
        return rewardsMultipliers.TournamentRaceMultipliers;
    }
}
