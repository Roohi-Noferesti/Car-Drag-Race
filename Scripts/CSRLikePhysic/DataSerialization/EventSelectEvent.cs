using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class EventSelectEvent
	{
		[ProtoMember(1)]
		public string FunctionName = string.Empty;

		[ProtoMember(2)]
		public float EventTime;

		[ProtoMember(3)]
		public EligibilityRequirements EventRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.EventRequirements = null;
		}
	}
}
