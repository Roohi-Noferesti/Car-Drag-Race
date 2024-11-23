using DataSerialization;

public class IsCurrentScreenInListCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string currentScreenID = gameState.GetCurrentScreenID();
		return details.StringValues.Contains(currentScreenID);
	}
}
