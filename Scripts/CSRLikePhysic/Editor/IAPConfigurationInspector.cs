using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IAPConfiguration))]
public class IAPConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<IAPConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((IAPConfiguration)target);
        }

        base.OnInspectorGUI();
    }
}
