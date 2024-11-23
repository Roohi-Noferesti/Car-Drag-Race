using System;

public class IAPBundlePopupGoldWidget : BundleOfferWidget
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
		string goldString = CurrencyUtils.GetGoldStringWithIcon(ProductManager.Instance.GetProductWithID(appStoreProduct.Identifier).Gold + ProductManager.Instance.GetProductWithID(appStoreProduct.Identifier).BonusGold);
		this.OfferDesc.text = goldString;
	}
}
