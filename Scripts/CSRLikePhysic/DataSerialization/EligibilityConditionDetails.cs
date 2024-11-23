using System;
using System.Collections.Generic;
using System.Globalization;
using ProtoBuf;
using UnityEngine;

namespace DataSerialization
{
    [ProtoContract]
	[Serializable]
	public class EligibilityConditionDetails
	{
        [ProtoContract]
		public enum ConditionMatchRequirment
		{
			Any,
			All,
			Single
		}

        [ProtoMember(1, IsRequired = true)]
		public bool RequiredResult = true;

        [ProtoMember(2, IsRequired = true)]
		public int MinValue = -2147483648;

        [ProtoMember(3, IsRequired = true)]
		public int MaxValue = 2147483647;

        [ProtoMember(4, IsRequired = true)]
		public float MinFloatValue = -3.40282347E+38f;

        [ProtoMember(5, IsRequired = true)]
		public float MaxFloatValue = 3.40282347E+38f;

        [ProtoMember(6, IsRequired = true)]
		public int IncrementValue;

        [ProtoMember(7, IsRequired = true)]
		public int Tier;

        [ProtoMember(8)]
		public string Objective = string.Empty;

        [ProtoMember(9)]
		public TimeSpan TimeSpanDifference = default(TimeSpan);

        [ProtoMember(10)]
		public string TimeDifference;

        [ProtoMember(11)]
		public DateTime MinDateTime = DateTime.MinValue;

        [ProtoMember(12)]
		public DateTime MaxDateTime = DateTime.MaxValue;

        [ProtoMember(13)]
		public string MinTime;

        [ProtoMember(14)]
		public string MaxTime;

        [ProtoMember(15)]
		public string StringValue = string.Empty;

        [ProtoMember(16)]
		public string SequenceID = string.Empty;

        [ProtoMember(17)]
		public string EventType = string.Empty;

        [ProtoMember(18)]
		public string ThemeID = string.Empty;

        [ProtoMember(18)]
		public string GameMode = string.Empty;
		
		[ProtoMember(19, IsRequired = true)]
		public bool BoolValue;

        [ProtoMember(20, IsRequired = true)]
        public int IntValue;

        [ProtoIgnore]
        public float FloatValue;

        [ProtoMember(21)]
		public List<string> StringValues = new List<string>();

        [ProtoMember(22)]
		public List<int> IntValues = new List<int>();

        [ProtoMember(23, IsRequired = true)]
		public bool Won = true;

        [ProtoMember(24)]
		public ConditionMatchRequirment MatchRequirmentEnum;

        [ProtoMember(25)]
		public string MatchRequirment;
		
		[ProtoMember(26)]
		public string RemoteConfigKey;
		
		[ProtoMember(27)]
		public string RemoteConfigValue;

        [ProtoIgnore]
	    public ScreenID ScreenID;

	    public static EligibilityConditionDetails CreateDefault()
		{
			return new EligibilityConditionDetails();
		}
	}
}
