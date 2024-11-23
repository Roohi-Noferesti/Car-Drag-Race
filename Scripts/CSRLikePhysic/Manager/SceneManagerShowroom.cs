using UnityEngine;
using UnityStandardAssets.Cameras;

[AddComponentMenu("GT/Logic/SceneManagers/SceneManagerShowroom")]
public class SceneManagerShowroom : MonoBehaviour, ICustomScreenFadeUp
{
	public delegate void CarSwitchedEventHandler();

	public GameObject backPanel;

	public GameObject logoPlacementDummy;

	public GameObject mainCamera;

	private AsyncBundleSlotDescription showroomSlot = AsyncBundleSlotDescription.AICar;

	private AsyncBundleSlotDescription showroomLiverySlot = AsyncBundleSlotDescription.AICarLivery;

	private AsyncBundleSlotDescription showroomLogoSlot = AsyncBundleSlotDescription.ManufacturerLogo;

    public GameObject placement;

    private ShowroomCameraManager cameraManager;

	private Camera mainSharedCamera;

	private Material backPanelMaterial;

	private bool readyToGo;

	private bool carOk;

	private bool liveryOk;

	//private bool logoOk;

	private string _lastLogoPositioned;
    private CarReflectionMapManager.ReflectionTexType ReflectionType = CarReflectionMapManager.ReflectionTexType.Showroom;

    public event CarSwitchedEventHandler CarSwitched;

	public static SceneManagerShowroom Instance
	{
		get;
		private set;
	}

	public CarInfo requestedCarInfo
	{
		get;
		private set;
	}

	public CarInfo currentCarInfo
	{
		get;
		private set;
	}

	public CarVisuals currentCarVisuals
	{
		get;
		private set;
	}

	private string currentlyShowingCarName
	{
		get
		{
			return AsyncSwitching.Instance.CurrentName(this.showroomSlot);
		}
	}

	private GameObject currentCar
	{
		get
		{
			return AsyncSwitching.Instance.CurrentObject(this.showroomSlot);
		}
	}

	private GameObject currentLogo
	{
		get
		{
			return AsyncSwitching.Instance.CurrentObject(this.showroomLogoSlot);
		}
	}

	private void Awake()
	{
		Instance = this;
	    this.cameraManager = GetComponent<ShowroomCameraManager>();//this.mainCamera.GetComponent<ShowroomCameraManager>();
        //this.backPanelMaterial = this.backPanel.renderer.material;
        //this.placement = new GameObject();
        //this.placement.name = "CarPlacement(Showroom)";
        //this.placement.transform.parent = base.transform;
		this.readyToGo = true;
	}

	private void OnDestroy()
	{
		Instance = null;
		this.currentCarVisuals = null;
        this.cameraManager = null;
		this.mainSharedCamera = null;
		this.logoPlacementDummy = null;
		this.mainCamera = null;
        //this.placement = null;
		if (this.currentLogo != null)
		{
			AsyncSwitching.Instance.ClearSlot(this.showroomLogoSlot);
		}
	}

	private void OnEnable()
	{
		if (!this.readyToGo)
		{
			return;
		}
	    ShowroomCameraManager.CameraAtBackEvent += CameraAtBack;
	    AssetDirectory.AssetUpdatedEvent += OnAssetUpdated;
        //GameObject gameObject = GameObject.Find("MainCamera (Shared)");
        //this.mainSharedCamera = gameObject.GetComponent<Camera>();
        //this.mainSharedCamera.enabled = false;
		this.carOk = false;
		this.currentCarInfo = null;
        this.cameraManager.ResetToBack();
        //if (FadeQuad.Instance != null)
        //{
        //    FadeQuad instance = FadeQuad.Instance;
        //    instance.SetColor(Color.black);
        //    instance.FadeTo(new Color(0f, 0f, 0f, 0f), 0.3f);
        //}
		if (PlayerProfileManager.Instance != null)
		{
			PlayerProfileManager.Instance.OnSceneChanged();
		}
	}

	private void OnDisable()
	{
		this.currentCarInfo = null;
		if (!this.readyToGo)
		{
			return;
		}
		if (this.currentCar != null)
		{
			this.currentCar.SetActive(false);
		}
		if (this.currentLogo != null)
		{
			this.currentLogo.SetActive(false);
		}
	    AssetDirectory.AssetUpdatedEvent -= OnAssetUpdated;
	    ShowroomCameraManager.CameraAtBackEvent -= CameraAtBack;
		AsyncSwitching.Instance.ClearCallbacks(this.showroomSlot, base.gameObject);
		AsyncSwitching.Instance.ClearCallbacks(this.showroomLiverySlot, base.gameObject);
		AsyncSwitching.Instance.ClearCallbacks(this.showroomLogoSlot, base.gameObject);
		if (this.mainSharedCamera != null)
		{
			this.mainSharedCamera.enabled = true;
		}
	}

