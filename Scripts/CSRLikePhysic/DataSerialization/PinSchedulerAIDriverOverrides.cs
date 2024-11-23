using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinSchedulerAIDriverOverrides
	{
		[ProtoMember(1)]
		public string Name;

		[ProtoMember(2)]
		public string NumberPlateString;

		public static PinSchedulerAIDriverOverrides CreateEmpty()
		{
			return new PinSchedulerAIDriverOverrides();
		}
	}
}
