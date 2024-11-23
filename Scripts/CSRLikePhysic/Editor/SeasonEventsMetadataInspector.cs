using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SeasonEventsMetadata))]
public class SeasonEventsMetadataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<SeasonEventsMetadata>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<SeasonEventsMetadata>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((SeasonEventsMetadata) target);
        }

        base.OnInspectorGUI();
    }
}
