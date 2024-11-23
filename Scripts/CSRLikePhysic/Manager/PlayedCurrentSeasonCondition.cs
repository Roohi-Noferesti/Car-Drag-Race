using DataSerialization;

public class PlayedCurrentSeasonCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    //int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
        //int seasonLastPlayedLeaderboardID = PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedLeaderboardID;
        //return mostRecentActiveSeasonLeaderboardID != -1 && mostRecentActiveSeasonLeaderboardID == seasonLastPlayedLeaderboardID;
	    return false;
	}
}
