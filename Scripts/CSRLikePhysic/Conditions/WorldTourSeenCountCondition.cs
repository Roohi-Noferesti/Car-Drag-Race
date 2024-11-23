using DataSerialization;

public class WorldTourSeenCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int worldTourThemeSeenCount = gameState.GetWorldTourThemeSeenCount(details.ThemeID.ToLower());
		return base.IsInRange(worldTourThemeSeenCount, details);
	}
}
