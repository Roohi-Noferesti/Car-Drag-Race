using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class CarPricesWindow : EditorWindow
{
    private List<CarDetails> m_carDetails = new List<CarDetails>();
    private CarInfo[] m_objects;
    private Vector2 m_scrollPos;
    private bool m_showUpgrades;
    private PricesConfiguration m_pricesConfiguration;
    private bool m_selected = true;
    private bool m_filter;
    private bool m_showGold = true;
    private bool m_showCash = true;

    [MenuItem("Car/Prices List ...")]
    private static void CreateWizard()
    {
        var window = GetWindow<CarPricesWindow>();
        window.Show();
    }

    void OnEnable()
    {
        GetObjects();
    }

    private void GetObjects()
    {
        EditorUtility.DisplayProgressBar("Loading....", "Getting Cars.Please wait ...", 0);
        var cars = new CarInfo[CarsList.Cars.Length];
        for (int i = 0; i < CarsList.Cars.Length; i++)
        {
            cars[i] = ResourceManager.GetCarAsset<CarInfo>(CarsList.Cars[i], ServerItemBase.AssetType.spec);
            //while (!rr.isDone)
            //{
            //    yield return 0;
            //}
            //cars[i] = (CarInfo) rr.asset;
            if (cars[i] == null)
            {
                Debug.Log(CarsList.Cars[i] + "  not found , you may change resources folder name");
            }
        }

        m_pricesConfiguration = AssetDatabase.LoadAssetAtPath<PricesConfiguration>("Assets/configuration/PricesConfiguration.asset");
        m_objects = cars;

        foreach (CarInfo car in cars)
        {
            var upgrades = m_pricesConfiguration.UpgradesData.Where(u => u.Name.Contains(car.Key.Replace("car", "")))
                .OrderBy(u => u.Name);
            m_carDetails.Add(new CarDetails()
            {
                Name = car.Key,
                PPIndex = car.PPIndex,
                Price = m_pricesConfiguration.CarsData.FirstOrDefault(c=>c.Name==car.Key).PriceInfo,
                UpgradePrices = upgrades.ToArray()
            });
        }
        EditorUtility.ClearProgressBar();

        if (m_objects.Any(c => c == null))
        {
            for (int i = 0; i < m_objects.Length; i++)
            {
                var carInfo = m_objects[i];
                if (carInfo == null)
                {
                    Debug.Log(CarsList.Cars[i] +"  not loaded");
                }
            }
        }
    }

    void OnGUI()
    {
        if (m_objects == null || m_objects.Any(c=>c==null))
        {
            if (GUILayout.Button("Get Objects"))
            {
                GetObjects();
            }
            return;
        }

        EditorGUILayout.BeginHorizontal();
        if (m_filter)
        {
            if (GUILayout.Button("Show All",GUILayout.Width(100)))
            {
                m_filter = false;
            }
        }
        else
        {
            if (GUILayout.Button("Filter selected", GUILayout.Width(100)))
            {
                m_filter = true;
            }
        }

        m_showGold = EditorGUILayout.ToggleLeft("Show Gold", m_showGold, GUILayout.Width(100));
        m_showCash = EditorGUILayout.ToggleLeft("Show Cash", m_showCash, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();

        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
        int i = 1;

        EditorGUILayout.BeginHorizontal(GUILayout.Width(600));
        var selected = EditorGUILayout.ToggleLeft("", m_selected, GUILayout.Width(50));
        if (selected != m_selected)
        {
            m_selected = selected;
            foreach (var carDetailse in m_carDetails)
            {
                carDetailse.Selected = selected;
            }
        }
        if (GUILayout.Button("Name", GUILayout.Width(250)))
        {
            m_carDetails = m_carDetails.OrderBy(m => m.Name).ToList();
        }
        if (GUILayout.Button("PPIndex", GUILayout.Width(100)))
        {
            m_carDetails = m_carDetails.OrderBy(m => m.PPIndex).ToList();
        }
        if (GUILayout.Button("Car Price", GUILayout.Width(150)))
        {
            m_carDetails = m_carDetails.OrderBy(m => m.Price.Gold).ToList();
        }


        foreach (var price in m_carDetails[0].UpgradePrices)
        {
            var label = price.Name.Replace(m_carDetails[0].Name.Replace("car", ""), "");
            if (GUILayout.Button(label, GUILayout.Width(150)))
            {
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndHorizontal();

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        foreach (var carInfo in m_carDetails)
        {
            if(m_filter && !carInfo.Selected)
                continue;
            EditorGUILayout.BeginHorizontal(GUILayout.Width(600));
            carInfo.Selected = EditorGUILayout.ToggleLeft("", carInfo.Selected, GUILayout.Width(50));
            EditorGUILayout.LabelField(i.ToString("00")+". "+carInfo.Name, GUILayout.Width(250));
            EditorGUILayout.LabelField(carInfo.PPIndex.ToString(), GUILayout.Width(100));
            var carPrice = (m_showGold?carInfo.Price.Gold.ToString("N0"):"") + "\t" + (m_showCash?carInfo.Price.Cash.ToString("N0"):"");
            EditorGUILayout.LabelField(carPrice, GUILayout.Width(150));

            foreach (var price in carInfo.UpgradePrices)
            {
                var prcieString = (m_showGold ? price.PriceInfo.Gold.ToString("N0"):"")+ "\t" + (m_showCash ? price.PriceInfo.Cash.ToString("N0"):"");
                EditorGUILayout.LabelField(prcieString, GUILayout.Width(150));
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();
            i++;
        }
        EditorGUILayout.EndScrollView();
    }
}


public class CarDetails
{
    public bool Selected = true;
    public string Name;
    public int PPIndex;
    public PriceInfo Price;
    public PriceInfoDictionary[] UpgradePrices;
}
