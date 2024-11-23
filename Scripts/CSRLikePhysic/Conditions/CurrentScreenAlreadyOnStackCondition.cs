using DataSerialization;

public class CurrentScreenAlreadyOnStackCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CurrentScreenAlreadyOnStack;
	}
}
