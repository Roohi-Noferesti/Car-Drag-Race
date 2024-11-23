using System.Collections.Generic;
using System.IO;
using ChartboostSDK;
using FlurrySDK;
using I2.Loc;
using KingKodeStudio;
using Objectives;
using UnityEditor;
using UnityEngine;

public class Z2HInitialisers
{
	public static GameObject managerGo;
	
	private static bool doneForceUpdate = false;

	private static string downloadUrl = null;

	public static List<Z2HIDInfo> initialisers = new List<Z2HIDInfo>
	{
        new Z2HIDInfo(MakeFlurry, false, "Manager Flurry"),
        new Z2HIDInfo(MakeManagerGameObject, false, "Manager Game Object creation"),
        new Z2HIDInfo(MakeCallbackHandler, false, "MakeCallbackHandler"),
        new Z2HIDInfo(MakeFreshChatHandler, false, "MakeFreshChatHandler"),
        new Z2HIDInfo(MakeUICamera, false, "Makeing Ui Camera"),
        new Z2HIDInfo(MakeResourceManager, false, "Making Resource Manager Object"),
        //new Z2HIDInfo(MakeClientConnectionManager, false, "Making Client Connection Manager"),
        new Z2HIDInfo(MakePopupManager, false, "Making Popup Manager"),
        new Z2HIDInfo(MakeZ2HApplicationManager, false, "Making Application Manager"),
        new Z2HIDInfo(MakePreUI, false, "MakePreUI"),
        new Z2HIDInfo(PlatformSpecificSetup, false, "PlatformSpecificSetup"),
        new Z2HIDInfo(MakePlayerProfileManager, false, "MakePlayerProfileManager"),
        new Z2HIDInfo(MakeCheapGameObjects1, false, "Making Cheap Objects(1)"),
        new Z2HIDInfo(MakeUserManager, true, "MakeUserManager"),
        new Z2HIDInfo(MakeCheapGameObjects2, false, "Making Cheap Objects(2)"),
        new Z2HIDInfo(MakeGameDatabase, false, "Making Game Database"),
        new Z2HIDInfo(MakeCheapGameObjects3, false, "Making Cheap Objects(3)"),
        new Z2HIDInfo(MakeUI, true, "MakeUI"),
        new Z2HIDInfo(MakeCheapGameObjects4, false, "Making Cheap Objects(4)"),
        new Z2HIDInfo(MakeCheapGameObjects7, false, "Make Cheap Objects (7)"),
        new Z2HIDInfo(MakeCheapGameObjects5, false, "Making Cheap Objects(5)"),
        new Z2HIDInfo(MakeCoroutineManager, false, "Making Coroutine Manager"),
        new Z2HIDInfo(MakeObjectiveManager, false, "Making ObjectiveManager"),
        new Z2HIDInfo(MakeMoPubIntegration, true, "MakeMoPubIntegration"),
        new Z2HIDInfo(MakeApsalarIntegration, true, "MakeApsalarIntegration"),
        new Z2HIDInfo(MakeTapjoyIntegration, true, "MakeTapjoyIntegration"),
        new Z2HIDInfo(MakeUICache, true, "MakeUICache"),
        new Z2HIDInfo(MakeRaceEventDatabase, true, "MakeRaceEventDatabase"),
        new Z2HIDInfo(MakeMetricsIntegration, true, "MakeMetricsIntegration"),
        new Z2HIDInfo(MakeRemoteConfigManager, true, "MakeRemoteConfigManager"),
        new Z2HIDInfo(MakeResourceManager, true, "MakeRemoteConfigManager"),
        new Z2HIDInfo(MakeAndroidBackgroundCRCCheck, false, "AndroidBackgroundCRCCheck"),
        new Z2HIDInfo(WarmUpShaders, true, "WarmUpShaders"),
        new Z2HIDInfo(MakeInGameTutorial, true, "MakeInGameTutorial"),
        new Z2HIDInfo(MakeCommonUI, true, "MakeCommonUI"),
        new Z2HIDInfo(MakeCarInfoUI, true, "MakeCarInfoUI"),
        new Z2HIDInfo(MakeCarGroundFlareContainer, true, "MakeCarGroundFlareComntainer"),
        new Z2HIDInfo(MakeAchievementsController, true, "MakeAchievementsController"),
        new Z2HIDInfo(MakeAdManager, true, "MakeAdManager"),
        new Z2HIDInfo(MakeOfferWallManager, true, "MakeOfferWallManager"),
        new Z2HIDInfo(MakePPEManager, true, "MakePPEManager"),
        new Z2HIDInfo(MakeNumberPlateManager, false, "MakeNumberPlateManager"),
        new Z2HIDInfo(MakeCheapGameObjects6, false, "Make Cheap Objects (6)"),
        new Z2HIDInfo(MakeSocialManagers, true, "MakeSocialManagers"),
        new Z2HIDInfo(MakeBundleOffers, false, "MakeBundleOffers"),
        new Z2HIDInfo(MakeMiniStore, false, "MakeMiniStore"),
        new Z2HIDInfo(MakeSMPNetworkManager, false, "MakeSMPNetworkManager"),
        new Z2HIDInfo(MakeCarSnapshotQueue, false, "MakeCarSnapshotQueue"),
        new Z2HIDInfo(MakeTutorialBubblesManager, false, "MakeTutorialBubbleManager"),
        new Z2HIDInfo(InitKeyboard, false, "InitKeyboard"),
        new Z2HIDInfo(MakeMenuAudio, true, "MakeMenuAudio"),
        new Z2HIDInfo(MakeAudioPrefab, true, "MakeAudioPrefab"),
        new Z2HIDInfo(ClearUnityCache, false, "Removing old data"),
        new Z2HIDInfo(MakeWebViewManager, false, "Making web view"),
        new Z2HIDInfo(MakeBazaarGameHub, false, "Make Bazaar GameHub"),
        new Z2HIDInfo(CheckForceUpdate, false, "Checking for an update"),
        new Z2HIDInfo(FinishInitialisation, false, "Checking oil levels"),
    };

