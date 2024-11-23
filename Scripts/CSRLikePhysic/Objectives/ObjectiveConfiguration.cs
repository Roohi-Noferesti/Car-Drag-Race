using System.Collections;
using System.Collections.Generic;
using Objectives;
using UnityEngine;

public class ObjectiveConfiguration : ScriptableObject
{
    public List<ObjectiveDefinition> DailyObjectives;
    public List<ObjectiveRewardDefinition> rewardDefinitions;
}
