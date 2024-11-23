using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ThemeAnimationDetail
	{
		[ProtoMember(1)]
		public string Name = string.Empty;

		[ProtoMember(2)]
		public List<PinAnimationDetail> PinAnimations = new List<PinAnimationDetail>();

		[ProtoMember(3)]
		public List<EventSelectEvent> EventSelectEvents = new List<EventSelectEvent>();

		[ProtoMember(4)]
		public List<SoundEventDetail> SoundEvents = new List<SoundEventDetail>();

		[ProtoMember(5)]
		public EligibilityRequirements AnimationRequirements = EligibilityRequirements.CreateNeverEligible();

		[ProtoMember(6)]
		public List<EventSelectEvent> InitEventSelectEvents = new List<EventSelectEvent>();
	}
}
