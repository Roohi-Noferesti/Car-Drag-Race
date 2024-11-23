using UnityEngine;

public static class CarReflectionMapManager
{
	public enum ReflectionTexType
	{
		Showroom,
		WorkshopDay,
		WorkshopNight,
		RaceNight,
        RaceDay,
        RaceCloudy,
        RaceAirport,
		MAX
	}

	private static string[] _lowMemoryCubemaps;

	private static string[] _highMemoryCubemaps;

	private static string[] _raceScrollReflectionTextures;

    private static string[] _blackShadeMaterial;

    private static string[] _glassMaterial;

    private static string[] _headlightGlassMaterial;

    private static string[] _textureOnlyCubemaps;

    private static string[] _glowHeadlightTexture;

    private static string[] _lighProjectionTexture;

    private static string[] _bodyMaterialMiddleName;

    private static string[] _ringMaterialPostFix;


	private static Cubemap[] _loadedCubemaps;

	private static Texture2D[] _loadedScrollReflectionTextures;

    private static Material[] _loadedBlackShadeMaterial;

    private static Material[] _loadedGlassMaterial;

    private static Material[] _loadedHeadlightGlassMaterial;

    private static Cubemap[] _loadedTextureOnlyCubemaps;

    private static Texture2D[] _loadedGlowHeadlighTexture;

    private static Texture2D[] _loadedLightProjectionTexture;




