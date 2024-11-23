using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using KingKodeStudio.IAB;
using KingKodeStudio.IAB.Zarinpal;
using Metrics;
using UnityEngine;
using UnityEngine.Purchasing;

public class UnityIAPStoreManager : IAppStoreManager, IStoreListener
{
    private class PurchaseState
    {
        public string Item;
        public PurchaseResult.eResult Result;
        public string OrderID;
        public string Error;
        public string PurchaseToken;
        public double Price;
        public PurchaseResult.PurchaseReport Report;
    }

    private PurchaseState m_purchaseStateCache;
    private bool _insideMyCountry = true;
    private static IStoreController m_StoreController; // The Unity Purchasing system.
    IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    //List of products that we want to fetch from store
    private List<GTProduct> _expextedProducts;

    //List of products that is fetched from store
    private List<AppStoreProduct> _appStoreProducts;
    private bool m_transactionResultVerified;
    private string m_purchaseTransactionID;

    public void Dispose()
    {
    }

    public event Action OnReceivedProductDataResponse;
    public bool IAPEnabled { get; private set; } = true;

    public bool UsingSimulator
    {
        get { return false; }
    }

    public bool IsProcessingTransaction { get; private set; }
    public string CurrencyCode { get; private set; }


    public UnityIAPStoreManager()
    {
        Initialize();
    }

    public void SetProducts(List<GTProduct> products)
    {
        _expextedProducts = products;
    }

    public void StartProductRequest()
    {
        if (_expextedProducts != null && m_StoreController != null)
        {
            HashSet<ProductDefinition> productDefinitions = new HashSet<ProductDefinition>();
            foreach (var expextedProduct in _expextedProducts)
            {
                productDefinitions.Add(new ProductDefinition(expextedProduct.Code, ProductType.Consumable));
            }

            m_StoreController.FetchAdditionalProducts(productDefinitions, () =>
                {
                    if (OnReceivedProductDataResponse != null)
                    {
                        OnReceivedProductDataResponse();
                    }
                },
                error => { IAPEnabled = false; });
        }
    }


    public List<AppStoreProduct> GetProducts()
    {
        if (_expextedProducts == null)
            return null;

        var anyProductNull = _appStoreProducts == null || _appStoreProducts.Any(p => string.IsNullOrEmpty(p.Price));
        if (anyProductNull || _appStoreProducts.Count < _expextedProducts.Count)
        {
            if (m_StoreController != null)
            {
                _appStoreProducts = new List<AppStoreProduct>();
                foreach (var product in m_StoreController.products.all)
                {
                    _appStoreProducts.Add(new AppStoreProduct()
                    {
                        Identifier = product.definition.id,
                        Description = product.metadata.localizedDescription,
                        LocalisedPrice = product.metadata.localizedPriceString,
                        Price = product.metadata.localizedPriceString,
                        CurrencyCode = product.metadata.isoCurrencyCode,
                        Title = product.metadata.localizedTitle,
                    });
                }
            }
        }

        return _appStoreProducts;
    }

    public void Purchase(string productCode)
    {
        IsProcessingTransaction = true;
        SetNewTransactionID();
        m_transactionResultVerified = false;
        var payload = string.Empty;

        if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
        {
            payload = UserManager.Instance.currentAccount.UserID.ToString();
        }
        m_StoreController.InitiatePurchase(productCode, payload);
    }

    public void RestorePurchases()
    {
        //throw new NotImplementedException();
    }

    public PurchaseResult GetPurchaseResult()
    {
        if (m_purchaseStateCache == null)
        {
            return null;
        }

        PurchaseResult purchaseResult = new PurchaseResult();
        purchaseResult.Report = m_purchaseStateCache.Report;
        purchaseResult.Result = m_purchaseStateCache.Result;
        purchaseResult.TransactionID = m_purchaseTransactionID;
        if (purchaseResult.Result == PurchaseResult.eResult.FAILED)
        {
            purchaseResult.ErrorDomain = m_purchaseStateCache.Error;
            //purchaseResult.ErrorCode = int.Parse(array[3]);
        }
        else
        {
            purchaseResult.Signature =
                m_purchaseStateCache.OrderID; //this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
            purchaseResult.Receipt =
                m_purchaseStateCache
                    .OrderID; //this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
            purchaseResult.ProductID = m_purchaseStateCache.Item;
            purchaseResult.Market = Market.GetMarket();
            purchaseResult.CurrencyCode = CurrencyCode;
            purchaseResult.Price = m_purchaseStateCache.Price;
        }

        m_purchaseStateCache = null;
        m_purchaseTransactionID = null;
        //We set this to true until purchase consumed
        if (purchaseResult.Result == PurchaseResult.eResult.SUCCEEDED)
        {
            //#if UNITY_EDITOR
            IsProcessingTransaction = false;
            //#else
            //            IsProcessingTransaction = true;//TODO
            //#endif
        }
        else
        {
            IsProcessingTransaction = false;
        }

        return purchaseResult;
    }

