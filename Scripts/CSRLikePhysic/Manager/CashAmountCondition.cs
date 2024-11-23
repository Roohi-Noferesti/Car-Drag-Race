using DataSerialization;

public class CashAmountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int currentCash = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
		return base.IsInRange(currentCash, details);
	}
}
