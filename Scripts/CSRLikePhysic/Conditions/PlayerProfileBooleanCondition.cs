using DataSerialization;

public class PlayerProfileBooleanCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.GetPlayerProfileBoolean(details.StringValue);
	}
}
