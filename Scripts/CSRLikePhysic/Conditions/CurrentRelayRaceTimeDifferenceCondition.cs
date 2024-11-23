using DataSerialization;

public class CurrentRelayRaceTimeDifferenceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		float timeDifference = RelayManager.GetTimeDifference();
		return base.IsInRange(timeDifference, details);
	}
}
