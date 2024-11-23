using DataSerialization;

public class IsCurrentSeasonCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int currentSeasonNumber = gameState.GetCurrentSeasonNumber();
		return base.IsInRange(currentSeasonNumber, details);
	}
}
