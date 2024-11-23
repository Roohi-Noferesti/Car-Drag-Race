using DataSerialization;

public class CompletedCarSpecificEventsPerTierCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
		switch (details.Tier)
		{
		case 1:
			return base.IsInRange(careerRaceEvents.Tier1.CarSpecificEvents.NumEventsComplete(), details);
		case 2:
			return base.IsInRange(careerRaceEvents.Tier2.CarSpecificEvents.NumEventsComplete(), details);
		case 3:
			return base.IsInRange(careerRaceEvents.Tier3.CarSpecificEvents.NumEventsComplete(), details);
		case 4:
			return base.IsInRange(careerRaceEvents.Tier4.CarSpecificEvents.NumEventsComplete(), details);
		case 5:
			return base.IsInRange(careerRaceEvents.Tier5.CarSpecificEvents.NumEventsComplete(), details);
		default:
			return false;
		}
	}
}
