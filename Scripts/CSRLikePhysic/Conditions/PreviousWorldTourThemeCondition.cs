using DataSerialization;

public class PreviousWorldTourThemeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.PreviousWorldTourThemeID == details.ThemeID.ToLower();
	}
}
