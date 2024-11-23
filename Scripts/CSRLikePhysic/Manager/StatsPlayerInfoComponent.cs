using System.Collections.Generic;

public class StatsPlayerInfoComponent : PlayerInfoComponent
{
	public List<string> _top10OwnedCarsDBKey = new List<string>();

	public List<int> _top10OwnedCarsPPIndex = new List<int>();

	public int _onlineRacesWon;

	public int _onlineRacesLost;

	public int _totalCash;

	public int _totalGold;

	public int _level;

	public int _carsOwned;

	public int _totalPlayTime;

	public float _totalDistanceTravelled;

	public float _halfMile;

	public float _quarterMile;

	public int _totalGaragePP;

	public int _starCount;

	public int _bossesBeaten;

	public List<string> GetTop10CarsDBKey
	{
		get
		{
			return this._top10OwnedCarsDBKey;
		}
	}

	public List<int> GetTop10CarsPPIndex
	{
		get
		{
			return this._top10OwnedCarsPPIndex;
		}
	}

	public int TotalCash
	{
		get
		{
			return this._totalCash;
		}
	}

	public int TotalGold
	{
		get
		{
			return this._totalGold;
		}
	}

	public float Level
	{
		get
		{
			return (float)this._level;
		}
	}

	public int CarsOwned
	{
		get
		{
			return this._carsOwned;
		}
	}

	public int TotalOnlineRacesWon
	{
		get
		{
			return this._onlineRacesWon;
		}
	}

	public int TotalOnlineRacesLost
	{
		get
		{
			return this._onlineRacesLost;
		}
	}

	public int TotalPlayTime
	{
		get
		{
			return this._totalPlayTime;
		}
	}

	public float TotalDistanceTravelled
	{
		get
		{
			return this._totalDistanceTravelled;
		}
	}

	public float HalfMile
	{
		get
		{
			return this._halfMile;
		}
	}

	public float QuarterMile
	{
		get
		{
			return this._quarterMile;
		}
	}

	public int TotalGaragePP
	{
		get
		{
			return this._totalGaragePP;
		}
	}

	public int StarCount
	{
		get
		{
			return this._starCount;
		}
	}

	public int BossesBeaten
	{
		get
		{
			return this._bossesBeaten;
		}
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		jsonDict.Set("rw", this._onlineRacesWon);
		jsonDict.Set("rl", this._onlineRacesLost);
		jsonDict.Set("tc", this._totalCash);
		jsonDict.Set("tg", this._totalGold);
		jsonDict.Set("ll", this._level);
		jsonDict.Set("pt", this._totalPlayTime);
		jsonDict.Set("dt", this._totalDistanceTravelled);
		jsonDict.Set("co", this._carsOwned);
		jsonDict.Set("hm", this._halfMile);
		jsonDict.Set("qm", this._quarterMile);
		jsonDict.Set("oi", this._top10OwnedCarsDBKey);
		jsonDict.Set("op", this._top10OwnedCarsPPIndex);
		jsonDict.Set("gp", this._totalGaragePP);
		jsonDict.Set("sc", this._starCount);
		jsonDict.Set("bb", this._bossesBeaten);
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("rw", out this._onlineRacesWon);
		jsonDict.TryGetValue("rl", out this._onlineRacesLost);
		jsonDict.TryGetValue("tc", out this._totalCash);
		jsonDict.TryGetValue("tg", out this._totalGold);
		jsonDict.TryGetValue("ll", out this._level);
		jsonDict.TryGetValue("co", out this._carsOwned);
		jsonDict.TryGetValue("hm", out this._halfMile);
		jsonDict.TryGetValue("qm", out this._quarterMile);
		jsonDict.TryGetValue("pt", out this._totalPlayTime);
		jsonDict.TryGetValue("dt", out this._totalDistanceTravelled);
		jsonDict.TryGetValue("gp", out this._totalGaragePP);
		jsonDict.TryGetValue("sc", out this._starCount);
		jsonDict.TryGetValue("bb", out this._bossesBeaten);
		jsonDict.TryGetValue("oi", out this._top10OwnedCarsDBKey, new List<string>());
		jsonDict.TryGetValue("op", out this._top10OwnedCarsPPIndex, new List<int>());
	}
}
