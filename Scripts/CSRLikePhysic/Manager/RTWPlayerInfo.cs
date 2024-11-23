public class RTWPlayerInfo : PlayerInfo
{
	public RTWPlayerInfo() : base(new RTWPersona())
	{
		base.AddComponent<StatsPlayerInfoComponent>();
		base.AddComponent<RTWPlayerInfoComponent>();
		base.AddComponent<RacePlayerInfoComponent>();
		base.AddComponent<ConsumablePlayerInfoComponent>();
	}
}
