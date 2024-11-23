using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BundleOffersDatabase : ConfigurationAssetLoader
{
	public BundleOffersPopUpConfiguration Configuration
	{
		get;
		private set;
	}

    public BundleOffersDatabase()
        : base(GTAssetTypes.configuration_file, "BundleIAPSConfiguration")
    {
        this.Configuration = null;
    }

	protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (BundleOffersPopUpConfiguration) scriptableObject;//JsonConverter.DeserializeObject<BundleOffersPopUpConfiguration>(assetDataString);
		if (this.Configuration.OneTimeOffers == null)
		{
		}
		this.Configuration.OneTimeOffers.ForEach(delegate(BundleOfferData offer)
		{
			offer.Initialise();
		});
		if (this.Configuration.RepeatableOffers == null)
		{
		}
		this.Configuration.RepeatableOffers.ForEach(delegate(BundleOfferData offer)
		{
			offer.Initialise();
		});
		if (Application.isPlaying)
		{
			this.UpdateOffers();
		}
	}

	public void UpdateOffers()
	{
		int lastBundleOfferPopupID = PlayerProfileManager.Instance.ActiveProfile.LastBundleOfferPopupID;
		if (!this.CheckForActiveAndStageForDisplay(this.Configuration.OneTimeOffers, lastBundleOfferPopupID))
		{
			this.CheckForActiveAndStageForDisplay(this.Configuration.RepeatableOffers, lastBundleOfferPopupID);
		}
	}

	private bool CheckForActiveAndStageForDisplay(List<BundleOfferData> offers, int lastSeenPopupID)
	{
		BundleOfferData bundleOfferData = offers.FirstOrDefault((BundleOfferData q) => q.ID == lastSeenPopupID);
		if (bundleOfferData == null)
		{
			return false;
		}
		IGameState gs = new GameStateFacade();
		if (!bundleOfferData.IsEligible(gs))
		{
			BundleOfferController.Instance.CleanUp();
			return false;
		}
		if (!this.CheckAppStoreHasItem(bundleOfferData))
		{
			return false;
		}
		BundleOfferController.Instance.StageOfferForDisplaying(bundleOfferData);
		return true;
	}

	public BundleOfferData GetEligbleOneTimeBundleOffer()
	{
		List<BundleOfferData> oneTimeOffers = this.Configuration.OneTimeOffers.Where(o => o.Active).ToList();
		IEnumerable<BundleOfferData> source = this.FilterGameStateAndAppStoreReadyBundleOffers(oneTimeOffers);
		IOrderedEnumerable<BundleOfferData> source2 = from p in source
		orderby p.Priority descending
		select p;
		return source2.FirstOrDefault<BundleOfferData>();
	}

	public BundleOfferData GetEligibleRepeatableOffer()
    {
        List<BundleOfferData> list = this.Configuration.RepeatableOffers.Where(o => o.Active).ToList();
        IGameState gs = new GameStateFacade();
        list = (from pd in list
                where pd.IsEligible(gs)
                select pd).ToList<BundleOfferData>();
        RepeatBundleOfferEvaluator repeatBundleOfferEvaluator = new RepeatBundleOfferEvaluator();
        return repeatBundleOfferEvaluator.GetRepeatOffer(list);
	}

	public IEnumerable<BundleOfferData> FilterGameStateAndAppStoreReadyBundleOffers(IEnumerable<BundleOfferData> offers)
	{
		IGameState gs = new GameStateFacade();
		IEnumerable<BundleOfferData> source = from pd in offers
		where pd.IsEligible(gs)
		select pd;
		return source.Where(new Func<BundleOfferData, bool>(this.CheckAppStoreHasItem));
	}

	private bool CheckAppStoreHasItem(BundleOfferData offer)
	{
		if (!AppStore.Instance.CheckItemAvailable(offer.PopupData.BundleOfferItem))
		{
			return false;
		}
		foreach (IBundleOfferWidgetInfo current in offer.PopupData.WidgetInfo)
		{
			if (!string.IsNullOrEmpty(current.ShopItem) && !AppStore.Instance.CheckItemAvailable(current.ShopItem))
			{
				return false;
			}
		}
		return true;
	}
}