	static CarReflectionMapManager()
	{
		_lowMemoryCubemaps = new string[7];
		_highMemoryCubemaps = new string[7];
		_raceScrollReflectionTextures = new string[7];
        _blackShadeMaterial = new string[7];
        _glassMaterial = new string[7];
        _headlightGlassMaterial = new string[7];
        _textureOnlyCubemaps = new string[7];
        _glowHeadlightTexture = new string[7];
        _lighProjectionTexture = new string[7];
        _bodyMaterialMiddleName = new string[7];
        _ringMaterialPostFix = new string[7];

		_loadedCubemaps = new Cubemap[7];
        _loadedScrollReflectionTextures = new Texture2D[7];
        _loadedBlackShadeMaterial = new Material[7];
        _loadedGlassMaterial = new Material[7];
        _loadedHeadlightGlassMaterial = new Material[7];
        _loadedTextureOnlyCubemaps = new Cubemap[7];
        _loadedGlowHeadlighTexture = new Texture2D[7];
        _loadedLightProjectionTexture = new Texture2D[7];

		_lowMemoryCubemaps[0] = "CarReflectionTextures/Showroom_Reflection_Low";
		_lowMemoryCubemaps[1] = "CarReflectionTextures/Garage_Day_Reflection_Low";
		_lowMemoryCubemaps[2] = "CarReflectionTextures/Garage_Night_Reflection_Low";
		_lowMemoryCubemaps[3] = "CarReflectionTextures/Race_Downtown_Reflection";
		_lowMemoryCubemaps[4] = "CarReflectionTextures/Race_SantaMonica_Reflection";
		_highMemoryCubemaps[0] = "CarReflectionTextures/Showroom_Reflection_High";
		_highMemoryCubemaps[1] = "CarReflectionTextures/Garage_Day_Reflection_High";
		_highMemoryCubemaps[2] = "CarReflectionTextures/Garage_Night_Reflection_High";
		_highMemoryCubemaps[3] = "CarReflectionTextures/Race_Downtown_Reflection";
		_highMemoryCubemaps[4] = "CarReflectionTextures/Race_SantaMonica_Reflection";
		_raceScrollReflectionTextures[3] = "CarReflectionTextures/Race_Downtown_Scroll";
		_raceScrollReflectionTextures[4] = "CarReflectionTextures/Race_Downtown_Scroll";
        _blackShadeMaterial[0] = "shared_assets/car_shared_shader/Black_Shade_Shared";
        _blackShadeMaterial[1] = "shared_assets/car_shared_shader/Black_Shade_Shared";
        _blackShadeMaterial[2] = "shared_assets/car_shared_shader/Black_Shade_Shared";
        _blackShadeMaterial[3] = "shared_assets/car_shared_shader/Black_Shade_Shared";
        _blackShadeMaterial[4] = "shared_assets/car_shared_shader/Black_Shade_Shared_Day";
        _blackShadeMaterial[5] = "shared_assets/car_shared_shader/Black_Shade_Shared_Day";
        _blackShadeMaterial[6] = "shared_assets/car_shared_shader/Black_Shade_Shared_Day";
        _glassMaterial[0] = "shared_assets/car_shared_shader/Car_Glass_ShowRoom_Shared";
        _glassMaterial[1] = "shared_assets/car_shared_shader/Car_Glass_Garage_Shared";
        _glassMaterial[2] = "shared_assets/car_shared_shader/Car_Glass_Garage_Shared";
        _glassMaterial[3] = "shared_assets/car_shared_shader/Car_Glass_Street_Shared";
        _glassMaterial[4] = "shared_assets/car_shared_shader/Car_Glass_Street_Shared_Day";
        _glassMaterial[5] = "shared_assets/car_shared_shader/Car_Glass_Street_Shared_Day";
        _glassMaterial[6] = "shared_assets/car_shared_shader/Car_Glass_Street_Shared_Day";
        _headlightGlassMaterial[0] = "shared_assets/car_shared_shader/Headlight_Glass_Night_Shared";
        _headlightGlassMaterial[1] = "shared_assets/car_shared_shader/Headlight_Glass_Night_Shared";
        _headlightGlassMaterial[2] = "shared_assets/car_shared_shader/Headlight_Glass_Night_Shared";
        _headlightGlassMaterial[3] = "shared_assets/car_shared_shader/Headlight_Glass_Night_Shared";
        _headlightGlassMaterial[4] = "shared_assets/car_shared_shader/Headlight_Glass_Shared_Day";
        _headlightGlassMaterial[5] = "shared_assets/car_shared_shader/Headlight_Glass_Shared_Day";
        _headlightGlassMaterial[6] = "shared_assets/car_shared_shader/Headlight_Glass_Shared_Day";
        _textureOnlyCubemaps[0] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[1] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[2] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[3] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[4] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[5] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _textureOnlyCubemaps[6] = "ReflectionMap/StreetDay_RefLmap_Sharp";
        _glowHeadlightTexture[0] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[1] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[2] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[3] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[4] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[5] = "CarSharedTexture/Glow_HeadLight";
        _glowHeadlightTexture[6] = "CarSharedTexture/Glow_HeadLight";
        _lighProjectionTexture[0] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[1] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[2] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[3] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[4] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[5] = "CarSharedTexture/Front_Light_Cookie";
        _lighProjectionTexture[6] = "CarSharedTexture/Front_Light_Cookie";
        _bodyMaterialMiddleName[0] = "_ShowRoom";
        _bodyMaterialMiddleName[1] = "_Garage";
        _bodyMaterialMiddleName[2] = "_Garage";
        _bodyMaterialMiddleName[3] = "_StreetNight";
        _bodyMaterialMiddleName[4] = "_StreetDay";
        _bodyMaterialMiddleName[5] = "_StreetDay";
        _bodyMaterialMiddleName[6] = "_StreetDay_Online";
        _ringMaterialPostFix[0] = "";
        _ringMaterialPostFix[1] = "";
        _ringMaterialPostFix[2] = "";
        _ringMaterialPostFix[3] = "";
        _ringMaterialPostFix[4] = "_Day";
        _ringMaterialPostFix[5] = "_Day";
        _ringMaterialPostFix[6] = "_Day";

	}

    #region Refection Map
    public static bool LoadCubemap(ReflectionTexType type)
	{
		if (_loadedCubemaps[(int)type] == null)
		{
			string cubemapName = GetCubemapName(type);
			Cubemap cubemap;
			try
			{
				cubemap = (Resources.Load(cubemapName) as Cubemap);
			}
			catch
			{
				cubemap = null;
			}
			if (cubemap == null)
			{
				return false;
			}
			_loadedCubemaps[(int)type] = cubemap;
		}
		return true;
	}

