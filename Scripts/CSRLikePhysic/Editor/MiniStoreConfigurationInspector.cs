using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiniStoreConfiguration))]
public class MiniStoreConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<MiniStoreConfiguration>(target, (obj) =>
            {
                (obj as MiniStoreConfiguration).Setup();
                EditorUtility.SetDirty(obj);

            });
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<MiniStoreConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((MiniStoreConfiguration) target);
        }

        if (GUILayout.Button("Fix Product Names"))
        {
            var config = target as MiniStoreConfiguration;
            foreach (var miniStoreLayoutKeyValue in config.AffordableGoldWithNearestCashAndGoldData)
            {
            }
        }

        base.OnInspectorGUI();
    }
}
