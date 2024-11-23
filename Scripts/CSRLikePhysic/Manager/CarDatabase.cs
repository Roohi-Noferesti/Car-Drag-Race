using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I2.Loc;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class CarDatabase : MonoBehaviour, IBundleOwner
{
	public static CarDatabase Instance;

	public static CarDatabaseUpdatedDelegate CarDatabaseUpdatedEvent;

	private CarInfo DefaultCarInfo;

	private List<CarInfo> CarsInDatabase = new List<CarInfo>();

#if UNITY_EDITOR
    private static CarInfo[] _cars;
#endif

    public bool isReady
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		this.isReady = false;
	}

	public void OnAssetDatabaseLoaded()
	{
		this._Reset();
	}

	public void ShutDown()
	{
		this.isReady = false;
	}

	private void _Reset()
	{
        this.isReady = false;
#if UNITY_EDITOR
        StartCoroutine(LoadCarsAsync());
#else
        AssetProviderClient.Instance.RequestAsset("CarMetadata".ToLower(), new BundleLoadedDelegate(this.CarInfoBundleReady), this);
#endif

    }


    private void CarInfoBundleReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
    {
        UnityEngine.Object[] cars = zAssetBundle.LoadAllAssets<ScriptableObject>();
        this.SetupCarDatabase(cars);
        AssetProviderClient.Instance.ReleaseAsset("CarMetadata".ToLower(), zOwner);
    }

#if UNITY_EDITOR

    private IEnumerator LoadCarsAsync()
    {
        //var cars = new CarInfo[CarsList.Cars.Length];
        //for (int i = 0; i < CarsList.Cars.Length; i++)
        //{
        //    cars[i] = ResourceManager.GetCarAsset<CarInfo>(CarsList.Cars[i], ServerItemBase.AssetType.spec);
        //    if (cars[i] == null)
        //    {
        //        Debug.LogWarning(CarsList.Cars[i] +"  not found , you may change resources folder name");
        //    }
        //    yield return new WaitForEndOfFrame();
        //}
        if (_cars == null || _cars.Length <= 0)
        {
            List<CarInfo> carinfos = new List<CarInfo>();
            string[] allAssetFiles = Directory.GetFiles(Application.dataPath + "/CarInfo", "*.asset", SearchOption.AllDirectories);
            foreach (string assetFilePath in allAssetFiles)
            {
                string assetPath = "Assets" + assetFilePath.Replace(Application.dataPath, "").Replace('\\', '/');
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (asset is CarInfo)
                {
                    carinfos.Add((CarInfo)asset);
                }

                yield return new WaitForEndOfFrame();
            }
            _cars = carinfos.ToArray();
        }

        this.SetupCarDatabase(_cars);
    }

    public static CarInfo[] GetCars()
    {
        if (_cars != null && _cars.Length > 0)
        {
            return _cars;
        }

        List<CarInfo> carinfos = new List<CarInfo>();
        string[] allAssetFiles = Directory.GetFiles(Application.dataPath + "/CarInfo", "*.asset", SearchOption.AllDirectories);
        foreach (string assetFilePath in allAssetFiles)
        {
            string assetPath = "Assets" + assetFilePath.Replace(Application.dataPath, "").Replace('\\', '/');
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (asset is CarInfo)
            {
                carinfos.Add((CarInfo)asset);
            }
            // .. do whatever you like
        }

        _cars = carinfos.ToArray();
        return _cars;
    }
