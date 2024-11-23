using DataSerialization;
using Metrics;
using System;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class BuyBundleOfferPopupDataAction : PopupDataActionBase
{
	private int popUpIndex = 2147483647;

	public override void Execute(EligibilityConditionDetails details)
	{
		this.popUpIndex = details.IntValue;
		ShopScreen.InitialiseForDirectPurchase(BundleOfferController.Instance.OfferItem, new Action(this.OnSuccess), new Action(this.OnFail));
		ScreenManager.Instance.PushScreen(ScreenID.Shop);
	}
	

	public void FireMetricsEvent()
	{
#if !UNITY_EDITOR
		if (!Debug.isDebugBuild)
		{
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.SPTitle,
					BundleOfferController.Instance.OfferItem
				},
				{
					Parameters.SPTime,
					BundleOfferController.Instance.AvailabilityTimer.ToString()
				},
				{
					Parameters.SPDiscount,
					BundleOfferController.Instance.CurrentOfferDiscount.ToString()
				},
				{
					Parameters.SPPopupIndex,
					BundleOfferController.Instance.ActiveOfferData.ID.ToString()
				}
			};
			Log.AnEvent(Events.BundlePackBought, data);
		}
#endif
	}


	public void OnSuccess()
	{
		MenuAudio.Instance.playSound(AudioSfx.Purchase);
		this.FireMetricsEvent();
		if (PlayerPrefs.GetString("mode") != "cheat")
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			bool isOfferRepeatable = BundleOfferController.Instance.ActiveOfferData.IsOfferRepeatable;
			activeProfile.SetPopupIsValid(this.popUpIndex, isOfferRepeatable);
			if (isOfferRepeatable)
			{
				activeProfile.QueuedOffersIDs.Remove(BundleOfferController.Instance.ActiveOfferData.ID);
			}
			activeProfile.SetPopupIsValid(this.popUpIndex, BundleOfferController.Instance.ActiveOfferData.IsOfferRepeatable);
			activeProfile.LastBundleOfferTimeShown = GTDateTime.Now;
			activeProfile.Save();
		}
		BundleOfferController.Instance.DismissBubbleMessage();
		BundleOfferController.Instance.HideOfferIcon();
		BundleOfferController.Instance.CleanUp();
	}

	public void OnFail()
	{
		BundleOfferController.Instance.DisplayOfferIcon();
	}
}
