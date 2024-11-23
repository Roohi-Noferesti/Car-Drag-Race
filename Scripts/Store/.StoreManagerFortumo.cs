using KingKodeStudio.IAB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class StoreManagerFortumo : IAppStoreManager
{
    public bool HasResult = false;
    public bool SuccessFulResult = false;

    private const string BASE_FORTUMO_URL = "https://pay.fortumo.com/mobile_payments/";
    private class PurchaseState
    {
        public string Item;
        public PurchaseResult.eResult Result;
        public string OrderID;
        public string Error;
        public string PurchaseToken;
        public double Price;
    }

    private List<AppStoreProduct> m_appStoreProducts;



    private AppStoreProduct m_chosenAppStoreProduct; // its the app store productthat user chose to but on fortumo
                                                     //  private PurchaseState m_purchaseStateCache;




    public bool IAPEnabled
    {
        get { return true; }
    }

    public bool UsingSimulator { get; set; }
    public bool IsProcessingTransaction { get; set; }
    public string CurrencyCode { get; set; }
    public event Action OnReceivedProductDataResponse;



    public StoreManagerFortumo()
    {
        Initialize();
        StartProductRequest();
    }



    public void ConsumePurchase(string productCode, string authority)
    {

    }

    public List<AppStoreProduct> GetProducts()
    {
        return m_appStoreProducts;
    }

    public PurchaseResult GetPurchaseResult()
    {
        if (!HasResult) return null;
        PurchaseResult result = new PurchaseResult();
        UnityEngine.Debug.Log("fortumo successfull result is:::" + SuccessFulResult);
        if (SuccessFulResult) result.TransactionID = Guid.NewGuid().ToString();
        else result.TransactionID = null;
        double parseValue = 0;
        var parseResult = double.TryParse(m_chosenAppStoreProduct.Price, out parseValue);
        result.Result = PurchaseResult.eResult.SUCCEEDED;
        result.Receipt = Guid.NewGuid().ToString();
        result.Price = parseValue;
        result.CurrencyCode = m_chosenAppStoreProduct.CurrencyCode;
        result.Market = "Fortumo";
        result.ProductID = m_chosenAppStoreProduct.Identifier;
        // result.Signature= m_chosenAppStoreProduct.  i cant get it
        HasResult = false;
        return result;
    }

    public void Initialize()
    {
        Debug.Log("Initializing");
        if (FortumoAndroiEvents.Instance == null)
        {
            GameObject fortumoEventHandler = new GameObject("fortumo");
            fortumoEventHandler.AddComponent<FortumoAndroiEvents>().fortumoReference = this;
            Debug.Log("FortumoAndroiEvents instantiated here");
        }
        else
        {
            FortumoAndroiEvents.Instance.fortumoReference = this;
        }
    }

    public void ProcessExistingTransactions()
    {

    }

    public void Purchase(string productCode)
    {
        SuccessFulResult = false;
        //  var itemPrice = GetItemPrice(productCode);
        var item = m_appStoreProducts.FirstOrDefault(p => p.Identifier == productCode);
        m_chosenAppStoreProduct = item;
        //  var desc = string.Format(ScriptLocalization.Get("TEXT_ZARINPAL_ITEM_DESC"), title);
        IsProcessingTransaction = true;

        string finalUrl = BASE_FORTUMO_URL + item.FortumoServiceID + "?cuid=" + UserManager.Instance.currentAccount.UserID;

        FortumoAndroiEvents.Instance.ShouldAnswer = true;
        InitFortumoActivity();
        Application.OpenURL(finalUrl);


        //   m_transactionResultVerified = false;
        //   m_purchaseTransactionID = Guid.NewGuid().ToString();
        // KingIAB.Purchase(productCode.ToLower(), itemPrice, desc);
        //KingIAB.Purchase(productCode,100,desc);//for test purpose
    }

    public void RestorePurchases()
    {

    }

    public void SetProducts(List<GTProduct> products)
    {

    }

    public void StartProductRequest()
    {
        if (m_appStoreProducts == null || m_appStoreProducts.Count <= 0)
        {

            JsonDict parameters = new JsonDict();
            parameters.Set("username", "dummy_username");
            //  parameters.Set("currency",LocalizedCurrencies.UserCurrency);
            parameters.Set("currency", LocalizedCurrencies.UserCurrencyCode);
            WebRequestQueue.Instance.StartCall("rtw_get_products_fortumo", "get all iap products", parameters,
                OnGetProductsResponse);
        }
    }

    private void OnGetProductsResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting fortumo products : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(zhttpcontent))
        {
            GTDebug.LogError(GTLogChannel.RPBonus,
                "error getting fortumo products : server send malformed json in response");
            return;
        }

        var products = parameters.GetObjectList<IapProduct>("products", GetIapProduct);

        m_appStoreProducts = new List<AppStoreProduct>();
        foreach (var marketItem in products)
        {
            m_appStoreProducts.Add(new AppStoreProduct()
            {
                CurrencyCode = LocalizedCurrencies.UserCurrencyCode,
                //  CurrencyCode = "IQD",
                Description = GetProductLocalizedTitle(marketItem.ID),
                Identifier = marketItem.ID,
                Price = marketItem.Value.ToString(),
                LocalisedPrice = GetLocalizedPrice(marketItem.Value),
                Title = GetProductLocalizedTitle(marketItem.ID),
                FortumoServiceID = marketItem.FortumoServiceID
            });


        }

        if (OnReceivedProductDataResponse != null)
        {
            OnReceivedProductDataResponse();
        }

        //  CheckAnyPendingPurchases();
    }


    private string GetProductLocalizedTitle(string productID)
    {
        return ScriptLocalization.Get("TEXT_PRODUCT_" + productID);
    }

    private void GetIapProduct(JsonDict jsondict, ref IapProduct product)
    {
        product.ID = jsondict.GetString("id");
        product.Name = jsondict.GetString("name");
        product.Value = jsondict.GetInt("value");
        product.Available = jsondict.GetBool("avlb");
        product.FortumoServiceID = jsondict.GetString("fls");
    }
    public string GetLocalizedPrice(int price)
    {
        return string.Format(LocalizedCurrencies.UserCurrencyFormat, price, LocalizedCurrencies.UserCurrencySymbol);
    }

    void InitFortumoActivity()
    {
        //StartPackage("com.kingkodestudio.z2h");
    }


    void StartPackage(string package)
    {
        AndroidJavaClass activityClass;
        AndroidJavaObject activity, packageManager;
        AndroidJavaObject launch;

        activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
        launch = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", package);
        activity.Call("startActivity", launch);

    }


    public void Dispose()
    {

    }
}
