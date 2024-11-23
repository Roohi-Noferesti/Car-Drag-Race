using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPicture : MonoBehaviour
{
	public enum eAvatarType
	{
		CSR_AVATAR,
		FACEBOOK_AVATAR,
		GAME_CENTER_AVATAR,
		GOOGLE_PLAY_GAMES_AVATAR,
		NO_AVATAR,
		NUM_AVATAR_TYPES
	}

	public Image Picture;

    public Image LoadingAnimation;

	public float AvatarSize = 0.52f;

	public bool ShowBadges = true;

    private List<Image> badgeTextures;

    private Texture2D avatarTexture;

    public event OnAvatarLoaded onAvatarLoaded;

	public static int NumberOfCSRAvatars
	{
		get
		{
			return 16;
		}
	}

	public static int FallbackCSRAvatar
	{
		get
		{
			return 3;
		}
	}

    public void SetAvatarTexture(Texture2D texture, List<Badge> badges)
    {
        this.avatarTexture = texture;
        this.OnAvatarTextureLoaded(badges);
    }

    private void OnAvatarTextureLoaded(List<Badge> badges)
    {
        if (this.onAvatarLoaded != null)
        {
            this.onAvatarLoaded();
        }
        if (this.avatarTexture == null)
        {
            this.ShowLoadFailedIcon();
            return;
        }
        this.ShowLoadingAnimation(false);
        this.ApplyAvatarTexture(badges);
    }

    private void ApplyAvatarTexture(List<Badge> badges)
    {
        float num = (float)this.avatarTexture.width;
        float num2 = (float)this.avatarTexture.height;
        if (num > num2)
        {
            num2 *= this.AvatarSize / num;
            num = this.AvatarSize;
        }
        else if (num < num2)
        {
            num *= this.AvatarSize / num2;
            num2 = this.AvatarSize;
        }
        else
        {
            num = this.AvatarSize;
            num2 = this.AvatarSize;
        }
        //this.Picture.Setup(num, num2, new Vector2(0f, (float)(this.avatarTexture.height - 1)), new Vector2((float)this.avatarTexture.width, (float)this.avatarTexture.height));
        this.avatarTexture.wrapMode = TextureWrapMode.Clamp;
        if (this.ShowBadges)
        {
            badges.ForEach(new Action<Badge>(this.CreateBadgesAndAddToSelf));
        }
        //this.Picture.SetTexture(this.avatarTexture);
        //EzGUIHelper.InitSprite(this.Picture);
    }

    private void CreateBadgesAndAddToSelf(Badge badge)
    {
        //GameObject gameObject = new GameObject(badge.TextureName);
        //Image spr = gameObject.AddComponent<Image>();
        //spr.gameObject.transform.parent = this.Picture.gameObject.transform;
        //spr.gameObject.transform.localPosition = this.ConvertUnitOffsetToAvatarSpace(badge.AvatarPortraitOffset);
        //spr.gameObject.transform.localScale = Vector3.one;
        //spr.gameObject.layer = LayerMask.NameToLayer("GUI");
        //spr.renderer.material = this.Picture.renderer.material;
        //float avatarScale = this.Picture.width * 0.6f;
        //TexturePack.RequestTextureFromBundle("BadgesTexturePack", badge.TextureName, delegate(Texture2D texture)
        //{
        //    spr.renderer.material.SetTexture("_MainTex", texture);
        //    spr.Setup(avatarScale, avatarScale, new Vector2(0f, (float)texture.height - 1f), new Vector2((float)texture.width, (float)texture.height));
        //});
    }

    private Vector3 ConvertUnitOffsetToAvatarSpace(Vector3 vec)
    {
        //float num = this.Picture.width * 0.5f;
        //float num2 = this.Picture.height * 0.5f;
        //return new Vector3(num * vec.x, num2 * vec.y, vec.z);
        return new Vector3();
    }

    private void ShowLoadFailedIcon()
	{
		this.Picture.gameObject.SetActive(false);
		this.LoadingAnimation.gameObject.SetActive(false);
	}

	private void ShowLoadingAnimation(bool show)
	{
		if (show)
		{
			//float num = 0.2f;
			//float num2 = this.AvatarSize * num;
			this.LoadingAnimation.gameObject.SetActive(true);
            //this.LoadingAnimation.Setup(num2, num2);
			this.Picture.gameObject.SetActive(false);
		}
		else
		{
			this.LoadingAnimation.gameObject.SetActive(false);
			this.Picture.gameObject.SetActive(true);
		}
	}

	private void Start()
	{
        this.ShowLoadingAnimation(this.avatarTexture == null);
	}
}
