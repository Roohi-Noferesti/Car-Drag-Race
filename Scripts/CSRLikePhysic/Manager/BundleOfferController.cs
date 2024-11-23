using System;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class BundleOfferController : BaseIAPOffersController
{
	private bool hasSeenThisSession;

	public BundleOfferData ActiveOfferData;

	public static BundleOfferController Instance
	{
		get;
		private set;
	}

	public bool ActiveOfferHasTimer()
	{
		return this.ActiveOfferData.PopupData.TimerActive;
	}

	private void Awake()
	{
		this.ValidBubbleMessageScreens.Add(ScreenID.Workshop);
		this.ValidBubbleMessageScreens.Add(ScreenID.Manufacturer);
		BundleOfferController.Instance = this;
	}

	public void StageOfferForDisplaying(BundleOfferData offerData)
	{
        this.SetupOffer(offerData.ID, offerData.PopupData.BundleOfferValidityDurationTimeSpan, offerData.PopupData.BundleOfferItem);
		base.DisplayOfferIcon();
		this.ActivateOffer(offerData);
		base.StartBubbleMessage();
	}

	public bool TryShowIAPBundleForScreen(bool forceDisplay = false)
	{
		if (AppStore.Instance.ShouldHideIAPInterface)
		{
			return false;
		}
		
		
        if (!PolledNetworkState.IsNetworkConnected)
        {
            if (ScreenManager.Instance.CurrentScreen == ScreenID.ShopOverview)
            {
                ScreenManager.Instance.PushScreen(ScreenID.Shop);
            }
            return false;
        }
		BundleOffersDatabase bundleOffers = GameDatabase.Instance.BundleOffers;
        BundleOfferData bundleOfferData;
#if UNITY_EDITOR
        if (bundleOffers.Configuration.DebugOneTimeOfferIndex > -1)
        {
            ActiveOfferData = bundleOfferData = bundleOffers.Configuration.OneTimeOffers[bundleOffers.Configuration.DebugOneTimeOfferIndex];
            forceDisplay = true;
        }
        else if (bundleOffers.Configuration.DebugRepeatedOfferIndex > -1)
        {
            ActiveOfferData = bundleOfferData = bundleOffers.Configuration.RepeatableOffers[bundleOffers.Configuration.DebugRepeatedOfferIndex];
            forceDisplay = true;
        }
        else
        {
            if (this.ActiveOfferData != null)
            {
                bundleOfferData = this.ActiveOfferData;
            }
            else
            {
                bundleOfferData = bundleOffers.GetEligbleOneTimeBundleOffer();
                if (bundleOfferData == null)
                {
                    bundleOfferData = bundleOffers.GetEligibleRepeatableOffer();
                    if (bundleOfferData == null)
                    {
                        return false;
                    }
                }
                this.ActivateOffer(bundleOfferData);
            }
        }
#else
            if (this.ActiveOfferData != null)
            {
                bundleOfferData = this.ActiveOfferData;
            }
            else
            {
                bundleOfferData = bundleOffers.GetEligbleOneTimeBundleOffer();
                if (bundleOfferData == null)
                {
                    bundleOfferData = bundleOffers.GetEligibleRepeatableOffer();
                    if (bundleOfferData == null)
                    {
                        return false;
                    }
                }
                this.ActivateOffer(bundleOfferData);
            }
#endif

        forceDisplay |= (PlayerProfileManager.Instance.ActiveProfile.GetPopupSeenCount(bundleOfferData.ID) == 0);
		int cumulativeSessions = PlayerProfileManager.Instance.ActiveProfile.CumulativeSessions;
		int numberOfSessionsBetweenOfferShown = GameDatabase.Instance.BundleOffers.Configuration.NumberOfSessionsBetweenOfferShown;
		if (forceDisplay || (!this.hasSeenThisSession && cumulativeSessions % numberOfSessionsBetweenOfferShown == 0))
		{
			this.hasSeenThisSession = true;
			this.ShowOfferPopup();
		}
		return true;
	}

	private void ShowOfferPopup()
	{
		PopUp popUp = this.ActiveOfferData.GetPopUp();
		if (popUp == null)
		{
			return;
		}
		if (PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Objective, null))
		{
			this.ActiveOfferData.PopupShowSuccess();
		}
	}

	private void DeActivateOffer()
	{
		this.OfferActive = false;
		this.ActiveOfferData = null;
		PlayerProfileManager.Instance.ActiveProfile.LastBundleOfferPopupID = -1;
		base.HideOfferIcon();
	}

	private void ActivateOffer(BundleOfferData activeOffer)
	{
		this.OfferActive = true;
		this.ActiveOfferData = activeOffer;
		PlayerProfileManager.Instance.ActiveProfile.LastBundleOfferPopupID = activeOffer.ID;
	}

	public override void CleanUp()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenID.ShopOverview)
		{
			ShopOverviewScreen shopOverviewScreen = ScreenManager.Instance.ActiveScreen as ShopOverviewScreen;
			if (shopOverviewScreen != null)
			{
				shopOverviewScreen.SetOfferButtons(false);
			}
		}
		this.DeActivateOffer();
	}

	public void SetupOffer(int id, TimeSpan validityDuration, string offerItem)
	{
		this.FirstSeenTime = PlayerProfileManager.Instance.ActiveProfile.GetPopupFirstSeenTime(id);
		this.ValidityDuration = validityDuration;
		this.OfferItem = offerItem;
	}

	public override bool TimerHasExpired()
	{
		return this.ActiveOfferData.PopupData.TimerActive && base.TimerHasExpired();
	}

	public override string GetTimeRemainingMessage()
	{
		if (!this.ActiveOfferData.PopupData.TimerActive)
		{
		    return LocalizationManager.GetTranslation("TEXT_BUNDLE_OFFER_AVAILABLE");
		}
		return base.GetTimeRemainingMessage();
	}
}
