using DataSerialization;

public class HasVisitedMechanicScreenCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.GetPlayerProfileBoolean("HasVisitedMechanicScreen");
	}
}
