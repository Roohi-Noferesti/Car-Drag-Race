public class Leaderboard
{
	private int idx;

	private string categoryIDName;

	public int Idx
	{
		get
		{
			return this.idx;
		}
	}

	public string CategoryIDName
	{
		get
		{
			return this.categoryIDName;
		}
	}

	public bool IsStandardRace
	{
		get
		{
			return Leaderboards.IsStandardRace(this);
		}
	}

	public bool HigherIsBetter
	{
		get
		{
			return Leaderboards.HigherIsBetter(this);
		}
	}

	public Leaderboard(int inIdx, string gcCategoryIDname)
	{
		this.idx = inIdx;
		this.categoryIDName = gcCategoryIDname;
	}

	public override string ToString()
	{
		return this.CategoryIDName;
	}

    public bool Matches(GameCenterLeaderboard inGCLeaderboard)
    {
        return GameCenterCategoryIDs.Matches(this.categoryIDName, inGCLeaderboard.categoryId);
    }

	public int BestScore()
	{
		if (this.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Count)
		{
			return 0;
		}
		return PlayerProfileManager.Instance.ActiveProfile.PlayerScores[this.Idx].score;
	}
}
