using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.spacepuppyeditor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RaceEventGroup))]
public class RaceEventDataGroupPropertyDrawer : PropertyDrawer
{
    private int minPP;
    private int maxPP;
    private bool modifyPPIndex;
    private string carKey;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, property.isExpanded);

        if (property.isExpanded)
        {
            var labelRect = new Rect(position.x, position.yMax - 100, position.width, 20);
            EditorGUI.LabelField(labelRect,"Normalizing Event PP Index");
            var buttonRect = new Rect(position.x, position.yMax - 20, position.width, 20);
            var modifyRect = new Rect(position.x, position.yMax - 40, position.width, 20);
            var carKeyRect = new Rect(position.x, position.yMax - 60, position.width, 20);
            var minRect = new Rect(position.x, position.yMax - 80, position.width/2, 20);
            var maxRect = new Rect(minRect.xMax, position.yMax - 80, position.width/2, 20);
            modifyPPIndex = EditorGUI.Toggle(modifyRect,"Modify PP", modifyPPIndex);
            minPP = EditorGUI.IntField(minRect,"MinValue", minPP);
            maxPP = EditorGUI.IntField(maxRect, "MaxValue", maxPP);
            carKey = EditorGUI.TextField(carKeyRect, "Car Key", carKey);
            if (GUI.Button(buttonRect, "Normalize"))
            {
                var raceEventGroup = (RaceEventGroup)GetPropertyValue(property);
                if (raceEventGroup != null)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Modify PP");
                    var eventMinPP = raceEventGroup.RaceEvents.First().AIPerformancePotentialIndex;
                    var eventMaxPP = raceEventGroup.RaceEvents.Last().AIPerformancePotentialIndex;
                    Debug.Log("Event start from : " + eventMinPP + " to " + eventMaxPP + " . length = " +
                              (eventMaxPP - eventMinPP));
                    Debug.Log("Requested pp start from : " + minPP + " to " + maxPP + " . length = " +
                              (maxPP - minPP));
                    for (int i = 0; i < raceEventGroup.RaceEvents.Count; i++)
                    {
                        var raceEvent = raceEventGroup.RaceEvents[i];
                        var ppIndex = raceEvent.AIPerformancePotentialIndex;
                        var normalizedValue = NormalizeBetween(ppIndex, eventMinPP, eventMaxPP, minPP, maxPP);
                        if (modifyPPIndex)
                        {
                            var eventProperty = property.FindPropertyRelative("RaceEvents").GetArrayElementAtIndex(i);
                            eventProperty.FindPropertyRelative("AIPerformancePotentialIndex").intValue =
                                (int)normalizedValue;
                            var ppIndexData = PPIndexLookupWindow.FindPPIndexData(carKey, (int) normalizedValue);
                            if (!string.IsNullOrEmpty(carKey) && ppIndexData != null)
                            {
                                PPIndexLookupWindow.FillUpgrades(ppIndexData, eventProperty);
                                var carModelrestriction = raceEvent.Restrictions.FirstOrDefault(r =>
                                    r.RestrictionType == eRaceEventRestrictionType.CAR_MODEL);

                                if (carModelrestriction != null)
                                {
                                    var indexOfRest = raceEvent.Restrictions.IndexOf(carModelrestriction);
                                    eventProperty.FindPropertyRelative("Restrictions")
                                            .GetArrayElementAtIndex(indexOfRest).FindPropertyRelative("Model")
                                            .stringValue =
                                        carKey;
                                }
                            }
                            else
                            {
                                Debug.LogError("PP Index not found for pp : "+ normalizedValue+" , ignoring...");
                            }

                        }
                        Debug.Log("event " + i + " : " + ppIndex + " normalized to : " + normalizedValue);
                    }

                    property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
        }
    }

    private float NormalizeBetween(int value, int a1, int b1, int a2, int b2)
    {
        var t = Mathf.InverseLerp(a1, b1, value);
        return Mathf.RoundToInt(Mathf.Lerp(a2, b2, t));

        //or (not tested)
        //return (a2 - b2) / (a1 - b1) * (value - b1) + b2;
    }

    private object GetPropertyValue(SerializedProperty serializedProperty)
    {
        return EditorHelper.GetTargetObjectOfProperty(serializedProperty);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return base.GetPropertyHeight(property, label);
        return EditorGUI.GetPropertyHeight(property, label,true)+110;
    }
}
