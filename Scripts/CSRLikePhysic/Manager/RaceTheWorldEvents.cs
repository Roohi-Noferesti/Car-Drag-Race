using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class RaceTheWorldEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    RaceTheWorldEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "RaceTheWorld";
    }

    public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Circle_Pink_Pin";
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Multiplayer/pin_overlay_rtw";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RACETHEWORLD");
	}

	public override Vector2 GetPinPosition()
	{
		return Vector2.zero;
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RACETHEWORLD_DESCRIPTION");
	}
}
