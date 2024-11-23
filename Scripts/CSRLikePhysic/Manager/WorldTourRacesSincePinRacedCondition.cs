using DataSerialization;

public class WorldTourRacesSincePinRacedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        string themeID = base.WorldTourThemeID(gameState, details);
        int pinScheduleRacesWonSinceStateChange = gameState.GetPinScheduleRacesWonSinceStateChange(themeID);
        ScheduledPinLifetimeData pinLifetimeData = gameState.GetPinLifetimeData(themeID, details.StringValue);
        int raceCountLastRacedAt = pinLifetimeData.RaceCountLastRacedAt;
        int value = pinScheduleRacesWonSinceStateChange - raceCountLastRacedAt;
        return base.IsInRange(value, details);
    }
}
