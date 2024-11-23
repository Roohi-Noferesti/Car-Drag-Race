using System;
using ProtoBuf;
using UnityEngine;

namespace DataSerialization
{
    [Serializable]
    [ProtoContract]
	public class PopupDataButtonAction
	{
        [ProtoMember(1)]
        //[HideInInspector]
        public string Type;

        [ProtoIgnore]
        public PopupDataButtonActionExtensions.PopupDataButtonActionType ActionType;

        [ProtoMember(2)]
        public EligibilityConditionDetails Details= EligibilityConditionDetails.CreateDefault();
	}
}
