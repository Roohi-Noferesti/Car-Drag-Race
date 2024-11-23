using DataSerialization;

public class AlwaysFalseCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return false;
	}
}
