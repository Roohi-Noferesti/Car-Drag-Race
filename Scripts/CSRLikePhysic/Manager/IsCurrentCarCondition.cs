using DataSerialization;

public class IsCurrentCarCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsCurrentCar(details.StringValue);
	}
}