    private static void MakeFreshChatHandler()
    {
        var freshchat = new GameObject("FreshChatListener").AddComponent<FreshChatListener>();
        Persist(freshchat.gameObject);
    }

    private static void MakeWebViewManager()
    {
	    var webView = Resources.Load<GameObject>("Prefabs/WebView");
	    var instance = Object.Instantiate(webView);
	    Persist(instance);
	    instance.name = "WebViewManager";
		
		
	    var goftino = managerGo.AddComponent<WebViewManager>();
	    goftino.SetWebView(instance.GetComponent<UniWebView>());
    }

    private static void MakeUserManager()
    {
        UserManager userManager = managerGo.GetComponent<UserManager>();
        if (userManager == null)
        {
            userManager = managerGo.AddComponent<UserManager>();
        }
        userManager.enabled = false;
    }

    private static void MakeUICamera()
    {
        var uiCamera = Resources.Load<GameObject>("Prefabs/UICamera");
        var instance = Object.Instantiate(uiCamera);
        Persist(instance);
        instance.name = "UICamera";
    }

    private static void MakeResourceManager()
    {
        ResourceManager resourceManager = managerGo.GetComponent<ResourceManager>();
        if (resourceManager == null)
        {
            resourceManager = managerGo.AddComponent<ResourceManager>();
        }
        managerGo.AddComponent<ResourceUtility>();
        Persist(resourceManager.gameObject);
    }

    private static void MakeClientConnectionManager()
    {
        OnlineRaceGameEvents onlineRaceGameEvents = managerGo.GetComponent<OnlineRaceGameEvents>();
        if (onlineRaceGameEvents == null)
        {
            onlineRaceGameEvents = managerGo.AddComponent<OnlineRaceGameEvents>();
        }
        Persist(onlineRaceGameEvents.gameObject);

        ClientConnectionManager connectionManager = managerGo.GetComponent<ClientConnectionManager>();
        if (connectionManager == null)
        {
            var go =
                ResourceManager.GetAsset<GameObject>("Prefabs/ClientConnectionManager");
            connectionManager = Object.Instantiate(go).GetComponent<ClientConnectionManager>();

        }
        Persist(connectionManager.gameObject);
    }

    private static void MakePopupManager()
    {
        PopUpManager popUpManager = managerGo.GetComponent<PopUpManager>();
        if (popUpManager == null)
        {
            var go =
                ResourceManager.GetAsset<GameObject>("Prefabs/PopupManager");
            popUpManager = Object.Instantiate(go).GetComponent<PopUpManager>();

        }
        Persist(popUpManager.gameObject);
    }

    private static void MakeRaceEventDatabase()
    {
        RaceEventDatabase.Create();
    }

    private static void MakeCallbackHandler()
	{
#if UNITY_ANDROID
#endif
    }
    
