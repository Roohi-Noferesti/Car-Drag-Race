using DataSerialization;

public class CurrentWorldTourThemeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CurrentWorldTourThemeID == details.ThemeID.ToLower();
	}
}
