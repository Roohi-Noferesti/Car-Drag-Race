using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class RestrictionRaceHelperOverride
	{
		[ProtoMember(1)]
		public int EventID;

		[ProtoMember(2)]
		public string BundledGraphicPath;

		[ProtoMember(3)]
		public bool IsCrewLeader;

		[ProtoMember(4)]
		public string BodyText;

		[ProtoMember(5)]
		public string ImageCaption;
	}
}
