using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using Objectives;
using PurchasableItems;
using UnityEngine;

public class ShowroomScreen : ShopScreenBase
{
	public class InitialisationState
	{
		public ShowroomMode screenMode = ShowroomMode.Cars_Per_Tier;

		public eCarTier CurrentTierManufacturer;

        public string CurrentManufacturer;

		public CarInfo HighlightedCar;

		public int LastSelectedModelIndex;

        public string LastSelectedManufacturer;

		public AgentCarDeal DealToApply;

		public IEnumerable<CarInfo> PresetCarList;
	}
    protected readonly float _timeToWaitBeforeChangingSelection = 0.4f;

	private const int HorizontalListBufferCount = 1;

	private const int tutorialDeliveryCost = 0;

	public static InitialisationState Init = new InitialisationState();

	private static bool _fromZoomed;

	//private static int _lastColourIndex;

    private static InitialisationState InitPrev;

	private List<CarInfo> _carsInList;

	private bool _shouldRecheckTimer;

	private float _idleTimer;

	private bool _modelSelectionJustChanged;

	private bool _colourSelectionJustChanged;

    private bool _sameCar;

	private string delegateCarArrival;

	private string delegateCarDrive;

    private string formalCarName;

	private static CostType CurrentBuyMode = CostType.CASHANDGOLD;

    private static string inAppPurchaseIdentifier;

	private static string inAppPurchasePriceString;

	private BubbleMessage BubbleMessage;

	private static string _carIDTracker;

    private static GameObject m_showRoomInstance;

    [SerializeField] private ShowRoomScrollerController m_scroller;

    //private ScreenFadeQude m_screenfadeQuade;

    private bool m_firstDeactivate = true;

    public List<BaseList> CarouselLists;


    private HorizontalList ModelList
    {
        get
        {
            return CarouselLists[0] as HorizontalList;
        }
    }

    private HorizontalList ColourList
    {
        get
        {
            return CarouselLists[1] as HorizontalList;
        }
    }

    public static string LastBoughtCar
	{
		get;
		private set;
	}

	public static string MostRecentCarID
	{
		get
		{
			return _carIDTracker;
		}
	}

    public override ScreenID ID
    {
        get { return ScreenID.Showroom; }
    }

    public CarInfo CurrentCarInfo
    {
        get
        {
            if (_carsInList != null)
                return _carsInList.FirstOrDefault(c => c.Key == m_scroller.SelectedID);
            return null;
        }
    }

