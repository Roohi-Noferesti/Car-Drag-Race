using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;
using UnityEngine;

public class GetMoreFuelScreen : ZHUDScreen
{
	public enum ButtonCondition
	{
		None,
		TankNotFull,
		UpgradeAvailable,
		UnlimitedFuelAvailable,
		ExcludeAmazon
	}

	[Serializable]
	public class ConditionalButton
	{
		public RuntimeTextButton Button;

		public ButtonCondition Condition;
	}

	private const int Allow2PipsTutorialTriggerRace = 537;

	public TextMeshProUGUI Title;

    public TextMeshProUGUI Body;

    public TextMeshProUGUI WaitText;

	public RuntimeTextButton FillTankButton;

	public List<ConditionalButton> ConditionalButtonList;

    public GameObject MainPanel;

    public static bool RedirectToUnlimited;

    //public DataDrivenPortrait Mechanic;

	private static bool InTutorialMode;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.GetMoreFuel;
        }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //this.Title.text = LocalizationManager.GetTranslation(this.Title.text);
        //this.Body.text = LocalizationManager.GetTranslation(this.Body.text);
        if (activeProfile.FuelRefillsBoughtWithGold == 0 &&
            (activeProfile.FuelPips <= 0 ||
             (activeProfile.FuelPips <= GameDatabase.Instance.CurrenciesConfiguration.Fuel.BossBattleCost - 1 &&
              activeProfile.EventsCompleted.Contains(537))))
        {
            InTutorialMode = true;
        }
        if (InTutorialMode)
        {
            FuelManager.Instance.StopFuelRefills = true;
            CoroutineManager.Instance.StartCoroutine(_delayedShowTutorialPopup());
            //this.TutorialPopupInit();
        }
        FuelManager.Instance.OnFuelTimerUpdated += BuildScreen;
		this.BuildScreen();
        //this.Mechanic.Init(PopUpManager.Instance.graphics_mechanicPrefab, LocalizationManager.GetTranslation("TEXT_NAME_MECHANIC").ToUpper(), null);

        if (RedirectToUnlimited)
        {
            OnUnlimitedFuelButton();
            RedirectToUnlimited = false;
        }
	}

	private void BuildScreen()
	{
		int goldToFillFuelTank = GameDatabase.Instance.Currencies.GetGoldToFillFuelTank();
	    var goldString =
	        CurrencyUtils.GetColoredGoldString(string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_TANK_BUTTON"),
	            CurrencyUtils.GetGoldStringWithIcon(goldToFillFuelTank)));
        this.FillTankButton.SetCallback(OnFillTankButton);
        if (this.FillTankButton.spriteText.text != goldString)
        {
            this.FillTankButton.spriteText.text = goldString;
        }
		this.SetTimerText();
        //Vector3 localPosition = this.ConditionalButtonList[0].Button.transform.localPosition;
        foreach (ConditionalButton current in this.ConditionalButtonList)
        {
            BaseRuntimeControl.State buttonState = this.GetButtonState(current.Condition);
            if (current.Button != null)
            {
                current.Button.CurrentState = buttonState;
                
                if (current.Button.name == "Clear_All_Free_Button" && GTAdManager.Instance.ShouldHideAdInterface)
	                current.Button.CurrentState = BaseRuntimeControl.State.Hidden;
                
                if (current.Button.name == "Upgrade_Button" && AppStore.Instance.ShouldHideIAPInterface)
	                current.Button.CurrentState = BaseRuntimeControl.State.Hidden;
                
                //if (buttonState != BaseRuntimeControl.State.Hidden)
                //{
                //    current.Button.transform.localPosition = localPosition;
                //    localPosition.y += this.ListItemOffset;
                //}
            }
        }
	}

	private BaseRuntimeControl.State GetButtonState(ButtonCondition condition)
	{
        //return BaseRuntimeControl.State.Active;
		switch (condition)
		{
		case ButtonCondition.TankNotFull:
			if (FuelManager.Instance.GetFuel() >= FuelManager.Instance.CurrentMaxFuel())
			{
				return BaseRuntimeControl.State.Disabled;
			}
			break;
		case ButtonCondition.UpgradeAvailable:
                if (!GameDatabase.Instance.IAPs.IsGasTankUpgradeAvailable())
                {
                    return BaseRuntimeControl.State.Hidden;
                }
                if (PlayerProfileManager.Instance.ActiveProfile.HasUpgradedFuelTank)
                {
                    return BaseRuntimeControl.State.Disabled;
                }
                break;
            case ButtonCondition.UnlimitedFuelAvailable:
                if (!GameDatabase.Instance.IAPs.IsUnlimitedFuelAvailable)
                {
                    return BaseRuntimeControl.State.Hidden;
                }
                if (UnlimitedFuelManager.TimeRemaining.TotalMinutes > (double)GameDatabase.Instance.IAPs.UnlimitedFuelRenewalAvailableMinutes)
                {
                    return BaseRuntimeControl.State.Disabled;
                }
                break;
            case ButtonCondition.ExcludeAmazon:
			if (BasePlatform.ActivePlatform.GetTargetAppStore() == GTAppStore.Amazon)
			{
				return BaseRuntimeControl.State.Hidden;
			}
			break;
		}
		return (!InTutorialMode) ? BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled;
	}

    public override void OnDeactivate()
	{
	    FuelManager.Instance.OnFuelTimerUpdated -= BuildScreen;
        this.FillTankButton.CurrentState = BaseRuntimeControl.State.Active;
	}

	public void OnFillTankButton()
	{
		this.AttemptToPurchaseFuel(GameDatabase.Instance.Currencies.GetGoldToFillFuelTank());
	}

	public void OnFreeFuelButtonPressed()
	{
        SocialInviteScreen.TriggerVideoAdForFuelFlow();
        Log.AnEvent(Events.OnClearFinesFreeButtonClicked);
	}

	public void OnWaitButton()
	{
        ScreenManager.Instance.PopScreen();
    }

	public void OnUnlimitedFuelButton()
	{
        MainPanel.SetActive(false);
        FuelUpgradeShopScreen.PushWithProduct(FuelUpgradeShopScreen.ProductType.UnlimitedFuel);
        Log.AnEvent(Events.OnClearFinesWithCoinButtonClicked);
	}

	public void OnUpgradeButton()
	{
        FuelUpgradeShopScreen.PushWithProduct(FuelUpgradeShopScreen.ProductType.UpgradeTank);
        Log.AnEvent(Events.OnUpgradeFinesCapacityButtonClicked);
	}

	public void SetTimerText()
	{
		string text;
		if (FuelManager.Instance.GetFuel() >= FuelManager.Instance.CurrentMaxFuel())
		{
			text = LocalizationManager.GetTranslation("TEXT_FILL_FUEL_TANK_BUTTON_FULL");
		}
		else
		{
			int timetoRefill = FuelManager.Instance.TimeUntilNextReplenish();
			if (timetoRefill > 420f)
			{
				var prevTimezone = PermanentData.GetTimeZone("TimeZone");
				if (prevTimezone != string.Empty && prevTimezone != AndroidSpecific.GetTimeZone())
				{
					Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
					{
						{Parameters.FirstTimeZone, prevTimezone},
						{Parameters.SecondTimeZone, AndroidSpecific.GetTimeZone()}
					};
					Log.AnEvent(Events.JumpInClearFuelTimer, data);
					PermanentData.SaveTimeZone("TimeZone", AndroidSpecific.GetTimeZone());
				}
			}
			int minutes = timetoRefill / 60;
            int seconds = timetoRefill % 60;
			if (minutes == 0)
			{
                string format = LocalizationManager.GetTranslation("TEXT_WAIT_FOR_FUEL_X_SECONDS");
				text = string.Format(format, timetoRefill);
			}
			else
			{
                string format = LocalizationManager.GetTranslation("TEXT_WAIT_FOR_FUEL_X_MINUTES");
				text = string.Format(format, minutes,seconds);
			}
		}

        //Debug.Log(text);
        if (this.WaitText.text != text)
        {
            this.WaitText.text = text.ToUpper();
        }
	}

	private void AttemptToPurchaseFuel(int zCost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		if (activeProfile.GetCurrentGold() < zCost)
		{

            //MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("fuel"), new ItemCost
            //{
            //    GoldCost = zCost
            //}, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_FUEL", null, null, null);
            PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                    "Fuel","Fuel",() =>
		    {
                ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                    ShopScreen.PurchaseSelectionType.Select);
                ScreenManager.Instance.PushScreen(ScreenID.Shop);
		    });
			return;
		}
		PopUpManager.Instance.TryShowPopUp(this.getPopUp_ConfirmPurchase(zCost), PopUpManager.ePriority.Default, null);
	}

	private PopUp getPopUp_ConfirmPurchase(int zCost)
	{
		string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_GET_FUEL_CONFIRMATION"), CurrencyUtils.GetGoldStringWithIcon(zCost));
		return new PopUp
		{
			Title = "TEXT_POPUPS_ARE_YOU_SURE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.ConfirmBuyFuel),
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_BUY",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
	}

	private void ConfirmBuyFuel()
	{
		int goldToFillFuelTank = GameDatabase.Instance.Currencies.GetGoldToFillFuelTank();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.SpendGold(goldToFillFuelTank,"buy", "fuel");
		activeProfile.FuelRefillsBoughtWithGold++;
		Log.AnEvent(Events.ApproveClearFinesWithCoin);
        /*int num = */FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
        //if (CommonUI.Instance.RankBarMode == eRankBarMode.MULTIPLAYER_RANK)
        //{
        //    string multiplayerStreakType = MultiplayerUtils.GetMultiplayerStreakType();
        //    Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        //    {
        //        {
        //            Parameters.Type,
        //            "Fuel"
        //        },
        //        {
        //            Parameters.CostGold,
        //            goldToFillFuelTank.ToString()
        //        },
        //        {
        //            Parameters.CostCash,
        //            "0"
        //        },
        //        {
        //            Parameters.DGld,
        //            (-goldToFillFuelTank).ToString()
        //        },
        //        {
        //            Parameters.DCsh,
        //            "0"
        //        },
        //        {
        //            Parameters.StreakType,
        //            multiplayerStreakType
        //        }
        //    };
        //    Log.AnEvent(Events.MultiplayerPurchase, data);
        //}
        //else
        //{
        //    Dictionary<Parameters, string> data2 = new Dictionary<Parameters, string>
        //    {
        //        {
        //            Parameters.DGld,
        //            (-goldToFillFuelTank).ToString()
        //        },
        //        {
        //            Parameters.Dfuel,
        //            num.ToString()
        //        },
        //        {
        //            Parameters.ItmClss,
        //            "fuel"
        //        }
        //    };
        //    Log.AnEvent(Events.PurchaseItem, data2);
        //}
		this.BuildScreen();
	}

    private IEnumerator _delayedShowTutorialPopup()
    {
        yield return new WaitForSeconds(1);
        TutorialPopupInit();
    }

	public void TutorialPopupInit()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_FUEL_TUTORIAL1_TITLE",
			BodyText = "TEXT_POPUPS_FUEL_TUTORIAL1_BODY",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.TutorialPopupInitDone),
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT",

		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void TutorialPopupInitDone()
	{
        this.FillTankButton.CurrentState = BaseRuntimeControl.State.Active;
	    var goldString = CurrencyUtils.GetColoredGoldString(string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_TANK_BUTTON"),
	        LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_FREE")));
	    this.FillTankButton.SetText(goldString, true, false);
        this.FillTankButton.CurrentState = BaseRuntimeControl.State.Active;
        this.FillTankButton.SetCallback(OnTutorialClickButton);
	}

	public void OnTutorialClickButton()
	{
        this.FillTankButton.SetCallback(OnFillTankButton);
        PlayerProfileManager.Instance.ActiveProfile.FuelRefillsBoughtWithGold++;
		FuelManager.Instance.StopFuelRefills = false;
		InTutorialMode = false;
		FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
		this.BuildScreen();
	    StartCoroutine(_delayedShowTotorialPopup2());
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

    private IEnumerator _delayedShowTotorialPopup2()
    {
        yield return new WaitForSeconds(1);
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_FUEL_TUTORIAL2_TITLE",
            BodyText = "TEXT_POPUPS_FUEL_TUTORIAL2_BODY",
            IsBig = true,
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }

    public override void RequestBackup()
	{
        if (InTutorialMode)
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_FUEL_TUTORIAL_TRYLEAVE_TITLE",
                BodyText = "TEXT_POPUPS_FUEL_TUTORIAL_TRYLEAVE_BODY",
                IsBig = true,
                ConfirmAction = new PopUpButtonAction(this.TutorialPopupInitDone),
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }
        else
        {
            base.RequestBackup();
        }
	}
}
