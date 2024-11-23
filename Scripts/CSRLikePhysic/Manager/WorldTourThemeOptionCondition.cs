using DataSerialization;

public class WorldTourThemeOptionCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CurrentWorldTourThemeOption == details.StringValue;
	}
}
