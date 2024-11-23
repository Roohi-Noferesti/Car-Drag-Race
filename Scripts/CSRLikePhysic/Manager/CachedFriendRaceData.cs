using System;
using System.Collections.Generic;
using System.Linq;

public class CachedFriendRaceData
{
	public const float TIME_NOT_SET = 0f;

	public Dictionary<string, float> CarTimes
	{
		get;
		set;
	}

	public List<string> ServiceID
	{
		get;
		set;
	}

	public int UserID
	{
		get;
		set;
	}

	public static CachedFriendRaceData CreateFromLocalProfile()
	{
		return new CachedFriendRaceData
		{
			UserID = UserManager.Instance.currentAccount.UserID,
			CarTimes = PlayerProfileManager.Instance.ActiveProfile.BestCarTimes
		};
	}

	public static CachedFriendRaceData PopulateFromServerCachedFriendRaceData(ServerCachedFriendRaceData serverData)
	{
		return new CachedFriendRaceData
		{
			UserID = serverData.UID,
			ServiceID = serverData.SID,
			CarTimes = serverData.RaceTimes
		};
	}

	public IEnumerable<string> GetCarNames()
	{
		return from car in this.CarTimes
		select car.Key;
	}

	public bool FriendHasCarTime(string carDbKey)
	{
		return this.CarTimes.ContainsKey(carDbKey);
	}

	public float GetTimeForCar(string carDBKey)
	{
		if (!this.CarTimes.ContainsKey(carDBKey))
		{
			return 0f;
		}
		return this.CarTimes[carDBKey];
	}

	public float GetBestTime()
	{
		if (this.CarTimes.Count == 0)
		{
			return 99f;
		}
		Dictionary<string, float>.KeyCollection keys = this.CarTimes.Keys;
		return this.FindBestTime(keys);
	}

	public float GetBestTimeForTier(eCarTier tier)
	{
		List<CarInfo> carsOfTier = CarDatabase.Instance.GetCarsOfTier(tier);
		IEnumerable<string> carKeys = from q in carsOfTier
		select q.Key;
		return this.FindBestTime(carKeys);
	}

	private float FindBestTime(IEnumerable<string> carKeys)
	{
		float num = 99f;
		foreach (string current in carKeys)
		{
			if (this.CarTimes.ContainsKey(current) && this.CarTimes[current] < num && this.CarTimes[current] != 0f)
			{
				num = this.CarTimes[current];
			}
		}
		return num;
	}

	public bool BelongsToService(string serviceId)
	{
		return this.ServiceID.Any((string s) => s.StartsWith(serviceId));
	}

	public string GetIdForService(string serviceId)
	{
		return this.ServiceID.First((string s) => s.StartsWith(serviceId));
	}

	public int CompareTime(float a, float b)
	{
		if (a == b)
		{
			return 0;
		}
		if (a == 0f)
		{
			return 1;
		}
		if (b == 0f)
		{
			return -1;
		}
		return (a >= b) ? 1 : -1;
	}
}
