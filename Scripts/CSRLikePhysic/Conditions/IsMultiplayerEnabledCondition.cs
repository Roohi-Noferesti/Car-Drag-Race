using DataSerialization;

public class IsMultiplayerEnabledCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsMultiplayerEnabled();
	}
}
