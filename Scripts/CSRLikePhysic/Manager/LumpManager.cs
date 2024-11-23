using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LumpManager : IBundleOwner
{
	private const string starLumpPrefix = "StarLump_";

	private bool clearCacheOnNextLumpResponse = true;

	public Dictionary<int, CachedFriendRaceData> FriendLumps = new Dictionary<int, CachedFriendRaceData>();

	public Dictionary<StarType, CachedFriendRaceData> StarLumps = new Dictionary<StarType, CachedFriendRaceData>();

	private static LumpManager _instance = new LumpManager();

	private List<StarType> validStarTypes = new List<StarType>
	{
		StarType.BRONZE,
		StarType.SILVER,
		StarType.GOLD
	};

	public static LumpManager Instance
	{
		get
		{
			return LumpManager._instance;
		}
	}

	public bool IsReady
	{
		get;
		private set;
	}

	public void Initialise()
	{
		this.IsReady = false;
		this.clearCacheOnNextLumpResponse = true;
		foreach (StarType current in this.validStarTypes)
		{
			string starLumpBundleID = LumpManager.GetStarLumpBundleID(current);
			AssetProviderClient.Instance.RequestAsset(starLumpBundleID, new BundleLoadedDelegate(this.BundleLoaded), this);
		}
	}

	public void Shutdown()
	{
		this.IsReady = false;
		this.StarLumps.Clear();
		this.FriendLumps.Clear();
	}

	public void OnSetBestTime()
	{
		this.clearCacheOnNextLumpResponse = false;
	}

	public List<int> GetAllFriendsInTimeBracketForCar(float slowest, float fastest, string carDBKey)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, CachedFriendRaceData> current in this.FriendLumps)
		{
			if (current.Value.FriendHasCarTime(carDBKey))
			{
				float timeForCar = current.Value.GetTimeForCar(carDBKey);
				if ((!RaceTimesManager.IsPlayerRaceTimeWinner(slowest, ref timeForCar, RaceTimeType.RYF) || slowest == 0f) && RaceTimesManager.IsPlayerRaceTimeWinner(fastest, ref timeForCar, RaceTimeType.RYF))
				{
					list.Add(current.Key);
				}
			}
		}
		return list;
	}

	public int GetLeaderboardPositionForTimeInCar(string carDBKey, float time)
	{
		List<CachedFriendRaceData> list = new List<CachedFriendRaceData>(from q in this.FriendLumps.Values
		where q.FriendHasCarTime(carDBKey)
		select q);
		int num = 1;
		foreach (CachedFriendRaceData current in list)
		{
			if (time == 0f || current.GetTimeForCar(carDBKey) < time)
			{
				num++;
			}
		}
		return num;
	}

	public IEnumerable<CachedFriendRaceData> FriendsSortedByTimeInCar(string carDBKey)
	{
		List<CachedFriendRaceData> source = new List<CachedFriendRaceData>(from q in this.FriendLumps.Values
		where q.FriendHasCarTime(carDBKey)
		select q);
		return from q in source
		orderby q.GetTimeForCar(carDBKey)
		select q;
	}

	public IEnumerable<CachedFriendRaceData> EveryoneAndNextStarSortedByTimeInCar(string carDBKey)
	{
		List<CachedFriendRaceData> list = new List<CachedFriendRaceData>(from q in this.FriendLumps.Values
		where q.FriendHasCarTime(carDBKey)
		select q);
		StarType myStarForCar = StarsManager.GetMyStarForCar(carDBKey);
		if (myStarForCar != StarType.GOLD)
		{
			StarType key = myStarForCar + 1;
			list.Add(LumpManager.Instance.StarLumps[key]);
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		bool flag = activeProfile.CarsOwned.Any((CarGarageInstance q) => q.CarDBKey == carDBKey);
		bool flag2 = activeProfile.BestCarTimes.ContainsKey(carDBKey);
		if (flag && flag2)
		{
			list.Add(CachedFriendRaceData.CreateFromLocalProfile());
		}
		if (list.Count == 0)
		{
			list.Add(LumpManager.Instance.StarLumps[StarType.BRONZE]);
		}
		IEnumerable<CachedFriendRaceData> enumerable = from q in list
		orderby q.GetTimeForCar(carDBKey)
		select q;
		if (flag && !flag2)
		{
			enumerable = enumerable.Concat(new CachedFriendRaceData[]
			{
				CachedFriendRaceData.CreateFromLocalProfile()
			});
		}
		return enumerable;
	}

	public IEnumerable<CachedFriendRaceData> EveryoneAndAllStarsSortedByTimeInCar(string carDBKey)
	{
		List<CachedFriendRaceData> lumps = new List<CachedFriendRaceData>(from q in this.FriendLumps.Values
		where q.FriendHasCarTime(carDBKey)
		select q);
		this.validStarTypes.ForEach(delegate(StarType star)
		{
			lumps.Add(LumpManager.Instance.StarLumps[star]);
		});
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		bool flag = activeProfile.CarsOwned.Any((CarGarageInstance q) => q.CarDBKey == carDBKey);
		bool flag2 = activeProfile.BestCarTimes.ContainsKey(carDBKey);
		if (flag && flag2)
		{
			lumps.Add(CachedFriendRaceData.CreateFromLocalProfile());
		}
		IEnumerable<CachedFriendRaceData> enumerable = from q in lumps
		orderby q.GetTimeForCar(carDBKey)
		select q;
		if (flag && !flag2)
		{
			enumerable = enumerable.Concat(new CachedFriendRaceData[]
			{
				CachedFriendRaceData.CreateFromLocalProfile()
			});
		}
		return enumerable;
	}

	public void BestFriendByStarCountForTier(eCarTier tier, out CachedFriendRaceData friend, out StarStats stars)
	{
		if (this.FriendLumps.Count == 0)
		{
			friend = null;
			stars = new StarStats();
			return;
		}
		IEnumerable<KeyValuePair<CachedFriendRaceData, StarStats>> source = from q in this.FriendLumps.Values
		select new KeyValuePair<CachedFriendRaceData, StarStats>(q, StarsManager.GetStarStats(q, tier));
		KeyValuePair<CachedFriendRaceData, StarStats> keyValuePair = source.Aggregate((KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next) => this.OrderFriendsByTier(agg, next, tier));
		friend = keyValuePair.Key;
		stars = keyValuePair.Value;
	}

	public int GetFriendsCountForTier(eCarTier tier)
	{
		IEnumerable<CachedFriendRaceData> source = from q in this.FriendLumps.Values
		where this.GetFriendStarCountForTier(q, tier) >= 1
		select q;
		return source.Count<CachedFriendRaceData>();
	}

	public int GetFriendStarCountForTier(CachedFriendRaceData friend, eCarTier tier)
	{
		int starCount = 0;
		List<CarInfo> carsOfTier = CarDatabase.Instance.GetCarsOfTier(tier);
		carsOfTier.ForEach(delegate(CarInfo q)
		{
			starCount = (int)(starCount + StarsManager.GetStarForCar(friend, q.Key));
		});
		return starCount;
	}

	private KeyValuePair<CachedFriendRaceData, StarStats> OrderFriends(KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next)
	{
		int totalStars = next.Value.TotalStars;
		int totalStars2 = agg.Value.TotalStars;
		if (totalStars == totalStars2)
		{
			float bestTime = next.Key.GetBestTime();
			float bestTime2 = agg.Key.GetBestTime();
			return (bestTime >= bestTime2) ? agg : next;
		}
		return (totalStars <= totalStars2) ? agg : next;
	}

	private KeyValuePair<CachedFriendRaceData, StarStats> OrderFriendsByTier(KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next, eCarTier tier)
	{
		int totalStars = next.Value.TotalStars;
		int totalStars2 = agg.Value.TotalStars;
		if (totalStars == totalStars2)
		{
			float bestTimeForTier = next.Key.GetBestTimeForTier(tier);
			float bestTimeForTier2 = agg.Key.GetBestTimeForTier(tier);
			return (bestTimeForTier >= bestTimeForTier2) ? agg : next;
		}
		return (totalStars <= totalStars2) ? agg : next;
	}

	public void BestOverAllFriendByStarCount(out CachedFriendRaceData friend, out StarStats stars)
	{
		if (this.FriendLumps.Count == 0)
		{
			friend = null;
			stars = new StarStats();
			return;
		}
		IEnumerable<KeyValuePair<CachedFriendRaceData, StarStats>> source = from q in this.FriendLumps.Values
		select new KeyValuePair<CachedFriendRaceData, StarStats>(q, StarsManager.GetStarStats(q));
		KeyValuePair<CachedFriendRaceData, StarStats> keyValuePair = source.Aggregate((KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next) => this.OrderFriends(agg, next));
		friend = keyValuePair.Key;
		stars = keyValuePair.Value;
	}

	public int CompareFriends(KeyValuePair<CachedFriendRaceData, StarStats> current, KeyValuePair<CachedFriendRaceData, StarStats> next)
	{
		int num = -current.Value.TotalStars.CompareTo(next.Value.TotalStars);
		return (num == 0) ? current.Key.GetBestTime().CompareTo(next.Key.GetBestTime()) : num;
	}

	public IEnumerable<KeyValuePair<CachedFriendRaceData, StarStats>> GetFriendsStarCount()
	{
		List<KeyValuePair<CachedFriendRaceData, StarStats>> list = (from q in this.FriendLumps.Values
		select new KeyValuePair<CachedFriendRaceData, StarStats>(q, StarsManager.GetStarStats(q))).ToList<KeyValuePair<CachedFriendRaceData, StarStats>>();
		CachedFriendRaceData cachedFriendRaceData = CachedFriendRaceData.CreateFromLocalProfile();
		list.Add(new KeyValuePair<CachedFriendRaceData, StarStats>(cachedFriendRaceData, StarsManager.GetStarStats(cachedFriendRaceData)));
		list.Sort((KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next) => this.CompareFriends(agg, next));
		return list;
	}

	public int CompareFriendsForTier(KeyValuePair<CachedFriendRaceData, StarStats> current, KeyValuePair<CachedFriendRaceData, StarStats> next, eCarTier tier)
	{
		int num = -current.Value.TotalStars.CompareTo(next.Value.TotalStars);
		return (num == 0) ? current.Key.GetBestTimeForTier(tier).CompareTo(next.Key.GetBestTimeForTier(tier)) : num;
	}

	public IEnumerable<KeyValuePair<CachedFriendRaceData, StarStats>> GetFriendsStarCountForTier(eCarTier tier)
	{
		List<KeyValuePair<CachedFriendRaceData, StarStats>> list = (from q in this.FriendLumps.Values
		select new KeyValuePair<CachedFriendRaceData, StarStats>(q, StarsManager.GetStarStats(q, tier))).ToList<KeyValuePair<CachedFriendRaceData, StarStats>>();
		CachedFriendRaceData cachedFriendRaceData = CachedFriendRaceData.CreateFromLocalProfile();
		list.Add(new KeyValuePair<CachedFriendRaceData, StarStats>(cachedFriendRaceData, StarsManager.GetStarStats(cachedFriendRaceData, tier)));
		list.Sort((KeyValuePair<CachedFriendRaceData, StarStats> agg, KeyValuePair<CachedFriendRaceData, StarStats> next) => this.CompareFriendsForTier(agg, next, tier));
		return list;
	}

	public IEnumerable<string> AllCarsTimeListScreen(eCarTier tier, bool sort = true)
	{
		IEnumerable<string> enumerable = from q in PlayerProfileManager.Instance.ActiveProfile.CarsOwned
		select q.CarDBKey;
		enumerable = this.ReduceAndSortCarList(enumerable, tier, false, sort);
		IEnumerable<string> enumerable2 = this.FriendLumps.SelectMany((KeyValuePair<int, CachedFriendRaceData> friend) => friend.Value.GetCarNames());
		enumerable2 = enumerable2.Except(enumerable);
		enumerable2 = this.ReduceAndSortCarList(enumerable2, tier, false, sort);
		IEnumerable<string> enumerable3 = from q in CarDatabase.Instance.GetAllCars()
		select q.Key;
		enumerable3 = enumerable3.Except(enumerable2).Except(enumerable);
		enumerable3 = this.ReduceAndSortCarList(enumerable3, tier, true, sort);
		return enumerable.Concat(enumerable2).Concat(enumerable3);
	}

	private static bool ValidRaceWithFriendsCar(string dbKey, eCarTier tier)
	{
		List<string> carsToExcludeFromRYF = GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromRYF;
		if (carsToExcludeFromRYF != null && carsToExcludeFromRYF.Contains(dbKey))
		{
			return false;
		}
		CarInfo carOrNull = CarDatabase.Instance.GetCarOrNull(dbKey);
		return carOrNull != null && carOrNull.BaseCarTier == tier && !CarDataDefaults.IsBossCar(dbKey);
	}

	private IEnumerable<string> ReduceCarList(IEnumerable<string> carList, eCarTier tier, bool noSeasonCars)
	{
		carList = from q in carList
		where LumpManager.ValidRaceWithFriendsCar(q, tier)
		select q;
		if (noSeasonCars)
		{
			carList = this.RemoveSeasonCars(carList);
		}
		return carList.Distinct<string>();
	}

	private IEnumerable<string> ReduceAndSortCarList(IEnumerable<string> carList, eCarTier tier, bool noSeasonCars, bool sort = true)
	{
		carList = this.ReduceCarList(carList, tier, noSeasonCars);
		if (sort)
		{
			carList = from q in carList
			orderby CarDatabase.Instance.GetCarNiceName(q)
			select q;
		}
		return carList;
	}

	private IEnumerable<string> RemoveBossCars(IEnumerable<string> carList)
	{
		return from q in carList
		where !CarDataDefaults.IsBossCar(q)
		select q;
	}

	private IEnumerable<string> RemoveSeasonCars(IEnumerable<string> carList)
	{
		return from q in carList
		where SeasonUtilities.CanCarBeUnlockedBySeason(CarDatabase.Instance.GetCar(q))
		select q;
	}

	private IEnumerable<string> RemoveCarsNotInDatabase(IEnumerable<string> carList)
	{
		return from q in carList
		where CarDatabase.Instance.PeekGetCar(q)
		select q;
	}

	public IEnumerable<string> AllSupportedCars()
	{
		IEnumerable<string> enumerable = from q in CarDatabase.Instance.GetAllCars()
		select q.Key;
		enumerable = this.RemoveBossCars(enumerable);
		enumerable = this.RemoveSeasonCars(enumerable);
		enumerable = this.RemoveCarsNotInDatabase(enumerable);
		return from q in enumerable
		orderby CarDatabase.Instance.GetCarNiceName(q)
		select q;
	}

	public IEnumerable<string> AllSupportedCarsInTier(eCarTier zTier)
	{
		IEnumerable<CarInfo> allCars = CarDatabase.Instance.GetAllCars((CarInfo q) => q.BaseCarTier == zTier);
		IEnumerable<string> enumerable = this.RemoveBossCars(from q in allCars
		select q.Key);
		enumerable = this.RemoveSeasonCars(enumerable);
		enumerable = this.RemoveCarsNotInDatabase(enumerable);
		return from q in enumerable
		orderby CarDatabase.Instance.GetCarNiceName(q)
		select q;
	}

	private void BundleLoaded(string assetID, AssetBundle assetBundle, IBundleOwner owner)
	{
		JsonDict jsonDict = new JsonDict();
		jsonDict.Read((assetBundle.mainAsset as TextAsset).text);
		CachedFriendRaceData value = CachedFriendRaceDataMapper.FromJson(jsonDict);
		StarType starTypeFromAssetID = LumpManager.GetStarTypeFromAssetID(assetID);
		this.StarLumps[starTypeFromAssetID] = value;
		AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
		this.IsReady = this.validStarTypes.All((StarType star) => this.StarLumps.ContainsKey(star));
	}

	public float GetStarTime(string carDBKey, StarType star)
	{
		float result = 0f;
		CachedFriendRaceData cachedFriendRaceData;
		if (this.StarLumps.TryGetValue(star, out cachedFriendRaceData))
		{
			cachedFriendRaceData.CarTimes.TryGetValue(carDBKey, out result);
		}
		return result;
	}

	public static string GetStarLumpBundleID(StarType star)
	{
		return "StarLump_" + star.ToString();
	}

	public static StarType GetStarTypeFromAssetID(string assetID)
	{
		string value = assetID.Replace("StarLump_", string.Empty);
		if (Enum.IsDefined(typeof(StarType), value))
		{
			return (StarType)((int)Enum.Parse(typeof(StarType), value));
		}
		return StarType.NONE;
	}

	public void HandleLumpResponse(List<ServerCachedFriendRaceData> serverLump)
	{
		this.FriendLumps.Clear();
		List<CachedFriendRaceData> list = new List<CachedFriendRaceData>(from q in serverLump
		select CachedFriendRaceData.PopulateFromServerCachedFriendRaceData(q));
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int userID = UserManager.Instance.currentAccount.UserID;
		list.FindAll((CachedFriendRaceData sl) => sl.UserID != userID).ForEach(delegate(CachedFriendRaceData sl)
		{
			this.FriendLumps.Add(sl.UserID, sl);
		});
		CachedFriendRaceData cachedFriendRaceData = list.Find((CachedFriendRaceData sl) => sl.UserID == userID);
		if (cachedFriendRaceData == null)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.FriendRacesPlayed > 0)
			{
			}
			if (serverLump.Count > 0)
			{
			}
			return;
		}
		activeProfile.UpdateFriends(this.FriendLumps, cachedFriendRaceData);
		if (this.CanOverwriteProfileTimes())
		{
			if (this.clearCacheOnNextLumpResponse && cachedFriendRaceData.CarTimes.Count > 0)
			{
				activeProfile.BestCarTimes = new Dictionary<string, float>();
				this.clearCacheOnNextLumpResponse = false;
			}
			foreach (KeyValuePair<string, float> current in cachedFriendRaceData.CarTimes)
			{
				float bestTimeForCar = activeProfile.GetBestTimeForCar(current.Key);
				if (bestTimeForCar.Equals(0f) || current.Value < bestTimeForCar)
				{
					activeProfile.SetBestTimeForCar(current.Key, current.Value);
				}
			}
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public bool IsStarLump(CachedFriendRaceData lump)
	{
		return this.StarLumps.ContainsValue(lump);
	}

	public PlayerInfo PlayerInfoFromCachedFriendRaceData(CachedFriendRaceData lump)
	{
		if (lump.UserID == UserManager.Instance.currentAccount.UserID)
		{
			return new PreviousBestPlayerInfo();
		}
		if (this.IsStarLump(lump))
		{
			StarType type = StarType.NONE;
			foreach (KeyValuePair<StarType, CachedFriendRaceData> current in this.StarLumps)
			{
				if (current.Value == lump)
				{
					type = current.Key;
				}
			}
			return new StarTimePlayerInfo(type);
		}
		return new FriendPlayerInfo(lump);
	}

	private bool CanOverwriteProfileTimes()
	{
		return PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon > 0 && PlayerProfileManager.Instance.ActiveProfile.FriendRacesPlayed > 1;
	}
}
