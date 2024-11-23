using DataSerialization;

public class WorldTourSequenceWonLastShownEventCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string themeID = base.WorldTourThemeID(gameState, details);
		if (details.StringValues.Count > 0)
		{
			foreach (string current in details.StringValues)
			{
				if (this.WonLastShownLevel(gameState, themeID, current))
				{
					return true;
				}
			}
			return false;
		}
		return this.WonLastShownLevel(gameState, themeID, details.StringValue);
	}

	private bool WonLastShownLevel(IGameState gameState, string themeID, string sequenceID)
	{
		int num = gameState.LastShownEventSequenceLevel(themeID, sequenceID);
		int num2 = gameState.LastWonEventSequenceLevel(themeID, sequenceID);
		return num2 == num && num2 >= 0;
	}
}
