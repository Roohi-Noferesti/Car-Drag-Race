using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicMenuListItem : GenericListItem, IBundleOwner
{
	public const float _fadeTime = 0.5f;

	public Image Overlay;

    public Image LoadingOverlay;

	private int _id = -1;

	public TextMeshProUGUI text;

    public TextMeshProUGUI iconText;

	private bool _showLoadingOverlay;

	private bool _showNormalOverlay;

	private float _opacityTimer;

	private float _lastOpacity = 3.40282347E+38f;

	public GameObject Notification;

	private Texture2D normalTex;

	private bool _needToFreeAssetRequest;

	public Color MultiplayerTextColor;

	public Color CareerTextColor;

	//private bool InversionDisabled;

	public int Index
	{
		get
		{
			return this._id;
		}
	}

	public override bool Ready
	{
		get
		{
			return this._showNormalOverlay;
		}
	}

	public bool NotificationActive
	{
		set
		{
			this.Notification.gameObject.SetActive(value);
		}
	}

	private float Inversion
	{
		set
		{
            //if (this.InversionDisabled)
            //{
            //    this.Overlay.renderer.material.SetFloat("_Invert", 0f);
            //}
            //else
            //{
            //    this.Overlay.renderer.material.SetFloat("_Invert", value);
            //}
		}
	}

	private Color CurrentTextColor
	{
		get
		{
			return (!MultiplayerUtils.GarageInMultiplayerMode) ? this.CareerTextColor : this.MultiplayerTextColor;
		}
	}

	private Color DisabledTextColor
	{
		get
		{
			Color currentTextColor = this.CurrentTextColor;
			currentTextColor.a = 0.25f;
			return currentTextColor;
		}
	}

	public CarouselTransition CarouselTransition
	{
		get
		{
			return base.GetComponent<CarouselTransition>();
		}
	}

	public void UpdateTransition(bool toCareer, float interp)
	{
		if (toCareer)
		{
			this.Inversion = 1f - interp;
            //this.text.SetColor(Color.Lerp(this.MultiplayerTextColor, this.CareerTextColor, interp));
		}
		else
		{
			this.Inversion = interp;
            //this.text.SetColor(Color.Lerp(this.CareerTextColor, this.MultiplayerTextColor, interp));
		}
	}

	public void DisableOverlayInversion()
	{
		//this.InversionDisabled = true;
		this.Inversion = 0f;
	}

	public override void Shutdown()
	{
		if (AssetProviderClient.Instance != null && this._needToFreeAssetRequest)
		{
			AssetProviderClient.Instance.ReleaseRequestsForOwner(this);
			this._needToFreeAssetRequest = false;
		}
		this.iconText.gameObject.SetActive(false);
		Resources.UnloadAsset(this.normalTex);
		this.normalTex = null;
		base.Shutdown();
	}

	protected override void Show()
	{
		this.SetTheUnityActiveStateOfTheOverlays(false);
		this.text.gameObject.SetActive(true);
		base.Show();
	}

	protected override void Hide()
	{
		this.Overlay.gameObject.SetActive(false);
		this.LoadingOverlay.gameObject.SetActive(false);
		this.text.gameObject.SetActive(false);
		base.Hide();
	}

	protected override void OnDestroy()
	{
		this.Overlay = null;
		this.LoadingOverlay = null;
		this.normalTex = null;
		base.OnDestroy();
	}

	public override void GreyOutThisItem(bool zVal)
	{
		if (zVal != base.IsThisGreyedOut())
		{
			this._opacityTimer = 0.5f;
            //this.text.SetColor((!zVal) ? this.CurrentTextColor : this.DisabledTextColor);
		}
		base.GreyOutThisItem(zVal);
	}

	private void SetTheUnityActiveStateOfTheOverlays(bool zDoFade)
	{
		if (this.Overlay == null)
		{
			return;
		}
		if (this.LoadingOverlay == null)
		{
			return;
		}
		if (zDoFade)
		{
			this._opacityTimer = 0.5f;
		}
		this.Overlay.gameObject.SetActive(this._showNormalOverlay);
		this.LoadingOverlay.gameObject.SetActive(this._showLoadingOverlay);
	}

	public void Create(int zIndex, string zText, Texture2D zOverlayTex, float zItemWidth, bool zRoundLeft, bool zRoundRight, bool ShouldToUpper)
	{
        //this.text.SetColor(this.CurrentTextColor);
		this._needToFreeAssetRequest = false;
		this._opacityTimer = 0f;
		string text = LocalizationManager.GetTranslation(zText);
		if (ShouldToUpper)
		{
			this.text.text = text.ToUpper();
		}
		else
		{
            this.text.text = text;
		}
		this._id = zIndex;
		base.name = "BasicMenuListItem-" + this._id;
		if (zOverlayTex != null)
		{
            //this.Overlay.gameObject.renderer.material.SetTexture("_MainTex", zOverlayTex);
            //this.Overlay.Setup((float)zOverlayTex.width / 200f, (float)zOverlayTex.height / 200f, new Vector2(0f, (float)(zOverlayTex.height - 1)), new Vector2((float)zOverlayTex.width, (float)zOverlayTex.height));
		}
		base.BaseCreate(zItemWidth, zRoundLeft, zRoundRight);
		this._showNormalOverlay = true;
		this._showLoadingOverlay = false;
		this.SetTheUnityActiveStateOfTheOverlays(false);
		this.SetupOverlayPosition();
		this.SetupNotification(zItemWidth);
		this.CarouselTransition.SetToDefaultMaterial();
		this.SetupOverlayInversion();
	}

	public void Create(int zIndex, string zText, string zNormalAsset, float zItemWidth, bool zRoundLeft, bool zRoundRight, bool ShouldToUpper)
	{
        //this.text.SetColor(this.CurrentTextColor);
		this._needToFreeAssetRequest = false;
		this._opacityTimer = 0f;
		string text = LocalizationManager.GetTranslation(zText);
		if (ShouldToUpper)
		{
			this.text.text = text.ToUpper();
		}
		else
		{
            this.text.text = text;
		}
		this._id = zIndex;
		base.name = "BasicMenuListItem-" + this._id;
		this.normalTex = null;
		AssetProviderClient.Instance.RequestAsset(zNormalAsset, new BundleLoadedDelegate(this.SwipeReady), this);
		this._needToFreeAssetRequest = true;
		base.BaseCreate(zItemWidth, zRoundLeft, zRoundRight);
		this._showLoadingOverlay = false;
		this._showNormalOverlay = false;
		this.SetTheUnityActiveStateOfTheOverlays(false);
		this.SetupOverlayPosition();
		this.SetupNotification(zItemWidth);
		base.StartCoroutine(this.WaitAndEnable(0.2f));
		this.CarouselTransition.SetToDefaultMaterial();
		this.SetupOverlayInversion();
	}

	private void SetupOverlayPosition()
	{
		if (base.name == string.Empty)
		{
            //GameObjectHelper.SetLocalY(this.Overlay.gameObject, 0f);
            //GameObjectHelper.SetLocalY(this.LoadingOverlay.gameObject, 0f);
		}
		else
		{
            //GameObjectHelper.SetLocalY(this.Overlay.gameObject, 0.07f);
            //GameObjectHelper.SetLocalY(this.LoadingOverlay.gameObject, 0.07f);
		}
	}

	private void SetupNotification(float itemWidth)
	{
        //this.Notification.transform.localPosition = new Vector3(itemWidth * 0.35f, this.CentralSprite.height * 0.5f, -0.15f);
		this.Notification.gameObject.SetActive(false);
	}

	private void SetupOverlayInversion()
	{
		//this.InversionDisabled = false;
		this.Inversion = (float)((!MultiplayerUtils.GarageInMultiplayerMode) ? 0 : 1);
	}

	private IEnumerator WaitAndEnable(float waitTime)
	{
	    //BasicMenuListItem.<WaitAndEnable>c__Iterator29 <WaitAndEnable>c__Iterator = new BasicMenuListItem.<WaitAndEnable>c__Iterator29();
        //<WaitAndEnable>c__Iterator.waitTime = waitTime;
        //<WaitAndEnable>c__Iterator.<$>waitTime = waitTime;
        //<WaitAndEnable>c__Iterator.<>f__this = this;
        //return <WaitAndEnable>c__Iterator;
	    return null;
	}

    protected override void AdjustForNewPressedState()
	{
		base.AdjustForNewPressedState();
        //this.Overlay.gameObject.renderer.material.SetFloat("_Fade", (!this.Pressed) ? 0.4f : 0.8f);
	}

	public void SwipeReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		Object[] array = zAssetBundle.LoadAllAssets();
		this.normalTex = (array[0] as Texture2D);
		if (array.Length > 1)
		{
			this.normalTex = (array[1] as Texture2D);
		}
		this._showLoadingOverlay = false;
		this._showNormalOverlay = true;
		this.SetTheUnityActiveStateOfTheOverlays(true);
        //this.SetTex(this.Overlay, this.normalTex);
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
		this._needToFreeAssetRequest = false;
	}

	private void SetTex(Image sprite, Texture2D tex)
	{
        //sprite.gameObject.renderer.material.SetTexture("_MainTex", tex);
        //sprite.Setup((float)tex.width / 200f, (float)tex.width / 200f, new Vector2(0f, (float)(tex.height - 1)), new Vector2((float)tex.width, (float)tex.height));
	}

	private void SetOpacityOfTheOverlays(float opacity)
	{
		if (Mathf.Abs(opacity - this._lastOpacity) < 0.002f)
		{
			return;
		}
		this._lastOpacity = opacity;
        //Image component = this.Overlay.gameObject.GetComponent<Image>();
        //Color color = component.Color;
        //color.a = opacity;
        //component.Color = color;
	}

	protected override void Update()
	{
		float num = 0.25f;
		float num2 = 1f;
		if (this.Overlay == null || !this.Overlay.gameObject.activeInHierarchy || this._thisIsDisabled || base.IsThisGreyedOut())
		{
			num2 = num;
		}
		if (this._opacityTimer > 0f)
		{
			this._opacityTimer -= Time.deltaTime;
			bool flag = !this._thisIsDisabled && !base.IsThisGreyedOut();
			if (this._opacityTimer <= 0f)
			{
				this._opacityTimer = 0f;
			}
			float num3 = this._opacityTimer / 0.5f;
			num2 = num;
			if (flag)
			{
				num2 += (1f - num) * (1f - num3);
			}
			else
			{
				num2 += (1f - num) * num3;
			}
		}
		this.SetOpacityOfTheOverlays(num2);
		base.Update();
	}

	public override void AdjustToBe(BaseRuntimeControl.State zState)
	{
		base.AdjustToBe(zState);
		switch (zState)
		{
		case BaseRuntimeControl.State.Active:
            //this.text.SetColor(this.CurrentTextColor);
			this.Pressed = false;
			this.AdjustForNewPressedState();
			break;
		case BaseRuntimeControl.State.Pressed:
            //this.text.SetColor(this.CurrentTextColor);
			break;
		case BaseRuntimeControl.State.Disabled:
            //this.text.SetColor(this.DisabledTextColor);
			break;
		}
	}
}
