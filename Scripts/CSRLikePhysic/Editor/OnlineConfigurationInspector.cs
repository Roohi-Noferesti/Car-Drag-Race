using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineConfiguration))]
public class OnlineConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<OnlineConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<OnlineConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((OnlineConfiguration) target);
        }

        base.OnInspectorGUI();
    }
}
