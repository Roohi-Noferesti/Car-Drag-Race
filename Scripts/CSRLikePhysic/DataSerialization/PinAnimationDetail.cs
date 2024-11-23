using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinAnimationDetail
	{
		[ProtoMember(1)]
		public string Name = string.Empty;

		[ProtoMember(2)]
		public string PinLabel = string.Empty;

		[ProtoMember(3)]
		public float EventTime;

		[ProtoMember(4)]
		public bool Required;

		[ProtoMember(5)]
		public EligibilityRequirements AnimationRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.AnimationRequirements = null;
		}
	}
}
