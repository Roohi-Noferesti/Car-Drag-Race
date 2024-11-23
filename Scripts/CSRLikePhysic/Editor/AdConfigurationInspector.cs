using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdConfiguration))]
public class AdConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<AdConfiguration>(target);
        }
        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((AdConfiguration)target);
        }
        base.OnInspectorGUI();
    }
}
