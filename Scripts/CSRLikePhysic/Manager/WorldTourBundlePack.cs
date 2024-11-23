using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WorldTourBundlePack
{
	public string ThemeName;

    public bool Active;

	public List<RepeatBundleInfo> BundleInfo;

	public int NumberOfValidOffersRemaining(List<BundleOfferData> eligibleOffers)
	{
		int num = 0;
		foreach (RepeatBundleInfo bundle in this.BundleInfo)
		{
			if (eligibleOffers.Any((BundleOfferData q) => q.ID == bundle.BundleID) && !PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(bundle.CarDBKey))
			{
				num++;
			}
		}
		return num;
	}

	public int GetRandomAvailableOfferID()
	{
		List<RepeatBundleInfo> list = (from q in this.BundleInfo
		where !PlayerProfileManager.Instance.ActiveProfile.IsCarOwned(q.CarDBKey)
		select q).ToList<RepeatBundleInfo>();
		if (list.Count<RepeatBundleInfo>() == 0)
		{
			return 0;
		}
		return list[UnityEngine.Random.Range(0, list.Count<RepeatBundleInfo>())].BundleID;
	}
}
