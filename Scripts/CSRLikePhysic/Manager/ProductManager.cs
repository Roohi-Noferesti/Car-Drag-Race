using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ProductManager : MonoBehaviour, IBundleOwner
{
	private enum eState
	{
		IDLE,
		PRODUCTS,
		RECEIPT
	}

	private const string inAppPurchasesAsset = "InAppPurchases";

	private List<GTProduct> mConsumables = new List<GTProduct>();

	public int discount = 0;
	private DateTime discountStart;
	private DateTime discountEnd;
	private InAppPurchases inAppPurchases;
	private string postFix = string.Empty;
	private List<string> GTProduct = new List<string>();
	private List<string> d15GTProduct = new List<string>();
	private List<string> d30GTProduct = new List<string>();
	public static ProductManager Instance
	{
		get;
		private set;
	}

	public List<GTProduct> Consumables
	{
		get
		{
			return this.mConsumables;
		}
	}

	public GTProduct GetProductWithID(string zID)
	{
		return this.mConsumables.Find((GTProduct p) => String.Equals(zID, p.Code, StringComparison.CurrentCultureIgnoreCase));
	}

	private void Awake()
	{
		if (ProductManager.Instance != null)
		{
			return;
		}
		ProductManager.Instance = this;
	}

	public void Init()
	{
		this.LoadInAppPurchases();
	}

	private void LoadInAppPurchases()
	{
#if UNITY_EDITOR && !USE_ASSET_BUNDLE
        var inAppPurchases = AssetDatabase.LoadAssetAtPath<InAppPurchases>("Assets/configuration/InAppPurchases.asset");
        InitialiseProducts(inAppPurchases);
#else
        AssetProviderClient.Instance.RequestAsset(inAppPurchasesAsset.ToLower(), new BundleLoadedDelegate(this.InAppPurchasesReady), this);
#endif

	}

	private void ChangeGtProductItems()
	{
		int i = 0;
		foreach (var item in inAppPurchases.ABTestReadyGtProducts)
		{
			var count = item.Code.Split('_').Length;
			var prefixCode = item.Code.Split('_');
			var lastIndexOf = item.Code.LastIndexOf('_');
			int result = 0;
			var isNumber = int.TryParse(prefixCode[count - 1], out result);
			if(count <= 1)
				item.Code += postFix;
			else
			{
				if(isNumber)
					item.Code += postFix;
				else
				{
					item.Code = item.Code.Remove(item.Code.LastIndexOf('_')) + postFix;
				}
			}
			i++;
		}
	}
	
	private void ChangeD15GtProductItems()
	{
		int i = 0;
		foreach (var item in inAppPurchases.GTProducts_d15)
		{
			var count = item.Code.Split('_').Length;
			var prefixCode = item.Code.Split('_');
			var lastIndexOf = item.Code.LastIndexOf('_');
			int result = 0;
			var isNumber = int.TryParse(prefixCode[count - 1], out result);
			if(count <= 1)
				item.Code += postFix;
			else
			{
				if(isNumber)
					item.Code += postFix;
				else
				{
					item.Code = item.Code.Remove(item.Code.LastIndexOf('_')) + postFix;
				}
			}
			i++;
		}
	}
	
	private void ChangeD30GtProductItems()
	{
		int i = 0;
		foreach (var item in inAppPurchases.GTProducts_d30)
		{
			var count = item.Code.Split('_').Length;
			var prefixCode = item.Code.Split('_');
			var lastIndexOf = item.Code.LastIndexOf('_');
			int result = 0;
			var isNumber = int.TryParse(prefixCode[count - 1], out result);
			if(count <= 1)
				item.Code += postFix;
			else
			{
				if(isNumber)
					item.Code += postFix;
				else
				{
					item.Code = item.Code.Remove(item.Code.LastIndexOf('_')) + postFix;
				}
			}
			i++;
		}
	}

	private void GetBranch()
	{
		postFix = UserManager.Instance.currentAccount.BranchPostfix;
	}

	private void SyncIapConfigsItemKeyWithInAppPurchaseItem()
	{
		var purchasableItems = GameDatabase.Instance.IAPs.GetPurchasableItems();
		foreach (var offerData in purchasableItems)
		{
			var suffix = from product in inAppPurchases.ABTestReadyGtProducts
				where product.Code.Split('_')[0] == offerData.IAPCode.Split('_')[0]
				select product.Code;
			var id = suffix.AsEnumerable().Select(x => x).ToList();
			if (id.Count > 0)
			{
				offerData.IAPCode = id[0];
			}
		}
	}

	private void SyncBundleIaPsItemKeyWithInAppPurchaseItem()
	{
		SyncOneTimeOffers();
		SyncRepeatableOffers();
	}

	private void SyncOneTimeOffers()
	{
		var bundleOffers = GameDatabase.Instance.BundleOffers;
		var oneTimeOffers = bundleOffers.Configuration.OneTimeOffers;
		foreach (var offerData in oneTimeOffers)
		{
			var suffix = from product in inAppPurchases.ABTestReadyGtProducts
				where product.Code.Split('_')[0] == offerData.PopupData.BundleOfferItem.Split('_')[0]
				select product.Code;
			var id = suffix.AsEnumerable().Select(x => x).ToList();
			if (id.Count > 0)
			{
				offerData.PopupData.BundleOfferItem = id[0];
				for (int i = 0; i < offerData.PopupData._StarterPackItem1.Length; i++)
				{
					var item = offerData.PopupData._StarterPackItem1[i];
					var count = item.Split('_').Length;
					int result;
					var isNumber = int.TryParse(offerData.PopupData._StarterPackItem1[i].Split('_')[count - 1], out result);
					if (isNumber)
						offerData.PopupData._StarterPackItem1[i] += postFix;
					else
						offerData.PopupData._StarterPackItem1[i] = offerData.PopupData._StarterPackItem1[i]
							.Remove(offerData.PopupData._StarterPackItem1[i].LastIndexOf('_')) + postFix;
				}

				for (int i = 0; i < offerData.PopupData._StarterPackItem2.Length; i++)
				{
					var item = offerData.PopupData._StarterPackItem2[i];
					var count = item.Split('_').Length;
					int result;
					var isNumber = int.TryParse(offerData.PopupData._StarterPackItem2[i].Split('_')[count - 1], out result);
					if (isNumber)
						offerData.PopupData._StarterPackItem2[i] += postFix;
					else
						offerData.PopupData._StarterPackItem2[i] = offerData.PopupData._StarterPackItem2[i]
							.Remove(offerData.PopupData._StarterPackItem2[i].LastIndexOf('_')) + postFix;
				}
			}
		}
	}

	private void SyncRepeatableOffers()
	{
		var bundleOffers = GameDatabase.Instance.BundleOffers;
		var repeatableOffers = bundleOffers.Configuration.RepeatableOffers;
		foreach (var offerData in repeatableOffers)
		{
			var suffix = from product in inAppPurchases.ABTestReadyGtProducts
				where product.Code.Split('_')[0] == offerData.PopupData.BundleOfferItem.Split('_')[0]
				select product.Code;
			var id = suffix.AsEnumerable().Select(x => x).ToList();
			if (id.Count > 0)
			{
				offerData.PopupData.BundleOfferItem = id[0];
				for (int i = 0; i < offerData.PopupData._StarterPackItem1.Length; i++)
				{
					var item = offerData.PopupData._StarterPackItem1[i];
					var count = item.Split('_').Length;
					int result;
					var isNumber = int.TryParse(offerData.PopupData._StarterPackItem1[i].Split('_')[count - 1], out result);
					if (isNumber)
						offerData.PopupData._StarterPackItem1[i] += postFix;
					else
						offerData.PopupData._StarterPackItem1[i] = offerData.PopupData._StarterPackItem1[i]
							.Remove(offerData.PopupData._StarterPackItem1[i].LastIndexOf('_')) + postFix;
				}
				for (int i = 0; i < offerData.PopupData._StarterPackItem2.Length; i++)
				{
					var item = offerData.PopupData._StarterPackItem2[i];
					var count = item.Split('_').Length;
					int result;
					var isNumber = int.TryParse(offerData.PopupData._StarterPackItem2[i].Split('_')[count - 1], out result);
					if (isNumber)
						offerData.PopupData._StarterPackItem2[i] += postFix;
					else
						offerData.PopupData._StarterPackItem2[i] = offerData.PopupData._StarterPackItem2[i]
							.Remove(offerData.PopupData._StarterPackItem2[i].LastIndexOf('_')) + postFix;
				}
			}
		}
	}

	private void SyncOfferPackItemsKeyWithInAppPurchaseItem()
	{
		var offerPack = GameDatabase.Instance.OfferPacks.Configuration;
		foreach (var offerData in offerPack.Offers)
		{
			var suffix = from product in inAppPurchases.ABTestReadyGtProducts
				where product.Code.Split('_')[0] == offerData.ProductCode.Split('_')[0]
				select product.Code;
			var id = suffix.AsEnumerable().Select(x => x).ToList();
			if (id.Count > 0)
			{
				offerData.ProductCode = id[0];
			}
		}
	}
    public void InAppPurchasesReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		this.mConsumables.Clear();
	    var inAppPurchases = zAssetBundle.LoadAsset<InAppPurchases>(zAssetBundle.GetAllAssetNames()[0]);
        InitialiseProducts(inAppPurchases);
        AssetProviderClient.Instance.ReleaseAsset(inAppPurchasesAsset.ToLower(), zOwner);
    }


    public void InitialiseProducts(InAppPurchases inAppPurchases)
    {
	    this.inAppPurchases = inAppPurchases;
	    JsonDict parameters = new JsonDict();
	    var instanceCurrentAccount = UserManager.Instance.currentAccount;
	    if (instanceCurrentAccount.PossibleStoresForIAPABTest != null &&(instanceCurrentAccount.PossibleStoresForIAPABTest.Contains(Market.GetMarket()) && instanceCurrentAccount.BranchPostfix != null))
	    {
		    if(this.inAppPurchases == null) return;
		    GetBranch();
		    ChangeGtProductItems();
		    //ChangeD15GtProductItems();
		    //ChangeD30GtProductItems();
		    SyncIapConfigsItemKeyWithInAppPurchaseItem();
		    SyncBundleIaPsItemKeyWithInAppPurchaseItem();
		    SyncOfferPackItemsKeyWithInAppPurchaseItem(); 
	    }
	    WebRequestQueue.Instance.StartCall("rtw_get_discount", "get discount", parameters, OnDiscountCallback);
    }


    private void OnDiscountCallback(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
	    if (zstatus == 200) {
		    if (string.IsNullOrEmpty(zerror)) {
			    GTProduct[] uncommonProducts = inAppPurchases.ABTestReadyGtProducts.Where(
				    i => 
					    !i.CodeWithIdentifier.Contains("gold") &&
					    !i.CodeWithIdentifier.Contains("cash")
			    ).ToArray();
			    JsonDict json = new JsonDict();
			    
			    if (json.Read(zhttpcontent)) {
				    discount = int.Parse(json.GetString("discount_index"));
				    discountStart = ConvertUnixToDate(json.GetString("discount_start"));
				    discountEnd = ConvertUnixToDate(json.GetString("discount_end"));
				    if (GTDateTime.UtcNow > discountEnd || GTDateTime.UtcNow < discountStart) {
					    discount = 0;
				    }
				    
				    GTProduct[] productPool = inAppPurchases.ABTestReadyGtProducts;
				    if (discount == 1) {
					    productPool = inAppPurchases.GTProducts_d15;
					    productPool = productPool.Concat(uncommonProducts).ToArray();
				    } else if (discount == 2) {
					    productPool = inAppPurchases.GTProducts_d30;
					    productPool = productPool.Concat(uncommonProducts).ToArray();
				    }
	    
				    foreach (GTProduct current in productPool) {
					    this.mConsumables.Add(current);
				    }

				    AppStore.Instance.SetExpectedProducts(this.Consumables);
			    }
		    } else {
			    Debug.LogError(zerror);
		    }
	    }
    }
    
    private DateTime ConvertUnixToDate(string unix)
    {
	    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
	    dtDateTime = dtDateTime.AddSeconds(double.Parse(unix));
	    return dtDateTime;
    }

    private string ConvertDateToUnix(string dtDateTime)
    {
	    TimeSpan timeSpan = DateTime.Parse(dtDateTime) - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
	    return ((long)timeSpan.TotalSeconds).ToString();
    }
}
