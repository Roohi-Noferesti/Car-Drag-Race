using System;
using UnityEngine;

[Serializable]
public class CarPerformanceIndexData : ScriptableObject
{
    public float PP100Time;

    public float PP200Time;

    public float PP300Time;

    public float PP400Time;

    public float PP500Time;

    public float PP600Time;

    public float PP700Time = 8f;

    public float PP800Time = 5.9f;

    public float PP100HMTime;

    public float PP200HMTime;

    public float PP300HMTime;

    public float PP400HMTime;

    public float PP500HMTime;

    public float PP600HMTime;

    public float PP700HMTime;

    public float PP800HMTime;

    public int Tier1LowerLimit;

    public int Tier1HigherLimit;

    public int Tier2LowerLimit;

    public int Tier2HigherLimit;

    public int Tier3LowerLimit;

    public int Tier3HigherLimit;

    public int Tier4LowerLimit;

    public int Tier4HigherLimit;

    public int Tier5LowerLimit;

    public int Tier5HigherLimit;
}
