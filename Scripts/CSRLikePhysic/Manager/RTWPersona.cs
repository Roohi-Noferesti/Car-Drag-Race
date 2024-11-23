using System.Collections.Generic;

public class RTWPersona : PersonaComponent
{
	public AvatarPicture.eAvatarType _avatarType;

	public string _facebookID;

	public string _gamecenterID;

	public string _googleplayID;

	public int _preferredCSRAvatar = 1;

	public int _preferredName;

    //public List<Badge> _badges = new List<Badge>();

	public string DisplayName
	{
		get;
		set;
	}

	public override string GetDisplayName()
	{
		return this.DisplayName;
	}

	public override string GetNumberPlate()
	{
		AIDriverData aIDriverData = RaceEventInfo.Instance.AIDriverData;
		return GameDatabase.Instance.AIPlayers.GetDriverNumberPlateString(aIDriverData);
	}

	protected override void RequestAvatar()
	{
		switch (this._avatarType)
		{
		case AvatarPicture.eAvatarType.CSR_AVATAR:
			base.LoadCsrAvatarFromResources(this._preferredCSRAvatar);
			break;
		case AvatarPicture.eAvatarType.FACEBOOK_AVATAR:
			base.LoadFacebookAvatarFromCacheOrUserID(this._facebookID);
			break;
		case AvatarPicture.eAvatarType.GAME_CENTER_AVATAR:
			base.LoadGamecenterAvatarFromCacheOrUserID(this._gamecenterID);
			break;
		case AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR:
			base.LoadGooglePlayAvatarFromCacheOrUserID(this._googleplayID);
			break;
		default:
			base.LoadDefaultCsrAvatarFromResources();
			break;
		}
	}

    //public override List<Badge> GetBadges()
    //{
    //    return this._badges;
    //}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		jsonDict.Set("dn", this.DisplayName);
		jsonDict.Set("fb", this._facebookID);
		jsonDict.Set("gc", this._gamecenterID);
		jsonDict.Set("gpg", this._googleplayID);
		jsonDict.Set("at", (int)this._avatarType);
		jsonDict.Set("av", this._preferredCSRAvatar);
        //jsonDict.Set("bdg", (from b in this._badges
        //select b.ID).ToList<string>());
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		string displayName;
		jsonDict.TryGetValue("dn", out displayName);
		this.DisplayName = displayName;
		jsonDict.TryGetValue("fb", out this._facebookID);
		jsonDict.TryGetValue("gc", out this._gamecenterID);
		jsonDict.TryGetValue("gpg", out this._googleplayID);
		int avatarType;
		jsonDict.TryGetValue("at", out avatarType);
		this._avatarType = (AvatarPicture.eAvatarType)avatarType;
		jsonDict.TryGetValue("av", out this._preferredCSRAvatar);
		List<string> list;
		jsonDict.TryGetValue("bdg", out list);
		list = (list ?? new List<string>());
        //this._badges = GameDatabase.Instance.Badges.GetBadgesWithIDs(list);
	}
}
