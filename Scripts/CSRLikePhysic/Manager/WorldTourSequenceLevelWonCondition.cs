using System.Collections.Generic;
using DataSerialization;

public class WorldTourSequenceLevelWonCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		List<string> themes = base.WorldTourThemeIDs(gameState, details);
		int value = -1;
		if (themes.Count > 0)
		{
			if (details.StringValues.Count > 0)
			{
				value = details.StringValues.MaxValue((string sequenceID) => themes.MaxValue((string t) => gameState.LastWonEventSequenceLevel(t, sequenceID)));
			}
			else
			{
				value = themes.MaxValue((string t) => gameState.LastWonEventSequenceLevel(t, details.StringValue));
			}
		}
		return base.IsInRange(value, details);
	}
}
