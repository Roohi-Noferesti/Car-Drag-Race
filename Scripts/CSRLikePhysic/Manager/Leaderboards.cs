using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Leaderboards
{
    public enum Entries
	{
        FIRST_LEADERBOARD,
        CSR_LDR_OVERALL_TIME,
        CSR_LDR_OVERALL_EARNINGS,
        CSR_LDR_CLASS_D_QUARTER,
        CSR_LDR_CLASS_D_HALF,
        CSR_LDR_CLASS_C_QUARTER,
        CSR_LDR_CLASS_C_HALF,
        CSR_LDR_CLASS_B_QUARTER,
        CSR_LDR_CLASS_B_HALF,
        CSR_LDR_CLASS_A_QUARTER,
        CSR_LDR_CLASS_A_HALF,
        CSR_LDR_CLASS_S_QUARTER,
        CSR_LDR_CLASS_S_HALF,
        CSR_MULTI_WORLD_RANK,
        Star_A,
        Star_B,
        Star_C,
        Star_D,
        Star_E,
	}

	private static List<Leaderboard> leaderboards;

	private static Dictionary<Entries, Leaderboard> leaderboardsMap;

	private static Dictionary<eCarTier, Leaderboard> quarterMiles;

	private static Dictionary<eCarTier, Leaderboard> halfMiles;

	private static List<Leaderboard> standardRaceLeaderboards;

	private static List<Leaderboard> higherIsBetterLeaderboards;

    private static List<StarLeaderboard> StarLeaderboards; 

	public static int Count
	{
		get
		{
			return leaderboards.Count;
		}
	}

	public static List<Leaderboard> All
	{
		get
		{
			return leaderboards;
		}
	}

	public static Leaderboard OverallEarnings
	{
		get
		{
			return Get(Entries.CSR_LDR_OVERALL_EARNINGS);
		}
	}

	public static Leaderboard OverallTime
	{
		get
		{
			return Get(Entries.CSR_LDR_OVERALL_TIME);
		}
	}

    static Leaderboards()
    {
        leaderboards = new List<Leaderboard>();
        leaderboardsMap = new Dictionary<Entries, Leaderboard>();
        IEnumerator enumerator = Enum.GetValues(typeof (Entries)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Entries entries = (Entries) ((int) enumerator.Current);
                int inIdx = (int) entries;
                string gcCategoryIDname = GameCenterCategoryIDs.CategoryIDName(entries.ToString());
                Leaderboard leaderboard = new Leaderboard(inIdx, gcCategoryIDname);
                leaderboards.Add(leaderboard);
                leaderboardsMap[entries] = leaderboard;
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        quarterMiles = new Dictionary<eCarTier, Leaderboard>
        {
            {
                eCarTier.TIER_1,
                Get(Entries.CSR_LDR_CLASS_D_QUARTER)
            },
            {
                eCarTier.TIER_2,
                Get(Entries.CSR_LDR_CLASS_C_QUARTER)
            },
            {
                eCarTier.TIER_3,
                Get(Entries.CSR_LDR_CLASS_B_QUARTER)
            },
            {
                eCarTier.TIER_4,
                Get(Entries.CSR_LDR_CLASS_A_QUARTER)
            },
            {
                eCarTier.TIER_5,
                Get(Entries.CSR_LDR_CLASS_S_QUARTER)
            }
        };
        halfMiles = new Dictionary<eCarTier, Leaderboard>
        {
            {
                eCarTier.TIER_1,
                Get(Entries.CSR_LDR_CLASS_D_HALF)
            },
            {
                eCarTier.TIER_2,
                Get(Entries.CSR_LDR_CLASS_C_HALF)
            },
            {
                eCarTier.TIER_3,
                Get(Entries.CSR_LDR_CLASS_B_HALF)
            },
            {
                eCarTier.TIER_4,
                Get(Entries.CSR_LDR_CLASS_A_HALF)
            },
            {
                eCarTier.TIER_5,
                Get(Entries.CSR_LDR_CLASS_S_HALF)
            }
        };
        standardRaceLeaderboards = new List<Leaderboard>
        {
            Get(Entries.CSR_LDR_CLASS_D_QUARTER),
            Get(Entries.CSR_LDR_CLASS_D_HALF),
            Get(Entries.CSR_LDR_CLASS_C_QUARTER),
            Get(Entries.CSR_LDR_CLASS_C_HALF),
            Get(Entries.CSR_LDR_CLASS_B_QUARTER),
            Get(Entries.CSR_LDR_CLASS_B_HALF),
            Get(Entries.CSR_LDR_CLASS_A_QUARTER),
            Get(Entries.CSR_LDR_CLASS_A_HALF),
            Get(Entries.CSR_LDR_CLASS_S_QUARTER),
            Get(Entries.CSR_LDR_CLASS_S_HALF)
        };
        higherIsBetterLeaderboards = new List<Leaderboard>
        {
            Get(Entries.CSR_LDR_OVERALL_EARNINGS),
            Get(Entries.CSR_MULTI_WORLD_RANK)
        };

        StarLeaderboards = new List<StarLeaderboard>
        {
            new StarLeaderboard((int) Entries.Star_A, "A", 400, 499),
            new StarLeaderboard((int) Entries.Star_B, "B", 300, 399),
            new StarLeaderboard((int) Entries.Star_C, "C", 200, 299),
            new StarLeaderboard((int) Entries.Star_D, "D", 100, 199),
            new StarLeaderboard((int) Entries.Star_E, "E", 0, 99)
        };
    }

    private static Leaderboard Get(Entries val)
	{
		if (leaderboardsMap.ContainsKey(val))
		{
			return leaderboardsMap[val];
		}
		return null;
	}

	public static Leaderboard GetByIDName(string gcCategoryIDName)
	{
		foreach (Leaderboard current in leaderboards)
		{
			string categoryIDName = current.CategoryIDName;
            if (GameCenterCategoryIDs.Matches(categoryIDName, gcCategoryIDName))
            {
                return current;
            }
		}
		return null;
	}

	public static Leaderboard GetByIdx(int idx)
	{
		return leaderboards[idx];
	}

	public static Leaderboard GetLeaderboardForRace(eCarTier tier, bool isHalfMile)
	{
		return (!isHalfMile) ? quarterMiles[tier] : halfMiles[tier];
	}

	public static bool IsStandardRace(Leaderboard leaderboard)
	{
		return standardRaceLeaderboards.Contains(leaderboard);
	}

	public static bool HigherIsBetter(Leaderboard leaderboard)
	{
		return higherIsBetterLeaderboards.Contains(leaderboard);
	}

    public static StarLeaderboard GetByStars(int playerStar)
    {
        var leaderboard = StarLeaderboards.FirstOrDefault(l => playerStar >= l.MinStars && playerStar <= l.MaxStars);

        if (leaderboard == null)
        {
            leaderboard =  StarLeaderboards.Last();
        }
        return leaderboard;
    }
}
