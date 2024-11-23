using System.Collections.Generic;

public class EventLeaderboard
{
	public int leaderboard_id;

	public int previousSeasonRank;

	public int previousSeasonRankPercentile;

	public int currentSeasonRankPercentile;

	public int currentSeasonRank;

	public int currentSeasonRP;

	public List<EventPeerRanking> peersBelow = new List<EventPeerRanking>();

	public List<EventPeerRanking> peersAbove = new List<EventPeerRanking>();

	public List<EventPeerRanking> topTenEntries = new List<EventPeerRanking>();
}
