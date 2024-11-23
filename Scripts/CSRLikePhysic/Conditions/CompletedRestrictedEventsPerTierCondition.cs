using DataSerialization;

public class CompletedRestrictedEventsPerTierCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
		switch (details.Tier)
		{
		case 1:
			return base.IsInRange(careerRaceEvents.Tier1.RestrictionEvents.NumEventsComplete(), details);
		case 2:
			return base.IsInRange(careerRaceEvents.Tier2.RestrictionEvents.NumEventsComplete(), details);
		case 3:
			return base.IsInRange(careerRaceEvents.Tier3.RestrictionEvents.NumEventsComplete(), details);
		case 4:
			return base.IsInRange(careerRaceEvents.Tier4.RestrictionEvents.NumEventsComplete(), details);
		case 5:
			return base.IsInRange(careerRaceEvents.Tier5.RestrictionEvents.NumEventsComplete(), details);
		default:
			return false;
		}
	}
}
