using DataSerialization;

public class CarePackageRewardsReceivedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        string stringValue = details.StringValue;
        int value = GameDatabase.Instance.CarePackages.ReceivedRewardCount(stringValue);
        return base.IsInRange(value, details);
	}
}
