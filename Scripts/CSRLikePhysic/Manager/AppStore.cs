using System;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
//using KingKodeStudio.IAB;
using Metrics;
using UnityEngine;

public class AppStore : MonoBehaviour
{
    public delegate void OnTransactionResult(PurchaseResult.eResult result);

    private bool waitingForProductData;

    private bool haveExpectedProducts;

    private ReceiptProcessor receiptProcessor;


    public IAppStoreManager appStoreManager;

    public event OnTransactionResult TransactionResult;


    //private static IABConfig m_setting;
    //public static IABConfig IabSetting
    //{
    //    get
    //    {
    //        if (m_setting == null)
    //        {
    //            m_setting = Resources.Load<IABConfig>("IABSetting");
    //            if (m_setting == null)
    //            {
    //                KKLog.LogError("'IABSetting' not found at Resources.Reimport plugin to fix this problem");
    //            }
    //        }
    //        return m_setting;
    //    }
    //}



    public static AppStore Instance
    {
        get;
        private set;
    }

    public bool IsWaitingForProductData
    {
        get
        {
            return this.waitingForProductData;
        }
    }

    public bool IAPEnabled
    {
        get
        {
            if (this.appStoreManager == null)
                return false;
            return this.appStoreManager.IAPEnabled;
        }
    }
    
    public bool ShouldHideIAPInterface
    {
        get
        {
            return BuildType.IsAppTuttiBuild || BuildType.IsVasBuild;
        }
    }

    public bool IsProcessingTransaction
    {
        get
        {
            return this.appStoreManager.IsProcessingTransaction;
        }
    }

    public string CurrencyCode
    {
        get
        {
            if (!string.IsNullOrEmpty(this.appStoreManager.CurrencyCode))
                return this.appStoreManager.CurrencyCode;
            else
            {
                if (BasePlatform.ActivePlatform.InsideCountry)
                {
                    return "IRT";
                }
                else
                {
                    return "USD";
                }
            }
        }
    }

    public List<AppStoreProduct> GetProducts()
    {
        var products = appStoreManager.GetProducts();
        if (products == null)
            return null;
        List<AppStoreProduct> list = (from p in products
                                      where p.Valid()
                                      select p).ToList<AppStoreProduct>();
        list.Sort((a, b) => a.Identifier.CompareTo(b.Identifier));
        return list;
    }

    public void Purchase(string id)//first line of shopping settings
    {
        try
        {
            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
            {
                {Parameters.Itm, id},
                {Parameters.Currency, CurrencyCode},
                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()}
            };
            Log.AnEvent(Events.PurchaseStart, eventData);
        } catch {}
        
