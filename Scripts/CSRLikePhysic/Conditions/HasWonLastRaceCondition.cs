using DataSerialization;

public class HasWonLastRaceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return RaceResultsTracker.You.IsWinner;
	}
}
