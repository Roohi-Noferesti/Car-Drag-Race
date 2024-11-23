using UnityEngine;
using UnityEditor;

static class ScriptableObjectUtilityMenu
{

    [MenuItem("Assets/Create/CarSpecification")]
    public static void createCarSpecification()
    {
        ScriptableObjectUtility2.CreateAsset<CarSpecification>();
    }

    [MenuItem("Assets/Create/CarUpgradeSpecification")]
    public static void createCarUpgradeSpecification()
    {
        ScriptableObjectUtility2.CreateAsset<CarUpgradeSpecifiction>();
    }

}