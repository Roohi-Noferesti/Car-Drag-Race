using DataSerialization;

public class RacesWonCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("RacesWon"), details);
	}
}
