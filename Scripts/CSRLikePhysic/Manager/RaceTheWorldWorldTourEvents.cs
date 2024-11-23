using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class RaceTheWorldWorldTourEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    RaceTheWorldWorldTourEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "RaceTheWorldWorldTour";
    }

    public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Circle_Teal_Pin";
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Multiplayer/pin_overlay_worldtour";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RACETHEWORLD_WORLDTOUR");
	}

	public override Vector2 GetPinPosition()
	{
		return Vector2.zero;
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RACETHEWORLD_WORLDTOUR_DESCRIPTION");
	}
}