    private static void MakeFlurry()
    {
	    //Flurry.SetDebugLogEnabled(false);
	    //Flurry.StartSession("TDQBJQ2BDYWD6YQSH4QF", "HXGDRDYJ6B9NK8DMVN98", true);
        
#if UNITY_ANDROID
	    string FLURRY_API_KEY = "HXGDRDYJ6B9NK8DMVN98";
#elif UNITY_IPHONE
        string FLURRY_API_KEY = "TDQBJQ2BDYWD6YQSH4QF";
#else
        string FLURRY_API_KEY = null;
#endif
        
	    new Flurry.Builder()
		    .WithCrashReporting(true)
		    .WithLogEnabled(true)
		    .WithLogLevel(Flurry.LogLevel.VERBOSE)
		    .WithMessaging(true)
		    .Build(FLURRY_API_KEY);
    }

	private static void MakeManagerGameObject()
	{
        managerGo = new GameObject("ManagerGO");
        //managerGo.AddComponent<TestManagerGo>();
        Persist(managerGo);
	}

	private static void ClearUnityCache()
	{
		string localStorageFilePath = FileUtils.GetLocalStorageFilePath("LastVersionCleared");
		string applicationVersion = BasePlatform.ActivePlatform.GetApplicationVersion();
		if (!File.Exists(localStorageFilePath))
		{
			Caching.ClearCache();
			File.WriteAllText(localStorageFilePath, applicationVersion);
		}
		else
		{
			string inVersion = File.ReadAllText(localStorageFilePath);
			if (ApplicationVersion.Compare(inVersion, applicationVersion) == -1)
			{
				Caching.ClearCache();
				File.WriteAllText(localStorageFilePath, applicationVersion);
			}
		}
	}

    private static void LoadPrefabFromResources(string path)
    {
        Persist(Object.Instantiate(Resources.Load(path)) as GameObject);
    }

	private static void MakePreUI()
	{
        //managerGo.AddComponent<NavBarAnimationManager>();
	}

	private static void MakeMoPubIntegration()
	{
        //GameObject gameObject = new GameObject();
        //gameObject.AddComponent<MoPubManager>();
        //gameObject.name = typeof(MoPubManager).ToString();
        //Persist(gameObject);
	}

	private static void MakeApsalarIntegration()
	{
        //GameObject gameObject = new GameObject();
        //gameObject.AddComponent<ApsalarIntegration>();
        //gameObject.name = "ApsalarIntegration";
        //Persist(gameObject);
	}

	private static void MakeTapjoyIntegration()
	{
        //GameObject gameObject = new GameObject();
        //gameObject.AddComponent<TapjoyIntegration>();
        //gameObject.name = "TapjoyIntegration";
        //Persist(gameObject);
	}

	private static void MakePlayerProfileManager()
	{
        PlayerProfileManager playerProfileManager = managerGo.AddComponent<PlayerProfileManager>();
		playerProfileManager.enabled = false;
	}

	private static void MakeCheapGameObjects1()
	{
        managerGo.AddComponent<MemoryValidator>();
        GameObject gameObject = new GameObject("SocialGamePlatformControllers");
        Persist(gameObject);
        //SocialManager socialManager = gameObject.AddComponent<SocialManager>();
        //Persist(socialManager.gameObject);
#if UNITY_ANDROID
        GooglePlayGamesController googlePlayGamesController = gameObject.AddComponent<GooglePlayGamesController>();
        googlePlayGamesController.enabled = false;
#endif
        SocialGamePlatformSelector socialGamePlatformSelector = gameObject.AddComponent<SocialGamePlatformSelector>();
        socialGamePlatformSelector.enabled = false;
        GameCenterController gameCenterController = gameObject.AddComponent<GameCenterController>();
        gameCenterController.enabled = false;
        managerGo.AddComponent<GameCenterEventListener>();
        WebRequestQueue webRequestQueue = managerGo.AddComponent<WebRequestQueue>();
        webRequestQueue.enabled = false;
        WebRequestQueueRTW webRequestQueueRTW = managerGo.AddComponent<WebRequestQueueRTW>();
        webRequestQueueRTW.enabled = false;
        managerGo.AddComponent<WebRequestStandalone>();
        var dailyPrizeManager = managerGo.AddComponent<DailyPrizeManager>();
	    dailyPrizeManager.enabled = false;
	}

