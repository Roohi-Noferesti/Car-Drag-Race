using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ManufacturesConfiguration))]
public class ManufacturesConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<ManufacturesConfiguration>(target, (obj) =>
            {
                var config = obj as ManufacturesConfiguration;
                foreach (var configManufacture in config.Manufactures)
                {
                    if (configManufacture.is_tier)
                    {
                        configManufacture.showroomType = Manufacturer.ShowroomType.Tier;
                    }
                    else if (configManufacture.is_worldtour)
                    {
                        configManufacture.showroomType = Manufacturer.ShowroomType.WorldTour;
                    }
                    else if (configManufacture.is_international)
                    {
                        configManufacture.showroomType = Manufacturer.ShowroomType.International;
                    }
                    else
                    {
                        configManufacture.showroomType = Manufacturer.ShowroomType.Manufacturer;
                    }
                }
                EditorUtility.SetDirty(config);
            });
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<ManufacturesConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((ManufacturesConfiguration) target);
        }

        base.OnInspectorGUI();
    }
}
