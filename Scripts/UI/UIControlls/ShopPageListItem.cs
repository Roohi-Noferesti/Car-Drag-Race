using Metrics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopPageListItem : ListItem
{
	public const int NUM_ITEMS_ON_PAGE = 3;

	public PrefabPlaceholder[] ShopItemPlaceholders;

	public ProductData LastTappedProduct;

	public ShopListItem GetShopItem(int i)
	{
		return this.ShopItemPlaceholders[i].GetBehaviourOnPrefab<ShopListItem>();
	}

	public OfferPackShopListItem GetOfferPackShopItem(int i)
	{
		return this.ShopItemPlaceholders[i].GetBehaviourOnPrefab<OfferPackShopListItem>();
	}

	public void Create(List<ProductData> products, ShopScreen.ItemType productType, int firstItemOffsetIndex, float pageWidth, GameObject uiPrefab)
	{
		this._ignoreThePressLock = true;
        //float num = pageWidth / 3f;
        //float num2 = -pageWidth / 2f;
		for (int i = 0; i < products.Count; i++)
		{
			if (i >= 3)
			{
				return;
			}
			if (productType == ShopScreen.ItemType.OfferPack)
			{
				OfferPackShopListItem offerPackShopItem = this.GetOfferPackShopItem(i);
				offerPackShopItem.Create(products[i], uiPrefab,null);
				offerPackShopItem.Tap += new ShopListItem.TapEventHandler(this.OnChildTap);
			}
			else
			{
				ShopListItem shopItem = this.GetShopItem(i);
				shopItem.Create(products[i], productType, uiPrefab,null);
				shopItem.Tap += new ShopListItem.TapEventHandler(this.OnChildTap);
			}
            //float zVal = num2 + num / 2f + num * (float)i;
            //GameObjectHelper.SetLocalX(this.ShopItemPlaceholders[i].gameObject, GameObjectHelper.To2DP(zVal));
		}
		for (int j = 0; j < 3 - products.Count; j++)
		{
			ShopListItem shopItem2 = this.GetShopItem(products.Count + j);
			shopItem2.gameObject.SetActive(false);
		}
	}

	public override void Shutdown()
	{
		for (int i = 0; i < this.ShopItemPlaceholders.Length; i++)
		{
			this.GetShopItem(i).Tap -= new ShopListItem.TapEventHandler(this.OnChildTap);
		}
	}

	private void OnChildTap(ShopListItem item)
	{
		this.LastTappedProduct = item.Product;
		Log.AnEvent(Events.ClickIAP, new Dictionary<Parameters, string>
		{
			{
				Parameters.IAP,
				this.LastTappedProduct.GtProduct.ToString()
			}
		});
		base.InvokeTapEvent();
	}

	protected override void Show()
	{
		for (int i = 0; i < this.ShopItemPlaceholders.Length; i++)
		{
			this.GetShopItem(i).Show();
		}
	}

	protected override void Hide()
	{
		for (int i = 0; i < this.ShopItemPlaceholders.Length; i++)
		{
			this.GetShopItem(i).Hide();
		}
	}
}
