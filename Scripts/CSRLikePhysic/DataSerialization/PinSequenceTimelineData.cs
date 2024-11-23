using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinSequenceTimelineData
	{
		[ProtoMember(1, IsRequired = true)]
		public bool ShowTimeline;

		[ProtoMember(2)]
		public string PredecessorSequence = string.Empty;

		[ProtoMember(3)]
		public string SuccessorSequence = string.Empty;
	}
}
