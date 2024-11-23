using System;

public class SeasonPrizeIdentifier
{
	public int PrizeID;

	public int LeaderboardID;

	public SeasonPrizeIdentifier()
	{
		this.PrizeID = -1;
		this.LeaderboardID = 0;
	}

	public SeasonPrizeIdentifier(int iD, int leaderboardID)
	{
		this.PrizeID = iD;
		this.LeaderboardID = leaderboardID;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		SeasonPrizeIdentifier seasonPrizeIdentifier = obj as SeasonPrizeIdentifier;
		return seasonPrizeIdentifier != null && this.Equals(seasonPrizeIdentifier);
	}

	public bool Equals(SeasonPrizeIdentifier otherIdentifier)
	{
		return this.LeaderboardID == otherIdentifier.LeaderboardID && this.PrizeID == otherIdentifier.PrizeID;
	}

	public override int GetHashCode()
	{
		return this.PrizeID.ToString().GetHashCode() ^ this.LeaderboardID;
	}
}
