using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public struct Vector3
	{
		[NonSerialized]
		public static readonly Vector3 one = new Vector3(1f, 1f, 1f);

		[ProtoMember(1, IsRequired = true)]
		public float x;

		[ProtoMember(2, IsRequired = true)]
		public float y;

		[ProtoMember(3, IsRequired = true)]
		public float z;

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}
