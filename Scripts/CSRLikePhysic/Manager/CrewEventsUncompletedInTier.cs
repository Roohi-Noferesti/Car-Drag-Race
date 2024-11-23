using DataSerialization;

public class CrewEventsUncompletedInTier : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		RaceEventData crewBattleEvent = RaceEventQuery.Instance.GetCrewBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents((eCarTier)(details.Tier - 1)), false);
		int value;
		if (crewBattleEvent != null)
		{
			value = crewBattleEvent.Parent.NumOfEvents() - crewBattleEvent.Parent.NumEventsComplete();
		}
		else
		{
			value = 0;
		}
		return base.IsInRange(value, details);
	}
}
