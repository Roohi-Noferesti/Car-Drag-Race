using DataSerialization;

public class NumberOfCarsInListOwned : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int num = 0;
		foreach (string current in details.StringValues)
		{
			if (gameState.IsCarOwned(current))
			{
				num++;
			}
		}
		return base.IsInRange(num, details);
	}
}
