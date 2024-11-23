using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ConditionallyModifiedString
	{
		[ProtoMember(1)]
		public StringModification StringModification = new StringModification();

		[ProtoMember(2)]
		public EligibilityRequirements Requirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.Requirements = null;
		}
	}
}
