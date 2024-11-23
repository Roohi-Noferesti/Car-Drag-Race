using KingKodeStudio;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HUDScreen),true),CanEditMultipleObjects]
public class HUDScreenInspector :Editor
{
    private SerializedObject so;
    private SerializedProperty prop;
    private string eventPropNames = "OnScreenOpen,OnScreenClosed,OnBackButton";

    void OnEnable()
    {
        so = new SerializedObject(target);
    }

    override public void OnInspectorGUI()
    {
        var screen = target as HUDScreen;
        var type = (HUDScreen.HudScreenVisibilityType)so.FindProperty("visibilityType").enumValueIndex;
        prop = so.GetIterator();
        prop.Next(true);
        while (prop.NextVisible(false))
        {
            //includeChildren = false;
            if (prop.name == "m_openAnimation" || prop.name == "m_closeAnimation")
            {
                if (type == HUDScreen.HudScreenVisibilityType.Animation)
                {
                    EditorGUILayout.PropertyField(prop);
                }
            }
            else if (eventPropNames.Contains(prop.name))
            {
                if (prop.name == "OnBackButton")
                {
                        ShowEventProp();
                }
                else
                {
                    ShowEventProp();
                }
                //while (eventPropNames.Contains(prop.name))
                //{
                //    prop.NextVisible(false);
                //}
            }
            else
            {
                var includeChildren = prop.hasChildren || prop.isExpanded;
                EditorGUILayout.PropertyField(prop, includeChildren);
            }

            if (prop.name == "visibilityType" && type == HUDScreen.HudScreenVisibilityType.Animation)
            {
                EditorStyles.label.wordWrap = true;
                EditorGUILayout.LabelField("Note:This gameobject must have an animator component"
                                           + " with two states : '" + screen.OpenAnimationName + "' And '" +
                                           screen.CloseAnimationName + "' .Set '" + screen.CloseAnimationName +
                                           "' as default state." +
                                           "and make no transition between states other than close default transition.");
            }

            //if (prop.hasChildren)
            //{
            //    includeChildren = prop.isExpanded;
            //}
        }
        

        if (GUI.changed)
        {
            so.ApplyModifiedProperties();
        }
    }

    public void ShowEventProp()
    {
        prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.name);

        if (prop.isExpanded)
        {
            EditorGUILayout.PropertyField(prop);

            //if (GUI.changed)
            //{
            //    so.ApplyModifiedProperties();
            //}
            //while (prop.Next(true))
            //{
            //    if (eventPropNames.Contains(prop.name))
            //        break;
            //    EditorGUILayout.PropertyField(prop);
            //}
        }
    }
}
