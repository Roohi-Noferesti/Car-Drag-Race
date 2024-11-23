[System.Serializable]
public class RaceStarReward
{
    public LeagueStarReward RegularLeagueReward;
    public LeagueStarReward BronzeLeagueReward;
    public LeagueStarReward SilverLeagueReward;
    public LeagueStarReward GoldenLeagueReward;
    public LeagueStarReward DiamondLeagueReward;

    public LeagueStarReward GetLeagueRewardByLeagueName(LeagueData.LeagueName leagueName)
    {
        LeagueStarReward resutReward = null;
        switch (leagueName)
        {
            case LeagueData.LeagueName.Regular:
                resutReward =  RegularLeagueReward;
                break;
            case LeagueData.LeagueName.Bronze:
                resutReward =  BronzeLeagueReward;
                break;
            case LeagueData.LeagueName.Silver:
                resutReward =  SilverLeagueReward;
                break;
            case LeagueData.LeagueName.Golden:
                resutReward =  GoldenLeagueReward;
                break;
            case LeagueData.LeagueName.Diamond:
                resutReward =  DiamondLeagueReward;
                break;
        }

        if (resutReward != null)
        {
            if (resutReward.LoseStar == 0)
            {
                resutReward.LoseStar = -15;
            }

            if (resutReward.WinStar == 0)
            {
                resutReward.WinStar = 30;
            }
        }

        return resutReward;
    }
}