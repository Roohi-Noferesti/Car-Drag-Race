using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RaceEventData))]
public class RaceEventDataPropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, property.isExpanded);

        if (property.isExpanded)
        {
            var rect = new Rect(position.x+100, position.yMax-20, position.width-100, 20);
            if (GUI.Button(rect, "Lookup ..."))
            {
                var window = EditorWindow.GetWindow<PPIndexLookupWindow>();
                PPIndexLookupWindow.Property = property;
                window.Show();
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return base.GetPropertyHeight(property, label);
        return EditorGUI.GetPropertyHeight(property, label,true)+20;
    }
}
