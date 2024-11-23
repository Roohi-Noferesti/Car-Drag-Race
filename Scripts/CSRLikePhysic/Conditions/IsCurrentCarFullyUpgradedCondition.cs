using DataSerialization;

public class IsCurrentCarFullyUpgradedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsCurrentCarFullyUpgraded();
	}
}
