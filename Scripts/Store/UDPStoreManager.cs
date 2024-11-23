//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UDP;

//public class UDPStoreManager : IAppStoreManager
//{
//    private class PurchaseState
//    {
//        public string Item;
//        public PurchaseResult.eResult Result;
//        public string OrderID;
//        public string Error;
//        public string PurchaseToken;
//        public double Price;
//    }

//    private PurchaseListener m_purchaseListener;
//    private InitListener m_initListener;
//    private List<AppStoreProduct> m_products;
//    private List<PurchaseInfo> m_pendingPurchases;
//    private PurchaseInfo m_currentPurchasedProduct;
//    private PurchaseState m_purchaseStateCache;
//    private bool m_transactionResultVerified;
//    private string m_purchaseTransactionID;


//    public UDPStoreManager()
//    {
//        m_initListener = new InitListener();
//        m_initListener.OnInitializedDelegate += M_initListener_OnInitializedDelegate;
//        m_initListener.OnInitializedFailedDelegate += M_initListener_OnInitializedFailedDelegate;
//        m_purchaseListener = new PurchaseListener();
//        m_purchaseListener.OnPurchaseDelegate += M_purchaseListener_OnPurchaseDelegate;
//        m_purchaseListener.OnPurchaseFailedDelegate += M_purchaseListener_OnPurchaseFailedDelegate;
//        m_purchaseListener.OnPurchaseConsumeDelegate += M_purchaseListener_OnPurchaseConsumeDelegate;
//        m_purchaseListener.OnPurchaseConsumeFailedDelegate += M_purchaseListener_OnPurchaseConsumeFailedDelegate;
//        m_purchaseListener.OnQueryInventoryDelegate += M_purchaseListener_OnQueryInventoryDelegate;
//        Initialize();
//    }

//    private void M_purchaseListener_OnQueryInventoryDelegate(Inventory obj)
//    {
//        GTDebug.Log(GTLogChannel.AppStore,"OnQueryInventoryDelegate Length : "+ obj.GetProductList().Count);
//        m_products = new List<AppStoreProduct>();
//        foreach (var productInfo in obj.GetProductList())
//        {
//            m_products.Add(new AppStoreProduct()
//            {
//                CurrencyCode = productInfo.Currency,
//                Identifier = productInfo.ProductId,
//                Title = productInfo.Title,
//                Description = productInfo.Description,
//                Price = productInfo.Price,
//                LocalisedPrice = productInfo.Price
//            });

//            if (string.IsNullOrEmpty(CurrencyCode))
//            {
//                CurrencyCode = productInfo.Currency;
//            }
//        }

//        GTDebug.Log(GTLogChannel.AppStore,"Product receieved : " + m_products.Count);
//        m_pendingPurchases = obj.GetPurchaseList();


//        if (OnReceivedProductDataResponse != null)
//        {
//            OnReceivedProductDataResponse();
//        }

//        ApplyPendingPurchases();
//    }


//    private void ApplyPendingPurchases()
//    {
//        GTDebug.Log(GTLogChannel.AppStore,"ApplyPendingPurchases..." + (m_pendingPurchases?.Count ?? 0));
//        if (m_pendingPurchases != null && m_pendingPurchases.Count > 0)
//        {
//            if (m_products != null && m_products.Count > 0)
//            {
//                var product = m_products.FirstOrDefault(p => p.Identifier == m_pendingPurchases[0].ProductId);
//                if (product == null)
//                {
//                    GTDebug.Log(GTLogChannel.AppStore,"Consuming pending purchase...");
//                    m_currentPurchasedProduct = m_pendingPurchases[0];
//                    ConsumePurchase(m_pendingPurchases[0].ProductId, m_pendingPurchases[0].GameOrderId);
//                }
//                else
//                {
//                    GTDebug.Log(GTLogChannel.AppStore,"repurchasing pending purchase...");
//                    var orderID = m_pendingPurchases[0].GameOrderId;
//                    //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
//                    m_purchaseStateCache = new PurchaseState()
//                    {
//                        Item = m_pendingPurchases[0].ProductId,
//                        Result = PurchaseResult.eResult.SUCCEEDED,
//                        OrderID = orderID,
//                        PurchaseToken = m_pendingPurchases[0].OrderQueryToken
//                    };
//                    if (m_products != null)
//                    {
//                        double p = 0;
//                        if (double.TryParse(product.Price, out p))
//                        {
//                            m_purchaseStateCache.Price = p;
//                        }
//                    }

