using DataSerialization;

public class HasSeenPopupCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(PlayerProfileManager.Instance.ActiveProfile.GetPopupSeenCount(details.IntValue), details);
	}
}
