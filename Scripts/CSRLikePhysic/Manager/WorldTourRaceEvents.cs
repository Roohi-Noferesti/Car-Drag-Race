using System;
using DataSerialization;
using I2.Loc;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public class WorldTourRaceEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    WorldTourRaceEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "WorldTour";
    }

    public override Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0.01f);
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
        PinDetail worldTourPinPinDetail = zEvent.GetWorldTourPinPinDetail();
        return worldTourPinPinDetail.GetEventOverlayTexture();
    }

    public override string GetBackgroundTextureName(RaceEventData zEvent)
    {
        PinDetail worldTourPinPinDetail = zEvent.GetWorldTourPinPinDetail();
        return worldTourPinPinDetail.GetEventBackgroundTexture();
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
        PinDetail worldTourPinPinDetail = zEvent.GetWorldTourPinPinDetail();
        return LocalizationManager.GetTranslation(worldTourPinPinDetail.EventDescription);
    }

    public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
    {
        //PinDetail worldTourPinPinDetail = raceEvent.GetWorldTourPinPinDetail();
        //if (worldTourPinPinDetail != null)
        //{
        //    ScheduledPin worldTourScheduledPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
        //    if (worldTourScheduledPinInfo != null && !string.IsNullOrEmpty(worldTourScheduledPinInfo.ParentSequence.RewardsMultipliersID))
        //    {
        //        return rewardsMultipliers.WorldTourRaceMultipliers[worldTourScheduledPinInfo.ParentSequence.RewardsMultipliersID];
        //    }
        //}
        return rewardsMultipliers.DefaultWorldTourRaceMultipliers;
    }
}
