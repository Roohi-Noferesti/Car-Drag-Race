using I2.Loc;

public class PopUpDatabaseUpgrades
{
	public void ShowConfirmBuyUpgradePopUp(float upgradePriceCash, float upgradePriceGold, PopUpButtonAction action,PopUpButtonAction cancelAction)
	{
	    string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_BUY_UPGRADE_CONFIRMATION"),
            CurrencyUtils.GetColouredCostStringBrief((int)upgradePriceCash, (int)upgradePriceGold,0));
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_BUY_UPGRADE_CONFIRMATION_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			ConfirmAction = action,
            CancelAction = cancelAction,
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_BUY",
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowShouldFitPartChangeRankPopUp(eCarTier fromTier, eCarTier toTier, PopUpButtonAction cancel, PopUpButtonAction action)
	{
		string arg = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(fromTier));
		string arg2 = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(toTier));
		string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_FITPART_CHANGETIER_BODY"), arg, arg2);
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_FITPART_CHANGETIER_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			CancelAction = cancel,
			ConfirmAction = action,
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_FIT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void ShowCheckFitPart(PopUpButtonAction fitAction)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_UPGRADE_CHECKFITENTER_TITLE",
			BodyText = "TEXT_POPUPS_UPGRADE_CHECKFITENTER_BODY",
			IsBig = true,
			ConfirmAction = fitAction,
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_FIT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}
}
