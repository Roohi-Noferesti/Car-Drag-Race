using DataSerialization;

public class WorldTourSequenceLevelCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string themeID = base.WorldTourThemeID(gameState, details);
		int value = gameState.LastShownEventSequenceLevel(themeID, details.StringValue);
		return base.IsInRange(value, details);
	}
}
