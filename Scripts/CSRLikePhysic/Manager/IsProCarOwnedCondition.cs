using DataSerialization;

public class IsProCarOwnedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsProCarOwned(details.StringValue);
	}
}