    public void ProcessExistingTransactions()
    {
    }

    public void ConsumePurchase(string productCode, string authority)
    {
        var product = m_StoreController.products.WithID(productCode);
        if (product.hasReceipt)
        {
            m_StoreController.ConfirmPendingPurchase(product);
        }
    }

    public void Initialize()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    private void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        if (PurchasingModuleSelection.Config.UseUnityIAPSetting)
        {
            _insideMyCountry = BasePlatform.ActivePlatform.InsideCountry;

            GTDebug.Log(GTLogChannel.AppStore, "country_code is : " + _insideMyCountry);
        }
        else
        {
            //don't force zarinpal because we set store explicitly ( zarinpal or bazaar or myket )
            _insideMyCountry = false;
        }

        var builder =
            ConfigurationBuilder.Instance(PurchasingModuleSelection.GetPurchasingModule(_insideMyCountry));

        try
        {
            builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(OnDeferredPurchase);
        }
        catch { }

        //Add Product Definition here
        builder.AddProduct("gold_pack_1", ProductType.Consumable);

        ZarinpalStore.SetQueryProvider(new GTQueryProvider());
        BazaarStore.SetQueryProvider(new GTQueryProvider());
        MyketStore.SetQueryProvider(new GTQueryProvider());
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    
    void OnDeferredPurchase(Product product)
    {
        Debug.Log($"Purchase of {product.definition.id} is deferred");
        m_purchaseStateCache = new PurchaseState()
        {
            Item = product.definition.id,
            Result = PurchaseResult.eResult.DEFERRED,
            OrderID = "DEFERRED",
            PurchaseToken = "DEFERRED",
            Report = new PurchaseResult.PurchaseReport()
        };
        //UpdateUI();
    }
    
    bool IsPurchasedProductDeferred(string productId)
    {
        var product = m_StoreController.products.WithID(productId);
        return m_GooglePlayStoreExtensions.IsPurchasedProductDeferred(product);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (m_GooglePlayStoreExtensions !=null && m_GooglePlayStoreExtensions.IsPurchasedProductDeferred(purchaseEvent.purchasedProduct))
        {
            //The purchase is Deferred.
            //Therefore, we do not unlock the content or complete the transaction.
            //ProcessPurchase will be called again once the purchase is Purchased.
            return PurchaseProcessingResult.Pending;
        }
        
        SetNewTransactionID();
        if (string.IsNullOrEmpty(CurrencyCode))
        {
            CurrencyCode = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode;
        }
        var orderID = purchaseEvent.purchasedProduct.transactionID;
        //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
        m_purchaseStateCache = new PurchaseState()
        {
            Item = purchaseEvent.purchasedProduct.definition.id,
            Result = PurchaseResult.eResult.SUCCEEDED,
            OrderID = orderID,
            PurchaseToken = orderID,
            Report = new PurchaseResult.PurchaseReport()
        };

        if (_appStoreProducts != null)
        {
            var product =
                _appStoreProducts.FirstOrDefault(p => p.Identifier == purchaseEvent.purchasedProduct.definition.id);
            if (product != null)
            {
                var currencyInfo = CurrencyUtils.ParseCurrencyString(product.Price);
                if (currencyInfo != null)
                {
                    m_purchaseStateCache.Price = currencyInfo.currencyValue;
                }
            }
        }

        m_purchaseStateCache.Report.prodID = purchaseEvent.purchasedProduct.definition.id;
        m_purchaseStateCache.Report.price = purchaseEvent.purchasedProduct.metadata.localizedPrice.ToString();
        m_purchaseStateCache.Report.currency = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode;
        m_purchaseStateCache.Report.receipt = purchaseEvent.purchasedProduct.receipt;
        JsonDict json = new JsonDict();
        if (json.Read(m_purchaseStateCache.Report.receipt)) {
            if(json.Exists("TransactionID"))
                m_purchaseStateCache.Report.transactionID = json.GetString("TransactionID");
            JsonDict payload = new JsonDict();
            if (json.Exists("Payload")) {
                payload.Read(json.GetString("Payload"));
                m_purchaseStateCache.Report.purchaseData = payload.GetString("json");
                m_purchaseStateCache.Report.signature = payload.GetString("signature"); 
            }
        }

        m_transactionResultVerified = true;
        return PurchaseProcessingResult.Complete;
    }

