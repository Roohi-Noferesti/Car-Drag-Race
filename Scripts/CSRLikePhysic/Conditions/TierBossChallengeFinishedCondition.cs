using DataSerialization;

public class TierBossChallengeFinishedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return BoostNitrous.TierBossChallengeFinished((eCarTier)(details.Tier - 1));
	}
}
