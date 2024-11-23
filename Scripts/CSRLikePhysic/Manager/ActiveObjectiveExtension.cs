using System.Collections;
using System.Collections.Generic;
using Objectives;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public static class ActiveObjectiveExtension
{
    public static IEnumerable<ObjectiveData> ToObjectiveDataEnumerable(this IEnumerable<AbstractObjective> abstractObjectives)
    {
        var objectiveDatas = new List<ObjectiveData>();

        foreach (var current in abstractObjectives)
        {
            var objectivedata = new ObjectiveData();
            objectivedata.ObjectiveID = current.ID;
            objectivedata.Completed = current.IsComplete;
            objectivedata.Data = current.ToDict().ToString();
            objectiveDatas.Add(objectivedata);
        }

        return objectiveDatas;
    }

    public static ObjectiveData ToObjectiveData(this AbstractObjective abstractObjectives)
    {
        var objectivedata = new ObjectiveData();
        objectivedata.ObjectiveID = abstractObjectives.ID;
        objectivedata.Completed = abstractObjectives.IsComplete;
        objectivedata.Data = abstractObjectives.ToDict().ToString();
        return objectivedata;
    }
}