//                    m_currentPurchasedProduct = m_pendingPurchases[0];
//                    m_purchaseTransactionID = Guid.NewGuid().ToString();
//                    m_transactionResultVerified = true;
//                }
//            }
//        }
//    }

//    private void M_initListener_OnInitializedFailedDelegate(string obj)
//    {
//        IAPEnabled = false;
//    }

//    private void M_initListener_OnInitializedDelegate(UserInfo obj)
//    {
//        IAPEnabled = true;
//    }

//    private void M_purchaseListener_OnPurchaseConsumeFailedDelegate(string arg1, PurchaseInfo arg2)
//    {
//    }

//    private void M_purchaseListener_OnPurchaseConsumeDelegate(PurchaseInfo obj)
//    {
//        IsProcessingTransaction = false;
//    }

//    private void M_purchaseListener_OnPurchaseFailedDelegate(string arg1, PurchaseInfo arg2)
//    {
//        if (m_transactionResultVerified)
//        {
//            return;
//        }

//        var productID = arg2 != null ? arg2.ProductId : string.Empty;
//        m_purchaseStateCache = new PurchaseState()
//        {
//            Item = productID,
//            Result = PurchaseResult.eResult.CANCELLED,
//            Error = "cancelled by user"
//        };
//        m_transactionResultVerified = true;
//    }

//    private void M_purchaseListener_OnPurchaseDelegate(PurchaseInfo obj)
//    {
//        m_currentPurchasedProduct = obj;
//        m_purchaseStateCache = new PurchaseState();
//        m_purchaseStateCache.Result = PurchaseResult.eResult.SUCCEEDED;
//        m_purchaseStateCache.OrderID = obj.GameOrderId;
//        m_purchaseStateCache.Item = obj.ProductId;
//        m_purchaseStateCache.PurchaseToken = obj.OrderQueryToken;

//        if (m_products != null)
//        {
//            var product = m_products.FirstOrDefault(p => p.Identifier == obj.ProductId);
//            if (product != null)
//            {
//                var currencyInfo = CurrencyUtils.ParseCurrencyString(product.Price);
//                if (currencyInfo!=null)
//                {
//                    m_purchaseStateCache.Price = currencyInfo.currencyValue;
//                }
//            }
//        }

//        m_transactionResultVerified = true;

//    }

//    public void Dispose()
//    {
//    }

//    public event Action OnReceivedProductDataResponse;

//    public bool IAPEnabled { get; private set; }

//    public bool UsingSimulator
//    {
//        get { return false; }
//    }
//    public bool IsProcessingTransaction { get; private set; }
//    public string CurrencyCode { get; private set; }

//    public void SetProducts(List<GTProduct> products)
//    {

//    }

//    public void StartProductRequest()
//    {
//        StoreService.QueryInventory(m_purchaseListener);
//    }

//    public List<AppStoreProduct> GetProducts()
//    {
//        return m_products;
//    }

//    public void Purchase(string productCode)
//    {
//        IsProcessingTransaction = true;
//        m_transactionResultVerified = false;
//        m_purchaseTransactionID = Guid.NewGuid().ToString();
//        StoreService.Purchase(productCode, "", m_purchaseListener);
//    }

//    public void RestorePurchases()
//    {
//    }

//    public PurchaseResult GetPurchaseResult()
//    {
//        if (m_purchaseStateCache == null)
//        {
//            return null;
//        }
//        PurchaseResult purchaseResult = new PurchaseResult();
//        purchaseResult.Result = m_purchaseStateCache.Result;
//        purchaseResult.TransactionID = m_purchaseTransactionID;
//        if (purchaseResult.Result == PurchaseResult.eResult.FAILED)
//        {
//            purchaseResult.ErrorDomain = m_purchaseStateCache.Error;
//            //purchaseResult.ErrorCode = int.Parse(array[3]);
//        }
//        else
//        {
//            purchaseResult.Signature = m_purchaseStateCache.PurchaseToken;//this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
//            purchaseResult.Receipt = m_purchaseStateCache.OrderID;//this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
//            purchaseResult.ProductID = m_purchaseStateCache.Item;
//            purchaseResult.Market = StoreService.StoreName;
//            purchaseResult.CurrencyCode = CurrencyCode;
//            purchaseResult.Price = m_purchaseStateCache.Price;
//        }
//        m_purchaseStateCache = null;
//        //We set this to true until purchase consumed
//        if (purchaseResult.Result == PurchaseResult.eResult.SUCCEEDED)
//        {
//            //#if UNITY_EDITOR
//            IsProcessingTransaction = false;
//            //#else
//            //            IsProcessingTransaction = true;
//            //#endif
//        }
//        else
//        {
//            IsProcessingTransaction = false;
//        }

