using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializedTimeSpan))]
public class TimeSpanDrawer : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var dayRect = new Rect(position.x, position.y, 50, position.height);
        var hourRect = new Rect(position.x + 60, position.y, 50, position.height);
        var minuteRect = new Rect(position.x + 125, position.y, 50, position.height);
        var secondRect = new Rect(position.x + 180, position.y, 50, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        var labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15;
        EditorGUI.PropertyField(dayRect, property.FindPropertyRelative("m_days"), new GUIContent("d"));
        EditorGUI.PropertyField(hourRect, property.FindPropertyRelative("m_hours"), new GUIContent("h"));
        EditorGUI.PropertyField(minuteRect, property.FindPropertyRelative("m_minutes"), new GUIContent("m"));
        EditorGUI.PropertyField(secondRect, property.FindPropertyRelative("m_seconds"), new GUIContent("s"));
        EditorGUIUtility.labelWidth = labelWidth;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