	public static bool UnloadCubemap(ReflectionTexType type)
	{
		if (_loadedCubemaps[(int)type] != null)
		{
			//string cubemapName = GetCubemapName(type);
			Resources.UnloadAsset(_loadedCubemaps[(int)type]);
			_loadedCubemaps[(int)type] = null;
		}
		return true;
	}

	public static Cubemap GetLoadedCubemap(ReflectionTexType type)
	{
		if (_loadedCubemaps[(int)type] == null)
		{
			LoadCubemap(type);
		}
		return _loadedCubemaps[(int)type];
	}

	private static string GetCubemapName(ReflectionTexType type)
	{
		if (BaseDevice.ActiveDevice.IsLowMemoryDevice() || BaseDevice.ActiveDevice.DeviceQuality == AssetQuality.Low)
		{
			return _lowMemoryCubemaps[(int)type];
		}
		return _highMemoryCubemaps[(int)type];
	}

    #endregion


    #region ScrollText
    public static bool LoadScrollTex(ReflectionTexType type)
	{
		if (_loadedScrollReflectionTextures[(int)type] == null)
		{
			string path = _raceScrollReflectionTextures[(int)type];
			Texture2D texture2D = Resources.Load(path) as Texture2D;
			if (texture2D == null)
			{
				return false;
			}
			_loadedScrollReflectionTextures[(int)type] = texture2D;
		}
		return true;
	}

	public static bool UnloadScrollTex(ReflectionTexType type)
	{
		if (_loadedScrollReflectionTextures[(int)type] != null)
		{
			//string text = _raceScrollReflectionTextures[(int)type];
			Resources.UnloadAsset(_loadedScrollReflectionTextures[(int)type]);
			_loadedScrollReflectionTextures[(int)type] = null;
		}
		return true;
	}

	public static Texture2D GetLoadedScrollTex(ReflectionTexType type)
	{
		if (_loadedScrollReflectionTextures[(int)type] == null)
		{
			LoadScrollTex(type);
		}
		return _loadedScrollReflectionTextures[(int)type];
	}

    #endregion


    #region BlackShade
    public static bool LoadBlackShadeMaterial(ReflectionTexType type)
    {
        if (_loadedBlackShadeMaterial[(int)type] == null)
        {
            string path = _blackShadeMaterial[(int)type];
            Material material = Resources.Load(path) as Material;
            if (material == null)
            {
                return false;
            }
            _loadedBlackShadeMaterial[(int)type] = material;
        }
        return true;
    }

    public static bool UnloadBlackShadeMaterial(ReflectionTexType type)
    {
        if (_loadedBlackShadeMaterial[(int)type] != null)
        {
            //string text = _blackShadeMaterial[(int)type];
            Resources.UnloadAsset(_loadedBlackShadeMaterial[(int)type]);
            _loadedBlackShadeMaterial[(int)type] = null;
        }
        return true;
    }

    public static Material GetLoadedBlackShadeMaterial(ReflectionTexType type)
    {
        if (_loadedBlackShadeMaterial[(int)type] == null)
        {
            LoadBlackShadeMaterial(type);
        }
        return _loadedBlackShadeMaterial[(int)type];
    }

    #endregion



    #region Glass
    public static bool LoadGlassMaterial(ReflectionTexType type)
    {
        if (_loadedGlassMaterial[(int)type] == null)
        {
            string path = _glassMaterial[(int)type];
            Material material = Resources.Load(path) as Material;
            if (material == null)
            {
                return false;
            }
            _loadedGlassMaterial[(int)type] = material;
        }
        return true;
    }

    public static bool UnloadGlassMaterial(ReflectionTexType type)
    {
        if (_loadedGlassMaterial[(int)type] != null)
        {
            //string text = _glassMaterial[(int)type];
            Resources.UnloadAsset(_loadedGlassMaterial[(int)type]);
            _loadedGlassMaterial[(int)type] = null;
        }
        return true;
    }

