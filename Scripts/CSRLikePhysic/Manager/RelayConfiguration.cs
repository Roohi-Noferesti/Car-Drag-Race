using System;
using System.Collections.Generic;
using DataSerialization;
using UnityEngine;

[Serializable]
public class RelayConfiguration:ScriptableObject
{
	public int RelayLegBadRunPPDifference;

	public int MechanicPPBoost;

	public int EasyGrindReward;

	public int ChallengingGrindReward;

	public int HardGrindReward;

	public float EasyTimeDifference;

	public float ChallengingTimeDifference;

	public float HardTimeDifference;

	public float TimeDifferenceToDifficultyGradient;

	public float TimeDifferenceToDifficultyBias;

	public Dictionary<string, float> HMTimes = new Dictionary<string, float>();

	public EligibilityRequirements ChallengingGrindRequirements;

	public EligibilityRequirements HardGrindRequirements;
}
