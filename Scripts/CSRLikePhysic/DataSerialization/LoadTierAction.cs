using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class LoadTierAction
	{
		[ProtoMember(1)]
		public string eventPaneTitle;

		[ProtoMember(2)]
		public string eventPaneDesc;

		[ProtoMember(3)]
		public string eventPaneSprite;

		[ProtoMember(4)]
		public string themeToLoad;

		[ProtoMember(5)]
		public string themeOption;

		[ProtoMember(6)]
		public List<PopupData> entryPopups = new List<PopupData>();
	}
}
