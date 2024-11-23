using System;
using System.Collections.Generic;

public class FriendPersona : PersonaComponent
{
	private string _numberPlate = string.Empty;

	private List<Badge> _badges = new List<Badge>();

	public List<string> SocialIDs
	{
		get;
		set;
	}

	public override string GetDisplayName()
	{
		return SocialFriendsManager.Instance.GetFriendName(this.SocialIDs);
	}

	public override string GetNumberPlate()
	{
		return this._numberPlate;
	}

	protected override void RequestAvatar()
	{
		SocialFriendsManager.Instance.SetupFriendAvatar(this.SocialIDs, this);
	}

	public override List<Badge> GetBadges()
	{
		return this._badges;
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("np", out this._numberPlate);
		this._numberPlate = NumberPlate.SafeString(this._numberPlate);
		List<string> list;
		jsonDict.TryGetValue("bdg", out list);
		list = (list ?? new List<string>());
		this._badges = GameDatabase.Instance.Badges.GetBadgesWithIDs(list);
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
	}
}