	public bool IsLoadingCar()
	{
		return AsyncSwitching.Instance.SlotIsLoading(this.showroomSlot);
	}

	private void CameraAtBack()
	{
		this.CheckForSwitch();
	}

	private void OnAssetUpdated(string zAssetID)
	{
		if (zAssetID == this.currentlyShowingCarName)
		{
			this.LoadCarInShowroom(this.currentCarInfo);
		}
	}

	public void LoadCarInShowroom(CarInfo carInfo)
	{
	    string modelPrefabString = carInfo.ModelPrefabString;
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.carOk = false;
		this.liveryOk = false;
		//this.logoOk = false;
		if (this.currentCarInfo != null && this.currentCarInfo == carInfo && carInfo == this.requestedCarInfo)
		{
			return;
		}
		this.requestedCarInfo = carInfo;
		this.currentCarInfo = null;
		AsyncSwitching.Instance.RequestAsset(this.showroomSlot, modelPrefabString, this.CarLoaded, base.gameObject, true, null);
		if (false)//!string.IsNullOrEmpty(carInfo.DefaultLiveryBundleName))
		{
			AsyncSwitching.Instance.RequestAsset(this.showroomLiverySlot, carInfo.DefaultLiveryBundleName, this.LiveryLoaded, base.gameObject, true, null);
		}
		else
		{
			AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICarLivery);
			this.liveryOk = true;
		}
        if (!this.cameraManager.IsBackCamEnabled())//.backCam.enabled)
        {
            this.cameraManager.TransitionToBack();
        }
		this.EnableCorrectLogo(carInfo);
	}

	private void CarLoaded(bool loadOk, string name)
	{
		this.carOk = loadOk;
		this.CheckForSwitch();
	}

	private void LiveryLoaded(bool loadOk, string name)
	{
		this.liveryOk = loadOk;
		if (loadOk)
		{
			GameObject livery = AsyncSwitching.Instance.GetLivery(AsyncBundleSlotDescription.AICarLivery);
			livery.name = name;
		}
		this.CheckForSwitch();
	}

	private void LogoLoaded(bool loadOk, string name)
	{
		//this.logoOk = loadOk;
		this.CheckForSwitch();
	}

	public void CheckForSwitch()
	{
        bool isBackCamEnabled = this.cameraManager.IsBackCamEnabled();
        if (!this.cameraManager.IsTransitioningToBack() && !isBackCamEnabled)
        {
            if (this.carOk && this.liveryOk && this.cameraManager.GetTransitionTime()<0.25f)//backToFront.GetTransitionTime() < 0.25f)
            {
                isBackCamEnabled = true;
            }
            else
            {
                this.cameraManager.TransitionToBack();
            }
        }
        if (!this.carOk /*|| !this.liveryOk || !this.logoOk*/ || !isBackCamEnabled)
        {
            return;
        }
		this.SwitchCarNow();
	}

	public void SwitchCarNow()
	{
		this.currentCarInfo = this.requestedCarInfo;
		this.SwitchLogoNow();
		AsyncSwitching.Instance.GetCar(this.showroomSlot);
		this.currentCar.transform.localPosition = this.placement.transform.position;
		ShowroomCarVisuals showroomCarVisuals = this.currentCar.AddComponent<ShowroomCarVisuals>();
		this.currentCarVisuals = this.currentCar.GetComponent<CarVisuals>();
        showroomCarVisuals.Setup(this.currentCarVisuals, this.currentCarInfo);

        //Body Material
        SetupBodyMaterial(null);
        //Ring Material
        SetupRingMaterial(null);
        //Headlight Material
        SetupHeadlightMaterial(null);
        //Sticker
        SetupSticker(this.currentCarInfo.ID, currentCarVisuals.DefaultStickerID);
        //Spoiler
        SetupSpoiler(null);
        //Height
        SetupHeight(0);

        this.currentCarVisuals.ApplyNumberPlateText(null);

        this.ReadyToTransitionToFront();

	    LimitLookCam.Instance.Target = this.currentCar.transform;
        LimitLookCam.Instance.ResetTransform();

        //if (ShowroomCameraManager.Instance.IsZoomedIn)
        //{
        //    GameObject liveryObj = AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.AICarLivery);
        //    this.currentCarVisuals.ApplyLivery(liveryObj, true, delegate(CarBaseTextureBaker baker)
        //    {
        //        this.ReadyToTransitionToFront();
        //    });
        //}
		this.InvokeCarSwitchedEvent();
	}

	public void ReadyToTransitionToFront()
	{
        if (!this.cameraManager.IsFrontCamEnabled() && !this.cameraManager.IsTransitioningToFront())
        {
            this.cameraManager.TransitionToFront();
        }
	}

	private void InvokeCarSwitchedEvent()
	{
		if (this.CarSwitched != null)
		{
			this.CarSwitched();
		}
	}

	private void EnableCorrectLogo(CarInfo info)
	{
        //if (!base.gameObject.activeInHierarchy)
        //{
        //    return;
        //}
        //string badge = ManufacturerDatabase.Instance.Get((int) info.ManufacturerID).badge;
        //if (this.currentLogo == null)
        //{
        //    this._lastLogoPositioned = null;
        //}
        //AsyncSwitching.Instance.RequestAsset(this.showroomLogoSlot, badge, this.LogoLoaded, base.gameObject, true, null);
	}

	public void SwitchLogoNow()
	{
		GameObject logo = AsyncSwitching.Instance.GetLogo(this.showroomLogoSlot);
		if (logo == null)
		{
			return;
		}
		this.PositionLogo();
	}

	private void PositionLogo()
	{
		if (this._lastLogoPositioned == this.currentLogo.name)
		{
			return;
		}
		this._lastLogoPositioned = this.currentLogo.name;
        ShowroomManufacturerLogo showroomManufacturerLogo = this.currentLogo.GetComponentsInChildren<ShowroomManufacturerLogo>(true)[0];
        if (showroomManufacturerLogo != null)
        {
            this.currentLogo.transform.localPosition = this.logoPlacementDummy.transform.position;
            this.currentLogo.transform.localRotation = this.logoPlacementDummy.transform.rotation;
            this.backPanelMaterial.SetTexture("_MainTex", showroomManufacturerLogo.backpanelTexture);
        }
	}


    public void SetupBodyMaterial(string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultBodyShaderID;
        var material = CarReflectionMapManager.GetCarBodyMaterial(item, ReflectionType);
        currentCarVisuals.BodyMaterial = material;

        if (currentCarVisuals.HaveAdditionalRenderer)
        {
            var additionalMaterial = CarReflectionMapManager.GetCarBodyMaterial(currentCarVisuals.AdditionalShaderID, CarReflectionMapManager.ReflectionTexType.WorkshopNight);
            currentCarVisuals.AdditionalMaterial = additionalMaterial;
        }
    }

    public void SetupRingMaterial(string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultRingShaderID;
        var material = CarReflectionMapManager.GetCarRingMaterial(item, ReflectionType);
        currentCarVisuals.RingMaterial = material;
    }

    public void SetupHeadlightMaterial(string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultHeadlightShaderID;
        var material = CarReflectionMapManager.GetCarHeadlightMaterial(item, ReflectionType);
        currentCarVisuals.HeadLightMaterial = material;
    }

    public void SetupSticker(string carID, string item)
    {
        Texture2D sticker;
        Vector2 texScale;
        if (string.IsNullOrEmpty(item) || item.ToLower()=="sticker_no")// item == currentCarVisuals.DefaultStickerID)
        {
            texScale = Vector2.one;
            sticker = CarReflectionMapManager.GetSharedItemID<Texture2D>(item, ServerItemBase.AssetType.sticker, ReflectionType);
        }
        else
        {
            CarReflectionMapManager.GetCarStickerTexture(carID, item, ReflectionType, out sticker, out texScale);
        }
        currentCarVisuals.CacheStickerScale(texScale);
        currentCarVisuals.Sticker = sticker;
    }

    public void SetupSpoiler(string item)
    {
        if (string.IsNullOrEmpty(item) || item.ToLower() == "spoiler_no")
        {
            currentCarVisuals.ClearSpoiler();
        }
        else
        {
            var spoiler = CarReflectionMapManager.GetSharedItemID<GameObject>(item, ServerItemBase.AssetType.spoiler, ReflectionType);

            if (spoiler == null)
            {
                GTDebug.LogWarning(GTLogChannel.Showroom, string.Format("No spoiler found width ID '{0}'", item));
                return;
            }
            var spoilerInstance = Instantiate(spoiler);
            currentCarVisuals.SetSpoiler(item, spoilerInstance);
        }
    }

    public void SetupHeight(float height)
    {
        currentCarVisuals.BodyHeight = currentCarVisuals.DefaultBodyHeight;
    }
}
