using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ProgressionVisualisation
	{
		[ProtoMember(1)]
		public string ViewStyleString = "None";

		[ProtoMember(2)]
		public string Accumulator = "Zero";

		[ProtoMember(3)]
		public List<string> SequenceIDs;

		[ProtoMember(4)]
		public string ThemeID;

	    public ProgressBarStyle GetViewStyleAsEnum()
	    {
	        return EnumHelper.FromString<ProgressBarStyle>(ViewStyleString);
	    }
    }
}
