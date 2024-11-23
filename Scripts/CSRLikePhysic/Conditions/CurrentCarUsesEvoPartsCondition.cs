using DataSerialization;

public class CurrentCarUsesEvoPartsCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CurrentCarUsesEvoParts();
	}
}
