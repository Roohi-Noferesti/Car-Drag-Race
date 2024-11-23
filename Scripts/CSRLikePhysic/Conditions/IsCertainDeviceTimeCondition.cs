using System;
using DataSerialization;

public class IsCertainDeviceTimeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        DateTime utcNow = GTDateTime.Now;
		return utcNow.CompareTo(details.MinDateTime) > 0 && utcNow.CompareTo(details.MaxDateTime) < 0;
	}
}
