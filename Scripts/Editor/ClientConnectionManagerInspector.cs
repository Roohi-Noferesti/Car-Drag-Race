using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ClientConnectionManager))]
public class ClientConnectionManagerInspector : Editor
{
    private SerializedObject m_so;
    private int m_selectedIndex;
    private SerializedProperty m_selectedProperty;

    void OnEnable()
    {
        m_so = new SerializedObject(target);
        m_selectedProperty = m_so.FindProperty("m_selectedServer");
        m_selectedIndex = m_selectedProperty.intValue;
    }
    public override void OnInspectorGUI()
    {
        if (m_so == null)
            return;

        var property = m_so.GetIterator();
        bool includeChildren = true;
        while (property.NextVisible(includeChildren))
        {
            if (property.name == "m_serverAddress")
            {
                var arraySize = property.arraySize;
                property.NextVisible(true);
                //Draw Size Property
                EditorGUILayout.PropertyField(property);
                //Next Property for skipping Size Property
                for (int i = 0; i < arraySize; i++)
                {
                    property.NextVisible(true);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(property,GUILayout.Width(350));
                    if (EditorGUILayout.Toggle(m_selectedIndex == i))
                    {
                        m_selectedIndex = i;
                        m_selectedProperty.intValue = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (property.name == "m_selectedServer")
            {
            }
            else
            {
                includeChildren = false;
                EditorGUILayout.PropertyField(property, true);
            }
        }

        if (GUI.changed)
        {
            m_so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }

}
