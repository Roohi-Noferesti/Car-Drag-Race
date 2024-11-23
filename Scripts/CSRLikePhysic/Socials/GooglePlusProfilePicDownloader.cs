using System;
using UnityEngine;

public class GooglePlusProfilePicDownloader : MonoBehaviour
{
	private class GooglePlusImage
	{
		public string url;
	}

	private WWW downloader;

	private string usrID;

	private string path;

	private float lastDownloadPercentage = -1f;

	private bool gotGooglePlusProfile;

	private static void GetPicData(JsonDict jsonDict, ref GooglePlusProfilePicDownloader.GooglePlusImage pic)
	{
		jsonDict.TryGetValue("url", out pic.url);
	}

	public void GetPlayerAvatar(string[] userIDAndFilename)
	{
		this.usrID = userIDAndFilename[0];
		this.path = userIDAndFilename[1];
		string url = string.Format("https://www.googleapis.com/plus/v1/people/{0}?key=AIzaSyCeIiZ2Yr_r5Ma0-k6iBHHhZHn5bBqx2qM", this.usrID);
		this.downloader = new WWW(url);
	}

	private void Update()
	{
		if (this.downloader != null)
		{
			if (this.downloader.error != null)
			{
#if UNITY_ANDROID
                GooglePlayGamesController.PlayerPhotoLoadError(this.usrID, this.downloader.error);
#endif
				this.Cleanup();
			}
			else if (this.downloader.isDone)
			{
				if (!this.gotGooglePlusProfile)
				{
					string text = this.downloader.text;
					JsonDict jsonDict = new JsonDict();
					GooglePlusProfilePicDownloader.GooglePlusImage googlePlusImage;
					if (jsonDict.Read(text) && jsonDict.TryGetObject<GooglePlusProfilePicDownloader.GooglePlusImage>("image", out googlePlusImage, new GetObjectDelegate<GooglePlusProfilePicDownloader.GooglePlusImage>(GooglePlusProfilePicDownloader.GetPicData)))
					{
						googlePlusImage.url = googlePlusImage.url.Split(new char[]
						{
							'?'
						}, StringSplitOptions.None)[0] + "?sz=200";
						this.gotGooglePlusProfile = true;
						this.downloader.Dispose();
						this.downloader = new WWW(googlePlusImage.url);
					}
					if (!this.gotGooglePlusProfile)
					{
#if UNITY_ANDROID
						GooglePlayGamesController.PlayerPhotoLoadError(this.usrID, this.downloader.error);
#endif
						this.Cleanup();
					}
				}
				else
				{
					Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
					this.downloader.LoadImageIntoTexture(texture2D);
					SocialController.Instance.GooglePlayAvatarLoader.WritePicToCache(texture2D, this.usrID);
#if UNITY_ANDROID
					GooglePlayGamesController.PlayerPhotoLoadComplete(this.usrID, this.path);
#endif
					this.Cleanup();
				}
			}
			else if (this.downloader.progress != this.lastDownloadPercentage)
			{
				this.lastDownloadPercentage = this.downloader.progress;
			}
		}
	}

	private void Cleanup()
	{
		this.lastDownloadPercentage = -1f;
		this.gotGooglePlusProfile = false;
		if (this.downloader != null)
		{
			this.downloader.Dispose();
			this.downloader = null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
