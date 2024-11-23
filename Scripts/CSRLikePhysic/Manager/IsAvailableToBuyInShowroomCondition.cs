using DataSerialization;

public class IsAvailableToBuyInShowroomCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsAvailableToBuyInShowroom(details.StringValue);
	}
}
