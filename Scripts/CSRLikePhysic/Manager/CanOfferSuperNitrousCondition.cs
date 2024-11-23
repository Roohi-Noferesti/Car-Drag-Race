using DataSerialization;

public class CanOfferSuperNitrousCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return gameState.CanOfferSuperNitrous;
	}
}
