public class Achievement
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

	public Achievement(int inIdx, string gcCategoryIDname)
	{
		this.idx = inIdx;
		this.categoryIDName = gcCategoryIDname;
	}

	public override string ToString()
	{
		return this.CategoryIDName;
	}

    public bool Matches(GameCenterAchievement inGCAchievement)
    {
        return GameCenterCategoryIDs.Matches(this.categoryIDName, inGCAchievement.categoryId);
    }
}
