using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class CarListWindow : EditorWindow
{
    private static CarInfo[] m_objects;
    private Vector2 m_scrollPos;
    private bool m_showUpgrades;
    private bool m_carListSelected = false;
    private int m_selectedIndex = -1;

    [MenuItem("Car/Car List ...")]
    private static void CreateWizard()
    {
        var window = GetWindow<CarListWindow>();
        window.Show();
    }

    void OnEnable()
    {
        GetOldGTCars();
    }

    public static CarInfo[] GetOldGTCars()
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
        m_objects = cars;//Resources.LoadAll<CarInfo>("").Where(c => c.Available && !string.IsNullOrEmpty(c.Key)).ToArray();
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

        return m_objects;
    }

    public static CarInfo[] LoadAllCarsAtPath(string path)
    {
        EditorUtility.DisplayProgressBar("Loading....", "Getting Cars.Please wait ...", 0);

        List<CarInfo> carinfos = new List<CarInfo>();
        string[] allAssetFiles = Directory.GetFiles(Application.dataPath + path, "*.asset", SearchOption.AllDirectories);
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

        EditorUtility.ClearProgressBar();

        m_objects = carinfos.ToArray();

        if (m_objects.Any(c => c == null))
        {
            for (int i = 0; i < m_objects.Length; i++)
            {
                var carInfo = m_objects[i];
                if (carInfo == null)
                {
                    Debug.Log(CarsList.Cars[i] + "  not loaded");
                }
            }
        }

        return m_objects;
    }


    public static CarInfo[] GetCSRCars()
    {
        return LoadAllCarsAtPath("/CSR Config 3.3.1/CarInfo");
    }


    public static CarInfo[] GetGTCars()
    {
        return LoadAllCarsAtPath("/CarInfo");
    }

    void OnGUI()
    {
        if (!m_carListSelected)
        {
            if (GUILayout.Button("Get old GT Cars"))
            {
                GetOldGTCars();
                m_carListSelected = true;
            }

            if (GUILayout.Button("Get CSR Cars"))
            {
                GetCSRCars();
                m_carListSelected = true;
            }

            if (GUILayout.Button("Get GT Cars"))
            {
                GetGTCars();
                m_carListSelected = true;
            }

            return;
        }

        if (GUILayout.Button("Return to Main"))
        {
            m_selectedIndex = -1;
            m_carListSelected = false;
            m_objects = null;
            return;
        }

        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
        int i = 1;

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Name", GUILayout.Width(300)))
        {
            m_objects = m_objects.OrderBy(m => m.name).ToArray();
        }
        if (GUILayout.Button("Tier", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.BaseCarTier).ToArray();
        }
        if (GUILayout.Button("Base PPIndex", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.PPIndex).ToArray();
        }
        if (GUILayout.Button("Max PPIndex", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.MaxPPIndex).ToArray();
        }
        if (GUILayout.Button("HorsePower", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.FlyWheelPower).ToArray();
        }
        if (GUILayout.Button("Weight", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.BaseChassisData.Mass).ToArray();
        }
        if (GUILayout.Button("Grip", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.BaseTyreData.TireGripMax).ToArray();
        }
        if (GUILayout.Button("Gearbox", GUILayout.Width(100)))
        {
            m_objects = m_objects.OrderBy(m => m.BaseGearBoxData.ClutchDelay).ToArray();
        }

        m_showUpgrades = GUILayout.Toggle(m_showUpgrades, "Upgrades", GUILayout.Width(100));

        if (GUILayout.Button("Engine", GUILayout.Width(200)))
        {
            m_objects = m_objects.OrderBy(m => m.CarEngineSound).ToArray();
        }

        EditorGUILayout.EndHorizontal();

        foreach (var carInfo in m_objects)
        {
            EditorGUILayout.BeginHorizontal();
            var color = GUI.contentColor;
            if (m_selectedIndex != -1 && m_selectedIndex ==i)
            {
                GUI.contentColor = Color.green;
            }
            if (GUILayout.Button(i.ToString("00") + ". " + carInfo.name, GUILayout.Width(300)))
            {
                m_selectedIndex = i;
                Selection.activeObject = carInfo;
            }
            EditorGUILayout.LabelField(carInfo.BaseCarTier.ToString(), GUILayout.Width(100));
            EditorGUILayout.LabelField(carInfo.PPIndex.ToString(), GUILayout.Width(100));
            EditorGUILayout.LabelField(carInfo.MaxPPIndex.ToString(), GUILayout.Width(100));
            var power = carInfo.FlyWheelPower.ToString();
            if (GUILayout.Button(power, GUILayout.Width(100)))
            {
                GUIUtility.systemCopyBuffer = power;
                m_selectedIndex = i;
            }
            var mass = carInfo.BaseChassisData.Mass.ToString();
            if (GUILayout.Button(mass, GUILayout.Width(100)))
            {
                GUIUtility.systemCopyBuffer = mass;
                m_selectedIndex = i;
            }

            var grip = carInfo.BaseTyreData.TireGripMax.ToString();
            if (GUILayout.Button(grip, GUILayout.Width(100)))
            {
                GUIUtility.systemCopyBuffer = grip;
                m_selectedIndex = i;
            }

            var gearbox = (carInfo.BaseGearBoxData.ClutchDelay * 1000).ToString();
            if (GUILayout.Button(gearbox, GUILayout.Width(100)))
            {
                GUIUtility.systemCopyBuffer = gearbox;
                m_selectedIndex = i;
            }

            GUILayout.BeginHorizontal(GUILayout.Width(100));
            if (m_showUpgrades || m_selectedIndex==i)
            {
                var Upgrades = "";
                foreach (var predefinedUpgradeSetsData in carInfo.PredefinedUpgradeSets.OrderBy(u => u.PPIndex))
                {
                    Upgrades += predefinedUpgradeSetsData.PPIndex + ",";
                }
                GUIStyle style = new GUIStyle();
                style.font = GUI.skin.font;
                style.stretchWidth = false;
                style.wordWrap = true;
                style.fixedWidth = 100;
                GUIContent mgc = new GUIContent(Upgrades);
                float myHeight = style.CalcHeight(mgc, 100);
                EditorGUILayout.LabelField(Upgrades, style, GUILayout.Height(myHeight + 10));
            }
            else
            {
                EditorGUILayout.LabelField("...", GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField(carInfo.CarEngineSound.ToString(), GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            i++;
            GUI.contentColor = color;
        }
        EditorGUILayout.EndScrollView();
    }

    private int GetCalculatedGrip(CarInfo carinfo)
    {
        return (int) (((carinfo.BaseChassisData.Mass*2.204f)/(2*carinfo.FlyWheelPower) + 10)*carinfo.FlyWheelPower);
    }

    [MenuItem("Car/Fix Car Layers")]
    public static void FixCarLayers()
    {
        var cars = Resources.LoadAll<CarInfo>("").Where(c => c.Available && !string.IsNullOrEmpty(c.Key)).ToArray();
        EditorUtility.DisplayProgressBar("Loading cars", "Please wait...", 0);
        int i = 0;
        foreach (var carInfo in cars)
        {
            EditorUtility.DisplayProgressBar("Applying fix", String.Format("Updating layers '{0}'...", carInfo.Key),
                (float) i/cars.Length);
            var carModel = ResourceManager.GetCarAsset<GameObject>(carInfo.Key, ServerItemBase.AssetType.garage_model);
            var childs = carModel.GetComponentsInChildren<Transform>(true);

            foreach (var transform in childs)
            {
                var name = transform.name.ToLower();
                if (name == "body")
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Car_Body");
                }
                else if (name.Contains("tire"))
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Car_Tire");
                }
                else if (name.Contains("ring"))
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Car_Ring");
                }

                EditorUtility.SetDirty(transform);
                i++;
            }
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", String.Format("{0} cars has been fixed", i), "OK");
    }
}
