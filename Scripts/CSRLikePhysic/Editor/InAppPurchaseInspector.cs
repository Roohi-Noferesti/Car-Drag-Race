using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InAppPurchases))]
public class InAppPurchaseInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<InAppPurchases>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<InAppPurchases>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((InAppPurchases) target);
        }

        base.OnInspectorGUI();
    }
}
