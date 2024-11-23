using DataSerialization;

public class VersusScreenModeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.GetVersusScreenMode() == details.StringValue;
	}
}
