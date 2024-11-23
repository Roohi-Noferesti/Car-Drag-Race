using System;
using DataSerialization;

public class TimeSinceMultiplayerRaceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		DateTime playerProfileDate = gameState.GetPlayerProfileDate("LastPlayedMultiplayer");
		bool playerProfileBoolean = gameState.GetPlayerProfileBoolean("HasPlayedMultiplayer");
        return playerProfileBoolean && playerProfileDate.Add(details.TimeSpanDifference) < ServerSynchronisedTime.Instance.GetDateTime();
	}
}
