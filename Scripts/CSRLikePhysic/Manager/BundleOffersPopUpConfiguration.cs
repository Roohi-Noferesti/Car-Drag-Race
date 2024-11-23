
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BundleOffersPopUpConfiguration:ScriptableObject
{
	public int NumberOfSessionsBetweenOfferShown;

	public List<BundleOfferData> OneTimeOffers = new List<BundleOfferData>();

	public List<BundleOfferData> RepeatableOffers = new List<BundleOfferData>();

	public RepeatBundleSettings RepeatOfferSettings = new RepeatBundleSettings();
    public int DebugOneTimeOfferIndex;
    public int DebugRepeatedOfferIndex;
}
