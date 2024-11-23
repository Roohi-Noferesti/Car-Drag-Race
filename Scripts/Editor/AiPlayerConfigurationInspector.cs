using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(AIPlayersConfiguration))]
public class AiPlayerConfigurationInspector : Editor
{
    private AIPlayersConfiguration m_aiPlayers;
    private SerializedObject m_so;
    private string m_searchKey;
    private SerializedProperty serializedProperty;
    private List<SerializedProperty> m_matchProperties = new List<SerializedProperty>();

    void OnEnable()
    {
        m_aiPlayers = target as AIPlayersConfiguration;
        m_so = new SerializedObject(target);
        serializedProperty = m_so.FindProperty("AIDrivers");
        Search();
    }
    public override void OnInspectorGUI()
    {
        var searchKey = EditorGUILayout.TextField("Search", m_searchKey, GUILayout.Width(400));
        var arraySizeProp = serializedProperty.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(arraySizeProp);

        if (searchKey != m_searchKey)
        {
            m_searchKey = searchKey;
            Search();
        }

        foreach (var matchProperty in m_matchProperties)
        {
            EditorGUILayout.PropertyField(matchProperty, true);
        }
        //EditorGUILayout.PropertyField(serializedProperty,
        //    serializedProperty.hasVisibleChildren && serializedProperty.isExpanded);
        //while (serializedProperty.NextVisible(hasChildren))
        //{
        //    if (string.IsNullOrEmpty(m_searchKey) || serializedProperty.stringValue.ToLower().Contains(m_searchKey.ToLower()))
        //    {
        //        EditorGUILayout.PropertyField(serializedProperty,
        //            serializedProperty.hasVisibleChildren && serializedProperty.isExpanded);
        //    }

        //    hasChildren = serializedProperty.hasVisibleChildren && serializedProperty.isExpanded;
        //}


        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            EditorUtility.DisplayProgressBar("Generating", "Generating AIs.Please wait...", 0);
            var cars = Resources.LoadAll<CarInfo>("").Where(c => c.Available && !string.IsNullOrEmpty(c.Key));

            var index = 0;
            foreach (var carInfo in cars)
            {
                var key = "CarSpecificDriver_" + carInfo.Key;
                var specificAI = m_aiPlayers.AIDrivers.FirstOrDefault(a => a.AIDriverDBKey == key);
                if (specificAI == null)
                {
                    specificAI = new AIDriverData();
                }
                specificAI.AIDriverDBKey = key;
                specificAI.FirstGearLimitChangeUpPercent = 90; //80f,
                specificAI.LaunchRPMVariation = 200; //500f,
                specificAI.Name = key;
                specificAI.ReactionTime = 0.2f; //0.2f,
                specificAI.RPMFromPeakPowerAtGearChange = 100; //300f,
                specificAI.TargetLaunchRPM = carInfo.OptimalLaunchRPM; //5000f
                m_aiPlayers.AIDrivers.Add(specificAI);


                key = "ExpertDriver_" + carInfo.Key;
                var expertAI = m_aiPlayers.AIDrivers.FirstOrDefault(a => a.AIDriverDBKey == key);
                if (expertAI == null)
                {
                    expertAI = new AIDriverData();
                }
                expertAI.AIDriverDBKey = key;
                expertAI.FirstGearLimitChangeUpPercent = 100; //80f,
                expertAI.LaunchRPMVariation = 0; //500f,
                expertAI.Name = key;
                expertAI.ReactionTime = 0.01f; //0.2f,
                expertAI.RPMFromPeakPowerAtGearChange = 10; //300f,
                expertAI.TargetLaunchRPM = carInfo.OptimalLaunchRPM; //5000f
                m_aiPlayers.AIDrivers.Add(expertAI);

                index++;
                EditorUtility.DisplayProgressBar("Generating", "Generating AIs.Please wait...",
                    (float) index/cars.Count());
            }
            m_so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorUtility.ClearProgressBar();
        }
    }

    private void Search()
    {
        m_matchProperties.Clear();
        for (int i = 0; i < serializedProperty.arraySize; i++)
        {
            var propertyAtArray = serializedProperty.GetArrayElementAtIndex(i);
            var keyProp = propertyAtArray.FindPropertyRelative("AIDriverDBKey");
            var nameprop = propertyAtArray.FindPropertyRelative("Name");

            if (string.IsNullOrEmpty(m_searchKey) || keyProp.stringValue.Contains(m_searchKey)
                || nameprop.stringValue.Contains(m_searchKey))
            {
                m_matchProperties.Add(propertyAtArray);
            }
        }

        m_matchProperties = m_matchProperties.OrderBy(p => p.FindPropertyRelative("AIDriverDBKey").stringValue).ToList();
    }
}
