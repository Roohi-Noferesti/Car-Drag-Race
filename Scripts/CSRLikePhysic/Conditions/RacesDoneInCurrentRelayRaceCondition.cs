using DataSerialization;

public class RacesDoneInCurrentRelayRaceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		int racesDone = RelayManager.GetRacesDone();
		return base.IsInRange(racesDone, details);
	}
}
