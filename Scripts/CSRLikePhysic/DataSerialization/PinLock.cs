using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinLock
	{
		[ProtoMember(1)]
		public string Type = "Never";

		[ProtoMember(2)]
		public PinLockDetails Details = new PinLockDetails();
	}
}
