using DataSerialization;

public class WorldTourSequenceCompleteCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int num = 0;
		string themeID = base.WorldTourThemeID(gameState, details);
		foreach (string current in details.StringValues)
		{
			int num2 = gameState.LastWonEventSequenceLevel(themeID, current);
			int num3 = gameState.GetEventCountInSequenceFromProfile(themeID, current) - 1;
			if (num2 == num3)
			{
				num++;
			}
		}
		return base.IsInRange(num, details);
	}
}
