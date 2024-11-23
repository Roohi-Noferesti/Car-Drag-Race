using System.Collections.Generic;
using UnityEngine;

public abstract class PersonaComponent : PlayerInfoComponent
{
	private List<AvatarPicture> avatarPictures = new List<AvatarPicture>();

	private Texture2D cachedAvatarTexture;

	private bool waitingOnCallback;

	public virtual string GetNumberPlate()
	{
		return "UNKNOWN";
	}

	public virtual string GetDisplayName()
	{
		return "UNKNOWN";
	}

    public virtual List<Badge> GetBadges()
    {
        return new List<Badge>();
    }

	public void SetupAvatarPicture(AvatarPicture picture)
	{
		if (this.cachedAvatarTexture != null)
		{
            picture.SetAvatarTexture(this.cachedAvatarTexture, this.GetBadges());
		}
		this.avatarPictures.Add(picture);
		if (!this.waitingOnCallback)
		{
			this.waitingOnCallback = true;
			this.RequestAvatar();
		}
	}

	protected virtual void RequestAvatar()
	{
		this.LoadDefaultCsrAvatarFromResources();
	}

	public virtual void OnAvatarLoaded(Texture2D texture)
	{
		this.waitingOnCallback = false;
		this.cachedAvatarTexture = texture;
		foreach (AvatarPicture current in this.avatarPictures)
		{
			if (!(current == null))
			{
                current.SetAvatarTexture(texture, this.GetBadges());
			}
		}
		this.avatarPictures.Clear();
	}

	public virtual void OnAvatarLoadFailed()
	{
		this.LoadDefaultCsrAvatarFromResources();
	}

	public void LoadDefaultCsrAvatarFromResources()
	{
		this.LoadCsrAvatarFromResources(AvatarPicture.FallbackCSRAvatar);
	}

	protected void LoadCsrAvatarFromResources(int csrAvatarNumber)
	{
		csrAvatarNumber = Mathf.Clamp(csrAvatarNumber, 0, AvatarPicture.NumberOfCSRAvatars - 1);
		string avatarName = "CSRAvatar" + csrAvatarNumber.ToString();
		this.LoadCsrAvatarFromResources(avatarName);
	}

	protected void LoadCsrAvatarFromResources(string avatarName)
	{
		Texture2D texture = Resources.Load("CSRAvatars/" + avatarName) as Texture2D;
		this.OnAvatarLoaded(texture);
	}

	public void LoadFacebookAvatarFromCacheOrUserID(string fbUserID)
	{
        SocialController.Instance.FacebookAvatarLoader.RequestProfilePictureFromUserID(fbUserID, this);
	}

	public void LoadFacebookAvatarFromCacheOrURL(string fbUserID, string URL)
	{
        SocialController.Instance.FacebookAvatarLoader.RequestProfilePictureFromURL(fbUserID, URL, this);
	}

	public void LoadGamecenterAvatarFromCacheOrUserID(string gcUserID)
	{
        SocialController.Instance.GamecenterAvatarLoader.RequestProfilePictureFromUserID(gcUserID, this);
	}

	public void LoadGooglePlayAvatarFromCacheOrUserID(string gpUserID)
	{
        SocialController.Instance.GooglePlayAvatarLoader.RequestProfilePictureFromUserID(gpUserID, this);
	}
}