	public override void OnActivate(bool zAlreadyOnStack)
	{
        CleanDownManager.Instance.OnEnterShowroom();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //if (ScreenManager.Instance.IsScreenOnStack(ScreenID.Shop))
        //{
        //    CommonUI.Instance.NavBar.ShopButton.gameObject.SetActive(false);
        //    CommonUI.Instance.NavBar.ShopButtonIsEnabled = false;
        //}
		/*else*/if (!activeProfile.HasBoughtFirstCar)
		{
            Init.CurrentManufacturer = "None";
			Init.screenMode = ShowroomMode.Tutorial_BuyCar;
		}
        //else
        //{
        //    Init.screenMode = ShowroomMode.Cars_Per_Tier;
        //}
        //base.CreateCrosshair();

        CarInfoUI.Instance.RepositionFor(ScreenID.Showroom);
        SceneManagerShowroom.Instance.CarSwitched += OnCarSwitched;
        //base.HorizontalList.NumBufferItems = 1;
        base.OnActivate(zAlreadyOnStack);
        PopulateItemLists();//This line actually replaced by base.OnActivate()
		UpdateUI(_carIDTracker);
        if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() >= 2 && activeProfile.DailyBattlesLastEventAt > DateTime.MinValue && activeProfile.HasBoughtFirstUpgrade && !activeProfile.HasVisitedManufacturerScreen)
        {
            Log.AnEvent(Events.TapCarDealer);
        }
		if (Init.screenMode == ShowroomMode.SpecialOffer)
		{
			Log.AnEvent(Events.DealShown);
		}
        GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null);
		if (activeProfile.HasBoughtFirstUpgrade && !activeProfile.HasVisitedManufacturerScreen)
		{
            //Vector3 vector = CommonUI.Instance.NavBar.ShopButton.transform.position;
            //vector += new Vector3(-0.16f, -0.4f, 0f);
            //this.BubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_SHOP_CARDEALER_INTRO", false, vector, BubbleMessage.NippleDir.UP, 1f, BubbleMessageConfig.ThemeStyle.SMALL, BubbleMessageConfig.PositionType.BOX_RELATIVE, 0.16f);
		}
	}

    public override void OnDeactivate()
    {
        Init.HighlightedCar = CurrentCarInfo;

        if(!m_firstDeactivate)
            Init.screenMode = ShowroomMode.Cars_Per_Tier;
        base.OnDeactivate();
        if (InitPrev != null && ScreenManager.Instance.NominalNextScreen() != ScreenID.ShowroomFreeCam)
        {
            Init = InitPrev;
            InitPrev = null;
        }
        _fromZoomed = ShowroomCameraManager.Instance.IsZoomedIn;
        if (SceneManagerShowroom.Instance!=null)
            SceneManagerShowroom.Instance.CarSwitched -= OnCarSwitched;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!activeProfile.PlayerOwnsCurrentCar())
        {
            return;
        }
        if (Init.screenMode != ShowroomMode.Tutorial_BuyCar &&
            Init.screenMode != ShowroomMode.Tutorial_DeliverCar)
        {
            activeProfile.UpdateCurrentPhysicsSetup();
        }
        if (PopUpManager.Instance.isShowingPopUp)
        {
            PopUpManager.Instance.KillPopUp();
        }
        if (BubbleMessage != null)
        {
            BubbleMessage.Dismiss();
            BubbleMessage = null;
        }
        m_firstDeactivate = false;
    }

    protected override void OnDestroy()
	{
		_carsInList = null;
        //base.OnDestroy();

        if (m_showRoomInstance != null)
        {
            Destroy(m_showRoomInstance);
            ResourceManager.UnloadUnusedAssets();
        }
        base.OnDestroy();
	}

	void SelectedItemChanged()
	{
        //ListItemProxy listItemProxy = zItem as ListItemProxy;
        //if (listItemProxy != null)
        //{
        //    BasicMenuListItem basicMenuListItem = listItemProxy.Instance as BasicMenuListItem;
        //    if (basicMenuListItem != null)
        //    {
        //        this.OnModelChanged(basicMenuListItem);
        //    }
        //}
        //ColourSwatchListItem colourSwatchListItem = zItem as ColourSwatchListItem;
        //if (colourSwatchListItem != null)
        //{
        //    this.OnColourChanged(colourSwatchListItem);
        //}
	}

	protected override void Update()
	{
        base.Update();
        UpdateArrivalTicker();
        UpdateIdleMenuTimer();
	}

	public void LateUpdate()
	{
        CarInfo currentCarInfo = SceneManagerShowroom.Instance.currentCarInfo;
        bool flag = currentCarInfo != null /*&& currentCarInfo.Key == this._carsInList[this.ModelList.SelectedIndex].Key*/ && !_colourSelectionJustChanged && !_modelSelectionJustChanged;
        if (currentCarInfo != null)
        {
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            flag &= (!PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(currentCarInfo.Key) || !activeProfile.PlayerCarInvalidRTW(currentCarInfo.Key));
        }
        m_costContainer.SetButtonsEnabled(flag);
	}

	void PopulateItemLists()
	{
        //base.PopulateItemLists();
		AddCars();
        //this.ColourList.CurrentState = BaseRuntimeControl.State.Hidden;
		if (Init.LastSelectedManufacturer == Init.CurrentManufacturer)
		{
			if (Init.LastSelectedModelIndex <= _carsInList.Count)
			{
                //this.ModelList.SelectedIndex = ShowroomScreen.Init.LastSelectedModelIndex;
			}
			//this._sameCar = true;
		}
	    if (Init.LastSelectedModelIndex>0)
	    {
	        m_scroller.SelectedIndex = Init.LastSelectedModelIndex;
	    }
	    else
	    {
            m_scroller.SetSelectedToFirstCar();
	    }
		UpdateCurrentCar();
		UpdateUI(_carIDTracker);
        //base.MarkAllItemsAddedToCarousels();
	}

    public override void RequestBackup()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_POPUPS_BUYFIRSTCAR_FORCE_TITLE";
			popUp.BodyText = "TEXT_POPUPS_BUYFIRSTCAR_FORCE_BODY";
			popUp.IsBig = true;
			popUp.ConfirmAction = delegate
			{
				Log.AnEvent(Events.FootRace);
			};
			popUp.ConfirmText = "TEXT_BUTTON_OK";
            popUp.GraphicPath = PopUpManager.Instance.graphics_agentPrefab;
			popUp.ImageCaption = "TEXT_NAME_AGENT";
			PopUp popup = popUp;
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
			return;
		}

        if (!SceneManagerShowroom.Instance.IsLoadingCar())
        {
            if (Init.screenMode == ShowroomMode.SpecialOffer &&
                !PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(Init.HighlightedCar.Key) &&
                !ArrivalManager.Instance.isCarOnOrder(Init.HighlightedCar.Key))
            {
                Log.AnEvent(Events.DealBackOut);
                PopUpManager.Instance.TryShowPopUp(PopUp_ConfirmAgentDealBackup(), PopUpManager.ePriority.Default,
                    null);
            }
            else
            {
                Init.HighlightedCar = null;
                Init.LastSelectedModelIndex = 0;
                Init.PresetCarList = null;
                Init.screenMode = ShowroomMode.Cars_Per_Tier;
                //if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.Manufacturer))
                //{
                //    CarInfoUI.Instance.SetCurrentCarIDKey(
                //        PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
                //}
                base.RequestBackup();
            }
        }
	}

    private void OnModelChanged(BasicMenuListItem item)
    {
        _modelSelectionJustChanged = true;
        _idleTimer = _timeToWaitBeforeChangingSelection;
    }

    private void OnColourChanged(ColourSwatchListItem item)
    {
        _colourSelectionJustChanged = true;
        _idleTimer = _timeToWaitBeforeChangingSelection;
    }

    private void RequestModel(BasicMenuListItem zItem)
    {
        if (zItem.Ready)
        {
            UpdateCurrentCar();
        }
    }

    private void RequestColour(ColourSwatchListItem zItem)
    {
        CarVisuals currentCarVisuals = SceneManagerShowroom.Instance.currentCarVisuals;
        int colorIndex = zItem.ColorIndex;
        currentCarVisuals.SetCurrentColor(colorIndex);
        //ShowroomScreen._lastColourIndex = colorIndex;
        GameObject liveryObj = AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.AICarLivery);
        currentCarVisuals.ApplyLivery(liveryObj, true, delegate {
            SceneManagerShowroom.Instance.ReadyToTransitionToFront();
        });
    }

	private void UpdateCarOnOrderCostContainer(string carid)
	{
		string str = LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_ARRIVES_IN:");
        string format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES_AND_SECONDS");
		int num = 0;
		int num2 = 0;
		ArrivalManager.Instance.GetTimeUntilDelivery(carid, out num, out num2);
		string str2 = string.Format(format, num, num2);
	    string text = str2;//string.Concat(str2, " ", str);
        if (text != m_costContainer.Title)
		{
            m_costContainer.SetupForBlueButton(text, LocalizationManager.GetTranslation("TEXT_BUTTON_DELIVER"), BaseRuntimeControl.State.Active, OnBuyButton);
		}
	}

	private void UpdateArrivalTicker()
	{
		if (_shouldRecheckTimer)
		{
		    string key = m_scroller.SelectedID;
			string timeStringTilCarDelivery = ArrivalManager.Instance.getTimeStringTilCarDelivery(key);
			if (timeStringTilCarDelivery.Length < 1)
			{
                m_costContainer.SetupForTitleOnly(LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_OWNED"));
				UpdateCurrentCar();
			}
			else
			{
				UpdateCarOnOrderCostContainer(key);
			}
		}
	}

	private void UpdateIdleMenuTimer()
	{
        if (_idleTimer > 0f)
        {
            if (ColourList.IsBeingDragged || ModelList.IsBeingDragged)
            {
                _idleTimer = _timeToWaitBeforeChangingSelection;
            }
            else
            {
                _idleTimer -= Time.deltaTime;
            }
            if (_idleTimer <= 0f && !SceneManagerShowroom.Instance.IsLoadingCar())
            {
                if (_modelSelectionJustChanged)
                {
                    ListItemProxy listItemProxy = ModelList.SelectedItem as ListItemProxy;
                    RequestModel(listItemProxy.Instance as BasicMenuListItem);
                    _modelSelectionJustChanged = false;
                }
                if (_colourSelectionJustChanged)
                {
                    RequestColour(ColourList.SelectedItem as ColourSwatchListItem);
                    _colourSelectionJustChanged = false;
                    ShowroomPimpageManager.UpdateColour();
                }
            }
        }
	}

	private void GetCostForCurrentItem(out int cash, out int gold)
	{
	    CarInfo carInfo = CurrentCarInfo;
		cash = carInfo.BuyPrice;
        gold = carInfo.GoldPrice;
		if (Init.screenMode == ShowroomMode.SpecialOffer)
		{
			cash = 0;
			gold = Init.DealToApply.GetGoldPrice();
		}
	}

	private eCarTier GetTierForCurrentItem()
	{
	    CarInfo carInfo = null;//this._carsInList[this.ModelList.SelectedIndex];
		return carInfo.BaseCarTier;
	}

	public void UpdateCurrentCar()
	{
        //if (_carIDTracker == m_scroller.SelectedID && ShowRoomManager.Instance.CarVisualInstance!=null)
        //    return;
        //if (ShowroomScreen.Init.LastSelectedModelIndex != this.ModelList.SelectedIndex)
        //{
        //    this.ColourList.CurrentState = BaseRuntimeControl.State.Hidden;
        //}
	    Init.LastSelectedModelIndex = m_scroller.SelectedIndex;// this.ModelList.SelectedIndex;
		Init.LastSelectedManufacturer = Init.CurrentManufacturer;
		if (_carsInList.Count == 0)
		{
			return;
		}
	    var key = m_scroller.SelectedID;
        CarInfoUI.Instance.SetCurrentCarIDKey(key);
        var currentCarIDKey = CarInfoUI.Instance.CurrentCarIDKey;
        var car = CarDatabase.Instance.GetCar(currentCarIDKey);
        if (SceneManagerShowroom.Instance.currentCarInfo==null || key != SceneManagerShowroom.Instance.currentCarInfo.ID)
            ScreenManager.Instance.Interactable = false;
        SceneManagerShowroom.Instance.LoadCarInShowroom(car);
        //if (ShowRoomManager.Instance.LoadedCarID != m_scroller.SelectedID)
        //{
        //    m_screenfadeQuade.FadeTo(new Color(0, 0, 0, 1), 0.45F, () =>
        //    {
        //        ShowRoomManager.Instance.SwitchCar(m_scroller.SelectedID, callback =>
        //        {
        //            m_screenfadeQuade.FadeTo(new Color(0, 0, 0, 0), .6F);
        //        });
        //    });
        //}
	    _shouldRecheckTimer = false;
		if (_fromZoomed)
		{
			OnCarSwitched();
			_fromZoomed = false;
		}
		UpdateUI(key);
		_carIDTracker = key;
	}

	private void UpdateUI(string carID)
	{
		bool isOwned = PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(carID);
		bool isOnOrder = ArrivalManager.Instance.isCarOnOrder(carID);
		if (isOwned && PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
		{
			bool flag3 = PlayerProfileManager.Instance.ActiveProfile.PlayerCarInvalidRTW(carID);
            m_costContainer.SetupForBlueButton(LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_OWNED"), LocalizationManager.GetTranslation("TEXT_BUTTON_DRIVE"), (!flag3) ? BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled, OnDriveButton);
			delegateCarDrive = carID;
		}
		else if (isOnOrder)
		{
			UpdateCarOnOrderCostContainer(carID);
			_shouldRecheckTimer = true;
		}
		else if (Init.screenMode == ShowroomMode.SpecialOffer)
		{
            Init.DealToApply.SetupCostContainer(m_costContainer, OnBuyButtonGoldChoice);
		}
		else if (Init.screenMode == ShowroomMode.OfferPack_Cars)
		{
            OfferPackData offerPackDataForProduct = GameDatabase.Instance.OfferPacks.GetOfferPackDataForProduct(inAppPurchaseIdentifier);
            string carCountText = string.Format(LocalizationManager.GetTranslation("TEXT_OFFERPACK_CARS_REMAINING"), offerPackDataForProduct.GetCarsRemainingInPack());
		    m_costContainer.SetupForOfferPackPurchase(
		        LocalizationManager.GetTranslation(offerPackDataForProduct.ShowroomTitleString), carCountText,
		        LocalizationManager.GetTranslation(offerPackDataForProduct.ShowroomBodyString), inAppPurchasePriceString,
                AppStore.Instance.CheckItemAvailable(inAppPurchaseIdentifier), OnBuyButtonOfferPackChoice);
		}
		else
        {
		    CarInfo carInfo = CurrentCarInfo;
            if (CurrentCarInfo == null)
                return;
			if (!CurrentCarInfo.IsAvailableToBuyInShowroom())
			{
                if (!string.IsNullOrEmpty(carInfo.PrizeInfoText))
                {
                    if (!string.IsNullOrEmpty(carInfo.PrizeInfoButton))
                    {
                        string[] array = carInfo.PrizeInfoButton.Split(',');
                        string title = LocalizationManager.GetTranslation(carInfo.PrizeInfoText);
                        string buttonText = LocalizationManager.GetTranslation(array[0]);
                        if (array[1].Contains("OnGoWorldTourHub"))
                        {
                            m_costContainer.SetupForWorldTour(title, buttonText, BaseRuntimeControl.State.Active, /*TierXManager.Instance,*/ OnGoWorldTourHub);
                        }
                        else if (array[1] == "OnGoRaceYourFriends")
                        {
                            m_costContainer.SetupForRaceYourFriends(title, buttonText, BaseRuntimeControl.State.Active, OnGoRaceYourFriends);
                        }
                        else if (array[1].Contains("OnGoDownloadClassics"))
                        {
                            m_costContainer.SetupForInstallClassics(title, buttonText, BaseRuntimeControl.State.Active, OnGoDownloadClassics);
                        }
                        else if (array[1].Contains("OnGoInternational"))
                        {
                            if (carInfo.IsUnlockedToBuy())
                            {
                                UpdateUIForBuyableCar(carInfo);
                            }
                            else if (carInfo.IsPrizeInCurrentSeason())
                            {
                                m_costContainer.SetupForMultiplayer(LocalizationManager.GetTranslation("TEXT_RTW_SHOWROOM"), LocalizationManager.GetTranslation("TEXT_BUTTON_RACE"), BaseRuntimeControl.State.Active, OnGoRaceTheWorld);
                            }
                            else if (carInfo.IsPrizeInFutureSeason())
                            {
                                m_costContainer.SetupForMultiplayer(LocalizationManager.GetTranslation("TEXT_COMING_SOON_TO_RTW"), null, BaseRuntimeControl.State.Active,null);
                            }
                            else
                            {
                                m_costContainer.SetupForInternational(LocalizationManager.GetTranslation(carInfo.PrizeInfoText), LocalizationManager.GetTranslation(array[0]), BaseRuntimeControl.State.Active, OnGoInternational);
                            }
                        }
                    }
                    else
                    {
                        m_costContainer.SetupForTitleOnly(LocalizationManager.GetTranslation(carInfo.PrizeInfoText));
                    }
                }
                else if (carInfo.IsComingSoonToShowroom())
                {
                    m_costContainer.SetupForMultiplayer(LocalizationManager.GetTranslation("TEXT_RTW_PRIZE_COMINGSOON"), null, BaseRuntimeControl.State.Active, null );
                }
                else if (carInfo.IsPrizeInCurrentSeason())
                {
                    m_costContainer.SetupForMultiplayer(LocalizationManager.GetTranslation("TEXT_RTW_SHOWROOM"), LocalizationManager.GetTranslation("TEXT_BUTTON_RACE"), BaseRuntimeControl.State.Active, OnGoRaceTheWorld);
                }
                else if (carInfo.IsPrizeInFutureSeason())
                {
                    m_costContainer.SetupForMultiplayer(LocalizationManager.GetTranslation("TEXT_COMING_SOON_TO_RTW"), null, BaseRuntimeControl.State.Active, null );
                }
			}
            else if (!CurrentCarInfo.IsUnlockedToBuy())
            {
                var unlockText = "TEXT_LOCK_CAR_EVENT_" + CurrentCarInfo.UnlockEventIds[0];
                m_costContainer.SetupForLock(LocalizationManager.GetTranslation(unlockText));
            }
			else
			{
				UpdateUIForBuyableCar(carInfo);
			}
		}
	}

	private void UpdateUIForBuyableCar(CarInfo car)
	{
		int cash;
		int gold;
		GetCostForCurrentItem(out cash, out gold);
	    PurchasableItem purchasableItemForCar = GameDatabase.Instance.IAPs.GetPurchasableItemForCar(CurrentCarInfo.Key);
		if (purchasableItemForCar != null)
		{
            inAppPurchaseIdentifier = purchasableItemForCar.IAPCode;
            inAppPurchasePriceString = ShopScreen.RequestPriceStringForIAP(purchasableItemForCar.IAPCode);
			bool flag = !string.IsNullOrEmpty(inAppPurchasePriceString);
			if (!flag)
			{
				inAppPurchasePriceString = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_OFFLINE").ToUpper();
			}
		    m_costContainer.SetupForDirectIAP(
		        LocalizationManager.GetTranslation("TEXT_SHOWROOM_PAYMENT_" + car.Key + "_TITLE"),
		        LocalizationManager.GetTranslation("TEXT_SHOWROOM_PAYMENT_" + car.Key + "_BODY"),
		        inAppPurchasePriceString, flag, this, OnBuyButtonIAPChoice);
		}
		else if (gold > 0 && cash > 0)
		{
            m_costContainer.SetupForCost(cash, gold, LocalizationManager.GetTranslation("TEXT_BUTTON_BUY"), OnBuyButton, OnBuyButtonGoldChoice,null);
		}
		else if (gold > 0)
		{
            m_costContainer.SetupForCost(cash, gold, LocalizationManager.GetTranslation("TEXT_BUTTON_BUY"), OnBuyButtonGoldChoice, OnBuyButtonGoldChoice, OnBuyButtonGoldChoice);
		}
		else if (cash > 0)
		{
            m_costContainer.SetupForCost(cash, gold, LocalizationManager.GetTranslation("TEXT_BUTTON_BUY"), OnBuyButton, OnBuyButton, OnBuyButton);
		}
	}

	private void OnCarSwitched()
	{
	    ScreenManager.Instance.Interactable = true;
        //this.ColourList.CurrentState = BaseRuntimeControl.State.Active;
		AddCarColourSwatches();
		UpdateUI(_carIDTracker);
        CleanDownManager.Instance.OnShowroomSwitchCar();
	}

	private void AddCarColourSwatches()
	{
        //this.ColourList.Clear();
        //CarVisuals currentCarVisuals = SceneManagerShowroom.Instance.currentCarVisuals;
        //if (currentCarVisuals == null)
        //{
        //    return;
        //}
        //int num = 0;
        //foreach (Color current in currentCarVisuals.BaseColors)
        //{
        //    Color zColour = current;
        //    zColour.a = 1f;
        //    CSRCarouselItemBuilder.AddColourSwatchItemToList(this.CarouselLists[1], zColour, num++);
        //}
        //CarGarageInstance carFromID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(ShowroomScreen._carIDTracker);
        //if (carFromID != null && PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
        //{
        //    int appliedColourIndex = carFromID.AppliedColourIndex;
        //    this.ColourList.SelectedIndex = appliedColourIndex;
        //    this.ColourList.CurrentState = BaseRuntimeControl.State.Disabled;
        //}
        //else
        //{
        //    this.WorkOutWhichColorShouldBeTheStartingColour();
        //}
        //base.MarkAllItemsAddedToCarousels();
	}

	public void LoadedCarAvatarAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		if (zAssetBundle.Contains(zAssetID + "_Colours"))
		{
            //TextAsset coloursAsset = zAssetBundle.Load(zAssetID + "_Colours") as TextAsset;
            //this.PrintCarShowroomColours(coloursAsset);
		}
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
	}

	private void PrintCarShowroomColours(TextAsset coloursAsset)
	{
		JsonDict jsonDict = new JsonDict();
		jsonDict.Read(coloursAsset.text);
		List<float> list = new List<float>();
		if (jsonDict.TryGetValue("ShowroomColours", out list))
		{
			List<Color> list2 = new List<Color>();
			for (int i = 0; i < list.Count; i += 3)
			{
				Color item = new Color(list[i], list[i + 1], list[i + 2]);
				list2.Add(item);
			}
		}
	}

	private void WorkOutWhichColorShouldBeTheStartingColour()
	{
        //if (ShowroomScreen._fromZoomed)
        //{
        //    this.ColourList.SelectedIndex = ShowroomScreen._lastColourIndex;
        //}
        //else if (this._sameCar)
        //{
        //    this.ColourList.SelectedIndex = ShowroomScreen._lastColourIndex;
        //    this._sameCar = false;
        //}
        //else
        //{
        //    this.ColourList.SelectedIndex = 0;
        //}
	}

	private static int ShowroomComparePrice(CarInfo x, CarInfo y)
	{
		if (x.IsAvailableToBuyInShowroom() != y.IsAvailableToBuyInShowroom())
		{
			return y.IsAvailableToBuyInShowroom().CompareTo(x.IsAvailableToBuyInShowroom());
		}
		int num = x.ComparableCashPrice();
		int num2 = y.ComparableCashPrice();
		if (num == num2)
		{
			return x.GoldPrice.CompareTo(y.GoldPrice);
		}
		return num.CompareTo(num2);
	}

	private static int ShowroomSortCars(CarInfo x, CarInfo y)
	{
		if (x.BasePerformanceIndex == y.BasePerformanceIndex)
		{
			return ShowroomComparePrice(x, y);
		}
		return x.BasePerformanceIndex.CompareTo(y.BasePerformanceIndex);
	}

	public static bool VisibleInShowroom(CarInfo car)
	{
		return car.IsAvailableToBuyInShowroom() || car.IsComingSoonToShowroom() || car.IsPrizeInCurrentSeason() || car.IsPrizeInFutureSeason() || !string.IsNullOrEmpty(car.PrizeInfoText);
	}

	private void AddCars()
	{
        //Debug.Log(Init.screenMode);
		switch (Init.screenMode)
		{
		case ShowroomMode.SpecialOffer:
		case ShowroomMode.Friends_BuyCar:
			_carsInList = new List<CarInfo>();
			_carsInList.Add(Init.HighlightedCar);
			break;
		case ShowroomMode.Tutorial_BuyCar:
		case ShowroomMode.Tutorial_DeliverCar:
			_carsInList = CarDatabase.Instance.GetCarsOfTier(eCarTier.TIER_1);
            _carsInList.RemoveAll((CarInfo car) => !car.IsAvailableToBuyInShowroom());
			break;
		case ShowroomMode.Cars_Per_Tier:
            _carsInList = CarDatabase.Instance.GetCarsOfTier(Init.CurrentTierManufacturer);
            _carsInList.RemoveAll((CarInfo car) => !car.IsAvailableToBuyInShowroom());
			break;
		case ShowroomMode.Cars_Per_Tier_Buyable_Only:
			_carsInList = CarDatabase.Instance.GetCarsOfTier(Init.CurrentTierManufacturer);
			_carsInList.RemoveAll((CarInfo car) => !car.IsAvailableToBuyInShowroom());
			break;
		case ShowroomMode.Preset_List:
			_carsInList = new List<CarInfo>(Init.PresetCarList);
			break;
		case ShowroomMode.WorldTour_Cars:
			_carsInList = CarDatabase.Instance.GetWorldTourWinnableCars();
			_carsInList.RemoveAll((CarInfo car) => car.IsBossCarOverride);
			break;
		case ShowroomMode.International_Cars:
			_carsInList = CarDatabase.Instance.GetInternationalCars();
			break;
		case ShowroomMode.OfferPack_Cars:
			_carsInList = new List<CarInfo>(Init.PresetCarList);
			break;
        case ShowroomMode.AllCars:
            _carsInList = CarDatabase.Instance.GetAllCars();
            break;
		default:
			_carsInList = CarDatabase.Instance.GetAllCarsFromManufacturer(Init.CurrentManufacturer);
			break;
		}
		
		List<CarInfo> list = new List<CarInfo>();
		Dictionary<string, AssetDatabaseAsset> assetsOfTypeDict = AssetDatabaseClient.Instance.Data.GetAssetsOfTypeDict(GTAssetTypes.vehicle);
		Dictionary<string, AssetDatabaseAsset> assetsOfTypeDict2 = AssetDatabaseClient.Instance.Data.GetAssetsOfTypeDict(GTAssetTypes.car_icon);
		foreach (CarInfo carsIn in _carsInList)
		{
			if (!VisibleInShowroom(carsIn))
			{
				list.Add(carsIn);
			}
			else
			{
				string modelPrefabString = carsIn.ModelPrefabString;
				if (!assetsOfTypeDict.ContainsKey(modelPrefabString))
				{
					list.Add(carsIn);
				}
				else
				{
//					string key = carsIn.Key + "_icon";
//					if (!assetsOfTypeDict2.ContainsKey(key))
//					{
//						list.Add(carsIn);
//					}
				}
			}
		}
		foreach (CarInfo item in list)
		{
			_carsInList.Remove(item);
		}

	   // m_scroller.SetCars(_carsInList.Cast<ICarSimpleSpec>().OrderBy(c=>c.PPIndex).ToArray());
       if (BasePlatform.ActivePlatform.InsideCountry && Init.CurrentTierManufacturer == eCarTier.TIER_1)
           m_scroller.SetCars(_carsInList.OrderBy(x => x.OrderInShowroom).ThenBy(x=>x.PPIndex).ToArray());
       else
       {
           m_scroller.SetCars(_carsInList.OrderBy(x => x.OrderInShowroom).ThenBy(c => c.PPIndex).ToArray());
       }

       //this._carsInList.Sort(new Comparison<CarInfo>(ShowroomScreen.ShowroomSortCars));
       //if (this.ModelList.SelectedIndex > this._carsInList.Count)
       //{
       //    this.ModelList.SelectedIndex = 0;
       //}
       //this.CarouselLists[0].Clear();
       //for (int i = 0; i < this._carsInList.Count; i++)
       //{
       //    CSRCarouselItemBuilder.AddProxyListItem(this.CarouselLists[0], i, this._carsInList[i].ShortName, this._carsInList[i].name + "_icon", i == 0, i == this._carsInList.Count - 1, false);
       //}
    }

	private void goFitCar()
	{
		if (delegateCarArrival != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = delegateCarArrival;
			CarInfoUI.Instance.SetCurrentCarIDKey(delegateCarArrival);
			PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentCarSetup();
			delegateCarArrival = null;
			OnDriveButton();
		}
        //AchievementChecks.ReportSpeedupDeliveryAchievement();
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
	}

	private void noFitCar()
	{
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
	}

	private void CancelLeavingSpecial()
	{
		Log.AnEvent(Events.DealBackOutCancel);
	}

	private void FinishLeavingSpecial()
	{
		Log.AnEvent(Events.DealBackOutConfirm);
		Init.HighlightedCar = null;
		Init.DealToApply = null;
		Init.screenMode = ShowroomMode.Default;
		RequestBackup();
	}

	private void OnDriveButton()
	{
		if (delegateCarDrive != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = delegateCarDrive;
			CarInfoUI.Instance.SetCurrentCarIDKey(delegateCarDrive);
			PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentCarSetup();
            OnDeactivate();
		    ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
			delegateCarDrive = null;
            Init.LastSelectedModelIndex = 0;
            _carIDTracker = null;
		}
	}

	private void OnGoRaceTheWorld()
	{
        //if (!MultiplayerUtils.IsMultiplayerUnlocked())
        //{
        //    PopUpDatabase.Common.ShowMultiplayerLockedPopup(null);
        //}
        //else
        //{
        //    MultiplayerUtils.GoToMultiplayerHubScreen();
        //}
	}

	private void OnGoRaceYourFriends()
	{
        //TierXManager.Instance.GoCareerModeMapScreen(0);
        //CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
        //if (careerModeMapScreen != null)
        //{
        //    EventPin eventPinMatchingCondition = careerModeMapScreen.EventSelect.GetEventPinMatchingCondition((EventPin e) => e.EventData.IsFriendRaceEvent());
        //    if (eventPinMatchingCondition != null)
        //    {
        //        careerModeMapScreen.OnEventSelected(eventPinMatchingCondition.EventData, false);
        //        careerModeMapScreen.OnEventStart(eventPinMatchingCondition.EventData);
        //    }
        //}
	}

	private void OnGoDownloadClassics()
	{
		string platformCSRClassicsURL = GTPlatform.GetPlatformGTClassicsURL();
		if (!string.IsNullOrEmpty(platformCSRClassicsURL))
		{
            //Log.AnEvent(Events.ShowroomClassicsPromo, new Dictionary<Parameters, string>
            //{
            //    {
            //        Parameters.SelCar,
            //        this._carsInList[this.ModelList.SelectedIndex].Key
            //    }
            //});
			Application.OpenURL(platformCSRClassicsURL);
		}
	}

	private void OnGoWorldTourHub()
	{
		OnGoWorldTourHub_Any();
	}

	private void OnGoInternational()
	{
        TierXManager.Instance.OnGoWorldTourHubTheme("TierX_International");
    }

	private void OnGoWorldTourHub_Any()
	{
        TierXManager.Instance.OnGoWorldTourHub_Any();
    }

	private void OnBuyButtonGoldChoice()
	{
		OnBuyButton(CostType.GOLD);
	}

	private void OnBuyButtonIAPChoice()
	{
		OnBuyButton(CostType.IAP);
	}

	private void OnBuyButtonOfferPackChoice()
	{
		OnBuyButton(CostType.OFFERPACK);
	}

	private void OnBuyButton()
	{
		OnBuyButton(CostType.CASH);
	}

	public void OnBuyButton(CostType buyMode)
	{
	    CarInfo current = CurrentCarInfo;
		if (ArrivalManager.Instance.isCarOnOrder(current.Key) && buyMode != CostType.OFFERPACK)
		{
			Arrival arrivalForCar = ArrivalManager.Instance.GetArrivalForCar(current.Key);
			if (arrivalForCar != null)
			{
				PopUpManager.Instance.TryShowPopUp(getPopUp_AlreadyOnOrderCarQ(arrivalForCar, true), PopUpManager.ePriority.Default, null);
			}
			return;
		}
		if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
		{
			sendCurrentCarMetricEvent(Events.BuyFirstCar);
		}
		CurrentBuyMode = buyMode;
		AgentCarDeal agentCarDeal = (Init.screenMode != ShowroomMode.SpecialOffer) ? null : Init.DealToApply;
		eBuyCarResult eBuyCarResult = PlayerProfileManager.Instance.ActiveProfile.CanBuyCar(current, current.Key, agentCarDeal, CurrentBuyMode);
		if (eBuyCarResult == eBuyCarResult.ALREADY_OWN_THIS_CAR)
		{
			PopUpManager.Instance.TryShowPopUp(getPopUp_AlreadyOwnCar(), PopUpManager.ePriority.Default, null);
			return;
		}
        int goldCost = (agentCarDeal == null) ? current.GoldPrice : agentCarDeal.GetGoldPrice();
		if (eBuyCarResult == eBuyCarResult.NOT_ENOUGH_CASH)
		{
            if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            {
                PopUp popup = new PopUp
                {
                    IsBig = true,
                    Title = "TEXT_POPUPS_BUY_FIRST_CAR_WIDTH_CASH_TITLE",
                    BodyText = "TEXT_POPUPS_BUY_FIRST_CAR_WIDTH_CASH_BODY",
                    ConfirmText = "TEXT_OK",
                    GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
                    ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
                };
                PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
            }
            else
            {
                MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(new ItemTypeId("car", current.Key), new ItemCost
                {
                    CashCost = current.BuyPrice,
                    GoldCost = goldCost
                }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY_CAR", delegate
                {
                    CurrentBuyMode = CostType.GOLD;
                    this.OnPurchaseAffordable(current);
                }, null, null, null);
            }
            return;

            //if (PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            //{
            //    PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Cash,
            //        "BuyCar", current.Key, () =>
            //    {
            //        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy,
            //            ShopScreen.PurchaseSelectionType.Select);
            //        ScreenManager.Instance.PushScreen(ScreenID.Shop);
            //        Init.LastSelectedModelIndex = m_scroller.SelectedIndex;
            //    });
                //}
                //else
                //{
                //          PopUp popup = new PopUp
                //          {
                //              IsBig = true,
                //              Title = "TEXT_POPUPS_BUY_FIRST_CAR_WIDTH_CASH_TITLE",
                //              BodyText = "TEXT_POPUPS_BUY_FIRST_CAR_WIDTH_CASH_BODY",
                //              ConfirmText = "TEXT_OK"
                //          };
                //          PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
                //}

                //      return;
            }
		if (eBuyCarResult == eBuyCarResult.NOT_ENOUGH_GOLD)
		{
            MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("car", current.Key), new ItemCost
            {
                GoldCost = goldCost
            }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_CAR", null, null, null);
            return;

      //      PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
      //              "BuyCar", current.Key, () =>
      //      {
      //          ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
      //              ShopScreen.PurchaseSelectionType.Select);
      //          ScreenManager.Instance.PushScreen(ScreenID.Shop);
      //          Init.LastSelectedModelIndex = m_scroller.SelectedIndex;
      //      });
		    //return;
		}
		OnPurchaseAffordable(current);
	}

	private void OnPurchaseAffordable(CarInfo current)
	{
		CarGarageInstance carForSnapshot = new CarGarageInstance();
		carForSnapshot.SetupNewGarageInstance(current);
        //carForSnapshot.AppliedColourIndex = _lastColourIndex;
        CarVisuals currentCarVisuals = SceneManagerShowroom.Instance.currentCarVisuals;
        GameObject currentShowroomCar = currentCarVisuals.gameObject;
        //NumberPlateManager.Instance.RenderImmediate(carForSnapshot.NumberPlate, delegate(Texture2D newPlateTexture)
        //{
        //    CarSnapshotManager.Instance.GenerateSnapshot(carForSnapshot, currentShowroomCar, newPlateTexture, delegate(Texture2D texture)
        //    {
        int carPriceCash = current.BuyPrice;
        int goldPrice = current.GoldPrice;
        if (Init.screenMode == ShowroomMode.SpecialOffer)
        {
            carPriceCash = 0;
            goldPrice = Init.DealToApply.GetGoldPrice();
        }
        string carname = LocalizationManager.GetTranslation(current.ShortName);
        switch (CurrentBuyMode)
        {
            case CostType.CASH:
                PopUpManager.Instance.TryShowPopUp(getPopUp_ConfirmBuyCar(carPriceCash, 0,0, carname, current.BaseCarTier), PopUpManager.ePriority.Default, null);
                break;
            case CostType.GOLD:
                PopUpManager.Instance.TryShowPopUp(getPopUp_ConfirmBuyCar(0, goldPrice,0, carname, current.BaseCarTier), PopUpManager.ePriority.Default, null);
                break;
            case CostType.IAP:
                OnConfirmIAPPurchase();
                break;
            case CostType.OFFERPACK:
                OnConfirmOfferPackPurchase();
                break;
        }
        ShowroomCarVisuals showroomCarVisuals = currentShowroomCar.AddComponent<ShowroomCarVisuals>();
        CarVisuals component = currentShowroomCar.GetComponent<CarVisuals>();
        showroomCarVisuals.Setup(component, current);
        GameObject liveryObj = AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.AICarLivery);
        component.ApplyLivery(liveryObj, true, delegate {
            SceneManagerShowroom.Instance.ReadyToTransitionToFront();
        });
        //    });
        //});
	}

	public void OnConfirmIAPPurchase()
	{
		string currentCar = CurrentCarInfo.Key;
        int currentColour = ColourList.SelectedIndex;
        ShopScreen.InitialiseForDirectPurchase(inAppPurchaseIdentifier, delegate
        {
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            MenuAudio.Instance.playSound(AudioSfx.Purchase);
            CommonUI.Instance.XPStats.LevelUpLockedState(true);
            GameDatabase.Instance.XPEvents.AddPlayerXP(GameDatabase.Instance.XPEvents.GetXPPrizeForPurchase());
            Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
            {
                {
                    Parameters.ItmClss,
                    "car"
                },
                {
                    Parameters.Itm,
                    currentCar
                }
            };
            // CarGarageInstance carGarageInstance = activeProfile.CarsOwned.Find((CarGarageInstance c) => c.CarDBKey == currentCar);
            // carGarageInstance.AppliedColourIndex = currentColour;
            Log.AnEvent(Events.PurchaseItem, data);
            FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
            activeProfile.AddToTransactionHistory(currentCar, 0, 0);
            AchievementChecks.CheckForManufacturerAchievement(currentCar);
            AchievementChecks.CheckForCarsOwnedAchievements();
            activeProfile.Save();
            //CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
        }, null);
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
	}

	public void OnConfirmOfferPackPurchase()
	{
		string currentCar = CurrentCarInfo.Key;
        // int currentColour = ColourList.SelectedIndex;
        ShopScreen.ItemType lastItemType = ShopScreen.ItemTypeToShow;
        ShopScreen.PurchaseType lastPurchaseType = ShopScreen.PurchaseTypeToShow;
        ShopScreen.PurchaseSelectionType lastSelectionType = ShopScreen.SelectionTypeToUse;
        ShopScreen.InitialiseForDirectPurchase(inAppPurchaseIdentifier, delegate
        {
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            MenuAudio.Instance.playSound(AudioSfx.Purchase);
            CommonUI.Instance.XPStats.LevelUpLockedState(true);
            // CarGarageInstance carGarageInstance = activeProfile.CarsOwned.Find((CarGarageInstance c) => c.CarDBKey == currentCar);
            // if (carGarageInstance != null)
            // {
            //     carGarageInstance.AppliedColourIndex = currentColour;
            // }
            FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
            OfferPackData offerPackDataForProduct = GameDatabase.Instance.OfferPacks.GetOfferPackDataForProduct(inAppPurchaseIdentifier);
            foreach (string current in offerPackDataForProduct.CarsInPack)
            {
                activeProfile.AddToTransactionHistory(current, 0, 0);
                AchievementChecks.CheckForManufacturerAchievement(current);
            }
            AchievementChecks.CheckForCarsOwnedAchievements();
            activeProfile.Save();
            //CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
            ScreenManager.Instance.PopToScreen(ScreenID.ShopOverview);
        }, delegate
        {
            ShopScreen.InitialiseForPurchase(lastItemType, lastPurchaseType, lastSelectionType);
        });
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
	}

	public void onConfirmBuy()
	{
        MenuAudio.Instance.playSound(AudioSfx.Purchase);
	    CarInfo carInfo = CurrentCarInfo;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        int currentCash = activeProfile.GetCurrentCash();
        int currentGold = activeProfile.GetCurrentGold();
        AgentCarDeal deal = (Init.screenMode != ShowroomMode.SpecialOffer) ? null : Init.DealToApply;
        activeProfile.OrderCar(carInfo, carInfo.Key, deal, CurrentBuyMode);
        int num = GameDatabase.Instance.Currencies.getCarDeliveryTime(carInfo.BaseCarTier) * 60;
        CommonUI.Instance.XPStats.LevelUpLockedState(true);
        //AchievementChecks.CheckForManufacturerAchievement(carInfo.ManufacturerID);
	    var xpPrize = GameDatabase.Instance.XPEvents.GetXPPrizeForPurchase();
        GameDatabase.Instance.XPEvents.AddPlayerXP(xpPrize);
        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        {
            {
                Parameters.DCsh,
                (activeProfile.GetCurrentCash() - currentCash).ToString()
            },
            {
                Parameters.DGld,
                (activeProfile.GetCurrentGold() - currentGold).ToString()
            },
            {
                Parameters.ItmClss,
                "car"
            },
            {
                Parameters.Itm,
                carInfo.Key
            }
        };
        Log.AnEvent(Events.PurchaseItem, data);
        //if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
        //{
            sendCurrentCarMetricEvent(Events.BuyThisCar);
        //}
            activeProfile.AddToTransactionHistory(carInfo.Key, currentCash - activeProfile.GetCurrentCash(), currentGold - activeProfile.GetCurrentGold());
        if (!activeProfile.HasBoughtFirstCar)
        {
            UpdateUI(carInfo.Key);
            DisableModelList();
            DisableColourList();
            activeProfile.Save();
            onDeliverNowTutorial();
            return;
        }
        delegateCarArrival = carInfo.Key;
        formalCarName = LocalizationManager.GetTranslation(carInfo.LongName);
        if (CurrentBuyMode == CostType.CASH)
        {
            Arrival arrival = new Arrival();
            arrival.carId = carInfo.Key;
            arrival.ColourIndex = 0;// this.ColourList.SelectedIndex;
            arrival.deliveryTimeSecs = num;
            arrival.arrivalType = ArrivalType.Car;
            ArrivalManager.Instance.AddArrival(arrival);
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            UpdateCurrentCar();
            PopUpManager.Instance.TryShowPopUp(getPopUp_SkipCarOrderWaitQ(arrival), PopUpManager.ePriority.Default, null);
        }
        else
        {
            activeProfile.GiveCar(carInfo.Key, 0, false);// this.ColourList.SelectedIndex, false);
            FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
            UpdateCurrentCar();
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            //this.UpdateCurrentCar();
            RateTheAppNagger.TryShowPrompt(RateTheAppTrigger.BUYACAR);
            if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.WorldTourChoice))
            {
                PopUpManager.Instance.TryShowPopUp(getPopUp_GoFitCarNow(carInfo.ShortName), PopUpManager.ePriority.Default, null);
            }
            else
            {
                CommonUI.Instance.XPStats.LevelUpLockedState(false);
                ScreenManager.Instance.PopScreen();
            }
        }
        if (Init.screenMode == ShowroomMode.SpecialOffer)
        {
            Init.DealToApply.OnCompleted();
            Log.AnEvent(Events.DealCompleted);
        }
        ObjectiveCommand.Execute(new CounterBuyCar(), true);
        AchievementChecks.CheckForManufacturerAchievement(carInfo.ManufacturerID);
        AchievementChecks.CheckForCarsOwnedAchievements();
        activeProfile.Save();
        //CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
	}

	private void sendCurrentCarMetricEvent(Events e)
	{
	    CarInfo carInfo = CurrentCarInfo;
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.PCr,
				carInfo.Key
			},
			{
				Parameters.CostCash,
				carInfo.BuyPrice.ToString()
			},
			{
				Parameters.CostGold,
				carInfo.GoldPrice.ToString()
			},
            {
				Parameters.FrstCar,
                PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar.ToString()
			}
		};
		Log.AnEvent(e, data);
	}

	public void onCancelBuy()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
		{
			sendCurrentCarMetricEvent(Events.CancelThisCar);
		}
	}

	public void onDeliverNowTutorial()
	{
	    CarInfo carInfo = CurrentCarInfo;//this._carsInList[this.ModelList.SelectedIndex];
        sendCurrentCarMetricEvent(Events.DeliverThisCar);
        PlayerProfileManager.Instance.ActiveProfile.SpendGold(0,"tutorial","tutorial");
        PlayerProfileManager.Instance.ActiveProfile.GiveCar(carInfo.Key, 0, false);
        PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = carInfo.Key;
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        UpdateCurrentCar();
        MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
        //PopUpManager.Instance.KillPopUp();
        ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
        Init.LastSelectedModelIndex = 0;
        //ShowRoomManager.SetActive (false);
	}

	public bool CurrentlySelectedCarIsGoldOnly()
	{
	    CarInfo carInfo = CurrentCarInfo;
		return carInfo.BuyPrice <= 0;
	}

	public void onDeliverCar()
	{
	    CarInfo carInfo = CurrentCarInfo;
        UpdateCurrentCar();
        RateTheAppNagger.TryShowPrompt(RateTheAppTrigger.BUYACAR);
        delegateCarArrival = carInfo.Key;
        formalCarName = LocalizationManager.GetTranslation(carInfo.LongName);
        if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.WorldTourChoice))
        {
            PopUpManager.Instance.TryShowPopUp(getPopUp_GoFitCarNow(carInfo.ShortName), PopUpManager.ePriority.Default, null);
        }
        else
        {
            CommonUI.Instance.XPStats.LevelUpLockedState(false);
            Init.LastSelectedModelIndex = 0;
            ScreenManager.Instance.PopScreen();
        }
	}

	public void onSkipCarOrderWait()
	{
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
	}

	private PopUp getPopUp_AlreadyOwnCar()
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_ALREADY_OWN_CAR_TITLE",
			BodyText = "TEXT_POPUPS_ALREADY_OWN_CAR_BODY",
			ConfirmText = "TEXT_BUTTON_OK"
		};
	}

	private PopUp getPopUp_AlreadyOnOrderCarQ(Arrival arrival, bool userSelected)
	{
		return DeliverNow.GetPopup(arrival, onDeliverCar, userSelected);
	}

	private PopUp getPopUp_SkipCarOrderWaitQ(Arrival arrival)
	{
		PopUp popUp_AlreadyOnOrderCarQ = getPopUp_AlreadyOnOrderCarQ(arrival, false);
		popUp_AlreadyOnOrderCarQ.CancelAction = onSkipCarOrderWait;
		return popUp_AlreadyOnOrderCarQ;
	}

	public PopUp getPopUp_GoFitCarNow(string carstring)
	{
		string arg = LocalizationManager.GetTranslation(carstring);
		string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_CARDELIVERY_ARRIVED_BODY"), arg);
		return new PopUp
		{
			Title = "TEXT_POPUPS_CARDELIVERY_ARRIVED_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			CancelAction = noFitCar,
			ConfirmAction = goFitCar,
			SocialAction = socialButton,
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_DRIVE",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
	}

	private void socialButton()
	{
	    // SocialManager.Instance.OnShareButton(this.formalCarName);
        //SocialController.Instance.OnShareButton(SocialController.MessageType.NEW_CAR, this.formalCarName, true, false);
	}

	private PopUp PopUp_ConfirmAgentDealBackup()
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_BUY_CAR_CONFIRM_TITLE",
			BodyText = "TEXT_POPUPS_CAR_DEAL_LEAVE",
			IsBig = true,
			CancelAction = CancelLeavingSpecial,
			ConfirmAction = FinishLeavingSpecial,
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_LEAVE",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
	}

    private PopUp getPopUp_ConfirmBuyCar(int carPriceCash, int carGoldPrice,int carKeyPrice, string carname, eCarTier carTier)
    {
        string str = string.Empty;
        //if (carTier > RaceEventQuery.Instance.getHighestUnlockedClass())
        //{
        //    str = LocalizationManager.GetTranslation("TEXT_POPUPS_HIGHERTIERCARBUY_WARNING") + "\n";
        //}
        //var b = "";
        string bodyText = str +
                          string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_BUY_CAR_CONFIRMATION"), carname,
                              CurrencyUtils.GetColouredCostStringBrief(carPriceCash, carGoldPrice,carKeyPrice));
        return new PopUp
        {
            Title = "TEXT_POPUPS_BUY_CAR_CONFIRM_TITLE",
            BodyText = bodyText,
            BodyAlreadyTranslated = true,
            IsBig = false,
            CancelAction = onCancelBuy,
            ConfirmAction = onConfirmBuy,
            CancelText = "TEXT_BUTTON_CANCEL",
            ConfirmText = "TEXT_BUTTON_BUY"
        };
    }

    public void DisableModelList()
	{
        //this.ModelList.DisableAllItems(true);
        //this.ModelList.CurrentState = BaseRuntimeControl.State.Disabled;
	}

	public void DisableColourList()
	{
        //this.ColourList.DisableAllItems(true);
        //this.ColourList.CurrentState = BaseRuntimeControl.State.Disabled;
	}

	public static void ShowScreenWithPresetCarList(IEnumerable<CarInfo> carList)
	{
        SetUpScreenWithPresetList(carList);
        //if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.Workshop))
        //{
        //    ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Showroom, new ScreenID[]
        //    {
        //        ScreenID.Workshop
        //    });
        //}
        //else
        //{
        //    ScreenManager.Instance.PushScreen(ScreenID.Showroom);
        //}
        ScreenManager.Instance.PushScreen(ScreenID.Showroom);
	}

	public static void SetUpScreenWithPresetList(IEnumerable<CarInfo> carList)
	{
		Init.screenMode = ShowroomMode.Preset_List;
		Init.PresetCarList = carList;
	}

	public static void SetUpScreenForOfferPack(IEnumerable<CarInfo> carList, ProductData product)
	{
        if (ScreenManager.Instance.IsScreenOnStack(ScreenID.Showroom))
        {
            InitPrev = Init;
            Init = new InitialisationState();
        }
        Init.screenMode = ShowroomMode.OfferPack_Cars;
        Init.PresetCarList = carList;
        inAppPurchaseIdentifier = product.AppStoreProduct.Identifier;
        inAppPurchasePriceString = product.AppStoreProduct.LocalisedPrice;
	}

	public static void ShowScreenWithTierCarList(eCarTier tier, bool buyableOnly = false,bool resetScreenStack = false)
	{
		Init.CurrentTierManufacturer = tier;
		Init.LastSelectedModelIndex = 0;
        Init.CurrentManufacturer = "None";
		Init.screenMode = ((!buyableOnly) ? ShowroomMode.Cars_Per_Tier : ShowroomMode.Cars_Per_Tier_Buyable_Only);

	    if (resetScreenStack)
	    {
	        ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Showroom
	            , new[] {ScreenID.Home, ScreenID.Workshop});
	    }
	    else
	    {
            ScreenManager.Instance.PushScreen(ScreenID.Showroom);
	    }
	}

	public static void SetupShowRoomForDeal(AgentCarDeal deal)
	{
        Init.CurrentManufacturer = "None";
		Init.DealToApply = deal;
		Init.HighlightedCar = CarDatabase.Instance.GetCar(deal.Car);
		Init.screenMode = ShowroomMode.SpecialOffer;
	}

    protected override void Awake()
    {
        base.Awake();
        m_scroller.SelectedIndexChanged += m_scroller_SelectedIndexChanged;
        //m_screenfadeQuade = GetComponentInChildren<ScreenFadeQude>();
    }

    void m_scroller_SelectedIndexChanged(int obj)
    {
        ToggleItem();
    }

    private void ShowRoomLoaded(GameObject obj)
    {
        m_showRoomInstance = Instantiate(obj);
        m_showRoomInstance.name = "ShowRoomManager";
        OnCreated(false);
    }

    public override void ToggleItem()
    {
        //base.ToggleItem();
        if (m_scroller.SelectedID != null && _carIDTracker != m_scroller.SelectedID)
        {
            UpdateCurrentCar();
        }
    }

    protected override void OnBlueEvoUnlockButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnBlueDeliverButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnBlueBuyButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnBlueFitButton()
    {
        throw new NotImplementedException();
    }

    public override bool CanSelectedItemBePurchased
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    protected override bool IsSelectedItemFitted
    {
        get { throw new NotImplementedException(); }
    }

    protected override bool IsSelectedItemOwned
    {
        get { throw new NotImplementedException(); }
    }

    public override bool IsSelectedItemBeingDelivered
    {
        get { throw new NotImplementedException(); }
    }

    protected override string GetItemBeenPurchased()
    {
        throw new NotImplementedException();
    }
}
