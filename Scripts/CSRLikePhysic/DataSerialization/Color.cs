using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public struct Color
	{
		[NonSerialized]
		public static readonly Color clear = default(Color);

		[NonSerialized]
		public static readonly Color magenta = new Color(1f, 0f, 1f, 1f);

		[ProtoMember(1, IsRequired = true)]
		public float r;

		[ProtoMember(2, IsRequired = true)]
		public float g;

		[ProtoMember(3, IsRequired = true)]
		public float b;

		[ProtoMember(4, IsRequired = true)]
		public float a;

		public Color(float r, float g, float b, float a = 1f)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
	}
}
