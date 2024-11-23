using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class BubbleMessageData
	{
		[ProtoMember(1)]
		public string Text;

		[ProtoMember(2, IsRequired = true)]
		public float YOffset = -0.2f;

		[ProtoMember(3, IsRequired = true)]
		public float NipplePos = 0.5f;

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"BubbleMessage: ",
				this.Text,
				", ",
				this.YOffset,
				", ",
				this.NipplePos
			});
		}
	}
}
