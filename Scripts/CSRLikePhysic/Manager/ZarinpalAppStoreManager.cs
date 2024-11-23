using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
//using KingKodeStudio.IAB;
//using KingKodeStudio.IAB.Zarinpal;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class ZarinpalAppStoreManager : IAppStoreManager
{
    private string m_lastRefID;
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
    private string purchasingItemID;

    public event Action OnReceivedProductDataResponse;

    public bool IAPEnabled
    {
        get; private set;
    }

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

    public ZarinpalAppStoreManager()
    {
        IAPEnabled = true;
        //Zarinpal.StoreInitialized += OnStoreInited;
        //Zarinpal.StoreInitializeFailed += OnBillingNotSupported;
        //Zarinpal.PaymentVerificationSucceed += Zarinpal_PaymentVerificationSucceed;
        //Zarinpal.PurchaseFailed += Zarinpal_PurchaseFailed;
        //Zarinpal.Initialize();
    }

    private void Zarinpal_PurchaseCanceled()
    {
        m_purchaseStateCache = new PurchaseState()
        {
            Item = purchasingItemID,
            Result = PurchaseResult.eResult.CANCELLED,
            Error = "cancelled by user"
        };
        m_purchaseTransactionID = null;
    }
    
    private void Zarinpal_PurchaseFailed(string obj)
    {
        m_purchaseStateCache = new PurchaseState()
        {
            Item = purchasingItemID,
            Result = PurchaseResult.eResult.CANCELLED,
            Error = "cancelled by user"
        };
        m_purchaseTransactionID = null;
    }

    //private void Zarinpal_PaymentVerificationSucceed(Purchase purchase)
    //{
    //    //Prevent twice calculation of purchase
    //    if (m_lastRefID == purchase.OrderId)
    //    {
    //        return;
    //    }

    //    m_lastRefID = purchase.OrderId;
    //    var orderID = purchase.OrderId;
    //    //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
    //    m_purchaseStateCache = new PurchaseState()
    //    {
    //        Item = purchasingItemID,
    //        Result = PurchaseResult.eResult.SUCCEEDED,
    //        OrderID = orderID
    //    };
    //}

    private void OnBillingNotSupported(string obj)
    {
        IAPEnabled = false;
    }


    public string GetLocalizedPrice(int price)
    {
        return string.Format("ﻥﺎﻣﻮﺗ {0:n0}", price);
    }

    void OnStoreInited()
    {
        IAPEnabled = true;
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
            purchaseResult.Signature = null;//this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
            purchaseResult.Receipt = m_purchaseStateCache.OrderID;//this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
            purchaseResult.ProductID = m_purchaseStateCache.Item;
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
        return m_appStoreProducts;
    }

    public void Purchase(string productCode)
    {
        m_lastRefID = null;
        IsProcessingTransaction = true;
        m_purchaseTransactionID = Guid.NewGuid().ToString();
        var itemPrice = GetItemPrice(productCode);
        var title = m_appStoreProducts.FirstOrDefault(p => p.Identifier == productCode).Title;
        var count = title.Split('_').Length;
        var codeElements = title.Split('_');
        int result = 0;
        var isNumber = int.TryParse(codeElements[count-1], out result);
        if(count > 1 && !isNumber)
        {
            title = title.Remove(title.LastIndexOf('_'));
        }
        var desc = string.Format(LocalizationManager.GetTranslation("TEXT_ZARINPAL_ITEM_DESC"), title);
        purchasingItemID = productCode;
        //Zarinpal.Purchase(itemPrice, desc, title);
    }

    private int GetItemPrice(string productCode)
    {
        int price = 0;
        var itemPrice = m_appStoreProducts.FirstOrDefault(p => p.Identifier == productCode).Price;
        int.TryParse(itemPrice, out price);
        return price;
    }

    public void RestorePurchases()
    {

    }

    public void Dispose()
    {
    }
}
