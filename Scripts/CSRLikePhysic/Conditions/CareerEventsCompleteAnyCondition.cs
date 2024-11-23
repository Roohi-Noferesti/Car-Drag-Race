using DataSerialization;

public class CareerEventsCompleteAnyCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int count = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted.Count;
		return count > 0;
	}
}
