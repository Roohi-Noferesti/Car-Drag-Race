using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class MiniStoreController : MonoBehaviour
{
	public class MetricData
	{
		public string LayoutType = string.Empty;

		public string LayoutOption = string.Empty;

		public string CurrencyType = string.Empty;

		public string ItemClass = string.Empty;

		public string ItemName = string.Empty;
	}

	public static MiniStoreController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private List<ProductData> GetGoldProducts()
	{
	    List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        List<ProductData> list = new List<ProductData>();
        int num = 0;
        if (products == null)
        {
            return list;
        }
        foreach (AppStoreProduct current in products)
        {
	        var gtProduct = ProductManager.Instance.GetProductWithID(current.Identifier);
	        if (gtProduct != null)
	        {
		        ProductData productData = new ProductData(current, gtProduct);
		        if (current.Identifier.Contains("gold") && productData.GtProduct != null)
		        {
			        productData.AnimFrameIndex = num;
			        num++;
			        list.Add(productData);
		        }
	        }
        }
        return list;
	}

    private List<ProductData> GetCashProducts()
    {
        List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        List<ProductData> list = new List<ProductData>();
        int num = 0;
        if (products == null)
        {
            return list;
        }
        foreach (AppStoreProduct current in products)
        {
	        var gtProduct = ProductManager.Instance.GetProductWithID(current.Identifier);
	        if (gtProduct != null)
	        {
		        ProductData productData = new ProductData(current, gtProduct);
		        if (current.Identifier.Contains("cash"))
		        {
			        productData.AnimFrameIndex = num;
			        num++;
			        list.Add(productData);
		        }
	        }

        }
        return list;
    }

    private ProductData GetNearestGoldProduct(ItemCost Cost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<ProductData> goldProducts = this.GetGoldProducts();
		ProductData result = null;
		float num = (float)(Cost.GoldCost - activeProfile.GetCurrentGold());
		foreach (ProductData current in goldProducts)
		{
			int num2 = current.GtProduct.Gold + current.GtProduct.BonusGold;
			if ((float)num2 >= num)
			{
				result = current;
				break;
			}
		}
		return result;
	}

	private ProductData GetNextNearestGoldProduct(ItemCost Cost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<ProductData> goldProducts = this.GetGoldProducts();
		ProductData result = null;
		bool flag = false;
		float num = (float)(Cost.GoldCost - activeProfile.GetCurrentGold());
		foreach (ProductData current in goldProducts)
		{
			int num2 = current.GtProduct.Gold + current.GtProduct.BonusGold;
			if ((float)num2 >= num)
			{
				if (flag)
				{
					result = current;
					break;
				}
				flag = true;
			}
		}
		return result;
	}

	private ProductData GetNearestCashProduct(ItemCost Cost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<ProductData> cashProducts = this.GetCashProducts();
		ProductData result = null;
		float num = (float)(Cost.CashCost - activeProfile.GetCurrentCash());
		foreach (ProductData current in cashProducts)
		{
			int num2 = current.GtProduct.Cash + current.GtProduct.BonusCash;
			if ((float)num2 >= num)
			{
				result = current;
				break;
			}
		}
		return result;
	}

	private int GetMostExpensiveCashProductValue()
	{
		List<ProductData> cashProducts = this.GetCashProducts();
		ProductData productData = null;
		int num = 0;
		foreach (ProductData current in cashProducts)
		{
			int num2 = current.GtProduct.Cash + current.GtProduct.BonusCash;
			if (num2 > num)
			{
				productData = current;
				num = num2;
			}
		}
		return (productData != null) ? num : 1000000;
	}

	private MiniStoreLayout ReplaceSpecialLayoutIdentifiers(MiniStoreLayout Layout, ItemCost Cost)
	{
		for (int i = 0; i < Layout.products.Count; i++)
		{
			string text = Layout.products[i];
			if (text.Equals("nearest_gold"))
			{
				Layout.products[i] = this.GetNearestGoldProduct(Cost).GtProduct.Code;
			}
			else if (text.Equals("nearest_cash"))
			{
				Layout.products[i] = this.GetNearestCashProduct(Cost).GtProduct.Code;
			}
			else if (text.Equals("next_nearest_gold"))
			{
				Layout.products[i] = this.GetNextNearestGoldProduct(Cost).GtProduct.Code;
			}
		}
		return Layout;
	}

	private MiniStoreLayout GetLayoutFromData(ProductData Product, Dictionary<string, MiniStoreLayout> Layouts, ItemCost Cost, MetricData MetricData)
	{
		if (Product != null && Layouts.ContainsKey(Product.GtProduct.Code))
		{
			string code = Product.GtProduct.Code;
			MiniStoreLayout miniStoreLayout = Layouts[code];
			MetricData.LayoutOption = code;
			return this.ReplaceSpecialLayoutIdentifiers(miniStoreLayout.GenerateCopy(), Cost);
		}
		if (Product != null)
		{
			string code2 = Product.GtProduct.Code;
		}
		return new MiniStoreLayout
		{
			products = new List<string>(),
			stickers = new List<string>()
		};
	}

	private MiniStoreLayout GetLayout_CheapestGoldWithGoldOnly(ItemCost Cost, MetricData MetricData)
	{
		MetricData.LayoutType = "CheapestGoldWithGoldOnly";
		return this.GetLayoutFromData(this.GetNearestGoldProduct(Cost), GameDatabase.Instance.MiniStore.Configuration.CheapestGoldWithGoldOnly, Cost, MetricData);
	}

	private MiniStoreLayout GetLayout_CheapestCashWithCashOnly(ItemCost Cost, MetricData MetricData)
	{
		MetricData.LayoutType = "CheapestCashWithCashOnly";
		return this.GetLayoutFromData(this.GetNearestCashProduct(Cost), GameDatabase.Instance.MiniStore.Configuration.CheapestCashWithCashOnly, Cost, MetricData);
	}

	private MiniStoreLayout GetLayout_CheapestCashWithNearestGold(ItemCost Cost, MetricData MetricData)
	{
		MetricData.LayoutType = "CheapestCashWithNearestGold";
		return this.GetLayoutFromData(this.GetNearestCashProduct(Cost), GameDatabase.Instance.MiniStore.Configuration.CheapestCashWithNearestGold, Cost, MetricData);
	}

	private MiniStoreLayout GetLayout_AffordableGoldWithNearestGold(ItemCost Cost, MetricData MetricData)
	{
		MetricData.LayoutType = "AffordableGoldWithNearestGold";
		return this.GetLayoutFromData(this.GetNearestGoldProduct(Cost), GameDatabase.Instance.MiniStore.Configuration.AffordableGoldWithNearestGold, Cost, MetricData);
	}

	private MiniStoreLayout GetLayout_AffordableGoldWithNearestCashAndGold(ItemCost Cost, MetricData MetricData)
	{
		MetricData.LayoutType = "AffordableGoldWithNearestCashAndGold";
		return this.GetLayoutFromData(this.GetNearestGoldProduct(Cost), GameDatabase.Instance.MiniStore.Configuration.AffordableGoldWithNearestCashAndGold, Cost, MetricData);
	}

	private bool MiniStoreProductsAreAvailable(MiniStoreLayout Layout)
	{
        List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        foreach (string Product in Layout.products)
        {
            if (products.FirstOrDefault((AppStoreProduct q) => q.Identifier==Product) != null)
            {
                return true;
            }
        }

        return false;
	}

	public void ShowMiniStoreNotEnoughGold(ItemTypeId Item, ItemCost Cost, string OfflineBodyTextID, PopUpButtonAction CloseButtonAction = null, PopUpButtonAction BuyButtonAction = null, PopUpButtonAction ShopButtonAction = null)
	{
		if (AppStore.Instance.ShouldHideIAPInterface) {
			PopUpDatabase.Common.ShowNotEnoughFundPopup_NoBankOffer();
			return;
		}
		
        MiniStoreController.MetricData metricData = new MiniStoreController.MetricData
        {
            CurrencyType = "GOLD",
            ItemClass = Item.Clss,
            ItemName = Item.Itm
        };
        MiniStoreLayout layout_CheapestGoldWithGoldOnly = this.GetLayout_CheapestGoldWithGoldOnly(Cost, metricData);
        if (!PolledNetworkState.IsNetworkConnected || !this.MiniStoreProductsAreAvailable(layout_CheapestGoldWithGoldOnly))
        {
            PopUpButtonAction confirmAction = delegate
            {
                ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Select);
                KingKodeStudio.ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Shop, new ScreenID[]
                {
                    ScreenID.ShopOverview
                });
                if (ShopButtonAction != null)
                {
                    ShopButtonAction();
                }
            };
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_INSUFFICIENT_GOLD_TITLE",
                BodyText = OfflineBodyTextID,
                CancelAction = CloseButtonAction,
                ConfirmAction = confirmAction,
                CancelText = "TEXT_BUTTON_CANCEL",
                ConfirmText = "TEXT_BUTTON_OK"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }
        else
        {
            PopUp popup2 = new PopUp
            {
                Title = "TEXT_MINI_STORE_TITLE",
                IsMiniStore = true,
                hasCloseButton = true,
                MiniStoreLayoutToShow = layout_CheapestGoldWithGoldOnly,
                MiniStoreAffordableGold = Cost.GoldCost,
                MiniStoreMetricData = metricData,
                MiniStoreCloseButtonAction = CloseButtonAction,
                MiniStoreBuyButtonAction = BuyButtonAction,
                MiniStoreShopButtonAction = ShopButtonAction
            };
            PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.Default, null);
        }
	}

	public void ShowMiniStoreNotEnoughCash(ItemTypeId Item, ItemCost Cost, string OfflineBodyTextID, PopUpButtonAction UseGoldAction = null, PopUpButtonAction CloseButtonAction = null, PopUpButtonAction BuyButtonAction = null, PopUpButtonAction ShopButtonAction = null)
	{
		if (AppStore.Instance.ShouldHideIAPInterface) {
			PopUpDatabase.Common.ShowNotEnoughFundPopup_NoBankOffer();
			return;
		}
		
		if (Cost.GoldCost > 0)
		{
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		bool flag = Cost.CashCost - activeProfile.GetCurrentCash() > this.GetMostExpensiveCashProductValue();
		MetricData metricData = new MetricData
		{
			CurrencyType = "CASH",
			ItemClass = Item.Clss,
			ItemName = Item.Itm
		};
		MiniStoreLayout miniStoreLayout;
		if (Cost.GoldCost > 0)
		{
			if (activeProfile.GetCurrentGold() >= Cost.GoldCost)
			{
				if (flag)
				{
					miniStoreLayout = this.GetLayout_AffordableGoldWithNearestGold(Cost, metricData);
				}
				else
				{
					miniStoreLayout = this.GetLayout_AffordableGoldWithNearestCashAndGold(Cost, metricData);
				}
			}
			else if (flag)
			{
				miniStoreLayout = this.GetLayout_CheapestGoldWithGoldOnly(Cost, metricData);
			}
			else
			{
				miniStoreLayout = this.GetLayout_CheapestCashWithNearestGold(Cost, metricData);
			}
		}
		else
		{
			if (flag)
			{
				PopUp popup = new PopUp
				{
					Title = "TEXT_POPUPS_EARNED_NOT_BOUGHT_TITLE",
					BodyText = "TEXT_POPUPS_EARNED_NOT_BOUGHT_BODY",
					IsBig = true,
					ConfirmAction = CloseButtonAction,
					ConfirmText = "TEXT_BUTTON_OK",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
					ImageCaption = "TEXT_NAME_AGENT"
				};
				PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
				Log.AnEvent(Events.NotEnough, new Dictionary<Parameters, string>
				{
					{
						Parameters.Currency,
						metricData.CurrencyType
					},
					{
						Parameters.ItmClss,
						metricData.ItemClass
					},
					{
						Parameters.Itm,
						metricData.ItemName
					},
					{
						Parameters.IAPBought,
						"None"
					},
					{
						Parameters.StoreType,
						"EarnedNotBought"
					}
				});
				return;
			}
			miniStoreLayout = this.GetLayout_CheapestCashWithCashOnly(Cost, metricData);
		}
		if (miniStoreLayout.products.Count == 0)
		{
		}
        if (!PolledNetworkState.IsNetworkConnected || !this.MiniStoreProductsAreAvailable(miniStoreLayout))
        {
            PopUpButtonAction confirmAction = delegate
            {
                ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Select);
                ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Shop, new ScreenID[]
                {
                    ScreenID.ShopOverview
                });
                if (ShopButtonAction != null)
                {
                    ShopButtonAction();
                }
            };
            PopUp popup2 = new PopUp
            {
                Title = "TEXT_POPUPS_INSUFFICIENT_CASH_TITLE",
                BodyText = OfflineBodyTextID,
                CancelAction = CloseButtonAction,
                ConfirmAction = confirmAction,
                CancelText = "TEXT_BUTTON_CANCEL",
                ConfirmText = "TEXT_BUTTON_OK"
            };
            PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.Default, null);
        }
        else
        {
            PopUp popup3 = new PopUp
            {
                Title = "TEXT_MINI_STORE_TITLE",
                IsMiniStore = true,
                hasCloseButton = true,
                MiniStoreLayoutToShow = miniStoreLayout,
                MiniStoreAffordableGold = Cost.GoldCost,
                MiniStoreMetricData = metricData,
                MiniStoreUseGoldAction = UseGoldAction,
                MiniStoreCloseButtonAction = CloseButtonAction,
                MiniStoreBuyButtonAction = BuyButtonAction,
                MiniStoreShopButtonAction = ShopButtonAction
            };
            PopUpManager.Instance.TryShowPopUp(popup3, PopUpManager.ePriority.Default, null);
        }
	}

	public void Initialize()
	{
		GameDatabase.Instance.MiniStore.Configuration.Initialize();
	}
}
