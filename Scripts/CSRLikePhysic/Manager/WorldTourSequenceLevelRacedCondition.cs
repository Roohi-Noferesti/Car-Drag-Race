using System.Collections.Generic;
using DataSerialization;

public class WorldTourSequenceLevelRacedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        List<string> list = base.WorldTourThemeIDs(gameState, details);
        int value = -1;
        if (list.Count > 0)
        {
            value = list.MaxValue((string t) => gameState.LastRacedEventSequenceLevel(t, details.StringValue));
        }
        return base.IsInRange(value, details);
    }
}
