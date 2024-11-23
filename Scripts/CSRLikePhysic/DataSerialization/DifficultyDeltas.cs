using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class DifficultyDeltas
	{
		[ProtoMember(1)]
		public int Easy;

		[ProtoMember(2)]
		public int Challenging;

		[ProtoMember(3)]
		public int Difficult;
	}
}
