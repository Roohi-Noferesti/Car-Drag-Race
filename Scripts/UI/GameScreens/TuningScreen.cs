using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using Objectives;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TuningScreen : ZHUDScreen
{
    public enum BlueButtonPressMode
	{
		None,
		FitPart,
		BuyPart,
		DeliverPart,
		UnlockEvoPart
	}

	private class PurchaseInfo
	{
		public CarGarageInstance CarBeingHandled;

		public CarUpgradeData UpgradeBeingHandled;
	}

	private PurchaseInfo _purchaseInProgress;

	public static eUpgradeType ExternalStartScreenOn = eUpgradeType.INVALID;

	public static bool StartScreenOnCurrentOwned;

	public static bool OfferMode;

	public static bool ExpressMode;

	private static eUpgradeType InternalStartScreenOn = eUpgradeType.INVALID;

	private string cachedMetricsUpgradeTypeString;

	private string cachedMetricsIsShortCut;

	private string cachedMetricsReceiptVerified;

	private bool newCachedMetricsEventToFlush;

	private int cachedMetricsDeltaCash;

	private int cachedMetricsDeltaGold;

	private BlueButtonPressMode _blueButtonMode;

	private bool _haveWarnedTyresThisSession;

    //public PrefabPlaceholder CostPlaceholder;

    public UpgradeScrollerController upgradeScrollerController;
    public CostContainer m_costContainer;

	public Texture2D[] UpgradeIconTextures;

	public Texture2D[] UpgradeIconTexturesSmallHit;

	public TextMeshProUGUI uiInformationText;

    public TextMeshProUGUI uiHeadingText;

    [SerializeField]
    private TextMeshProUGUI m_powerText;
    [SerializeField]
    private TextMeshProUGUI m_bodyText;
    [SerializeField]
    private TextMeshProUGUI m_gripText;
    [SerializeField]
    private TextMeshProUGUI m_transmissionText;

    [SerializeField]
    private TwoValueSlider m_powerSlider;

    [SerializeField]
    private TwoValueSlider m_bodySlider;

    [SerializeField]
    private TwoValueSlider m_tranmisionSlider;

    [SerializeField]
    private TwoValueSlider m_gripSlider;

    [SerializeField] 
    private GameObject purchaseButtonSilver;

    public DummyTextListButton uiListButton;

    public float CostWindowWidth;

	public Image uiSprImportStickerFitted;

    public Image uiSprImportStickerLookAt;

    public Image FreeUpgradeToken;

	public TextMeshProUGUI FreeUpgradesText;

    private Image uiSprOverlay;

	private bool UpdateCostContainerForArrival;

	private CarGarageInstance CurrentCar;

	private int carPPBeforeTracksidePurchase;

	private int carPPBeforeUpgrade;

	private CostType CurrentBuyMode = CostType.CASHANDGOLD;

	private BubbleMessage BuyButtonBubbleMessage;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.Tuning;
        }
    }

    private eUpgradeType CurrentUpgradeType
    {
        get
        {
            if (upgradeScrollerController != null)
                return upgradeScrollerController.CurrentUpgrade;
            return eUpgradeType.ENGINE;
        }
        set
        {
            upgradeScrollerController.CurrentUpgrade = value;
        }
    }

    private int maxUpgradeLevel = 5;

	private CarUpgradeData CurrentUpgradeData
	{
		get
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			return CarDatabase.Instance.GetCarUpgrade(activeProfile.CurrentlySelectedCarDBKey, this.CurrentUpgradeType, uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue);
		}
	}

	private CostContainer CostContainer
	{
		get
		{
		    return m_costContainer;//this.CostPlaceholder.GetBehaviourOnPrefab<CostContainer>();
		}
	}

	private bool IsCurrentPartOwned
	{
		get
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
			int upgradeLevelOwned = activeProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType);
			bool flag = indexOfCurrentValue > upgradeLevelOwned;
			return !flag;
		}
	}

	private bool IsCurrentPartAnImport
	{
		get
		{
			CarUpgradeData currentUpgradeData = this.CurrentUpgradeData;
			return currentUpgradeData != null && currentUpgradeData.IsImportPart;
		}
	}

	private bool CanCurrentPartBePurchased
	{
		get
		{
			if (this.IsCurrentCarABossCar())
			{
				return false;
			}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
			int upgradeLevelOwned = activeProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType);
			return indexOfCurrentValue - upgradeLevelOwned == 1;
		}
	}

	private bool IsCurrentPartFitted
	{
		get
		{
			int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
			return (int)this.CurrentCar.UpgradeStatus[this.CurrentUpgradeType].levelFitted == indexOfCurrentValue;
		}
	}

	private bool IsCurrentPartEvoUnlocked
	{
		get
		{
            int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
			return (int)this.CurrentCar.UpgradeStatus[this.CurrentUpgradeType].evoOwned >= indexOfCurrentValue;
		}
	}

	private bool IsCurrentPartBeingDelivered
	{
		get
		{
			return ArrivalManager.Instance.isUpgradeOnOrder(this.CurrentCar.CarDBKey, this.CurrentUpgradeData.UpgradeType);
		}
	}
	
	private int NumberOfFreeUpgrades()
	{
		if (PlayerProfileManager.Instance == null || PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return 0;
		}
		int freeUpgradesLeft = PlayerProfileManager.Instance.ActiveProfile.FreeUpgradesLeft;
		if (freeUpgradesLeft < 0)
		{
			return 0;
		}
		return freeUpgradesLeft;
	}

	private bool HaveFreeUpgrades()
	{
		return this.NumberOfFreeUpgrades() > 0 && !ExpressMode;
	}

	private int GetCashPrice()
	{
		return this.GetCashPrice(this.CurrentUpgradeData);
	}

	private int GetCashPrice(CarUpgradeData upgrade)
	{
		if (this.HaveFreeUpgrades())
		{
			return 0;
		}
		if (ExpressMode)
		{
			return 0;
		}
		return upgrade.CostInCash;
	}

	private int GetGoldPrice()
	{
		return this.GetGoldPrice(this.CurrentUpgradeData);
	}

	private int GetGoldPrice(CarUpgradeData upgrade)
	{
		if (this.HaveFreeUpgrades())
		{
			return 0;
		}
		if (ExpressMode)
		{
		    return GetExpressModeGoldCost(upgrade,
		        CarDatabase.Instance.GetCar(this.CurrentCar.CarDBKey).BaseCarTier);
		}
		return upgrade.GoldPrice;
	}

	private bool IsCurrentCarABossCar()
	{
	    return CarDataDefaults.IsBossCar(CurrentCar.CarDBKey);
	}

	private bool IsCurrentCarAnEvoCar()
	{
		return GameDatabase.Instance.CarsConfiguration.CarsWithEvoUpgrades.Contains(this.CurrentCar.CarDBKey);
	}

	public static int GetExpressModeGoldCost(CarUpgradeData upgrade, eCarTier baseTier)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<CarUpgradeData> allUpgradesOfTypeForCar = CarDatabase.Instance.GetAllUpgradesOfTypeForCar(activeProfile.CurrentlySelectedCarDBKey, upgrade.UpgradeType);
		List<int> list = new List<int>();
		for (int i = 0; i < allUpgradesOfTypeForCar.Count; i++)
		{
			list.Add(GetExpressModeUpgradeGoldCost(allUpgradesOfTypeForCar[i], baseTier));
		}
		list.Sort();
		return list[(int)upgrade.UpgradeLevel];
	}

	private static int GetExpressModeUpgradeGoldCost(CarUpgradeData upgrade, eCarTier baseTier)
	{
	    //if (upgrade.GoldPrice > 0)
        //{
        //    return Mathf.CeilToInt((float)upgrade.GoldPrice * GameDatabase.Instance.OnlineConfiguration.ExpressGoldMultiplier);
        //}
        //float expressUpgradeCashToGold = GameDatabase.Instance.OnlineConfiguration.ExpressUpgradeCashToGold;
        //float num = 1f / (float)(baseTier + 1);
        //return Mathf.CeilToInt((float)upgrade.CostInCash * expressUpgradeCashToGold * num);
	    return 0;
	}

    private void CalculateFreshCarStatsAndUpdateTheCarStatsUI(CarUpgradeSetup zUpgradeSetup, eUpgradeType zUpgradeType, int zCurrentUpgradeLevel, int zNewUpgradeLevel)
	{
        CarStatsCalculator.Instance.CalculateStatsForUpgradeScreen(zUpgradeSetup, zUpgradeType, zCurrentUpgradeLevel, zNewUpgradeLevel);
        CarStatsCalculator.Instance.SetOutStats(eCarStatsType.PLAYER_NEXT_UPGRADE_CAR);
	}

	private void OnSelectedUpgradeLevelChanged()
	{
		this.RefreshAllUI();
	}

	public void RefreshAllUI()
	{
		RaceResultsTracker.NullTrackingData();
        this.RefreshUIElement_CostContainer();
        //this.RefreshUIElement_FreeUpgradesText();
		this.RefreshUIElement_UpgradeLevelTextListButton();
        //this.RefreshUIElement_InformationText();\
		this.RefreshUIElement_BlueButtonMode();
		this.RefreshUIElement_TitleText();
        //this.RefreshUIElement_LookAtImportPartSticker();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CarUpgradeSetup currentCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
		int upgradeLevelFitted = activeProfile.GetUpgradeLevelFitted(this.CurrentUpgradeType);
        int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
        //CarUpgradeData carUpgrade = CarDatabase.Instance.GetCarUpgrade(activeProfile.CurrentlySelectedCarDBKey, this.CurrentUpgradeType, upgradeLevelFitted);
        //this.uiSprImportStickerFitted.gameObject.SetActive(carUpgrade != null && carUpgrade.IsImportPart);
        this.CalculateFreshCarStatsAndUpdateTheCarStatsUI(currentCarUpgradeSetup, this.CurrentUpgradeType, upgradeLevelFitted, indexOfCurrentValue);

        this.RefreshUIElement_UpgradeButtons(currentCarUpgradeSetup);
    }


    private void RefreshUIElement_UpgradeButtons(CarUpgradeSetup upgradeSetup)
    {
        upgradeScrollerController.SetCarUpgradeSetup(upgradeSetup);
    }

    private void RefreshUIElement_LookAtImportPartSticker()
	{
		this.uiSprImportStickerLookAt.gameObject.SetActive(this.IsCurrentPartAnImport);
	}

	private void RefreshUIElement_UpgradeLevelTextListButton()
	{
        int upgradeLevelOwned = PlayerProfileManager.Instance.ActiveProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType);
        bool zRight = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue <= upgradeLevelOwned && uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue < maxUpgradeLevel;
        bool zLeft = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue > 0;
        if (this.IsCurrentCarABossCar())
		{
            zRight = false;
            zLeft = false;
        }
        this.uiListButton.runtimeButtonBehaviour.SetButtonsEnabled(zLeft, zRight);
    }

	private void RefreshUIElement_TitleText()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int upgradeLevelFitted = activeProfile.GetUpgradeLevelFitted(this.CurrentUpgradeType);
        string str = LocalizationManager.GetTranslation("TEXT_UPGRADES_" + CarUpgrades.UpgradeNames[CurrentUpgradeType]);
        //string text = LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_HEADING_STOCK_PART").ToUpper();
        string format = LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_HEADING_STAGE_X_FITTED");
	    string text2 = string.Format(format, str, upgradeLevelFitted + 1);
	    string fullyUpgradedText = string.Format(LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_HEADING_FULLY_UPGRADED")
	        , str);
        this.uiHeadingText.text = upgradeLevelFitted == maxUpgradeLevel ? fullyUpgradedText : text2;//((upgradeLevelFitted != 0) ? text2 : text + " - " + str);
	}

	private void RefreshUIElement_InformationText()
	{
		if (this.CurrentUpgradeData != null)
		{
			string text;
			if (this.IsCurrentCarAnEvoCar() && !this.IsCurrentPartEvoUnlocked)
			{
				int upgradeLevel = (int)this.CurrentUpgradeData.UpgradeLevel;
				int num = this.CurrentCar.UpgradeStatus.Values.Count((CarUpgradeStatus x) => (int)x.evoOwned > upgradeLevel);
				int num2 = CarDatabase.Instance.GetCar(this.CurrentCar.CarDBKey).EvolutionUpgradesEarned(upgradeLevel);
				if (num2 > num)
				{
					text = LocalizationManager.GetTranslation("TEXT_UPGRADE_UNLOCK_EVO_" + (int)(this.CurrentUpgradeData.UpgradeLevel + 1));
				}
				else
				{
					text = LocalizationManager.GetTranslation("TEXT_UPGRADE_LOCKED_EVO_" + (int)(this.CurrentUpgradeData.UpgradeLevel + 1));
				}
				this.uiInformationText.text = text.ToUpper();
				return;
			}
			text = LocalizationManager.GetTranslation(this.CurrentUpgradeData.UpgradeLocalisationTextID);
            this.uiInformationText.text = text.ToUpper();
		}
		else
		{
            this.uiInformationText.text = LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_INFORMATION_STOCK_PART").ToUpper();
		}
		if (!this.IsCurrentPartOwned && !this.CanCurrentPartBePurchased)
		{
            this.uiInformationText.text = LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_HEADING_LOCKED").ToUpper();
		}
	}

	private void RefreshUIElement_BlueButtonMode()
	{
		this._blueButtonMode = BlueButtonPressMode.None;
		if (this.IsCurrentPartOwned)
		{
			if (!this.IsCurrentPartFitted && !ExpressMode)
			{
				this._blueButtonMode = BlueButtonPressMode.FitPart;
			}
		}
		else if (this.IsCurrentPartBeingDelivered)
		{
			if (ExpressMode)
			{
				this._blueButtonMode = BlueButtonPressMode.None;
			}
			else
			{
				this._blueButtonMode = BlueButtonPressMode.DeliverPart;
			}
		}
		else if (this.IsCurrentCarAnEvoCar() && !this.IsCurrentPartEvoUnlocked)
		{
			this._blueButtonMode = BlueButtonPressMode.UnlockEvoPart;
		}
		else if (this.CanCurrentPartBePurchased)
		{
			this._blueButtonMode = BlueButtonPressMode.BuyPart;
		}
	}

    protected void RefreshUIElement_CostContainer()
	{
		this.UpdateCostContainerForArrival = false;
		if (this.IsCurrentPartOwned)
		{
			if (this.IsCurrentPartFitted)
			{
				this.CostContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FITTED"), BaseRuntimeControl.State.Disabled,OnBlueButton);
			}
			else if (!ExpressMode && !this.IsCurrentCarABossCar())
			{
                this.CostContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FIT"), BaseRuntimeControl.State.Active, OnBlueButton);
			}
			else
			{
                this.CostContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_LOCKED"), BaseRuntimeControl.State.Disabled, OnBlueButton);
			}
		}
		else
		{
			if (this.CurrentCar == null)
			{
				return;
			}
			if (ArrivalManager.Instance.isUpgradeOnOrder(this.CurrentCar.CarDBKey, this.CurrentUpgradeData.UpgradeType))
			{
				this.UpdateCostContainerForArrival = true;
				this.updateCostContainerArrivalTime();
			}
			else if (this.IsCurrentCarAnEvoCar())
			{
				if (this.IsCurrentPartEvoUnlocked)
				{
					string title = (!ExpressMode) ? LocalizationManager.GetTranslation("TEXT_BUTTON_UPGRADE").ToUpper() : CurrencyUtils.GetColouredGoldString(LocalizationManager.GetTranslation("TEXT_TRACKSIDE"));
                    this.CostContainer.SetupForCost(this.GetCashPrice(), this.GetGoldPrice(), title, OnBlueButtonCashOption, OnBlueButtonGoldOption, OnBlueButtonGoldOption);
				}
				else
				{
					int upgradeLevel = (int)this.CurrentUpgradeData.UpgradeLevel;
					int num = this.CurrentCar.UpgradeStatus.Values.Count((CarUpgradeStatus x) => (int)x.evoOwned > upgradeLevel);
					int num2 = CarDatabase.Instance.GetCar(this.CurrentCar.CarDBKey).EvolutionUpgradesEarned(upgradeLevel);
					if (num2 > num)
					{
						this.CostContainer.SetupForEvolutionButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_UNLOCK_EVO"), BaseRuntimeControl.State.Active,OnBlueButton);
					}
					else
					{
						this.CostContainer.SetupForEvolutionButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_LOCKED_EVO"), BaseRuntimeControl.State.Disabled,OnBlueButton);
					}
				}
			}
			else if (this.CanCurrentPartBePurchased)
			{
				string title2 = (!ExpressMode) ? LocalizationManager.GetTranslation("TEXT_BUTTON_UPGRADE").ToUpper() : CurrencyUtils.GetColouredGoldString(LocalizationManager.GetTranslation("TEXT_TRACKSIDE"));
                this.CostContainer.SetupForCost(this.GetCashPrice(), this.GetGoldPrice(), title2, OnBlueButtonCashOption, OnBlueButtonGoldOption, OnBlueButtonGoldOption);
			}
			else
			{
				this.CostContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_LOCKED"), BaseRuntimeControl.State.Disabled,OnBlueButton);
			}
		}
	}

	private void updateCostContainerArrivalTime()
	{
		if (this.CurrentCar == null)
		{
			return;
		}
		string str = LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_ARRIVES_IN:");
		string format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES_AND_SECONDS");
		int num = 0;
		int num2 = 0;
		ArrivalManager.Instance.GetTimeUntilDelivery(this.CurrentCar.CarDBKey, this.CurrentUpgradeData.UpgradeType, out num, out num2);
		string str2 = string.Format(format, num, num2);
	    string title = str2;//str2 + " " + str;
		BaseRuntimeControl.State state = (!ExpressMode) ? BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled;
		this.CostContainer.SetupForBlueButton(title, LocalizationManager.GetTranslation("TEXT_BUTTON_DELIVER"), state, OnBlueButton);
	}

	private void RefreshUIElement_FreeUpgradesText()
	{
        if (this.HaveFreeUpgrades())
        {
            this.FreeUpgradeToken.gameObject.SetActive(true);
            string textID = (this.NumberOfFreeUpgrades() <= 1) ? "TEXT_FREE_UPGRADE_REMAINING" : "TEXT_FREE_UPGRADES_REMAINING";
            string text = string.Format(LocalizationManager.GetTranslation(textID), this.NumberOfFreeUpgrades());
            this.FreeUpgradesText.text = text.ToUpper();
        }
        else
        {
            this.FreeUpgradeToken.gameObject.SetActive(false);
        }
	}

	private void SetSelectedIndex(eUpgradeType type)
	{
        //CurrentUpgradeType = type;
		InternalStartScreenOn = type;
        CoroutineManager.Instance.StartCoroutine(_delayed(type));
	}

    private IEnumerator _delayed(eUpgradeType type)
    {
        upgradeScrollerController.SetTweenTypeToImmediate();
        yield return new WaitForSeconds(0.1F);
        CurrentUpgradeType = type;
        upgradeScrollerController.ResetTweenType();
    }

	private void OnBlueButton()
	{
		switch (this._blueButtonMode)
		{
		case BlueButtonPressMode.None:
			return;
		case BlueButtonPressMode.FitPart:
			this.OnBlueFitButton();
			return;
		case BlueButtonPressMode.BuyPart:
			this.OnBlueBuyButton(false);
			return;
		case BlueButtonPressMode.DeliverPart:
			this.OnBlueDeliverButton();
			return;
		case BlueButtonPressMode.UnlockEvoPart:
			this.OnBlueEvoUnlockButton();
			return;
		default:
			return;
		}
	}

	private void OnBlueButtonGoldOption()
	{
		switch (this._blueButtonMode)
		{
		case BlueButtonPressMode.None:
			return;
		case BlueButtonPressMode.FitPart:
			this.OnBlueFitButton();
			return;
		case BlueButtonPressMode.BuyPart:
			this.OnBlueBuyButton(true);
			return;
		case BlueButtonPressMode.DeliverPart:
			this.OnBlueDeliverButton();
			return;
		default:
			return;
		}
	}

	private void OnBlueButtonCashOption()
	{
		switch (this._blueButtonMode)
		{
		case BlueButtonPressMode.None:
			return;
		case BlueButtonPressMode.FitPart:
			this.OnBlueFitButton();
			return;
		case BlueButtonPressMode.BuyPart:
			this.OnBlueBuyButton(false);
			return;
		case BlueButtonPressMode.DeliverPart:
			this.OnBlueDeliverButton();
			return;
		default:
			return;
		}
	}

	private void OnBlueFitButton()
	{
        CarStatsCalculator.Instance.CalculateUpgradeScreenPerformanceIndex();
		this.FitCurrentUpgradeLevelInProfileAndRefreshUI();

        //Update server
        //int zLevel = PlayerProfileManager.Instance.ActiveProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType);
	    //CarUpgradeData upgradeInQuestion = CarDatabase.Instance.GetCarUpgrade(this.CurrentCar.CarDBKey,
	    //    this.CurrentUpgradeType, zLevel);
	}

	private void OnBlueBuyButton(bool IsGoldOptionPurchase)
	{
		BubbleManager.Instance.DismissMessages();
		//eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
        //eCarTier highestCarTierOwned = PlayerProfileManager.Instance.ActiveProfile.GetHighestCarTierOwned();
        //if (highestCarTierOwned < highestUnlockedClass && highestCarTierOwned < eCarTier.TIER_5 && !PlayerProfileManager.Instance.ActiveProfile.DoneUpgradeWarningOnNewTier)
        //{
        //    this.DoAgentWarningPopup();
        //    PlayerProfileManager.Instance.ActiveProfile.DoneUpgradeWarningOnNewTier = true;
        //    return;
        //}
		this.CurrentBuyMode = ((!this.HaveFreeUpgrades()) ? ((!IsGoldOptionPurchase) ? CostType.CASH : CostType.GOLD) : CostType.FREE);
		if (this.CurrentCar == null)
		{
			return;
		}

		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int zLevel = activeProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType) + 1;
		CarUpgradeData upgradeInQuestion = CarDatabase.Instance.GetCarUpgrade(this.CurrentCar.CarDBKey, this.CurrentUpgradeType, zLevel);
		int num = this.GetCashPrice();
		int goldPrice = this.GetGoldPrice();
		if (!activeProfile.CanBuyUpgrade(this.CurrentCar, num, goldPrice, this.CurrentBuyMode))
		{
		    if ((this.CurrentBuyMode == CostType.CASH || this.CurrentBuyMode == CostType.CASHANDGOLD) &&
		        activeProfile.GetCurrentCash() < num)
		    {
                MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(
                    new ItemTypeId("upg", upgradeInQuestion.AssetDatabaseID), new ItemCost
                    {
                        CashCost = num,
                        GoldCost = goldPrice
                    }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY_UPGRADE", delegate
                    {
                        this.DoGoldPurchase(goldPrice, this.CurrentCar, upgradeInQuestion);
                    }, null, null, null);
                //PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Cash,
                //    "BuyUpgrade", upgradeInQuestion.AssetDatabaseID, () =>
                //{
                //    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy,
                //        ShopScreen.PurchaseSelectionType.Select);
                //    ScreenManager.Instance.PushScreen(ScreenID.Shop);
                //});
            }
		    else
		    {
                MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(
                    new ItemTypeId("upg", upgradeInQuestion.AssetDatabaseID), new ItemCost
                    {
                        GoldCost = goldPrice
                    }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_UPGRADE", null, null, null);

                //PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                //    "BuyUpgrade", upgradeInQuestion.AssetDatabaseID, () =>
                //{
                //    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                //        ShopScreen.PurchaseSelectionType.Select);
                //    ScreenManager.Instance.PushScreen(ScreenID.Shop);
                //});
		    }
		    return;
		}
		this._purchaseInProgress = new PurchaseInfo();
		this._purchaseInProgress.CarBeingHandled = this.CurrentCar;
		this._purchaseInProgress.UpgradeBeingHandled = upgradeInQuestion;
		num = ((this.CurrentBuyMode != CostType.FREE && this.CurrentBuyMode != CostType.GOLD && !ExpressMode) ? num : 0);
		goldPrice = ((this.CurrentBuyMode != CostType.FREE && this.CurrentBuyMode != CostType.CASH) ? goldPrice : 0);
        //PopUpDatabase.Upgrades.ShowConfirmBuyUpgradePopUp((float)num, (float)goldPrice, this.PurchaseUpgradeInProfile, CancelPurchaseUpgradeInProfile);
	    PurchaseUpgradeInProfile();
	    LogToMetrics();
	}

	private void LogToMetrics()
	{
		switch (CurrentUpgradeType)
		{
			case eUpgradeType.BODY:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstBodyUpgrade)
				{
					Log.AnEvent(Events.IsFirstBodyUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstBodyUpgrade = true;
				}
				break;
			case eUpgradeType.ENGINE:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstEngineUpgrade)
				{
					Log.AnEvent(Events.IsFirstEngineUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstEngineUpgrade = true;
				}
				break;
			case eUpgradeType.INTAKE:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstIntakeUpgrade)
				{
					Log.AnEvent(Events.IsFirstIntakeUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstIntakeUpgrade = true;
				}
				break;
			case eUpgradeType.NITROUS:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstNitrousUpgrade)
				{
					Log.AnEvent(Events.IsFirstNitroUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstNitrousUpgrade = true;
				}
				break;
			case eUpgradeType.TRANSMISSION:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstTransmissionUpgrade)
				{
					Log.AnEvent(Events.IsFirstTransmissionUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstTransmissionUpgrade = true;
				}
				break;
			case eUpgradeType.TURBO:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstTurboUpgrade)
				{
					Log.AnEvent(Events.IsFirstTurboUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstTurboUpgrade = true;
				}
				break;
			case eUpgradeType.TYRES:
				if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstTyresUpgrade)
				{
					Log.AnEvent(Events.IsFirstTyresUpgrade);
					PlayerProfileManager.Instance.ActiveProfile.IsFirstTyresUpgrade = true;
				}
				break;
			default:
				return;
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private void OnBlueEvoUnlockButton()
	{
		CostContainerEvolution componentInChildren = this.CostContainer.gameObject.GetComponentInChildren<CostContainerEvolution>();
		if (componentInChildren != null)
		{
            //base.CarouselList.DisableAllItems(true);
            //base.HorizontalList.ListenForGestures = false;
            //AnimationUtils.PlayAnim(componentInChildren.animation, "ResearchAnimation");
		}
	}

	public void UnlockEvoToken()
	{
		int indexOfCurrentValue = uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue;
        //base.CarouselList.DisableAllItems(false);
        //base.HorizontalList.ListenForGestures = true;
		this.CurrentCar.UpgradeStatus[this.CurrentUpgradeType].evoOwned = (byte)indexOfCurrentValue;
		this.RefreshAllUI();
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private void DoGoldPurchase(int GoldPrice, CarGarageInstance CarInQuestion, CarUpgradeData UpgradeInQuestion)
	{
		this.CurrentBuyMode = CostType.GOLD;
		this._purchaseInProgress = new PurchaseInfo();
		this._purchaseInProgress.CarBeingHandled = CarInQuestion;
		this._purchaseInProgress.UpgradeBeingHandled = UpgradeInQuestion;
		PopUpDatabase.Upgrades.ShowConfirmBuyUpgradePopUp(0f, (float)GoldPrice, new PopUpButtonAction(this.PurchaseUpgradeInProfile),CancelPurchaseUpgradeInProfile);
	}

	private void OnBlueDeliverButton()
	{
		Arrival arrivalForUpgrade = ArrivalManager.Instance.GetArrivalForUpgrade(this.CurrentCar.CarDBKey, this.CurrentUpgradeType);
		PopUpManager.Instance.TryShowPopUp(DeliverNow.GetPopup(arrivalForUpgrade, new Action(this.OnDeliverNow), true), PopUpManager.ePriority.Default, null);
	}

	private void DoAgentWarningPopup()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_UPGRADE_NEWTIER_WARNING_TITLE",
			BodyText = "TEXT_POPUPS_UPGRADE_NEWTIER_WARNING",
			IsBig = true,
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void OnDeliverNow()
	{
		this.FitCurrentUpgradeLevelInProfileAndRefreshUI(true);
	}

	private void CachePurchaseUpgradeMetricsEvent(int deltaCash, int deltaGold, string upgradeType, string hasShortCutDelivery, string receiptVerified)
	{
		this.newCachedMetricsEventToFlush = true;
		this.cachedMetricsUpgradeTypeString = upgradeType;
		this.cachedMetricsIsShortCut = hasShortCutDelivery;
		this.cachedMetricsReceiptVerified = receiptVerified;
		this.cachedMetricsDeltaCash = deltaCash;
		this.cachedMetricsDeltaGold = deltaGold;
	}

	private void FitCurrentUpgradeLevelInProfileAndRefreshUI()
	{
		this.FitCurrentUpgradeLevelInProfileAndRefreshUI(false);
	}

	private void FitCurrentUpgradeLevelInProfileAndRefreshUI(bool forceIncrement)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
	    string soundName = (uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue <= activeProfile.GetUpgradeLevelFitted(this.CurrentUpgradeType))
	        ? AudioEvent.Frontend_UpgradeRemove
	        : AudioEvent.Frontend_UpgradeAdd;
        upgradeScrollerController.PlayFitAnimation(CurrentUpgradeType);
		AudioManager.Instance.PlaySound(soundName, Camera.main.gameObject);
        activeProfile.SetUpgradeLevelFitted(this.CurrentUpgradeType, uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue);
        //ConnectionOrder.
		this.carPPBeforeUpgrade = activeProfile.GetCurrentCar().CurrentPPIndex;
        upgradeScrollerController.SetUpgradeLevel(CurrentUpgradeType, uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue);
		if (this._purchaseInProgress != null || forceIncrement)
		{
			this._purchaseInProgress = null;
            if (uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue < uiListButton.runtimeButtonBehaviour.MaxIndex)
            {
                uiListButton.runtimeButtonBehaviour.SetCurrentIndex(uiListButton.runtimeButtonBehaviour.IndexOfCurrentValue + 1);
            }
            else
            {
                RefreshAllUI();
            }
        }
		else
		{
			this.RefreshAllUI();
		}
		this.ShowTutorialPopUpRelevantToCurrentStateIfThereIsOne();
	}

	private void RecordMetricsDeltas(PlayerProfile profile, int cashBefore, int goldBefore)
	{
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.DCsh,
				(profile.GetCurrentCash() - cashBefore).ToString()
			},
			{
				Parameters.DGld,
				(profile.GetCurrentGold() - goldBefore).ToString()
			},
			{
				Parameters.ItmClss,
				"upg"
			},
			{
				Parameters.Itm,
				this._purchaseInProgress.UpgradeBeingHandled.UpgradeType.ToString() + ((int)(this._purchaseInProgress.UpgradeBeingHandled.UpgradeLevel + 1)).ToString()
			},
			{
				Parameters.PCrPP,
				this.carPPBeforeUpgrade.ToString()
			}
		};
		Log.AnEvent(Events.PurchaseItem, data);
	}

	private void PurchaseUpgradeInProfile()
	{
		if (this._purchaseInProgress == null)
		{
			return;
		}
		if (this._purchaseInProgress.CarBeingHandled == null || this._purchaseInProgress.UpgradeBeingHandled == null)
		{
			return;
		}
        CommonUI.Instance.XPStats.LevelUpLockedState(true);
		MenuAudio.Instance.playSound(AudioSfx.Purchase);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int currentCash = activeProfile.GetCurrentCash();
		int currentGold = activeProfile.GetCurrentGold();
		int cashPrice = this.GetCashPrice(this._purchaseInProgress.UpgradeBeingHandled);
		int goldPrice = this.GetGoldPrice(this._purchaseInProgress.UpgradeBeingHandled);
		activeProfile.OrderUpgrade(this._purchaseInProgress.CarBeingHandled, this.CurrentBuyMode, cashPrice, goldPrice);
        //DifficultyManager.OnUpgradePurchased();
	    var byDelay = this.CurrentUpgradeData.IsImportPart && this.CurrentBuyMode != CostType.GOLD &&
	    this.CurrentBuyMode != CostType.FREE;
	    int xpPrize;
        if (byDelay)
		{
			Arrival arrival = new Arrival();
			arrival.deliveryTimeSecs = (float)(GameDatabase.Instance.Currencies.GetPartDeliveryTime(CarDatabase.Instance.GetCar(CurrentCar.CarDBKey).BaseCarTier) * 60);
			arrival.arrivalType = ArrivalType.Upgrade;
			arrival.carId = this.CurrentCar.CarDBKey;
			arrival.upgradeType = this.CurrentUpgradeType;
			ArrivalManager.Instance.AddArrival(arrival);
			this.RefreshAllUI();
			this.RecordMetricsDeltas(activeProfile, currentCash, currentGold);
		    xpPrize = GameDatabase.Instance.XPEvents.GetXPPrizeForUpgradePurchase(this._purchaseInProgress.UpgradeBeingHandled);
            GameDatabase.Instance.XPEvents.AddPlayerXP(xpPrize);
			Arrival arrivalForUpgrade = ArrivalManager.Instance.GetArrivalForUpgrade(this.CurrentCar.CarDBKey, this.CurrentUpgradeType);
			PopUpManager.Instance.TryShowPopUp(DeliverNow.GetPopup(arrivalForUpgrade, new Action(this.OnDeliverNow), false), PopUpManager.ePriority.Default, null);
            CommonUI.Instance.XPStats.LevelUpLockedState(false);
			AchievementChecks.CheckForAllUpgradesAchievement();
            return;
		}

        CarStatsCalculator.Instance.CalculateUpgradeScreenPerformanceIndex();
		activeProfile.GiveUpgrade(this._purchaseInProgress.CarBeingHandled.CarDBKey, this._purchaseInProgress.UpgradeBeingHandled.UpgradeType);
	    xpPrize =
	        GameDatabase.Instance.XPEvents.GetXPPrizeForUpgradePurchase(this._purchaseInProgress.UpgradeBeingHandled);
        GameDatabase.Instance.XPEvents.AddPlayerXP(xpPrize);
        //AchievementChecks.CheckForAllUpgradesAchievement();
		string hasShortCutDelivery = (this.CurrentBuyMode != CostType.FREE) ? "0" : "2";
	    this.CachePurchaseUpgradeMetricsEvent(activeProfile.GetCurrentCash() - currentCash,
	        activeProfile.GetCurrentGold() - currentGold,
	        this._purchaseInProgress.UpgradeBeingHandled.UpgradeType.ToString() +
	        ((int) (this._purchaseInProgress.UpgradeBeingHandled.UpgradeLevel + 1)).ToString(), hasShortCutDelivery,
	        string.Empty);
		this.FitCurrentUpgradeLevelInProfileAndRefreshUI();
        CommonUI.Instance.XPStats.LevelUpLockedState(false);

	    GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID);

	    StartCoroutine(DelayInput());
	}


    private IEnumerator DelayInput()
    {
        ScreenManager.Instance.Interactable = false;
        yield return new WaitForSeconds(1);
        ScreenManager.Instance.Interactable = true;
    }

    private void CancelPurchaseUpgradeInProfile()
    {
    }

	private void ShowTutorialPopUpRelevantToCurrentStateIfThereIsOne()
	{
        if (!this._haveWarnedTyresThisSession && AgentUpgradeNags.TryNag_UpgradeTyres())
        {
            this._haveWarnedTyresThisSession = true;
        }
	}

	private void SetupTextListButton()
	{
		List<string> list = new List<string>();
		list.Add(LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_LIST_BUTTON_STOCK"));
		for (int i = 0; i < 5; i++)
		{
			string format = LocalizationManager.GetTranslation("TEXT_UPGRADES_SCREEN_LIST_BUTTON_STAGE_X");
			list.Add(string.Format(format, i + 1));
		}
        this.uiListButton.runtimeButtonBehaviour.ResetList(list);
    }

    // void OnDestroy()
    //{
    //    this.UpgradeIconTextures = null;
    //    this.UpgradeIconTexturesSmallHit = null;
    //    this.uiInformationText = null;
    //    this.uiHeadingText = null;
    //    this.uiSprOverlay = null;
    //    //this.uiListButton = null;
    //    //base.OnDestroy();
    //}

    void Start()
    {
        upgradeScrollerController.SelectedIndexChanged += SelectedItemChanged;
        InternalStartScreenOn = eUpgradeType.NITROUS;
        if(!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade)
			IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial4);
}

    protected override void OnDestroy()
    {
        upgradeScrollerController.SelectedIndexChanged -= SelectedItemChanged;
        base.OnDestroy();
    }

    public override void OnCreated(bool zAlreadyOnStack)
	{
        //var carkey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
        //var carInfo = CarDatabase.Instance.GetCar(carkey);
        //var performanceIndexData = GameDatabase.Instance.CarsConfiguration.CarPPData;
        //CarStatsCalculator.Instance = new CarStatsCalculator(new CarPhysics(), carInfo, performanceIndexData);
        //CarStatsCalculator.Instance.CarStatsUpdated += carStatsCalculator_CarStatsUpdated;
        //this.DataDrivenNode.CreateDataDrivenObjectFromAssetID("UI_Upgrade_Screen", new DataDrivenObject.DataDrivenObjectCreatedDelegate(this.DataDrivenObjectCreated));
        //base.CreateCrosshair();
        this.SetupTextListButton();
        //base.OnActivate(zAlreadyOnStack);
        InternalStartScreenOn = eUpgradeType.BODY;
		this.CurrentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        this.SelectedItemChanged(0);
        CarStatsCalculator.Instance.ResetStatCache();
        CarInfoUI.Instance.ResetDeltaStatCache();
        CarInfoUI.Instance.RepositionFor(ScreenID.Tuning);
		this.carPPBeforeTracksidePurchase = this.CurrentCar.CurrentPPIndex;
		this.carPPBeforeUpgrade = this.CurrentCar.CurrentPPIndex;
        if (PlayerProfileManager.Instance.ActiveProfile.RacesEntered >= 2 && GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 0)
        {
	        Log.AnEvent(Events.TapToUpgrade);
        }

        if (!PlayerProfileManager.Instance.ActiveProfile.IsFirstTapToUpgrade)
        {
	        PlayerProfileManager.Instance.ActiveProfile.IsFirstTapToUpgrade = true;
	        Log.AnEvent(Events.FirstTapToUpgrade);
        }
        GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID, null);

        //FrontendStatBarData frontendCarStatBarData = GameDatabase.Instance.CarsConfiguration.FrontendCarStatBarData;
        //frontendCarStatBarData.MaxCarWeight = 6500;
        //m_powerSlider.maxValue = frontendCarStatBarData.MaxHorsePower;
        //m_bodySlider.maxValue = frontendCarStatBarData.MaxCarWeight;
        //m_gripSlider.maxValue = frontendCarStatBarData.MaxTyreGrip;
        //m_tranmisionSlider.maxValue = frontendCarStatBarData.MaxGearShiftTime;
    }

    public override void OnAfterActivate()
    {
        base.OnAfterActivate();
        //upgradeScrollerController.Interactable = PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade;
        upgradeScrollerController.Reload();
        if (ExternalStartScreenOn != eUpgradeType.INVALID)
        {
            this.SetSelectedIndex(ExternalStartScreenOn);
        }
        else
        {
            this.SetSelectedIndex(InternalStartScreenOn);
        }
    }

    public override void OnDeactivate()
	{
        InternalStartScreenOn = this.CurrentUpgradeType;
        CarInfoUI.Instance.ResetCarData();
        StartScreenOnCurrentOwned = false;
        base.OnDeactivate();
    }

	protected override void Update()
	{
        base.Update();
		if (this.UpdateCostContainerForArrival)
		{
			if (!ArrivalManager.Instance.isUpgradeOnOrder(this.CurrentCar.CarDBKey, this.CurrentUpgradeData.UpgradeType))
			{
				this.RefreshAllUI();
			}
			else
			{
				this.updateCostContainerArrivalTime();
			}
		}
        if (this.newCachedMetricsEventToFlush && !CarStatsCalculator.Instance.IsCalculatingPerformance)
		{
			this.newCachedMetricsEventToFlush = false;
			Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
			{
				{
					Parameters.DCsh,
					this.cachedMetricsDeltaCash.ToString()
				},
				{
					Parameters.DGld,
					this.cachedMetricsDeltaGold.ToString()
				}
			};
			if (ExpressMode)
			{
				RacePlayerInfoComponent component = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
				int num = component.PPIndex - this.carPPBeforeTracksidePurchase;
				dictionary[Parameters.Type] = "TSU";
				dictionary[Parameters.CostGold] = (-this.cachedMetricsDeltaGold).ToString();
				dictionary[Parameters.CostCash] = "0";
				dictionary[Parameters.PreUpgradeDelta] = num.ToString();
                //if (!MultiplayerUtils.GetMultiplayerStreakType().Equals("NA"))
                //{
                //    dictionary[Parameters.StreakType] = MultiplayerUtils.GetMultiplayerStreakType();
                //}
				Log.AnEvent(Events.MultiplayerPurchase, dictionary);
			}
			else
			{
				dictionary[Parameters.ItmClss] = "upg";
				dictionary[Parameters.Itm] = this.cachedMetricsUpgradeTypeString;
				dictionary[Parameters.Shortcut] = this.cachedMetricsIsShortCut;
				dictionary[Parameters.RecVer] = this.cachedMetricsReceiptVerified;
				dictionary[Parameters.PCrPUpgdPP] = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CurrentPPIndex.ToString();
				dictionary[Parameters.PCrPP] = this.carPPBeforeUpgrade.ToString();
				dictionary[Parameters.Upsl] = ((!OfferMode) ? "no" : ExternalStartScreenOn.ToString());
                //if (ScreenManager.Instance.IsScreenOnStack(ScreenID.FriendTimeList))
                //{
                //    string carDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
                //    float bestTimeForCar = PlayerProfileManager.Instance.ActiveProfile.GetBestTimeForCar(carDBKey);
                //    //StarStats myStarStats = StarsManager.GetMyStarStats();
                //    int num2 = 0;
                //    myStarStats.NumStars.TryGetValue(StarType.GOLD, out num2);
                //    dictionary.Add(Parameters.LPos, LumpManager.Instance.GetLeaderboardPositionForTimeInCar(carDBKey, bestTimeForCar).ToString());
                //    dictionary.Add(Parameters.Friends, LumpManager.Instance.FriendLumps.Count.ToString());
                //    dictionary.Add(Parameters.CmlStars, myStarStats.TotalStars.ToString());
                //    dictionary.Add(Parameters.CarStars, ((int)StarsManager.GetStarForTime(carDBKey, bestTimeForCar)).ToString());
                //    dictionary.Add(Parameters.ThreeStarCars, num2.ToString());
                //    dictionary.Add(Parameters.FriendUpgrade, "TRUE");
                //    dictionary.Add(Parameters.FriendsInCar, (from q in LumpManager.Instance.FriendLumps.Values
                //    where q.FriendHasCarTime(carDBKey)
                //    select q).Count<CachedFriendRaceData>().ToString());
                //}
				Log.AnEvent(Events.PurchaseItem, dictionary);
			}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			this.carPPBeforeUpgrade = activeProfile.GetCurrentCar().CurrentPPIndex;
			this.carPPBeforeTracksidePurchase = activeProfile.GetCurrentCar().CurrentPPIndex;
			if (this.BuyButtonBubbleMessage != null)
			{
				this.BuyButtonBubbleMessage.Dismiss();
				this.BuyButtonBubbleMessage = null;
			}
		}


        //if (CarStatsCalculator.Instance != null)
        //{
        //    CarStatsCalculator.Instance.Update();
        //}
	}

	void PopulateItemLists()
	{
        //foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        //{
        //    int num = CarUpgrades.ValidUpgrades.IndexOf(current);
        //    bool zRoundLeft = current == CarUpgrades.ValidUpgrades[0];
        //    bool zRoundRight = current == CarUpgrades.ValidUpgrades[CarUpgrades.ValidUpgrades.Count - 1];
        //    CSRCarouselItemBuilder.AddBasicMenuItemToList(this.CarouselLists[0], num, CarUpgrades.UpgradeTextIDsForLocalisation[current], this.UpgradeIconTexturesSmallHit[num], zRoundLeft, zRoundRight);
        //}
        //base.MarkAllItemsAddedToCarousels();
	}

	void SelectedItemChanged(int i)
	{
		int upgradeLevelOwned = PlayerProfileManager.Instance.ActiveProfile.GetUpgradeLevelOwned(this.CurrentUpgradeType);
		int upgradeLevelFitted = PlayerProfileManager.Instance.ActiveProfile.GetUpgradeLevelFitted(this.CurrentUpgradeType);

        if (this.CurrentUpgradeType != InternalStartScreenOn)
		{
			StartScreenOnCurrentOwned = false;
		}
        int num = upgradeLevelFitted;
        if (!ExpressMode)
		{
			bool flag = StartScreenOnCurrentOwned || upgradeLevelFitted < upgradeLevelOwned;
			num = upgradeLevelOwned + ((!flag) ? 1 : 0);
		}
		else
		{
			num = upgradeLevelOwned + 1;
			if (num > 5)
			{
				num = upgradeLevelFitted;
			}
		}
		if (num > 5)
		{
			num = 5;
		}
        this.uiListButton.runtimeButtonBehaviour.SetCurrentIndex(num);
        this.RefreshAllUI();
		InternalStartScreenOn = this.CurrentUpgradeType;
	}

	public void GoViewUpgrade(eUpgradeType upgrade)
	{
		this.SetSelectedIndex(upgrade);
	}

    public override void RequestBackup()
	{
		if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() >= 2 && !PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade)
		{
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_FIRSTUPGRADE_FORCE_UPGRADE_TITLE",
				BodyText = "TEXT_POPUPS_FIRSTUPGRADE_FORCE_UPGRADE_BODY",
				IsBig = true,
				ConfirmAction = new PopUpButtonAction(this.RecordMetricsEvent2),
				ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
			return;
		}
        CarStatsCalculator.Instance.Stop();
        CarStatsCalculator.Instance.ApplyUpgradeSetup();
		PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
        AgentUpgradeNags.ResetLastUpgradeNags();
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		ExpressMode = false;
		OfferMode = false;
		ExternalStartScreenOn = eUpgradeType.INVALID;
        //if (ScreenManager.Instance.IsScreenOnStack(ScreenID.VSDummy))
        //{
        //    base.StartCoroutine(this.WaitForBackup());
        //    return;
        //}
        ObjectiveCommand.Execute(new CounterTuneCar(), true);
        base.RequestBackup();
	}

	private void RecordMetricsEvent2()
	{
		Log.AnEvent(Events.NoReallyYouShouldBuyUpgrade);
	}

	private IEnumerator WaitForBackup()
	{
        yield return 0;
        //TuningScreen.<WaitForBackup>c__Iterator36 <WaitForBackup>c__Iterator = new TuningScreen.<WaitForBackup>c__Iterator36();
        //<WaitForBackup>c__Iterator.<>f__this = this;
        //return <WaitForBackup>c__Iterator;
	}

	private void DataDrivenObjectCreated(GameObject go)
	{
        //this.uiSprOverlay = go.GetComponentInChildren<Image>();
        //this.uiSprOverlay.autoResize = true;
        //this.uiSprOverlay.PauseAnim();
        //this.RefreshUIElement_BigOverlay();
	}

    void carStatsCalculator_CarStatsUpdated(UpgradeScreenCarStats obj)
    {
        var car = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        RefreshUIElement_CarInfo(car, obj);
    }

    private void RefreshUIElement_CarInfo(CarGarageInstance car, UpgradeScreenCarStats obj)
    {
        //m_powerSlider.value = obj.CurrentHP;
        //m_powerSlider.value2 = obj.DeltaHP + obj.CurrentHP;
        //m_powerText.text = obj.CurrentHP.ToString();

        //m_gripSlider.value = obj.CurrentGrip;
        //m_gripSlider.value2 = obj.DeltaGrip + obj.CurrentGrip;
        //m_gripText.text = obj.CurrentGrip.ToString();

        //m_tranmisionSlider.value = obj.CurrentGearShiftTime;
        //m_tranmisionSlider.value2 = obj.DeltaGearShiftTime + obj.CurrentGearShiftTime;
        //m_transmissionText.text = obj.CurrentGearShiftTime.ToString();

        //m_bodySlider.value = obj.CurrentWeight;
        //m_bodySlider.value2 = obj.DeltaWeight + obj.CurrentWeight;
        //m_bodyText.text = obj.CurrentWeight.ToString();
    }
}
