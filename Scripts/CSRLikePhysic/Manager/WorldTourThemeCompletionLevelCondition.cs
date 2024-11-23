using System.Collections.Generic;
using DataSerialization;

public class WorldTourThemeCompletionLevelCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        List<string> list = base.WorldTourThemeIDs(gameState, details);
        int value = 0;
        if (list.Count > 0)
        {
            value = list.MaxValue((string themeID) => (int)gameState.GetWorldTourThemeCompletionLevel(themeID));
        }
        return base.IsInRange(value, details);
	}
}
