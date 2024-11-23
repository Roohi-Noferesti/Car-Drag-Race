using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(SceneManagerTestCarPhysics))]
public class SceneManagerTestCarPhysicsInspector : Editor
{
    private SerializedObject so;
    private SerializedProperty prop;
    private static string[] m_carNames;

    void OnEnable()
    {
        if (Application.isPlaying)
            return;
        if (m_carNames == null || m_carNames.Length == 0)
        {
            m_carNames = CarsList.Cars.OrderBy(c=>c).ToArray();
            var listName = new List<string>(m_carNames);
            listName.Insert(0, "None");
            m_carNames = listName.ToArray();
        }
    }
    public override void OnInspectorGUI()
    {
        var so = serializedObject;
        //var sceneManagerTest = target as SceneManagerTestCarPhysics;
        prop = so.GetIterator();
        prop.Next(true);
        while (prop.NextVisible(true))
        {
            //includeChildren = false;
            if (prop.name == "m_raceEventData")
                continue;
            if (prop.name == "HumanCar" || prop.name == "AICar")
            {
                var selected = prop.stringValue;
                var selectedIndex = ArrayUtility.IndexOf(m_carNames, selected);
                if (selectedIndex == -1)
                    selectedIndex = 0;

                var index = EditorGUILayout.Popup(prop.name, selectedIndex, m_carNames);

                if (index != selectedIndex)
                {
                    selectedIndex = index;
                    prop.stringValue = m_carNames[selectedIndex] == "None" ? String.Empty : m_carNames[selectedIndex];
                }
            }
            else
            {
                var includeChildren = prop.hasChildren || prop.isExpanded;
                EditorGUILayout.PropertyField(prop, includeChildren);
            }
        }

        if (GUI.changed)
        {
            so.ApplyModifiedProperties();
        }
    }
}
