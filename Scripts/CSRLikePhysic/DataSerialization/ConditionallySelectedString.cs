using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
    [Serializable]
	public class ConditionallySelectedString
	{
		[ProtoMember(1)]
		public List<ConditionallyModifiedString> Strings = new List<ConditionallyModifiedString>();

		public static ConditionallySelectedString CreateEmpty()
		{
			return new ConditionallySelectedString();
		}
	}
}
