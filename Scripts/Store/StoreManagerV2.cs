using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using I2.Loc;
//using KingKodeStudio.IAB;
//using KingKodeStudio.IAB.Zarinpal;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class StoreManagerV2 : IAppStoreManager
{
    private class PurchaseState
    {
        public string Item;
        public PurchaseResult.eResult Result;
        public string OrderID;
        public string Error;
        public string PurchaseToken;
        public double Price;
    }

    //private List<Purchase> m_pendingPurchases;
    private PurchaseState m_purchaseStateCache;
    private List<AppStoreProduct> m_appStoreProducts;
    private bool _insideMyCountry = true;
    private static string m_purchaseTransactionID;
    private static bool m_transactionResultVerified;
    private List<string> m_skusFetched = new List<string>();
    private string[] _gtProductsIDs;
    private int m_retryCount;
    private const int MAX_QUERY_SKU_RETRY = 10;

    public event Action OnReceivedProductDataResponse;

    public bool IAPEnabled
    {
        get; private set; }

    public bool UsingSimulator
    {
        get
        {
            return false;
        }
    }
    public bool IsProcessingTransaction { get; private set; }
    public string CurrencyCode { get; private set; }

    public string[] GtProductsIDs
    {
        get
        {
            return _gtProductsIDs;
        }
    }

    public StoreManagerV2()
    {
        //StoreEvents.OnMarketItemsRefreshFinished += onMarketItemRefreshed;
        //KingIAB.StoreInitialized += BillingSupportedEvent;
        //KingIAB.PurchaseSucceed += purchaseSucceededEvent;
        //KingIAB.StoreInitializeFailed += BillingNotSupportedEvent;
        //KingIAB.PurchaseFailed += PurchaseFailedEvent;
        //KingIAB.QueryPurchasesSucceeded += QueryPurchasesSucceeded;
        //KingIAB.ConsumeSucceed += ConsumeSucceeded;
        //KingIAB.PurchaseStarted += PurchaseStarted;
        //KingIAB.QuerySkuDetailsFailed += QuerySkuDetailsFailed;
        //KingIAB.QuerySkuDetailsSucceeded += QuerySkuDetailsSucceeded;
        //Zarinpal.SetQueryProvider(new GTQueryProvider());
        Initialize();
    }

    public void Dispose()
    {
        //KingIAB.StoreInitialized -= BillingSupportedEvent;
        //KingIAB.PurchaseSucceed -= purchaseSucceededEvent;
        //KingIAB.StoreInitializeFailed -= BillingNotSupportedEvent;
        //KingIAB.PurchaseFailed -= PurchaseFailedEvent;
        //KingIAB.QueryPurchasesSucceeded -= QueryPurchasesSucceeded;
        //KingIAB.ConsumeSucceed -= ConsumeSucceeded;
        //KingIAB.PurchaseStarted -= PurchaseStarted;
        //KingIAB.QuerySkuDetailsFailed -= QuerySkuDetailsFailed;
        //KingIAB.QuerySkuDetailsSucceeded -= QuerySkuDetailsSucceeded;
        //KingIAB.Close();
    }

    private void QuerySkuDetailsFailed(string error)
    {
        GTDebug.LogError(GTLogChannel.AppStore,"Sku detail faileds : " + error);

        //if (KingIAB.BillingPlatformName == BillingPlatformName.GooglePlay)
        //{
        //    if (m_retryCount >= MAX_QUERY_SKU_RETRY || m_skusFetched.Count >0)
        //    {
        //        GTDebug.Log(GTLogChannel.AppStore, "some products received , due to consuctive error on getting product , OnReceivedProductDataResponse will be called");
        //        if (OnReceivedProductDataResponse != null)
        //        {
        //            OnReceivedProductDataResponse();
        //        }
        //    }
        //}
    }

    public void Initialize()
    {
//        if (KingIAB.Initialized)
//            return;

//#if UNITY_IOS
//        _insideMyCountry = true;
//#else
//        if (KingIAB.Setting.IsGooglePlay)
//        {
//            _insideMyCountry = BasePlatform.ActivePlatform.InsideCountry;

//            GTDebug.Log(GTLogChannel.AppStore, "country_code is : " + _insideMyCountry + " , inside iran : " + _insideMyCountry);
//        }
//        else
//        {
//            //don't force zarinpal because we set store explicitly ( zarinpal or bazaar or myket or iranapss )
//            _insideMyCountry = false;
//        }
//#endif

//        KingIAB.Initialize(_insideMyCountry);
    }

    //private void QuerySkuDetailsSucceeded(List<SkuInfo> skuinfos)
    //{
    //    GTDebug.Log(GTLogChannel.AppStore,"Sku detail succeeded : " + (skuinfos != null ? skuinfos.Count.ToString() : "null"));
    //    if (m_appStoreProducts == null)
    //        m_appStoreProducts = new List<AppStoreProduct>();
    //    foreach (var marketItem in skuinfos)
    //    {
    //        if (m_appStoreProducts.All(i => i.Identifier != marketItem.ProductId))
    //        {
    //            var appStoreProduct = new AppStoreProduct()
    //            {
    //                CurrencyCode = marketItem.PriceCurrencyCode,
    //                Description = marketItem.Description,
    //                Identifier = marketItem.ProductId,
    //                Price = marketItem.Price,
    //                LocalisedPrice = marketItem.Price, //GetLocalizedPrice(marketItem.Price),
    //                Title = marketItem.Title
    //            };
    //            var currencyInfo = CurrencyUtils.ParseCurrencyString(marketItem.Price);
    //            appStoreProduct.Price = currencyInfo.currencyValue.ToString(CultureInfo.InvariantCulture);
    //            m_appStoreProducts.Add(appStoreProduct);


    //            if (string.IsNullOrEmpty(CurrencyCode))
    //            {
    //                CurrencyCode = marketItem.PriceCurrencyCode;
    //            }
    //        }

    //        if (!m_skusFetched.Contains(marketItem.ProductId))
    //        {
    //            m_skusFetched.Add(marketItem.ProductId);
    //        }
    //    }

    //    //if all skus details fetch , then invoke OnReceivedProductDataResponse otherwise try to fetch remaining
    //    //Note : we can not query more than 20 skus in one call because of google limitations.So we have to split
    //    //our query into several steps
    //    if (m_retryCount >= MAX_QUERY_SKU_RETRY || GtProductsIDs.Length == m_skusFetched.Count)
    //    {
    //        GTDebug.Log(GTLogChannel.AppStore, "All products received ...");
    //        if (OnReceivedProductDataResponse != null)
    //        {
    //            OnReceivedProductDataResponse();
    //        }
        
    //        CheckAnyPendingPurchases();
    //    }
    //    else
    //    {
    //        GTDebug.Log(GTLogChannel.AppStore, "Continue getting products ...");
    //        StartProductRequest();
    //    }


    //}

    //private void PurchaseStarted(string productCode,string authority)
    //{
    //    var zarinpal = KingIAB.BillingPlatform as Zarinpal;
    //    if (zarinpal!=null)
    //    {
    //        JsonDict parameters = new JsonDict();
    //        parameters.Set("username", UserManager.Instance.currentAccount.Username);
    //        parameters.Set("product_code", productCode);
    //        parameters.Set("authority", authority);
    //        parameters.Set("app_store", BasePlatform.ActivePlatform.GetTargetAppStore().ToString());
    //        WebRequestQueue.Instance.StartCall("acc_zarinpal_purchase_start", "start zarinpal purchase", parameters, ZarinpalPurchaseStartResponse,
    //            null, ProduceHashSource(parameters));//moeen
    //    }
    //}
    
    private string ProduceHashSource(JsonDict dict)
    {
        string text = string.Empty;
        foreach (string current in dict.Keys)
        {
            text += dict.GetString(current);
        }
        return text;
    }

    private void ZarinpalPurchaseStartResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.Account, "error zarinpal purchase started : " + zerror);
            return;
        }

