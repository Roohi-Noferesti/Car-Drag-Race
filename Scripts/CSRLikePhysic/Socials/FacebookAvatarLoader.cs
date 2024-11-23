using System;
using System.Collections;

public class FacebookAvatarLoader : BaseAvatarLoader
{
	public class fbPicInfo
	{
		public bool isSilhouette = true;

		public string url = string.Empty;
	}

	public FacebookAvatarLoader() : base("fb")
	{
		NativeEvents.fbGotFriendProfilePicEvent += new NativeEvents_DelegateString2(this.fbGotFriendProfilePic);
		NativeEvents.fbGotFriendProfilePicFailedEvent += new NativeEvents_DelegateString(this.fbGotFriendProfilePicFailed);
	}

	protected override void DoRequestProfilePictureFromUserID(string userID)
	{
		if (!SocialController.Instance.isLoggedIntoFacebook)
		{
			base.RequestComplete(userID, null);
		}
		BasePlatform.ActivePlatform.GetFacebookFriendProfilePic(userID);
	}

	public void fbGotFriendProfilePic(string userID, string friendPicJSON)
	{
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(friendPicJSON))
		{
			base.RequestComplete(userID, null);
			return;
		}
		FacebookAvatarLoader.fbPicInfo fbPicInfo;
		if (!jsonDict.TryGetObject<FacebookAvatarLoader.fbPicInfo>("picture", out fbPicInfo, new GetObjectDelegate<FacebookAvatarLoader.fbPicInfo>(FacebookAvatarLoader.GetPicData)))
		{
			base.RequestComplete(userID, null);
			return;
		}
		if (fbPicInfo.isSilhouette)
		{
			base.RequestComplete(userID, null);
			return;
		}
		this.StartPicDownloadCoroutine(fbPicInfo.url, userID);
	}

	private static void GetPicData(JsonDict jsonDict, ref FacebookAvatarLoader.fbPicInfo pic)
	{
		jsonDict.TryGetObject<FacebookAvatarLoader.fbPicInfo>("data", out pic, new GetObjectDelegate<FacebookAvatarLoader.fbPicInfo>(FacebookAvatarLoader.GetPicInfo));
	}

	private static void GetPicInfo(JsonDict jsonDict, ref FacebookAvatarLoader.fbPicInfo pic)
	{
		jsonDict.TryGetValue("is_silhouette", out pic.isSilhouette);
		jsonDict.TryGetValue("url", out pic.url);
	}

	private void StartPicDownloadCoroutine(string url, string userID)
	{
		string filename = string.Format("{0}_large_fb.png", userID);
		SocialController.Instance.StartCoroutine(this.PicDownloadCoroutine(url, filename, userID));
	}

	private IEnumerator PicDownloadCoroutine(string url, string filename, string userID)
	{
	    //FacebookAvatarLoader.<PicDownloadCoroutine>c__Iterator1E <PicDownloadCoroutine>c__Iterator1E = new FacebookAvatarLoader.<PicDownloadCoroutine>c__Iterator1E();
        //<PicDownloadCoroutine>c__Iterator1E.url = url;
        //<PicDownloadCoroutine>c__Iterator1E.userID = userID;
        //<PicDownloadCoroutine>c__Iterator1E.<$>url = url;
        //<PicDownloadCoroutine>c__Iterator1E.<$>userID = userID;
        //<PicDownloadCoroutine>c__Iterator1E.<>f__this = this;
        //return <PicDownloadCoroutine>c__Iterator1E;
	    return null;
	}

    public void fbGotFriendProfilePicFailed(string userID)
	{
		base.RequestComplete(userID, null);
	}

	public void RequestProfilePictureFromURL(string userID, string URL, PersonaComponent persona)
	{
		base.CheckCacheOrDoRequest(userID, persona, delegate
		{
			this.StartPicDownloadCoroutine(URL, userID);
		});
	}
}
