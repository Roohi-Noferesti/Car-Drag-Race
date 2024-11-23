using DataSerialization;

public class WorldTourChoiceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string themeID = base.WorldTourThemeID(gameState, details);
		if (details.StringValues.Count != 2)
		{
			return false;
		}
		string sequenceID = details.StringValues[0];
		string pinID = details.StringValues[1];
		int value = gameState.ChoiceSelection(themeID, sequenceID, pinID);
		return base.IsInRange(value, details);
	}
}
