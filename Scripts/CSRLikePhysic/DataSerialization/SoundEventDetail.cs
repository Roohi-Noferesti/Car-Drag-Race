using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class SoundEventDetail
	{
		[ProtoMember(1)]
		public string Name = string.Empty;

		[ProtoMember(2)]
		public float EventTime;

		[ProtoMember(3)]
		public float SoundStart;

		[ProtoMember(4)]
		public EligibilityRequirements SoundRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.SoundRequirements = null;
		}
	}
}
