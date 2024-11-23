using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using VoxelBusters.NativePlugins;

#if USE_OLD_IAP
public class IOSStoreManager : IAppStoreManager
{
    private List<AppStoreProduct> m_appStoreProducts;
    public event Action OnReceivedProductDataResponse;
    public bool IsProcessingTransaction { get; private set; }

    public string CurrencyCode { get; private set; }
    private BillingProduct[] _regProductsList;
    private PurchaseState m_purchaseStateCache;
    private static string m_purchaseTransactionID;

    private bool m_transactionResultVerified;
    private bool m_productArrived;

    private class PurchaseState
    {
        public string Item;
        public PurchaseResult.eResult Result;
        public string OrderID;
        public string Error;
        public string PurchaseToken;
    }

    public bool UsingSimulator
    {
        get { return false; }
    }

    public bool IAPEnabled
    {
        get
        {
            {
                return NPBinding.Billing.CanMakePayments();
            }
        }
    }


    public IOSStoreManager()
    {
        Billing.DidFinishRequestForBillingProductsEvent += OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent += OnDidFinishTransaction;
        m_appStoreProducts = new List<AppStoreProduct>();
    }


    ~IOSStoreManager()
    {
        Billing.DidFinishRequestForBillingProductsEvent -= OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent -= OnDidFinishTransaction;
    }

    public void Dispose()
    {
    }

    private void OnDidFinishProductsRequest(BillingProduct[] _regProductsList, string _error)
    {
        if (m_productArrived)
            return;
        // Hide activity indicator
        if (_regProductsList != null)
        {
//            Debug.Log("UNity : OnDidFinishProductsRequest "+_regProductsList.Length);
        }
        else
        {
//            Debug.Log("failed to get product : " + _error);
        }
        // Handle response
        if (_error != null)
        {
//            Debug.Log("failed to get product : " + _error);
            // Something went wrong
        }
        else
        {
            this._regProductsList = _regProductsList;
            // Inject code to display received products
            foreach (var billingProduct in _regProductsList)
            {
                if (string.IsNullOrEmpty(CurrencyCode))
                    CurrencyCode = billingProduct.CurrencyCode;
                m_appStoreProducts.Add(new AppStoreProduct()
                {
                    Identifier = billingProduct.ProductIdentifier,
                    CurrencyCode = billingProduct.CurrencyCode,
                    CurrencySymbol = billingProduct.CurrencySymbol,
                    Description = billingProduct.Description,
                    LocalisedPrice = billingProduct.LocalizedPrice,
                    Price = billingProduct.Price.ToString(),
                    Title = billingProduct.Name,

                });
            }
            
            if (OnReceivedProductDataResponse != null)
            {
//                Debug.Log("callback Products arrived");
                OnReceivedProductDataResponse();
            }

            m_productArrived = true;
//            Debug.Log("Products arrived");
        }
    }

    private void OnDidFinishTransaction(BillingTransaction _transaction)
    {
        if (_transaction != null)
        {
            if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS)
            {
                if (_transaction.TransactionState == eBillingTransactionState.PURCHASED)
                {
                    // Your code to handle purchased products
                    var orderID = _transaction.TransactionReceipt;
                    //var orderID = "260933663077296160";//This is a valid Bazaar purchase token
                    m_purchaseStateCache = new PurchaseState()
                    {
                        Item = _transaction.ProductIdentifier,
                        Result = PurchaseResult.eResult.SUCCEEDED,
                        OrderID = orderID,
                        PurchaseToken = _transaction.TransactionIdentifier
                    };
                    m_transactionResultVerified = true;
                    IsProcessingTransaction = false;
                    return;
                }
            }

            if (m_transactionResultVerified)
            {
                return;
            }
            m_purchaseStateCache = new PurchaseState()
            {
                Item = _transaction.ProductIdentifier,
                Result = PurchaseResult.eResult.CANCELLED,
                Error = "cancelled by user"
            };
            IsProcessingTransaction = false;
            m_transactionResultVerified = true;
        }
    }

    public void SetProducts(List<GTProduct> products)
    {
    }

    public void StartProductRequest()
    {
        if (m_productArrived)
        {
            if (OnReceivedProductDataResponse != null)
            {
                GTDebug.Log(GTLogChannel.AppStore,"callback Products arrived");
                OnReceivedProductDataResponse();
            }
        }
        else
        {
            NPBinding.Billing.RequestForBillingProducts(NPSettings.Billing.Products);
        }
    }

    public List<AppStoreProduct> GetProducts()
    {
        return m_appStoreProducts;
    }

    public void Purchase(string productCode)
    {
        var _product = _regProductsList.FirstOrDefault(i => i.ProductIdentifier == productCode);
        //if (NPBinding.Billing.IsProductPurchased(_product.ProductIdentifier))
        //{
        //    // Show alert message that item is already purchased
        //    return;
        //}

        IsProcessingTransaction = true;
        m_transactionResultVerified = false;
        m_purchaseTransactionID = Guid.NewGuid().ToString();

        // Call method to make purchase
        NPBinding.Billing.BuyProduct(_product);

        // At this point you can display an activity indicator to inform user that task is in progress
    }

    public void RestorePurchases()
    {

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
            purchaseResult.Market = "AppStore";
        }
        m_purchaseStateCache = null;
        //We set this to true until purchase consumed
        if (purchaseResult.Result == PurchaseResult.eResult.SUCCEEDED)
        {
            IsProcessingTransaction = true;
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
        //Since we have not consume prodict on iOS , then we make proccessing transaction finished
        IsProcessingTransaction = false;
    }

    public void Initialize()
    {
        //Calling this method force plugin to be initialized.
        NPBinding.Billing.IsAvailable();
    }
}
#endif
