using System;
using System.Collections.Generic;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class MiniStorePopup : PopUpDialogue, IBundleOwner
{
	private const int NumStoreItems = 3;

//	public PrefabPlaceholder UseGoldItemPlaceholder;

	public Sprite[] GoldSprites;
	public Sprite[] CashSprites;

	public MiniStoreUseGoldItem UseGoldItem;

//	public PrefabPlaceholder[] ShopItemPlaceholders;

	public ShopListItem[] ShopItems;

	public float ProductSpriteScale = 0.8f;

	private bool AssetBundlesLoaded;

	private int NumAssetBundlesLoading = -1;

	private bool StoreItemsInitialised;

	public GameObject GoldUIPrefab;

	public GameObject CashUIPrefab;

	private MiniStoreLayout LayoutToShow;

	private int AffordableGold;

	private MiniStoreController.MetricData MetricData;

	private PopUpButtonAction UseGoldAction;

	private PopUpButtonAction BuyButtonAction;

	private PopUpButtonAction ShopButtonAction;

	private PopUpButtonAction CloseButtonAction;

	private BubbleMessage ShopButtonMessage;

	private bool IsHidden;

	private ShopListItem GetStoreItem(int i)
	{
		return ShopItems[i];
//		return this.ShopItemPlaceholders[i].GetBehaviourOnPrefab<ShopListItem>();
	}

	private MiniStoreUseGoldItem GetUseGoldItem()
	{
		return UseGoldItem;
//		return this.UseGoldItemPlaceholder.GetBehaviourOnPrefab<MiniStoreUseGoldItem>();
	}

//	private MiniStoreItemGlow GetItemGlow(int i)
//	{
//		return this.ShopItemPlaceholders[i].GetBehaviourOnPrefab<MiniStoreItemGlow>();
//	}

	public void Show()
	{
		if (this.IsHidden)
		{
			base.gameObject.transform.localPosition += new Vector3(0f, 0f, 10f);
			this.IsHidden = false;
		}
	}

	public void Hide()
	{
		if (!this.IsHidden)
		{
			base.gameObject.transform.localPosition -= new Vector3(0f, 0f, 10f);
			this.IsHidden = true;
		}
	}

	private void ShowShopButtonMessage()
	{
		//Vector3 offset = new Vector3(-0.16f, -0.24f, 0f);
		if (this.ShopButtonMessage == null)
		{
            //base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(7.5f, "TEXT_MINI_STORE_MORE_OPTIONS", false, CommonUI.Instance.NavBar.ShopButton.gameObject.transform, offset, BubbleMessage.NippleDir.UP, 0.5f, BubbleMessageConfig.ThemeStyle.SMALL, delegate(BubbleMessage b)
            //{
            //    if (PopUpManager.Instance.isShowingPopUp && PopUpManager.Instance.GetCurrentPopUp() == this.popup)
            //    {
            //        this.ShopButtonMessage = b;
            //    }
            //    else
            //    {
            //        b.KillNow();
            //    }
            //}));
		}
	}

	private void HideShopButtonMessage()
	{
		if (this.ShopButtonMessage != null)
		{
			this.ShopButtonMessage.Dismiss();
			this.ShopButtonMessage = null;
		}
	}

	public override void Setup(PopUp Pop)
	{
		base.Setup(Pop);
        this.LayoutToShow = Pop.MiniStoreLayoutToShow;
		this.AffordableGold = Pop.MiniStoreAffordableGold;
        this.MetricData = Pop.MiniStoreMetricData;
		this.UseGoldAction = Pop.MiniStoreUseGoldAction;
		this.BuyButtonAction = Pop.MiniStoreBuyButtonAction;
		this.ShopButtonAction = Pop.MiniStoreShopButtonAction;
		this.CloseButtonAction = Pop.MiniStoreCloseButtonAction;
        //CommonUI.Instance.NavBar.ForceEnableShopButtonForPopup(this.StopOutsideClicksObject.transform.position.z, new Action(this.OnShopButtonTapped));
		this.ShowShopButtonMessage();
//		this.RequestUIAssetBundles();
		//GT
		SetupStoreItems();
		this.Hide();
	}

	private Dictionary<string, MiniStoreItemData> ConstructItemDataDictionary()
	{
	    Dictionary<string, MiniStoreItemData> dictionary = new Dictionary<string, MiniStoreItemData>();
        List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        int num = 0;
        int num2 = 0;
        foreach (AppStoreProduct current in products)
        {
            ProductData productData = new ProductData(current, ProductManager.Instance.GetProductWithID(current.Identifier));
            if (productData.GtProduct!=null && current.Identifier.Contains("gold"))
            {
                productData.AnimFrameIndex = num;
                num++;
                MiniStoreItemData value = new MiniStoreItemData
                {
                    Product = productData,
                    Type = ShopScreen.ItemType.Gold,
                    UIPrefab = this.GoldUIPrefab
                };
                dictionary.Add(productData.GtProduct.Code, value);
            }
            else if (productData.GtProduct!=null && current.Identifier.Contains("cash"))
            {
                productData.AnimFrameIndex = num2;
                num2++;
                MiniStoreItemData value2 = new MiniStoreItemData
                {
                    Product = productData,
                    UIPrefab = this.CashUIPrefab
                };
                dictionary.Add(productData.GtProduct.Code, value2);
            }
        }
        return dictionary;
	}

    private void OnDestroy()
	{
        //CommonUI.Instance.NavBar.ResetShopButtonPosition();
		this.HideShopButtonMessage();
	}

	private void Close()
	{
		PopUpManager.Instance.KillPopUp();
	}

	private void LogMetricForPurchase(string IAPBought, int PlayerGoldBefore, int PlayerCashBefore)
	{
		if (this.MetricData != null)
		{
			Log.AnEvent(Events.NotEnough, new Dictionary<Parameters, string>
			{
				{
					Parameters.Currency,
					this.MetricData.CurrencyType
				},
				{
					Parameters.ItmClss,
					this.MetricData.ItemClass
				},
				{
					Parameters.Itm,
					this.MetricData.ItemName
				},
				{
					Parameters.IAPBought,
					IAPBought
				},
				{
					Parameters.StoreType,
					this.MetricData.LayoutType
				},
				{
					Parameters.StoreOption,
					this.MetricData.LayoutOption
				},
				{
					Parameters.BGld,
					PlayerGoldBefore.ToString()
				},
				{
					Parameters.BCsh,
					PlayerCashBefore.ToString()
				}
			});
		}
	}

	private void LogMetric(string IAPBought, int GoldSpent = 0)
	{
		if (this.MetricData != null)
		{
			Log.AnEvent(Events.NotEnough, new Dictionary<Parameters, string>
			{
				{
					Parameters.Currency,
					this.MetricData.CurrencyType
				},
				{
					Parameters.ItmClss,
					this.MetricData.ItemClass
				},
				{
					Parameters.Itm,
					this.MetricData.ItemName
				},
				{
					Parameters.IAPBought,
					IAPBought
				},
				{
					Parameters.StoreType,
					this.MetricData.LayoutType
				},
				{
					Parameters.StoreOption,
					this.MetricData.LayoutOption
				},
				{
					Parameters.DGld,
					GoldSpent.ToString()
				}
			});
		}
	}

	public void OnStoreItemTapped(ShopListItem Item)
	{
		string ItemIdentifier = Item.Product.GtProduct.Code;
		int PlayerGoldBefore = PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold();
		int PlayerCashBefore = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
        ShopScreen.InitialiseForDirectPurchase(Item.Product.AppStoreProduct.Identifier, delegate
        {
            this.LogMetricForPurchase(ItemIdentifier, PlayerGoldBefore, PlayerCashBefore);
        }, delegate
        {
            this.LogMetricForPurchase("Failure", PlayerGoldBefore, PlayerCashBefore);
        });
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
		this.Close();
		if (this.BuyButtonAction != null)
		{
			this.BuyButtonAction();
		}
	}

	public void OnUseGoldItemTapped(ShopListItem Item)
	{
		if (this.UseGoldAction != null)
		{
			this.LogMetric("UseGold", this.AffordableGold);
			this.Close();
			this.UseGoldAction();
		}
	}

	public void OnShopButtonTapped()
	{
		this.LogMetric("Store", 0);
		if (this.ShopButtonAction != null)
		{
			this.ShopButtonAction();
		}
	}

	public override void OnCloseButton()
	{
		base.OnCloseButton();
		this.LogMetric("None", 0);
		MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
		this.Close();
		if (this.CloseButtonAction != null)
		{
			this.CloseButtonAction();
		}
	}

	private GTProduct.StickerType StickerStringToType(string StickerString)
	{
		if (Enum.IsDefined(typeof(GTProduct.StickerType), StickerString))
		{
			return (GTProduct.StickerType)((int)Enum.Parse(typeof(GTProduct.StickerType), StickerString));
		}
		return GTProduct.StickerType.None;
	}

	private void SetupStoreItems()
	{
		Dictionary<string, MiniStoreItemData> dictionary = this.ConstructItemDataDictionary();
		bool flag = false;
		int num;
		if (this.LayoutToShow == null)
		{
			num = 0;
		}
		else
		{
			num = Math.Min(3, this.LayoutToShow.products.Count);
		}
		for (int i = 0; i < num; i++)
		{
			string text = this.LayoutToShow.products[i];
			MiniStoreItemData miniStoreItemData;
			if (dictionary.ContainsKey(text))
			{
				miniStoreItemData = dictionary[text];
			}
			else
			{
				miniStoreItemData = null;
			}
			ShopListItem storeItem = this.GetStoreItem(i);
			if (miniStoreItemData == null && text.Equals("affordable_gold"))
			{
				MiniStoreUseGoldItem useGoldItem = this.GetUseGoldItem();
				Vector3 position = storeItem.transform.position;
				useGoldItem.transform.position = new Vector3(position.x, position.y, position.z);
				useGoldItem.Create(this.GoldUIPrefab, this.AffordableGold);
				useGoldItem.Tap += new ShopListItem.TapEventHandler(this.OnUseGoldItemTapped);
				useGoldItem.transform.SetParent(storeItem.transform.parent, false);
				useGoldItem.transform.SetSiblingIndex(storeItem.transform.GetSiblingIndex());
				storeItem.gameObject.SetActive(false);
				flag = true;
			}
			else if (miniStoreItemData != null)
			{
				if (i < this.LayoutToShow.stickers.Count)
				{
					string stickerString = this.LayoutToShow.stickers[i];
					GTProduct.StickerType stickerType = this.StickerStringToType(stickerString);
					if (stickerType != GTProduct.StickerType.None)
					{
					}
					miniStoreItemData.Product.GtProduct.Sticker = stickerType;
				}
				else
				{
					miniStoreItemData.Product.GtProduct.Sticker = GTProduct.StickerType.None;
				}
//				if (i < this.LayoutToShow.glowing.Count && this.LayoutToShow.glowing[i])
//				{
//					this.GetItemGlow(i).Create();
//				}
				var sprites = miniStoreItemData.Type == ShopScreen.ItemType.Gold ? GoldSprites : CashSprites;
				storeItem.Create(miniStoreItemData.Product, miniStoreItemData.Type, miniStoreItemData.UIPrefab,sprites[miniStoreItemData.Product.AnimFrameIndex]);
				storeItem.Tap += new ShopListItem.TapEventHandler(this.OnStoreItemTapped);
                //if (storeItem.TitleText.TotalWidth > 0.8f)
                //{
                //    float num2 = 0.8f / storeItem.TitleText.TotalWidth;
                //    storeItem.TitleText.SetCharacterSize(storeItem.TitleText.characterSize * num2);
                //}
			}
			else
			{
				storeItem.CreateDummy();
			}
		}
		for (int j = num; j < 3; j++)
		{
			this.GetStoreItem(j).CreateDummy();
		}
		if (!flag)
		{
			this.GetUseGoldItem().gameObject.SetActive(false);
		}
		this.StoreItemsInitialised = true;
		this.Show();
	}

//	protected override void Update()
//	{
//		base.Update();
//		if (/*this.AssetBundlesLoaded && */ !this.StoreItemsInitialised)
//		{
//			this.SetupStoreItems();
//		}
//	}

//	private void RequestUIAssetBundles()
//	{
//		this.NumAssetBundlesLoading = 2;
//		AssetProviderClient.Instance.RequestAsset("UI_Shop_Cash", new BundleLoadedDelegate(this.CashUIPrefabReady), this);
//		AssetProviderClient.Instance.RequestAsset("UI_Shop_Gold", new BundleLoadedDelegate(this.GoldUIPrefabReady), this);
//	}

//	private void GoldUIPrefabReady(string assetID, AssetBundle assetBundle, IBundleOwner owner)
//	{
//		this.NumAssetBundlesLoading--;
//		if (this.NumAssetBundlesLoading == 0)
//		{
//			this.AssetBundlesLoaded = true;
//		}
//        this.GoldUIPrefab = DataDrivenObject.CreateDataDrivenScreenPrefab(assetBundle);
//		AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
//		//this.GoldUIPrefab.transform.localScale = Vector3.one * this.ProductSpriteScale;
//	}

//	private void CashUIPrefabReady(string assetID, AssetBundle assetBundle, IBundleOwner owner)
//	{
//		this.NumAssetBundlesLoading--;
//		if (this.NumAssetBundlesLoading == 0)
//		{
//			this.AssetBundlesLoaded = true;
//		}
//        this.CashUIPrefab = DataDrivenObject.CreateDataDrivenScreenPrefab(assetBundle);
//		AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
//		//this.CashUIPrefab.transform.localScale = Vector3.one * this.ProductSpriteScale;
//	}
}
