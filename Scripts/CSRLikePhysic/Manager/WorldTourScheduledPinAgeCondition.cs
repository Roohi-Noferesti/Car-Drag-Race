using DataSerialization;

public class WorldTourScheduledPinAgeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    //string themeID = base.WorldTourThemeID(gameState, details);
        //int pinScheduleRacesWonSinceStateChange = gameState.GetPinScheduleRacesWonSinceStateChange(themeID);
        //ScheduledPinLifetimeData pinLifetimeData = gameState.GetPinLifetimeData(themeID, details.StringValue);
        //int num = pinLifetimeData.RaceCountFirstShownAt;
        //if (num < 0)
        //{
        //    num = pinScheduleRacesWonSinceStateChange;
        //}
        //int value = pinScheduleRacesWonSinceStateChange - num;
        //return base.IsInRange(value, details);
	    return false;
	}
}