    private void SetNewTransactionID()
    {
        if (string.IsNullOrEmpty(m_purchaseTransactionID))
        {
            m_purchaseTransactionID = Guid.NewGuid().ToString();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if (m_transactionResultVerified)
        {
            return;
        }

        m_purchaseStateCache = new PurchaseState()
        {
            Item = product.definition.id,
            Result = PurchaseResult.eResult.CANCELLED,
            Error = "cancelled by user"
        };
        m_transactionResultVerified = true;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("Store initialized");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        
        try {
            m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }
        catch (Exception e)  {}
            

        try
        {
            var zarinpalExtension = m_StoreExtensionProvider.GetExtension<IZarinpalExtension>();

            if (zarinpalExtension != null)
            {
                zarinpalExtension.PurchaseStarted += ZarinpalExtension_PurchaseStarted;
                zarinpalExtension.PaymentVerificationCompleted += ZarinpalExtension_PaymentVerificationCompleted;
            }
        }
        catch (Exception e)
        {

        }

        IAPEnabled = true;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        IAPEnabled = false;
    }


    #region Zarinpal Callbacks


    private void ZarinpalExtension_PurchaseStarted(string productCode, string authority)
    {
        var zarinpalExtension = m_StoreExtensionProvider.GetExtension<IZarinpalExtension>();

        if (zarinpalExtension != null)
        {
            JsonDict parameters = new JsonDict();
            parameters.Set("username", UserManager.Instance.currentAccount.Username);
            parameters.Set("product_code", productCode);
            parameters.Set("authority", authority);
            parameters.Set("app_store", BasePlatform.ActivePlatform.GetTargetAppStore().ToString());
            parameters.Set("app_version", BasePlatform.ActivePlatform.GetApplicationVersion());
            parameters.Set("player_level", GameDatabase.Instance.XPEvents.GetPlayerLevel().ToString());
            parameters.Set("crew_progress", PlayerProfileManager.Instance.ActiveProfile.GetCrewBattleCompletedCount().ToString());
            WebRequestQueue.Instance.StartCall("acc_zarinpal_purchase_start", "start zarinpal purchase", parameters,
                ZarinpalPurchaseStartResponse,
                null, ProduceHashSource(parameters));
        }
    }

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

        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(content))
        {
            var authority = jsonDict.GetString("authority");
            var zarinpalExtension = m_StoreExtensionProvider.GetExtension<IZarinpalExtension>();
            zarinpalExtension?.StartPay(authority);

            GTDebug.Log(GTLogChannel.Account, "zarinpal purchase started response with authority : " + authority);
        }
    }


    private void ZarinpalExtension_PaymentVerificationCompleted(Purchase purchase)
    {
        var zarinpalExtension = m_StoreExtensionProvider.GetExtension<IZarinpalExtension>();

        if (zarinpalExtension != null)
        {
            JsonDict parameters = new JsonDict();
            parameters.Set("authority", purchase.Signature);
            parameters.Set("refid", purchase.OrderId);

            GTDebug.Log(GTLogChannel.Account, "consuming product with order id : " + purchase.OrderId);
            WebRequestQueue.Instance.StartCall("acc_zarinpal_consume_purchase", "consume zarinpal purchase", parameters,
                ZarinpalConsumePurchaseResponse,
                null, ProduceHashSource(parameters));
        }
        else
        {
            IsProcessingTransaction = false;
        }
    }


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

    #endregion
}
