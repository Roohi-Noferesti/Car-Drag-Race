using DataSerialization;

public class FriendsCarHasWonStarCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//base.IsInRange((int)StarsManager.GetMyStarForCar(details.StringValue), details);
	}
}
