using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
    [Serializable]
	public class CrewMemberLayout
	{
		[ProtoMember(1)]
		public string Texture;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public string Event;

		[ProtoMember(4)]
		public string ScheduleID;
	}
}
