using System.Collections.Generic;
using DataSerialization;

public class WorldTourSequenceCompleteCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		List<string> stringValues = base.WorldTourThemeIDs(gameState, details);
		return base.AreMatchesValid(stringValues, new IsStringMatchValidDelegate(this.SequenceComplete), gameState, details);
	}

	private bool SequenceComplete(string theme, IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.IsSequenceComplete(theme, details.StringValue);
	}
}