	private static void MakeCheapGameObjects2()
	{
        managerGo.AddComponent<CleanDownManager>();
        ArrivalManager arrivalManager = managerGo.AddComponent<ArrivalManager>();
        arrivalManager.enabled = false;
        managerGo.AddComponent<CarStatsCalculator>();
        managerGo.AddComponent<SceneLoadManager>();
        managerGo.AddComponent<FriendsDownloadReplayManager>();
        
        
	}

	private static void MakeBazaarGameHub()
	{
		BazaarGameHubManager.Create();
	}

	private static void MakeGameDatabase()
	{
		GameDatabase.Create();
        managerGo.AddComponent<ManufacturerDatabase>();
		managerGo.AddComponent<CarDatabase>();
	}
	
    
    private static void CheckForceUpdate()
    {
	    string platform = null;
#if UNITY_ANDROID
	    platform = "Android";
#elif UNITY_IPHONE || UNITY_IOS
	platform = "IOS";
#else
	platform = "Android";
#endif
	    // if (PurchasingModuleSelection.Config.IsIosZarinpal)
	    // 	platform = "IOS";
	    // else
	    // {
	    // 	platform = "Android";
	    // }
	    JsonDict parameters = new JsonDict();
	    parameters.Set("clientVersion", Application.version);
	    parameters.Set("clientMarket", Market.GetMarket());
	    parameters.Set("platform", platform );
	    parameters.Set("isAppTuttiBuild", BuildType.IsAppTuttiBuild);
	    WebRequestQueue.Instance.StartCall("rtw_get_force_update", "get force update details", parameters, GetForceUpdate);

    }
	
    private static void GetForceUpdate(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
	    if (zstatus == 200) {
		    if (string.IsNullOrEmpty(zerror)) 
		    {
			    JsonDict json = new JsonDict();
			    if (json.Read(zhttpcontent)) 
			    {
				    var error = json.GetString("error");
				    downloadUrl = json.GetString("Url");
				    var noticeUpdate = json.GetString("IsNotice");
				    if (!string.IsNullOrEmpty(error))
				    {
					    PopUp popup = new PopUp
					    {
						    Title = "TEXT_POPUPS_NEW_VERSION_TITLE",
						    BodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_FORCE_UPDATE"),
						    BodyAlreadyTranslated = true,
						    ConfirmAction = OnUpdateButtonClick,
						    ConfirmText = "TEXT_BUTTON_UPDATE",
					    };
					    if (noticeUpdate.ToLower() == "true")
					    {
						    popup.CancelText = "TEXT_BUTTON_CANCEL";
					    }
					    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
				    }
			    }
		    }
		    else 
		    {
			    Debug.LogError(zerror);
		    }
	    }
	    doneForceUpdate = true;
    }
    
    private static void OnUpdateButtonClick()
    {
	    doneForceUpdate = true;
#if UNITY_EDITOR
	    Application.OpenURL(downloadUrl);
	    EditorApplication.isPlaying = false;
#else
		Application.OpenURL(downloadUrl);
        Application.Quit();
#endif
    }
	
    private static void MakeCoroutineManager()
	{
        managerGo.AddComponent<CoroutineManager>();
	}

    private static void MakeObjectiveManager()
    {
        var objectivemanager = (Object.Instantiate(Resources.Load("Prefabs/ObjectiveManager")) as GameObject).GetComponent<ObjectiveManager>();
        objectivemanager.enabled = false;
        Persist(objectivemanager.gameObject);
    }

	private static void MakeCheapGameObjects3()
	{
        RaceEventQuery.Create();
        RaceEventDifficulty.Create();
        ProductManager productManager = managerGo.AddComponent<ProductManager>();
        productManager.enabled = false;
        managerGo.AddComponent<AssetDatabaseClient>();
	}

	private static void MakeCheapGameObjects4()
	{
        AssetDatabaseVersionPoll assetDatabaseVersionPoll = managerGo.AddComponent<AssetDatabaseVersionPoll>();
        assetDatabaseVersionPoll.enabled = false;
        managerGo.AddComponent<AssetProviderClient>();
		RaceEventInfo.Create();
		CompetitorManager.Create();

	    var bubblePrefab = Resources.Load<GameObject>("Prefabs/BubbleManager");
	    var bubbleInstance = Object.Instantiate(bubblePrefab);
        Persist(bubbleInstance);
        //managerGo.AddComponent<BubbleManager>();
        managerGo.AddComponent<AsyncSwitching>();
	}

