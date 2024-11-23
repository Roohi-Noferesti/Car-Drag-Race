using DataSerialization;

public class CurrentMapPaneSelectedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return true;//CareerModeMapScreen.mapPaneSelected == details.IntValue;
	}
}
