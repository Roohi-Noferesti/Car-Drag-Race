using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class WorldTourBossPinDetais
	{
		[ProtoMember(1)]
		public string PieceTexture;

		[ProtoMember(2)]
		public WorldTourBossPinPieceDetails[] PinDetails;
	}
}