	private static void MakeCheapGameObjects5()
	{
        AppStore appStore = managerGo.AddComponent<AppStore>();
        appStore.enabled = false;
        FuelManager fuelManager = managerGo.AddComponent<FuelManager>();
        fuelManager.enabled = false;
        //managerGo.AddComponent<DynamicFontManager>();
        managerGo.AddComponent<TierXManager>();
        managerGo.AddComponent<DailyBattleRewardManager>();
        
        
	}

	private static void PlatformSpecificSetup()
	{
		NotificationManager.CreateActiveManager();
        BasePlatform.ActivePlatform.Initialise();
    }

    private static void MakeUICache()
	{
        UICacheManager.Create();
        Persist(UICacheManager.Instance.gameObject);
	}

	public static void ResetUICache()
	{
        if (UICacheManager.Instance != null)
        {
            UICacheManager.Instance.DestroyInstances();
            Object.Destroy(UICacheManager.Instance.gameObject);
        }
		MakeUICache();
	}

	private static void MakeSocialManagers()
	{
        managerGo.AddComponent<SocialController>();
        SocialFriendsManager socialFriendsManager = managerGo.AddComponent<SocialFriendsManager>();
        socialFriendsManager.enabled = false;
        managerGo.AddComponent<RYFStatusManager>();
	    managerGo.AddComponent<ReferralManager>();
        managerGo.AddComponent<InvitationManager>().enabled = false;

        var leaderboardmanager = managerGo.AddComponent<LegacyLeaderboardManager>();
        Persist(leaderboardmanager.gameObject);
	    leaderboardmanager.enabled = false;
	}

	private static void MakeBundleOffers()
	{
        managerGo.AddComponent<BundleOfferController>();
	}

	private static void MakeMiniStore()
	{
        managerGo.AddComponent<MiniStoreController>();
	}

    private static void MakeSMPNetworkManager()
    {
        GameObject gameObject = new GameObject("SMPNetworkManager");
        Persist(gameObject);
        gameObject.AddComponent<SMPNetworkManager>();
        gameObject.AddComponent<SMPWinStreakMonitor>();
        gameObject.SetActive(true);
    }

	private static void MakeCarSnapshotQueue()
	{
        managerGo.AddComponent<CarSnapshotQueue>();
	}

	private static void MakeTutorialBubblesManager()
	{
		managerGo.AddComponent<TutorialBubblesManager>();
	}

    private static void MakeMetricsIntegration()
    {
	    /*
        var adjust = (Object.Instantiate(Resources.Load("Prefabs/Adjust")) as GameObject);
        Persist(adjust);
        */

        MetricsIntegration metricsIntegration = managerGo.AddComponent<MetricsIntegration>();
        metricsIntegration.Initialize();
        metricsIntegration.enabled = false;
    }
    
    private static void MakeRemoteConfigManager()
    {
	    managerGo.AddComponent<RemoteConfigManager>();
    }

    private static void WarmUpShaders()
	{
        //Object.Instantiate(Resources.Load("Prefabs/WarmUpShaders"));
	}

