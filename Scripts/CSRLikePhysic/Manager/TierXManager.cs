using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataSerialization;
using KingKodeStudio;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public class TierXManager : MonoBehaviour, IBundleOwner
{
	public delegate void OnTierXReady();

	public const string MainOverviewTheme = "tierx_overview";

	public bool OverrideDifficultyDeltas;

	public DifficultyDeltas OverriddenDifficultyDeltas = new DifficultyDeltas();

	public ThemeLayout ThemeDescriptor;



    public PinScheduleConfiguration PinSchedule;

	public PopUpConfiguration PopUps;

#if UNITY_EDITOR
    private TierXConfiguration TierXConfigurationEditor;
#endif

    public NarrativeSceneConfiguration Narrative;

	public ThemeAnimationsConfiguration ThemeAnimations;

	public GameObject ThemeObject;

    public TierXThemeDescriptor ThemeDescriptorPrefab;

	public Dictionary<string, Texture2D> BossTextures = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture2D> PinTextures = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture2D> PinBackgrounds = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture2D> ProgressScreenMemberPanels = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture2D> CrewMemberEventPaneTextures = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture2D> CrewLogos = new Dictionary<string, Texture2D>();

	public Texture2D CrossTexture;

	public Dictionary<string, GameObject> PinAnimations = new Dictionary<string, GameObject>();

	public Dictionary<string, Texture2D> EventHubTextures = new Dictionary<string, Texture2D>();

    public WorldTourProgressTextures ProgressTextures;

	public Texture2D[] CrewLogo;

	private string LastThemeOptionIntroNarrativeSeen = string.Empty;

	private bool bundleReady;

	private CareerModeMapEventSelect EventSelect;

	private bool jsonLoaded;

	public bool IsPizzaPinActive;

	private string ThemeName = string.Empty;

	private string ThemeOption;

	public static bool forceCareerModePane;

    public static TierXManager Instance
	{
		get;
		private set;
	}

	public bool ThemeDescriptorPopulated
	{
		get;
		private set;
	}

	public bool IsJsonLoaded
	{
		get
		{
			return this.jsonLoaded;
		}
	}

	public string CurrentThemeName
	{
		get
		{
			return this.ThemeName;
		}
	}

	public string NarrativeScenesResourcesPath
	{
		get
		{
			return "World_Tour/" + this.CurrentThemeName + "/NarrativeScenes/";
		}
	}

	public string CurrentThemeOption
	{
		get
		{
			return this.ThemeOption;
		}
	}

	public string PreviousThemeName
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		this.ThemeDescriptor = null;
		this.PinSchedule = null;
		this.PopUps = null;
		this.Narrative = null;
		this.ThemeAnimations = null;
		this.ThemeDescriptorPopulated = false;
	}

	public bool IsTierThemeActive()
	{
		return this.ThemeDescriptorPopulated && !this.ThemeDescriptor.IsOverviewTheme;
	}

	public bool IsOverviewThemeActive()
	{
		return this.ThemeDescriptorPopulated && this.ThemeDescriptor.IsOverviewTheme;
	}

	public bool IsMainOverviewThemeActive()
	{
		return this.CurrentThemeName == "tierx_overview";
	}

	public bool IsReady()
	{
		return this.bundleReady;
	}

	public bool CanSwipe()
	{
		return this.ThemeDescriptorPopulated && this.ThemeDescriptor.CanSwipe;
	}

	public bool ShouldLoadThemeOnBackOut()
	{
		return this.ThemeDescriptorPopulated && !string.IsNullOrEmpty(this.ThemeDescriptor.ThemeToLoadOnBackOut);
	}

	public PopupData GetPopupDataWithID(string id)
	{
		if (this.PopUps != null && this.PopUps.PopUpLookup.ContainsKey(id))
		{
			return this.PopUps.PopUpLookup[id];
		}
		return null;
	}

	private Texture2D GetTexture(Dictionary<string, Texture2D> dictionary, string textureID)
	{
		if (dictionary.ContainsKey(textureID))
		{
			return dictionary[textureID];
		}
		return (Texture2D)Resources.Load(textureID);
	}

	public Texture2D GetEventHubTexture(string textureID)
	{
		return this.GetTexture(this.EventHubTextures, textureID);
	}

	public Texture2D GetCrewMemberEventPaneTexture(string textureID)
	{
		if (BuildType.IsAppTuttiBuild && textureID=="crew_italia_portrait_other" && !textureID.Contains("apptutti"))
			textureID += "_apptutti";
		return this.GetTexture(this.CrewMemberEventPaneTextures, textureID);
	}

	public Texture2D GetBossTexture(string textureID)
	{
		if (BuildType.IsAppTuttiBuild && textureID.ToLower().Contains("crew-it") && !textureID.Contains("apptutti"))
			textureID += "_apptutti";
		return this.GetTexture(this.BossTextures, textureID);
	}

	public void ShutDown()
	{
		this.bundleReady = false;
		this.ThemeDescriptorPopulated = false;
		this.jsonLoaded = false;
		this.Cleardown();
	}

	public void OnAssetSystemLoad()
	{
		this.bundleReady = false;
		this.jsonLoaded = false;
		this.ThemeName = "tierx_overview";
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
	    var themeObject =
	        AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/TierX/TierX_Overview/ThemeObject.prefab");

        //We load TierxConfiguration to save time building budle of tierx every time
        TierXConfigurationEditor =
            AssetDatabase.LoadAssetAtPath<TierXConfiguration>(
                "Assets/TierX/TierX_Overview/TierX_Overview.asset");
        BundleReadyInternal(themeObject, delegate { });
#else
        AssetProviderClient.Instance.RequestAsset("tierx_overview", delegate(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
		{
			this.BundleReady(zAssetID, zAssetBundle, zOwner, delegate
			{
			});
		}, Instance);
#endif
	}

    public void CheckAndIncreaseThemeCompletionLevel()
	{
		IGameState gameState = new GameStateFacade();
		if (this.ThemeDescriptor.Progression.IncreaseThemeCompletionLevelRequirements.IsEligible(gameState))
		{
			gameState.IncrementWorldTourThemeCompletionLevel(this.CurrentThemeName);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public bool DisplayThemeProgressionPopup()
	{
        return GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ScreenID.CareerModeMap, null);
	}

	public bool DisplayThemeIntroNarrative()
	{
		IGameState gameState = new GameStateFacade();
		NarrativeSceneForEventData introNarrative = this.ThemeDescriptor.Progression.IntroNarrative;
		if (!introNarrative.IsIntroSceneEligible(gameState))
		{
			return false;
		}
		string introSceneID = introNarrative.GetIntroSceneID();
		NarrativeScene scene = null;
		if (!this.GetNarrativeScene(introSceneID, out scene))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(this.CurrentThemeOption) && this.CurrentThemeOption.Equals(this.LastThemeOptionIntroNarrativeSeen))
		{
			return false;
		}
        if (scene.CharactersDetails.CharacterGroups.Count > 0 &&
            !string.IsNullOrEmpty(scene.CharactersDetails.CharacterGroups[0].LogoTextureName))
        {
            CrewProgressionScreen.BackgroundImageText = scene.CharactersDetails.CharacterGroups[0].LogoTextureName;
        }
        ScreenManager.Instance.PushScreen(ScreenID.CrewProgression);
        ScreenManager.Instance.UpdateImmediately();
        CrewProgressionScreen crewProgressionScreen = ScreenManager.Instance.ActiveScreen as CrewProgressionScreen;
		if (crewProgressionScreen == null)
		{
			return false;
		}
        crewProgressionScreen.SetupForNarrativeScene(scene);
        gameState.IncrementWorldTourThemeCompletionLevel(this.CurrentThemeName);
		this.LastThemeOptionIntroNarrativeSeen = this.CurrentThemeOption;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		return true;
	}

	public void OnEventSelected()
	{
		this.CheckAndIncreaseThemeCompletionLevel();
	}

	private bool StartAutoStartEventIfRequired(EventPin pin)
	{
		if (pin == null)
		{
			return false;
		}
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen == null)
		{
			return false;
		}
		TierXPin component = pin.GetComponent<TierXPin>();
		if (component != null && component.pinDetails != null && component.pinDetails.WorldTourScheduledPinInfo != null && component.pinDetails.WorldTourScheduledPinInfo.AutoStart)
		{
			careerModeMapScreen.OnEventSelected(pin.EventData, false);
			careerModeMapScreen.OnEventStart(pin.EventData);
			return true;
		}
		return false;
	}

	public void SetupThemeAnimations(CareerModeMapEventSelect eventSelect)
	{
		eventSelect.ResetThemeAnimation();
		if (this.ThemeAnimations == null)
		{
			return;
		}
		GameStateFacade gameState = new GameStateFacade();
		ThemeAnimationDetail eligibleAnimation = this.ThemeAnimations.GetEligibleAnimation(gameState);
		if (eligibleAnimation == null)
		{
			return;
		}
		if (eventSelect.SetupThemeAnimation(eligibleAnimation, this.PinAnimations))
		{
            AnimationUtils.PlayAnim(eventSelect.GetComponent<Animation>(), "ThemeAnimation");
			foreach (EventSelectEvent current in eligibleAnimation.InitEventSelectEvents)
			{
				if (current.IsEligible(gameState))
				{
                    AnimationUtils.TriggerEvent(eventSelect, current.GetAnimationEvent());
				}
			}
		}
		else
		{
			eventSelect.ResetThemeAnimation();
		}
	}

	public void RefreshThemeMap()
	{
		IGameState gameState = new GameStateFacade();
		if (this.ThemeDescriptor.CheckForProgression)
		{
			this.CheckAndIncreaseThemeCompletionLevel();
			if (!RaceEventInfo.Instance.IsRelayEventInProgress())
			{
				if (!this.DisplayThemeProgressionPopup())
				{
                    this.DisplayThemeIntroNarrative();
                }
			}
		}
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen != null)
		{
		    careerModeMapScreen.eventPane.Hide();
            careerModeMapScreen.ResetHighlight();
			this.EventSelect = careerModeMapScreen.EventSelect;
            if(!IsOverviewThemeActive())
			    this.EventSelect.RemoveAll(true);
			EventPin pin = this.EventSelect.PopulateTierX(this.ThemeDescriptor, this.PinSchedule);
			if (this.IsPizzaPinActive)
			{
                MapScreenCache.WorldTourBossPin.SetPinPiecesUp();
            }
			else
			{
                MapScreenCache.WorldTourBossPin.DisablePinPieces();
            }
			if (this.StartAutoStartEventIfRequired(pin))
			{
				return;
			}
		}
		if (!this.IsOverviewThemeActive() && string.IsNullOrEmpty(this.CurrentThemeOption))
		{
			if (careerModeMapScreen != null)
			{
				careerModeMapScreen.SetupObjectiveText();
			}
			if (gameState.ShouldPlayWorldTourFinalAnimation(this.ThemeDescriptor.ID, this.ThemeDescriptor.ThemePrizeCar, this.ThemeDescriptor.OutroAnimFlag))
			{
                MapScreenCache.WorldTourBossPin.PlayOutroAnimation();
            }
			else
			{
                MapScreenCache.WorldTourBossPin.SelectAndPlayPizzaIntroAnimation();
            }
		}
		TutorialBubblesManager.Instance.OnScreenChanged();
		TutorialBubblesManager.Instance.TriggerEvent(TutorialBubblesEvent.RefreshThemeMap);
	}

    private void BundleReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner, OnTierXReady onTXready)
	{
	    Object mainAsset = zAssetBundle.LoadAsset("ThemeObject");
	    BundleReadyInternal(mainAsset, onTXready);
	    AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
    }

    private void BundleReadyInternal(Object assetObject, OnTierXReady onTXready)
    {
        this.ThemeObject = (GameObject)Instantiate(assetObject);
        this.ThemeObject.transform.parent = base.gameObject.transform;
        this.ThemeDescriptorPrefab = this.ThemeObject.GetComponent<TierXThemeDescriptor>();
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
        this.ThemeDescriptor = TierXConfigurationEditor.ThemeLayout;
        this.ThemeDescriptor.DoDeSerializationStaff();
#else
        this.ThemeDescriptor = this.LoadFromBinary<ThemeLayout>(this.ThemeDescriptorPrefab.ThemeDescriptor.bytes);
#endif
        this.ThemeDescriptor.Initialise();
        if (this.OverrideDifficultyDeltas)
        {
            this.ThemeDescriptor.DifficultyDeltas = this.OverriddenDifficultyDeltas;
        }
        this.ThemeDescriptorPopulated = true;
        this.LoadTextures();
        this.LoadNarrative();
        this.LoadThemeAnimations();
        this.ThemeDescriptor.Progression.Initialise();
        this.bundleReady = true;
        CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
        if (careerModeMapScreen != null)
        {
            CareerModeMapScreen.mapPaneSelected =
                ThemeName == "tierx_overview" ? MapPaneType.SinglePlayer : MapPaneType.WorldTour;
            careerModeMapScreen.TierXPanelSetup(onTXready);
            MapCamera.MoveToPositionImmediately(careerModeMapScreen.EventSelect.WorldTourRaces,true);
        }
        else if (onTXready != null)
        {
            onTXready();
        }
        IGameState gameState = new GameStateFacade();
        gameState.IncrementWorldTourThemeSeenCount(this.ThemeName);
        if (RequiresLoadingSpinner(this.ThemeName))
        {
            PopUpManager.Instance.KillPopUp();
        }
    }

    public void SetupBackground()
	{
        //if (!this.ThemeDescriptorPrefab.BackgroundTextures.Any<Texture2D>())
        //{
        //    MapScreenCache.SetDefaultTierXBackground(1f);
        //}
        //else
        //{
        //    MapScreenCache.ChangeTierXBackground(this.ThemeDescriptorPrefab.BackgroundTextures[0], this.ThemeDescriptorPrefab.BackgroundScale);
        //}
	}

	private void LoadTextures()
	{
        Texture2D[] pinTextures = this.ThemeDescriptorPrefab.PinTextures;
        for (int i = 0; i < pinTextures.Length; i++)
        {
            Texture2D texture2D = pinTextures[i];
            string textureName = texture2D.name;
            if (!this.PinTextures.ContainsKey(textureName))
            {
                this.PinTextures.Add(textureName, texture2D);
            }
        }
        Texture2D[] pinBackgrounds = this.ThemeDescriptorPrefab.PinBackgrounds;
        for (int j = 0; j < pinBackgrounds.Length; j++)
        {
            Texture2D texture2D2 = pinBackgrounds[j];
            if (!this.PinBackgrounds.ContainsKey(texture2D2.name))
            {
                this.PinBackgrounds.Add(texture2D2.name, texture2D2);
            }
        }
        Texture2D[] bossTextures = this.ThemeDescriptorPrefab.BossTextures;
        for (int k = 0; k < bossTextures.Length; k++)
        {
            Texture2D texture2D3 = bossTextures[k];
            if (!this.BossTextures.ContainsKey(texture2D3.name))
            {
                this.BossTextures.Add(texture2D3.name, texture2D3);
            }
        }
        Texture2D[] crewMemberEventPaneIcons = this.ThemeDescriptorPrefab.CrewMemberEventPaneIcons;
        for (int l = 0; l < crewMemberEventPaneIcons.Length; l++)
        {
            Texture2D texture2D4 = crewMemberEventPaneIcons[l];
            if (!this.CrewMemberEventPaneTextures.ContainsKey(texture2D4.name))
            {
                this.CrewMemberEventPaneTextures.Add(texture2D4.name, texture2D4);
            }
        }
        Texture2D[] crewLogos = this.ThemeDescriptorPrefab.CrewLogos;
        for (int m = 0; m < crewLogos.Length; m++)
        {
            Texture2D texture2D5 = crewLogos[m];
            if (!this.CrewLogos.ContainsKey(texture2D5.name))
            {
                this.CrewLogos.Add(texture2D5.name, texture2D5);
            }
        }
        this.ProgressTextures = this.ThemeDescriptorPrefab.ProgressTextures;
        Texture2D[] crewMembers = this.ProgressTextures.CrewMembers;
        for (int n = 0; n < crewMembers.Length; n++)
        {
	        //UnityEngine.Debug.Log("crewMembers : "+n);
            Texture2D texture2D6 = crewMembers[n];
            if (!this.ProgressScreenMemberPanels.ContainsKey(texture2D6.name))
            {
                this.ProgressScreenMemberPanels.Add(texture2D6.name, texture2D6);
            }
        }
        this.CrossTexture = this.ThemeDescriptorPrefab.CrossTexture;
        Texture2D[] eventHubTextures = this.ThemeDescriptorPrefab.EventHubTextures;
        for (int num = 0; num < eventHubTextures.Length; num++)
        {
            Texture2D texture2D7 = eventHubTextures[num];
            if (!this.EventHubTextures.ContainsKey(texture2D7.name))
            {
                this.EventHubTextures.Add(texture2D7.name, texture2D7);
            }
        }
    }

	public void OnAssetSystemUnload()
	{
		this.Cleardown();
	}

	private void Cleardown()
	{
		if (this.ThemeDescriptor != null && !string.IsNullOrEmpty(this.ThemeDescriptor.Localisation))
		{
            //LocalisationManager.ReleaseTextPack(this.ThemeDescriptor.Localisation);
		}
		Destroy(Instance.ThemeObject);
		this.PinTextures.Clear();
		this.PinBackgrounds.Clear();
		this.BossTextures.Clear();
		this.CrewMemberEventPaneTextures.Clear();
		this.CrewLogos.Clear();
		this.ProgressScreenMemberPanels.Clear();
		this.PinAnimations.Clear();
		this.EventHubTextures.Clear();
		this.ThemeDescriptor = null;
		this.PinSchedule = null;
		this.PopUps = null;
		this.Narrative = null;
		this.ThemeAnimations = null;
		this.CrossTexture = null;
        this.ProgressTextures = null;
		this.ThemeDescriptorPopulated = false;
	}

	public T LoadFromBinary<T>(byte[] bytes) where T : class
	{
        TierXConfigurationSerializer tierXConfigurationSerializer = new TierXConfigurationSerializer();
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            try
            {
                return tierXConfigurationSerializer.Deserialize(memoryStream, null, typeof(T)) as T;
            }
            catch (Exception var_2_34)
            {
                GTDebug.LogError(GTLogChannel.TierX, var_2_34.Message);
            }
        }
		return (T)((object)null);
	}

	public void LoadTierXJson(OnTierXReady onTXready)
	{
		if (!this.ThemeDescriptorPopulated)
		{
			this.ThemeDescriptorPopulated = true;
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
            this.ThemeDescriptor = TierXConfigurationEditor.ThemeLayout;
            this.ThemeDescriptor.DoDeSerializationStaff();
#else
            this.ThemeDescriptor = this.LoadFromBinary<ThemeLayout>(this.ThemeDescriptorPrefab.ThemeDescriptor.bytes);
#endif
            this.ThemeDescriptor.Initialise();
		}
        if (this.PinSchedule == null && this.ThemeDescriptorPrefab.PinSchedule != null)
        {
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
            this.PinSchedule = TierXConfigurationEditor.PinScheduleConfiguration;
#else
            this.PinSchedule = this.LoadFromBinary<PinScheduleConfiguration>(this.ThemeDescriptorPrefab.PinSchedule.bytes);
#endif
            this.PinSchedule.Initialise(this.ThemeName);
        }
        if (this.PopUps == null && this.ThemeDescriptorPrefab.PopUps != null)
        {
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
            this.PopUps = TierXConfigurationEditor.PopUpConfiguration;
            this.PopUps.DoDeSerializationStaff();
#else
            this.PopUps = this.LoadFromBinary<PopUpConfiguration>(this.ThemeDescriptorPrefab.PopUps.bytes);
#endif
            this.PopUps.Initialise();
        }
        if (false)//!string.IsNullOrEmpty(this.ThemeDescriptor.Localisation))
        {
            //LocalisationManager.LoadTextPack(this.ThemeDescriptor.Localisation, delegate(string a)
            //{
            //    if (onTXready != null)
            //    {
            //        onTXready();
            //    }
            //    this.jsonLoaded = true;
            //});
        }
        else
        {
            if (onTXready != null)
            {
                onTXready();
            }
            this.jsonLoaded = true;
        }
        GameDatabase.Instance.Colours.SetTierXColour(this.ThemeDescriptor.Colour.AsUnityColor());
	}

	private void AddNarrativeIdsFromFilenames(ref NarrativeSceneConfiguration config)
	{
        config.NarrativeScenes = new List<string>();
        TextAsset[] narrativeScenes = this.ThemeDescriptorPrefab.NarrativeScenes;
        for (int i = 0; i < narrativeScenes.Length; i++)
        {
            TextAsset textAsset = narrativeScenes[i];
            config.NarrativeScenes.Add(textAsset.name);
        }
    }

	private void LoadNarrative()
	{
        if (this.Narrative == null)
        {
            if (this.ThemeDescriptorPrefab.Narrative == null || string.IsNullOrEmpty(this.ThemeDescriptorPrefab.Narrative.text))
            {
                this.Narrative = new NarrativeSceneConfiguration();
                this.AddNarrativeIdsFromFilenames(ref this.Narrative);
            }
            else
            {
                this.Narrative = JsonConverter.DeserializeObject<NarrativeSceneConfiguration>(this.ThemeDescriptorPrefab.Narrative.text);
            }
            TextAsset[] narrativeScenes = this.ThemeDescriptorPrefab.NarrativeScenes;
            for (int i = 0; i < narrativeScenes.Length; i++)
            {
                TextAsset textAsset = narrativeScenes[i];
                if (textAsset != null)
                {
                    this.Narrative.RegisterScene(textAsset);
                }
            }
        }
    }

	private void LoadThemeAnimations()
	{
        if (this.ThemeAnimations == null && this.ThemeDescriptorPrefab.ThemeAnimations != null)
        {
            this.ThemeAnimations = this.LoadFromBinary<ThemeAnimationsConfiguration>(this.ThemeDescriptorPrefab.ThemeAnimations.bytes);
            GameObject[] pinAnimations = this.ThemeDescriptorPrefab.PinAnimations;
            for (int i = 0; i < pinAnimations.Length; i++)
            {
                GameObject gameObject = pinAnimations[i];
                if (!this.PinAnimations.ContainsKey(gameObject.name))
                {
                    if (!(gameObject.GetComponent<Animation>().clip == null))
                    {
                        this.PinAnimations.Add(gameObject.name, gameObject);
                    }
                }
            }
        }
    }

	private void Update()
	{
	}

	public void LoadMainOverviewTheme(OnTierXReady onTXready = null)
	{
		this.LoadTheme("tierx_overview", onTXready, null);
	}

	public void LoadBackOutTheme(OnTierXReady onTXready = null)
	{
		this.LoadTheme(this.ThemeDescriptor.ThemeToLoadOnBackOut, onTXready, null);
	}

	private static bool RequiresLoadingSpinner(string theme)
	{
		return theme.Equals("TierX_International") && BaseDevice.ActiveDevice.DeviceQuality == AssetQuality.Low;
	}

	public void LoadTheme(string themeToLoad, OnTierXReady onTXready = null, string themeOption = null)
	{
		this.Cleardown();
		this.bundleReady = false;
		this.jsonLoaded = false;
		this.PreviousThemeName = this.ThemeName;
		this.ThemeName = themeToLoad.ToLower();
		this.ThemeOption = themeOption;
        ScreenManager.Instance.ClearAlreadyOnStackFlag();
        if (RequiresLoadingSpinner(this.ThemeName))
		{
			PopUpDatabase.Common.ShowWaitSpinnerPopup();
		}
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
	    var themeObject =
	        AssetDatabase.LoadAssetAtPath<GameObject>(
	            "Assets/TierX/" + ThemeName + "/ThemeObject.prefab");
        //We load TierxConfiguration to save time building budle of tierx every time
        TierXConfigurationEditor =
            AssetDatabase.LoadAssetAtPath<TierXConfiguration>(
                "Assets/TierX/" + ThemeName + "/" + ThemeName + ".asset");
        BundleReadyInternal(themeObject, onTXready);
#else
        AssetProviderClient.Instance.RequestAsset(this.ThemeName, delegate (string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	    {
	        this.BundleReady(zAssetID, zAssetBundle, zOwner, onTXready);
	    }, Instance);
#endif

	}

    private IEnumerator RemoveOldPins()
    {
        yield return new WaitForSeconds(0.5f);
        var scr = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
        if (scr != null)
        {
            scr.EventSelect.RemoveOld();
        }
    }

    public RaceEventData GetFirstAvailableEvent(IGameState gameState)
	{
		if (this.PinSchedule == null || this.ThemeDescriptor == null)
		{
			return null;
		}
		ScheduledPin scheduledPin = this.PinSchedule.GetFirstAvailablePin(gameState);
		PinDetail pinDetail = this.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == scheduledPin.PinID);
		if (pinDetail == null)
		{
			return null;
		}
		pinDetail.WorldTourScheduledPinInfo = scheduledPin;
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(pinDetail.EventID);
		if (eventByEventIndex == null)
		{
			return null;
		}
        eventByEventIndex.SetWorldTourPinPinDetail(pinDetail);
        return eventByEventIndex;
	}

	public PinDetail GetPin(ScheduledPin scheduledPin)
	{
		PinDetail pinDetail = this.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == scheduledPin.PinID);
		pinDetail.WorldTourScheduledPinInfo = scheduledPin;
		if (!string.IsNullOrEmpty(pinDetail.TemplateName))
		{
			PinTemplate pinTemplate = this.ThemeDescriptor.PinTemplates.FirstOrDefault((PinTemplate q) => q.TemplateName == pinDetail.TemplateName);
			if (pinTemplate != null)
			{
                pinDetail.ApplyTemplate(pinTemplate);
			}
		}
		return pinDetail;
	}

	private void OverwriteEvents(PinDetail pin, List<CarOverride> carOverrides)
	{
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(pin.EventID);
		if (eventByEventIndex == null || carOverrides == null)
		{
			return;
		}
		if (eventByEventIndex.IsRelay)
		{
			for (int i = 0; i < eventByEventIndex.Group.RaceEvents.Count; i++)
			{
				RaceEventData raceEventData = eventByEventIndex.Group.RaceEvents[i];
                raceEventData.SetWorldTourPinPinDetail(pin);
                if (i < carOverrides.Count)
				{
					CarOverride carOverride = carOverrides[i];
					ScheduledPin scheduledPin;
					this.OverwriteEvent(raceEventData, carOverride, out scheduledPin);
				}
			}
		}
		else if (carOverrides.Count > 0)
		{
			CarOverride carOverride2 = carOverrides[0];
			ScheduledPin scheduledPin;
			this.OverwriteEvent(eventByEventIndex, carOverride2, out scheduledPin);
			if (scheduledPin != null)
			{
				pin.WorldTourScheduledPinInfo.AIDriverOverrides = scheduledPin.AIDriverOverrides;
			}
		}
	}

	private void OverwriteEvent(RaceEventData targetEvent, CarOverride carOverride, out ScheduledPin choiceScheduledPin)
	{
		IGameState gameState = new GameStateFacade();
		string themeID = carOverride.ThemeID ?? this.CurrentThemeName;
		int num = gameState.ChoiceSelection(themeID, carOverride.SequenceID, carOverride.ScheduledPinID);
		PinSequence sequence = this.PinSchedule.GetSequence(carOverride.ChoiceSequenceID);
		if (num < 0 || num >= sequence.Pins.Count)
		{
			choiceScheduledPin = null;
			return;
		}
		choiceScheduledPin = sequence.Pins[num];
		string choicePinID = choiceScheduledPin.PinID;
		PinDetail pinDetail = this.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == choicePinID);
		int eventID = pinDetail.EventID;
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(eventID);
		Type typeFromHandle = typeof(RaceEventData);
		foreach (string current in CarOverride.EventFieldsToCopy)
		{
			FieldInfo field = typeFromHandle.GetField(current);
			if (field != null)
			{
				object value = field.GetValue(eventByEventIndex);
				field.SetValue(targetEvent, value);
			}
		}
		if (carOverride.ShouldSetHumanCarToAI)
		{
			targetEvent.HumanCar = eventByEventIndex.AICar;
		}
	}

	private List<PinDetail> GetPins(List<ScheduledPin> scheduledPinList, bool overrideEvents = true)
	{
		List<PinDetail> list = new List<PinDetail>();
		foreach (ScheduledPin scheduledPin in scheduledPinList)
		{
			PinDetail pin = this.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == scheduledPin.PinID);
			if (pin != null)
			{
				pin.WorldTourScheduledPinInfo = scheduledPin;
				if (!string.IsNullOrEmpty(pin.TemplateName))
				{
					PinTemplate pinTemplate = this.ThemeDescriptor.PinTemplates.FirstOrDefault((PinTemplate q) => q.TemplateName == pin.TemplateName);
					if (pinTemplate != null)
					{
						pin.ApplyTemplate(pinTemplate);
					}
				}
				if (overrideEvents && pin.EventID > 0)
				{
					this.OverwriteEvents(pin, scheduledPin.CarOverrides);
				}
				list.Add(pin);
			}
		}
		return list;
	}

	public List<PinDetail> GetAllPins(bool overrideEvents = true)
	{
		if (this.PinSchedule != null)
		{
			return this.GetPins(this.PinSchedule.GetAllPins(), overrideEvents);
		}
		return this.ThemeDescriptor.PinDetails;
	}

	private void OverrideTimelineProperties(List<PinDetail> pins, List<TimelinePinDetails> details, PinDetail.TimelineDirection direction)
	{
		int num = 0;
		while (num < pins.Count && num < details.Count)
		{
			PinDetail pinDetail = pins[num];
			pinDetail.CurrentTimelineDirection = direction;
			pinDetail.Position = details[num].Position;
			pinDetail.Label = string.Format("Timeline_{1}_{2}", pinDetail.WorldTourScheduledPinInfo.ParentSequence.ID, direction, num + 1);
			pinDetail.TimelineDetails = details[num];
			pinDetail.IsSelectable = false;
			num++;
		}
	}

	private List<PinDetail> GetTimelinePins(ScheduledPin scheduledPin)
	{
		List<TimelinePinDetails> nextPinsLayout = this.ThemeDescriptor.NextPinsLayout;
		List<TimelinePinDetails> previousPinsLayout = this.ThemeDescriptor.PreviousPinsLayout;
		int pinIndex = scheduledPin.GetPinIndex();
		PinSequence parentSequence = scheduledPin.ParentSequence;
		List<ScheduledPin> scheduledPinList = parentSequence.GetNextTimelinePins(pinIndex, nextPinsLayout.Count).ToList<ScheduledPin>();
		List<PinDetail> pins = this.GetPins(scheduledPinList, false);
		List<ScheduledPin> scheduledPinList2 = parentSequence.GetPreviousTimelinePins(pinIndex, previousPinsLayout.Count).ToList<ScheduledPin>();
		List<PinDetail> pins2 = this.GetPins(scheduledPinList2, false);
		this.OverrideTimelineProperties(pins, nextPinsLayout, PinDetail.TimelineDirection.Next);
		this.OverrideTimelineProperties(pins2, previousPinsLayout, PinDetail.TimelineDirection.Previous);
		List<PinDetail> list = pins2;
		list.AddRange(pins);
		return list;
	}

	private List<PinDetail> SetupPinsForTimeline(List<PinDetail> pinDetails)
	{
		List<PinDetail> list = new List<PinDetail>();
		foreach (PinDetail current in pinDetails)
		{
			current.CurrentTimelineDirection = PinDetail.TimelineDirection.None;
			if (current.WorldTourScheduledPinInfo.ParentSequence.TimelineData.ShowTimeline)
			{
				current.IsSelectable = true;
				List<PinDetail> timelinePins = this.GetTimelinePins(current.WorldTourScheduledPinInfo);
				list.AddRange(timelinePins);
				current.Label = "Timeline_Current";
			}
		}
		pinDetails.AddRange(list);
		return pinDetails;
	}

	private void SetupPinLabels(List<PinDetail> pinDetails)
	{
		foreach (PinDetail current in pinDetails)
		{
			if (current.WorldTourScheduledPinInfo != null)
			{
				current.Label = current.WorldTourScheduledPinInfo.ParentSequence.ID;
			}
			else
			{
				current.Label = string.Empty;
			}
		}
	}

	public List<PinDetail> GetPins()
	{
		List<PinDetail> list2;
		if (this.PinSchedule != null)
		{
			GameStateFacade gameState = new GameStateFacade();
			List<ScheduledPin> pins = this.PinSchedule.GetPins(gameState);
			List<PinDetail> list = this.GetPins(pins, true);
			this.SetupPinLabels(list);
			list = this.SetupPinsForTimeline(list);
			list2 = list;
		}
		else
		{
			list2 = this.ThemeDescriptor.PinDetails;
			this.SetupPinLabels(list2);
		}
		return list2;
	}

	public bool GetNarrativeScene(string sceneID, out NarrativeScene scene)
	{
		return this.Narrative.GetScene(sceneID, out scene);
	}

	public RestrictionRaceHelperOverride GetRestrictionHelpOverride(int eventId)
	{
		return this.ThemeDescriptor.RestrictionRaceHelperOverrides.Find((RestrictionRaceHelperOverride x) => x.EventID == eventId);
	}

	public List<int> GetValidSuperNitrousEventsForTheme()
	{
		List<int> list = new List<int>();
		foreach (PinDetail current in this.ThemeDescriptor.PinDetails)
		{
			if (current.IsSuperNitrous)
			{
				list.Add(current.EventID);
			}
		}
		return list;
	}

    /// <summary>
    /// This method is called by invoke message of WorldTourBossPinProgression Class
    /// </summary>
	public void AwardWorldTourPrizeCar()
	{
		ThemeLayout themeDescriptor = Instance.ThemeDescriptor;
		string carID = themeDescriptor.ThemePrizeCar;
		bool themePrizeCarIsElite = themeDescriptor.ThemePrizeCarIsElite;
		if (!string.IsNullOrEmpty(carID))
		{
			CarGarageInstance carGarageInstance = new CarGarageInstance();
			CarUpgradeSetup upgradeSetup = new CarUpgradeSetup();
		    var carinfo = CarDatabase.Instance.GetCar(carID);
            carGarageInstance.SetupNewGarageInstance(carinfo);
			CarInfo car = CarDatabase.Instance.GetCar(carID);
			if (themePrizeCarIsElite && (car.HasEliteLivery() || !string.IsNullOrEmpty(car.EliteOverrideCarAssetID)))
			{
				carGarageInstance.AppliedLiveryName = car.name + "_Livery_Elite";
			}
			BoostNitrous.AwardBossCar(carGarageInstance, upgradeSetup, themePrizeCarIsElite);
			PlayerProfile profile = PlayerProfileManager.Instance.ActiveProfile;
			PopupData carAwardPopupData = Instance.ThemeDescriptor.CarAwardPopupData;
			carAwardPopupData.Initialise();
			PopUpManager.Instance.TryShowPopUp(carAwardPopupData.GetPopup(delegate
			{
				profile.CurrentlySelectedCarDBKey = carID;
				profile.UpdateCurrentCarSetup();
				profile.UpdateCurrentPhysicsSetup();
				CarInfoUI.Instance.SetCurrentCarIDKey(carID);
                ScreenManager.Instance.PopScreen();
                //ScreenManager.Instance.UpdateImmediately();
			}, null), PopUpManager.ePriority.Default, null);
			IGameState gameState = new GameStateFacade();
			gameState.IncrementWorldTourThemeCompletionLevel(this.CurrentThemeName);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public void GoCareerModeMapScreen(int pane)
	{
		CarInfoUI.Instance.SetCurrentCarIDKey(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
		ScreenManager.Instance.PopScreen();
	    if (ScreenManager.Instance.CurrentScreen == ScreenID.Manufacturer)
	    {
            ScreenManager.Instance.PopScreen();
	    }
        //if (ScreenManager.Instance.PeekScreen() == ScreenID.Manufacturer)
        //{
        //    ScreenManager.Instance.PopScreen();
        //}
		forceCareerModePane = true;
		//CareerModeMapScreen.mapPaneSelected = pane;
	    ScreenManager.Instance.PushScreen(ScreenID.CareerModeMap);
        ScreenManager.Instance.UpdateImmediately();
	}

	public void OnGoWorldTourHub_IT()
	{
		Instance.OnGoWorldTourHubTheme("TierX_Italia");
	}

	public void OnGoWorldTourHub_EU()
	{
		Instance.OnGoWorldTourHubTheme("TierX_EU");
	}

	public void OnGoWorldTourHub_UK()
	{
		Instance.OnGoWorldTourHubTheme("TierX_UK");
	}

	public void OnGoWorldTourHub_US()
	{
		Instance.OnGoWorldTourHubTheme("TierX_USA");
	}

	public void OnGoToSpecificWorldTourHub(string methodName)
	{
		MethodInfo method = base.GetType().GetMethod(methodName);
		method.Invoke(this, null);
	}

	public void OnGoWorldTourHubTheme(string theme)
	{
		if (this.IsOverviewThemeActive())
		{
			this.OnGoWorldTourHub_Any();
			CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
			if (careerModeMapScreen != null)
			{
				EventPin groupPin = careerModeMapScreen.EventSelect.GetGroupPinMatchingCondition(delegate(EventPin e)
				{
					TierXPin component = e.GetComponent<TierXPin>();
					return component != null && component.pinDetails.ClickAction.themeToLoad == theme;
				});
				if (groupPin != null)
				{
					careerModeMapScreen.EventSelect.OnTierXPinPress(groupPin);
				}
			}
		}
		else if (this.CurrentThemeName != theme)
		{
			this.LoadMainOverviewTheme(delegate
			{
				this.OnGoWorldTourHubTheme(theme);
			});
		}
		else
		{
			this.OnGoWorldTourHub_Any();
		}
	}

	public void OnGoWorldTourHub_Any()
	{
		this.GoCareerModeMapScreen(1);
	}

	public bool TryLoadThemeTransition(OnTierXReady onTXReady)
	{
		if (this.ThemeDescriptor == null)
		{
			return false;
		}
		IGameState gameState = new GameStateFacade();
		foreach (ThemeTransition current in this.ThemeDescriptor.Transitions)
		{
			if (current.IsEligible(gameState))
			{
				this.LoadTheme(current.ThemeID, onTXReady, current.Option);
				return true;
			}
		}
		return false;
	}
}