    public static Material GetLoadedGlassMaterial(ReflectionTexType type)
    {
        if (_loadedGlassMaterial[(int)type] == null)
        {
            LoadGlassMaterial(type);
        }
        return _loadedGlassMaterial[(int)type];
    }

    #endregion


    #region HeadlightGlass
    public static bool LoadHeadlightGlassMaterial(ReflectionTexType type)
    {
        if (_loadedHeadlightGlassMaterial[(int)type] == null)
        {
            string path = _headlightGlassMaterial[(int)type];
            Material material = Resources.Load(path) as Material;
            if (material == null)
            {
                return false;
            }
            _loadedHeadlightGlassMaterial[(int)type] = material;
        }
        return true;
    }

    public static bool UnloadHeadlightGlassMaterial(ReflectionTexType type)
    {
        if (_loadedHeadlightGlassMaterial[(int)type] != null)
        {
            //string text = _headlightGlassMaterial[(int)type];
            Resources.UnloadAsset(_loadedHeadlightGlassMaterial[(int)type]);
            _loadedHeadlightGlassMaterial[(int)type] = null;
        }
        return true;
    }

    public static Material GetLoadedHeadlightGlassMaterial(ReflectionTexType type)
    {
        if (_loadedHeadlightGlassMaterial[(int)type] == null)
        {
            LoadHeadlightGlassMaterial(type);
        }
        return _loadedHeadlightGlassMaterial[(int)type];
    }

    #endregion


    #region TextureOnly
    public static bool LoadTextureOnlyCubemaps(ReflectionTexType type)
    {
        if (_loadedTextureOnlyCubemaps[(int)type] == null)
        {
            string path = _textureOnlyCubemaps[(int)type];
            Cubemap material = Resources.Load(path) as Cubemap;
            if (material == null)
            {
                return false;
            }
            _loadedTextureOnlyCubemaps[(int)type] = material;
        }
        return true;
    }

    public static bool UnloadTextureOnlyCubemaps(ReflectionTexType type)
    {
        if (_loadedTextureOnlyCubemaps[(int)type] != null)
        {
            //string text = _textureOnlyCubemaps[(int)type];
            Resources.UnloadAsset(_loadedTextureOnlyCubemaps[(int)type]);
            _loadedTextureOnlyCubemaps[(int)type] = null;
        }
        return true;
    }

    public static Cubemap GetLoadedTextureOnlyCubemaps(ReflectionTexType type)
    {
        if (_loadedTextureOnlyCubemaps[(int)type] == null)
        {
            LoadTextureOnlyCubemaps(type);
        }
        return _loadedTextureOnlyCubemaps[(int)type];
    }

    #endregion



    #region Glow Headlight
    public static bool LoadGlowHeadlightTexture(ReflectionTexType type)
    {
        if (_loadedGlowHeadlighTexture[(int)type] == null)
        {
            string path = _glowHeadlightTexture[(int)type];
            Texture2D texture = Resources.Load(path) as Texture2D;
            if (texture == null)
            {
                return false;
            }
            _loadedGlowHeadlighTexture[(int)type] = texture;
        }
        return true;
    }

    public static bool UnloadGlowHeadlightTexture(ReflectionTexType type)
    {
        if (_loadedGlowHeadlighTexture[(int)type] != null)
        {
            //string text = _glowHeadlightTexture[(int)type];
            Resources.UnloadAsset(_loadedGlowHeadlighTexture[(int)type]);
            _loadedGlowHeadlighTexture[(int)type] = null;
        }
        return true;
    }

    public static Texture2D GetLoadedGlowHeadlightTexture(ReflectionTexType type)
    {
        if (_loadedGlowHeadlighTexture[(int)type] == null)
        {
            LoadGlowHeadlightTexture(type);
        }
        return _loadedGlowHeadlighTexture[(int)type];
    }

