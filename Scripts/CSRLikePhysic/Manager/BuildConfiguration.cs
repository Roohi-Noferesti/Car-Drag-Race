using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildConfiguration", menuName = "ScriptableObjects/BuildConfiguration", order = 1)]
public class BuildConfiguration : ScriptableObject
{
    public BuildTargetStore buildTargetStore = BuildTargetStore.Normal;
    
    public enum BuildTargetStore
    {
        Normal = 0,
        AppTutti = 1,
        Vas = 2
    }
}
