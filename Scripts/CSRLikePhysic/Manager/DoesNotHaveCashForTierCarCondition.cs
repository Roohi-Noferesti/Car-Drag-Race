using DataSerialization;

public class DoesNotHaveCashForTierCarCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int lowestCashPriceInTier = CarDatabase.Instance.GetLowestCashPriceInTier((eCarTier)(details.Tier - 1));
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash() < lowestCashPriceInTier;
	}
}
