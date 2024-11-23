using DataSerialization;

public class WorldTourLastSequenceWonCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    var themeID = details.ThemeID.ToLower();
        string worldTourLastSequenceRaced = gameState.GetWorldTourLastSequenceRaced(themeID);
		if (!string.IsNullOrEmpty(worldTourLastSequenceRaced) && (details.StringValue == worldTourLastSequenceRaced || details.StringValues.Contains(worldTourLastSequenceRaced)))
		{
			int num = gameState.LastRacedEventSequenceLevel(themeID, worldTourLastSequenceRaced);
			int num2 = gameState.LastWonEventSequenceLevel(themeID, worldTourLastSequenceRaced);
			return num2 == num;
		}
		return false;
	}
}
