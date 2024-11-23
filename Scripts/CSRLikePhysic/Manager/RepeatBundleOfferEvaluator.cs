using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RepeatBundleOfferEvaluator
{
	public BundleOfferData GetRepeatOffer(List<BundleOfferData> eligibleOffers)
	{
		if (eligibleOffers.Count <= 0)
		{
			return null;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.QueuedOffersIDs.Count == 0)
		{
			this.CreateQueueOfBundleOffers(eligibleOffers);
		}
		return this.GetQueuedBundleOffer(eligibleOffers);
	}

	private BundleOfferData GetQueuedBundleOffer(List<BundleOfferData> eligibleOffers)
	{
		foreach (int id in PlayerProfileManager.Instance.ActiveProfile.QueuedOffersIDs)
		{
			BundleOfferData bundleOfferData = eligibleOffers.FirstOrDefault((BundleOfferData q) => q.ID == id);
			if (bundleOfferData != null)
			{
				return bundleOfferData;
			}
		}
		return null;
	}

	private void CreateQueueOfBundleOffers(List<BundleOfferData> eligibleOffers)
	{
		if (eligibleOffers.Count <= 3)
		{
			foreach (BundleOfferData current in eligibleOffers)
			{
				PlayerProfileManager.Instance.ActiveProfile.QueuedOffersIDs.Add(current.ID);
			}
		}
		else
		{
			BundleOfferData firstOffer = this.GetPriorityOffer(eligibleOffers);
			List<BundleOfferData> eligibleOffers2 = (from q in eligibleOffers
			where q.ID != firstOffer.ID
			select q).ToList<BundleOfferData>();
			for (int i = 0; i < 2; i++)
			{
				BundleOfferData offer = this.GetRandomOffer(eligibleOffers2);
				if (offer != null)
				{
					PlayerProfileManager.Instance.ActiveProfile.QueuedOffersIDs.Add(offer.ID);
					eligibleOffers2 = (from q in eligibleOffers
					where q.ID != offer.ID
					select q).ToList<BundleOfferData>();
				}
			}
		}
	}

	private BundleOfferData GetPriorityOffer(List<BundleOfferData> eligibleOffers)
	{
        //Uncomment when add worldTour
        List<WorldTourBundlePack> bundleOfferSet = GameDatabase.Instance.BundleOffers.Configuration.RepeatOfferSettings.BundleOfferSet;
        WorldTourBundlePack worldTourBundlePack = null;
        int minOfferCount = int.MaxValue;
        foreach (WorldTourBundlePack current in bundleOfferSet.Where(c=>c.Active))
        {
            int remainingOfferCount = current.NumberOfValidOffersRemaining(eligibleOffers);
            if (remainingOfferCount < minOfferCount && remainingOfferCount > 0)
            {
                worldTourBundlePack = current;
                minOfferCount = remainingOfferCount;
            }
        }
        if (worldTourBundlePack == null)
        {
            return null;
        }
        int selectedOfferID = worldTourBundlePack.GetRandomAvailableOfferID();
        return eligibleOffers.FirstOrDefault((BundleOfferData q) => q.ID == selectedOfferID);
    }

	private BundleOfferData GetRandomOffer(List<BundleOfferData> eligibleOffers)
	{
		if (eligibleOffers.Count == 0)
		{
			return null;
		}
		return eligibleOffers[UnityEngine.Random.Range(0, eligibleOffers.Count)];
	}
}
