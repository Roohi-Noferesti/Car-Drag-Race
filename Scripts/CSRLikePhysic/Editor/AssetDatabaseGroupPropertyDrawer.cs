using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(AssetDatabaseGroup))]
public class AssetDatabaseGroupPropertyDrawer : PropertyDrawer
{
    private string _search = "";
    private bool _searchChanged;
    private bool _firstRun = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var assetsProp = property.FindPropertyRelative("AssetDatabaseAssets");
        var textpos = position;
        textpos.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.BeginChangeCheck();
        _search = EditorGUI.TextField(textpos, _search);
        var changed = EditorGUI.EndChangeCheck();
        if (changed)
            _searchChanged = true;
        var y = textpos.yMax;
        for (int i = 0; i < assetsProp.arraySize; i++)
        {
            var arrayProp = assetsProp.GetArrayElementAtIndex(i);
            if (arrayProp.FindPropertyRelative("m_code").stringValue.Contains(_search))
            {
                var arrayPos = new Rect(textpos.x, y, textpos.width, EditorGUIUtility.singleLineHeight);
                arrayPos.height = EditorGUI.GetPropertyHeight(arrayProp);
                EditorGUI.PropertyField(arrayPos, arrayProp);
                y = arrayPos.yMax;
            }
        }
    }

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{
    //    var height = EditorGUIUtility.singleLineHeight;
    //    var assetsProp = property.FindPropertyRelative("AssetDatabaseAssets");
    //    if (_firstRun || _searchChanged)
    //    {
    //        for (int i = 0; i < assetsProp.arraySize; i++)
    //        {
    //            var arrayProp = assetsProp.GetArrayElementAtIndex(i);
    //            if (arrayProp.FindPropertyRelative("m_code").stringValue.Contains(_search))
    //            {
    //                height += EditorGUI.GetPropertyHeight(assetsProp.GetArrayElementAtIndex(i));
    //            }
    //        }

    //        _firstRun = false;
    //    }


    //    return height;
    //}
}
