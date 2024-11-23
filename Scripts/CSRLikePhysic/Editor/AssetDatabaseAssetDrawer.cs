using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(AssetDatabaseAsset))]
public class AssetDatabaseAssetDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.isExpanded = false;
        EditorGUIUtility.labelWidth = 1;
        var objectArrayProp = property.FindPropertyRelative("_objects");
        var foldOutRect = new Rect(position.x, position.y, position.width * .1F, EditorGUIUtility.singleLineHeight);
        objectArrayProp.isExpanded = EditorGUI.Foldout(foldOutRect, objectArrayProp.isExpanded, GUIContent.none);

        //Code
        var codeprop = property.FindPropertyRelative("m_code");
        var codeRect = new Rect(foldOutRect.xMax, position.y, position.width * .4F, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(codeRect, codeprop, new GUIContent());

        var versionprop = property.FindPropertyRelative("m_version");

        //A Plus
        var aPlusRect = new Rect(codeRect.xMax+10, position.y, position.width * .05F, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(aPlusRect, "<"))
        {
            versionprop.ToPreviousVersion();
        }

        //Version
        var versionRect = new Rect(aPlusRect.xMax, position.y, position.width * .1F, EditorGUIUtility.singleLineHeight);
        EditorGUI.IntField(versionRect, versionprop.intValue);

        //A Minus
        aPlusRect = new Rect(versionRect.xMax, position.y, position.width * .05F, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(aPlusRect, ">"))
        {
            versionprop.ToNextVersion();
        }

        //Type
        var typeprop = property.FindPropertyRelative("m_type");
        var typeRect = new Rect(aPlusRect.xMax, position.y, position.width * .25F, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(typeRect, typeprop, new GUIContent());


        //var objectPropheight = EditorGUI.GetPropertyHeight(property);
        //var objectRect = position;
        //objectRect.y += EditorGUIUtility.singleLineHeight;
        //objectRect.height = objectPropheight;
        //EditorGUI.PropertyField(objectRect, objectArrayProp);
        if (objectArrayProp.isExpanded)
        {
            for (int i = 0; i < objectArrayProp.arraySize; i++)
            {
                var indexProp = objectArrayProp.GetArrayElementAtIndex(i);
                var objectElementRect = codeRect;
                objectElementRect.y += EditorGUIUtility.singleLineHeight * (i + 1);
                objectElementRect.width = position.width;
                EditorGUI.PropertyField(objectElementRect, indexProp);
            }
        }
    }

    private bool IsExpanded(SerializedProperty property)
    {
        var objectArrayProp = property.FindPropertyRelative("_objects");
        return objectArrayProp.isExpanded;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var defaultHeight = base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
        if (IsExpanded(property))
            defaultHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_objects"));//property.FindPropertyRelative("_objects").arraySize* EditorGUIUtility.singleLineHeight);
        return defaultHeight;
    }
}
