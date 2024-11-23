using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CarPerformanceWizard : EditorWindow
{
    private Vector2 m_scrollPos;
    private CarInfo m_carInfo;
    //private byte[] m_upgradeLevels;
    private bool m_useNitrous;
    private float m_power;
    private float m_topSpeed;
    private float m_et;
    private float m_kph;
    private float m_rate;
    private int basePower;
    private int baseWeight;
    private int baseGrip;
    private int basegearbox;
    private int newPerformanceIndex;
    private eCarTier m_tier;
    private int m_upgradelevelStep = 3;
    private bool m_sortByPPIndex;
    //private int m_lastPredefinedPpIndex = 0;
    private int m_maxPredefinedCount = 100;
    private eCarTier m_filterGenerationTier;
    SpeedMileStoneTimer timer  = new SpeedMileStoneTimer();

    private bool m_isCalBasePerformanceIndex;
    private string m_errorText;
    private CarStatsCalculatorEditor calc;
    private UpgradeScreenCarStats m_stats;
    private BaseRunCarPhysicsInTightLoop m_baseRunCarPhysics;
    private CarUpgradeSetup upgradeSetup;
    private int _upgradeIndex;
    private SerializedProperty m_selectedUpgradeProp;
    private SerializedObject m_carInfoSerializeObject;
    //private int m_predefinedUpgradeTypeIndex;
    private bool m_generatingPredefinedUpgrades;
    //private bool m_checkedAll;
    private CarDatabase m_cardatabase;
    private bool m_hasMoreUpgrades = true;
    private int totalPossibleUpgrades = 0;
    private bool m_stopGeneratingUpgrades;
    private DateTime m_generationStartAt;
    private int m_randomness = 2;
    private CarInfo[] m_cars;
    private int m_generatingUpgradeForCarIndex;
    private bool m_batchGenerateUpgrades;
    private bool m_clearUpgradesFirst;
    private int m_carOffset;

    [MenuItem("Car/Power Performance Wizard...")]
    private static void CreateWizard()
    {
        var window = GetWindow<CarPerformanceWizard>();
        window.Show();
    }

    void OnEnable()
    {
        //m_upgradeLevels = new byte[CarUpgrades.ValidUpgrades.Count];
        calc = new GameObject("CarStatCalculator_Editor").AddComponent<CarStatsCalculatorEditor>();
        calc.NewPPIndexCalculated += calc_NewPPIndexCalculated;
        calc.CarStatsUpdated += calc_CarStatsUpgraded;
    }

    void OnDestroy()
    {
        if (calc != null)
        {
            calc.NewPPIndexCalculated -= calc_NewPPIndexCalculated;
            calc.CarStatsUpdated -= calc_CarStatsUpgraded;
            DestroyImmediate(calc.gameObject);
        }

        if (m_cardatabase != null)
        {
            //m_carPhysic.OnDestroy();
            DestroyImmediate(m_cardatabase.gameObject);
        }
    }

    void calc_CarStatsUpgraded(UpgradeScreenCarStats obj)
    {
        m_stats = obj;
        Repaint();
    }

    private void OnGUI()
    {
        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, false, false);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(400));
        EditorGUI.BeginChangeCheck();


        m_carInfo = (CarInfo) EditorGUILayout.ObjectField("Car Information", m_carInfo,
            typeof (CarInfo), false);
        var changed = EditorGUI.EndChangeCheck();

        if (GUILayout.Button("batch generate upgrades"))
        {
            m_cars = Resources.LoadAll<CarInfo>("cars");
            if (m_filterGenerationTier < eCarTier.MAX_CAR_TIERS)
            {
                m_cars = m_cars.Where(c => c.BaseCarTier == m_filterGenerationTier).ToArray();
            }
            m_generatingUpgradeForCarIndex = m_carOffset-1;
            m_batchGenerateUpgrades = true;

            if (m_sortByPPIndex)
            {
                m_cars = m_cars.OrderBy(c => c.PPIndex).ToArray();
            }
        }

        m_clearUpgradesFirst = EditorGUILayout.Toggle("Clear all upgrades", m_clearUpgradesFirst);
        m_carOffset = EditorGUILayout.IntField("Start from", m_carOffset);
        m_sortByPPIndex = EditorGUILayout.Toggle("Sort By PPIndex", m_sortByPPIndex);
        m_filterGenerationTier = (eCarTier)EditorGUILayout.EnumPopup("Generation Tier", m_filterGenerationTier);
        DrawGenerationSetting();

        if (m_batchGenerateUpgrades)
        {
            if (!m_generatingPredefinedUpgrades)
            {
                m_generatingUpgradeForCarIndex++;

                if (m_generatingUpgradeForCarIndex >= m_cars.Length)
                {
                    m_batchGenerateUpgrades = false;
                    m_carInfo = null;
                }
                else
                {
                    m_carInfo = m_cars[m_generatingUpgradeForCarIndex];
                }
                changed = true;
            }
        }

        if (changed && m_carInfo != null)
        {
            m_carInfoSerializeObject = new SerializedObject(m_carInfo);

            if (m_cardatabase != null)
            {
                //m_carPhysic.OnDestroy();
                DestroyImmediate(m_cardatabase.gameObject);
            }
            m_cardatabase = CarDatabase.SetupCarDatabaseForEditor(m_carInfo);

            //calc = new CarStatsCalculatorEditor();//m_carPhysic, m_carInfo,m_carsConfiguration.CarPPData);
            ResetUpgradeStatus();

            calc.CalculateStatsForStockCar(m_carInfo.Key);
            calc.SetOutStats(eCarStatsType.STOCK_CAR);
            basePower = calc.HorsePower;
            baseWeight = calc.Weight;
            baseGrip = calc.TyreGrip;
            basegearbox = calc.GearShiftTime;
        }

        if (m_carInfo == null)
        {
            GUILayout.EndScrollView();
            return;
        }


        if (GUILayout.Button("Calculate FlyWheel Power"))
        {
            var stockCarPhysicsSetup = new CarPhysicsSetupCreator(m_carInfo, calc.carPhysics);//,m_carsConfiguration.CarPPData);
            stockCarPhysicsSetup.InitialiseCarPhysics(CarUpgradeSetup.NullCarSetup);
            var power = calc.carPhysics.Engine.TruePeakHorsePower();
            m_carInfo.FlyWheelPower = power;
            EditorUtility.SetDirty(m_carInfo);
        }
        EditorGUILayout.LabelField("Flywheel Power : " + m_carInfo.FlyWheelPower);

        if (GUILayout.Button("Calculate base PPIndex"))
        {
            m_isCalBasePerformanceIndex = true;
            //calc.CalculateStatsForStockCar(m_carInfo.Key);
            var tempUpgradeSetup = new CarUpgradeSetup()
            {
                CarDBKey = m_carInfo.Key,
                UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>()
            };
            foreach (eUpgradeType t in CarUpgrades.ValidUpgrades)
            {
                tempUpgradeSetup.UpgradeStatus[t] = new CarUpgradeStatus()
                {
                    levelFitted = 0
                };
            }
            calc.CalculateUpgradeScreenPerformanceIndex(tempUpgradeSetup, false);
        }
        EditorGUILayout.LabelField("Base PPIndex : " + m_carInfo.BasePerformanceIndex);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < CarUpgrades.ValidUpgrades.Count; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal();
            upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted =
                (byte)EditorGUILayout.IntSlider(CarUpgrades.ValidUpgrades[i].ToString(), upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted,
                    0, GetUpgradeCount(CarUpgrades.ValidUpgrades[i]));

            if (GUILayout.Button("Cfg"))
            {
                if (upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted > 0)
                {

                    m_selectedUpgradeProp =
                        m_carInfoSerializeObject.FindProperty("AvailableUpgradeData")
                            .GetArrayElementAtIndex(
                                upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted - 1 + (i * 5));
                    m_selectedUpgradeProp.isExpanded = true;
                }
                else
                    m_selectedUpgradeProp = null;

            }

            GUILayout.EndHorizontal();

            //upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted = m_upgradeLevels[i];

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Clear All Presets :" +m_carInfo.PredefinedUpgradeSets.Length))
        {
            if(EditorUtility.DisplayDialog("Clear Presets","All presets will be lost?","Ok","Cancel"))
            {
                m_carInfo.PredefinedUpgradeSets = new PredefinedUpgradeSetsData[0];
                m_carInfoSerializeObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_carInfo);
            }
        }
        if (GUILayout.Button("Add To Preset"))
        {
            Undo.RecordObject(m_carInfo, "Add to presets");
            //calc.CalculateUpgradeScreenPerformanceIndex();
            //calc.CalculateStatsForUpgradeScreen(upgradeSetup, CarUpgrades.ValidUpgrades[i], 1,
            //    m_upgradeLevels[i]);
            calc.CalculateStatsForHumanCarWithUpgradeSetup(upgradeSetup);
            PredefinedUpgradeSetsData predefinedUpgradeSets = new PredefinedUpgradeSetsData();
            predefinedUpgradeSets.SetFromUpgradeSetup(newPerformanceIndex, upgradeSetup);

            var definedUpgrades =
                m_carInfo.PredefinedUpgradeSets.FirstOrDefault(p => p.UpgradeData == predefinedUpgradeSets.UpgradeData);

            if (definedUpgrades == null)
            {
                Array.Resize(ref m_carInfo.PredefinedUpgradeSets, m_carInfo.PredefinedUpgradeSets.Length + 1);
                m_carInfo.PredefinedUpgradeSets[m_carInfo.PredefinedUpgradeSets.Length - 1] = predefinedUpgradeSets;
            }
            else
            {
                definedUpgrades.PPIndex = newPerformanceIndex;
            }
            m_carInfoSerializeObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_carInfo);
            //calc.CalculateStatsForStockCar();
        }

        if (GUILayout.Button("Set Invalid Predefined"))
        {
            var predefined = m_carInfo.PredefinedUpgradeSets.FirstOrDefault(p => p.PPIndex <= 0);
            if (predefined == null)
            {
                EditorUtility.DisplayDialog("Not found", "Not found any invalid upgrades", "Ok");
            }
            else
            {
                predefined.FillUpgradeSetup(m_carInfo, ref upgradeSetup);
                timer = CalculateMileStoneTime();
                //calc.CalculatePerformanceIndexWorder(upgradeSetup);
                calc.CalculateUpgradeScreenPerformanceIndex(upgradeSetup,true);
            }
        }

        EditorGUILayout.Separator();
        DrawGenerationSetting();

        if (GUILayout.Button(m_generatingPredefinedUpgrades ? "Cancel" : "Generate")
            || (!m_generatingPredefinedUpgrades && m_batchGenerateUpgrades
                && m_carInfo != null))
        {
            if (!m_generatingPredefinedUpgrades)
            {
                if (m_batchGenerateUpgrades && m_clearUpgradesFirst)
                {
                    m_carInfo.PredefinedUpgradeSets = new PredefinedUpgradeSetsData[0];
                }
                GeneratePredefiendUpgrades();
            }
            else
            {
                CancelGeneratingUpgrades();
            }
        }


        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Upgrade Stat");
        if (GUILayout.Button("Apply Upgrades"))
        {
            //calc.CalculateUpgradeScreenPerformanceIndex();
            calc.CalculateStatsForUpgradeScreen(upgradeSetup, CarUpgrades.ValidUpgrades[0], 0,
                upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[0]].levelFitted);
            //calc.CalculateStatsForStockCar();
        }
        //Engine
        var realDelta = m_stats.CurrentHP + m_stats.DeltaHP - basePower;
        var symbol = realDelta > 1 ? "+" : "-";
        EditorGUILayout.LabelField(string.Format("Power      {0:0}   {1}{2:0} --->  {3:0}", basePower,
            symbol,
            Mathf.Abs(realDelta), m_stats.CurrentHP + m_stats.DeltaHP));

        //Weight
        realDelta = m_stats.CurrentWeight + m_stats.DeltaWeight - baseWeight;
        symbol = realDelta > 0 ? "+" : "-";
        EditorGUILayout.LabelField(string.Format("Weight     {0:0}   {1}{2:0} --->  {3:0}", baseWeight,
            symbol,
            Mathf.Abs(realDelta), m_stats.CurrentWeight + m_stats.DeltaWeight));

        //Grip
        realDelta = m_stats.CurrentGrip + m_stats.DeltaGrip - baseGrip;
        symbol = realDelta > 1 ? "+" : "-";
        EditorGUILayout.LabelField(string.Format("Grip       {0:0}   {1}{2:0} --->  {3:0}", baseGrip,
            symbol,
            Mathf.Abs(realDelta), m_stats.CurrentGrip + m_stats.DeltaGrip));


        //Gearbox
        realDelta = m_stats.CurrentGearShiftTime + m_stats.DeltaGearShiftTime - basegearbox;
        symbol = realDelta > 1 ? "-" : "+";
        EditorGUILayout.LabelField(string.Format("Gearbox    {0:0}   {1}{2:0} --->  {3:0}",
            basegearbox,
            symbol,
            Mathf.Abs(realDelta), m_stats.CurrentGearShiftTime + m_stats.DeltaGearShiftTime));



        if (GUILayout.Button("Calculate Performance"))
        {
            timer = CalculateMileStoneTime();
            //calc.CalculatePerformanceIndexWorder(upgradeSetup);
            calc.CalculateUpgradeScreenPerformanceIndex(upgradeSetup,true);
        }
        EditorGUILayout.LabelField("New PPIndex : " +
                           (calc.IsCalculatingPerformance ? "Calculating ... " : newPerformanceIndex.ToString()));
        EditorGUILayout.LabelField("Car Tier : " + m_tier);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Format("Half K Time : {0:0.000} s" , timer.mQuarterMileTime));
        EditorGUILayout.LabelField(string.Format("Half K Speed : {0:0} k/h" , timer.SpeedAtQuarter));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Format("One K Time : {0:0.000} s" , timer.mHalfMileTime));
        EditorGUILayout.LabelField(string.Format("One K Speed : {0:0} k/h" , timer.SpeedAtHalf));
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField(string.Format("One Half K Time : {0:0.000} s" , timer.m1500MeterTime));
        //EditorGUILayout.LabelField(string.Format("One Half K Speed : {0:0} k/h" , timer.SpeedAt1500Meter));
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Format("0 to 60 : {0:0.000} s" , timer.TimeToReachMilestone[0]));
        EditorGUILayout.LabelField(string.Format("0 to 100 : {0:0.000} s" , timer.TimeToReachMilestone[1]));
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (m_selectedUpgradeProp != null)
        {
            EditorGUILayout.PropertyField(m_selectedUpgradeProp, true);
            if (GUILayout.Button("Save changes"))
            {
                m_carInfoSerializeObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_carInfo);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No upgrade selected");
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

    }

    private void DrawGenerationSetting()
    {
        EditorGUILayout.LabelField("Generation Setting");
        m_upgradelevelStep = EditorGUILayout.IntField("Level Steps", m_upgradelevelStep);
        m_randomness = EditorGUILayout.IntField("Randomness", m_randomness);
        m_maxPredefinedCount = EditorGUILayout.IntField("Generation Count", m_maxPredefinedCount);
    }

    void calc_NewPPIndexCalculated(string name, eCarTier tier, int PPIndex, float QMTime)
    {
        if (m_isCalBasePerformanceIndex)
        {
            m_carInfo.BasePerformanceIndex = PPIndex;
            m_carInfo.BaseCarTier = tier;
            m_isCalBasePerformanceIndex = false;
            EditorUtility.SetDirty(m_carInfo);
        }
        else
        {
            newPerformanceIndex = PPIndex;
        }

        m_tier = tier;
        Repaint();
    }

    private int GetUpgradeCount(eUpgradeType upgradeType)
    {
       return m_carInfo.AvailableUpgradeData.Count(i => i.UpgradeType == upgradeType);
    }

    private CarUpgradeData GetUpgrade(eUpgradeType upgradeType, int levelFitted)
    {
        return
            m_carInfo.AvailableUpgradeData.FirstOrDefault(
                i => i.UpgradeType == upgradeType && i.UpgradeLevel == levelFitted);
    }

    private SpeedMileStoneTimer CalculateMileStoneTime()
    {
        //int currentPPIndex =m_carInfo.BasePerformanceIndex;
        //upgradeSetup.CalculateFettle(m_carInfo, currentPPIndex);
        //upgradeSetup.SetupConsumableParams(m_carInfo, currentPPIndex);
        CarPhysicsSetupCreator setupCreator = new CarPhysicsSetupCreator(m_carInfo, calc.carPhysics);//, m_carsConfiguration.CarPPData);
        setupCreator.InitialiseCarPhysics(upgradeSetup);
        setupCreator.ApplyCarUpgrades(upgradeSetup);
        OptimalGearChangeSpeedCalculator optimalGearChangeSpeedCalculator =
            new OptimalGearChangeSpeedCalculator(setupCreator.CarPhysics);
        optimalGearChangeSpeedCalculator.CalculateGearChangeSpeeds();
        m_baseRunCarPhysics = new BaseRunCarPhysicsInTightLoop(setupCreator.CarPhysics);
        m_baseRunCarPhysics.StartTightLoopRun();
        m_baseRunCarPhysics.Execute();

        return m_baseRunCarPhysics.speedMileStoneTimer;
    }

    private void Update()
    {
        if (calc != null)
        {
            calc.Update();

            if (m_generatingPredefinedUpgrades)
            {
                if (!calc.IsCalculatingPerformance && calc.hasFinishedCalculatingPerformance)
                {
                    //if (calc.piCalculator.PerformanceIndex - m_lastPredefinedPpIndex > m_predefinedPPIndexStep)
                    if (m_carInfo.PredefinedUpgradeSets.All(p => p.PPIndex != newPerformanceIndex))
                    {
                        PredefinedUpgradeSetsData predefinedUpgradeSets = new PredefinedUpgradeSetsData();
                        predefinedUpgradeSets.SetFromUpgradeSetup(newPerformanceIndex, upgradeSetup);
                        Array.Resize(ref m_carInfo.PredefinedUpgradeSets, m_carInfo.PredefinedUpgradeSets.Length + 1);
                        m_carInfo.PredefinedUpgradeSets[m_carInfo.PredefinedUpgradeSets.Length - 1] =
                            predefinedUpgradeSets;
                        //m_lastPredefinedPpIndex = calc.piCalculator.PerformanceIndex;
                    }

                    NextUpgrade();

                    //m_predefinedUpgradeTypeIndex++;
                    //if (m_predefinedUpgradeTypeIndex >= 8)
                    //{
                    //    if (upgradeSetup.UpgradeStatus[(eUpgradeType) 1].levelFitted ==
                    //        CarUpgradeData.NUM_UPGRADE_LEVELS)
                    //    {
                    //        m_checkedAll = true;
                    //    }
                    //    else
                    //    {
                    //        m_predefinedUpgradeTypeIndex = 1;
                    //    }
                    //}

                    //if (m_predefinedUpgradeTypeIndex < 8)
                    //    upgradeSetup.UpgradeStatus[(eUpgradeType) m_predefinedUpgradeTypeIndex].levelFitted += 1;

                    //if (m_carInfo.PredefinedUpgradeSets.Length >= m_maxPredefinedCount || m_checkedAll)
                    if (!m_hasMoreUpgrades)
                    {
                        m_generatingPredefinedUpgrades = false;
                        m_carInfoSerializeObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(m_carInfo);

                        EditorUtility.ClearProgressBar();
                        if (!m_batchGenerateUpgrades)
                        {
                            EditorUtility.DisplayDialog("Done", "total Test : " + totalPossibleUpgrades
                                                                + "\nTotal Added : " +
                                                                m_carInfo.PredefinedUpgradeSets.Length
                                                                + "\nElapsed Time : " + GetElapsedTime(), "Ok");
                        }
                    }
                    else
                    {
                        calc.CalculateUpgradeScreenPerformanceIndex(upgradeSetup,true);
                        EditorUtility.DisplayProgressBar("Generating Predefiend Upgrades",
                            String.Format("Generating for '{0}'.Please wait ... \nElapsedTime : {1}   Found : {2}", m_carInfo!=null ? m_carInfo.ID:"", GetElapsedTime(), m_carInfo.PredefinedUpgradeSets.Length),
                            (float)totalPossibleUpgrades / (Mathf.Pow(6/m_upgradelevelStep, 6)));
                    }
                }
            }
        }
    }

    private void ResetUpgradeStatus()
    {
        upgradeSetup = new CarUpgradeSetup()
        {
            CarDBKey = m_carInfo.Key,
            UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>()
        };
        foreach (eUpgradeType t in CarUpgrades.ValidUpgrades)
        {
            upgradeSetup.UpgradeStatus[t] = new CarUpgradeStatus()
            {
                levelFitted = 0
            };
        }
    }

    private void NextUpgrade()
    {
        totalPossibleUpgrades++;
        int i = Enum.GetValues(typeof(eUpgradeType)).Length-2;
        bool flag = true;
        while (flag)
        {
            flag = false;
//            if (i == 3)
//            {
//                upgradeSetup.UpgradeStatus[(eUpgradeType) i].levelFitted = CarUpgradeData.NUM_UPGRADE_LEVELS;
//            }

            upgradeSetup.UpgradeStatus[(eUpgradeType)i].levelFitted += (byte)m_upgradelevelStep;
            if (upgradeSetup.UpgradeStatus[(eUpgradeType) i].levelFitted > CarUpgradeData.NUM_UPGRADE_LEVELS)
            {
//                if (i == 3)
//                {
//                    upgradeSetup.UpgradeStatus[(eUpgradeType) i].levelFitted = 0;
//                }
//                else
//                {
                    upgradeSetup.UpgradeStatus[(eUpgradeType)i].levelFitted = (byte)UnityEngine.Random.Range(0, m_randomness);
//                }
                i--;
                if (i < 0)
                {
                    m_hasMoreUpgrades = false;
                }
                else
                {
                    flag = true;
                }
            }
        }

        if (m_stopGeneratingUpgrades || m_carInfo.PredefinedUpgradeSets.Length >= m_maxPredefinedCount)
        {
            m_hasMoreUpgrades = false;
        }
    }

    private void GeneratePredefiendUpgrades()
    {
        Undo.RecordObject(m_carInfo, "Generating Predefined upgrades");
        //m_lastPredefinedPpIndex = m_carInfo.BasePerformanceIndex;
        m_generatingPredefinedUpgrades = true;
        //m_checkedAll = false;
        totalPossibleUpgrades = 0;
        m_hasMoreUpgrades = true;
        m_stopGeneratingUpgrades = false;

        for (int i = 0; i < CarUpgrades.ValidUpgrades.Count; i++)
        {
            upgradeSetup.UpgradeStatus[CarUpgrades.ValidUpgrades[i]].levelFitted = 0;
        }
        //m_predefinedUpgradeTypeIndex = 1;
        //upgradeSetup.UpgradeStatus[(eUpgradeType) m_predefinedUpgradeTypeIndex].levelFitted = 1;
        calc.CalculateUpgradeScreenPerformanceIndex(upgradeSetup,true);
        m_generationStartAt = DateTime.Now;

        EditorUtility.DisplayProgressBar("Generating Predefiend Upgrades", "", 0);
    }

    private void CancelGeneratingUpgrades()
    {
        m_hasMoreUpgrades = false;
        m_stopGeneratingUpgrades = true;
        m_batchGenerateUpgrades = false;
    }

    private string GetElapsedTime()
    {
        var timeSpan = DateTime.Now - m_generationStartAt;

        if (timeSpan.Hours > 0)
        {
            return String.Format("{0} h:{1} m:{2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else if (timeSpan.Minutes > 0)
        {
            return String.Format("{0} m:{1} s", timeSpan.Minutes, timeSpan.Seconds);
        }
        else if (timeSpan.Seconds > 0)
        {
            return String.Format("{0} s",timeSpan.Seconds);
        }
        return "-";
    }
}
