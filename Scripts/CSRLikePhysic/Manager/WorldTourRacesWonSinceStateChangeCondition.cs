using DataSerialization;

public class WorldTourRacesWonSinceStateChangeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		string themeID = base.WorldTourThemeID(gameState, details);
		int pinScheduleRacesWonSinceStateChange = gameState.GetPinScheduleRacesWonSinceStateChange(themeID);
		int minValue = details.MinValue;
		if (pinScheduleRacesWonSinceStateChange == minValue)
		{
			return true;
		}
		int num = pinScheduleRacesWonSinceStateChange - minValue;
		return num > 0 && num % details.IncrementValue == 0;
	}
}
