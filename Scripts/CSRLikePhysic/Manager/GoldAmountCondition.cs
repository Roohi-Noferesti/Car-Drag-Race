using DataSerialization;

public class GoldAmountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int currentGold = PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold();
		return base.IsInRange(currentGold, details);
	}
}
