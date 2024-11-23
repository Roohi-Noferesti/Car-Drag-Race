using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public static class DeliverNow
{

	public static string PopUpBodyText(this Arrival arrival)
	{
	    var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
        int goldCostForSkip = arrival.GetGoldCostForSkip(secondsPerGold);
		string textID = (arrival.arrivalType != ArrivalType.Car) ? "TEXT_POPUPS_UPGRADEDELIVERY_BODY" : "TEXT_POPUPS_CARDELIVERY_BODY";
		return string.Format(LocalizationManager.GetTranslation(textID), arrival.GetRemainingMinutes(), CurrencyUtils.GetColouredCostStringBrief(0, goldCostForSkip,0), CurrencyUtils.GetColouredCurrentGoldString());
	}

	public static string PopUpTitleText(this Arrival arrival)
	{
		return (arrival.arrivalType != ArrivalType.Car) ? "TEXT_POPUPS_UPGRADEDELIVERY_TITLE" : "TEXT_POPUPS_CARDELIVERY_TITLE";
	}

	public static string GetItmClss(this Arrival arrival)
	{
		return (arrival.arrivalType != ArrivalType.Car) ? "upg" : "car";
	}

	public static string GetItm(this Arrival arrival)
	{
		return (arrival.arrivalType != ArrivalType.Car) ? arrival.upgradeType.ToString() : arrival.carId;
	}

	public static PopUp GetPopup(Arrival arrival, Action onDelivered, bool userSelected)
	{
        var secondsPerGold = GameDatabase.Instance.Currencies.getSecondsGainedPerGold();
        int goldCostForSkip = arrival.GetGoldCostForSkip(secondsPerGold);
		string bodyText = arrival.PopUpBodyText();
		string textID = (!userSelected) ? "TEXT_BUTTON_ILLWAIT" : "TEXT_BUTTON_CANCEL";
        List<ButtonDetails> list = new List<ButtonDetails>
        {
            new ButtonDetails
            {
                Label = CurrencyUtils.GetColouredCostStringBrief(0, goldCostForSkip,0),
                Type = (goldCostForSkip <= 0) ? ButtonManager.CSRButtonType.GreenCompact : ButtonManager.CSRButtonType.GreenCompactGold,
                Action = delegate
                {
                    if (DeliverNow.doDeliver(arrival))
                    {
                        onDelivered();
                    }
                }
            },
            new ButtonDetails
            {
                Label = LocalizationManager.GetTranslation(textID),
                Type = ButtonManager.CSRButtonType.BlackTextCompact,
                Action = new PopUpButtonAction(DeliverNow.doCloseAndSave)
            }
        };
		if (goldCostForSkip > 0)
		{
			VideoForRewardConfiguration configuration = 
				(arrival.arrivalType == ArrivalType.Car
				? GameDatabase.Instance.Ad.GetConfiguration(VideoForRewardConfiguration.eRewardID.CarDelivery)
				: GameDatabase.Instance.Ad.GetConfiguration(VideoForRewardConfiguration.eRewardID.PartDelivery));
			
            if (configuration.Enabled) //&& GTAdManager.Instance.AnyEnabled(configuration.GetAdSpace()))
            {
                int rewardAmount = VideoForRewardsManager.GetRewardAmount(configuration);
                int timeMinutes = (int)Mathf.Floor((float)(rewardAmount / 60));
                string bubbleMessage;
                if (rewardAmount > arrival.GetRemainingSeconds())
                {
                    bubbleMessage = LocalizationManager.GetTranslation("TEXT_BUTTON_DELIVERNOW");
                }
                else
                {
                    string textID2 = (timeMinutes != 1) ? "TEXT_VIDEO_FOR_DELIVERY_REDUCE_BY_PLURAL" : "TEXT_VIDEO_FOR_DELIVERY_REDUCE_BY_SINGLUAR";
                    bubbleMessage = string.Format(LocalizationManager.GetTranslation(textID2), timeMinutes);
                }

                if (!GTAdManager.Instance.ShouldHideAdInterface) {
	                list.Add(new ButtonDetails
	                {
		                Label = rewardAmount > arrival.GetRemainingSeconds()? LocalizationManager.GetTranslation("TEXT_BUTTON_DELIVERNOW") : string.Format(LocalizationManager.GetTranslation("TEXT_VIDEO_FOR_RPBONUS_PRE_OK_BUTTON"), timeMinutes),
		                Type = ButtonManager.CSRButtonType.BlackTextTVIcon,
		                Action = delegate
		                {
			                DeliverNow.doWatchToSkip(arrival);
		                },
		                BubbleMessage = bubbleMessage
	                });
                }
            }
		}
		return new PopUp
		{
			Title = arrival.PopUpTitleText(),
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
            GraphicPath = PopUpManager.Instance.graphics_postmanPrefab,
			ImageCaption = "TEXT_NAME_DELIVERY",
            Buttons = list
		};
	}

	public static void doWatchToSkip(Arrival arrival)
	{
		PopUpManager.Instance.KillPopUp();
        if (BasePlatform.ActivePlatform.GetReachability() == BasePlatform.eReachability.OFFLINE)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup();
            return;
        }
        ArrivalManager.Instance.RequestPriorityDelivery(arrival);
	}

	public static bool doDeliver(Arrival arrival)
	{
		if (arrival.arrivalType == ArrivalType.Car)
		{
			return doDeliverCar(arrival);
		}
		else
		{
			return doDeliverUpgrade(arrival);
		}
	}

	private static bool doDeliverCar(Arrival arrival)
	{
		if (PopUpManager.Instance.isShowingPopUp)
		{
			PopUpManager.Instance.KillPopUp();
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int currentGold = activeProfile.GetCurrentGold();
        if (!activeProfile.DeliverCarNow(arrival.carId))
        {
            PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                "DeliverCar", arrival.carId, () =>
            {
                ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                    ShopScreen.PurchaseSelectionType.Select);
                ScreenManager.Instance.PushScreen(ScreenID.Shop);
            });
            //MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("dlvr"), new ItemCost
            //{
            //    GoldCost = activeProfile.GetDeliveryCost(arrival.carId)
            //}, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_DELIVERY", null, null, null);
            return false;
        }
		PlayerProfileManager.Instance.ActiveProfile.GiveCar(arrival.carId, arrival.ColourIndex, false);
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.DGld,
				(activeProfile.GetCurrentGold() - currentGold).ToString()
			},
			{
				Parameters.ItmClss,
				arrival.GetItmClss()
			},
			{
				Parameters.Itm,
				arrival.carId
			},
			{
				Parameters.Shortcut,
				"1"
			}
		};
		Log.AnEvent(Events.PurchaseItem, data);
		ArrivalManager.Instance.RemoveCar(arrival.carId);
		FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
		activeProfile.Save();
	    return true;
	}

	private static bool doDeliverUpgrade(Arrival arrival)
	{
		if (PopUpManager.Instance.isShowingPopUp)
		{
			PopUpManager.Instance.KillPopUp();
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int currentGold = activeProfile.GetCurrentGold();
        if (!activeProfile.DeliverUpgradeNow(arrival.carId, arrival.upgradeType))
        {
            PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                "DeliverUpgrade", arrival.carId, () =>
                {
                    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                        ShopScreen.PurchaseSelectionType.Select);
                ScreenManager.Instance.PushScreen(ScreenID.Shop);
            });
            //MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("dlvr"), new ItemCost
            //{
            //    GoldCost = activeProfile.GetUpgradeDeliveryCost(arrival.carId, arrival.upgradeType)
            //}, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_DELIVERY", null, null, null);
            return false;
        }
		int num = activeProfile.GetUpgradeLevelOwned(arrival.upgradeType) + 1;
		string value = arrival.upgradeType.ToString() + num.ToString();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.DGld,
				(activeProfile.GetCurrentGold() - currentGold).ToString()
			},
			{
				Parameters.ItmClss,
				arrival.GetItmClss()
			},
			{
				Parameters.Itm,
				value
			},
			{
				Parameters.Shortcut,
				"1"
			}
		};
		Log.AnEvent(Events.PurchaseItem, data);
        CarStatsCalculator.Instance.CalculateUpgradeScreenPerformanceIndex();
		activeProfile.GiveUpgrade(arrival.carId, arrival.upgradeType);
		ArrivalManager.Instance.RemoveUpgrade(arrival.carId, arrival.upgradeType);
	    return true;
	}

	private static void doCloseAndSave()
	{
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		PopUpManager.Instance.InitiateKillPopup();
	}
}
