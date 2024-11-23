using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CarSnapshotManager : MonoBehaviour
{
	private class AsyncJob
	{
		public ImageLoaded Callback;

		public WWW AsyncLoader;

		public string FilePath;
	}

	public delegate void ImageLoaded(Texture2D tex, bool loadedOk);

	private const int _snapshotVersionNumber = 3;

	private const string _snapshotVersionPreamble = "_Version";

	public Transform PlacementNode;

	public Light DefaultDirectionalLight;

	public Light DefaultAmbientLight;

	public Light PrizeDirectionalLight;

	public Light PrizeAmbientLight;

	public Camera SnapshotCamera;

	public Camera VsSnapshotCameraRight;

	public Camera VsSnapshotCameraLeft;

	public Camera MultiplayerPrizeCamera;

	public Camera MultiplayerModeSelectCamera;

	public Camera PlayerListCamera;

	public AsyncBundleSlotDescription CarSlot = AsyncBundleSlotDescription.AICar;

	public AsyncBundleSlotDescription LiverySlot = AsyncBundleSlotDescription.AICarLivery;

	private Camera _currentCamera;

	private float _aspectRatio = 1f;

	private int _snapshotSize = 256;

	private CarSnapshotType _snapshotType;

	private string _snapshotVersionFileNameText;

	private List<AsyncJob> _asyncJobs = new List<AsyncJob>();

	public static CarSnapshotManager Instance
	{
		get;
		private set;
	}

	public string CachePath
	{
		get;
		private set;
	}

	public string ShortCachePath
	{
		get;
		private set;
	}

	public int SnapshotSize
	{
		get
		{
			return this._snapshotSize;
		}
		set
		{
			this._snapshotSize = value;
		}
	}

	public CarSnapshotType SnapshotType
	{
		get
		{
			return this._snapshotType;
		}
		set
		{
			this._aspectRatio = GetAspectRatio(value);
			switch (value)
			{
			case CarSnapshotType.Default:
				this._currentCamera = this.SnapshotCamera;
				break;
			case CarSnapshotType.VSScreenRight:
				this._currentCamera = this.VsSnapshotCameraRight;
				break;
			case CarSnapshotType.VSScreenLeft:
				this._currentCamera = this.VsSnapshotCameraLeft;
				break;
			case CarSnapshotType.WorldTourPrizePreview:
				this._currentCamera = this.VsSnapshotCameraRight;
				break;
			case CarSnapshotType.MultiplayerPrize:
				this._currentCamera = this.MultiplayerPrizeCamera;
				break;
			case CarSnapshotType.MultiplayerModeSelect:
				this._currentCamera = this.MultiplayerModeSelectCamera;
				break;
			case CarSnapshotType.PlayerList:
				this._currentCamera = this.PlayerListCamera;
				break;
			}
			this._snapshotType = value;
		}
	}

	public Light AmbientLight
	{
		get
		{
			switch (this.SnapshotType)
			{
			case CarSnapshotType.MultiplayerPrize:
			case CarSnapshotType.PlayerList:
				return this.PrizeAmbientLight;
			}
			return this.DefaultAmbientLight;
		}
	}

	public Light DirectionalLight
	{
		get
		{
			switch (this.SnapshotType)
			{
			case CarSnapshotType.MultiplayerPrize:
			case CarSnapshotType.PlayerList:
				return this.PrizeDirectionalLight;
			}
			return this.DefaultDirectionalLight;
		}
	}

	public static float GetAspectRatio(CarSnapshotType snapshotType)
	{
        //return 1.337F;
	    return 0.747F;
        //switch (snapshotType)
        //{
        //case CarSnapshotType.VSScreenRight:
        //case CarSnapshotType.VSScreenLeft:
        //case CarSnapshotType.WorldTourPrizePreview:
        //case CarSnapshotType.MultiplayerPrize:
        //case CarSnapshotType.PlayerList:
        //    return 0.5f;
        //}
        //return 1f;
	}

	private static string GetSnapshotTypeSuffix(CarSnapshotType snapshotType)
	{
		switch (snapshotType)
		{
		case CarSnapshotType.Default:
			return string.Empty;
		case CarSnapshotType.VSScreenRight:
			return "_VsRight";
		case CarSnapshotType.VSScreenLeft:
			return "_VsLeft";
		case CarSnapshotType.MultiplayerPrize:
			return "_Prize";
		case CarSnapshotType.MultiplayerModeSelect:
			return "_ModeSelect";
		case CarSnapshotType.PlayerList:
			return "_PlayerList";
		}
		return string.Empty;
	}

	private void Awake()
	{
		Instance = this;
		this.SetCachePath("temp");
		this.SnapshotType = CarSnapshotType.Default;
		this.SetCacheVersionText();
		this.DeleteWrongVersionSnapshots();
	    _currentCamera.enabled = false;
	}

	public void ResetToDefaults()
	{
		this.CarSlot = AsyncBundleSlotDescription.AICar;
		this.LiverySlot = AsyncBundleSlotDescription.AICarLivery;
		this.SnapshotType = CarSnapshotType.Default;
		this.SnapshotSize = 256;
	}

	public void GenerateSnapshot(string carDBKey)
	{
		this.GenerateSnapshot(carDBKey, delegate(Texture2D texture)
		{
		});
	}

	public void GenerateSnapshot(string carDBKey, Action<Texture2D> callback)
	{
	    var car = CarDatabase.Instance.GetCar(carDBKey);
		CarGarageInstance carGarageInstance = new CarGarageInstance();
        carGarageInstance.SetupNewGarageInstance(car);
		this.GenerateSnapshot(carGarageInstance, callback);
	}

	public void GenerateSnapshot(CarGarageInstance carGarageInstance)
	{
		this.GenerateSnapshot(carGarageInstance, delegate(Texture2D texture)
		{
		});
	}

	public void GenerateSnapshot(CarGarageInstance carGarageInstance, Action<Texture2D> callback)
	{
		GarageCarLoader garageCarLoader = new GarageCarLoader(carGarageInstance, this.CarSlot, this.LiverySlot, base.gameObject);
		garageCarLoader.Load(delegate(CarGarageInstance info, GameObject car, GameObject livery)
		{
			CameraPostRender.Instance.AddProcess("snapshot for " + this.GetSnapshotCacheName(carGarageInstance), delegate
			{
                CarVisuals component = car.GetComponent<CarVisuals>();
                component.ApplyLivery(livery, true);
                component.SetCurrentColor(carGarageInstance.AppliedColourIndex);
				this.GenerateSnapshot(carGarageInstance, car, null, callback);
			});
		}, delegate(CarGarageInstance info2)
		{
			if (callback != null)
			{
				callback(null);
			}
		});
	}

	public void GenerateSnapshot(CarGarageInstance carGarageInstance, GameObject loadedCar, Texture2D numberPlate, Action<Texture2D> callback)
	{
		if (loadedCar == null)
		{
			return;
		}
		if (!loadedCar.activeInHierarchy)
		{
			return;
		}
		CarVisuals component = loadedCar.GetComponent<CarVisuals>();
		this.ApplyVisualSettings(carGarageInstance, component, component.CurrentlyAppliedLivery, numberPlate);
		Texture2D obj;
	    _currentCamera.enabled = true;
		if (this.ShouldUseManualAA())
		{
			obj = this.RenderSnapshotAndCacheManualAA(carGarageInstance);
		}
		else
		{
			obj = this.RenderSnapshotAndCache(carGarageInstance);
		}
        _currentCamera.enabled = false;
        this.RemoveVisualSettings(loadedCar);
		if (callback != null)
		{
			callback(obj);
		}
	}

	private bool ShouldUseManualAA()
	{
		return BasePlatform.TargetPlatform == GTPlatforms.iOS || BasePlatform.TargetPlatform == GTPlatforms.ANDROID || BasePlatform.TargetPlatform == GTPlatforms.METRO || BasePlatform.TargetPlatform == GTPlatforms.WP8;
	}

	public void ClearCache()
	{
		Directory.Delete(this.CachePath, true);
		Directory.CreateDirectory(this.CachePath);
	}

	public bool SnapshotCacheExists(CarGarageInstance carGarageInstance)
	{
		string snapshotCacheName = this.GetSnapshotCacheName(carGarageInstance);
		string path = Path.Combine(this.CachePath, snapshotCacheName);
		return File.Exists(path);
	}

	public bool SnapshotCacheExists(string carKey)
	{
	    var car = CarDatabase.Instance.GetCar(carKey);
		CarGarageInstance carGarageInstance = new CarGarageInstance();
        carGarageInstance.SetupNewGarageInstance(car);
		return this.SnapshotCacheExists(carGarageInstance);
	}

	public void ClearSnapshotFromCache(CarGarageInstance carGarageInstance)
	{
		string snapshotCachePath = this.GetSnapshotCachePath(carGarageInstance);
		if (FileUtils.Exists(snapshotCachePath))
		{
			FileUtils.EraseLocalStorageFile(snapshotCachePath, false);
		}
	}

	public Texture2D LoadSnapshotFromCache(CarGarageInstance carGarageInstance)
	{
		string snapshotCachePath = this.GetSnapshotCachePath(carGarageInstance);
		if (!FileUtils.Exists(snapshotCachePath))
		{
			return null;
		}
		byte[] data = null;
		FileUtils.ReadLocalStorage(snapshotCachePath, ref data, false);
		Texture2D texture2D = new Texture2D(this.SnapshotSize, this.SnapshotSize, TextureFormat.ARGB32, false);
		texture2D.LoadImage(data);
		texture2D.filterMode = FilterMode.Trilinear;
		return texture2D;
	}

	public string GetSnapshotCachePath(CarGarageInstance carGarageInstance)
	{
		return Path.Combine(this.ShortCachePath, this.GetSnapshotCacheName(carGarageInstance));
	}

	private void DeleteWrongVersionSnapshots()
	{
		string[] files = Directory.GetFiles(this.CachePath);
		if (files != null)
		{
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string path = array[i];
				string fileName = Path.GetFileName(path);
				if (!fileName.Contains(this._snapshotVersionFileNameText))
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception) // var_5_44)
					{
					    // ignored
					}
				}
			}
		}
	}

	private void ClearAllSnapshots()
	{
		string[] files = Directory.GetFiles(this.CachePath);
		if (files != null)
		{
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string path = array[i];
				try
				{
					File.Delete(path);
				}
				catch (Exception)// var_4_2A)
				{
				    // ignored
				}
			}
		}
	}

	public void ClearUnusedSnapshots(CarGarageInstance carGarageInstance)
	{
		string b = this.GetSnapshotCacheName(carGarageInstance).ToLower();
		string value = carGarageInstance.CarDBKey.ToLower();
		string[] files = Directory.GetFiles(this.CachePath);
		if (files == null)
		{
			return;
		}
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string path = array[i];
			string text = Path.GetFileName(path).ToLower();
			if (!(text == b))
			{
				if (text.StartsWith(value))
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception)// var_7_74)
					{
					    // ignored
					}
				}
			}
		}
	}

	public void ClearSnapshotsOfType(CarSnapshotType type)
	{
		string[] files = Directory.GetFiles(this.CachePath);
		string snapshotTypeSuffix = GetSnapshotTypeSuffix(type);
		if (files != null)
		{
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string path = array[i];
				if (base.name.Contains(snapshotTypeSuffix))
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception) // var_5_44)
					{
					    // ignored
					}
				}
			}
		}
	}

	public void OnProfileChanged()
	{
		string name = PlayerProfileManager.Instance.ActiveProfile.Name;
		this.SetCachePath(name);
		this.SetCacheVersionText();
		this.DeleteWrongVersionSnapshots();
	}

	private void SetCacheVersionText()
	{
		this._snapshotVersionFileNameText = "_Version" + 3;
	}

	private void SetCachePath(string profileName)
	{
		this.CachePath = FileUtils.GetLocalStorageFilePath("CarSnapshots");
		this.CachePath = Path.Combine(this.CachePath, profileName);
		this.ShortCachePath = "CarSnapshots/" + profileName;
		if (!Directory.Exists(this.CachePath))
		{
			Directory.CreateDirectory(this.CachePath);
		}
	}

	private void ApplyVisualSettings(CarGarageInstance carGarageInstance, CarVisuals carVisuals, GameObject livery, Texture2D numberPlate)
	{
		CarSnapshotVisuals carSnapshotVisuals = carVisuals.gameObject.AddComponent<CarSnapshotVisuals>();
		carSnapshotVisuals.Setup(carVisuals, CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey));

        //Body Material
        SetupBodyMaterial(carVisuals,null);
        //Ring Material
        SetupRingMaterial(carVisuals,null);
        //Headlight Material
        SetupHeadlightMaterial(carVisuals,null);
        //Sticker
        SetupSticker(carVisuals,carGarageInstance.ID, null);
        //Spoiler
        SetupSpoiler(carVisuals,null);
        //Height
        SetupHeight(carVisuals,0);


		if (carGarageInstance.UseColorOverride)
		{
			carVisuals.SetCurrentColor(carVisuals.AddNewColor(carGarageInstance.ColorOverride));
		}
		else
		{
            carVisuals.SetCurrentColor(carGarageInstance.AppliedColourIndex);
		}
        carVisuals.ApplyLivery(livery, true);
		if (numberPlate == null)
		{
            //carVisuals.ApplyNumberPlate(NumberPlateManager.Instance.RenderImmediate(carGarageInstance.NumberPlate));
		}
		else
		{
			carVisuals.ApplyNumberPlate(numberPlate);
		}
	}

	private void RemoveVisualSettings(GameObject car)
	{
		CarSnapshotVisuals component = car.GetComponent<CarSnapshotVisuals>();
		DestroyImmediate(component);
	}

	private Texture2D RenderSnapshotAndCacheManualAA(CarGarageInstance carGarageInstance)
	{
		int num = (int)((float)(this.SnapshotSize * 2) * this._aspectRatio);
		RenderTexture active = RenderTexture.active;
		Texture2D texture2D = new Texture2D(this.SnapshotSize, num / 2, TextureFormat.ARGB32, false);
		if (CarQualitySettings.RenderSnapshotsWithDownsampledAA())
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this.SnapshotSize * 2, num, 16, RenderTextureFormat.ARGB32);
			BasePlatform.ActivePlatform.PrepareRenderTexture(temporary);
			this._currentCamera.targetTexture = temporary;
			this._currentCamera.Render();
			this._currentCamera.targetTexture = null;
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.SnapshotSize, num / 2, 0, RenderTextureFormat.ARGB32);
			BasePlatform.ActivePlatform.PrepareRenderTexture(temporary2);
			temporary2.DiscardContents();
			Graphics.Blit(temporary, temporary2);
			RenderTexture.active = temporary2;
			RenderTexture.ReleaseTemporary(temporary);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)this.SnapshotSize, (float)this.SnapshotSize), 0, 0, false);
			texture2D.Apply(false, false);
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(this.SnapshotSize, num / 2, 16, RenderTextureFormat.ARGB32);
			BasePlatform.ActivePlatform.PrepareRenderTexture(temporary3);
			this._currentCamera.targetTexture = temporary3;
			this._currentCamera.Render();
			this._currentCamera.targetTexture = null;
			RenderTexture.active = temporary3;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)this.SnapshotSize, (float)this.SnapshotSize), 0, 0, false);
			texture2D.Apply(false, false);
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary3);
		}
		this.CacheSnapshotOnDisk(carGarageInstance, texture2D);
		return texture2D;
	}

	private Texture2D RenderSnapshotAndCache(CarGarageInstance carGarageInstance)
	{
		int height = (int)((float)this.SnapshotSize * this._aspectRatio);
		RenderTexture active = RenderTexture.active;
		RenderTexture temporary = RenderTexture.GetTemporary(this.SnapshotSize, height, 16, RenderTextureFormat.ARGB32);
		temporary.antiAliasing = 4;
		BasePlatform.ActivePlatform.PrepareRenderTexture(temporary);
		this._currentCamera.targetTexture = temporary;
		this._currentCamera.Render();
		this._currentCamera.targetTexture = null;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(this.SnapshotSize, height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.SnapshotSize, (float)this.SnapshotSize), 0, 0);
		texture2D.Apply(false, false);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		this.CacheSnapshotOnDisk(carGarageInstance, texture2D);
		return texture2D;
	}

	private void CacheSnapshotOnDisk(CarGarageInstance carGarageInstance, Texture2D texture)
	{
		string snapshotCacheName = this.GetSnapshotCacheName(carGarageInstance);
		string text = string.Empty;
		try
		{
			text = Path.Combine(this.CachePath, snapshotCacheName);
			File.WriteAllBytes(text, texture.EncodeToPNG());
			BasePlatform.ActivePlatform.AddSkipBackupAttributeToItem(text);
		}
		catch (Exception) // var_2_38)
		{
		    // ignored
		}
	}

	private string GetSnapshotCacheName(CarGarageInstance carGarageInstance)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string value = carGarageInstance.CarDBKey;
		CarInfo car = CarDatabase.Instance.GetCar(carGarageInstance.CarDBKey);
		if (carGarageInstance.IsEliteLiveryApplied && !string.IsNullOrEmpty(car.EliteOverrideCarAssetID))
		{
			value = car.EliteOverrideCarAssetID;
		}
		stringBuilder.Append(value);
		if (!string.IsNullOrEmpty(carGarageInstance.AppliedLiveryName))
		{
			string[] array = carGarageInstance.AppliedLiveryName.Split(new char[]
			{
				'_'
			});
			string arg = array[array.Length - 1];
			stringBuilder.AppendFormat("_{0}", arg);
		}
		string text = string.Empty;
		if (carGarageInstance.NumberPlate != null)
		{
			text = carGarageInstance.NumberPlate.Text;
		}
		text = text.Replace(' ', '_');
		stringBuilder.AppendFormat("_c{0}_s{1}_n{2}", 0/*carGarageInstance.AppliedColourIndex*/, this.SnapshotSize, text);
		stringBuilder.Append(GetSnapshotTypeSuffix(this.SnapshotType));
		stringBuilder.Append(this._snapshotVersionFileNameText);
		stringBuilder.Append(".png");
		return stringBuilder.ToString();
	}

	public void Update()
	{
		bool flag = BaseDevice.ActiveDevice.IsLowMemoryDevice();
		bool flag2 = false;
		for (int i = 0; i < this._asyncJobs.Count; i++)
		{
			if (flag && flag2)
			{
				return;
			}
			AsyncJob asyncJob = this._asyncJobs[i];
			if (asyncJob.AsyncLoader != null)
			{
				if (asyncJob.AsyncLoader.isDone)
				{
					Texture2D texture2D = null;
					bool flag3 = string.IsNullOrEmpty(asyncJob.AsyncLoader.error);
					if (flag3)
					{
						texture2D = asyncJob.AsyncLoader.textureNonReadable;
						texture2D.filterMode = FilterMode.Trilinear;
					}
					asyncJob.Callback(texture2D, flag3);
					this._asyncJobs.RemoveAt(i);
					i--;
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				asyncJob.AsyncLoader = new WWW(asyncJob.FilePath);
				asyncJob.AsyncLoader.threadPriority = ThreadPriority.Low;
				flag2 = true;
			}
		}
	}

	public void AsyncLoadSnapshot(CarGarageInstance carGarageInstance, ImageLoaded loadedCallback)
	{
		string snapshotCachePath = this.GetSnapshotCachePath(carGarageInstance);
		if (!FileUtils.Exists(snapshotCachePath))
		{
			loadedCallback(null, false);
			return;
		}
		string filePath = "file://" + FileUtils.GetLocalStorageFilePath(snapshotCachePath);
		AsyncJob asyncJob = new AsyncJob();
		asyncJob.Callback = loadedCallback;
		asyncJob.FilePath = filePath;
		this._asyncJobs.Add(asyncJob);
	}

	public void KillAsyncLoad(ImageLoaded callbackToMatch)
	{
		for (int i = 0; i < this._asyncJobs.Count; i++)
		{
			if (this._asyncJobs[i].Callback == callbackToMatch)
			{
				if (this._asyncJobs[i].AsyncLoader != null)
				{
					this._asyncJobs[i].AsyncLoader.Dispose();
				}
				this._asyncJobs.RemoveAt(i);
				i--;
			}
		}
	}




    public void SetupBodyMaterial(CarVisuals currentCarVisuals,string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultBodyShaderID;
        var material = CarReflectionMapManager.GetCarBodyMaterial(item, CarReflectionMapManager.ReflectionTexType.Showroom);
        currentCarVisuals.BodyMaterial = material;

        if (currentCarVisuals.HaveAdditionalRenderer)
        {
            var additionalMaterial = CarReflectionMapManager.GetCarBodyMaterial(currentCarVisuals.AdditionalShaderID, CarReflectionMapManager.ReflectionTexType.WorkshopNight);
            currentCarVisuals.AdditionalMaterial = additionalMaterial;
        }
    }

    public void SetupRingMaterial(CarVisuals currentCarVisuals, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultRingShaderID;
        var material = CarReflectionMapManager.GetCarRingMaterial(item, CarReflectionMapManager.ReflectionTexType.Showroom);
        currentCarVisuals.RingMaterial = material;
    }

    public void SetupHeadlightMaterial(CarVisuals currentCarVisuals, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = currentCarVisuals.DefaultHeadlightShaderID;
        var material = CarReflectionMapManager.GetCarHeadlightMaterial(item, CarReflectionMapManager.ReflectionTexType.Showroom);
        currentCarVisuals.HeadLightMaterial = material;
    }

    public void SetupSticker(CarVisuals currentCarVisuals, string carID, string item)
    {
        Texture2D sticker;
        Vector2 texScale;
        if (string.IsNullOrEmpty(item) || item.ToLower() == "sticker_no")
        {
            texScale = Vector2.one;
            sticker = CarReflectionMapManager.GetSharedItemID<Texture2D>(item, ServerItemBase.AssetType.sticker, CarReflectionMapManager.ReflectionTexType.Showroom);
        }
        else
        {
            CarReflectionMapManager.GetCarStickerTexture(carID, item, CarReflectionMapManager.ReflectionTexType.Showroom, out sticker, out texScale);
        }
        currentCarVisuals.CacheStickerScale(texScale);
        currentCarVisuals.Sticker = sticker;
    }

    public void SetupSpoiler(CarVisuals currentCarVisuals, string item)
    {
        if (string.IsNullOrEmpty(item) || item.ToLower() == "spoiler_no")
        {
            currentCarVisuals.ClearSpoiler();
        }
        else
        {
            var spoiler = CarReflectionMapManager.GetSharedItemID<GameObject>(item, ServerItemBase.AssetType.spoiler, CarReflectionMapManager.ReflectionTexType.Showroom);

            if (spoiler == null)
            {
                GTDebug.LogWarning(GTLogChannel.CarSnapshot, string.Format("No spoiler found width ID '{0}'", item));
                return;
            }
            var spoilerInstance = Instantiate(spoiler);
            currentCarVisuals.SetSpoiler(item, spoilerInstance);
        }
    }

    public void SetupHeight(CarVisuals currentCarVisuals, float height)
    {
        currentCarVisuals.BodyHeight = currentCarVisuals.DefaultBodyHeight;
    }
}
