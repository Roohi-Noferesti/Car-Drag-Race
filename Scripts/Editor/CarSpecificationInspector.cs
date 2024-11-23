using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarSpecification))]
public class CarSpecificationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save to database"))
        {
        }
    }
}
