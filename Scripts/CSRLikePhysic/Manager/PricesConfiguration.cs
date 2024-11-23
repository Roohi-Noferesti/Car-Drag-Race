using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PricesConfiguration:ScriptableObject
{
    public Dictionary<string, PriceInfo> Cars;

    public Dictionary<string, PriceInfo> Upgrades;
    
    public Dictionary<string, PriceInfo> Liveries;

    [ValueDropdown("tags")]
    public string CarType;

    [ValueDropdown("parts")] 
    public string CarPart;

    public List<PriceInfoDictionary> CarsData;

    public List<PriceInfoDictionary> UpgradesData;
    
    [Searchable]
    public List<PriceInfoDictionary> LiveriesData;

    public static List<string> tags = new List<string>
    {
        CarsList.Cars[0], CarsList.Cars[1], CarsList.Cars[2], CarsList.Cars[3], CarsList.Cars[4], CarsList.Cars[5], CarsList.Cars[6], CarsList.Cars[7], CarsList.Cars[8], CarsList.Cars[9], CarsList.Cars[10],
        CarsList.Cars[11], CarsList.Cars[12], CarsList.Cars[13], CarsList.Cars[14], CarsList.Cars[15], CarsList.Cars[16], CarsList.Cars[17], CarsList.Cars[18], CarsList.Cars[19], CarsList.Cars[20], CarsList.Cars[21],
        CarsList.Cars[22], CarsList.Cars[23], CarsList.Cars[24], CarsList.Cars[25], CarsList.Cars[26], CarsList.Cars[27], CarsList.Cars[28], CarsList.Cars[29], CarsList.Cars[30], CarsList.Cars[31], CarsList.Cars[32],
        CarsList.Cars[33], CarsList.Cars[34], CarsList.Cars[35], CarsList.Cars[36], CarsList.Cars[37], CarsList.Cars[38], CarsList.Cars[39], CarsList.Cars[40], CarsList.Cars[41], CarsList.Cars[42], CarsList.Cars[43],
        CarsList.Cars[44], CarsList.Cars[45], CarsList.Cars[46], CarsList.Cars[47], CarsList.Cars[48], CarsList.Cars[49], CarsList.Cars[50], CarsList.Cars[51], CarsList.Cars[52], CarsList.Cars[53], CarsList.Cars[54],
        CarsList.Cars[55], CarsList.Cars[56], CarsList.Cars[57], CarsList.Cars[58], CarsList.Cars[59], CarsList.Cars[60], CarsList.Cars[61], CarsList.Cars[62], CarsList.Cars[63], CarsList.Cars[64], CarsList.Cars[65],
        CarsList.Cars[66], CarsList.Cars[67], CarsList.Cars[68], CarsList.Cars[69], CarsList.Cars[70]
       
    };
    
    public static List<string> parts = new List<string>
    {
        "", "Body", "Light", "Ring", "Spoiler", "Sticker"
       
    };
    
    public void Initialize()
    {
        Cars= new Dictionary<string, PriceInfo>();
        foreach (var data in CarsData)
        {
            Cars.Add(data.Name, data.PriceInfo);
        }

        Upgrades = new Dictionary<string, PriceInfo>();
        foreach (var data in UpgradesData)
        {
            Upgrades.Add(data.Name, data.PriceInfo);
        }

        Liveries = new Dictionary<string, PriceInfo>();
        foreach (var data in LiveriesData)
        {
            Liveries.Add(data.Name, data.PriceInfo);
        }
    }

#if UNITY_EDITOR
    
    public void OnAfterDeserialization()
    {
        CarsData = new List<PriceInfoDictionary>();
        foreach (var data in Cars)
        {
            CarsData.Add(new PriceInfoDictionary()
            {
                Name = data.Key,
                PriceInfo = data.Value
            });
        }

        UpgradesData = new List<PriceInfoDictionary>();
        foreach (var data in Upgrades)
        {
            UpgradesData.Add(new PriceInfoDictionary()
            {
                Name = data.Key,
                PriceInfo = data.Value
            });
        }

        LiveriesData = new List<PriceInfoDictionary>();
        foreach (var data in Liveries)
        {
            LiveriesData.Add(new PriceInfoDictionary()
            {
                Name = data.Key,
                PriceInfo = data.Value
            });
        }
    }
    
    [Button]
    public void ChangeBodyPrices()
    {
        int _maxSimple = 21;
        int _maxMatte = 22;
        int _maxCustom = 17;
        switch (CarPart)
        {
            case "Body":
                int simple = 1;
                int matte = 1;
                int custom = 0;
                IEnumerable<string> names = new List<string>();
                List<string> keyNames = new List<string>();
                while (simple <= _maxSimple)
                {
                    names = GetKeyForSpecificCar(0, simple);
                    keyNames.Add(names.First());
                    simple++;
                }
                //CSVWriter.AddItems(keyNames);
                _ChangePrices(keyNames);
                _ClearListData(keyNames);
                while (matte <= _maxMatte)
                {
                    names = GetKeyForSpecificCar(1, matte);
                    keyNames.Add(names.First());
                    matte++;
                }
                //CSVWriter.AddItems(keyNames);
                _ChangePrices(keyNames);
                _ClearListData(keyNames);
                while (custom <= _maxCustom)
                {
                    names = GetKeyForSpecificCar(2, custom);
                    keyNames.Add(names.First());
                    custom++;
                }
                //CSVWriter.AddItems(keyNames);
                _ChangePrices(keyNames);
                _ClearListData(keyNames);
                break;
        }

    }

    [Button]
    public void ChangeHeadLightPrices()
    {
        int _maxHeadLightCount = 8;
        IEnumerable<string> names = new List<string>();
        List<string> keyNames = new List<string>();
        int headLightNumber = 1;
        while (headLightNumber <= _maxHeadLightCount)
        {
            names = GetKeyForSpecificCar(3, headLightNumber);
            keyNames.Add(names.First());
            headLightNumber++;
        }
        //CSVWriter.AddItems(keyNames);
        _ChangePrices(keyNames);
        _ClearListData(keyNames);
        
    }
    
    [Button]
    public void ChangeRingPrices()
    {
        int _maxRingCount = 10;
        IEnumerable<string> names = new List<string>();
        List<string> keyNames = new List<string>();
        int ringNumber = 1;
        while (ringNumber <= _maxRingCount)
        {
            names = GetKeyForSpecificCar(4, ringNumber);
            keyNames.Add(names.First());
            ringNumber++;
        }
        //CSVWriter.AddItems(keyNames);
        _ChangePrices(keyNames);
        _ClearListData(keyNames);
        
    }
    
    [Button]
    public void ChangeSpoilerPrices()
    {
        int _maxSpoilerCount = 7;
        IEnumerable<string> names = new List<string>();
        List<string> keyNames = new List<string>();
        int spoilerNumber = 1;
        while (spoilerNumber <= _maxSpoilerCount)
        {
            names = GetKeyForSpecificCar(5, spoilerNumber);
            keyNames.Add(names.First());
            spoilerNumber++;
        }
        //CSVWriter.AddItems(keyNames);
        _ChangePrices(keyNames);
        _ClearListData(keyNames);
        
    }
    
    [Button]
    public void ChangeStickerPrices()
    {
        int _maxStickerCount = 59;
        IEnumerable<string> names = new List<string>();
        List<string> keyNames = new List<string>();
        int StickerNumber = 0;
        while (StickerNumber <= _maxStickerCount)
        {
            names = GetKeyForSpecificCar(6, StickerNumber);
            keyNames.Add(names.First());
            StickerNumber++;
        }
        //CSVWriter.AddItems(keyNames);
        _ChangePrices(keyNames);
        _ClearListData(keyNames);
        
    }

    private IEnumerable<string> _GetCar()
    {
        return from cars in CarsList.Cars
            where cars == CarType
            select cars;
    }

    private void _ChangePrices(List<string> keyNames)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("LiveriesData");
        foreach (var key in keyNames)
        {
            var liveryData = LiveriesData.First(x => x.Name == key);
            if (data.Any(x =>
                x["CarLivery"].ToString().ToLower() == key.ToLower()))
            {
                liveryData.PriceInfo.Cash = (int) data.First(x => 
                    x["CarLivery"].ToString().ToLower() == key.ToLower())["Cash"];
                liveryData.PriceInfo.Gold = (int) data.First(x => 
                    x["CarLivery"].ToString().ToLower() == key.ToLower())["Gold"];
            }
        }
        EditorUtility.SetDirty(this);
    }

    private void _ClearListData<T>(List<T> list)
    {
        list.Clear();
    }

    private IEnumerable<string> GetKeyForSpecificCar(int carIndex, int carNumber)
    {
        return LiveriesData.Where(x=>
            {
                var first = CarAccessories.Cars[carIndex] + carNumber.ToString() + "_" + _GetCar().First();
                return x.Name == first;
            })
            .Select(x=>x.Name).ToArray();
    }
    
