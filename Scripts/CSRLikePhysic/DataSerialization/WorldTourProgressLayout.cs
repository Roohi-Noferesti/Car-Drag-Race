using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class WorldTourProgressLayout
	{
		[ProtoMember(1)]
		public string ProgressText;

		[ProtoMember(2)]
		public string CrewLeaderName;

		[ProtoMember(3)]
		public string CrewLeaderEvent;

		[ProtoMember(4)]
		public List<CrewMemberLayout> CrewMembers;
	}
}
