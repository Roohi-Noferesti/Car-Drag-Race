using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using com.spacepuppyeditor;
using UnityEditor;

public class PPIndexLookupWindow : EditorWindow
{
    public class PPIndexData
    {
        public string CarKey;
        public eCarTier CarTier;
        public PredefinedUpgradeSetsData predefinedUpgrade;
        public float Weight;
        public int Power;
        public eDriveType WheelType;
    }

    private bool m_listingCars;
    private Vector2 m_scrollPos;
    public static CarInfo[] Cars;
    private static int m_ppindex;
    private static eCarTier m_carTier = eCarTier.MAX_CAR_TIERS;
    private List<PPIndexData> m_filteredPPIndex = new List<PPIndexData>();
    private static int m_thresholdMin;
    private static int m_thresholdMax;
    public static SerializedProperty Property;
    private int _currentCarIndex;
    private CarInfo[] _searchingCars;
    private int _currentUpgradeIndex;
    private bool wasCalculating;
    private string _currentUpgradeData;
    private CarStatsCalculatorEditor calc;
    private CarDatabase m_cardatabase;
    private bool m_ppIndexSet;


    [MenuItem("Car/PPIndex Lookup ...")]
    private static void CreateWizard()
    {
        var window = GetWindow<PPIndexLookupWindow>();
        window.Show();
    }

    void OnEnable()
    {
        GetObjects();
        if (m_cardatabase != null)
        {
            //m_carPhysic.OnDestroy();
            DestroyImmediate(m_cardatabase.gameObject);
        }
        m_cardatabase = CarDatabase.SetupCarDatabaseForEditor(Cars);
        calc = new GameObject("CarStatCalculator_Editor").AddComponent<CarStatsCalculatorEditor>();
    }

    void OnDisable()
    {
        if (calc != null)
        {
            DestroyImmediate(calc.gameObject);
        }

        if (m_cardatabase != null)
        {
            DestroyImmediate(m_cardatabase.gameObject);
        }
    }

    private static void GetObjects()
    {
        Cars = CarListWindow.GetGTCars();
    }

    private void Search()
    {
        m_filteredPPIndex.Clear();
        m_listingCars = true;
        _currentCarIndex = 0;
        _currentUpgradeIndex = -1;
    }

    void Update()
    {
        if (calc == null)
        {
            return;
        }

        if (!m_listingCars)
            return;

        calc.Update();

        if (calc.IsCalculatingPerformance)
        {
            return;
        }

        if (wasCalculating)
        {
            var currentCarInfo1 = _searchingCars[_currentCarIndex];
            var predefinedUpgradeSetsData1 = currentCarInfo1.PredefinedUpgradeSets[_currentUpgradeIndex];
            m_filteredPPIndex.Add(new PPIndexData()
            {
                CarTier = currentCarInfo1.BaseCarTier,
                CarKey = currentCarInfo1.Key,
                predefinedUpgrade = predefinedUpgradeSetsData1,
                Weight = calc.playerCarPhysicsSetup.NewWeight,
                Power = calc.playerCarPhysicsSetup.NewPeakHP,
                WheelType = currentCarInfo1.DriveType
            });
            wasCalculating = false;
        }


        _searchingCars = Cars.Where(c => c.Available && (m_carTier >= (eCarTier) 5 || c.BaseCarTier == m_carTier))
            .ToArray();


        _currentUpgradeIndex++;
        if (_currentCarIndex < 0)
        {
            _currentCarIndex = 0;
            _currentUpgradeIndex = 0;
        }
        var currentCarInfo = _searchingCars[_currentCarIndex];

        if (currentCarInfo.PredefinedUpgradeSets.Length <= _currentUpgradeIndex)
        {
            _currentCarIndex++;
            _currentUpgradeIndex = 0;
            if (_searchingCars.Length <= _currentCarIndex)
            {
                m_listingCars = false;
                m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.predefinedUpgrade.PPIndex).ToList();
                EditorUtility.ClearProgressBar();
                Repaint();
                return;
            }
            currentCarInfo = _searchingCars[_currentCarIndex];
        }



        for (; _currentUpgradeIndex < currentCarInfo.PredefinedUpgradeSets.Length; _currentUpgradeIndex++)
        {
            var predefinedUpgradeSetsData = currentCarInfo.PredefinedUpgradeSets[_currentUpgradeIndex];
            if (predefinedUpgradeSetsData.PPIndex >= m_ppindex - m_thresholdMin &&
                predefinedUpgradeSetsData.PPIndex <= m_ppindex + m_thresholdMax)
            {
                CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
                predefinedUpgradeSetsData.FillUpgradeSetup(currentCarInfo, ref carUpgradeSetup);
                calc.CalculateStatsForHumanCarWithUpgradeSetup(carUpgradeSetup);
                wasCalculating = true;
                break;
            }
        }


