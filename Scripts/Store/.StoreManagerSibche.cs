//#define APPLE_RELEASE

#if APPLE_RELEASE
using System;
using System.Collections.Generic;
using System.Linq;
using SibcheStoreKit;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class StoreManagerSibche : IAppStoreManager
{
    public event Action OnReceivedProductDataResponse;
    public bool IAPEnabled { get; private set; }
    public bool UsingSimulator { get; }
    public bool IsProcessingTransaction { get;  set; }
    public string CurrencyCode { get; }
    public void SetProducts(List<GTProduct> products)
    {
        
    }



    private string SibcheApiKey= "Wewg8E2nl9DmJN1kmQ3QkGqYrRVXMZ";
    private string SibcheScheme="gt";
    public StoreManagerSibche()
    {
        Sibche.Initialize(SibcheApiKey,SibcheScheme);
        UnityEngine.Debug.Log("sibche initiated successfully");
        StartProductRequest();
    }



//    public void StartProductRequest()
//    {
//        if(m_appStoreProducts==null || m_appStoreProducts.Count<=0)
//        Sibche.FetchPackages((bool isSuccessful, SibcheError error, List<SibchePackage> packages) =>
//        {
//            if (isSuccessful)
//            {
//                m_appStoreProducts= new List<AppStoreProduct>();
//                foreach (var item in packages)
//                {
//                    UnityEngine.Debug.Log("sibche packageid::::>>"+  item.packageId);
//                    UnityEngine.Debug.Log("sibche name::::>>"+  item.name);
//                    UnityEngine.Debug.Log("sibche code::::>>"+  item.code);
//                    m_appStoreProducts.Add(
//                        new AppStoreProduct()
//                        {
//                            CurrencyCode ="تومان",
//                            //  CurrencyCode = "IQD",
//                            Description =item.packageDescription,
//                            Identifier =item.code,
//                            
//                            Price = item.price.ToString(),
//                            LocalisedPrice = item.price.ToString(),
//                            Title = item.name,
//                           // FortumoServiceID = marketItem.FortumoServiceID
//                        }
//                        );
//                }
//               UnityEngine.Debug.Log("sibche get pakcage success");
//               if(m_appStoreProducts!=null)
//               UnityEngine.Debug.Log("sibche number of products" + m_appStoreProducts.Count);
//               else UnityEngine.Debug.Log("sibche products are null");
//                IAPEnabled = true;
//                
//                if (OnReceivedProductDataResponse != null)
//                {
//                    OnReceivedProductDataResponse();
//                    UnityEngine.Debug.Log("sibche action called");
//                }
//                else {UnityEngine.Debug.Log("sibche action was null");}
//            }
//            else
//            {
//                Debug.Log( "sibche get pakcage error"+error.message);
//                IAPEnabled = false;
//            }
//        });
//    }


 public void StartProductRequest()
    {
        if (m_appStoreProducts == null || m_appStoreProducts.Count <= 0)
        {

            JsonDict parameters = new JsonDict();
            parameters.Set("username", "dummy_username");
            //  parameters.Set("currency",LocalizedCurrencies.UserCurrency);
           // parameters.Set("currency", LocalizedCurrencies.UserCurrency);
            WebRequestQueue.Instance.StartCall("rtw_get_products", "get all iap products", parameters,
                OnGetProductsResponse);
        }
    }


    private void OnGetProductsResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
      
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting weekly leaderboard : " + zerror);
            IAPEnabled = false;
            UnityEngine.Debug.Log("sibche get products list fail");
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(zhttpcontent))
        {
            GTDebug.LogError(GTLogChannel.RPBonus,
                "error getting weekly leaderboard : server send malformed json in response");
            IAPEnabled = false;
            return;
            UnityEngine.Debug.Log("sibche read products fail");
        }

        var products = parameters.GetObjectList<IapProduct>("products", GetIapProduct);

        m_appStoreProducts = new List<AppStoreProduct>();
        foreach (var marketItem in products)
        {
            m_appStoreProducts.Add(new AppStoreProduct()
            {
                CurrencyCode =LocalizedCurrencies.UserCurrencyCode,
            //  CurrencyCode = "IQD",
                Description = marketItem.Name,
                Identifier = marketItem.ID,
                Price = marketItem.Value.ToString(),
              //  LocalisedPrice ="تومان" ,//GetLocalizedPrice(marketItem.Value),
              LocalisedPrice= marketItem.Value.ToString()+"تومان",
                Title = marketItem.Name,
                FortumoServiceID = marketItem.FortumoServiceID
            });
        }

        if (OnReceivedProductDataResponse != null)
        {
            OnReceivedProductDataResponse();
        }

        IAPEnabled = true;
        UnityEngine.Debug.Log("");
        //  CheckAnyPendingPurchases();
    }

  private void GetIapProduct(JsonDict jsondict, ref IapProduct product)
    {
        product.ID = jsondict.GetString("id");
        product.Name = jsondict.GetString("name");
        product.Value = jsondict.GetInt("value");
        product.Available = jsondict.GetBool("avlb");
        product.FortumoServiceID = jsondict.GetString("fls");
    }


    private List<AppStoreProduct> m_appStoreProducts;
    public List<AppStoreProduct> GetProducts()
    {
        return m_appStoreProducts;
 
    }


    
    public PurchaseResult FinalPurchaseResult=null;// result of all purchases should be saved in here 
    private SibchePurchasePackage cachePurchasedPackaged;// in this time its only used for consume
    public void Purchase(string productCode)
    {
        IsProcessingTransaction = true;
        if (SibcheIosEvents._instance == null)
        {
            GameObject SibcheEventHandler= new GameObject("sibche");
            SibcheEventHandler.AddComponent<SibcheIosEvents>();
            SibcheEventHandler.GetComponent<SibcheIosEvents>().SibcheReference = this;
        }
        
        
        Sibche.Purchase(productCode, (bool isSuccessful, SibcheError error, SibchePurchasePackage purchasedPackage) =>
        {
            UnityEngine.Debug.Log("purchase resulkt is::::>>"+ isSuccessful.ToString());
          if(error!=null)  UnityEngine.Debug.Log("sibche purchase error is:::::>>>>"+error.message);
            if(purchasedPackage!=null) UnityEngine.Debug.Log("sibche purchase package is" + purchasedPackage.code);

            cachePurchasedPackaged = purchasedPackage;
            if (purchasedPackage == null)
            {
                UnityEngine.Debug.Log("sibche package result was null");
                SibcheNothingHappened();
            }
            
            if (isSuccessful)
            {
                FinalPurchaseResult= new PurchaseResult();
                FinalPurchaseResult.ProductID = productCode;//todo ::::: im not sure
                FinalPurchaseResult.Market = "sibche";
                FinalPurchaseResult.TransactionID = Guid.NewGuid().ToString();//;
                FinalPurchaseResult.Result = PurchaseResult.eResult.SUCCEEDED;
                FinalPurchaseResult.Receipt = Guid.NewGuid().ToString();
                double parsedValue = 0;
                double.TryParse( m_appStoreProducts.First(x => x.Identifier == productCode).Price,out parsedValue);
                FinalPurchaseResult.Price = parsedValue;
                IsProcessingTransaction = false;
                PurchaseResultWaiting = true;
               ConsumePurchase(purchasedPackage.purchasePackageId,"");
              UnityEngine.Debug.Log("sibche purchase happened success fuly");
            }
            else
            {
                FinalPurchaseResult = new PurchaseResult();
                FinalPurchaseResult.ProductID = purchasedPackage.package.packageId;//todo ::::: im not sure
                FinalPurchaseResult.Market = "sibche";
                FinalPurchaseResult.TransactionID = null;
                FinalPurchaseResult.Result = PurchaseResult.eResult.FAILED;
                FinalPurchaseResult.Receipt = purchasedPackage.code;
                IsProcessingTransaction = false;
                PurchaseResultWaiting = true;
                UnityEngine.Debug.Log("sibche purchase failed");
            }
        });
    }

    public void RestorePurchases()
    {
       
    }

    public bool PurchaseResultWaiting = false;
    public PurchaseResult GetPurchaseResult()
    {
        if (!PurchaseResultWaiting) return null;
        PurchaseResultWaiting = false;
        return FinalPurchaseResult;
    }

    public void ProcessExistingTransactions()
    {
       
    }

    public void ConsumePurchase(string productCode, string authority)
    {
        Sibche.Consume(productCode, (bool isSuccessful, SibcheError error) =>
        {
            if (isSuccessful)
            {
               // IsProcessingTransaction = false;
               UnityEngine.Debug.Log("sibche consume success");
            }
            else
            {
               // IsProcessingTransaction = false;
               UnityEngine.Debug.Log("sibche consume failed");
            }
        });
    }

    public void Initialize()
    {
     
    }



    public void SibcheNothingHappened()
    {
        FinalPurchaseResult = new PurchaseResult();
        FinalPurchaseResult.ProductID =null;//purchasedPackage.package.packageId;//todo ::::: im not sure
        FinalPurchaseResult.Market = "sibche";
        FinalPurchaseResult.TransactionID = null;
        FinalPurchaseResult.Result = PurchaseResult.eResult.FAILED;
        FinalPurchaseResult.Receipt = null;//purchasedPackage.code;
        IsProcessingTransaction = false;
        PurchaseResultWaiting = true;
        UnityEngine.Debug.Log("sibche nothing happened");
    }

    public void Dispose()
    {
    }
}
#endif
