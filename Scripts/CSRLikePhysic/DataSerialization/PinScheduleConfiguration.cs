using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinScheduleConfiguration
	{
		[ProtoMember(1)]
		public List<PinSequence> Sequences = new List<PinSequence>();

		[ProtoIgnore]
		public string themeID;
	}
}