        if (EditorUtility.DisplayCancelableProgressBar("Calculating",
            "Calculating " + _currentCarIndex + "/" + _searchingCars.Length+":"+_currentUpgradeIndex,
            (float) _currentCarIndex / _searchingCars.Length))
        {
            m_listingCars = false;
            EditorUtility.ClearProgressBar();
            Repaint();
        }

    }

    void OnGUI()
    {
        if (Property != null && !m_ppIndexSet)
        {
            var ppIndex = Property.FindPropertyRelative("AIPerformancePotentialIndex").intValue;
            if (ppIndex > 0)
            {
                var aiCar = Property.FindPropertyRelative("AICar").stringValue;
                if (Cars != null)
                {
                    var carInfo = Cars.FirstOrDefault(c => c.Key == aiCar);
                    if (carInfo != null)
                    {
                        m_carTier = carInfo.BaseCarTier;
                    }
                }
                m_ppindex = ppIndex;
                Search();
            }

            m_ppIndexSet = true;
        }

        GUI.enabled = !m_listingCars;
        if (Cars == null || Cars.Any(c => c == null))
        {
            if (GUILayout.Button("Get Objects"))
            {
                GetObjects();
            }
            return;
        }
        EditorGUILayout.BeginVertical();
        m_ppindex = EditorGUILayout.IntField("Look For PPIndex", m_ppindex,GUILayout.Width(400));
        m_carTier = (eCarTier) EditorGUILayout.EnumPopup("Tier", m_carTier);
        m_thresholdMin = EditorGUILayout.IntSlider("Min Threshold", m_thresholdMin, 0, 15, GUILayout.Width(400));
        m_thresholdMax = EditorGUILayout.IntSlider("Max Threshold", m_thresholdMax, 0, 15, GUILayout.Width(400));
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Search"))
        {
            Search();
        }

        if (m_listingCars)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Listing Cars " + _currentCarIndex);
            return;
        }


        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
        int i = 1;

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Name", GUILayout.Width(200)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.CarKey).ToList();
        }
        if (GUILayout.Button("Tier", GUILayout.Width(80)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.CarTier).ToList();
        }
        if (GUILayout.Button("PPIndex", GUILayout.Width(80)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.predefinedUpgrade.PPIndex).ToList();
        }
        if (GUILayout.Button("Upgrade Set", GUILayout.Width(100)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.predefinedUpgrade.UpgradeData).ToList();
        }
        if (GUILayout.Button("Weight", GUILayout.Width(100)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.Weight).ToList();
        }
        if (GUILayout.Button("Power", GUILayout.Width(100)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.Power).ToList();
        }

        if (GUILayout.Button("Drivetrain", GUILayout.Width(100)))
        {
            m_filteredPPIndex = m_filteredPPIndex.OrderBy(m => m.WheelType).ToList();
        }

        EditorGUILayout.EndHorizontal();

        foreach (var item in m_filteredPPIndex)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString("00") + ". " + item.CarKey, GUILayout.Width(200));
            EditorGUILayout.LabelField(item.CarTier.ToString(), GUILayout.Width(80));
            EditorGUILayout.LabelField(item.predefinedUpgrade.PPIndex.ToString(), GUILayout.Width(80));
            EditorGUILayout.LabelField(item.predefinedUpgrade.UpgradeData, GUILayout.Width(100));
            EditorGUILayout.LabelField(item.Weight.ToString(), GUILayout.Width(100));
            EditorGUILayout.LabelField(item.Power.ToString(), GUILayout.Width(100));
            EditorGUILayout.LabelField(item.WheelType.ToString(), GUILayout.Width(100));
            if (GUILayout.Button("Select", GUILayout.Width(100)))
            {
                if (Property!=null)
                    FillUpgrades(item,Property);
                Close();
            }
            EditorGUILayout.EndHorizontal();
            i++;
        }
        EditorGUILayout.EndScrollView();
    }


    public static PPIndexData FindPPIndexData(string carKey, int ppIndex)
    {
        if (Cars == null)
        {
            GetObjects();
        }
        var carInfo = Cars.FirstOrDefault(c => c.Key == carKey || c.name == carKey);
        if (carInfo == null)
        {
            return null;
        }
        foreach (var predefinedUpgradeSetsData in carInfo.PredefinedUpgradeSets)
        {
            if (predefinedUpgradeSetsData.PPIndex == ppIndex)
            {
                return new PPIndexData()
                {
                    CarTier = carInfo.BaseCarTier,
                    CarKey = carInfo.Key,
                    predefinedUpgrade = predefinedUpgradeSetsData
                };
            }
        }

        return null;
    }



    public static void FillUpgrades(PPIndexData predefinedUpgrade,SerializedProperty property)
    {
        Undo.RecordObject(property.serializedObject.targetObject,"Undo Fill Upgrades");
        string value = predefinedUpgrade.predefinedUpgrade.UpgradeData.Substring(0, 8);
        byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(value, 16));
        property.FindPropertyRelative("BodyUpgradeLevel").intValue = CarUpgradeStatus.Convert(bytes[0] >> 4);
        property.FindPropertyRelative("EngineUpgradeLevel").intValue = CarUpgradeStatus.Convert((int)(bytes[0] & 15));
        property.FindPropertyRelative("IntakeUpgradeLevel").intValue = CarUpgradeStatus.Convert(bytes[1] >> 4);
        property.FindPropertyRelative("NitrousUpgradeLevel").intValue = CarUpgradeStatus.Convert((int)(bytes[1] & 15));
        property.FindPropertyRelative("TransmissionUpgradeLevel").intValue = CarUpgradeStatus.Convert(bytes[2] >> 4);
        property.FindPropertyRelative("TurboUpgradeLevel").intValue = CarUpgradeStatus.Convert((int)(bytes[2] & 15));
        property.FindPropertyRelative("TyreUpgradeLevel").intValue = CarUpgradeStatus.Convert(bytes[3] >> 4);
        int num = predefinedUpgrade.predefinedUpgrade.UpgradeData.IndexOf(":");
        if (num == -1)
        {
            property.FindPropertyRelative("ModifiedCarMass").floatValue = 0f;
        }
        else
        {
            property.FindPropertyRelative("ModifiedCarMass").floatValue = float.Parse(predefinedUpgrade.predefinedUpgrade.UpgradeData.Substring(num + 1),
                CultureInfo.InvariantCulture.NumberFormat);
        }
        property.FindPropertyRelative("UpgradePercentage").floatValue = -1;
        property.FindPropertyRelative("AIPerformancePotentialIndex").intValue = predefinedUpgrade.predefinedUpgrade.PPIndex;

        var oldAiCar = property.FindPropertyRelative("AICar").stringValue;
        var oldAiDriver = property.FindPropertyRelative("AIDriver").stringValue;

        var newAiDriver = GetProperAIDriver(oldAiCar, oldAiDriver, predefinedUpgrade.CarKey);

        property.FindPropertyRelative("AICar").stringValue = predefinedUpgrade.CarKey;
        if (!string.IsNullOrEmpty(newAiDriver))
            property.FindPropertyRelative("AIDriver").stringValue = newAiDriver;

        var raceEvent =  (RaceEventData)property.GetTargetObjectOfProperty();
        var carModelRestr =
            raceEvent.Restrictions.FirstOrDefault(r => r.RestrictionType == eRaceEventRestrictionType.CAR_MODEL);

        if (carModelRestr!= null)
        {
            var index = raceEvent.Restrictions.IndexOf(carModelRestr);
            property.FindPropertyRelative("Restrictions").GetArrayElementAtIndex(index).FindPropertyRelative("Model")
                .stringValue = predefinedUpgrade.CarKey;
        }


        var manufactureRestr =
            raceEvent.Restrictions.FirstOrDefault(r => r.RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER);

        if (manufactureRestr != null)
        {
            var index = raceEvent.Restrictions.IndexOf(manufactureRestr);
            var carInfo = Cars.FirstOrDefault(c => c.Key == predefinedUpgrade.CarKey);
            property.FindPropertyRelative("Restrictions").GetArrayElementAtIndex(index).FindPropertyRelative("Manufacturer")
                .stringValue = carInfo.ManufacturerName;
        }
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }


    private static string GetProperAIDriver(string carKey,string driverKey,string newCarKey)
    {
        var selectedCar = Cars.FirstOrDefault(c => c.Key == carKey || c.name == carKey);

        string aiDriverNameWithoutCarName = null;
        if (!string.IsNullOrEmpty(driverKey) && selectedCar!=null)
        {
            aiDriverNameWithoutCarName = driverKey.Replace(selectedCar.name, string.Empty);
        }

        var csrCarKey = CSRToGTCars.CSR_TO_GT_Cars.FirstOrDefault(c => c.Value == newCarKey).Key;
        var newCar = Cars.FirstOrDefault(c => c.Key == csrCarKey || c.name == csrCarKey);
        if (!string.IsNullOrEmpty(aiDriverNameWithoutCarName) && newCar!=null)
        {
            return aiDriverNameWithoutCarName  + newCar.name;
        }

        return null;
    }
}
