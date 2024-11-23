using System.Collections;
using System.Collections.Generic;
using Objectives;
using UnityEngine;

[System.Serializable]
public class ObjectiveDefinition
{
    public string ObjectiveID;
    public ObjectiveImplementationType Type;
    public int TargetValue;
    public eCarTier TargetTier;
    public string Title;
    public string Description;
    public DifficultyLevel Difficulty;
    public ObjectiveCategory Category;
    public int TotalProgressSteps;
    public float TotalProgressStepsOverride;
    public bool CanUpdateWhenInactive;
    public bool IsSequential;
    public string rewardID;

}