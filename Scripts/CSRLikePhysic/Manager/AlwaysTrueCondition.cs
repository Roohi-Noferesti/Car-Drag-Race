using DataSerialization;

public class AlwaysTrueCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return true;
	}
}
