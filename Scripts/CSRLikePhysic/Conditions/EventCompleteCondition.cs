using DataSerialization;

public class EventCompleteCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(details.IntValue);
	}
}
