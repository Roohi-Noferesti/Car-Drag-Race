using DataSerialization;

public class CarOwnedCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Count, details);
	}
}
