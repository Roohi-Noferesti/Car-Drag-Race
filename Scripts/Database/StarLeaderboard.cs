public class StarLeaderboard : Leaderboard
{
    public int MinStars { get; private set; }
    public int MaxStars { get; private set; }

    public StarLeaderboard(int inIdx, string gcCategoryIDname,int minStars,int maxStars) : base(inIdx, gcCategoryIDname)
    {
        MinStars = minStars;
        MaxStars = maxStars;
    }
}
