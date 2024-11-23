using DataSerialization;

public class LadderRacesCompleteCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
		LadderEvents ladderEvents = null;
		switch (details.Tier)
		{
		case 1:
			ladderEvents = careerRaceEvents.Tier1.LadderEvents;
			break;
		case 2:
			ladderEvents = careerRaceEvents.Tier2.LadderEvents;
			break;
		case 3:
			ladderEvents = careerRaceEvents.Tier3.LadderEvents;
			break;
		case 4:
			ladderEvents = careerRaceEvents.Tier4.LadderEvents;
			break;
		case 5:
			ladderEvents = careerRaceEvents.Tier5.LadderEvents;
			break;
		}
		return ladderEvents != null && base.IsInRange(ladderEvents.NumEventsComplete(), details);
	}
}
