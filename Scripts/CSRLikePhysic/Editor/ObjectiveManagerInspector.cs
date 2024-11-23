using System.Collections;
using System.Collections.Generic;
using Objectives;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectiveManager))]
public class ObjectiveManagerInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Validate"))
        {
            var duplicateDic = new Dictionary<string, int>();
            var noDescription = new List<string>();

            var manager = target as ObjectiveManager;

            var objectives = manager.GetComponentsInChildren<AbstractObjective>();

            foreach (var objective in objectives)
            {
                if (!duplicateDic.ContainsKey(objective.ID))
                {
                    duplicateDic.Add(objective.ID, 0);
                }
                duplicateDic[objective.ID]++;

                if (string.IsNullOrEmpty(objective.Title) || string.IsNullOrEmpty(objective.Description))
                {
                    noDescription.Add(objective.ID);
                }
            }

            int duplicateCount = 0;
            var result = string.Empty;
            foreach (var keyvalue in duplicateDic)
            {
                if (keyvalue.Value > 1)
                {
                    result += keyvalue.Key + " : " + keyvalue.Value + "\n";
                    duplicateCount++;
                }
            }

            if (noDescription.Count > 0)
            {
                foreach (var current in noDescription)
                {
                    result += "No Desc : "+current + "\n";
                    duplicateCount++;
                }
            }



            EditorUtility.DisplayDialog("Validation done ...", result == string.Empty ? "no conflict found" : duplicateCount+ " conflict found:\n"+result,
                "Ok");
        }

        if (GUILayout.Button("Print Count"))
        {
            var objectives = (target as ObjectiveManager).gameObject.GetComponents<AbstractObjective>();
            Debug.Log("Objectives Cont : "+objectives.Length);

        }
    }
}
