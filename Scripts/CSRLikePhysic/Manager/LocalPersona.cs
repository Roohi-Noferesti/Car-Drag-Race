public class LocalPersona : PersonaComponent
{
	public override string GetDisplayName()
	{
		return PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback();
	}

	public override string GetNumberPlate()
	{
		return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().NumberPlate.Text;
	}

	protected override void RequestAvatar()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		switch (activeProfile.GetAvatarTypeToDisplayForDefault())
		{
		case AvatarPicture.eAvatarType.CSR_AVATAR:
			base.LoadCsrAvatarFromResources(activeProfile.PreferredCsrAvatarPicture);
			break;
		case AvatarPicture.eAvatarType.FACEBOOK_AVATAR:
            //base.LoadFacebookAvatarFromCacheOrURL(this.getSerialisableFacebookID(), SocialController.Instance.GetFacebookPictureURL());
			break;
		case AvatarPicture.eAvatarType.GAME_CENTER_AVATAR:
			base.LoadGamecenterAvatarFromCacheOrUserID(this.getSerialisableGameCenterID());
			break;
		case AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR:
            //base.LoadGooglePlayAvatarFromCacheOrUserID(GooglePlayGamesController.Instance.GetPlayerID());
			break;
		default:
			base.LoadDefaultCsrAvatarFromResources();
			break;
		}
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		jsonDict.Set("dn", activeProfile.DisplayNameWithUserNameFallback());
		jsonDict.Set("fb", this.getSerialisableFacebookID());
		jsonDict.Set("gc", this.getSerialisableGameCenterID());
		jsonDict.Set("gpg", this.getSerialisableGooglePlayID());
		jsonDict.Set("at", (int)activeProfile.GetAvatarTypeToDisplayForDefault());
		jsonDict.Set("av", activeProfile.PreferredCsrAvatarPicture);
		jsonDict.Set("np", this.GetNumberPlate());
        //jsonDict.Set("bdg", (from b in this.GetBadges()
        //select b.ID).ToList<string>());
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
	}

    //public override List<Badge> GetBadges()
    //{
    //    return GameDatabase.Instance.Badges.GetObtainedBadges(new GameStateFacade());
    //}

	private string getSerialisableFacebookID()
	{
        //if (SocialController.Instance.isLoggedIntoFacebook)
        //{
        //    return SocialController.Instance.GetFacebookID();
        //}
		return string.Empty;
	}

	private string getSerialisableGameCenterID()
	{
        //if (GameCenterController.Instance.isPlayerLoggedIn())
        //{
        //    return GameCenterController.Instance.currentID();
        //}
		return string.Empty;
	}

	private string getSerialisableGooglePlayID()
	{
        //if (GooglePlayGamesController.Instance.IsPlayerAuthenticated())
        //{
        //    return GooglePlayGamesController.Instance.GetPlayerID();
        //}
		return string.Empty;
	}
}
