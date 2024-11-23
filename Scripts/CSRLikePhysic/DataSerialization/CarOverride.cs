using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class CarOverride
	{
		[ProtoMember(1)]
		public string SequenceID;

		[ProtoMember(2)]
		public string ScheduledPinID;

		[ProtoMember(3)]
		public string ChoiceSequenceID;

		[ProtoMember(4)]
		public string ThemeID;

		[ProtoMember(5, IsRequired = true)]
		public bool ShouldSetHumanCarToAI = true;

		[NonSerialized]
		public static List<string> EventFieldsToCopy = new List<string>
		{
			"AIDriver",
			"AICar",
			"AIColourR",
			"AIColourG",
			"AIColourB",
			"AIDriverLivery",
			"UseAIColour",
			"AIDriverCrew",
			"AIDriverCrewNumber"
		};
	}
}
