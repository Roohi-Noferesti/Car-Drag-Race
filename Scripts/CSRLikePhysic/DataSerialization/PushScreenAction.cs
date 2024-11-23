using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PushScreenAction
	{
		[ProtoMember(1)]
		public string ScreenID;
	}
}
