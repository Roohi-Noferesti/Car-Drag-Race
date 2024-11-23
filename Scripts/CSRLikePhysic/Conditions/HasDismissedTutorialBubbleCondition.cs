using DataSerialization;

public class HasDismissedTutorialBubbleCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.HasDismissedTutorialBubble(details.StringValue);
	}
}
