using DataSerialization;

public class IsRelayPlayerCarFullyUpgradedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsRelayCarFullyUpgraded(details.IntValue);
	}
}
