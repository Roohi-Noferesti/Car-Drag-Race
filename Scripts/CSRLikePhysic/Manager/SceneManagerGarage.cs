using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;
using Object = UnityEngine.Object;

[AddComponentMenu("GT/Logic/SceneManagers/SceneManagerGarage")]
public class SceneManagerGarage : MonoBehaviour, ICustomScreenFadeUp
{
	public GameObject WorldTourPoster;

	public GameObject EnvironmentParent;

	private List<Material> environmentMaterials;

	public GameObject MultiplayerObjects;

	public GameObject SingleplayerObjects;

	public ModeSwitchRendererGroup[] ModeSwitchRendererGroups;

	private bool _started;

	private GarageCarLoader _carLoader;

	private string _lastCarLoaded;

	public Action OnCarLoaded;

    private CarReflectionMapManager.ReflectionTexType ReflectionType = CarReflectionMapManager.ReflectionTexType.WorkshopNight;

	public static SceneManagerGarage Instance
	{
		get;
		private set;
	}

	private GameObject currentCar
	{
		get
		{
			return AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.HumanCar);
		}
	}

	private GameObject currentLivery
	{
		get
		{
			return AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.HumanCarLivery);
		}
	}

	public bool WorldTourPosterInFlight
	{
		get;
		private set;
	}

	public CarVisuals currentCarVisuals
	{
		get;
		private set;
	}

	public bool CarIsLoaded
	{
		get;
		private set;
	}

	public bool RunningDirectlyInGarageScene
	{
		get;
		set;
	}

	public bool SuppressLoadingPanel
	{
		get;
		set;
	}

	public bool SuppressSnapshotAfterLoad
	{
		get;
		set;
	}

	private void Awake()
	{
		SceneManagerGarage.Instance = this;
		this.StoreMaterialReferences();
	}

	private void OnDestroy()
	{
		SceneManagerGarage.Instance = null;
		this.currentCarVisuals = null;
		if (this.environmentMaterials != null)
		{
			foreach (Material current in from m in this.environmentMaterials
			where m != null
			select m)
			{
				if (current.HasProperty("_MainTex"))
				{
					Texture2D texture2D = current.mainTexture as Texture2D;
					if (texture2D != null)
					{
						Resources.UnloadAsset(texture2D);
					}
				}
				if (current.HasProperty("_LightMap"))
				{
					Texture2D texture2D2 = current.GetTexture("_LightMap") as Texture2D;
					if (texture2D2 != null)
					{
						Resources.UnloadAsset(texture2D2);
					}
				}
				UnityEngine.Object.Destroy(current);
			}
		}
	}

	private void OnEnable()
	{
		if (PlayerProfileManager.Instance != null)
		{
			PlayerProfileManager.Instance.OnSceneChanged();
		}
		if (this._started)
		{
            this.LoadCurrentCar();
		}
		this.UpdateActiveMode(false);
	}

	private void OnDisable()
	{
		if (this._carLoader != null)
		{
			this._carLoader.Abort();
			this._carLoader = null;
		}
		if (this.currentCar != null)
		{
			this.currentCar.SetActive(false);
		}
        if (LoadingScreenManager.Instance!=null)
		    LoadingScreenManager.Instance.ClearLoadingPanel();
	}

	private void Start()
	{
		this._started = true;
		this.LoadCurrentCar();
		this.WorldTourPosterInFlight = false;
		this.UpdatePoster();
	}

	private void UpdatePoster()
	{
		if (BaseDevice.ActiveDevice.DeviceQuality == AssetQuality.High)
		{
            //this.WorldTourPosterInFlight = true;
            //TexturePack.RequestTextureFromBundle(true?/*(!MultiplayerUtils.GarageInMultiplayerMode) ?*/ "WorldTourPoster.GaragePosterNight" : "WorldTourPoster.GaragePosterDay", delegate(Texture2D texture)
            //{
            //    this.WorldTourPoster.SetActive(true);
            //    this.WorldTourPoster.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            //    this.environmentMaterials.Add(this.WorldTourPoster.GetComponent<Renderer>().material);
            //    this.WorldTourPosterInFlight = false;
            //    this.CheckAllLoaded();
            //});
		}
	}

	public void StoreMaterialReferences()
	{
		Renderer[] componentsInChildren = this.EnvironmentParent.GetComponentsInChildren<Renderer>();
		this.environmentMaterials = componentsInChildren.SelectMany((Renderer r) => r.materials).ToList<Material>();
	}

    private void LoadCurrentCar()
    {
        if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            return;
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		this.LoadCar(currentCar);
	}

	public void LoadCar(CarGarageInstance garageCar)
	{
		if (this._carLoader != null)
		{
			this._carLoader.Abort();
			this._carLoader = null;
		}
		this.CarIsLoaded = false;
		if (!this.SuppressLoadingPanel)
		{
			LoadingScreenManager.Instance.TriggerLoadingPanel("TEXT_LOADING_CAR");
            //ScreenManager.Instance.CheckerGradientBackground.Show(false);
			GarageCameraManager.Instance.SetStateClear();
		}
		this._carLoader = new GarageCarLoader(garageCar, AsyncBundleSlotDescription.HumanCar, AsyncBundleSlotDescription.HumanCarLivery, base.gameObject);
		this._carLoader.LastCarLoaded = this._lastCarLoaded;
		this._carLoader.Load(this.CarLoaded, this.CarLoadFailed);
	}

	private void CarLoaded(CarGarageInstance info, GameObject car, GameObject livery)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.currentCarVisuals = car.GetComponent<CarVisuals>();
		GarageCarVisuals garageCarVisuals = this.currentCar.GetComponent<GarageCarVisuals>();
		if (garageCarVisuals == null)
		{
			garageCarVisuals = this.currentCar.AddComponent<GarageCarVisuals>();
			garageCarVisuals.Setup(this.currentCarVisuals, CarDatabase.Instance.GetCar(info.CarDBKey));
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null)
		{
			this.currentCarVisuals.SetCurrentColor(activeProfile.GetCurrentCar().AppliedColourIndex);
			this.currentCarVisuals.ApplyLivery(livery, true, delegate(CarBaseTextureBaker baker)
			{
				this.CarGotBaked(info, baker);
			});
		    var numberPlateText = PlayerProfileManager.Instance.ActiveProfile.DisplayName;
            this.currentCarVisuals.ApplyNumberPlateText(numberPlateText);
            //NumberPlateManager.Instance.RenderPlayerNumberPlate(delegate(Texture2D texture)
            //{
            //    this.currentCarVisuals.ApplyNumberPlate(texture);
            //});

            var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		    //var reflType = CarReflectionMapManager.ReflectionTexType.WorkshopNight;

            //Body Material
            var item = currentCar.GetEquipedVirtualItemID(VirtualItemType.BodyShader);
		    SetupBodyMaterial(item);

            //Ring Material
            item = currentCar.GetEquipedVirtualItemID(VirtualItemType.RingShader);
		    SetupRingMaterial(item);

            //Headlight Material
            item = currentCar.GetEquipedVirtualItemID(VirtualItemType.HeadLighShader);
		    SetupHeadlightMaterial(item);

            //Sticker
            item = currentCar.GetEquipedVirtualItemID(VirtualItemType.CarSticker,true, this.currentCarVisuals.DefaultStickerID);
		    SetupSticker(currentCar.ID, item);

            //Spoiler
            item = currentCar.GetEquipedVirtualItemID(VirtualItemType.CarSpoiler);
		    SetupSpoiler(item);

            //Height
		    SetupHeight(currentCar.BodyHeight);
		}
		this._lastCarLoaded = info.CarDBKey;
	}

    public void SetupBodyMaterial(string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultBodyShaderID;
        var material = CarReflectionMapManager.GetCarBodyMaterial(item, ReflectionType);
        currentCarVisuals.BodyMaterial = material;

        if (currentCarVisuals.HaveAdditionalRenderer)
        {
            var additionalMaterial = CarReflectionMapManager.GetCarBodyMaterial(currentCarVisuals.AdditionalShaderID, ReflectionType);
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
        material.SetInt("_Day", 0);
        currentCarVisuals.HeadLightMaterial = material;
    }

    public void SetupSticker(string carID,string item)
    {
        Texture2D sticker;
        Vector2 texScale;
        if (string.IsNullOrEmpty(item) || item.ToLower() == "sticker_no")
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
                GTDebug.LogWarning(GTLogChannel.Garage, string.Format("No spoiler found width ID '{0}'", item));
                return;
            }
            var spoilerInstance = Instantiate(spoiler);
            currentCarVisuals.SetSpoiler(item, spoilerInstance);
        }
    }

    public bool SetupHeight(float height)
    {
        if (currentCarVisuals.BodyHeight != height)
        {
            currentCarVisuals.BodyHeight = height;
            return true;
        }
        return false;
    }

	private void CarLoadFailed(CarGarageInstance info)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (info.CarDBKey != this._lastCarLoaded)
		{
			this._lastCarLoaded = null;
		}
		this._carLoader = null;
		LoadingScreenManager.Instance.ClearLoadingPanel();
	}

	private void CarGotBaked(CarGarageInstance garageCar, CarBaseTextureBaker baker)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this._carLoader = null;
		this.ReadyToShowCar();
	}

	private void ReadyToShowCar()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.CarIsLoaded = true;
		this.CheckAllLoaded();
	}

	private void CheckAllLoaded()
	{
		if (!this.CarIsLoaded || this.WorldTourPosterInFlight)
		{
			return;
		}
		if (LoadingScreenManager.Instance.IsShowingLoadingPanel)
		{
		    if (GarageScreen.Instance==null || !GarageScreen.Instance.IsCarNew)
		    {
		        GarageCameraManager.Instance.ResetCameraToStartPos();
		        LoadingScreenManager.Instance.ClearLoadingPanel();
		    }
		}
        //if (ScreenManager.Instance.ActiveScreen.ScreenBackground != BackgroundManager.BackgroundType.Gradient && ScreenManager.Instance.ActiveScreen.ScreenBackground != BackgroundManager.BackgroundType.ColourGradient)
        //{
        //    ScreenManager.Instance.CheckerGradientBackground.FadeOutIfRequired(1f, 0.1f);
        //}
		if (this.OnCarLoaded != null)
		{
			this.OnCarLoaded();
		}
	}

	public void UpdateActiveMode(bool refreshPoster = false)
	{
	    //var garageInMultiplayer = false;//MultiplayerUtils.GarageInMultiplayerMode;
        //this.SingleplayerObjects.gameObject.SetActive(!garageInMultiplayer);
        //this.MultiplayerObjects.gameObject.SetActive(garageInMultiplayer);
		if (refreshPoster)
		{
			this.UpdatePoster();
		}
		if (this.currentCar != null)
		{
			CarVisuals component = this.currentCar.GetComponent<CarVisuals>();
			GarageCarVisuals component2 = this.currentCar.GetComponent<GarageCarVisuals>();
			if (component2 != null)
			{
				component2.RefreshVisuals(component);
			}
		}
		ModeSwitchRendererGroup[] modeSwitchRendererGroups = this.ModeSwitchRendererGroups;
		for (int i = 0; i < modeSwitchRendererGroups.Length; i++)
		{
			ModeSwitchRendererGroup modeSwitchRendererGroup = modeSwitchRendererGroups[i];
			Renderer[] renderers = modeSwitchRendererGroup.Renderers;
			for (int j = 0; j < renderers.Length; j++)
			{
				Renderer renderer = renderers[j];
				if (MultiplayerUtils.GarageInMultiplayerMode)
				{
					renderer.material.SetTexture("_LightMap", modeSwitchRendererGroup.MultiplayerLightmap);
				}
				else
				{
					renderer.material.SetTexture("_LightMap", modeSwitchRendererGroup.SingleplayerLightmap);
				}
			}
		}
	}

    public void ActivateAndEnable()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        if (!enabled)
        {
            enabled = true;
        }
    }
}
