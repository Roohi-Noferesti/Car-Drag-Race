using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
    [Serializable]
	public class WorldTourBossPinPieceDetails
	{
		[ProtoMember(1)]
		public Vector2 StartPosition;

		[ProtoMember(2)]
		public Vector2 EndPosition;

		[ProtoMember(3)]
		public string SequenceID;

		[ProtoIgnore]
		public object EventPin;
	}
}
