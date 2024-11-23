using System;

public class GamecenterAvatarLoader : BaseAvatarLoader
{
	public GamecenterAvatarLoader() : base("gc")
	{
		GameCenterManager.playerPhotoLoaded += new GameCenter2StringEventHandler(this.GameCenterPlayerPhotoLoaded);
		GameCenterManager.playerPhotoFailed += new GameCenter2StringEventHandler(this.GameCenterPlayerPhotoFailed);
	}

	protected override void DoRequestProfilePictureFromUserID(string userID)
	{
		if (!GameCenterController.Instance.isPlayerLoggedInAndGameCenterPicsAvailable())
		{
			base.RequestComplete(userID, null);
			return;
		}
		string[] playerIdArray = new string[]
		{
			userID
		};
		GameCenterBinding.loadPlayerData(playerIdArray);
	}

	private void GameCenterPlayerPhotoLoaded(string userID, string filename)
	{
		base.RequestComplete(userID, base.LoadAvatarFromCache(userID));
	}

	private void GameCenterPlayerPhotoFailed(string userID, string filename)
	{
		base.RequestComplete(userID, null);
	}
}
