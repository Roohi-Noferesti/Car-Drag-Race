using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ThemeOptionLayoutDetails
	{
		[ProtoMember(1)]
		public Color Colour = new Color(0.25f, 0.6f, 0.75f, 1f);

		[ProtoMember(2)]
		public string Background = string.Empty;

		[ProtoMember(3)]
		public string Title = string.Empty;

		[ProtoMember(4)]
		public ConditionallySelectedString ProgressionText;

		[ProtoMember(5)]
		public ConditionallySelectedString EventNameText;

		[ProtoMember(6)]
		public ConditionallySelectedSnapshotList ProgressionSnapshots;

		[ProtoMember(7)]
		public bool UseBackgroundGlow;
	}
}
