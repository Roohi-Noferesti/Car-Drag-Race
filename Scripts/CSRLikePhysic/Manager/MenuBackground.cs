using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour, IPersistentUI
{
	public Image Lower;

    public Image Upper;

    public Image DropShadow;

    public Image DropShadowUpper;

	public GameObject AnimOffset;

	//private CSRScreen scrn;

	private Vector3 UpperOrigin;

	public static Vector3 UpperHideOffset = new Vector3(0f, -0.51f, 0f);

	public CarouselTransition CarouselTransition
	{
		get
		{
			return base.GetComponent<CarouselTransition>();
		}
	}

	private void Awake()
	{
		this.CalculateSize();
	}

	public void CalculateSize()
	{
		//float w = ResolutionManager.PixelSizeToWorldSpaceSize(BaseDevice.ActiveDevice.GetScreenWidth());
        //this.Lower.SetSize(w, this.Lower.height);
        //this.Upper.SetSize(w, this.Upper.height);
        //this.DropShadow.SetSize(w, this.DropShadow.height);
        //this.DropShadowUpper.SetSize(w, this.DropShadowUpper.height);
	}

	public void OnScreenChanged(ScreenID zNewScreen)
	{
		//this.scrn = (ScreenManager.Instance.ActiveScreen as CSRScreen);
		//bool hasLowerCarousel = this.scrn.HasLowerCarousel;
		//bool flag = !this.scrn.HasUpperCarousel;
        //bool flag2 = !this.Upper.IsHidden() && flag;
        //this.Lower.Hide(!hasLowerCarousel);
        //this.Upper.Hide(flag);
        //this.DropShadow.Hide(!hasLowerCarousel);
        //this.DropShadowUpper.Hide(!this.scrn.HasUpperCarousel);
        //if (flag2 && GarageCameraManager.Instance != null)
        //{
        //    GarageCameraManager.Instance.ForceUpdateCameraRect();
        //}
		this.CarouselTransition.SetToDefaultMaterial(MultiplayerUtils.GarageInMultiplayerMode);
	}

	public void Show(bool zShow)
	{
		this.Lower.gameObject.SetActive(zShow);
		this.Upper.gameObject.SetActive(zShow);
		this.DropShadow.gameObject.SetActive(zShow);
		this.DropShadowUpper.gameObject.SetActive(zShow);
	}

	public void SetAnimOffsetPosition(Vector3 vect)
	{
		this.AnimOffset.transform.localPosition = vect;
	}

	public void Start()
	{
		this.UpperOrigin = new Vector3(0f, 0.7f, 0.2f);
	}

	public void SetUpperHideAmount(float interp)
	{
		this.Upper.transform.localPosition = this.UpperOrigin + interp * MenuBackground.UpperHideOffset;
	}
}
