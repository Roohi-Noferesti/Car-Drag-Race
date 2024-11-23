using DataSerialization;

public class IsServerTimeValidCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return true;//ServerSynchronisedTime.Instance.ServerTimeValid;
	}
}
