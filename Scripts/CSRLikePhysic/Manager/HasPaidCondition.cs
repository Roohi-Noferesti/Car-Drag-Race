using DataSerialization;

public class HasPaidCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.GetPlayerProfileBoolean("HasPaidForSomething");
	}
}
