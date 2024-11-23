using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using EventPaneRestriction;
using Fabric;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CareerModeMapScreen : ZHUDScreen
{
	public const int FRIENDS_MAP_PANE = -2;

	public const int TIER_X_MAP_PANE = 1;

	private const float TutorialLadderRacesBubbleDelay = 0.4f;

    public DataDrivenPortrait WorldTourCar;

    public CareerModeMapEventSelect EventSelect;

	public GameObject EventPanePrefab;

	public Vector3 targetTierPosition;

	public static List<eCarTier> mapPaneToTier = new List<eCarTier>
	{
		eCarTier.TIER_1,
		eCarTier.TIER_2,
		eCarTier.TIER_3,
		eCarTier.TIER_4,
		eCarTier.TIER_5,
		eCarTier.TIER_X
	};

	//private static int _lastPaneSelected = 0;

    private static MapPaneType _lastPaneSelected;

	private static int _friendCarTier = 0;

	private Vector3 dragOffset = default(Vector3);

	private static bool _isDragging = false;

	public TextMeshProUGUI TierText;

    public TextMeshProUGUI TierDescriptionText;

    public TextMeshProUGUI ObjectiveText;

    //public Button StopUserInput;

	public Transform BottomRight;

	public Transform TopRight;

    //private FriendRanking friendRankingItem;

	private bool haveCheckedObjectiveSystem;

	private bool haveCheckedFuelPopDown;

    private bool m_loadingMap;

	public Transform TopOffset;

	public Transform BottomOffset;

    //private MapPinSelected pinSelected;

    //private MapPinSelected singlePlayerPinSelected;

    public Button WorkshopButton;

	private bool bIgnoreHardwareBackButton;

	private FlowConditionalBase screenConditional;

    private TierXPin _selectedTierXPin;

	private Transform MapPinBubbleTarget;

	private BubbleMessage MapPinBubble;

	private BubbleMessage DifficultyBubble;

	private bool MapPinBubbleShow;

	private bool DifficultyBubbleShow;

	private static eCarTier LastSelectedCarTier = eCarTier.TIER_1;

	private float GoToRaceAnimationTimer;


    #region Overrides of ZHUDScreen

    public override DashboardType DashboardType
    {
        get
        {
            if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstTutorialRace())
            {
                return DashboardType.None;
            }
            return base.DashboardType;
        }
    }

    #endregion

    public static CareerModeMapScreen Instance { get; private set; }

	public EventPane eventPane
	{
		get;
		private set;
	}

	public static eCarTier carTierSelected
	{
		get
		{
		    return eCarTier.TIER_1;//mapPaneToTier[Mathf.Max(mapPaneSelected, 0)];
		}
		set
		{
		    //mapPaneSelected = value > eCarTier.TIER_5 ? 1 : 0;
		}
	}


    public static MapPaneType mapPaneSelected { get; set; }

    //public static int mapPaneSelected
    //{
    //    get
    //    {
    //        return PlayerProfileManager.Instance.ActiveProfile.MapPaneSelected;
    //    }
    //    set
    //    {
    //        PlayerProfileManager.Instance.ActiveProfile.MapPaneSelected = value;
    //    }
    //}

    public static GameModeType CurrentGameMode
	{
		get
		{
            if (mapPaneSelected == MapPaneType.SinglePlayer)
            {
                return GameModeType.SP;
            }
            //if (mapPaneSelected == -1)
            //{
            //    return GameModeType.RTW;
            //}
            //if (mapPaneSelected == -2)
            //{
            //    return GameModeType.RYF;
            //}
            if (mapPaneSelected == MapPaneType.WorldTour)
            {
                return GameModeType.WT;
            }
            return GameModeType.UNKNOWN;
        }
    }

	public static int friendCarTier
	{
		get
		{
			return _friendCarTier;
		}
	}

	public static bool isDragging
	{
		get
		{
			return _isDragging;
		}
		private set
		{
			_isDragging = value;
		}
	}

	public override ScreenID ID
	{
		get
		{
			return ScreenID.CareerModeMap;
		}
	}

	public bool InPosition
	{
		get
		{
		    //return (this.targetTierPosition - this.tierPosition).sqrMagnitude < 0.0005f;
		    return false;
		}
	}

	public bool GoToRaceAnimating
	{
		get;
		private set;
	}

	protected override void Awake()
	{
	    if (Instance == null)
	        Instance = this;
		base.Awake();
	}

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        this.screenConditional = new MapScreenConditional();
        StreakManager.Reset();
        //Camera.main.GetComponent<HideCamera>().Hide();
        this.GoToRaceAnimating = false;
        var eventPaneInstance = Instantiate(this.EventPanePrefab) as GameObject;
        eventPaneInstance.transform.SetParent(transform, false);
        eventPaneInstance.rectTransform().anchoredPosition = Vector2.zero;
        //eventPaneInstance.transform.parent = MapScreenCache.GetMapTransform(0);
        this.eventPane = eventPaneInstance.GetComponent<EventPane>();
        this.eventPane.OnRacePressed += new OnEventPanePressed(this.OnEventStart);
        this.eventPane.OnRestrictionPressed += new OnEventPanePressed(this.EventSelect.OnRestrictionPressed);
        if (MapScreenCache.Map != null)
            MapScreenCache.Map.SetActive(true);
        this.EventSelect.RemoveAll(false);
        //GameObject item = UICacheManager.Instance.GetItem("Career/pinSelected", false);
        //this.singlePlayerPinSelected = item.GetComponent<MapPinSelected>();
        //this.pinSelected = this.singlePlayerPinSelected;
        //item.transform.parent = this.EventSelect.transform;
        //item.transform.localPosition = Vector3.zero;
        //      if (mapPaneSelected == -1)
        //{
        //	mapPaneSelected = 0;
        //}
        int highestUnlockedClass = (int) RaceEventQuery.Instance.getHighestUnlockedClass();
        if (!zAlreadyOnStack /*&& mapPaneSelected > highestUnlockedClass*/)
        {
            if (!TierXManager.forceCareerModePane)
            {
                //mapPaneSelected = highestUnlockedClass;
            }

            TierXManager.forceCareerModePane = false;
        }

        //this.UpdatePinSelectedGraphic(mapPaneSelected);
        this.OnPanelSelected(mapPaneSelected);
        this.EventSelect.TryRestoreSelectedEvent();
		if(this.eventPane!=null)
			MapScreenCache.OnEnterMap(this.eventPane.PaneWidth);
        //if (mapPaneSelected >= 0 && mapPaneSelected < 1)
        //{
        //          //this.Pagination.AnimateIn();
        //}
        //this.StopUserInput.gameObject.SetActive(false);
        //NativeEvents.fbDidLoginEvent += new NativeEvents_DelegateToken(this.fbDidLogin);
        this.screenConditional.EvaluateAll();
        CarInfoUI.Instance.SetCurrentCarIDKey(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
        this.bIgnoreHardwareBackButton = false;

        if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstTutorialRace())
        {
            FocusOnEvent(ProgressionMapPinEventType.TUTORIAL_RACES);
            EventSelect.GetMapPin(ProgressionMapPinEventType.TUTORIAL_RACES).SetHightlight(true);
        }
        else
        {
            if (mapPaneSelected == MapPaneType.SinglePlayer)
            {
                var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
                var currentCarInfo = CarDatabase.Instance.GetCar(currentCar.CarDBKey);
                if (currentCarInfo.IsWorldTour)
                {
                    FocusOnTier(5);
                }
                else
                {
                    var tier = CarDatabase.Instance.GetCar(currentCar.CarDBKey).BaseCarTier;
                    FocusOnTier((int)tier);
                }
            }
            else
            {
                MapCamera.MoveToPositionImmediately(EventSelect.WorldTourRaces,true);
            }
        }
    }

    public override void OnDeactivate()
	{
		if (Camera.main)
		{
            //Camera.main.GetComponent<HideCamera>().Show();
		}
		if (this.eventPane)
		{
			this.eventPane.OnRacePressed -= new OnEventPanePressed(this.OnEventStart);
			this.eventPane.OnRestrictionPressed -= new OnEventPanePressed(this.EventSelect.OnRestrictionPressed);
			Destroy(this.eventPane.gameObject);
			this.eventPane = null;
		}
        //MapScreenCache.Map.transform.parent = null;
	    if (MapScreenCache.Map != null)
	        MapScreenCache.Map.SetActive(false);
        //MapScreenCache.DestroyInternationalBackground();
		this.EventSelect.RemoveAll(true);
		//GestureEventSystem.Instance.Flick -= new GestureEventSystem.GestureEventHandler(this.OnFlick);
		//GestureEventSystem.Instance.DragUpdate -= new GestureEventSystem.GestureEventHandler(this.OnDragUpdate);
		//GestureEventSystem.Instance.DragComplete -= new GestureEventSystem.GestureEventHandler(this.OnDragEnd);
        //this.pinSelected = null;
        //if (this.singlePlayerPinSelected != null)
        //{
        //    //UICacheManager.Instance.ReleaseItem(this.singlePlayerPinSelected.gameObject);
        //}
		NativeEvents.fbDidLoginEvent -= new NativeEvents_DelegateToken(this.fbDidLogin);
		if (this.MapPinBubble != null)
		{
			this.MapPinBubble.KillNow();
			this.MapPinBubble = null;
			this.MapPinBubbleShow = false;
		}
		if (this.DifficultyBubble != null)
		{
			this.DifficultyBubble.KillNow();
			this.DifficultyBubble = null;
			this.DifficultyBubbleShow = false;
		}
	}

	private void SetupPeekingPanel(int panel)
	{
		if (panel >= 0 && panel < 1)
		{
            //this.Pagination.AnimateIn();
		}
		if (panel == 1)
		{
            //this.SetUpPeekingTierX(TierXManager.Instance.ThemeDescriptor);
		}
		else if (panel >= 0)
		{
			this.SetupPeekingTier((eCarTier)panel);
		}
		else if (panel == -2)
		{
			this.SetUpOnlinePeeking(CustomPin.CustomType.FriendRace);
		}
	}

	private void SetupPeekingTier(eCarTier zTier)
	{
		RaceEventData raceEventFromTier = this.GetRaceEventFromTier(zTier);
		bool flag = true;
		if (zTier != eCarTier.MAX_CAR_TIERS)
		{
			eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
			flag = (carTierSelected > highestUnlockedClass);
		}
		if (flag)
		{
			this.eventPane.OnStoryTierLocked((int)zTier);
		}
		else if (raceEventFromTier == null)
		{
			this.eventPane.OnGroupSelected(this.GetRaceGroupFromTier(zTier));
		}
		else
		{
			this.eventPane.OnEventSelected(raceEventFromTier);
		}
		this.SetupEventPaneTransform((int)zTier);
	}

	private void SetUpOnlinePeeking(CustomPin.CustomType customSetUp)
	{
		this.eventPane.SetUpForOnlineEvent(customSetUp);
		this.SetupEventPaneTransform(-1);
        //this.Pagination.AnimateOut();
	}

	private void SetupEventPaneTransform(int transformMapIndex)
	{
		if (this.eventPane == null)
		{
			return;
		}
        //this.eventPane.transform.parent = MapScreenCache.GetMapTransform(transformMapIndex);
        //float y = -CommonUI.Instance.NavBar.GetHeightTight();
        //this.eventPane.transform.localPosition = new UnityEngine.Vector3(GUICamera.Instance.ScreenWidth - this.eventPane.PaneWidth + 0.02f, y, -0.5f);
	}

	public static Vector3 GetTierPositionOffset(eCarTier zTier)
	{
		return GetPanelPositionOffset((int)zTier);
	}

	public static Vector3 GetPanelPositionOffset(int panel)
	{
	    //return new UnityEngine.Vector3((float)panel * -GUICamera.Instance.ScreenWidth, 0f, 0f);
	    return new Vector3();
	}

	private void FixedUpdate()
	{
        //UnityEngine.Vector3 vector = this.targetTierPosition - this.tierPosition + this.dragOffset;
        //if (vector.sqrMagnitude < 0.0005f)
        //{
        //    this.tierPosition += vector;
        //}
        //else
        //{
        //    this.tierPosition += vector * 0.2f;
        //}
        //this.tierPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(this.tierPosition);
        //if (!CareerModeMapScreen.isDragging)
        //{
        //    this.dragOffset = UnityEngine.Vector3.zero;
        //}
	}

	protected override void Update()
	{
        base.Update();
		if (!this.haveCheckedObjectiveSystem)
		{
			this.haveCheckedObjectiveSystem = true;
		}
		if (!this.haveCheckedFuelPopDown && PlayerProfileManager.Instance.ActiveProfile.RacesEntered < 1 && PlayerProfileManager.Instance.ActiveProfile.FuelPips > 0)
		{
			//float num = -0.3f;
            //float num2 = GUICamera.Instance.ScreenWidth / 4f;
            //if (LocalisationManager.GetSystemLanguage() == LocalisationManager.ISO6391.JA)
            //{
            //    num2 += GUICamera.Instance.ScreenWidth * 0.025f;
            //    num = -0.45f;
            //}
			//float newX = num;
            //NavBarInfoPane navBarInfoPane = CommonUI.Instance.NavBarInfoManager.NewForScreen("TEXT_FUEL_TUTORIAL_POPDOWN", false, num2, ScreenID.CareerModeMap, null, true);
            //if (navBarInfoPane != null)
            //{
            //    navBarInfoPane.MoveNipple(newX);
            //}
			this.haveCheckedFuelPopDown = true;
		}
		if (this.GoToRaceAnimating)
		{
			this.HandleGoToRaceAnimation();
		}
		FlowConditionBase nextValidCondition = this.screenConditional.GetNextValidCondition();
		if (nextValidCondition != null)
		{
            PopUp popup = nextValidCondition.GetPopup();
            /*bool flag = */PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }
	}

	public override void RequestBackup()
	{
	    if (m_loadingMap)
	    {
	        return;
	    }
        if (bIgnoreHardwareBackButton)
            return;
	    if (this.eventPane != null && this.eventPane.IsVisible)
	    {
	        eventPane.Hide();
	    }
	    else
	    {
	        MapCamera.Instance.Interactable = true;

	        if (this.eventPane.GoRaceBubble != null)
	        {
	            this.eventPane.GoRaceBubble.Dismiss();
	        }
            //if (mapPaneSelected == -2)
            //{
            //    this.RefreshTierForReturn();
            //    return;
            //}
            if (CareerModeMapScreen.mapPaneSelected == MapPaneType.WorldTour  && GameDatabase.Instance.Career.IsWorldTourUnlocked() && TierXManager.Instance.ShouldLoadThemeOnBackOut())
            {
                m_loadingMap = true;
                LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 1), .3F, () =>
                {
                    MapScreenCache.SetMap(MapPaneType.SinglePlayer);
                    TouchManager.DisableButtonsFor(0.5f);
                    this.EventSelect.RemoveAll(true);
                    this.SingleplayerPanelSetup();
                    TierXManager.Instance.LoadBackOutTheme(null);
                    this.EventSelect.ResetThemeAnimation();
                    mapPaneSelected = MapPaneType.SinglePlayer;
                    LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 0), 0.3F, () =>
                    {
                        m_loadingMap = false;
                    });
                });
                return;
            }
            base.RequestBackup();
	    }
	}

	private void RefreshTierForReturn()
	{
		//if (mapPaneSelected > 0)
		//{
		//	if (!this.GoToRaceAnimating)
		//	{
		//		this.OnTierSelected(carTierSelected);
		//	}
		//}
		//else
		//{
		//	this.OnTierSelected(LastSelectedCarTier);
		//}
	}

	public void GoToTier1()
	{
        //if (ScreenManager.Active.CurrentScreenName != (int) ScreenID.CareerModeMap)
        //{
        //    return;
        //}
		this.OnTierSelected(eCarTier.TIER_1);
	}

	public void GoToCarTier()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			this.GoToTier1();
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup == null)
		{
			this.GoToTier1();
			return;
		}
		eCarTier baseCarTier = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.BaseCarTier;
		eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
		if (highestUnlockedClass == eCarTier.TIER_X)
		{
			this.OnTierSelected(eCarTier.TIER_X);
		}
		else if (baseCarTier < highestUnlockedClass)
		{
			this.OnTierSelected(baseCarTier);
		}
		else
		{
			this.OnTierSelected(highestUnlockedClass);
		}
	}

	public void OnTierSelected(eCarTier zTier)
	{
		//this.OnPanelSelected((int)zTier);
	}

	public void ResetHighlight()
	{
        //this.pinSelected.gameObject.SetActive(true);
        //this.pinSelected.OnActivate();
	}

	private void UpdatePinSelectedGraphic(int panel)
	{
        //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Invisible);
        //this.pinSelected.gameObject.SetActive(false);
        //this.pinSelected = this.singlePlayerPinSelected;
        //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Visible);
	}

	public void OnPanelSelected(MapPaneType mapPaneType)
	{
        //if (mapPaneSelected == -2 && panel >= 0)
        //{
        //          //this.Pagination.SetPage(-2);
        //          //this.Pagination.AnimateIn();
        //}
        //if (mapPaneSelected == 1)
        //{
        //          //this.Pagination.SetPage(1);
        //}
        mapPaneSelected = mapPaneType;
        //this.pinSelected.gameObject.SetActive(true);
        //this.pinSelected.OnActivate();
        //this.Pagination.SetPage(panel);
        MapScreenCache.SetMap(mapPaneType);
        MapScreenCache.UpdateWorldTourLogo();

		this.EventSelect.RemoveAll(false);
        //this.SetUpRankBar(panel);
        this.DecidePanelToSetUp(mapPaneType);
		//if (panel >= 0 && panel <= 4)
		//{
		//	_friendCarTier = panel;
		//}
		GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(this.ID, null);
	}

    private void DecidePanelToSetUp(MapPaneType mapPaneType)
    {
        //if (selectedPanel == -2)
        //{
        //    this.SetupFriendsPanel();
        //}
        //else if (selectedPanel == 1)
        //{
        //    this.TierXPanelSetup(null);
        //}
        //else
        //{
        //    this.SingleplayerPanelSetup();
        //}

        bool isWorldTourUnlocked = GameDatabase.Instance.Career.IsWorldTourUnlocked();

        if (TierXManager.Instance.IsOverviewThemeActive())
            this.SingleplayerPanelSetup();

        if (isWorldTourUnlocked)
        {
            this.TierXPanelSetup(null);
        }

        //base.StopCoroutine("RemoveOldPins");
        //base.StartCoroutine("RemoveOldPins");


        //_lastPaneSelected = selectedPanel;
        _lastPaneSelected = mapPaneType;
    }

    private void SingleplayerPanelSetup()
	{
		this.ShowPinSelection();
		eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
		bool flag = carTierSelected > highestUnlockedClass;
		if (!flag)
		{
            GameDatabase.Instance.ProgressionMapPins.Populate(this.EventSelect);
		}
		//if (_lastPaneSelected == -2)
		//{
  //          //this.tierPosition = this.targetTierPosition;
		//}
		if (flag)
		{
			this.TierText.gameObject.SetActive(false);
			this.TopOffset.gameObject.SetActive(false);
			this.ObjectiveText.gameObject.SetActive(false);
		}
		else
		{
            this.SetupTierText(CarInfo.ConvertCarTierEnumToString((eCarTier)0), false);
			this.SetupObjectiveText();
            //this.TopOffset.gameObject.SetActive(false);
		}
        //this.SetupPeekingPanel(panel);
  //      CoroutineManager.Instance.StopCoroutine(RemoveOldPins());
		//CoroutineManager.Instance.StartCoroutine(RemoveOldPins());
		if (flag)
		{
			this.eventPane.OnStoryTierLocked((int)carTierSelected);
		}
		else
		{
            //this.AutoSelectGroupOrEvent();
            //this.eventPane.SkipAnimationIn();
		}
        MapScreenCache.WorldTourBossPin.DisablePinPieces();
    }

	public void CheckForMapPinsBubble()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 0 && !activeProfile.HasCompletedFirstCrewRace())
		{
			this.MapPinBubbleTarget = this.EventSelect.GetEventPinMatchingCondition((EventPin e) => e.EventData.IsLadderEvent()).transform;
			this.ShowBubbleOnLadderRacesMapPin(false);
		}
		else if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 1 &&
		         !activeProfile.HasVisitedMechanicScreen && !activeProfile.HasCompletedFirstCrewRace())
		{
		    CustomPin mechanicPin = this.EventSelect.GetMechanicPin();
		    if (mechanicPin == null)
		    {
		        return;
		    }
		    Action<BubbleMessage> callback = delegate(BubbleMessage q)
		    {
		        if (this.MapPinBubble != null || !this.MapPinBubbleShow)
		        {
		            q.KillNow();
		            return;
		        }
		        this.MapPinBubble = q;
		        this.MapPinBubble.transform.parent.parent = this.MapPinBubbleTarget.parent;
		    };
		    this.MapPinBubbleTarget = mechanicPin.transform;
		    base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.4f, "TEXT_TAP_TO_ACCESS_MECHANIC", false,
		        this.MapPinBubbleTarget, new Vector3(0f, 0.15f, -0.5f), BubbleMessage.NippleDir.DOWN, 0.5f,
		        BubbleMessageConfig.ThemeStyle.SMALL, false, callback));
		    this.MapPinBubbleShow = true;
		}
		else if (activeProfile.DailyBattlesLastEventAt == DateTime.MinValue && !activeProfile.HasCompletedFirstCrewRace())
		{
		    EventPin eventPinMatchingCondition =
		        this.EventSelect.GetEventPinMatchingCondition((EventPin e) => e.EventData.IsDailyBattle());
		    if (eventPinMatchingCondition == null)
		    {
		        return;
		    }
		    this.MapPinBubbleTarget = eventPinMatchingCondition.transform;
		    this.MapPinBubbleTarget.Translate(new Vector3(0f, 0f, -0.25f));
		    this.ShowBubbleOnDailyBattlesMapPin(false);
		}
	}

	private void ShowBubbleOnPin(bool selected, Vector3 pos, BubbleMessage.NippleDir dir)
	{
		if (!selected)
		{
		    if (this.MapPinBubble == null && !this.MapPinBubbleShow)
		    {
		        Action<BubbleMessage> callback = delegate(BubbleMessage q)
		        {
		            if (this.MapPinBubble != null || !this.MapPinBubbleShow)
		            {
		                q.KillNow();
		                return;
		            }
		            this.MapPinBubble = q;
		            this.MapPinBubble.transform.parent.parent = this.MapPinBubbleTarget.parent;
		        };
		        base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.4f, "TEXT_TAP_TO_SELECT", false,
		            this.MapPinBubbleTarget, pos, dir, 0.5f, BubbleMessageConfig.ThemeStyle.SMALL,false, callback));
		        this.MapPinBubbleShow = true;
		    }
		}
		else
		{
			if (this.MapPinBubble != null)
			{
				this.MapPinBubble.Dismiss();
			}
			this.MapPinBubble = null;
			this.MapPinBubbleShow = false;
		}
	}

	private void ShowBubbleOnLadderRacesMapPin(bool ladderSelected)
	{
        //this.ShowBubbleOnPin(ladderSelected, new Vector3(0f, -0.2f, -0.5f), BubbleMessage.NippleDir.UP);
	}

	private void ShowBubbleOnDailyBattlesMapPin(bool dailyBattleSelected)
	{
        //this.ShowBubbleOnPin(dailyBattleSelected, new Vector3(0f, 0.16f, -0.5f), BubbleMessage.NippleDir.DOWN);
	}

	private void ShowBubbleOnLadderRacesDifficulty(bool ladderSelected)
	{
		if (ladderSelected)
		{
			CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
			if (careerModeMapScreen != null && this.DifficultyBubble == null && !this.DifficultyBubbleShow)
			{
				Action<BubbleMessage> callback = delegate(BubbleMessage q)
				{
					if (!this.DifficultyBubbleShow || this.DifficultyBubble != null)
					{
						q.KillNow();
						return;
					}
					this.DifficultyBubble = q;
				};
                base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.4f, "TEXT_BUBBLE_EASY_RACES", false, /*careerModeMapScreen.eventPane.DifficultyGraphic.transform*/careerModeMapScreen.eventPane.transform, new Vector3(0f, 0.075f, -1.05f), BubbleMessage.NippleDir.DOWN, 0.9f, BubbleMessageConfig.ThemeStyle.SMALL,false, callback));
				this.DifficultyBubbleShow = true;
			}
		}
		else
		{
			if (this.DifficultyBubble != null)
			{
				this.DifficultyBubble.Dismiss();
			}
			this.DifficultyBubble = null;
			this.DifficultyBubbleShow = false;
		}
	}

	private void SetupFriendsPanel()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!activeProfile.FriendTutorial_HasSeenFriendsPane)
		{
			activeProfile.FriendTutorial_HasSeenFriendsPane = true;
		}
		else if (!activeProfile.FriendTutorial_HasSupressedTutorial)
		{
			activeProfile.FriendTutorial_HasSupressedTutorial = true;
		}
        //this.targetTierPosition = CareerModeMapScreen.GetPanelPositionOffset(-1);
        //this.targetTierPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(this.targetTierPosition);
        //this.tierPosition = this.targetTierPosition;
        //this.SetupTierText(string.Empty, false);
        //MapScreenCache.MultiplayerGO.SetActive(true);
        //MapScreenCache.WorldTourBossPin.DisablePinPieces();
        //if (SocialController.Instance.isLoggedIntoFacebook && StarsManager.GetMyStarForCar("MiniCooperS_RWF") >= StarType.BRONZE)
        //{
        //    this.SetUpFriendsLeaderBoard();
        //}
        //else
        //{
        //    this.SetUpFriendsSignInScreen();
        //}
        //this.SetupPeekingPanel(-2);
        //this.eventPane.SkipAnimationIn();
        //base.StopCoroutine("RemoveOldPins");
        //base.StartCoroutine("RemoveOldPins");
        //if (!RYFStatusManager.NetworkStateValidToEnterRYF())
        //{
        //    PopUp popup = new PopUp
        //    {
        //        Title = "TEXT_WEB_REQUEST_STATUS_CODE_0",
        //        BodyText = "TEXT_CONNECT_SCREEN_INFO_ERROR_CONNECTING_TO_SERVICE",
        //        IsBig = true,
        //        ConfirmAction = new PopUpButtonAction(this.GoToTier1),
        //        ConfirmText = "TEXT_BUTTON_OK"
        //    };
        //    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
        //}
        //else if (SocialController.Instance.isLoggedIntoFacebook && !SocialController.Instance.RecievedFBUserInfo)
        //{
        //    SocialController.Instance.GetFacebookUserInfo();
        //    SocialController.Instance.GetFacebookUserPermissions();
        //    SocialController.Instance.GetFacebookFriendsList();
        //}
	}

	private void SetUpFriendsLeaderBoard()
	{
        //MapScreenCache.FriendsInviteScreenGO.SetActive(false);
        //MapScreenCache.FriendsLeaderboardGO.SetActive(true);
        //if (this.friendRankingItem == null)
        //{
        //    this.friendRankingItem = MapScreenCache.FriendsLeaderboardGO.GetComponent<FriendRanking>();
        //}
        //this.friendRankingItem.Populate();
        //this.ObjectiveText.gameObject.SetActive(false);
        //this.TopOffset.gameObject.SetActive(false);
	}

	private void SetUpFriendsSignInScreen()
	{
        //MapScreenCache.FriendsInviteScreenGO.SetActive(true);
        //FriendsSignInScreen component = MapScreenCache.FriendsInviteScreenGO.GetComponent<FriendsSignInScreen>();
        //component.SetUp();
        //MapScreenCache.FriendsLeaderboardGO.SetActive(false);
        //this.ObjectiveText.gameObject.SetActive(false);
        //this.TierText.gameObject.SetActive(false);
        //this.TopOffset.gameObject.SetActive(false);
	}

	private void fbDidLogin(string accessToken, string exprirationToken)
	{
		//if (_lastPaneSelected == -2)
		//{
		//	this.SetupFriendsPanel();
		//}
	}

	public void SetupTierText(string tierText, bool ToUpper = false)
	{
        //bool flag = !string.IsNullOrEmpty(tierText);
        //this.TierText.gameObject.SetActive(flag);
        //if (!flag)
        //{
        //    return;
        //}
        //if (ToUpper)
        //{
        //    this.TierText.text = LocalizationManager.GetTranslation(tierText).ToUpper();
        //}
        //else
        //{
        //    this.TierText.text = LocalizationManager.GetTranslation(tierText);
        //}
	}

	public void SetupTierDescriptionText(string tierDecriptionText, bool ToUpper = false)
	{
        //if (string.IsNullOrEmpty(tierDecriptionText))
        //{
        //    return;
        //}
        //if (ToUpper)
        //{
        //    this.TierDescriptionText.text = LocalizationManager.GetTranslation(tierDecriptionText).ToUpper();
        //}
        //else
        //{
        //    this.TierDescriptionText.text = LocalizationManager.GetTranslation(tierDecriptionText);
        //}
	}

	public void SetupObjectiveText()
	{
        //this.ObjectiveText.gameObject.SetActive(true);
        //string eligibleProgressionMapTextString = GameDatabase.Instance.ProgressionMapTexts.GetEligibleProgressionMapTextString();
        //if (!string.IsNullOrEmpty(eligibleProgressionMapTextString))
        //{
        //    this.ObjectiveText.text = eligibleProgressionMapTextString;
        //}
	}

	public void SetWorkshopPinActive(bool active)
	{
        //this.WorkshopButton.gameObject.SetActive(active);
	}

	public void OnMultiplayerModeSwitch()
	{
		if (!TouchManager.AttemptToUseButton("OnMultiplayerModeSwitch"))
		{
			return;
		}
        //ScreenManager.Instance.PushScreen(ScreenID.MultiplayerModeSelect);
	}

	public void OnWorkshopPinPress()
	{
		if (!TouchManager.AttemptToUseButton("OnWorkshopPinPress"))
		{
			return;
		}
		EventManager.Instance.PostEvent("MenuUpgradeAdd", EventAction.PlaySound, null, null);
        //base.RequestBackup();
	}

	public void OnFriendModeSwitch()
	{
		TouchManager.DisableButtonsFor(0.5f);
		this.ToggleBetweenMode(-2);
	}

	private void ToggleBetweenMode(int selectedMode)
	{
		//if (mapPaneSelected < 0)
		//{
		//	this.OnTierSelected(LastSelectedCarTier);
		//}
		//else
		//{
		//	this.OnPanelSelected(selectedMode);
		//}
	}

	private IEnumerator RemoveOldPins()
	{
	    yield return new WaitForSeconds(0.5F);
        EventSelect.RemoveOld();
	}

	public void OnCustomSelectedAsEvent(CustomPin.CustomType pinType)
	{
		this.eventPane.SetUpForOnlineEvent(pinType);
		//Vector3 panelPositionOffset = GetPanelPositionOffset(mapPaneSelected);
		//Vector2 zPosition = this.EventSelect.GetPinPositionWorldSpace(pinType) - new Vector2(panelPositionOffset.x, panelPositionOffset.y);
        //this.pinSelected.OnEventSelected(zPosition);
	}

	public void OnEventSelected(RaceEventData zRaceEventData, bool isAutoSelect = false)
	{
	    _selectedTierXPin = null;
        if (zRaceEventData.Parent is SMPRaceEvents)
	    {
            if (!PolledNetworkState.IsNetworkConnected)
            {
                PopUpDatabase.Common.ShowNoInternetConnectionPopup();
                return;
            }
            if (!ServerSynchronisedTime.Instance.IsServerTimeMatchClient)
            {
                PopUpDatabase.Common.ShowServerTimeMismatch();
                return;
            }

#if UNITY_EDITOR
	        if (GameDatabase.Instance.EventDebugConfiguration.ShowAllMapPins)
	        {
                ScreenManager.Instance.PushScreen(ScreenID.SMPLobby);
                return;


                //if (MultiplayerUtils.HasNeverTakenAMultiplayerRace())
                //{
                //    if (!SeasonUtilities.ValidForRTWAndDoWeHaveStatusAndStandings())
                //    {
                //        PopUpDatabase.Common.ShowCouldNotConnectPopup(null);
                //        return;
                //    }
                //    if (!SeasonServerDatabase.Instance.IsAnySeasonActive())
                //    {
                //        SeasonUtilities.ShowSeasonEndedPopUp(null);
                //        return;
                //    }
                //    RaceTheWorldEvents raceTheWorldEvents =
                //        GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.RaceTheWorldEvents;
                //    RaceEventInfo.Instance.PopulateFromRaceEvent(raceTheWorldEvents.RaceEventGroups[0].RaceEvents[0]);
                //    StreakManager.ResetOpponentsList();
                //    MultiplayerUtils.SelectedMultiplayerMode = MultiplayerMode.RACE_THE_WORLD;
                //    MultiplayerUtils.GoToMultiplayerHubScreen();
                //}
                //else
                //{
                //    ScreenManager.Instance.PushScreen(ScreenID.MultiplayerModeSelect);
                //}
                //return;
	        }
#endif

	        //if (GameDatabase.Instance.OnlineConfiguration.DebugSMPPRaces)
            //{
            //    ScreenManager.Instance.PushScreen(ScreenID.SMPLobby);
            //    return;
            //}
	        if (RaceEventQuery.Instance.getHighestUnlockedClass() < eCarTier.TIER_3)
	        {

	            PopUpManager.Instance.TryShowPopUp(new PopUp
	            {
	                Title = "TEXT_POPUPS_ONLINE_RACES_LOCK_TITLE",
	                BodyText = "TEXT_POPUPS_ONLINE_RACES_LOCK_BODY",
	                ConfirmText = "TEXT_BUTTON_OK",
	                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
	            }, PopUpManager.ePriority.Default, null);
	            return;
	        }


            CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            eCarTier currentTier = CarDatabase.Instance.GetCar(currentCar.CarDBKey).BaseCarTier;

            if (currentTier < eCarTier.TIER_3)
            {

                PopUpManager.Instance.TryShowPopUp(new PopUp
                {
                    Title = "TEXT_POPUPS_ONLINE_RACES_NEED_TIER3_OR_HIGHER_TITLE",
                    BodyText = "TEXT_POPUPS_ONLINE_RACES_NEED_TIER3_OR_HIGHER_BODY",
                    ConfirmText = "TEXT_BUTTON_OK",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                    ImageCaption = "TEXT_NAME_AGENT"
                }, PopUpManager.ePriority.Default, null);
                return;
            }

	        ScreenManager.Instance.PushScreen(ScreenID.SMPLobby);
	    }
	    else
	    {
            this.CheckMapPinBubbleOnEventSelected(zRaceEventData);
            this.CheckDifficultyBubbleOnEventSelected(zRaceEventData);
            if (zRaceEventData.IsFriendRaceEvent())
            {
            }
            if (!isAutoSelect)
            {
                MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
            }
            this.UpdateProgressionSnapshots(zRaceEventData);
            this.eventPane.OnEventSelected(zRaceEventData);
            this.eventPane.OnCustomPressTop = null;
            this.eventPane.OnCustomPressBot = null;
            //Vector3 panelPositionOffset = GetPanelPositionOffset(mapPaneSelected);
            //Vector2 zPosition = this.EventSelect.GetPinPositionWorldSpace(zRaceEventData, null) - new Vector2(panelPositionOffset.x, panelPositionOffset.y);
            //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Visible);
            //this.pinSelected.OnEventSelected(zPosition);
	    }
	}

	private void CheckMapPinBubbleOnEventSelected(RaceEventData zRaceEventData)
	{
		if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 0)
		{
			bool ladderSelected = zRaceEventData.IsLadderEvent();
			this.ShowBubbleOnLadderRacesMapPin(ladderSelected);
		}
		else if (PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue)
		{
			bool dailyBattleSelected = zRaceEventData.IsDailyBattle();
			this.ShowBubbleOnDailyBattlesMapPin(dailyBattleSelected);
		}
	}

	private void CheckDifficultyBubbleOnEventSelected(RaceEventData zRaceEventData)
	{
		Tier1 tier = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1;
		if (tier.LadderEvents.NumEventsComplete() == 0 && tier.CrewBattleEvents.NumEventsComplete() == 0)
		{
			//bool ladderSelected = zRaceEventData.IsLadderEvent();
            //this.ShowBubbleOnLadderRacesDifficulty(ladderSelected);
		}
	}

	public void OnGroupSelected(RaceEventGroup zRaceEventGroup)
	{
	    _selectedTierXPin = null;
        this.eventPane.OnGroupSelected(zRaceEventGroup);
	    //this.ShowBubbleOnLadderRacesMapPin(false);
	    //this.ShowBubbleOnDailyBattlesMapPin(false);
	    //this.ShowBubbleOnLadderRacesDifficulty(false);
	    //Vector3 tierPositionOffset = GetTierPositionOffset(carTierSelected);
	    //Vector2 zPosition = this.EventSelect.GetPinPositionWorldSpace(null, zRaceEventGroup) - new Vector2(tierPositionOffset.x, tierPositionOffset.y);
	    //this.pinSelected.OnEventSelected(zPosition);
	}

	public void OnAutoCompletePress(RaceEventData zRaceEventData)
	{
		this.OnConfirmedEventStart(zRaceEventData, true);
		RaceResultsData raceResultsData = new RaceResultsData();
		raceResultsData.IsWinner = true;
		raceResultsData.RaceTime = 10f;
		RaceResultsData raceResultsData2 = new RaceResultsData();
		raceResultsData2.IsWinner = false;
		raceResultsData2.RaceTime = 15f;
		PlayerProfileManager.Instance.ActiveProfile.RaceCompleted(raceResultsData, raceResultsData2);
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private void UpdateProgressionSnapshots(RaceEventData race)
	{
		if (MapScreenCache.InternationalEventHubGO != null)
		{
            //EventHubBackgroundManager component = MapScreenCache.InternationalEventHubGO.GetComponent<EventHubBackgroundManager>();
            //if (component != null)
            //{
            //    component.UpdateProgressionSnapshots(race);
            //}
		}
	}

	public void OnEventStart(RaceEventData zRaceEventData)
	{
		if (mapPaneSelected == MapPaneType.SinglePlayer && TierXManager.Instance.IsOverviewThemeActive()
		    && this._selectedTierXPin != null)
		{
            //if (this._selectedTierXPin == null)
            //{
            //    return;
            //}
            if (this._selectedTierXPin.pinDetails != null && !this._selectedTierXPin.pinDetails.IsLocked())
            {
                m_loadingMap = true;
                LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 1), .3F, () =>
                {
                    MapScreenCache.SetMap(MapPaneType.WorldTour);
                    this._selectedTierXPin.LoadTheme(() =>
                    {
                        MapScreenCache.UpdateWorldTourLogo();
                    });
                    LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 0), 0.3F, () =>
                    {
                        m_loadingMap = false;
                    });
                });
            }
        }
		else
		{
			if (mapPaneSelected == MapPaneType.WorldTour && !TierXManager.Instance.IsReady())
			{
				return;
			}
			if (zRaceEventData == null)
			{
				return;
			}
			if (PopUpManager.Instance.WaitingToShowPopup)// || PopUpManager.Instance.isShowingPopUp)
			{
				return;
			}
			if (zRaceEventData.IsFriendRaceEvent())
			{
				if (!PlayerProfileManager.Instance.ActiveProfile.IsCarOwned("MiniCooperS_RWF"))
				{
                    //if (SocialController.Instance.isLoggedIntoFacebook)
                    //{
                    //    ScreenManager.Instance.SwapScreen(ScreenID.FriendUnlock);
                    //    Log.AnEvent(Events.rwf_tut_002);
                    //    return;
                    //}
					//if (mapPaneSelected == -2)
					//{
					//	Log.AnEvent(Events.rwf_tut_002a);
					//}
					//else
					//{
					//	Log.AnEvent(Events.rwf_tut_002);
					//}
				}
                //else if (StarsManager.GetMyStarForCar("MiniCooperS_RWF") < StarType.BRONZE && SocialController.Instance.isLoggedIntoFacebook)
                //{
                //    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                //    activeProfile.SelectCar("MiniCooperS_RWF");
                //    CarInfoUI.Instance.SetCurrentCarIDKey("MiniCooperS_RWF");
                //    ScreenManager.Instance.PushScreen(ScreenID.FriendCarLeaderboard);
                //    return;
                //}
			}
			//if (zRaceEventData.IsWorldTourRace())
			//{
   //             PinDetail worldTourPinPinDetail = zRaceEventData.GetWorldTourPinPinDetail();
   //             if (worldTourPinPinDetail.WorldTourScheduledPinInfo.ChoiceScreen != null)
   //             {
   //                 WorldTourChoiceScreen.SetupChoice(worldTourPinPinDetail.WorldTourScheduledPinInfo);
   //                 ScreenManager.Instance.PushScreen(ScreenID.WorldTourChoice);
   //                 ScreenManager.Instance.UpdateImmediately();
   //                 return;
   //             }
   //         }
			if (zRaceEventData.IsFriendRaceEvent())
			{
				RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
				PlayerProfile activeProfile2 = PlayerProfileManager.Instance.ActiveProfile;
				if (activeProfile2.FriendTutorial_HasSupressedTutorial)
				{
					activeProfile2.FriendTutorial_HasSupressedTutorial = false;
					PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
				}
				//bool flag = mapPaneSelected == -2;
				//if (flag)
				//{
    //                if (SocialController.Instance.isLoggedIntoFacebook)
    //                {
    //                    FriendTimeListScreen.SelectedCarTier = (eCarTier)CareerModeMapScreen._friendCarTier;
    //                    ScreenManager.Instance.PushScreen(ScreenID.FriendTimeList);
    //                }
    //                else
    //                {
    //                    ScreenManager.Instance.PushScreen(ScreenID.FriendFacebookSignin);
    //                }
    //            }
				//else
				//{
				//	this.OnFriendModeSwitch();
				//}
			}
			else if (zRaceEventData.IsRelay)
			{
				if (zRaceEventData.IsRandomRelay() && !FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(zRaceEventData)))
				{
				    ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
					return;
				}
				RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
                if (!CrewProgressionSetup.PreRaceSetupForNarrativeScene(ScreenID.Invalid))
                {
                    ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
                }
			}
			else
			{
				this.OnConfirmedEventStart(zRaceEventData, false);
			}
		}
	}

	private bool IsSwipeDisabledForTierX()
	{
		return mapPaneSelected == MapPaneType.WorldTour && !TierXManager.Instance.CanSwipe();
	}

	private void OnFlick(GenericTouch zTouch)
	{
		if (this.IsSwipeDisabledForTierX())
		{
			return;
		}
		if (mapPaneSelected < 0)
		{
			return;
		}
		if (zTouch.AverageEndingVelocity.x > 0f)
		{
			if (mapPaneSelected <= 0)
			{
				return;
			}
			this.OnPreviousTier();
		}
		else if (zTouch.AverageEndingVelocity.x < 0f)
		{
			//if (mapPaneSelected >= 1)
			//{
			//	return;
			//}
			this.OnNextTier();
		}
	}

	//private void OnDragUpdate(GenericTouch zTouch)
	//{
	//	if (this.IsSwipeDisabledForTierX())
	//	{
	//		return;
	//	}
	//	Vector2 deltaPosition = zTouch.DeltaPosition;
	//	this.dragOffset += new Vector3(deltaPosition.x / 200f, 0f, 0f) * 0.3f;
	//	if (mapPaneSelected < 0)
	//	{
	//		this.dragOffset = Vector3.zero;
	//		isDragging = false;
	//		return;
	//	}
	//	if (mapPaneSelected == 0 && this.dragOffset.x > 0f)
	//	{
	//		this.dragOffset.x = 0f;
	//	}
	//	isDragging = true;
	//}

	//private void OnDragEnd(GenericTouch zTouch)
	//{
	//	isDragging = false;
	//}

	private void OnPreviousTier()
	{
		EventManager.Instance.PostEvent("MapSwipe", EventAction.PlaySound, null);
		this.OnPanelSelected(mapPaneSelected - 1);
	}

	private void OnNextTier()
	{
		EventManager.Instance.PostEvent("MapSwipe", EventAction.PlaySound, null);
		this.OnPanelSelected(mapPaneSelected + 1);
	}

    private void OnConfirmedEventStart(RaceEventData zRaceEventData, bool zSkipRace)
	{
		if (this.GoToRaceAnimating)
		{
			return;
		}
		if (!zRaceEventData.ForceUserInCar)
		{
		    CommonUI.Instance.FuelStats.FuelLockedState(true);
			if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(zRaceEventData)))
			{
			    CommonUI.Instance.FuelStats.FuelLockedState(false);
                ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
				return;
			}
		}
		this.GoToRaceAnimationTimer = 0f;
		this.GoToRaceAnimating = true;
        //this.StopUserInput.gameObject.SetActive(true);
		TouchManager.Instance.GesturesEnabled = false;
		MenuAudio.Instance.playSound(AudioSfx.StartTheRace);
		RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
		this.bIgnoreHardwareBackButton = true;
	}

    public override bool IgnoreHardwareBackButton()
	{
		return this.bIgnoreHardwareBackButton;
	}

	protected override void OnEnterPressed()
	{
		if (this.eventPane != null)
		{
			this.eventPane.OnRacePress();
		}
	}

	private void GoToRaceAnimationFinished()
	{
        bool NotCrewProgression = !CrewProgressionSetup.PreRaceSetupCrewProgressionScreen();
        NotCrewProgression &= !CrewProgressionSetup.PreRaceSetupForNarrativeScene(ScreenID.Invalid);
		TouchManager.Instance.GesturesEnabled = true;
		if (NotCrewProgression)
		{
            PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
            if (worldTourPinPinDetail == null || !worldTourPinPinDetail.ActivateVSLoadingScreen())
            {
                ScreenManager.Instance.PushScreen(ScreenID.VS);
                //SceneManagerFrontend.ButtonStart();
            }
        }
		//else
		//{
  //          //ScreenManager.Active.openTopScreen();
		//}
	}

	private void AutoSelectGroupOrEvent()
	{
		RaceEventData raceEventData = null;
		bool flag = false;
		if (this.EventSelect.HighlightedPin == null)
		{
			RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		    if (currentEvent != null && !currentEvent.IsRaceTheWorldOrClubRaceEvent())
		    {
		        raceEventData = RaceEventQuery.Instance.GetNextEventOnPin(currentEvent);
		    }
		}
		else if (this.EventSelect.HighlightedPin.IsEventPin)
		{
			if (this.EventSelect.HighlightedPin.EventData != null)
			{
				raceEventData = RaceEventQuery.Instance.GetNextEventOnPin(this.EventSelect.HighlightedPin.EventData);
				flag = true;
			}
		}
		else if (this.EventSelect.HighlightedPin.IsGroupPin && this.EventSelect.HighlightedPin.GroupData != null)
		{
			raceEventData = RaceEventQuery.Instance.GetNextEventOnPin(this.EventSelect.HighlightedPin.GroupData.RaceEvents[0]);
			flag = true;
		}
		bool flag3 = true;
		if (raceEventData != null && raceEventData.Parent != null)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup == null)
			{
				flag3 = false;
			}
			else
			{
				eCarTier baseCarTier = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.BaseCarTier;
				eCarTier carTier = raceEventData.Parent.GetTierEvents().GetCarTier();
				if (!flag && carTier != baseCarTier)
				{
					flag3 = false;
				}
				if (carTier != carTierSelected)
				{
					flag3 = false;
				}
			}
		}
		if (raceEventData != null && raceEventData.Parent != null && raceEventData.Group != null && flag3)
		{
			bool flag4 = raceEventData.IsDifficultySelectEvent();
			eCarTier carTier2 = raceEventData.Group.Parent.GetTierEvents().GetCarTier();
			if (carTier2 != carTierSelected)
			{
				this.OnTierSelected(carTier2);
			}
			else if (flag4)
			{
				this.OnGroupSelected(raceEventData.Group);
			}
			else
			{
				this.OnEventSelected(raceEventData, true);
			}
		}
		else if (raceEventData != null && raceEventData.IsFriendRaceEvent())
		{
			this.OnEventSelected(raceEventData, true);
		}
		else
		{
			this.AutoSelectFromTier(carTierSelected);
		}
	}

	private void AutoSelectFromTier(eCarTier zTier)
	{
		RaceEventData raceEventFromTier = this.GetRaceEventFromTier(carTierSelected);
		if (raceEventFromTier == null)
		{
			this.OnGroupSelected(this.GetRaceGroupFromTier(carTierSelected));
		}
		else
		{
			this.OnEventSelected(raceEventFromTier, false);
		}
	}

	private RaceEventData GetRaceEventFromTier(eCarTier zTier)
	{
		BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zTier);
		RaceEventData crewBattleEvent = RaceEventQuery.Instance.GetCrewBattleEvent(tierEvents, false);
		if (crewBattleEvent != null)
		{
			return crewBattleEvent;
		}
		RaceEventData ladderEvent = RaceEventQuery.Instance.GetLadderEvent(tierEvents, false);
		if (ladderEvent != null)
		{
			return ladderEvent;
		}
		return null;
	}

	private RaceEventGroup GetRaceGroupFromTier(eCarTier zTier)
	{
		return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zTier).RegulationRaceEvents.RaceEventGroups[0];
	}

	public void OnCustomSelected(CustomPin.CustomType zType,string pinName)
	{
	    switch (zType)
	    {
	        case CustomPin.CustomType.Mechanic:
	            ScreenManager.Instance.PushScreen(ScreenID.Mechanic);
	            break;
	        case CustomPin.CustomType.RaceTheWorld:
	        case CustomPin.CustomType.ClubRacing:
	        case CustomPin.CustomType.FriendRace:
	            this.OnCustomSelectedAsEvent(zType);
	            break;
	        case CustomPin.CustomType.PrizeList:
	            ScreenManager.Instance.PushScreen(ScreenID.PrizeList);
	            break;
	        case CustomPin.CustomType.Leaderboards:
	            ScreenManager.Instance.PushScreen(ScreenID.Leaderboards);
	            break;
	        case CustomPin.CustomType.SeasonInfo:
	            ScreenManager.Instance.PushScreen(ScreenID.SeasonInfo);
	            break;
	        case CustomPin.CustomType.WorldTourFake:
	            PopUp popUp = new PopUp()
	            {
	                Title = "TEXT_POPUP_WORLD_TOUR_TITLE_LOCKED",
	                BodyText = string.Format("TEXT_POPUP_WORLD_TOUR_{0}_BODY_LOCKED", pinName),
	                ConfirmText = "TEXT_BUTTON_OK",
                    IsBig = true,
                    ImageCaption = "TEXT_NAME_AGENT",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab
                };
	            PopUpManager.Instance.TryShowPopUp(popUp);
	            break;
        }
	}

	private void HandleGoToRaceAnimation()
	{
        this.GoToRaceAnimationTimer += Time.deltaTime;
        if (this.GoToRaceAnimationTimer >0.2F)// 1.8f)
        {
            this.GoToRaceAnimating = false;
            this.GoToRaceAnimationTimer = 0f;
            this.GoToRaceAnimationFinished();
        }
	}

	private void GoRaceOrGetCashElite()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num = StreakManager.RefreshCostCash();
		if (activeProfile.GetCurrentCash() < num)
		{
			MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(new ItemTypeId("race"), new ItemCost
			{
				CashCost = num
			}, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY", null, null, null, null);
		}
		else
		{
			this.eventPane.StartRace();
		}
	}

	private void SetUpPeekingTierX(ThemeLayout tierTheme)
	{
		this.SetupEventPaneTransform(1);
	}

	public void ShowPinSelection()
	{
        //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Visible);
	}

	public void HidePinSelection()
	{
        //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Invisible);
	}

	public void DisablePinSelection()
	{
        //this.pinSelected.gameObject.SetActive(false);
	}

	public void EnablePinSelection()
	{
        //this.pinSelected.gameObject.SetActive(true);
	}

	public void OnTierXGroupSelected(EventPin eventPin, bool isAutoSelect = false)
	{
		RaceEventGroup groupData = eventPin.GroupData;
		this.eventPane.OnGroupSelected(groupData);
		this.UpdateProgressionSnapshots(eventPin.EventData);
		this.CheckTierXMapPinBubbles(eventPin, isAutoSelect);
		if (this.ShouldHighlightTierXPin(eventPin))
		{
            //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Visible);
            //Vector3 localPosition = eventPin.gameObject.transform.localPosition;
            //this.pinSelected.OnEventSelected(new Vector2(localPosition.x, localPosition.y));
		}
		else
		{
            //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Invisible);
		}
	}

	public void OnTierXEventSelected(EventPin eventPin, bool isAutoSelect = false)
	{
		RaceEventData eventData = eventPin.EventData;
		TierXManager.Instance.OnEventSelected();
		if (!isAutoSelect)
		{
			MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
		}
		this.UpdateProgressionSnapshots(eventData);
		this.eventPane.OnEventSelected(eventData);
		this.eventPane.OnCustomPressTop = null;
		this.eventPane.OnCustomPressBot = null;
		this.CheckTierXMapPinBubbles(eventPin, isAutoSelect);
		if (this.ShouldHighlightTierXPin(eventPin))
		{
            //if (eventPin.EventData.GetWorldTourPinPinDetail().ShowSelectionArrow)
            //{
            //    this.pinSelected.SetVisible(MapPinSelected.eVisibility.Visible);
            //}
            //else
            //{
            //    this.pinSelected.SetVisible(MapPinSelected.eVisibility.VisibleWithoutArrow);
            //}
            //Vector3 localPosition = eventPin.gameObject.transform.localPosition;
            //this.pinSelected.OnEventSelected(new Vector2(localPosition.x, localPosition.y));
		}
		else
		{
            //this.pinSelected.SetVisible(MapPinSelected.eVisibility.Invisible);
		}
	}

	private bool ShouldHighlightTierXPin(EventPin eventPin)
	{
		//string currentThemeName = TierXManager.Instance.CurrentThemeName;
		//IGameState gameState = new GameStateFacade();
        //return TierXManager.Instance.ThemeDescriptor.EventIDsForAnimation.Count<string>() < 1 || gameState.GetWorldTourThemeCompletionLevel(currentThemeName) >= ThemeCompletionLevel.LEVEL_4;
	    return false;
	}

	private void CheckTierXMapPinBubbles(EventPin selectedPin, bool isAutoSelect = false)
	{
        //We comment this code because we dont want any bubble on pin or eventpane
		//List<EventPin> eventPins = this.EventSelect.GetEventPins();
		//foreach (EventPin current in eventPins)
		//{
		//	if (!current.Equals(selectedPin) && !current.IsBubbleMessageShown())
		//	{
		//		current.ShowBubbleMessage((!isAutoSelect) ? 0.25f : 0.75f);
		//	}
		//}
		selectedPin.DismissBubble();
	}

    public void SetUpTierXPanePreview(EventPin eventPin, TierXPin tierXPin)
    {
        bool isUnlocked = !tierXPin.pinDetails.IsLocked();
        //UnityEngine.Vector3 localPosition = eventPin.gameObject.transform.localPosition;
        //this.pinSelected.OnEventSelected(new UnityEngine.Vector2(localPosition.x, localPosition.y));
        if (!isUnlocked)
        {
            this.eventPane.OnTierXThemeLocked(tierXPin, TierXManager.Instance.ThemeDescriptor.Colour.AsUnityColor());
        }
        else
        {
            this.eventPane.OnTierXthemeUnlocked(tierXPin, TierXManager.Instance.ThemeDescriptor.Colour.AsUnityColor());
        }
        this.eventPane.ClearEventData();
        TextureDetail textureDetails = tierXPin.pinDetails.GetTextureDetails(PinDetail.TextureKeys.CarRender);
        if (textureDetails != null)
        {
            //this.WorldTourCar.Init(textureDetails.GetName(), string.Empty, null);
            //this.WorldTourCar.gameObject.transform.localPosition = textureDetails.Offset.AsUnityVector3();
            //this.WorldTourCar.gameObject.transform.localScale = textureDetails.Scale.AsUnityVector3();
        }
        else
        {
            //this.WorldTourCar.gameObject.SetActive(false);
        }
        this._selectedTierXPin = tierXPin;
        eventPane.Show();
    }

    private void LoadTierXJson(int panel, TierXManager.OnTierXReady onTXready)
    {
        TierXManager.Instance.LoadTierXJson(delegate
        {
            this.TierXPanelSetupCompete(panel);
            if (onTXready != null)
            {
                onTXready();
            }
        });
    }

    public void TierXPanelSetup(TierXManager.OnTierXReady onTXready)
    {
        bool isWorldTourUnlocked = GameDatabase.Instance.Career.IsWorldTourUnlocked();
        int panel = 1;
        if (isWorldTourUnlocked && !TierXManager.Instance.TryLoadThemeTransition(delegate
        {
            this.LoadTierXJson(panel, onTXready);
        }))
        {
            if (!TierXManager.Instance.IsJsonLoaded)
            {
                this.LoadTierXJson(panel, onTXready);
            }
            else
            {
                this.TierXPanelSetupCompete(panel);
            }
        }
        if (ScreenManager.Instance.CurrentScreen != this.ID)
        {
            return;
        }
        this.targetTierPosition = CareerModeMapScreen.GetPanelPositionOffset(panel);
        //this.targetTierPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(this.targetTierPosition);
        //if (CareerModeMapScreen._lastPaneSelected == -2)
        //{
        //    //this.tierPosition = this.targetTierPosition;
        //}
        if (!isWorldTourUnlocked)
        {
            this.TierText.gameObject.SetActive(false);
            this.TopOffset.gameObject.SetActive(false);
            this.ObjectiveText.gameObject.SetActive(false);
        }
        this.SetupPeekingPanel(panel);
        //base.StopCoroutine("RemoveOldPins");
        //base.StartCoroutine("RemoveOldPins");
        if (!isWorldTourUnlocked)
        {
            this.eventPane.OnTierXLocked();
        }
        CleanDownManager.Instance.OnTierXThemeChanged();
    }

    private void ShowWorldTourHighStakesScreen(int challengeEventID, string challengePinID)
    {
        RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(challengeEventID);
        if (eventByEventIndex != null)
        {
            List<PinDetail> allPins = TierXManager.Instance.GetAllPins(false);
            PinDetail pinDetail = (from x in allPins
                                   where x.EventID == challengeEventID && x.PinID == challengePinID
                                   select x).FirstOrDefault<PinDetail>();
            if (pinDetail == null)
            {
            }
            eventByEventIndex.SetWorldTourPinPinDetail(pinDetail);
            HighStakesScreenBase.IsSetupForWorldTourHighStakes = true;
            HighStakesScreenBase.WorldTourHighStakesRaceEvent = eventByEventIndex;
            ScreenManager.Instance.SwapScreen(ScreenID.HighStakesChallenge);
        }
    }

    private bool ShowWorldTourHighStakesIfRequired()
    {
        if (TierXManager.Instance.IsOverviewThemeActive())
        {
            return false;
        }
        List<int> validSuperNitrousEventsForTheme = TierXManager.Instance.GetValidSuperNitrousEventsForTheme();
        bool flag = false;
        IEnumerable<KeyValuePair<int, string>> enumerable = PlayerProfileManager.Instance.ActiveProfile.WorldTourBoostNitrous.ChallengeIDsToShow(BossChallengeStateEnum.BEGIN);
        foreach (KeyValuePair<int, string> current in enumerable)
        {
            if (validSuperNitrousEventsForTheme.Contains(current.Key))
            {
                this.ShowWorldTourHighStakesScreen(current.Key, current.Value);
                flag = true;
            }
        }
        if (!flag)
        {
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            WorldTourBoostNitrous worldTourBoostNitrous = PlayerProfileManager.Instance.ActiveProfile.WorldTourBoostNitrous;
            IEnumerable<KeyValuePair<int, string>> enumerable2 = worldTourBoostNitrous.ChallengeIDsToShow(BossChallengeStateEnum.INPROGRESS);
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            foreach (KeyValuePair<int, string> current2 in enumerable2)
            {
                if (validSuperNitrousEventsForTheme.Contains(current2.Key))
                {
                    if (UserManager.Instance.currentAccount.SuperNitrous <= activeProfile.BoostNitrousUsed)
                    {
                        activeProfile.BoostNitrousUsed = UserManager.Instance.currentAccount.SuperNitrous;
                        list.Add(current2);
                    }
                    else if (activeProfile.EventsCompleted.Contains(current2.Key))
                    {
                        this.ShowWorldTourHighStakesScreen(current2.Key, current2.Value);
                        flag = true;
                    }
                }
            }
            foreach (KeyValuePair<int, string> current3 in list)
            {
                worldTourBoostNitrous.SetRaceFinished(current3.Key, current3.Value);
            }
        }
        return flag;
    }

    private bool ShowDeferredNarrativeSceneIfRequired()
    {
        Dictionary<string, PlayerProfileData.DeferredNarrativeScene> worldTourDeferredNarrativeScenes = PlayerProfileManager.Instance.ActiveProfile.WorldTourDeferredNarrativeScenes;
        PlayerProfileData.DeferredNarrativeScene deferredNarrativeScene;
        NarrativeScene scene;
        if (worldTourDeferredNarrativeScenes.TryGetValue(TierXManager.Instance.CurrentThemeName, out deferredNarrativeScene) && TierXManager.Instance.GetNarrativeScene(deferredNarrativeScene.SceneID, out scene))
        {
            if (scene.CharactersDetails.CharacterGroups.Count > 0 &&
                !string.IsNullOrEmpty(scene.CharactersDetails.CharacterGroups[0].LogoTextureName))
            {
                CrewProgressionScreen.BackgroundImageText = scene.CharactersDetails.CharacterGroups[0].LogoTextureName;
            }
            ScreenManager.Instance.PushScreen(ScreenID.CrewProgression);
            ScreenManager.Instance.UpdateImmediately();
            CrewProgressionScreen crewProgressionScreen = ScreenManager.Instance.ActiveScreen as CrewProgressionScreen;
            crewProgressionScreen.SetupForNarrativeScene(scene);
            return true;
        }
        return false;
    }

    private void TierXPanelSetupCompete(int panel)
    {
        ThemeLayout themeDescriptor = TierXManager.Instance.ThemeDescriptor;
        if (themeDescriptor.ShowTitle)
        {
            this.SetupTierText(themeDescriptor.Name, true);
        }
        else
        {
            this.SetupTierText(string.Empty, true);
        }
        //if (themeDescriptor.ShowEventPane)
        //{
        //    this.eventPane.ActivateAll();
        //}
        //else
        //{
        //    this.eventPane.DeactivateAll();
        //}
        //this.TierText.gameObject.SetActive(themeDescriptor.ShowTierText);
        //this.TopOffset.gameObject.SetActive(themeDescriptor.ShowSelectedThemeDescription);
        //this.ObjectiveText.gameObject.SetActive(themeDescriptor.ShowObjective);
        if (themeDescriptor.ShowObjective)
        {
            this.SetupObjectiveText();
        }
        if (TierXManager.Instance.IsOverviewThemeActive())
        {
            UnityEngine.Vector3 b = new UnityEngine.Vector3(0f, 0f, 0f);
            CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            EventPane eventPane = careerModeMapScreen.eventPane;
            b.x = eventPane.PaneWidthTight / 2f;
            //this.TopOffset.transform.parent = this.EventSelect.transform;
            //this.TopOffset.transform.localPosition = new UnityEngine.Vector3(0f, 2.2f, 0f) - CareerModeMapScreen.GetPanelPositionOffset(CareerModeMapScreen.mapPaneSelected) - b;
        }
        if (themeDescriptor.CanSwipe)
        {
            //this.Pagination.AnimateIn();
        }
        else
        {
            //this.Pagination.SetInvisible();
        }
        if (themeDescriptor.GetThemeOptionLayoutDetails() != null)
        {
            if (MapScreenCache.InternationalEventHubGO == null)
            {
                MapScreenCache.InternationalEventHubGO = (UnityEngine.Object.Instantiate(Resources.Load("Career/MapInternationalEventHub")) as GameObject);
            }
            MapScreenCache.InternationalEventHubGO.GetComponent<EventHubBackgroundManager>().Show(themeDescriptor.GetThemeOptionLayoutDetails());
        }
        else
        {
            MapScreenCache.DestroyInternationalBackground();
        }
        if (this.ShowWorldTourHighStakesIfRequired())
        {
            return;
        }
        if (this.ShowDeferredNarrativeSceneIfRequired())
        {
            return;
        }
        TierXManager.Instance.RefreshThemeMap();
        if (RelayManager.GoToNextRelayRaceIfRequired())
        {
            return;
        }
        TierXManager.Instance.SetupBackground();
    }

    public void FocusOnEvent(ProgressionMapPinEventType eventType, int tier = -1, float delay = 0,
        bool setHighlight = true, string pinName = null)
    {
        IMapPin mapPin;
        if (eventType == ProgressionMapPinEventType.WORLD_TOURS)
        {
            mapPin = EventSelect.GetMapPin(eventType,eCarTier.TIER_1, pinName);
        }
        else
        {
            if (tier == -1)
                mapPin = EventSelect.GetMapPin(eventType);
            else
            {
                var eTier = (eCarTier)(tier - 1);
                mapPin = EventSelect.GetMapPin(eventType, eTier);
            }
        }


        //foreach (var eventpaneData in EventSelect.GetEventPins())
        //{
        //    eventpaneData.EventPane.SetHighlight(false);
        //}
        if (mapPin != null)
        {
            MapCamera.MoveToPosition(mapPin.position, delay);
            mapPin.SetHightlight(setHighlight);
            if (eventType == ProgressionMapPinEventType.CREW_RACES && tier > 0 && (eCarTier)(tier - 1) == eCarTier.TIER_1)
            {
	            if(GameDatabase.Instance.TutorialConfiguration.IsOn)
					IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial2);
            }
        }
        else
        {
	        GTDebug.Log(GTLogChannel.Map,"EventPin not found for pin : " + eventType + "  and tier : " + tier);
        }
        //eventPaneData.EventPane.SetEventActive(true, true);
    }


    public void FocusOnTier(int tier, float delay = 0)
    {
        var position = EventSelect.GetTierPosition(tier);
        MapCamera.MoveToPositionImmediately(position);
        //eventPaneData.EventPane.SetEventActive(true, true);
    }

    public void DisableEverythingExceptEvent(ProgressionMapPinEventType eventType, int tier = -1, float delay = 0)
    {
        IMapPin mapPin;
        if (tier == -1)
            mapPin = EventSelect.GetMapPin(eventType);
        else
        {
            var eTier = (eCarTier)(tier - 1);
            mapPin = EventSelect.GetMapPin(eventType, eTier);
        }

        //foreach (var eventpaneData in EventSelect.GetEventPins())
        //{
        //    eventpaneData.EventPane.SetHighlight(false);
        //}
        if (mapPin != null)
        {
            MapCamera.Instance.Interactable = false;
            EventSelect.SetAllMApPinsInteractable(false);
            mapPin.interactable = true;
        }
        else
        {
	        GTDebug.Log(GTLogChannel.Map,"EventPin not found for pin : " + eventType + "  and tier : " + tier);
        }
        //eventPaneData.EventPane.SetEventActive(true, true);
    }
}
