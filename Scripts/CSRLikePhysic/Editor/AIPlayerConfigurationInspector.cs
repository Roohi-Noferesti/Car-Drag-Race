using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIPlayersConfiguration))]
public class AIPlayerConfigurationInspector : Editor
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
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<AIPlayersConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<AIPlayersConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((AIPlayersConfiguration) target);
        }

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
    }

    private void Search()
    {
        CultureInfo culture = CultureInfo.CurrentCulture;;
        m_matchProperties.Clear();
        for (int i = 0; i < serializedProperty.arraySize; i++)
        {
            var propertyAtArray = serializedProperty.GetArrayElementAtIndex(i);
            var keyProp = propertyAtArray.FindPropertyRelative("AIDriverDBKey");
            var nameprop = propertyAtArray.FindPropertyRelative("Name");

            if (string.IsNullOrEmpty(m_searchKey) || culture.CompareInfo.IndexOf(keyProp.stringValue, m_searchKey, CompareOptions.IgnoreCase) >= 0
                || culture.CompareInfo.IndexOf(nameprop.stringValue, m_searchKey, CompareOptions.IgnoreCase)>0)
            {
                m_matchProperties.Add(propertyAtArray);
            }
        }

        m_matchProperties = m_matchProperties.OrderBy(p => p.FindPropertyRelative("AIDriverDBKey").stringValue).ToList();
    }
}

