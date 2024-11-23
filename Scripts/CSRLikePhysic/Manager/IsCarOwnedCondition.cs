using DataSerialization;

public class IsCarOwnedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsCarOwned(details.StringValue);
	}
}
