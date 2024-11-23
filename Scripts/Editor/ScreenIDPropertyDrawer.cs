using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

[CustomPropertyDrawer(typeof(string))]
public class ScreenIDPropertyDrawer : PropertyDrawer
{
    private static ManufacturesConfiguration _manufacturers;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newPos = position;
        newPos.width -= 30;
        //Debug.Log(property.name+"   ,   "+property.stringValue+"  ,  "+ property.propertyPath);
        if (property.propertyPath.Contains("ScreenID"))
        {
            Enum.TryParse(property.stringValue, false, out ScreenID screenID);
            screenID = (ScreenID) EditorGUI.EnumPopup(newPos, label, screenID);
            property.stringValue = screenID.ToString();
            DrawCopyButton(newPos, property.stringValue);
        }
        else if (property.propertyPath.Contains("EventTypeString") || property.propertyPath.Contains("EventType"))
        {

            Enum.TryParse(property.stringValue, false, out ProgressionMapPinEventType eventType);
            eventType = (ProgressionMapPinEventType)EditorGUI.EnumPopup(newPos, label, eventType);
            property.stringValue = eventType.ToString();
            DrawCopyButton(newPos, property.stringValue);
        }
        else if (property.propertyPath.Contains("ConfirmAction") && property.propertyPath.Contains("Type"))
        {
            Enum.TryParse(property.stringValue, false, out PopupDataButtonActionExtensions.PopupDataButtonActionType type);
            type = (PopupDataButtonActionExtensions.PopupDataButtonActionType)EditorGUI.EnumPopup(newPos, label, type);
            property.stringValue = type.ToString();
            DrawCopyButton(newPos, property.stringValue);
        }
        else if (property.propertyPath.Contains("Conditions") && property.propertyPath.Contains("Type"))
        {
            Enum.TryParse(property.stringValue, false, out EligbilityConditionType type);
            type = (EligbilityConditionType)EditorGUI.EnumPopup(newPos, label, type);
            property.stringValue = type.ToString();
            DrawCopyButton(newPos,property.stringValue);
        }
        else if (property.propertyPath.EndsWith("ManufacturerID"))
        {
            var manufacturers = ManufacturerIDs;
            int selectedndex = Math.Max(0, manufacturers.ToList().IndexOf(property.stringValue));
            selectedndex = EditorGUI.Popup(newPos, property.displayName, selectedndex, manufacturers.ToArray());
            property.stringValue = manufacturers[selectedndex];
            DrawCopyButton(newPos, property.stringValue);
        }
        else if (property.propertyPath.EndsWith("Manufacturer"))
        {
            var manufacturers = ManufacturerNames;
            int selectedndex = Math.Max(0, manufacturers.ToList().IndexOf(property.stringValue));
            selectedndex = EditorGUI.Popup(newPos, property.displayName, selectedndex, manufacturers.ToArray());
            property.stringValue = manufacturers[selectedndex];
            DrawCopyButton(newPos, property.stringValue);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            //base.OnGUI(position, property, label);
        }
    }


    private List<string> ManufacturerIDs
    {
        get
        {
            if (_manufacturers == null)
            {
                _manufacturers =
                    AssetDatabase.LoadAssetAtPath<ManufacturesConfiguration>("Assets/configuration/Manufactures.asset");
            }

            if (_manufacturers != null)
            {
                var list = _manufacturers.Manufactures.Select(m => m.id).ToList();
                list.Insert(0, "None");
                return list;
            }
            return new List<string>();
        }
    }


    private List<string> ManufacturerNames
    {
        get
        {
            if (_manufacturers == null)
            {
                _manufacturers =
                    AssetDatabase.LoadAssetAtPath<ManufacturesConfiguration>("Assets/configuration/Manufactures.asset");
            }

            if (_manufacturers != null)
            {
                var list = _manufacturers.Manufactures.Select(m => m.name).ToList();
                list.Insert(0, "None");
                return list;
            }
            return new List<string>();
        }
    }

    private void DrawCopyButton(Rect position,string value)
    {
        position.x = position.xMax;
        position.width = 30;
        if (GUI.Button(position, "C"))
        {
            EditorGUIUtility.systemCopyBuffer = value;
        }
    }
}
