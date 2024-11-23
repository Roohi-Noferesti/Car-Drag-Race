using DataSerialization;

public class CurrentWorldTourSequenceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CurrentWorldTourSequenceID == details.StringValue;
	}
}
