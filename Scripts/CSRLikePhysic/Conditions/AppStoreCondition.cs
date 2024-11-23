using DataSerialization;

public class AppStoreCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return details.StringValues.Contains(gameState.AppStore());
	}
}
