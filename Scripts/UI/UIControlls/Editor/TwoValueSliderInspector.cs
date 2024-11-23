using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TwoValueSlider))]
public class TwoValueSliderInspector : SliderEditor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var prop1 = serializedObject.FindProperty("m_fill2");
        var prop2 = serializedObject.FindProperty("m_value2");
        var prop3 = serializedObject.FindProperty("color2");

        EditorGUILayout.PropertyField(prop1);
        EditorGUILayout.PropertyField(prop2);
        EditorGUILayout.PropertyField(prop3);

        if (GUI.changed)
            serializedObject.ApplyModifiedProperties();
    }
}
