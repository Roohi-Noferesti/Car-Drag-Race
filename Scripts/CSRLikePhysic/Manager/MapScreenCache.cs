using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public static class MapScreenCache
{
    private static RawImage[] _mapSprites;

    public static GameObject _singleplayerMapObject;

    public static GameObject _worldTourMapObject;

    private static Image[] _mapEdgeSprites;

    private static Texture _backupTierxTexture;

    //private static TierLockedGraphic[] _tierLocked;

    public static RawImage TierXMapSprite;

	public static GameObject MultiplayerGO;

	public static GameObject FriendsLeaderboardGO;

	public static GameObject FriendsInviteScreenGO;

	public static GameObject InternationalEventHubGO;

    public static WorldTourBossPinProgression WorldTourBossPin;

    private static Camera m_mapCamera;

    public static GameObject Map
	{
		get;
		private set;
	}

    public static Canvas MapCanvas
    {
        get; private set; }

	public static void Load()
	{
		if (Map != null)
		{
			return;
		}
        Map = UnityEngine.Object.Instantiate(ResourceManager.GetAsset<GameObject>("Career/Map_Root"));
	    m_mapCamera = Map.GetComponentInChildren<Camera>();
	    _singleplayerMapObject = CreateMapObject("Career/MapTier1");
	    _worldTourMapObject = CreateMapObject("Career/MapTierX");
        //MapScreenCache._backupTierxTexture = MapScreenCache.TierXMapSprite.texture;//.GetComponent<Renderer>().material.GetTexture("_MainTex");
        WorldTourBossPin = _worldTourMapObject.GetComponentInChildren<WorldTourBossPinProgression>();
        WorldTourBossPin.DisablePinPieces();
        UnityEngine.Object.DontDestroyOnLoad(Map);
        Map.SetActive(false);
	}


    private static GameObject CreateMapObject(string prefab)
    {

        UnityEngine.Object mapPrefab = Resources.Load(prefab);
        GameObject mapInstance = UnityEngine.Object.Instantiate(mapPrefab) as GameObject;
        //MapScreenCache._mapSprites[i] = mapInstance.GetComponentInChildren<RawImage>();
        mapInstance.transform.SetParent(Map.transform, false);
        mapInstance.gameObject.SetActive(false);
        return mapInstance;
    }

    public static Transform GetMapTransform(int zTierIndex)
	{
	    if (zTierIndex == -1)
		{
			return MultiplayerGO.transform;
		}
		//int num = zTierIndex + 1;
		//return MapScreenCache._mapSprites[num].transform;
	    return null;
	}

    private static void SetTierXBackgroundScale(float scale)
	{
        //float num = GUICamera.Instance.ScreenWidth * scale;
        //float num2 = GUICamera.Instance.ScreenHeight * scale;
        //MapScreenCache.TierXMapSprite.SetSize(num, num2);
        //Vector3 tierPositionOffset = MapScreenCache.GetTierPositionOffset(5);
        //tierPositionOffset.y = -0.5f * num2 * (1f - scale);
        //tierPositionOffset.x = 0.5f * num * (1f - scale) - tierPositionOffset.x;
        //tierPositionOffset.z += 1f;
        //MapScreenCache.TierXMapSprite.transform.localPosition = tierPositionOffset;
	}

	public static void ChangeTierXBackground(Texture texture = null, float scale = 1f)
	{
		if (texture == null)
		{
			return;
		}
		TierXMapSprite.GetComponent<Renderer>().material.shader = Shader.Find("CSR Gui/GuiGradientScreenRadialOpaque (Texture)");
		TierXMapSprite.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
		SetTierXBackgroundScale(scale);
	}

	public static void SetDefaultTierXBackground(float scale = 1f)
	{
		TierXMapSprite.GetComponent<Renderer>().material.shader = Shader.Find("CSR Gui/GuiGradientScreenRadialOpaque (Alpha-8)");
		//MapScreenCache.TierXMapSprite.GetComponent<Renderer>().material.SetTexture("_MainTex", MapScreenCache._backupTierxTexture);
		SetTierXBackgroundScale(scale);
	}

	public static void DestroyInternationalBackground()
	{
		if (InternationalEventHubGO != null)
		{
			UnityEngine.Object.Destroy(InternationalEventHubGO);
			InternationalEventHubGO = null;
		}
	}

	public static void OnEnterMap(float zPaneWidth)
	{
        //for (int i = 0; i < MapPanes; i++)
        //{
            //TierLockedGraphic tierLockedGraphic = MapScreenCache._tierLocked[i];
            //if (!(tierLockedGraphic == null))
            //{
            //    if (i == 0)
            //    {
            //        tierLockedGraphic.gameObject.SetActive(false);
            //    }
            //    else
            //    {
            //        eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
            //        bool flag = i - 1 > (int)highestUnlockedClass;
            //        bool flag2 = i - 1 == 5 && !GameDatabase.Instance.Career.IsWorldTourUnlocked();
            //        if (flag || flag2)
            //        {
            //            tierLockedGraphic.gameObject.SetActive(true);
            //            int num = i - 1;
            //            string title = LocalizationManager.GetTranslation("TEXT_TIER_" + CarTierHelper.TierToString[num]);
            //            string subTitle = LocalizationManager.GetTranslation("TEXT_TIER_LOCKED_" + CarTierHelper.TierToString[num]);
            //            tierLockedGraphic.Setup(title, subTitle, num);
            //            //tierLockedGraphic.transform.localPosition = new Vector3((GUICamera.Instance.ScreenWidth - zPaneWidth) * 0.5f, -GUICamera.Instance.ScreenWidth * 0.25f, -0.1f);
            //        }
            //        else
            //        {
            //            tierLockedGraphic.gameObject.SetActive(false);
            //        }
            //    }
            //}
        //}
        
    }

	public static Vector3 GetTierPositionOffset(int tier)
	{
	    //return new Vector3((float)tier * -1f * GUICamera.Instance.ScreenWidth, 0f, 0f);
	    return new Vector3();
	}

    public static void Unload()
	{
		if (Map == null)
		{
			return;
		}
        //for (int i = 0; i < _mapSprites.Length; i++)
        //{
        //    if (!(_mapSprites[i] == null) && !(_mapSprites[i].GetComponent<Renderer>() == null))
        //    {
        //        UnityEngine.Object.Destroy(_mapSprites[i].GetComponent<Renderer>().material);
        //        UnityEngine.Object.Destroy(_mapSprites[i].gameObject);
        //        _mapSprites[i] = null;
        //    }
        //}
        _mapSprites = null;
        UnityEngine.Object.Destroy(MultiplayerGO);
        MultiplayerGO = null;
        UnityEngine.Object.Destroy(FriendsLeaderboardGO);
        FriendsLeaderboardGO = null;
        //for (int j = 0; j < _mapEdgeSprites.Length; j++)
        //{
        //    UnityEngine.Object.Destroy(_mapEdgeSprites[j].GetComponent<Renderer>().material);
        //    UnityEngine.Object.Destroy(_mapEdgeSprites[j].gameObject);
        //    _mapEdgeSprites[j] = null;
        //}
        _mapEdgeSprites = null;
        UnityEngine.Object.Destroy(Map);
        Map = null;
        MapCanvas = null;
		CleanDownManager.Instance.OnMapScreenCacheReleased();
	}

    public static void SetMap(MapPaneType mapPaneType)
    {
        switch (mapPaneType)
        {
            case MapPaneType.SinglePlayer:
                _singleplayerMapObject.SetActive(true);
                _worldTourMapObject.SetActive(false);
                MapCanvas = _singleplayerMapObject.GetComponentInChildren<Canvas>();
                break;
            case MapPaneType.WorldTour:
                _singleplayerMapObject.SetActive(false);
                _worldTourMapObject.SetActive(true);
                MapCanvas = _worldTourMapObject.GetComponentInChildren<Canvas>();
                break;
        }
        MapCanvas.worldCamera = m_mapCamera;
    }

    internal static void UpdateWorldTourLogo()
    {
        _worldTourMapObject.transform.Find("CrewLogo").GetComponent<RawImage>().texture =
	        (BuildType.IsAppTuttiBuild && TierXManager.Instance.ThemeDescriptorPrefab.ProgressTextures.CrewBackgroundIcon_AppTutti!=null)?
		        TierXManager.Instance.ThemeDescriptorPrefab.ProgressTextures.CrewBackgroundIcon_AppTutti:    
		        TierXManager.Instance.ThemeDescriptorPrefab.ProgressTextures.CrewBackgroundIcon;
    }
}

public enum MapPaneType
{
    SinglePlayer,
    WorldTour
}
