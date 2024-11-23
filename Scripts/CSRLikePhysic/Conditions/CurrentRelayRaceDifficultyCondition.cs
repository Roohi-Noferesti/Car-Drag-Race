using DataSerialization;

public class CurrentRelayRaceDifficultyCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		float currentRelayRaceDifficulty = gameState.GetCurrentRelayRaceDifficulty();
		return base.IsInRange(currentRelayRaceDifficulty, details);
	}
}
