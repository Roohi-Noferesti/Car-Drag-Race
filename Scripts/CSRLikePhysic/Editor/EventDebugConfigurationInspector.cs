using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventDebugConfiguration))]
public class EventDebugConfigurationInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset All Events"))
        {
            var eventdebug = target as EventDebugConfiguration;

            eventdebug.ResetAllEvents();
            EditorUtility.SetDirty(target);
        }
    }
}
