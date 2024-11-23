using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class FriendRaceEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    FriendRaceEvents\n";
		return str + base.ToString();
	}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Circle_Orange_Pin";
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Multiplayer/pin_overlay_friends";
	}

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_FRIENDS");
	}

	public override Vector2 GetPinPosition()
	{
		return Vector2.zero;
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_FRIENDS_EVENT_DESCRIPTION");
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.RaceWithFriendsRaceMultipliers;
	}
}
