using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressionMapPinsConfiguration))]
public class ProgressionMapPinsConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<ProgressionMapPinsConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<ProgressionMapPinsConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((ProgressionMapPinsConfiguration) target);
        }

        if (GUILayout.Button("Log Event Complete"))
        {
            var pins = target as ProgressionMapPinsConfiguration;
            foreach (var progressionMapPinsData in pins.MapPins)
            {
                for (var i = 0;
                    i < progressionMapPinsData.ShowingRequirements.PossibleGameStates.Count;
                    i++)
                {
                    var showingRequirementsPossibleGameState =
                        progressionMapPinsData.ShowingRequirements.PossibleGameStates[i];
                    for (var j = 0; j < showingRequirementsPossibleGameState.Conditions.Count; j++)
                    {
                        var eligibilityCondition = showingRequirementsPossibleGameState.Conditions[j];
                        if (eligibilityCondition.Type == EligbilityConditionType.EventComplete.ToString())
                            Debug.Log("Event Complete at : " + progressionMapPinsData.Name + ",game state " + i + ", condition " + j+" : "+ eligibilityCondition.Details.IntValue);
                    }
                }
            }
        }

        base.OnInspectorGUI();
    }
}
