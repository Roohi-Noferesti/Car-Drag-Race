using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialBubblesConfiguration))]
public class TutorialBubblesConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<TutorialBubblesConfiguration>(target, (obj) =>
            {
                (obj as TutorialBubblesConfiguration).AfterDeserialization();
                EditorUtility.SetDirty(obj);
            });
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<TutorialBubblesConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((TutorialBubblesConfiguration) target);
        }

        base.OnInspectorGUI();
    }
}
