using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public struct Vector2
	{
		[NonSerialized]
		public static readonly Vector2 one = new Vector2(1f, 1f);

		[ProtoMember(1, IsRequired = true)]
		public float x;

		[ProtoMember(2, IsRequired = true)]
		public float y;

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
