using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class FormatStringParameterData
	{
		[ProtoMember(1)]
		public string Value = string.Empty;

		[ProtoMember(2)]
		public string DecorationString = "NONE";
	}
}