#endif

    public void Clear()
    {
        //Cars = new List<PriceInfoDictionary>();
        //Upgrades = new List<PriceInfoDictionary>();
        //Liveries = new List<PriceInfoDictionary>();
    }
    
    /*[Button("Copy Car Liveries")]
    private void CopyCarLiveries()
    {
        try
        {
            string fromCarID = "car_bentley_gt_v8";
            string toCarID = "car_lamborghini_urus";

            EditorUtility.DisplayProgressBar("Updating Prices...", "Loading configurations ", 0);

            //var allCars = Resources.LoadAll<CarInfo>("");
            for (int i=0 ; i<this.LiveriesData.Count ; i++)
            {
                var liveryData = this.LiveriesData[i];
                if (!liveryData.Name.Contains(fromCarID))
                    continue;

                PriceInfoDictionary newLiveryData = new PriceInfoDictionary();
                newLiveryData.Name = liveryData.Name.Replace(fromCarID, toCarID);
                newLiveryData.PriceInfo = liveryData.PriceInfo;
                
                this.LiveriesData.Add(newLiveryData);
            }

            EditorUtility.SetDirty(this);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Update Price Completed", "All Price Info updated", "Ok");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Update Price Failed", e.ToString(), "Ok");
        }
    }*/
}


[Serializable]
public class PriceInfoDictionary
{
    public string Name;
    public PriceInfo PriceInfo;
}

[Serializable]
public class NewPrices
{
    public string CarName;
    public PriceInfo prices;
} 

public class Prices
{
    public int Cash { get; set; }
    public int Gold { get; set; }
    public int MinIndex { get; set; }
}

