using DataSerialization;

public class HasWonMultiplayerOnlineRaceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.GetPlayerProfileInteger("OnlineRacesWon") > 0;
	}
}
