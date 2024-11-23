using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DealConfiguration))]
public class DealConfigurationInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        //if (GUILayout.Button("Load From json"))
        //{
        //    var filePath = EditorUtility.OpenFilePanel("Deal Json", Application.persistentDataPath, "txt");

        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        var json = File.ReadAllText(filePath);
        //        var deal = JsonConverter.DeserializeObject<DealConfiguration>(json);
        //        if (deal != null)
        //        {
        //            var dealConfig = target as DealConfiguration;

        //            dealConfig.Car = deal.Car;

        //            EditorUtility.SetDirty(target);
        //        }
        //    }
        //}

        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<DealConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<DealConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((DealConfiguration)target);
        }

        base.OnInspectorGUI();
    }
}
