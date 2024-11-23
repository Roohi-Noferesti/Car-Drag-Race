using DataSerialization;

public class CompletedEventsPerTierCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("EventsCompletedTier" + details.Tier), details);
	}
}
