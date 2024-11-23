using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Metrics;
using Z2HSharedLibrary.DatabaseEntity;

public class PopUpDatabaseCommon
{
	public void ShowGoGetFuelPopUp(PopUpButtonAction goGetFuelAction)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_INRACE_NOFUEL_RESTART_TITLE",
			BodyText = "TEXT_POPUPS_INRACE_NOFUEL_RESTART_BODY",
			IsBig = true,
			ConfirmAction = goGetFuelAction,
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_FUEL",
			ShouldCoverNavBar = true,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowBuyFuelAndRestartPopUp(PopUpButtonAction buyFuelAndRestartAction)
	{
		int goldToFillFuelTank = GameDatabase.Instance.Currencies.GetGoldToFillFuelTank();
		string bodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_INRACE_NOFUEL_RESTART_BODY") + "\n\n" + string.Format(CurrencyUtils.GoldColour + LocalizationManager.GetTranslation("TEXT_FILL_FUEL_TANK_BUTTON"), CurrencyUtils.GetColouredGoldString(goldToFillFuelTank));
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_INRACE_NOFUEL_RESTART_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = buyFuelAndRestartAction,
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_BUY",
			ShouldCoverNavBar = true,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowLeaderboardSkipWaitPopup(PopUpButtonAction actionSkip)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_RP_NOT_READY_HEADER",
			BodyText = "TEXT_RP_NOT_READY_BODY",
			IsBig = true,
			ConfirmAction = actionSkip,
			CancelText = "TEXT_BUTTON_WAIT",
			ConfirmText = "TEXT_RP_NOT_READY_SKIP",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowLeaderboardErrorPopup(PopUpButtonAction actionSkip, string skipString)
	{
	    string bodyText = null;//(!MultiplayerUtils.IsServerUnavailable()) ? "TEXT_POPUP_PLAYERLIST_NET_ERROR_BODY" : "TEXT_MULTIPLAYER_UNAVAILABLE";
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
			BodyText = bodyText,
			IsBig = true,
			ConfirmAction = actionSkip,
			ConfirmText = skipString,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}


    public void ShowErrorPopup()
    {
        string bodyText = "TEXT_POPUP_ERROR_BODY";
        PopUp popup = new PopUp
        {
            BodyText = bodyText,

        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }

    public PopUp GetStargazerPopup(string title, string body, PopUpButtonAction onPopupDismissed, bool bodyAlreadyTranslated = false)
	{
		return new PopUp
		{
			Title = title,
			BodyText = body,
			IsBig = true,
			ConfirmAction = onPopupDismissed,
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_stargazerPrefab,
			ImageCaption = "TEXT_NAME_FRANKIE",
			ShouldCoverNavBar = true,
			BodyAlreadyTranslated = bodyAlreadyTranslated
		};
	}

	public void ShowMultiplayerLockedPopup(PopUpButtonAction onPopupConfirm = null)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_MULTIPLAYER_LOCKED_TIER2_TITLE",
			BodyText = "TEXT_MULTIPLAYER_LOCKED_TIER2_BODY",
			IsBig = true,
			ConfirmText = "TEXT_BUTTON_OK",
			ConfirmAction = onPopupConfirm,
            GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
			ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowStargazerPopup(string title, string body, PopUpButtonAction onPopupDismissed, bool bodyAlreadyTranslated = false)
	{
		PopUpManager.Instance.TryShowPopUp(this.GetStargazerPopup(title, body, onPopupDismissed, bodyAlreadyTranslated), PopUpManager.ePriority.Objective, null);
	}

	public void ShowCouldNotConnectPopup(PopUpButtonAction onPopupConfirm = null)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_WEB_REQUEST_STATUS_CODE_0",
			BodyText = "TEXT_CONNECT_SCREEN_INFO_ERROR_CONNECTING_TO_SERVICE",
			IsBig = true,
			ConfirmAction = onPopupConfirm,
			ConfirmText = "TEXT_BUTTON_OK",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

	public void ShowStargazerPopupCancelConfirm(string title, string body, PopUpButtonAction onCancel, PopUpButtonAction onConfirm)
	{
		PopUp popup = new PopUp
		{
			Title = title,
			BodyText = body,
			CancelAction = onCancel,
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmAction = onConfirm,
			ConfirmText = "TEXT_BUTTON_BANK",
            GraphicPath = PopUpManager.Instance.graphics_stargazerPrefab,
			ImageCaption = "TEXT_NAME_FRANKIE",
			ShouldCoverNavBar = true
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
	}

	public void ShowIAPOffPopup()
    {
        var body = GetIAPOffBody();
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_IAPOFF_TITLE",
            BodyText = body,
            BodyAlreadyTranslated = true,
            IsBig = true,
			ConfirmText = "TEXT_BUTTON_OK",
			ID = PopUpID.IAPMessage,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

    private string GetIAPOffBody()
    {
        var format = LocalizationManager.GetTranslation("TEXT_POPUPS_IAPOFF_BODY");
        var appStore = BasePlatform.ActivePlatform.GetTargetAppStore();
        var appStoreName = LocalizationManager.GetTranslation("TEXT_APPSTORE_" + appStore);
        return string.Format(format, appStoreName);
    }

    public void ShowTierUnlocked(PopUpButtonAction action)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_TIER_UNLOCKED_TITLE",
			BodyText = "TEXT_POPUPS_TIER_UNLOCKED_BODYE",
			ConfirmAction = action,
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
	}


    public void ShowDailyBattleEventUnlocked(PopUpButtonAction action)
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_DAILY_BATTLE_UNLOCKED_TITLE",
            BodyText = "TEXT_POPUPS_DAILY_BATTLE_UNLOCKED_BODYE",
            ConfirmAction = action,
            ConfirmText = "TEXT_BUTTON_OK"
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
    }

	public void ShowWaitSpinnerPopup()
	{
		PopUp pop = new PopUp
		{
			Title = "TEXT_POPUP_TITLE_PLEASE_WAIT_SPINNER",
			IsBig = true,
            ID = PopUpID.WaitSpinner,
			IsWaitSpinner = true
		};
		PopUpManager.Instance.ForcePopup(pop, PopUpManager.ePriority.SystemUrgent, null);
	}

	public PopUp BuyTierCarPopup(eCarTier tier, PopUpButtonAction carOwnedAction, PopUpButtonAction buyCarAction)
	{
		List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
		int num = (int)(tier + 1);
		string title = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_ENTER_TIER_TITLE"), num);
		string bodyText = string.Empty;
		if (carsOwned.Any((CarGarageInstance q) => q.CurrentTier == tier))
		{
			bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_ENTER_TIER_HAVECAR_BODY"), num, num);
			return new PopUp
			{
				Title = title,
				TitleAlreadyTranslated = true,
				BodyText = bodyText,
				BodyAlreadyTranslated = true,
				IsBig = true,
				ConfirmAction = carOwnedAction,
				ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT",
			};
		}
		bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_TIER_UNLOCKED_BODY"), num);
		return new PopUp
		{
			Title = title,
			TitleAlreadyTranslated = true,
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = buyCarAction,
			ConfirmText = "TEXT_BUTTON_SHOWROOM",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
	}

    public void ShowBuyCarPropertyPopup(int cashPrice, int goldPrice, VirtualItemType itemType, PopUpButtonAction onConfirmBuyItem, PopUpButtonAction onCancelBuyItem)
    {
        string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_BUY_UPGRADE_CONFIRMATION"), CurrencyUtils.GetColouredCostStringBrief((int)cashPrice, (int)goldPrice,0));

        PopUp popup = new PopUp
        {
            //Title = "TEXT_POPUPS_IAPOFF_TITLE",
            BodyText = bodyText,
            BodyAlreadyTranslated = true,
            ConfirmAction = onConfirmBuyItem,
            CancelAction = onCancelBuyItem,
            CancelText = "TEXT_BUTTON_CANCEL",
            ConfirmText = "TEXT_BUTTON_OK",
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }

    public void ShowNoInternetConnectionPopup(PopUpButtonAction okAction = null)
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
            BodyText = "TEXT_NETWORK_UNAVAILABLE",
            IsBig = false,
            ConfirmText = "TEXT_BUTTON_OK",
            ConfirmAction = okAction,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }    
    
    public void ShowServerTimeMismatch(PopUpButtonAction okAction = null)
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "TEXT_POPUPS_LEADERBOARD_CHECK_YOUR_CLOCK_TITLE",
            BodyText = "TEXT_POPUPS_LEADERBOARD_CHECK_YOUR_CLOCK_BODY",
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
        }, PopUpManager.ePriority.Default, null);
    }