    #endregion



    #region Light Projection
    public static bool LoadLightProjectionTexture(ReflectionTexType type)
    {
        if (_loadedLightProjectionTexture[(int)type] == null)
        {
            string path = _lighProjectionTexture[(int)type];
            Texture2D texture = Resources.Load(path) as Texture2D;
            if (texture == null)
            {
                return false;
            }
            _loadedLightProjectionTexture[(int)type] = texture;
        }
        return true;
    }

    public static bool UnloadLightProjectionTexture(ReflectionTexType type)
    {
        if (_loadedLightProjectionTexture[(int)type] != null)
        {
            //string text = _lighProjectionTexture[(int)type];
            Resources.UnloadAsset(_loadedLightProjectionTexture[(int)type]);
            _loadedLightProjectionTexture[(int)type] = null;
        }
        return true;
    }

    public static Texture2D GetLoadedLightProjectionTexture(ReflectionTexType type)
    {
        if (_loadedLightProjectionTexture[(int)type] == null)
        {
            LoadLightProjectionTexture(type);
        }
        return _loadedLightProjectionTexture[(int)type];
    }

    #endregion


    #region Body Material
    public static Material GetCarBodyMaterial(string bodyShaderID,ReflectionTexType type)
    {
        string middlename = _bodyMaterialMiddleName[(int) type];
        bodyShaderID = bodyShaderID.Insert(7, middlename);
        return ResourceManager.GetSharedAsset<Material>(bodyShaderID,ServerItemBase.AssetType.body_shader);
    }

    #endregion

    public static Material GetCarRingMaterial(string item, ReflectionTexType type)
    {
        string postFix = _ringMaterialPostFix[(int)type];
        item = item.Insert(4, postFix);
        //Debug.Log(item);
        return ResourceManager.GetSharedAsset<Material>(item, ServerItemBase.AssetType.ring_shader);
    }

    public static Material GetCarHeadlightMaterial(string item, ReflectionTexType type)
    {
        return ResourceManager.GetSharedAsset<Material>(item, ServerItemBase.AssetType.headlight_shader);
    }

    public static void GetCarStickerTexture(string carID,string item, ReflectionTexType type, out Texture2D sticker,
        out Vector2 scale)
    {
	    foreach (var VARIABLE in CarDatabase.Instance.GetCar(carID).Stickers)
	    {
		    //Debug.LogError(carID + " " + VARIABLE);
	    }
	    
        int stickerNumber = int.Parse(item.ToLower().Replace("sticker_", ""));

        if (stickerNumber < 40)
        {
            scale = new Vector2(1, 1);
            sticker = ResourceManager.GetCarAsset<Texture2D>(carID, ServerItemBase.AssetType.sticker, item);
            
            if (
	            (
		            carID.ToLower().Contains("bentley_gt_v8") ||
		            carID.ToLower().Contains("ford_focus_rs") ||
		            carID.ToLower().Contains("honda_civic_type_r") ||
		            carID.ToLower().Contains("jaguar_ftype") ||
		            carID.ToLower().Contains("jaguar_xfr") ||
		            carID.ToLower().Contains("lexus_is_f") ||
		            carID.ToLower().Contains("benz_a_250") ||
		            carID.ToLower().Contains("minicooper_s") ||
		            carID.ToLower().Contains("tesla_model_s") ||
		            carID.ToLower().Contains("toyota_gt")
	            ) && !CarDatabase.Instance.GetCar(carID).Stickers.Contains(stickerNumber))
            {
	            sticker = null;
            }
        }
        else
        {
            scale = new Vector2(2, 2);
            sticker = ResourceManager.GetSharedAsset<Texture2D>(item, ServerItemBase.AssetType.sticker);
        }
    }

    public static T GetSharedItemID<T>(string item,ServerItemBase.AssetType assetType, ReflectionTexType reflType) where T : Object
    {
        return ResourceManager.GetSharedAsset<T>(item, assetType);
    }
}
