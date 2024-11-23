using System;
using System.Collections.Generic;
using System.Linq;

public class StarsManager
{
	public static StarType GetMyStarForCar(string CarDBKey)
	{
		return StarsManager.GetStarForTime(CarDBKey, PlayerProfileManager.Instance.ActiveProfile.GetBestTimeForCar(CarDBKey));
	}

	public static StarType GetStarForCar(CachedFriendRaceData friend, string CarDBKey)
	{
		if (!friend.FriendHasCarTime(CarDBKey))
		{
			return StarType.NONE;
		}
		return StarsManager.GetStarForTime(CarDBKey, friend.GetTimeForCar(CarDBKey));
	}

	public static bool DoesTimeBeatStar(string CarDBKey, float time, StarType star)
	{
		float starTime = StarsManager.GetStarTime(CarDBKey, star);
		return RaceTimesManager.IsPlayerRaceTimeWinner(time, ref starTime, RaceTimeType.RYF);
	}

	public static StarType GetStarFromPlayerInfo(PlayerInfo info)
	{
		RacePlayerInfoComponent component = info.GetComponent<RacePlayerInfoComponent>();
		RWFPlayerInfoComponent component2 = info.GetComponent<RWFPlayerInfoComponent>();
		if (!component2.HasSetTimeInCurrentCar)
		{
			return StarType.NONE;
		}
		return StarsManager.GetStarForTime(component.CarDBKey, component2.GetBestTimeInCurrentCar);
	}

	public static StarType GetStarForTime(string CarDBKey, float time)
	{
		if (time.Equals(0f))
		{
			return StarType.NONE;
		}
		StarType[] source = (StarType[])Enum.GetValues(typeof(StarType));
		return (from q in source
		where q != StarType.NONE && q != StarType.MAX
		select q).LastOrDefault((StarType q) => StarsManager.DoesTimeBeatStar(CarDBKey, time, q));
	}

	public static StarStats GetMyStarStats()
	{
		return StarsManager.GetStarStats(PlayerProfileManager.Instance.ActiveProfile.BestCarTimes);
	}

	public static StarStats GetStarStatsForAllTiers()
	{
		StarStats starStats = new StarStats();
		foreach (object current in Enum.GetValues(typeof(eCarTier)))
		{
			if ((int)current == 6)
			{
				break;
			}
			StarStats myStarStats = StarsManager.GetMyStarStats((eCarTier)((int)current));
			foreach (object current2 in Enum.GetValues(typeof(StarType)))
			{
				if ((int)current2 != 0 && (int)current2 != 4)
				{
					Dictionary<StarType, int> numStars;
					Dictionary<StarType, int> expr_8C = numStars = starStats.NumStars;
					StarType key;
					StarType expr_96 = key = (StarType)((int)current2);
					int num = numStars[key];
					expr_8C[expr_96] = num + myStarStats.NumStars[(StarType)((int)current2)];
				}
			}
		}
		return starStats;
	}

	public static int GetTotalNumberOfStarsAvailable()
	{
		int num = 0;
		foreach (object current in Enum.GetValues(typeof(eCarTier)))
		{
			if ((int)current == 6)
			{
				break;
			}
			num += StarsManager.GetAvailableStarsForTier((eCarTier)((int)current));
		}
		return num;
	}

	public static StarStats GetMyStarStats(eCarTier tier)
	{
		IEnumerable<KeyValuePair<string, float>> playerTimes = from q in PlayerProfileManager.Instance.ActiveProfile.BestCarTimes
		where CarDatabase.Instance.IsCarInTier(q.Key, tier)
		select q;
		return StarsManager.GetStarStats(playerTimes);
	}

	public static int GetAvailableStarsForTier(eCarTier tier)
	{
		int num = LumpManager.Instance.AllCarsTimeListScreen(tier, false).Count<string>();
		int num2 = 0;
		foreach (object current in Enum.GetValues(typeof(StarType)))
		{
			if ((int)current != 0 && (int)current != 4)
			{
				num2 += num;
			}
		}
		return num2;
	}

	public static StarStats GetStarStats(CachedFriendRaceData friend)
	{
		return StarsManager.GetStarStats(friend.CarTimes);
	}

	public static StarStats GetStarStats(CachedFriendRaceData friend, eCarTier tier)
	{
		return StarsManager.GetStarStats(from q in friend.CarTimes
		where CarDatabase.Instance.IsCarInTier(q.Key, tier)
		select q);
	}

	public static float GetStarTime(string carDBKey, StarType star)
	{
		return LumpManager.Instance.GetStarTime(carDBKey, star);
	}

	public static StarStats GetStarStats(IEnumerable<KeyValuePair<string, float>> playerTimes)
	{
		StarStats starStats = new StarStats();
		Dictionary<StarType, int> dictionary = new Dictionary<StarType, int>(starStats.NumStars);
		foreach (KeyValuePair<StarType, int> star in dictionary)
		{
			starStats.NumStars[star.Key] = playerTimes.Count((KeyValuePair<string, float> q) => StarsManager.DoesTimeBeatStar(q.Key, q.Value, star.Key));
		}
		return starStats;
	}
}
