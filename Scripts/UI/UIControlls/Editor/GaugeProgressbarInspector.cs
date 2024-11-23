using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(GaugeProgressBar))]
public class GaugeProgressbarInspector : SliderEditor
{
    private SerializedObject so;
    protected override void OnEnable()
    {
        base.OnEnable();
        so = new SerializedObject(target);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //var gauge = target as GaugeProgressBar;
        ShowProp("m_niddleTransform");
        ShowProp("m_minNiddleDegree");
        ShowProp("m_maxNiddleDegree");
        ShowProp("m_fillImage");
        ShowProp("m_minImageFillAmount");
        ShowProp("m_maxImageFillAmount");


        if (GUI.changed)
            so.ApplyModifiedProperties();
    }

    public void ShowProp(string propName)
    {
        var prop = so.FindProperty(propName);
        EditorGUILayout.PropertyField(prop);
    }
}
