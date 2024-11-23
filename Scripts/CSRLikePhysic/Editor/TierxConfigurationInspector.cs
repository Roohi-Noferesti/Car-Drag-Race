using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TierXConfiguration))]
public class TierxConfigurationInspector : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //    if (GUILayout.Button("Migrate Dictionary"))
    //    {
    //        var tierX = target as TierXConfiguration;
    //        foreach (var pinDetail in tierX.ThemeLayout.PinDetails)
    //        {
    //            foreach (var keyValue in pinDetail._texturesSerializable.dictionary)
    //            {
    //                if (!pinDetail._texturesSerializableV2.ContainsKey(keyValue.Key))
    //                    pinDetail._texturesSerializableV2.Add(keyValue.Key, keyValue.Value);
    //            }

    //        }


    //        foreach (var pinTemplate in tierX.ThemeLayout.PinTemplates)
    //        {
    //            foreach (var keyValue in pinTemplate._texturesSerializable.dictionary)
    //            {
    //                if (!pinTemplate._texturesSerializableV2.ContainsKey(keyValue.Key))
    //                    pinTemplate._texturesSerializableV2.Add(keyValue.Key, keyValue.Value);
    //            }

    //        }
    //    }
    //}
}
