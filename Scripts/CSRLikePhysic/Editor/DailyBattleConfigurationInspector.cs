using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DailyBattleConfiguration))]
public class DailyBattleConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<DailyBattleConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<DailyBattleConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((DailyBattleConfiguration) target);
        }

        base.OnInspectorGUI();
    }
}
