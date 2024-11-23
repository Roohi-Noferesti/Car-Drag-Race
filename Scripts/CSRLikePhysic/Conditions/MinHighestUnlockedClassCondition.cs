using DataSerialization;

public class MinHighestUnlockedClassCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return RaceEventQuery.Instance.getHighestUnlockedClass() >= (eCarTier)(details.Tier - 1);
	}
}
