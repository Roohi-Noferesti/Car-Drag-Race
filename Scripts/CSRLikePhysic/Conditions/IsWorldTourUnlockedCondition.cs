using DataSerialization;

public class IsWorldTourUnlockedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return GameDatabase.Instance.Career.Configuration != null && GameDatabase.Instance.Career.IsWorldTourUnlocked();
	}
}
