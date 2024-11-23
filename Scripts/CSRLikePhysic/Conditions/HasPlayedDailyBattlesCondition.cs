using DataSerialization;

public class HasPlayedDailyBattlesCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileDate("DailyBattlesLastEventAt"), details);
	}
}
