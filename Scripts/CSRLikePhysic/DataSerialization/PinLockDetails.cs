using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinLockDetails
	{
		[ProtoMember(1, IsRequired = true)]
		public int IntValue = -1;
	}
}
