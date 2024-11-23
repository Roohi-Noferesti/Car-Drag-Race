using DataSerialization;

public class HasSeenSeasonUnlockableThemeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.HasSeenSeasonUnlockableTheme(details.ThemeID.ToLower());
	}
}
