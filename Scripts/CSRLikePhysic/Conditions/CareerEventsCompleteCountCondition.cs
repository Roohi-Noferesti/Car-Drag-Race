using DataSerialization;

public class CareerEventsCompleteCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int num = 0;
		foreach (int current in details.IntValues)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(current))
			{
				num++;
			}
		}
		return base.IsInRange(num, details);
	}
}
