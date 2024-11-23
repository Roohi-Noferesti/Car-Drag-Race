using System;
using System.Collections.Generic;

[Serializable]
public class BundleOfferProfileData
{
	private string LAST_BUNDLE_ID_PROFILE_KEY = "ldpi";

	private string QUEUED_BUNDLES_PROFILE_KEY = "qboi";

	public int LastBundleOfferedPopupID;

	public List<int> QueuedOffersIDs = new List<int>();

	public void ToJson(ref JsonDict dict)
	{
		dict.Set(this.LAST_BUNDLE_ID_PROFILE_KEY, this.LastBundleOfferedPopupID);
		dict.Set(this.QUEUED_BUNDLES_PROFILE_KEY, this.QueuedOffersIDs);
	}

	public void FromJson(ref JsonDict dict)
	{
		dict.TryGetValue(this.LAST_BUNDLE_ID_PROFILE_KEY, out this.LastBundleOfferedPopupID);
		if (!dict.TryGetValue(this.QUEUED_BUNDLES_PROFILE_KEY, out this.QueuedOffersIDs))
		{
			this.QueuedOffersIDs = new List<int>();
		}
	}
}
