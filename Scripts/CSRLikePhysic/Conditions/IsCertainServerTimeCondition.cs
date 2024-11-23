using System;
using DataSerialization;

public class IsCertainServerTimeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        if (!ServerSynchronisedTime.Instance.ServerTimeValid)
        {
            return false;
        }
        DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
        return base.IsInRange(dateTime, details);
	}
}
