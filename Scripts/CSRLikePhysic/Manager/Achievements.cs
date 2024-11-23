using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements
{
	private enum Entries
	{
		FIRST_ACHIEVEMENT,
		GT_ACH_HALF_TIER_1,
		GT_ACH_ALL_TIER_1,
		GT_ACH_HALF_TIER_2,
		GT_ACH_ALL_TIER_2,
		GT_ACH_HALF_TIER_3,
		GT_ACH_ALL_TIER_3,
		GT_ACH_HALF_TIER_4,
		GT_ACH_ALL_TIER_4,
		GT_ACH_BEAT_TIER_1_CREW_LEADER,
		GT_ACH_BEAT_TIER_2_CREW_LEADER,
		GT_ACH_BEAT_TIER_3_CREW_LEADER,
		GT_ACH_BEAT_TIER_4_CREW_LEADER,
		GT_ACH_BEAT_ALL_CREW_LEADERS,
		GT_ACH_WHEELSPIN_500,
		GT_ACH_10_SEC_QUARTER,
		GT_ACH_15_SEC_HALF,
		GT_ACH_REACH_180,
		GT_ACH_WIN_BY_A_SECOND,
		GT_ACH_WIN_BY_POINT_01,
		GT_ACH_LOSE_BY_POINT_01,
		GT_ACH_PERFECT_SHIFT,
		GT_ACH_FULLY_UPGRADE_A_CAR,
		GT_ACH_BUY_A_CAR_FROM_4_TIERS,
		GT_ACH_OWN_18_CARS,
		GT_ACH_CHANGE_COLOUR,
		GT_ACH_ADJUST_TUNING,
		GT_ACH_BIG_SPENDER,
		GT_ACH_USE_MECHANIC,
		GT_ACH_USE_ROLLING_ROAD,
		GT_ACH_WIN_IN_FIRST,
		GT_ACH_SPEEDUP_DELIVERY,
		GT_ACH_SPEND_250_GOLD,
		GT_ACH_BUY_BMW,
		GT_ACH_BUY_CHEVROLET,
		GT_ACH_BUY_NISSAN,
		GT_ACH_BUY_AUDI,
		GT_ACH_BUY_MINI,
		GT_ACH_BUY_FORD,
		GT_ACH_RATE,
		GT_ACH_HALF_TIER_5,
		GT_ACH_ALL_TIER_5
	}

	private static List<Achievement> achievements;

	private static Dictionary<Entries, Achievement> achievementsMap;

	public static int Count
	{
		get
		{
			return achievements.Count;
		}
	}

	public static List<Achievement> All
	{
		get
		{
			return achievements;
		}
	}
	
	public static Achievement FirstAchievement
	{
		get
		{
			return achievementsMap[Entries.FIRST_ACHIEVEMENT];
		}
	}

	public static Achievement HalfTier1
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_HALF_TIER_1];
		}
	}

	public static Achievement AllTier1
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ALL_TIER_1];
		}
	}

	public static Achievement HalfTier2
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_HALF_TIER_2];
		}
	}

	public static Achievement AllTier2
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ALL_TIER_2];
		}
	}

	public static Achievement HalfTier3
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_HALF_TIER_3];
		}
	}

	public static Achievement AllTier3
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ALL_TIER_3];
		}
	}

	public static Achievement HalfTier4
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_HALF_TIER_4];
		}
	}

	public static Achievement AllTier4
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ALL_TIER_4];
		}
	}

	public static Achievement HalfTier5
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_HALF_TIER_5];
		}
	}

	public static Achievement AllTier5
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ALL_TIER_5];
		}
	}

	public static Achievement BeatTier1CrewLeader
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BEAT_TIER_1_CREW_LEADER];
		}
	}

	public static Achievement BeatTier2CrewLeader
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BEAT_TIER_2_CREW_LEADER];
		}
	}

	public static Achievement BeatTier3CrewLeader
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BEAT_TIER_3_CREW_LEADER];
		}
	}

	public static Achievement BeatTier4CrewLeader
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BEAT_TIER_4_CREW_LEADER];
		}
	}

	public static Achievement BeatAllCrewLeaders
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BEAT_ALL_CREW_LEADERS];
		}
	}

	public static Achievement Wheelspin500
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_WHEELSPIN_500];
		}
	}

	public static Achievement TenSecQuarter
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_10_SEC_QUARTER];
		}
	}

	public static Achievement FifteenSecHalf
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_15_SEC_HALF];
		}
	}

	public static Achievement Reach180
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_REACH_180];
		}
	}

	public static Achievement WinByASecond
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_WIN_BY_A_SECOND];
		}
	}

	public static Achievement WinByPoint01
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_WIN_BY_POINT_01];
		}
	}

	public static Achievement LoseByPoint01
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_LOSE_BY_POINT_01];
		}
	}

	public static Achievement PerfectShift
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_PERFECT_SHIFT];
		}
	}

	public static Achievement FullyUpgradeACar
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_FULLY_UPGRADE_A_CAR];
		}
	}

	public static Achievement BuyACarFrom4Tiers
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_A_CAR_FROM_4_TIERS];
		}
	}

	public static Achievement Own18Cars
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_OWN_18_CARS];
		}
	}

	public static Achievement Own40Cars
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_RATE];
		}
	}

	public static Achievement ChangeColour
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_CHANGE_COLOUR];
		}
	}

	public static Achievement AdjustTuning
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_ADJUST_TUNING];
		}
	}

	public static Achievement BigSpender
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BIG_SPENDER];
		}
	}

	public static Achievement GetAchievement(int x)
	{
		return achievementsMap[(Entries)x];
	}

	public static Achievement UseMechanic
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_USE_MECHANIC];
		}
	}

	public static Achievement UseRollingRoad
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_USE_ROLLING_ROAD];
		}
	}

	public static Achievement WinInFirst
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_WIN_IN_FIRST];
		}
	}

	public static Achievement SpeedupDelivery
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_SPEEDUP_DELIVERY];
		}
	}

	public static Achievement Spend250Gold
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_SPEND_250_GOLD];
		}
	}

	public static Achievement BuyBMW
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_BMW];
		}
	}

	public static Achievement BuyChevrolet
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_CHEVROLET];
		}
	}

	public static Achievement BuyNissan
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_NISSAN];
		}
	}

	public static Achievement BuyAudi
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_AUDI];
		}
	}

	public static Achievement BuyMini
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_MINI];
		}
	}

	public static Achievement BuyFord
	{
		get
		{
			return achievementsMap[Entries.GT_ACH_BUY_FORD];
		}
	}

	static Achievements()
	{
		achievements = new List<Achievement>();
		achievementsMap = new Dictionary<Entries, Achievement>();
		IEnumerator enumerator = Enum.GetValues(typeof(Entries)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entries entries = (Entries)((int)enumerator.Current);
				int inIdx = (int)entries;
				string gcCategoryIDname = GameCenterCategoryIDs.CategoryIDName(entries.ToString());
				Achievement achievement = new Achievement(inIdx, gcCategoryIDname);
				//Debug.LogError("inIdx: " + inIdx.ToString() + " / gcCategoryIDname:" + gcCategoryIDname);
				achievements.Add(achievement);
				achievementsMap[entries] = achievement;
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
	}

	public static Achievement GetByIdx(int idx)
	{
		return achievements[idx];
	}

	public static Achievement GetByCategoryIDName(string gcCategoryIDName)
	{
		foreach (Achievement current in achievements)
		{
			string categoryIDName = current.CategoryIDName;
            if (GameCenterCategoryIDs.Matches(categoryIDName, gcCategoryIDName))
            {
                return current;
            }
		}
		return null;
	}
}
