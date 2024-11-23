using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectiveConfiguration))]
public class ObjectiveConfigurationInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("Sort"))
        //{
        //    var oc = target as ObjectiveConfiguration;

        //    var o = oc.DailyObjectives.OrderBy(i => i.Order);
        //    oc.DailyObjectives = o.ToList();

        //    EditorUtility.SetDirty(target);
        //}
    }
}
