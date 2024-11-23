using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ThemeTransition
	{
		[ProtoMember(1)]
		public string ThemeID;

		[ProtoMember(2)]
		public string Option;

		[ProtoMember(3)]
		public EligibilityRequirements TransitionRequirements = EligibilityRequirements.CreateNeverEligible();
	}
}
