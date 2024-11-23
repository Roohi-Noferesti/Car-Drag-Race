using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fabric;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class UpdateLiveryPrices : EditorWindow
{

    private GUILayoutOption[] objreference;
    public Object source;
    private CarInfo[] cars;

    private string Coefficient = "";
    private string GoldUnit = "";
    private string CashUnit = "";
    [MenuItem("Car/ Update Livery Prices")]
    private static void OpenWindow()
    {
        var window = GetWindow<UpdateLiveryPrices>();
        window.Show();
    }



    void OnGUI()
    {
        source = EditorGUILayout.ObjectField(source, typeof(PricesConfiguration), true);

        EditorGUILayout.LabelField("coefficient");
        Coefficient = EditorGUILayout.TextField(Coefficient);
        EditorGUILayout.LabelField("Gold Unit");
        GoldUnit = EditorGUILayout.TextField(GoldUnit);
        EditorGUILayout.LabelField("Cash Unit");
        CashUnit = EditorGUILayout.TextField(CashUnit);
        if (GUILayout.Button("change prices"))
        {
            UpdatePrices(Coefficient, GoldUnit, CashUnit);
        }
    }

    private void UpdatePrices(string coefficient, string goldUnit, string cashUnit)
    {
        float coef = float.Parse(coefficient);
        //   float gUnit = float.Parse(goldUnit);
        //  float cUnit = float.Parse(cashUnit);
        PricesConfiguration config = source as PricesConfiguration;
        GetCarsList();
        Debug.Log("elements number in cars array:::>>>" + cars.Length);
        Debug.Log("Count of Liveries:>>>>" + config.Liveries.Count);
        int operationCounter = 0;
        foreach (var item in config.LiveriesData)
        {
            operationCounter++;
            string carName = "";
            try
            {
                carName = GetCarName(item.Name);

                CarInfo car = cars.First(x => x.name.ToLower() == carName.ToLower());
                int tier = GetCarTierNumber(car.BaseCarTier.ToString());
                item.PriceInfo.Cash = CalculateCashPrice(item.PriceInfo.Cash, tier, coef);
                item.PriceInfo.Gold = CalculateGoldPrice(item.PriceInfo.Gold, tier, coef);
            }
            catch (Exception e)
            {
                Debug.Log("Car Error:::>>>" + carName + ">>>>" + e);
                continue;
            }

        }
        Debug.Log("Number of Operations:>>>" + operationCounter);
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }



    private void GetCarsList()
    {
        cars = new CarInfo[CarsList.Cars.Length];

        for (int i = 0; i < CarsList.Cars.Length; i++)
        {
            cars[i] = ResourceManager.GetCarAsset<CarInfo>(CarsList.Cars[i], ServerItemBase.AssetType.spec);

            if (cars[i] == null)
            {
                //   Debug.Log(CarsList.Cars[i] + "  not found , you may change resources folder name");
            }
        }
    }


    private string GetCarName(string data)
    {

        string[] res = data.Split(new string[] { "car" }, StringSplitOptions.None);
        return "car" + res[1];

    }

    private int GetCarTierNumber(string input)
    {
        string[] res = input.Split('_');
        return int.Parse(res[1]);
    }

    private int CalculateGoldPrice(int currentPrice, int tier, float coef)
    {
        return (int)(currentPrice + ((tier - 1) * coef * currentPrice));
    }

    private int CalculateCashPrice(int currentPrice, int tier, float coef)
    {
        return (int)(currentPrice + ((tier - 1) * coef * currentPrice));

    }


}
