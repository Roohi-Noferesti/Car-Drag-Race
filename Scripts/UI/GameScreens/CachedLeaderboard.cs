using System;
using Z2HSharedLibrary.DatabaseEntity;

public class CachedLeaderboard
{
    public LeaderboardRecord[] CachedRecords { get; private set; }
    public DateTime LastLeaderboardAt { get; private set; }

    public int TopCount { get; private set; }

    public bool IsCachedAvailableAndNotExpire()
    {
        return (CachedRecords != null && (GTDateTime.Now - LastLeaderboardAt).TotalMinutes < 5);
    }

    public void UpdateCache(LeaderboardRecord[] cached,int topCount)
    {
        CachedRecords = cached;
        TopCount = topCount;
        //LastLeaderboardAt = ServerSynchronisedTime.Now;
    }

    public void RenewTime()
    {
        LastLeaderboardAt = GTDateTime.Now;
    }

}