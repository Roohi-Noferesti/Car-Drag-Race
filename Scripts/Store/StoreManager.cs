using System;
using System.Collections.Generic;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class StoreManager : IAppStoreManager
{
    private class PurchaseState
    {
        public string Item;
        public PurchaseResult.eResult Result;
        public string OrderID;
        public string Error;
    }

    private PurchaseState m_purchaseStateCache;
    private List<AppStoreProduct> m_appStoreProducts;
    private static string m_purchaseTransactionID;
    private static bool m_transactionResultVerified;
    public static event Action storeInited;
    public event Action OnReceivedProductDataResponse;

    public bool IAPEnabled
    {
        get; private set; }

    public bool UsingSimulator
    {
        get
        {
//#if UNITY_EDITOR
//            return true;
//#endif
            return false;
        }
    }
    public bool IsProcessingTransaction { get; private set; }
    public string CurrencyCode { get; private set; }

    public StoreManager()
    {
//        IAPEnabled = true;
//        storeAsset = new StoreAssets();
//        StoreEvents.OnSoomlaStoreInitialized += onStoreInited;
//        StoreEvents.OnMarketPurchase += onMarketPurchased;
//        StoreEvents.OnMarketPurchaseCancelled += onMarketPurchaseCancelled;
//        //StoreEvents.OnMarketItemsRefreshFinished += onMarketItemRefreshed;
//        StoreEvents.OnBillingNotSupported += OnBillingNotSupported;
//        StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactionFinished;
//        SoomlaStore.Initialize(storeAsset);
    }

    private void onRestoreTransactionFinished(bool obj)
    {
        GTDebug.Log(GTLogChannel.AppStore,"transaction restore result : "+obj);
        if (obj == false)
        {
            m_purchaseStateCache = new PurchaseState()
            {
                Item = null,
                Result = PurchaseResult.eResult.FAILED,
                OrderID = null
            };
        }
    }

    private void OnBillingNotSupported()
    {
        IAPEnabled = false;
    }

    //private void onMarketItemRefreshed(List<MarketItem> obj)
    //{
    //    m_appStoreProducts = new List<AppStoreProduct>();
    //    foreach (var marketItem in obj)
    //    {
    //        m_appStoreProducts.Add(new AppStoreProduct()
    //        {
    //            CurrencyCode = marketItem.MarketCurrencyCode,
    //            Description = marketItem.MarketDescription,
    //            Identifier = marketItem.ProductId,
    //            Price = marketItem.Price.ToString(),
    //            LocalisedPrice = GetLocalizedPrice(marketItem.ProductId),
    //            Title = marketItem.MarketTitle
    //        });
    //    }
    //    //var handler = MarketItemRefreshed;
    //    //if (handler != null) handler(obj);
    //    if (OnReceivedProductDataResponse != null)
    //    {
    //        OnReceivedProductDataResponse();
    //    }
    //}


    //public string GetLocalizedPrice(string productID)
    //{
    //    double price;
    //    int amount;
    //    string marketPrice;
    //    StoreManager.TryGetMarketItemPrice(productID, out price, out marketPrice, out amount);
    //    if (string.IsNullOrEmpty(marketPrice))
    //    {
    //        var price_format = "{0}";
    //        var price_amount = String.Format("ﻥﺎﻣﻮﺗ {0:n0}", price).ToNativeNumber();
    //        return String.Format(price_format, price_amount);
    //    }
    //    else
    //    {
    //        return marketPrice.ToCurrency();
    //    }
    //}

    public string GetLocalizedPrice(int price)
    {
        return string.Format("ﻥﺎﻣﻮﺗ {0:n0}", price);
    }

//    private void onMarketPurchaseCancelled(PurchasableVirtualItem obj)
//    {
//        if (m_transactionResultVerified)
//        {
//            return;
//        }
//        m_purchaseStateCache = new PurchaseState()
//        {
//            Item = obj,
//            Result = PurchaseResult.eResult.CANCELLED,
//            Error = "cancelled by user"
//        };
//        m_transactionResultVerified = true;
//
//        //This cause a lot of failed purchase , We are going to test this on next version
//        //I don't know exactly what happen , but it seems that is some cases , cancel purchase call back may called
//        //and then purchase completed callback called . thus m_purchaseTransactionID variable would be null
//        //and purchase turn to failed purchase
//        var handler = MarketItemPurchaseCancelled;
//        if (handler != null) handler(obj);
//    }

//    private void onMarketPurchased(PurchasableVirtualItem purchasableVirtualItem, string arg2, Dictionary<string, string> arg3)
//    {
//        var handler = MarketItemPurchased;
//        if (handler != null) handler(purchasableVirtualItem, arg2,arg3);
//
//        var orderID = arg3["orderId"];
//        //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
//        m_purchaseStateCache = new PurchaseState()
//        {
//            Item = purchasableVirtualItem,
//            Result = PurchaseResult.eResult.SUCCEEDED,
//            OrderID = orderID
//        };
//        m_transactionResultVerified = true;
//    }

    void onStoreInited()
    {
        var handler = storeInited;
        if (handler != null) handler();
    }

//    public static bool TryGetVirtualItemPrice(string itemID,out int price,out int amount)
//    {
//        price = -1;
//        amount = -1;
//        var item = StoreInfo.GetItemByItemId(itemID);
//        if (item is VirtualCurrencyPack)
//        {
//            var vcp = (item as VirtualCurrencyPack);
//            amount = vcp.CurrencyAmount;
//            price = (int) GetPurchasableItemPrice(vcp);
//            return true;
//        }
//
//        if (item is SingleUsePackVG)
//        {
//            var sup = item as SingleUsePackVG;
//            amount = sup.GoodAmount;
//            price = (int) GetPurchasableItemPrice(sup);
//            return true;
//        }
//
//        return false;
//    }

//    private static double GetPurchasableItemPrice(PurchasableVirtualItem purchasableVirtualItem)
//    {
//        if (purchasableVirtualItem.PurchaseType is PurchaseWithVirtualItem)
//        {
//            return (purchasableVirtualItem.PurchaseType as PurchaseWithVirtualItem).Amount;
//        }
//        if (purchasableVirtualItem.PurchaseType is PurchaseWithMarket)
//        {
//            return (purchasableVirtualItem.PurchaseType as PurchaseWithMarket).MarketItem.Price;
//        }
//        return 0;
//    }

//    public static bool TryGetMarketItemPrice(string itemID,out double price,out string marketPrice,out int amount)
//    {
//        price = -1;
//        amount = -1;
//        var item = StoreInfo.GetPurchasableItemWithProductId(itemID);
//        marketPrice = null;
//
//        if (item != null)
//        {
//            var marketItem = (PurchaseWithMarket) item.PurchaseType;
//            price = marketItem.MarketItem.Price;
//            marketPrice = marketItem.MarketItem.MarketPriceAndCurrency;
//
//            if (item is VirtualCurrencyPack)
//            {
//                amount = (item as VirtualCurrencyPack).CurrencyAmount;
//                return true;
//            }
//
//            if (item is SingleUsePackVG)
//            {
//                amount = (item as SingleUsePackVG).GoodAmount;
//                return true;
//            }
//        }
//
//        return false;
//    }

//    public static float GetItemPrice(string itemID)
//    {
//        int amount;
//        int price;
//        TryGetVirtualItemPrice(itemID, out amount, out price);
//        return price;
//    }

    public PurchaseResult GetPurchaseResult()
    {
        if (m_purchaseStateCache == null)
        {
            return null;
        }
        PurchaseResult purchaseResult = new PurchaseResult();
        purchaseResult.Result = m_purchaseStateCache.Result;
        purchaseResult.TransactionID = m_purchaseTransactionID;
        if (purchaseResult.Result == PurchaseResult.eResult.FAILED)
        {
            purchaseResult.ErrorDomain = m_purchaseStateCache.Error;
            //purchaseResult.ErrorCode = int.Parse(array[3]);
        }
        else
        {
            purchaseResult.Signature = null;//this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
            purchaseResult.Receipt = m_purchaseStateCache.OrderID;//this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
//            purchaseResult.ProductID = m_purchaseStateCache.Item.ID;
        }
        m_purchaseStateCache = null;
        IsProcessingTransaction = false;
        return purchaseResult;
    }

    public void ProcessExistingTransactions()
    {
        //throw new NotImplementedException();
    }

    public void ConsumePurchase(string productCode,string authority)
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        
    }

    public List<string> GetPurchases()
    {
        return null;
    }

    public void SetProducts(List<GTProduct> products)
    {
        //throw new NotImplementedException();
    }

    //    public void StartProductRequest()
    //    {
    //#if UNITY_EDITOR
    //        var list = storeAsset.GetCurrencyPacks().Where(i => i.PurchaseType is PurchaseWithMarket)
    //            .Select(i => (i.PurchaseType as PurchaseWithMarket).MarketItem).ToList();
    //        list.AddRange(storeAsset.GetGoods().Where(i => i.PurchaseType is PurchaseWithMarket)
    //            .Select(i => (i.PurchaseType as PurchaseWithMarket).MarketItem).ToList());
    //        onMarketItemRefreshed(list);
    //#else
    //        SoomlaStore.RefreshMarketItemsDetails();
    //#endif
    //    }

    public void StartProductRequest()
    {
        JsonDict parameters = new JsonDict();
        parameters.Set("username", "dummy_username");
        WebRequestQueue.Instance.StartCall("rtw_get_products", "get all iap products", parameters, OnGetProductsResponse);
    }

    private void OnGetProductsResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting weekly leaderboard : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(zhttpcontent))
        {
            GTDebug.LogError(GTLogChannel.RPBonus,
                "error getting weekly leaderboard : server send malformed json in response");
            return;
        }

        var products = parameters.GetObjectList<IapProduct>("products", GetIapProduct);

        m_appStoreProducts = new List<AppStoreProduct>();
        foreach (var marketItem in products)
        {
            m_appStoreProducts.Add(new AppStoreProduct()
            {
                CurrencyCode = "IRT",
                Description = marketItem.Name,
                Identifier = marketItem.ID,
                Price = marketItem.Value.ToString(),
                LocalisedPrice = GetLocalizedPrice(marketItem.Value),
                Title = marketItem.Name
            });
        }

        if (OnReceivedProductDataResponse != null)
        {
            OnReceivedProductDataResponse();
        }
    }

    private void GetIapProduct(JsonDict jsondict, ref IapProduct product)
    {
        product.ID = jsondict.GetString("id");
        product.Name = jsondict.GetString("name");
        product.Value = jsondict.GetInt("value");
        product.Available = jsondict.GetBool("avlb");
    }

    public List<AppStoreProduct> GetProducts()
    {
        Debug.LogError("yo 2");
        return m_appStoreProducts;
    }

    public void Purchase(string productCode)
    {
        IsProcessingTransaction = true;
        m_transactionResultVerified = false;
        m_purchaseTransactionID = Guid.NewGuid().ToString();
//        SoomlaStore.BuyMarketItem(productCode, m_purchaseTransactionID);
    }

    public void RestorePurchases()
    {
//        SoomlaStore.RestoreTransactions();
    }

    public void Dispose()
    {
    }
}