//        return purchaseResult;
//    }

//    public void ProcessExistingTransactions()
//    {
//    }

//    public void ConsumePurchase(string productCode, string authority)
//    {
//        if (m_currentPurchasedProduct != null)
//        {
//            StoreService.ConsumePurchase(m_currentPurchasedProduct, m_purchaseListener);
//        }
//    }

//    public void Initialize()
//    {
//        GTDebug.Log(GTLogChannel.AppStore,"Initializing UDP");
//        StoreService.Initialize(m_initListener);
//        GTDebug.Log(GTLogChannel.AppStore,"Enable UDP Logging");
//        StoreService.EnableDebugLogging(true);

//    }
//}




//public class InitListener : IInitListener
//{
//    public event Action<UserInfo> OnInitializedDelegate;
//    public event Action<string> OnInitializedFailedDelegate;
//    public void OnInitialized(UserInfo userInfo)
//    {
//        GTDebug.Log(GTLogChannel.AppStore,"UDP initialized");
//        if (OnInitializedDelegate != null)
//        {
//            OnInitializedDelegate(userInfo);
//        }
//        // You can call the QueryInventory method here
//        // to check whether there are purchases that haven’t be consumed.       
//    }

//    public void OnInitializeFailed(string message)
//    {
//        if (OnInitializedFailedDelegate != null)
//        {
//            OnInitializedFailedDelegate(message);
//        }
//    }
//}


//public class PurchaseListener : IPurchaseListener
//{
//    public event Action<PurchaseInfo> OnPurchaseDelegate;
//    public event Action<string,PurchaseInfo> OnPurchaseFailedDelegate;
//    public event Action<string> OnPurchaseRepeatedDelegate;
//    public event Action<PurchaseInfo> OnPurchaseConsumeDelegate;
//    public event Action<string,PurchaseInfo> OnPurchaseConsumeFailedDelegate;
//    public event Action<Inventory> OnQueryInventoryDelegate;
//    public event Action<string> OnQueryInventoryFailedDelegate;
//    public void OnPurchase(PurchaseInfo purchaseInfo)
//    {
//        if (OnPurchaseDelegate != null)
//        {
//            OnPurchaseDelegate(purchaseInfo);
//        }
//    }

//    public void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo)
//    {
//        if (OnPurchaseFailedDelegate != null)
//        {
//            OnPurchaseFailedDelegate(message,purchaseInfo);
//        }
//    }

//    public void OnPurchaseRepeated(string productId)
//    {
//        if (OnPurchaseRepeatedDelegate != null)
//        {
//            OnPurchaseRepeatedDelegate(productId);
//        }
//    }

//    public void OnPurchaseConsume(PurchaseInfo purchaseInfo)
//    {
//        if (OnPurchaseConsumeDelegate != null)
//        {
//            OnPurchaseConsumeDelegate(purchaseInfo);
//        }
//    }

//    public void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo)
//    {
//        if (OnPurchaseConsumeFailedDelegate != null)
//        {
//            OnPurchaseConsumeFailedDelegate(message, purchaseInfo);
//        }
//    }

//    public void OnQueryInventory(Inventory inventory)
//    {
//        if (OnQueryInventoryDelegate != null)
//        {
//            OnQueryInventoryDelegate(inventory);
//        }
//    }

//    public void OnQueryInventoryFailed(string message)
//    {
//        if (OnQueryInventoryFailedDelegate != null)
//        {
//            OnQueryInventoryFailedDelegate(message);
//        }
//    }
//}

