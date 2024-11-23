using System;
using DataSerialization;

public class TimePastSinceLastBundleOfferCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		TimeSpan t = gameState.GetPlayerProfileDate("LastBundleOfferTimeShown").Add(details.TimeSpanDifference).Subtract(GTDateTime.Now);
		return t < TimeSpan.Zero;
	}
}
