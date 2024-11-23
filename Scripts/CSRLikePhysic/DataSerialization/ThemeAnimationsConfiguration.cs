using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ThemeAnimationsConfiguration
	{
		[ProtoMember(1)]
		public List<ThemeAnimationDetail> Animations = new List<ThemeAnimationDetail>();
	}
}