#endif

    public void WhenReady(Action task)
	{
		if (this.isReady)
		{
			task();
		}
		else
		{
			base.StartCoroutine(this.DoWhenReady(task));
		}
	}

	public IEnumerator DoWhenReady(Action task)
	{
	    while (!isReady)
	    {
	        yield return new WaitForEndOfFrame();
	    }
	    task();
	}

    public static CarDatabase SetupCarDatabaseForEditor(params CarInfo[] carinfo)
    {
        Instance = new GameObject("Cardatabase_Editor").AddComponent<CarDatabase>();
        Instance.SetupCarDatabase(carinfo);
        return Instance;
    }


    private void SetupCarDatabase(UnityEngine.Object[] cars)
    {
        this.CarsInDatabase = (from q in cars
                               select q as CarInfo).ToList<CarInfo>();
        this.CarsInDatabase.RemoveAll((CarInfo q) => q == null);
        AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
        Dictionary<string, AssetDatabaseAsset> assetList = data.GetAssetsOfTypeDict(GTAssetTypes.vehicle);
        IEnumerable<CarInfo> first = from car in this.CarsInDatabase
                                     where car.PredefinedUpgradeSets == null
                                     select car;
        IEnumerable<CarInfo> first2 = from car in this.CarsInDatabase
                                      where string.IsNullOrEmpty(car.ModelPrefabString)
                                      select car;
        IEnumerable<CarInfo> second = from car in this.CarsInDatabase
                                      where !assetList.ContainsKey(car.ModelPrefabString)
                                      select car;
        IEnumerable<CarInfo> second2 = first.Concat(first2.Concat(second));
        this.CarsInDatabase = this.CarsInDatabase.Except(second2).ToList<CarInfo>();
        this.CarsInDatabase.ForEach(delegate (CarInfo info)
        {
            info.Process();
        });
        this.CarsInDatabase.RemoveAll((CarInfo q) => CarDataDefaults.CarIsExcludedFromGame(q));
        this.isReady = true;
        this.DefaultCarInfo = CarDatabase.Instance.GetCar("car_hyundai_Genesis_Coupe");
        if (CarDatabase.CarDatabaseUpdatedEvent != null)
        {
            CarDatabase.CarDatabaseUpdatedEvent();
        }
    }

    private void SetupCarDatabase(CarInfo[] cars)
	{
	    this.CarsInDatabase = (from q in cars where q != null && q.Available && !string.IsNullOrEmpty(q.Key)
		select q as CarInfo).ToList<CarInfo>();
        //foreach (var carInfo in cars)
        //{
        //    Debug.Log(carInfo.Key + "   ");
        //}
        //AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
        //Dictionary<string, AssetDatabaseAsset> assetList = data.GetAssetsOfTypeDict(CSRAssetTypes.vehicle);
        //IEnumerable<CarInfo> first = from car in this.CarsInDatabase
        //where car.PredefinedUpgradeSets == null
        //select car;
        //IEnumerable<CarInfo> first2 = from car in this.CarsInDatabase
        //where string.IsNullOrEmpty(car.ModelPrefabString)
        //select car;
        //IEnumerable<CarInfo> second = from car in this.CarsInDatabase
        //where !assetList.ContainsKey(car.ModelPrefabString)
        //select car;
        //IEnumerable<CarInfo> second2 = first.Concat(first2.Concat(second));
        //this.CarsInDatabase = this.CarsInDatabase.Except(second2).ToList<CarInfo>();
		this.CarsInDatabase.ForEach(delegate(CarInfo info)
		{
			info.Process();
		});
        //this.CarsInDatabase.RemoveAll((CarInfo q) => CarDataDefaults.CarIsExcludedFromGame(q));
		this.isReady = true;
        this.DefaultCarInfo = Instance.GetCar("car_hyundai_Genesis_Coupe");
		if (CarDatabaseUpdatedEvent != null)
		{
			CarDatabaseUpdatedEvent();
		}
	}

	public List<CarInfo> GetAllCars(Predicate<CarInfo> match)
	{
		return this.CarsInDatabase.FindAll(match);
	}

	public List<CarInfo> GetAllCars()
	{
		return new List<CarInfo>(this.CarsInDatabase);
	}

    public List<CarInfo> GetAllCarsFromManufacturer(string manufacturerID)
	{
		return this.CarsInDatabase.FindAll((CarInfo ci) => ci.ManufacturerID == manufacturerID);
	}

	public CarInfo GetCar(string zCarDBKey)
	{
		CarInfo carInfo = this.CarsInDatabase.Find((CarInfo x) => x.Key == zCarDBKey);
		if (carInfo != null)
		{
			return carInfo;
		}
		return this.DefaultCarInfo;
	}

	public bool IsCarInTier(string zCarDBKey, eCarTier tier)
	{
		CarInfo carInfo = this.CarsInDatabase.Find((CarInfo x) => x.Key == zCarDBKey);
		return carInfo != null && carInfo.BaseCarTier == tier;
	}

	public string GetCarNiceName(string zCarDBKey)
	{
		CarInfo car = this.GetCar(zCarDBKey);
        //return car.MediumName;//LocalizationManager.GetTranslation(car.MediumName);
        return LocalizationManager.GetTranslation(car.ShortName);
	}

	public string GetCarShortName(string zCarDBKey)
	{
		CarInfo car = this.GetCar(zCarDBKey);
	    return car.ShortName; //LocalizationManager.GetTranslation(car.ShortName);
	}

	public CarInfo GetRandomCar()
	{
		if (this.CarsInDatabase.Count == 0)
		{
			return this.GetDefaultCar();
		}
		int index = Random.Range(0, this.CarsInDatabase.Count);
		return this.CarsInDatabase[index];
	}

	private List<string> GetCarsToExcludeFromRegulationRace(bool halfMile)
	{
		List<string> list = new List<string>();
        if (GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromRegulationRaces != null)
        {
            list.AddRange(GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromRegulationRaces);
        }
        if (halfMile && GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromHalfMileRegulation != null)
        {
            list.AddRange(GameDatabase.Instance.CarsConfiguration.CarsToExcludeFromHalfMileRegulation);
        }
		return list;
	}

	public CarInfo GetRandomRegulationRaceCar(eCarTier zCarTier, bool halfMile)
	{
		if (this.CarsInDatabase.Count == 0)
		{
			return this.GetDefaultCar();
		}
		List<CarInfo> list = this.CarsInDatabase.FindAll((CarInfo x) => x.BaseCarTier == zCarTier);
		List<string> exclusionCars = this.GetCarsToExcludeFromRegulationRace(halfMile);
		list.RemoveAll((CarInfo x) => exclusionCars.Contains(x.Key));
		if (list.Count == 0)
		{
			return this.GetDefaultCar();
		}
		int index = Random.Range(0, list.Count);
		return list[index];
	}

	public CarInfo GetCarOrNull(string zCarDBKey)
	{
		return this.CarsInDatabase.Find((CarInfo x) => x.Key == zCarDBKey);
	}

	public bool PeekGetCar(string zCarDBKey)
	{
        //foreach (var carInfo in CarsInDatabase)
        //{
        //    Debug.Log(carInfo.Key + "   " + zCarDBKey);
        //}
		CarInfo x2 = this.CarsInDatabase.Find(x => x.Key == zCarDBKey);
		return x2 != null;
	}

	public CarInfo GetDefaultCar()
	{
		return this.DefaultCarInfo;
	}

	public List<CarUpgradeData> GetAllUpgradesOfTypeForCar(string zCarID, eUpgradeType zType)
	{
		List<CarUpgradeData> allAvailableUpgradesForCar = this.GetAllAvailableUpgradesForCar(zCarID);
		return allAvailableUpgradesForCar.FindAll((CarUpgradeData x) => x.UpgradeType == zType);
	}

	public List<CarUpgradeData> GetAllAvailableUpgradesForCar(string zCarID)
	{
		CarInfo car = this.GetCar(zCarID);
		if (car == null)
		{
			return new List<CarUpgradeData>();
		}
		return new List<CarUpgradeData>(car.AvailableUpgradeData);
	}

	public CarUpgradeData GetCarUpgrade(string zCarID, eUpgradeType zUpgradeType, int zLevel)
	{
	    if (zLevel > 0)
	    {
	        List<CarUpgradeData> allUpgradesOfTypeForCar = GetAllUpgradesOfTypeForCar(zCarID, zUpgradeType);
	        return allUpgradesOfTypeForCar.Find(x => x.UpgradeLevel+1 == zLevel);
	    }
        else
            return new CarUpgradeData();
	}

	public List<CarInfo> GetCarsOfTier(eCarTier carTier)
	{
#if UNITY_IOS
		var cars = this.CarsInDatabase.Where((CarInfo x) => x.BaseCarTier == carTier).OrderBy(x=>x.OrderInShowroom).ToList();
		if (!BasePlatform.ActivePlatform.InsideCountry)
		{
			return cars;
		}
		else
		{
			cars.RemoveAll(c =>
				c.ManufacturerID == "id_icko" ||
				c.ManufacturerID == "id_saipa" || c.name.Contains("persia") || c.IsAvailableInShowroomOnlyInsideCountry);
			return cars;
		}
			
#else
        return this.CarsInDatabase.Where((CarInfo x) => x.BaseCarTier == carTier).OrderBy(x=>x.OrderInShowroom).ToList();
#endif
	}

    public int GetUnlockLevelForTier(eCarTier tier)
    {
        switch (tier)
        {
                case eCarTier.TIER_1:
                return 1;
                case eCarTier.TIER_2:
                return 8;
                case eCarTier.TIER_3:
                return 13;
                case eCarTier.TIER_4:
                return 18;
                case eCarTier.TIER_5:
                return 26;
        }
        return 1;
    }

	public List<CarInfo> GetWorldTourWinnableCars()
	{
		return this.CarsInDatabase.FindAll((CarInfo x) => x.IsWorldTourWinnableCar);
	}

    public List<CarInfo> GetWorldTourCars()
    {
        return this.CarsInDatabase.FindAll((CarInfo x) => x.IsWorldTour || x.IsWorldTourWinnableCar);
    }

    public List<CarInfo> GetInternationalCars()
	{
		return this.CarsInDatabase.FindAll((CarInfo x) => x.IsInternationalCar);
	}

    public string GetCarManufacturerID(string carDBKey)
	{
		CarInfo car = this.GetCar(carDBKey);
		if (car == null)
		{
			return "None";
		}
		return car.ManufacturerID;
	}

	public int GetLowestCashPriceInTier(eCarTier tier)
	{
		List<CarInfo> carsOfTier = this.GetCarsOfTier(tier);
		List<CarInfo> list = (from q in carsOfTier
		where q.BuyPrice > 0
		select q).ToList<CarInfo>();
		list.Sort((CarInfo x, CarInfo y) => x.BuyPrice - y.BuyPrice);
		return list[0].BuyPrice;
	}
}
