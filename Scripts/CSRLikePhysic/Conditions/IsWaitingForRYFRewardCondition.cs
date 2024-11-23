using DataSerialization;

public class IsWaitingForRYFRewardCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//FriendsRewardManager.Instance.HasToGiveRewards();
	}
}
