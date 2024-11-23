using DataSerialization;

public class WorldTourRaceResultCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string themeID = base.WorldTourThemeID(gameState, details);
		string sequenceID = details.SequenceID;
		string stringValue = details.StringValue;
		bool won = details.Won;
		int worldTourRaceResultCount = gameState.GetWorldTourRaceResultCount(themeID, sequenceID, stringValue, won);
		return base.IsInRange(worldTourRaceResultCount, details);
	}
}
