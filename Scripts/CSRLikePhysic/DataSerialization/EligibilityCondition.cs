using System;
using ProtoBuf;
using UnityEngine;

namespace DataSerialization
{
    [ProtoContract]
	[Serializable]
	public class EligibilityCondition
	{
        [ProtoMember(1)]
        //[HideInInspector]
        public string Type;

        public bool IsActive = true;

        [HideInInspector]
        [ProtoIgnore]
        public EligbilityConditionType ConditionType;

        [ProtoMember(2)]
	    public EligibilityConditionDetails Details= EligibilityConditionDetails.CreateDefault();
	}
}