	private static void MakeZ2HApplicationManager()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        managerGo.AddComponent<AndroidApplicationManager>();
#else
        managerGo.AddComponent<ApplicationManager>();
#endif
	}

	private static void AndroidPreprocess()
	{
	}

	private static void MakeAndroidBackgroundCRCCheck()
	{
#if UNITY_ANDROID
        if (PlayerProfileManager.Instance.ActiveProfile == null)
        {
            UserManager.Instance.StartConnect();
        }
#endif
	}

	private static void MakeUI()
	{
		GameObject ui = Object.Instantiate(Resources.Load("Prefabs/UI")) as GameObject;
	    ui.name = "UI";
	    ui.AddComponent<ScreenEvents>();
		Persist(ui);


        GameObject carinfoUI = Object.Instantiate(Resources.Load("CommonUI/CarInfo")) as GameObject;
        carinfoUI.name = "Runtime Car Info UI";
        carinfoUI.transform.SetParent(ui.transform,false);
        var screenIndex = ui.transform.Find("Screens").GetSiblingIndex();
        carinfoUI.transform.SetSiblingIndex(screenIndex + 1);
        //carinfoUI.SetActive(false);
    }

    private static void MakeCommonUI()
	{
        GameObject gameObject = Object.Instantiate(Resources.Load("CommonUI/CommonUI")) as GameObject;
		Persist(gameObject);
		gameObject.SetActive(false);

	    managerGo.AddComponent<Z2hScreenManager>();
	}

	private static void MakeInGameTutorial()
	{
		Persist(Object.Instantiate(Resources.Load("Prefabs/InGameTutorial")) as GameObject);
	}

	private static void MakeCarInfoUI()
	{
		//GameObject gameObject = Object.Instantiate(Resources.Load("CommonUI/CarInfo")) as GameObject;
		//gameObject.name = "Runtime Car Info UI";
		//gameObject.SetActive(false);
		//Persist(gameObject);
	}

	private static void MakeCarGroundFlareContainer()
	{
        //GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/CarGroundFlares")) as GameObject;
        //gameObject.name = "CarGroundFlaresDefault";
        //gameObject.SetActive(false);
        //Persist(gameObject);
        //CarVisuals.CarGroundFlaresDefaultGO = gameObject;
	}

	private static void MakeAchievementsController()
	{
        managerGo.AddComponent<AchievementsController>();
	}

	private static void MakeAdManager()
	{
        managerGo.AddComponent<GTAdManager>();
        managerGo.AddComponent<Chartboost>();
    }

	private static void MakePPEManager()
	{
        //PPEManager.Initialise();
	}

	private static void MakeOfferWallManager()
	{
        OfferWallManager.Initialise();
	}

	private static void MakeAudioPrefab()
	{
        GameObject original = Resources.Load("Prefabs/Sounds/Audio") as GameObject;
        GameObject gameObject = Object.Instantiate(original);
        gameObject.name = "Audio";
        Object.DontDestroyOnLoad(gameObject);
        Persist(gameObject);
	}

	private static void MakeMenuAudio()
	{
		GameObject original = Resources.Load("Prefabs/Sounds/MenuSound") as GameObject;
		GameObject gameObject = Object.Instantiate(original);
		gameObject.name = "MenuAudio";
		Persist(gameObject);
	}

	private static void MakeNumberPlateManager()
	{
        //GameObject original = Resources.Load("Prefabs/NumberPlateManager") as GameObject;
        //GameObject gameObject = Object.Instantiate(original);
        //gameObject.name = "NumberPlateManager";
        //Persist(gameObject);
	}

	private static void MakeCheapGameObjects6()
	{
        GameObject original = Resources.Load("Prefabs/SnapShotManager") as GameObject;
		GameObject gameObject = Object.Instantiate(original);
        gameObject.name = "SnapShotManager";
		Persist(gameObject);
	    managerGo.AddComponent<PreRaceCarGarageSetup>();


        original = Resources.Load("Prefabs/SnapShotManager_New") as GameObject;
        gameObject = Object.Instantiate(original);
        gameObject.name = "SnapShotManager_New";
        Persist(gameObject);
        managerGo.AddComponent<ScreenshotCapture>();//For sharing purpose
        if (!MultiplayerUtils.DisableMultiplayer)
        {
            NetworkReplayManager networkReplayManager = managerGo.AddComponent<NetworkReplayManager>();
            networkReplayManager.enabled = false;
            RTWStatusManager rTWStatusManager = managerGo.AddComponent<RTWStatusManager>();
            rTWStatusManager.enabled = false;
        }
	}

	public static void MakeCheapGameObjects7()
	{
        ServerAccountConnectActions serverAccountConnectActions = managerGo.AddComponent<ServerAccountConnectActions>();
        serverAccountConnectActions.enabled = false;
        managerGo.AddComponent<AssetBackgroundDownloader>();
        AssetSystemManager.Create();
        managerGo.AddComponent<VideoForRewardsManager>();
        ServerSynchronisedTime.Create();
	}

	private static void InitKeyboard()
	{
        var keyMapping = new GameObject("KeyMappings").AddComponent<KeyMappings>();
        Persist(keyMapping.gameObject);
	}

	private static void FinishInitialisation()
	{
        RateTheAppNagger.initNagger();
	    ScreenManager.Instance.PushScreen(ScreenID.Dummy);
        PauseGame.Initialise();
        StartSessionTimer();
        Z2HInitialisation.Instance.Complete = true;
        GTSystemOrder.StartUpGameSystems();
	}

	private static void StartSessionTimer()
	{
        GameObject gameObject = new GameObject();
        var sessionTimer = gameObject.AddComponent<Z2HSessionTimer>();
        gameObject.name = "Z2HSessionTimer";
	    sessionTimer.StartSessionTimer();
        Persist(gameObject);
	}

	public static void Persist(GameObject obj)
	{
		obj.transform.SetParent(Z2HInitialisation.persistence.transform);
        //Object.DontDestroyOnLoad(obj);
	}
}

