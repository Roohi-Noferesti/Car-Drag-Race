using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressionPopupsConfiguration))]
public class ProgressionPopupsConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<ProgressionPopupsConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<ProgressionPopupsConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((ProgressionPopupsConfiguration) target);
        }

        if (GUILayout.Button("Log Event Complete"))
        {
            var popups = target as ProgressionPopupsConfiguration;
            for (var index = 0; index < popups.PopupsData.Count; index++)
            {
                var progressionMapPinsData = popups.PopupsData[index];
                for (var i = 0;
                    i < progressionMapPinsData.Popup.PopupRequirements.PossibleGameStates.Count;
                    i++)
                {
                    var showingRequirementsPossibleGameState =
                        progressionMapPinsData.Popup.PopupRequirements.PossibleGameStates[i];
                    for (var j = 0; j < showingRequirementsPossibleGameState.Conditions.Count; j++)
                    {
                        var eligibilityCondition = showingRequirementsPossibleGameState.Conditions[j];
                        if (eligibilityCondition.Type == EligbilityConditionType.EventComplete.ToString())
                        {
                            //if (eligibilityCondition.Details.IntValue == 534)
                            //{
                            //    eligibilityCondition.Details.IntValue = 4101;
                            //}
                            //else if (eligibilityCondition.Details.IntValue == 535)
                            //{
                            //    eligibilityCondition.Details.IntValue = 4102;
                            //}
                            //else if (eligibilityCondition.Details.IntValue == 536)
                            //{
                            //    eligibilityCondition.Details.IntValue = 4103;
                            //}
                            Debug.Log("Event Complete at : " + index + "-" + progressionMapPinsData.Desc + ",game state " + i +
                                      ", condition " + j + " : " + eligibilityCondition.Details.IntValue);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(target);
        }

        base.OnInspectorGUI();
    }
}
