using DataSerialization;

public class HasSeenTutorialBubbleCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetTutorialBubbleSeenCount(details.StringValue), details);
	}
}