    public void ShowTimeoutPopop(PopUpButtonAction okAction = null)
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUP_TIME_OUT_TITLE",
            BodyText = "TEXT_POPUP_TIME_OUT_BODY",
            IsBig = false,
            ConfirmText = "TEXT_BUTTON_OK",
            ConfirmAction = okAction
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }


    public void ShowNotEnoughFundPopup(ShopScreen.ItemType itemType, string itemClass, string ItemName, PopUpButtonAction okAction)
    {
	    if (AppStore.Instance.ShouldHideIAPInterface)
	    {
		    ShowNotEnoughFundPopup_NoBankOffer();
		    return;
	    }
		    
        Log.AnEvent(Events.NotEnough, new Dictionary<Parameters, string>
        {
            {
                Parameters.Currency,
                itemType.ToString()
            },
            {
                Parameters.ItmClss,
                itemClass
            },
            {
                Parameters.Itm,
                ItemName
            },
        });
        PopUp popup = new PopUp
        {
            Title = "NOT_ENOUGH_FUND_TITLE",
            BodyText = LocalizationManager.GetTranslation("NOT_ENOUGH_FUND"),
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmAction = () =>
            {
                Log.AnEvent(Events.ConfirmBank);
                okAction();
            },
            ConfirmText = "TEXT_BUY_FUND",
            CancelText = "TEXT_BUTTON_CANCEL",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT",
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
    }
    
    public void ShowNotEnoughFundPopup_NoBankOffer ()
    {
	    PopUp popup = new PopUp
	    {
		    Title = "NOT_ENOUGH_FUND_TITLE",
		    BodyText = LocalizationManager.GetTranslation("NOT_ENOUGH_FUND_NO_BANK"),
		    BodyAlreadyTranslated = true,
		    IsBig = true,
		    CancelText = "TEXT_BUTTON_OK",
		    GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
		    ImageCaption = "TEXT_NAME_AGENT",
	    };
	    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
    }
}
