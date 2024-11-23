using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class SMPRaceEvents : RaceEventTopLevelCategory
{
    //public override RaceEventCategory EventCategory
    //{
    //    get
    //    {
    //        return RaceEventCategory.SMPRace;
    //    }
    //}

    //public override string GetBackgroundSpriteName(RaceEventData eventData)
    //{
    //    return "CentralHubBadge";
    //}

    //public override string GetTypeSpriteName(RaceEventData eventData)
    //{
    //    return "LobbyIcon";
    //}

    //public override string GetAudioEventName()
    //{
    //    return AudioEvent.MapPinSelect_SMPRace;
    //}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Star_Orange";
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
        return "Restrictions/Pin_Online_Race";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_SMP_PROGRESS_BAR");
	}

	public override Vector2 GetPinPosition()
	{
		return Vector2.zero;
	}

	public override string GetPinString(RaceEventData zEvent)
	{
        return LocalizationManager.GetTranslation("TEXT_RACE_PIN_SMP_LOBBY");
	}

    public override string GetName()
    {
        return "SMPRace";
    }
}
