using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarVisuals))]
[CanEditMultipleObjects]
public class CarVisualizerInspector : Editor
{
    private SerializedObject m_so;
    void OnEnable()
    {
        //Debug
        m_so = new SerializedObject(target);
    }
    public override void OnInspectorGUI()
    {
        var p = m_so.GetIterator();
        p.NextVisible(true);
        EditorGUILayout.PropertyField(p);

        while (p.NextVisible(false))
        {
            if (p.name != "m_spoilers")
                EditorGUILayout.PropertyField(p,true);
            else
            {
                EditorGUILayout.PropertyField(p);
                var size = p.arraySize;
                if (p.isExpanded)
                {
                    //Size
                    p.NextVisible(true);
                    EditorGUILayout.PropertyField(p);

                    if (!GUI.changed)
                    for (int i = 0; i < size; i++)
                    {
                        p.NextVisible(false);
                        EditorGUILayout.PropertyField(p, p.isExpanded);
                        var posProperty =
                            m_so.FindProperty("m_spoilers").GetArrayElementAtIndex(i).FindPropertyRelative("m_position");
                        var scaleProperty =
                            m_so.FindProperty("m_spoilers").GetArrayElementAtIndex(i).FindPropertyRelative("m_scale");
                        var rotationProperty =
                            m_so.FindProperty("m_spoilers").GetArrayElementAtIndex(i).FindPropertyRelative("m_rotation");
                        var idProperty =
                            m_so.FindProperty("m_spoilers")
                                .GetArrayElementAtIndex(i)
                                .FindPropertyRelative("m_spoilerID");
                        if (p.isExpanded)
                        {
                            GUI.enabled = Selection.activeTransform != null;
                            if (GUILayout.Button("Get Transform"))
                            {
                                posProperty.vector3Value = Selection.activeTransform.localPosition;
                                scaleProperty.vector3Value = Selection.activeTransform.localScale;
                                rotationProperty.quaternionValue = Selection.activeTransform.localRotation;
                                idProperty.stringValue = Selection.activeTransform.name;
                            }

                            if (GUILayout.Button("Set Transform"))
                            {
                                Undo.RecordObject(Selection.activeTransform, "Set Transform");
                                Selection.activeTransform.localPosition = posProperty.vector3Value;
                                Selection.activeTransform.localScale = scaleProperty.vector3Value;
                                Selection.activeTransform.localRotation = rotationProperty.quaternionValue;
                            }
                            GUI.enabled = true;
                        }
                    }
                }

            }
        }
        if (GUI.changed)
            m_so.ApplyModifiedProperties();

        if (GUILayout.Button("CacheNodes"))
        {
            var visual = (target as CarVisuals);
            visual.CacheChildNodes();
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("SetAsDefaultShaders"))
        {
            var visual = (target as CarVisuals);
            visual.SetDefaultShaders();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("SetDefaultHeight"))
        {
            var visual = (target as CarVisuals);
            visual.SetDefaultBodyHeight();
            EditorUtility.SetDirty(target);
        }
    }
}
