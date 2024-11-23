using System;
using UnityEditor;
using UnityEngine;

public class CarPowerWizard : EditorWindow
{
    //private ICarSpecProvider m_specProvider;
    private CarSpecification m_carSpecification;
    private CarUpgradeSpecifiction m_carUpgradeSpecification;
    private int m_massLevel;
    private int m_engineLevel;
    private int m_gearBoxLevel;
    private int m_nitrousLevel;
    private int m_tyreLevel;
    //private int m_turboLevel;
    private float m_nitrousForce = 1000;
    private bool m_useNitrous;
    //private float m_power;
    //private float m_topSpeed;
    //private float m_et;
    //private float m_kph;
    //private float m_rate;

    private string m_errorText;

    [MenuItem("Car/Power Wizard...")]
    private static void CreateWizard()
    {
        var window = GetWindow<CarPowerWizard>();
        window.Show();
    }

    private void OnGUI()
    {
        m_carSpecification = (CarSpecification) EditorGUILayout.ObjectField("Car Specification", m_carSpecification,
            typeof (CarSpecification), false);
        m_carUpgradeSpecification =
            (CarUpgradeSpecifiction) EditorGUILayout.ObjectField("Car Upgrade Specification", m_carUpgradeSpecification,
                typeof (CarUpgradeSpecifiction), false);

        if (m_carUpgradeSpecification == null || m_carSpecification == null) return;

        //Body
        EditorGUILayout.BeginHorizontal();
        m_massLevel = EditorGUILayout.IntSlider("Body", m_massLevel, 0,
            m_carUpgradeSpecification.bodyLevelCount - 1);
        var modifier = m_carUpgradeSpecification.getMassModifier(m_massLevel);
        var symbol = modifier > 1 ? "+" : "-";
        EditorGUILayout.LabelField(string.Format("{0:0} kg   {1}{2:0}% --->  {3:0} kg", m_carSpecification.mass, symbol,
            Mathf.Abs(1 - modifier)*100, modifier*m_carSpecification.mass));
        EditorGUILayout.EndHorizontal();


        //Engine
        EditorGUILayout.BeginHorizontal();
        m_engineLevel = EditorGUILayout.IntSlider("Engine", m_engineLevel, 0,
            m_carUpgradeSpecification.engineLevelCount - 1);
        modifier = m_carUpgradeSpecification.getEngineModifier(m_engineLevel);
        symbol = modifier > 1 ? "+" : "-";
        EditorGUILayout.LabelField(string.Format("{0:0} hp   {1}{2:0}% --->  {3:0} hp", m_carSpecification.maxPower,
            symbol,
            Mathf.Abs(1 - modifier)*100, modifier*m_carSpecification.maxPower));
        EditorGUILayout.EndHorizontal();


        //Gearbox
        EditorGUILayout.BeginHorizontal();

        m_gearBoxLevel = EditorGUILayout.IntSlider("Gearbox", m_gearBoxLevel, 0,
            m_carUpgradeSpecification.shiftLevelCount - 1);
        modifier = m_carUpgradeSpecification.getShiftDelayModifer(m_gearBoxLevel);
        symbol = modifier > 1 ? "-" : "+";
        EditorGUILayout.LabelField(string.Format("shift time ---> {0:0.00}", modifier));
        EditorGUILayout.EndHorizontal();


        //Tyre
        EditorGUILayout.BeginHorizontal();

        m_tyreLevel = EditorGUILayout.IntSlider("Tyre", m_tyreLevel, 0,
            m_carUpgradeSpecification.tyreLevelCount - 1);
        modifier = m_carUpgradeSpecification.getTyreModifer(m_tyreLevel);
        symbol = modifier > 1 ? "-" : "+";
        EditorGUILayout.LabelField(string.Format("ET {0}{1:0.0}%", symbol, Mathf.Abs(modifier - 1)*22));
        EditorGUILayout.EndHorizontal();

        //Turbo
        EditorGUILayout.BeginHorizontal();

        //m_tyreLevel = EditorGUILayout.IntSlider("Turbo", m_turboLevel, 0,
            //m_carUpgradeSpecification.turboLevelCount - 1);
        modifier = m_carUpgradeSpecification.getTurboModifer(m_tyreLevel);
        symbol = modifier > 1 ? "-" : "+";
        EditorGUILayout.LabelField(string.Format("{0:0} hp   {1}{2:0}% --->  {3:0} hp", m_carSpecification.maxPower,
            symbol,
            Mathf.Abs(1 - modifier) * 100, modifier * m_carSpecification.maxPower));
        EditorGUILayout.EndHorizontal();
        //Nitrous
        m_useNitrous = EditorGUILayout.ToggleLeft("Use Nitrous", m_useNitrous);
        if (m_useNitrous)
        {
            m_nitrousForce = EditorGUILayout.FloatField("Nitrous Force", m_nitrousForce);

            EditorGUILayout.BeginHorizontal();

            m_nitrousLevel = EditorGUILayout.IntSlider("Nitrous", m_nitrousLevel, 0,
                m_carUpgradeSpecification.nitrousLevelCount - 1);
            modifier = m_carUpgradeSpecification.getNitrousDuration(m_nitrousLevel);
            symbol = modifier > 1 ? "+" : "-";
            //EditorGUILayout.LabelField(string.Format("{0:0} hp   {1}{2:0}% --->  {3:0} hp", m_carSpecification.horsePower, symbol,
            //    Mathf.Abs(1 - modifier) * 100, modifier * m_carSpecification.horsePower));
            EditorGUILayout.EndHorizontal();
        }



        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Performance :");
        EditorGUI.indentLevel = 1;
        //EditorGUILayout.LabelField(String.Format("Power :{0:0} ", m_power));
        //EditorGUILayout.LabelField(String.Format("Top speed :{0:0} kmh ", m_topSpeed));
        //EditorGUILayout.LabelField(String.Format("Rate : {0:0} ", m_rate));
        //EditorGUILayout.LabelField(String.Format("Elapsed Time (ET) ~ {0:00.00} s ", m_et));
        //EditorGUILayout.LabelField(String.Format("Top End Speed (KPH) ~ {0:0.0} km/h ", m_kph));

        if (GUI.changed)
        {
            m_errorText = "";
            if (m_carSpecification == null)
                m_errorText += "\nCar Specification is null";

            if (m_carSpecification == null)
                m_errorText += "\nCar Upgrade Specification is null";

            if (!string.IsNullOrEmpty(m_errorText))
                return;


            //m_specProvider = new SpecProviderBase(m_carSpecification, m_carUpgradeSpecification,
            //    m_massLevel, m_engineLevel, m_gearBoxLevel, m_nitrousLevel, m_useNitrous,m_tyreLevel,m_turboLevel);
            //m_power = m_specProvider.maxPower;
            //m_topSpeed = m_specProvider.topSpeed;
            //m_et = m_specProvider.ET;
            //m_rate = 10000/m_et;
            //m_kph = m_specProvider.KPH;
        }


        if (!string.IsNullOrEmpty(m_errorText))
        {
            var errors = m_errorText.Split("\n".ToCharArray());
            EditorGUILayout.LabelField("Please Fix this error(s) : ");

            foreach (var error in errors)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField(error);
            }
        }
    }
}
