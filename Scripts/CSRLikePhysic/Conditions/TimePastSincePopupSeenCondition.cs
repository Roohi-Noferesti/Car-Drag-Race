using System;
using DataSerialization;

public class TimePastSincePopupSeenCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile.GetPopupSeenCount(details.IntValue) < 1)
		{
			return false;
		}
		TimeSpan t = PlayerProfileManager.Instance.ActiveProfile.GetPopupFirstSeenTime(details.IntValue).Add(details.TimeSpanDifference).Subtract(GTDateTime.Now);
		return t < TimeSpan.Zero;
	}
}
