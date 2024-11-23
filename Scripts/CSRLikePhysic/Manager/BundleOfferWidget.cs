using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BundleOfferWidget : MonoBehaviour
{
	public Image OfferSprite;

	public TextMeshProUGUI OfferDesc;

	public abstract void Setup(IBundleOfferWidgetInfo placementInfo);

	public bool GetProductInfo(string itemCode, out AppStoreProduct item)
	{
		if (AppStore.Instance.IsWaitingForProductData)
		{
			item = null;
		}
		else
		{
			List<AppStoreProduct> products = AppStore.Instance.GetProducts();
			item = products.FirstOrDefault((AppStoreProduct q) => q.Identifier==itemCode);
		}
		return item != null;
	}
}
