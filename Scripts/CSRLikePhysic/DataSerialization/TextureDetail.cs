using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class TextureDetail
	{
		[ProtoMember(1)]
		public string Name = string.Empty;

		[ProtoMember(2)]
		public Vector2 Offset;

		[ProtoMember(3)]
		public Vector2 Scale = Vector2.one;

		[ProtoMember(4)]
		public bool AppendThemeChoiceToName;
	}
}
