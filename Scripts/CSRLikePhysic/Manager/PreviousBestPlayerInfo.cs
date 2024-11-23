using System;

public class PreviousBestPlayerInfo : PlayerInfo
{
	public PreviousBestPlayerInfo() : base(new PreviousBestPersona())
	{
		base.AddComponent<RWFPlayerInfoComponent>();
		base.AddComponent<RacePlayerInfoComponent>();
		base.AddComponent<MechanicPlayerInfoComponent>();
	}
}
