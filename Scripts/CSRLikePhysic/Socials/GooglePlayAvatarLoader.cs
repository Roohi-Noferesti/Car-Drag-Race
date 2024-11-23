using System;
using UnityEngine;

public class GooglePlayAvatarLoader : BaseAvatarLoader
{
	public GooglePlayAvatarLoader() : base("gp")
	{
#if UNITY_ANDROID
	            GooglePlayGamesController.PlayerPhotoLoaded += new GameCenter2StringEventHandler(this.GooglePlayPhotoLoaded);
		GooglePlayGamesController.PlayerPhotoLoadFailed += new GameCenter2StringEventHandler(this.GooglePlayPhotoFailed);
#endif

    }

    protected override void DoRequestProfilePictureFromUserID(string userID)
	{
		string cachePicPath = base.GetCachePicPath(userID);
		GameObject gameObject = new GameObject("G+ProfilePicDownloader");
		GooglePlusProfilePicDownloader googlePlusProfilePicDownloader = gameObject.AddComponent<GooglePlusProfilePicDownloader>();
		googlePlusProfilePicDownloader.SendMessage("GetPlayerAvatar", new string[]
		{
			userID,
			cachePicPath
		});
	}

	private void GooglePlayPhotoLoaded(string userID, string filename)
	{
		base.RequestComplete(userID, base.LoadAvatarFromCache(userID));
	}

	private void GooglePlayPhotoFailed(string userID, string filename)
	{
		base.RequestComplete(userID, null);
	}
}
