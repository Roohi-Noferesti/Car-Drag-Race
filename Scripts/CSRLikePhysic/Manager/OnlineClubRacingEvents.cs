using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class OnlineClubRacingEvents : RaceEventTopLevelCategory
{
    public override string ToString()
    {
        string str = "    OnlineClubRacingEvents\n";
        return str + base.ToString();
    }

    public override string GetBackgroundTextureName(RaceEventData zEvent)
    {
        return "Backgrounds/Circle_Blue_Pin";
    }

    public override string GetOverlayTextureName(RaceEventData zEvent)
    {
        return "Multiplayer/pin_overlay_clubrace";
    }

    public override string GetProgressBarText(RaceEventData zEvent)
    {
        return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_CLUBRACING");
    }

    public override Vector2 GetPinPosition()
    {
        return Vector2.zero;
    }

    public override string GetPinString(RaceEventData zEvent)
    {
        return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_CLUBRACING_DESCRIPTION");
    }

    public override Vector2 GetOverlayOffset()
    {
        return new Vector2(0f, 0f);
    }
}
