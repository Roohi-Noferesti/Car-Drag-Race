using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ConditionallySelectedSnapshots
	{
		[ProtoMember(1)]
		public List<CarOverride> Cars = new List<CarOverride>();

		[ProtoMember(2)]
		public EligibilityRequirements Requirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.Requirements = null;
		}
	}
}
