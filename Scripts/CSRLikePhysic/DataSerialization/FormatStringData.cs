using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class FormatStringData
	{
		[ProtoMember(1)]
		public string StringFormatBase = string.Empty;

		[ProtoMember(2)]
		public bool AlreadyTranslated;

		[ProtoMember(3)]
		public List<FormatStringParameterData> StringFormatParameters = new List<FormatStringParameterData>();
	}
}
