using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

[CustomEditor(typeof(ServerItemConfiguration))]
public class ServerItemConfigurationInspector : Editor
{
    private static VirtualItemType m_itemType;
    private ServerItemConfiguration m_serverItemConfiguration;

    private int m_pageNumber;
    private int m_pageCount;
    private int m_rowCount = 10;
    private List<SerializedProperty> m_filterItemProps; 

    void OnEnable()
    {
        m_serverItemConfiguration = target as ServerItemConfiguration;
        m_pageNumber = 0;
        m_filterItemProps = new List<SerializedProperty>();
    }
    public override void OnInspectorGUI()
    {
        m_itemType = (VirtualItemType) EditorGUILayout.EnumPopup("Item Type", m_itemType);
        var itemsProp = serializedObject.FindProperty("m_virtualItems");

        if (GUI.changed)
        {
            m_filterItemProps.Clear();
            for (int i = 0; i < m_serverItemConfiguration.ItemLength; i++)
            {
                if (m_serverItemConfiguration.Items[i].ItemType == m_itemType)
                    m_filterItemProps.Add(itemsProp.GetArrayElementAtIndex(i));
            }

            var count = m_filterItemProps.Count;
            m_pageCount = 1 + count/m_rowCount;
            m_pageNumber = 0;
        }

        EditorGUI.BeginChangeCheck();
        for (int i = m_pageNumber * m_rowCount; i < m_filterItemProps.Count && i < (m_pageNumber + 1) * m_rowCount; i++)
        {
            //if (m_itemAsset[i].Item.ItemType == m_itemType)
            //{
            EditorGUILayout.PropertyField(m_filterItemProps[i],true);
            //}

        }
        var end = EditorGUI.EndChangeCheck();

        if (end)
            serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(String.Format("Page {0}/{1}", m_pageNumber + 1, m_pageCount), GUILayout.Width(100));
        GUI.enabled = (m_pageNumber > 0);
        if (GUILayout.Button("<--"))
        {
            m_pageNumber--;
        }

        GUI.enabled = (m_pageNumber < m_pageCount - 1);
        if (GUILayout.Button("-->"))
        {
            m_pageNumber++;
        }
        GUI.enabled = true;

        if (GUILayout.Button("Debug item id"))
        {
            for (int i = 0; i < m_serverItemConfiguration.ItemLength; i++)
            {
                if (m_serverItemConfiguration.Items[i].ItemID.Contains("lykan"))
                    m_filterItemProps.Add(itemsProp.GetArrayElementAtIndex(i));
            }

            var count = m_filterItemProps.Count;
            m_pageCount = 1 + count / m_rowCount;
            m_pageNumber = 0;
        }
        EditorGUILayout.EndHorizontal();


    }
}
