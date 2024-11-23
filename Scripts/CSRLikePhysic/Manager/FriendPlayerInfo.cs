using System;

public class FriendPlayerInfo : PlayerInfo
{
	public FriendPlayerInfo(CachedFriendRaceData lump) : base(new FriendPersona
	{
		SocialIDs = lump.ServiceID
	})
	{
		base.AddComponent<RWFPlayerInfoComponent>();
		base.AddComponent<RacePlayerInfoComponent>();
		base.AddComponent<MechanicPlayerInfoComponent>();
		this._csrUserName = NameValidater.CreateIdUsername(lump.UserID);
	}
}