        if (this.waitingForProductData)
        {
        }
        this.appStoreManager.Purchase(id);

    }

    public void RestorePurchases()
    {
        if (this.waitingForProductData)
        {
        }
        this.appStoreManager.RestorePurchases();
    }

    public static bool ShouldPoll()
    {
        return SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend || (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race && RaceController.Instance.Machine.StateAfter(RaceStateEnum.race));
    }

    private void ProcessReceipt(PurchaseResult result)
    {
        if (!this.appStoreManager.UsingSimulator && !string.IsNullOrEmpty(result.Receipt))
        {
            this.receiptProcessor.ValidateReceipt(result);
        }
        UserManager.Instance.UpdateUserAccountFromPurchaseResult(result);// smkp
    }

    private void Update()
    {
        if (UserManager.Instance == null || UserManager.Instance.currentAccount == null)
        {
            return;
        }
        if (this.receiptProcessor == null)
        {
            return;
        }
        if (ShouldPoll() && GTSystemOrder.systemsReady && ScreenManager.Instance.CurrentScreen != ScreenID.Splash)
        {
            PurchaseResult purchaseResult = this.appStoreManager.GetPurchaseResult();

            if (purchaseResult != null)
            {
                GTDebug.Log(GTLogChannel.AppStore,"Purchase Result has value");
                Log.AnEvent(Events.IAPPurchaseDiagnostic, new Dictionary<Parameters, string>
                {
                    {
                        Parameters.Ver,
                        ApplicationVersion.Current
                    },
                    {
                        Parameters.Platform,
                        GTPlatform.Target.ToString()
                    },
                    {
                        Parameters.CSRID,
                        UserManager.Instance.currentAccount.UserID.ToString()
                    },
                    {
                        Parameters.PurchaseData,
                        purchaseResult.ToMetric()
                    }
                });
                GTDebug.Log(GTLogChannel.AppStore,"Purchase result : "+ purchaseResult.Result);
                if (purchaseResult.Result != PurchaseResult.eResult.FAILED && purchaseResult.Result != PurchaseResult.eResult.DEFERRED
                    && purchaseResult.Result != PurchaseResult.eResult.CANCELLED)
                {
                    GTDebug.Log(GTLogChannel.AppStore,"Checking purchase validation : "+ purchaseResult.IsValid());
                    if (purchaseResult.IsValid())
                    {
                        this.ProcessReceipt(purchaseResult);
                    }
                    else
                    {
                        GTDebug.Log(GTLogChannel.AppStore,"purchase is not valid : "+ purchaseResult.ToString());
                        purchaseResult.Result = PurchaseResult.eResult.FAILED;
                    }
                }
                if (this.TransactionResult != null)
                {
                    try
                    {
                        if (purchaseResult.Result == PurchaseResult.eResult.SUCCEEDED) {
                            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
                            {
                                {Parameters.Itm, purchaseResult.ProductID},
                                {Parameters.Currency, purchaseResult.CurrencyCode},
                                {Parameters.Price, purchaseResult.Price.ToString()},
                                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()}
                            };
                            Log.AnEvent(Events.PurchaseSuccess, eventData);
                        } else if (purchaseResult.Result == PurchaseResult.eResult.CANCELLED) {
                            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
                            {
                                {Parameters.Itm, purchaseResult.ProductID},
                                {Parameters.Currency, purchaseResult.CurrencyCode},
                                {Parameters.Price, purchaseResult.Price.ToString()},
                                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()}
                            };
                            Log.AnEvent(Events.PurchaseCancel, eventData);
                        } else if (purchaseResult.Result == PurchaseResult.eResult.FAILED)  {
                            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
                            {
                                {Parameters.Itm, purchaseResult.ProductID},
                                {Parameters.Currency, purchaseResult.CurrencyCode},
                                {Parameters.Price, purchaseResult.Price.ToString()},
                                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()},
                                {Parameters.ErrorDomain, purchaseResult.ErrorDomain==null?"":purchaseResult.ErrorDomain},
                                {Parameters.ErrorCode, purchaseResult.ErrorCode==null?"":purchaseResult.ErrorCode.ToString()}
                            };
                            Log.AnEvent(Events.PurchaseFail, eventData);
                        } 
                    }
                    catch {}
                    GTDebug.Log(GTLogChannel.AppStore,"Calling TransactionResult");
                    this.TransactionResult(purchaseResult.Result);
                }
            }
        }
        this.receiptProcessor.Update();
    }

    public void SetExpectedProducts(List<GTProduct> products)
    {
        this.haveExpectedProducts = false;
        this.waitingForProductData = false;
        this.appStoreManager.SetProducts(products);
        this.waitingForProductData = true;
        this.haveExpectedProducts = true;
        this.receiptProcessor = new ReceiptProcessor(GameDatabase.Instance.RevenueTrackingConfiguration);
        this.StartProductRequestIfStillWaiting();

    }

    //private static bool isSibche = true;//todo: just 
    private static IAppStoreManager CreateAppStoreManager()
    {
//#if UNITY_IOS
//#if APPLE_RELEASE
//        if (IabSetting.IsSibche)
//        {
//            return new StoreManagerSibche();
//        }
//        else if (BasePlatform.ActivePlatform.InsideCountry)
//#else
//        Debug.Log("inside country : "+BasePlatform.ActivePlatform.InsideCountry);
//        if (BasePlatform.ActivePlatform.InsideCountry)
//#endif
//        {
//            return new StoreManagerV2();
//        }
//        else
//        {
//            return new IOSStoreManager();
//        }
//#else
//            return new StoreManagerV2();
//#endif

        return new UnityIAPStoreManager();
    }

    public void EnsureAppStoreCurrectSetting()
    {
//        bool isGooglePlay = false;
//#if UNITY_ANDROID || UNITY_EDITOR
//        isGooglePlay = KingIAB.Setting.IsGooglePlay;
//#endif
//        if (isGooglePlay && BasePlatform.ActivePlatform.InsideFortumoZone && UserManager.Instance.currentAccount.IsFortumo
//            && !(appStoreManager is StoreManagerFortumo))
//        {
//            SwitchAppStoreToFortumo();
//        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        // this is only for test. comment this for release.
        //  UserManager.Instance.currentAccount.IsFortumo = true;
        this.appStoreManager = CreateAppStoreManager();
        this.appStoreManager.OnReceivedProductDataResponse += this.OnRecievedProductDataResponse;
        ApplicationManager.DidBecomeActiveEvent += this.StartProductRequestIfStillWaiting;
        PolledNetworkState.JustCameOnline += this.StartProductRequestIfStillWaiting;
        UserManager.LoggedInEvent += UserManager_LoggedInEvent;
    }

    private void UserManager_LoggedInEvent()
    {
        EnsureAppStoreCurrectSetting();
    }

    //public void SwitchAppStoreToFortumo()
    //{
    //    if (this.appStoreManager != null)
    //    {
    //        this.appStoreManager.OnReceivedProductDataResponse -= this.OnRecievedProductDataResponse;
    //        this.appStoreManager.Dispose();
    //    }

    //    this.waitingForProductData = true;
    //    this.haveExpectedProducts = true;
    //    this.appStoreManager = new StoreManagerFortumo();
    //    this.appStoreManager.OnReceivedProductDataResponse += this.OnRecievedProductDataResponse;
    //}


    public void SwitchAppStoreToGooglePlay()
    {
        if (this.appStoreManager != null)
        {
            this.appStoreManager.OnReceivedProductDataResponse -= this.OnRecievedProductDataResponse;
            this.appStoreManager.Dispose();
        }

        this.waitingForProductData = true;
        this.haveExpectedProducts = true;
        this.appStoreManager = new StoreManagerV2();
        this.appStoreManager.OnReceivedProductDataResponse += this.OnRecievedProductDataResponse;
    }

    public void ProcessExistingTransactions()
    {
        this.appStoreManager.ProcessExistingTransactions();
    }

    public void StartProductRequestIfStillWaiting()
    {
        GTDebug.Log(GTLogChannel.AppStore,"StartProductRequestIfStillWaiting : " + haveExpectedProducts + "   ,   " + this.waitingForProductData);
        if (this.haveExpectedProducts && this.waitingForProductData)
        {
            this.appStoreManager.StartProductRequest();
        }
    }

    private void OnRecievedProductDataResponse()
    {
        GTDebug.Log(GTLogChannel.AppStore,"OnRecievedProductDataResponse : " + waitingForProductData + " , " + this.haveExpectedProducts);
        if (this.waitingForProductData && this.haveExpectedProducts)
        {
            this.waitingForProductData = false;
        }
      
    }

    public bool CheckItemAvailable(string itemKey)
    {
        var products = this.GetProducts();
        return !this.IsWaitingForProductData && products != null && products.Any((AppStoreProduct p) => p.Identifier.EndsWith(itemKey));
    }

    public void ConsumePurchase(string productCode, string authority)
    {
        this.appStoreManager.ConsumePurchase(productCode, authority);
    }

    public void ConnectToStore()
    {
        //this.appStoreManager.Initialize();
    }
}
