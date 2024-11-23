using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class CareerConfiguration:ScriptableObject
{
	public RaceEventDatabaseData CareerRaceEvents;

	public int FuelReplenishTime = 360;

	public int FuelReplenishMaxDisplay = 599940;

	public int FuelCheatPrevention = 1;

	public int MinimumUpgradeNagSeparationTime = 24;

	public eCarTier TierToUnlockMultiplayer;

	public bool LockMultiplayerForNewUsers;

	public string WelcomeMessage;

	public string WelcomeMessageTitle;

	public int WelcomeMessageId;

	public string WelcomeMessageDynamicImageAsset = string.Empty;

	public int EventIDToUnlockWorldTour;

    public int LeaderboardUnlockLevel = 4;

    public int CustomiseUnlockLevel = 3;

    [Conditional("UNITY_EDITOR")]
	public void AddWorldTourEvents(CareerConfiguration extraConfiguration)
	{
		List<RaceEventGroup> raceEventGroups = extraConfiguration.CareerRaceEvents.TierX.WorldTourRaceEvents.RaceEventGroups;
		List<RaceEventGroup> raceEventGroups2 = this.CareerRaceEvents.TierX.WorldTourRaceEvents.RaceEventGroups;
		raceEventGroups2.AddRange(raceEventGroups);
	}
}
