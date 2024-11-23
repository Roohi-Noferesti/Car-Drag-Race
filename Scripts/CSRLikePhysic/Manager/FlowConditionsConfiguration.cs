using System;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using UnityEngine;

[Serializable]
public class FlowConditionsConfiguration:ScriptableObject
{
    [SerializeField]
    private EligibilityRequirementsDictionary[] m_eligibilityRequirements;

    private Dictionary<string, EligibilityRequirements> FlowConditionRequirements =
        new Dictionary<string, EligibilityRequirements>();


    public void Init()
    {
        FlowConditionRequirements = m_eligibilityRequirements.ToDictionary(k => k.ConditionName,
            v => v.EligibilityRequirements);
    }

	public bool IsFlowConditionValid(string conditionName, IGameState gameState)
	{
	    return !this.FlowConditionRequirements.ContainsKey(conditionName) || this.FlowConditionRequirements[conditionName].IsEligible(gameState);
	}

    [Serializable]
    public class EligibilityRequirementsDictionary
    {
        public string ConditionName;
        public EligibilityRequirements EligibilityRequirements;
    }
}
