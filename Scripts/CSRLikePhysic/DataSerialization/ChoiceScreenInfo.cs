using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ChoiceScreenInfo
	{
		[ProtoMember(1)]
		public string SequenceID = string.Empty;

		[ProtoMember(2)]
		public string Title = string.Empty;

		[ProtoMember(3)]
		public Color Colour = Color.clear;
	}
}
