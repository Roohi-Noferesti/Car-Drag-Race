public static class LeaderboardExtension
{
    public static bool IsWeekly(this LegacyLeaderboardManager.LeaderboardType leaderboardType)
    {
        return leaderboardType == LegacyLeaderboardManager.LeaderboardType.Weekly
               || leaderboardType == LegacyLeaderboardManager.LeaderboardType.PastWeekly;
    }
}