using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinSequence
	{
		[ProtoContract]
		public enum eSequenceType
		{
			Ladder,
			Cycle,
			CycleNextValidAfterWin,
			CycleNextValidAfterWinOrLose,
			CycleNextValidNotRaced,
			NextRandomValidAfterWin,
			NextRandomValidAfterWinOrLose,
			Choice
		}

        [ProtoMember(1)]
		public string ID;

		[ProtoMember(2)]
		public string Type;

		[ProtoMember(3)]
		public EligibilityRequirements Requirements;

		[ProtoMember(4)]
		public string RewardsMultipliersID;

		[ProtoMember(5)]
		public List<ScheduledPin> Pins = new List<ScheduledPin>();

		[ProtoMember(6, IsRequired = true)]
		public bool Repeatable;

		[ProtoMember(7, IsRequired = true)]
		public bool AllowRestarts = true;

		[ProtoMember(8)]
		public PinSequenceTimelineData TimelineData = new PinSequenceTimelineData();

		[ProtoMember(9)]
		public eSequenceType TypeEnum;
	}
}
