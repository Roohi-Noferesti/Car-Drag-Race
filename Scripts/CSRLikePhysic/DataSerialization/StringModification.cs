using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class StringModification
	{
		[ProtoContract]
		[Serializable]
		public class Details
		{
			[ProtoMember(1)]
			public string Default;

			[ProtoMember(2)]
			public string StringValue;

			[ProtoMember(3)]
			public string[] StringValues;

			[ProtoMember(4)]
			public string ThemeID;

			[ProtoMember(5, IsRequired = true)]
			public bool Translate = true;

			[ProtoMember(6, IsRequired = true)]
			public int Offset;

			[ProtoMember(7)]
			public string[] DefaultModifications;
		}

		[ProtoMember(1)]
		public string Type;

		[ProtoMember(2)]
		public Details ModificationDetails;
	}
}
