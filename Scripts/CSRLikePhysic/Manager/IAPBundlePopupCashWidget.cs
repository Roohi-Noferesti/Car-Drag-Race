using System;

public class IAPBundlePopupCashWidget : BundleOfferWidget
{
	public override void Setup(IBundleOfferWidgetInfo placementInfo)
	{
        //base.gameObject.transform.localPosition = placementInfo.Position;
        //this.OfferSprite.transform.localPosition = placementInfo.SpritePosition;
		this.SetOfferText(placementInfo.ShopItem);
        //this.OfferDesc.transform.localPosition = placementInfo.DescPosition;
	}

	private void SetOfferText(string productID)
	{
		AppStoreProduct appStoreProduct = null;
		if (!base.GetProductInfo(productID, out appStoreProduct))
		{
			return;
		}
		string cashString = CurrencyUtils.GetCashString(ProductManager.Instance.GetProductWithID(appStoreProduct.Identifier).Cash + ProductManager.Instance.GetProductWithID(appStoreProduct.Identifier).BonusCash);
		this.OfferDesc.text = cashString;
	}
}
