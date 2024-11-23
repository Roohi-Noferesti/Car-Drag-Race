using Objectives;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(Component),true)]
public class AbstractObjectiveInspector : Editor 
{
    protected override void OnHeaderGUI()
    {
        //base.OnHeaderGUI();
        EditorGUILayout.LabelField("hello");
        //EditorGUILayout.LabelField((target as AbstractObjective).ID);
    }

    public override void OnInspectorGUI()
    {
        //var so = new SerializedObject(target);
        //var sp = so.GetIterator();

        //while (sp.Next(true))
        //{
        //    EditorGUILayout.PropertyField(sp);
        //}
        //base.OnInspectorGUI();
    }
}
