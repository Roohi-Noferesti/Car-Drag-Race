using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	public class ConditionallySelectedSnapshotList
	{
		[ProtoMember(1)]
		public List<ConditionallySelectedSnapshots> Lists = new List<ConditionallySelectedSnapshots>();
	}
}