//        JsonDict jsonDict = new JsonDict();
//        if (jsonDict.Read(content))
//        {
//            var authority = jsonDict.GetString("authority");
//#if UNITY_ANDROID
//                ((KingIAB.BillingPlatform as Zarinpal).Platform as ZarinpalAndroid).StartPurchaseActivity();
//#elif UNITY_IOS
//                ((KingIAB.BillingPlatform as Zarinpal).Platform as ZarinpaliOS).StartPurchase(authority);
//#endif
//            GTDebug.Log(GTLogChannel.Account, "zarinpal purchase started response with authority : " + authority);
//        }
    }

    //private void ConsumeSucceeded(Purchase purchase)
    //{
    //    if (KingIAB.BillingPlatform is Zarinpal)
    //    {
    //        JsonDict parameters = new JsonDict();
    //        parameters.Set("authority", purchase.Signature);
    //        parameters.Set("refid", purchase.OrderId);

    //        GTDebug.Log(GTLogChannel.Account,"consuming product with order id : "+purchase.OrderId);
    //        WebRequestQueue.Instance.StartCall("acc_zarinpal_consume_purchase", "consume zarinpal purchase", parameters, ZarinpalConsumePurchaseResponse,
    //            null, ProduceHashSource(parameters));
    //    }
    //    else
    //    {
    //        IsProcessingTransaction = false;
    //    }
    //}
    
    
    private void ZarinpalConsumePurchaseResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.Account, "error consume zarinpal purchase : " + zerror);
            return;
        }

        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(content))
        {
            var authority = jsonDict.GetString("authority");
            IsProcessingTransaction = false;
            GTDebug.Log(GTLogChannel.Account, "zarinpal consume purchase succeed for authority : " + authority);
        }
    }

    //private void QueryPurchasesSucceeded(List<Purchase> purchases)
    //{
    //    m_pendingPurchases = purchases;
    //}

    private void CheckAnyPendingPurchases()
    {
        //if (m_pendingPurchases != null && m_pendingPurchases.Count > 0)
        //{
        //    if (m_appStoreProducts != null && m_appStoreProducts.Count > 0)
        //    {
        //        var product = m_appStoreProducts.FirstOrDefault(p => p.Identifier == m_pendingPurchases[0].ProductId);
        //        if (product == null)
        //        {
        //            Debug.Log("Consuming purchase : "+m_pendingPurchases[0].ProductId+","+m_pendingPurchases[0].Signature);
        //            ConsumePurchase(m_pendingPurchases[0].ProductId, m_pendingPurchases[0].Signature);
        //        }
        //        else
        //        {
        //            var orderID = m_pendingPurchases[0].OrderId;
        //            //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
        //            m_purchaseStateCache = new PurchaseState()
        //            {
        //                Item = m_pendingPurchases[0].ProductId,
        //                Result = PurchaseResult.eResult.SUCCEEDED,
        //                OrderID = orderID,
        //                PurchaseToken = m_pendingPurchases[0].PurchaseToken
        //            };
        //            if (m_appStoreProducts != null)
        //            {
        //                    double p = 0;
        //                    if (double.TryParse(product.Price, out p))
        //                    {
        //                        m_purchaseStateCache.Price = p;
        //                    }
        //            }
        //            m_purchaseTransactionID = Guid.NewGuid().ToString();
        //            m_transactionResultVerified = true;
        //        }
        //    }
        //}
    }

    private void PurchaseFailedEvent(string obj)
    {


        if (m_transactionResultVerified)
        {
            return;
        }
        m_purchaseStateCache = new PurchaseState()
        {
            Item = obj,
            Result = PurchaseResult.eResult.CANCELLED,
            Error = "cancelled by user"
        };
        m_transactionResultVerified = true;

        //This cause a lot of failed purchase , We are going to test this on next version
        //I don't know exactly what happen , but it seems that is some cases , cancel purchase call back may called
        //and then purchase completed callback called . thus m_purchaseTransactionID variable would be null
        //and purchase turn to failed purchase
    }

    private void BillingNotSupportedEvent(string obj)
    {
        IAPEnabled = false;
    }

    private void BillingSupportedEvent()
    {
        IAPEnabled = true;
        
        //KingIAB.QueryPurchases();
    }

    //private void purchaseSucceededEvent(Purchase obj)
    //{
    //    var orderID = obj.OrderId;
    //    //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
    //    m_purchaseStateCache = new PurchaseState()
    //    {
    //        Item = obj.ProductId,
    //        Result = PurchaseResult.eResult.SUCCEEDED,
    //        OrderID = orderID,
    //        PurchaseToken = obj.PurchaseToken,
    //    };
    //    if (m_appStoreProducts != null)
    //    {
    //        var product = m_appStoreProducts.FirstOrDefault(p => p.Identifier == obj.ProductId);
    //        if (product != null)
    //        {
    //            var currencyInfo = CurrencyUtils.ParseCurrencyString(product.Price);
    //            if (currencyInfo != null)
    //            {
    //                m_purchaseStateCache.Price = currencyInfo.currencyValue;
    //            }
    //        }
    //    }

    //    m_transactionResultVerified = true;
    //}

    private void onRestoreTransactionFinished(bool obj)
    {
        //Debug.Log("transaction restore result : "+obj);
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

    public string GetLocalizedPrice(int price)
    {
        return string.Format("تومان {0:n0}", price);
    }

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
            purchaseResult.Signature = m_purchaseStateCache.PurchaseToken;//this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
            purchaseResult.Receipt = m_purchaseStateCache.OrderID;//this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
            purchaseResult.ProductID = m_purchaseStateCache.Item;
            purchaseResult.Market = GetMarket();
            purchaseResult.CurrencyCode = CurrencyCode;
            purchaseResult.Price = m_purchaseStateCache.Price;
        }
        m_purchaseStateCache = null;
        //We set this to true until purchase consumed
        if (purchaseResult.Result == PurchaseResult.eResult.SUCCEEDED)
        {
//#if UNITY_EDITOR
            IsProcessingTransaction = false;
//#else
//            IsProcessingTransaction = true;
//#endif
        }
        else
        {
            IsProcessingTransaction = false;
        }

        return purchaseResult;
    }

    private string GetMarket()
    {
        //return KingIAB.BillingPlatformName.ToString();
        return null;
    }

    public void ProcessExistingTransactions()
    {
        //throw new NotImplementedException();
    }

    public void ConsumePurchase(string productCode,string authority)
    {
        //var itemPrice = GetItemPrice(productCode);
        //Debug.Log("consuming product : " + productCode + " with amount : " + itemPrice);
        //KingIAB.Consume(productCode.ToLower(),authority,itemPrice);
        ////KingIAB.Consume(productCode, authority,100);//for test purpose
    }

    public void SetProducts(List<GTProduct> products)
    {
        //set expected product ids that client should fetch from store
        _gtProductsIDs = products == null ? new string[0] : products.Select(p => p.Code).ToArray();
        //throw new NotImplementedException();
    }

    public void StartProductRequest()
    {
        //if (KingIAB.BillingPlatformName == BillingPlatformName.GooglePlay)
        //{
        //    m_retryCount++;
        //    if (m_retryCount >=MAX_QUERY_SKU_RETRY || m_skusFetched.Count == GtProductsIDs.Length)
        //    {
        //        GTDebug.Log(GTLogChannel.AppStore,
        //            m_skusFetched.Count + " products (all) is received from store.Not not to query skus again");
        //        if (OnReceivedProductDataResponse != null)
        //        {
        //            OnReceivedProductDataResponse();
        //        }

        //        CheckAnyPendingPurchases();
        //    }
        //    else
        //    {
        //        var list = new List<string>();
        //        foreach (var product in GtProductsIDs)
        //        {
        //            if (!m_skusFetched.Contains(product) && list.Count < 20)
        //            {
        //                list.Add(product);
        //            }
        //        }
        //        if (list.Count != 0)
        //        {
        //            GTDebug.Log(GTLogChannel.AppStore, "Query sku details for " + list.Count + " items");
        //            KingIAB.QuerySkuDetails(list.ToArray());
        //        }
        //        else
        //        {
        //            GTDebug.Log(GTLogChannel.AppStore, "Can not query empty sku list .Stop here");
        //        }
        //    }
        //}
        //else
        //{
        //    JsonDict parameters = new JsonDict();
        //    parameters.Set("username", "dummy_username");
        //    JsonDict productJson = new JsonDict();
        //    productJson.Set("expected_products", _gtProductsIDs);
        //    parameters.Set("expected_products_json", productJson.ToString());
        //    WebRequestQueue.Instance.StartCall("rtw_get_products", "get all iap products", parameters,
        //        OnGetProductsResponse);
        //}
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
            var price = marketItem.Value.ToString();
            var localisedPrice = GetLocalizedPrice(marketItem.Value);
#if UNITY_EDITOR
            var product =
                GameDatabase.Instance.RevenueTrackingConfiguration.Prices.FirstOrDefault(r =>
                    r.ProductID == marketItem.ID);
            if (product != null)
            {
                price = string.Format("{0:C}$", product.CADPrice);
                localisedPrice = price;
            }
#endif

            m_appStoreProducts.Add(new AppStoreProduct()
            {
                CurrencyCode = "IRT",
                Description = GetProductLocalizedTitle(marketItem.ID),
                Identifier = marketItem.ID,
                Price = price,
                LocalisedPrice = localisedPrice,
                Title = GetProductLocalizedTitle(marketItem.ID),
            });
            
            if (string.IsNullOrEmpty(CurrencyCode))
            {
                CurrencyCode = "IRT";
            }
        }

        if (OnReceivedProductDataResponse != null)
        {
            OnReceivedProductDataResponse();
        }
        
        CheckAnyPendingPurchases();
    }


    private string GetProductLocalizedTitle(string productID)
    {
        return LocalizationManager.GetTranslation("TEXT_PRODUCT_" + productID);
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
        Debug.LogError("yo 3");
        return m_appStoreProducts;
    }

    public void Purchase(string productCode)
    {
        var itemPrice = GetItemPrice(productCode);
        var title = m_appStoreProducts.FirstOrDefault(p => p.Identifier == productCode).Title;
        var desc = string.Format(LocalizationManager.GetTranslation("TEXT_ZARINPAL_ITEM_DESC"), title);
        IsProcessingTransaction = true;
        m_transactionResultVerified = false;
        m_purchaseTransactionID = Guid.NewGuid().ToString();
        //KingIAB.Purchase(productCode.ToLower(),itemPrice, desc);
        //KingIAB.Purchase(productCode,100,desc);//for test purpose

    }
    
    private int GetItemPrice(string productCode)
    {
        int price = 0;
        var product = m_appStoreProducts.FirstOrDefault(p => p.Identifier == productCode);
        if (product != null)
        {
            var itemPrice = product.Price;
            int.TryParse(itemPrice, out price);
            return price;
        }

        return 0;
    }

    public void RestorePurchases()
    {
        //SoomlaStore.RestoreTransactions();
    }

    public static bool TryGetMarketItemPrice(string itemID,out double price,out string marketPrice)
    {
        price = 0;
        marketPrice = "";

        return true;
    }
}
