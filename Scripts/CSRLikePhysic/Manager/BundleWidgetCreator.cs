using System;
using System.Collections.Generic;
using UnityEngine;

public class BundleWidgetCreator
{
	private string filePath = "BundleOfferWidgets/";

	private Dictionary<string, string> OfferWidgets = new Dictionary<string, string>
	{
		{
			"CASH",
			"BundleCashWidget"
		},
		{
			"GOLD",
			"BundleGoldWidget"
		},
		{
			"CAR",
			"BundleCarWidget"
		}
	};

	public GameObject LoadPrefab(string bundleType)
	{
		string path = this.filePath + this.OfferWidgets[bundleType];
		UnityEngine.Object original = Resources.Load(path);
		return UnityEngine.Object.Instantiate(original) as GameObject;
	}
}
