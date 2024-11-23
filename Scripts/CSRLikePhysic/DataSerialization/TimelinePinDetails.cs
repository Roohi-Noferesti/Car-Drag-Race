using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class TimelinePinDetails
	{
		[ProtoMember(1)]
		public Vector2 Position;

		[ProtoMember(2, IsRequired = true)]
		public float Alpha = 1f;

		[ProtoMember(3)]
		public float Greyness;

		[ProtoMember(4)]
		public Vector2 Scale = Vector2.one;
	}
}
