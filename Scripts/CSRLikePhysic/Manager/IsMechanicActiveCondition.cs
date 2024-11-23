using DataSerialization;

public class IsMechanicActiveCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsMechanicActive();
	}
}
