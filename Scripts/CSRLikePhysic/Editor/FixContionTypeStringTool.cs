using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class FixContionTypeStringTool : Editor
{
    [MenuItem("Tools/FixConditionType")]
    public static void FixConditionType()
    {
        var selections = Selection.assetGUIDs;

        foreach (var selection in selections)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(selection);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            FixRecursively(asset);
        }
        
        EditorUtility.DisplayDialog("Done", "Fix Done!", "OK");
    }

    static void FixRecursively(object asset)
    {
        if (asset != null)
        {
            var fields = asset.GetType().GetFields().Where(f => (Attribute.IsDefined(f, typeof(SerializeField)) || (f.IsPublic && !f.IsStatic && !Attribute.IsDefined(f, typeof(NonSerializedAttribute)))));

            foreach (var fieldInfo in fields)
            {
                object propValue = fieldInfo.GetValue(asset);
                var elems = propValue as IList;
                if (elems != null)
                {
                    foreach (var item in elems)
                    {
                        FixRecursively(item);
                    }
                }
                else if (fieldInfo.FieldType == typeof(EligibilityRequirements))
                {
                    var value = fieldInfo.GetValue(asset);
                    var eligibleRequirement = value as EligibilityRequirements;

                    foreach (var possibleGameState in eligibleRequirement.PossibleGameStates)
                    {
                        foreach (var condition in possibleGameState.Conditions)
                        {
                            if (string.IsNullOrEmpty(condition.Type))
                            {
                                condition.Type = condition.ConditionType.ToString();
                            }
                        }
                    }
                }
                else
                {
                    FixRecursively(propValue);
                }
            }
        }
    }
}
