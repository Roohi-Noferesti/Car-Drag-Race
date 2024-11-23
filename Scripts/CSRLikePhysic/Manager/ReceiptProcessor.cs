using Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameAnalyticsSDK;
using ir.metrix.unity;
using KingKodeStudio;
using UnityEngine;
using AppsFlyerSDK;
using FlurrySDK;
using Debug = UnityEngine.Debug;

public class ReceiptProcessor
{
    private Queue<PurchaseResult> pendingReceipts = new Queue<PurchaseResult>();

    private bool waitingForResponse;

    private RevenueTrackingConfiguration _revenueTrackingConfiguration;

    public ReceiptProcessor(RevenueTrackingConfiguration revenueTrackingConfig)
    {
        this._revenueTrackingConfiguration = revenueTrackingConfig;
        this.LoadReceiptQueue();
    }

    private void LoadReceiptQueue()
    {
        string text = FileUtils.DecompressFromLocalStorage("receipts", true, false);
        if (!string.IsNullOrEmpty(text))
        {
            PurchaseResult[] array = JsonConverter.DeserializeObject<PurchaseResult[]>(text);
            if (array != null)
            {
                this.pendingReceipts = new Queue<PurchaseResult>(array);
            }
        }
    }

    private void SaveReceiptQueue()
    {
        string zContent = JsonConverter.SerializeObject(this.pendingReceipts.ToArray(), true);
        FileUtils.CompressToLocalStorage("receipts", zContent, true, false);
    }

    public void ValidateReceipt(PurchaseResult result)
    {
        this.pendingReceipts.Enqueue(result);
        this.SaveReceiptQueue();
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

    public void Update()
    {
        bool flag = this.pendingReceipts.Count > 0 && UserManager.Instance.isLoggedIn &&
                    PolledNetworkState.IsNetworkConnected && !WebRequestQueue.Instance.isOffline &&
                    SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend;
        if (flag && !this.waitingForResponse)
        {
            this.waitingForResponse = true;
            PurchaseResult purchaseResult = this.pendingReceipts.Peek();
            if (!purchaseResult.IsValid())
            {
                this.pendingReceipts.Dequeue();
                this.SaveReceiptQueue();
                return;
            }

            JsonDict jsonDict = new JsonDict();
            jsonDict.Set("use_v2", "true");
            jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
            jsonDict.Set("transaction_id", purchaseResult.TransactionID);
            jsonDict.Set("signed_data", purchaseResult.Receipt);
            jsonDict.Set("signature",
                string.IsNullOrEmpty(purchaseResult.Signature) ? string.Empty : purchaseResult.Signature);
            jsonDict.Set("product_code", purchaseResult.ProductID);
            jsonDict.Set("currency_code", purchaseResult.CurrencyCode);
            jsonDict.Set("price", purchaseResult.Price.ToString());
            jsonDict.Set("is_google_play",
                (BasePlatform.ActivePlatform.GetTargetAppStore() != GTAppStore.GooglePlay ||
                 BasePlatform.ActivePlatform.GetTargetAppStore() != GTAppStore.None)
                    ? "0"
                    : "1");
            jsonDict.Set("app_store", BasePlatform.ActivePlatform.GetTargetAppStore().ToString());
            jsonDict.Set("coreid", MetricsIntegration.GetNMCoreIDSafe());
            jsonDict.Set("market", string.IsNullOrEmpty(purchaseResult.Market) ? "" : purchaseResult.Market);
            jsonDict.Set("app_version", BasePlatform.ActivePlatform.GetApplicationVersion());
            jsonDict.Set("player_level", GameDatabase.Instance.XPEvents.GetPlayerLevel().ToString());
            jsonDict.Set("crew_progress", PlayerProfileManager.Instance.ActiveProfile.GetCrewBattleCompletedCount().ToString());
            MetricsIntegration.Instance.AddAppsFlyerPurchaseData(ref jsonDict);

            WebRequestQueue.Instance.StartCall("acc_receipt_android", "Verifying android product receipt", jsonDict,
                new WebClientDelegate2(this.ReceiptVerificationResponse), null, this.ProduceHashSource(jsonDict));
        }
    }

    private JsonDict VerifyResponse(string content)
    {
        JsonDict jsonDict = new JsonDict();
        if (!jsonDict.Read(content))
        {
            return null;
        }

        if (!jsonDict.ContainsKey("gold") || !jsonDict.ContainsKey("cash") || !jsonDict.ContainsKey("supernitrous") ||
            !jsonDict.ContainsKey("is_valid_receipt"))
        {
            return null;
        }

        string zSource = string.Concat(new string[]
        {
            jsonDict.GetInt("gold").ToString(),
            jsonDict.GetInt("cash").ToString(),
            jsonDict.GetInt("supernitrous").ToString(),
            jsonDict.GetInt("is_valid_receipt").ToString(),
            UserManager.Instance.webClient.Session
        });
        string text = BasePlatform.ActivePlatform.HMACSHA1_Hash(zSource, BasePlatform.eSigningType.Server_Accounts);
        if (text != jsonDict.GetString("secret"))
        {
            string error = string.Concat(new string[]
            {
                "server response failed checksum, client='",
                text,
                "', server='",
                jsonDict.GetString("secret"),
                "'"
            });
            GTDebug.LogError(GTLogChannel.ShopScreen, error);
            return null;
        }

        return jsonDict;
    }

    private void SetMetricDeltaConsumables(string productID, Dictionary<Parameters, string> data)
    {
        GTProduct productWithID = ProductManager.Instance.GetProductWithID(productID);
        if (productWithID != null)
        {
            data[Parameters.DCsh] = (productWithID.Cash + productWithID.BonusCash).ToString();
            data[Parameters.DGld] = (productWithID.Gold + productWithID.BonusGold).ToString();
            data[Parameters.DXp] = GameDatabase.Instance.XPEvents.GetXPPrizeForPurchase().ToString();
        }
    }

    private void ReceiptVerificationResponse(string content, string zError, int zStatus, object zUserData)
    {
        this.waitingForResponse = false;
        if (this.pendingReceipts.Count == 0)
        {
            return;
        }

        if (zStatus != 200 || !string.IsNullOrEmpty(zError))
        {
            return;
        }

        JsonDict jsonDict = this.VerifyResponse(content);
        if (jsonDict == null)
        {
            return;
        }
        
        bool isTestPurchase = false;
        if (jsonDict.ContainsKey("purchaseType"))
        {
            isTestPurchase = (jsonDict.GetInt("purchaseType") == 0);
        }

        int isValidReceipt = jsonDict.GetInt("is_valid_receipt");
        PurchaseResult purchaseResult = this.pendingReceipts.Peek();
        bool isSuperNitrous = purchaseResult.ProductID.Contains("super");
        string text = purchaseResult.ProductID;
        if (isSuperNitrous)
        {
            text = text + "_" + BoostNitrous.GetBestBoostNitrousTierForUser().ToString();
        }

        if (isValidReceipt == 1)
        {
            if (purchaseResult.Result != PurchaseResult.eResult.RESTORED)
            {
                Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
                {
                    {
                        Parameters.ItmClss,
                        "iap"
                    },
                    {
                        Parameters.Itm,
                        text
                    },
                    {
                        Parameters.RecVer,
                        "1"
                    }
                };
                this.SetMetricDeltaConsumables(text, data);
                Log.AnEvent(Events.PurchaseItem, data);
                GameDatabase.Instance.XPEvents.AddPlayerXP(GameDatabase.Instance.XPEvents.GetXPPrizeForPurchase());
                
                try
                {
                    this.SendApsalarRevenueTrackingEvent(purchaseResult, isTestPurchase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        else
        {
            Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
            {
                {
                    Parameters.ItmClss,
                    "iap"
                },
                {
                    Parameters.RecVer,
                    "0"
                }
            };
            this.SetMetricDeltaConsumables(purchaseResult.ProductID, dictionary);
            dictionary[Parameters.Itm] = text;
            Log.AnEvent(Events.PurchaseItem, dictionary);
        }

        this.pendingReceipts.Dequeue();
        this.SaveReceiptQueue();
        UserManager.Instance.UpdateUserAccountFromReceipt(jsonDict);
        var productCode = jsonDict.GetString("code");
        var orderID = jsonDict.GetString("orderid");
        GTDebug.Log(GTLogChannel.AssetPurchasing, "Consuming purchase : " + productCode + "   ,  " + isValidReceipt);
        if (isValidReceipt == 1 && !string.IsNullOrEmpty(productCode))
        {
            //We put
            AppStore.Instance.ConsumePurchase(productCode, orderID);
        }

        if (this.pendingReceipts.Count == 0)
        {
            UserManager.Instance.UpdateAccountBalanceFromReceipt(jsonDict);
        }
    }

    private void SendApsalarRevenueTrackingEvent(PurchaseResult purchaseResult, bool isTestPurchase)
    {
        if (this._revenueTrackingConfiguration != null && this._revenueTrackingConfiguration.Enabled)
        {
            string value = "NM_GP";
            string productID = purchaseResult.ProductID;
            string transactionID = purchaseResult.TransactionID;
            string orderID = purchaseResult.Receipt;
            string currency = purchaseResult.CurrencyCode;
            double price = purchaseResult.Price;

            if (IsDuplicatedPurchase(orderID))
            {
                UnityEngine.Debug.Log("duplicate purchase . ignore logging to trackers");
                return;
            }


            if (this._revenueTrackingConfiguration.ApsalarSendEventEnabled)
            {
                if (BasePlatform.ActivePlatform.InsideCountry)
                {
                    // We convert each 1000 toman to 0.01$(1 cent) due of marketing strategy decided by Marketing Team
                    //For Example 5000 toman will be reported as 0.05$
                    if (purchaseResult.CurrencyCode == "IRT")
                    {
                        price = (float)purchaseResult.Price / 100000;
                    }
                    else if (purchaseResult.CurrencyCode == "IRR")
                    {
                        price = (float)purchaseResult.Price / 1000000;
                    }

                    currency = "USD";
                }
				
				
                if (!PurchasingModuleSelection.IsUDP && !isTestPurchase && BuildType.CanCollectData())
                {
                    if (BasePlatform.ActivePlatform.InsideCountry)
                    {
                        LogRevenueToMetrix(purchaseResult.CurrencyCode, purchaseResult.Price, orderID, productID);
                    }
                    LogRevenueToAppsMetrica(purchaseResult.ProductID, price, currency, orderID, purchaseResult.Signature);
                    LogRevenueToFlurry(purchaseResult.ProductID, price, currency,
                        orderID);
                    LogRevenueToGameAnalytics(currency, price, orderID, productID, purchaseResult.Signature);
                    LogRevenueToUnityAnalytics(currency, price, orderID, productID, purchaseResult.Signature, purchaseResult.Market, purchaseResult.Receipt);
                    // if (!BasePlatform.ActivePlatform.InsideCountry) {
                    //     LogRevenueToAppsFlyer(purchaseResult.Report); 
                    // } else {
                    //     Dictionary<string, string> appsflyerParameters = new Dictionary<string, string>
                    //     {
                    //         {"UserID", UserManager.Instance.currentAccount.UserID.ToString()},
                    //         {"ProductID", productID},
                    //         {"Price", price.ToString()},
                    //         {"Currency", currency},
                    //         {"TransactionID", purchaseResult.Report.signature},
                    //         {"AppVersion", BasePlatform.ActivePlatform.GetApplicationVersion()},
                    //         {"Market", BasePlatform.ActivePlatform.GetTargetAppStore().ToString()}
                    //     };
                    //     AppsFlyer.sendEvent("af_revenue_ir", appsflyerParameters);
                    // }
                    UnityEngine.Debug.Log("Logging revenue to other trackers : currency - " +  currency + " , price - " + price);
                }
                
                if (!PlayerProfileManager.Instance.ActiveProfile.FirstPurchaseDone)
                {
                    Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
                    {
                        {
                            Parameters.Itm,
                            purchaseResult.ProductID
                        },
                        {
                            Parameters.Currency,
                            currency
                        },
                        {
                            Parameters.Price,
                            price.ToString()
                        }
                    };
                    Log.AnEvent(Events.FirstTimePurchase, data);
                    PlayerProfileManager.Instance.ActiveProfile.FirstPurchaseDone = true;
                    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                }

                SavePurchaseOrder(orderID);
            }
        }
    }

    private bool IsDuplicatedPurchase(string orderID)
    {
        var purchasesString = PlayerPrefs.GetString("DUPLICATED_PURCHASES");

        if (!string.IsNullOrEmpty(purchasesString))
        {
            var purchases = purchasesString.Split("\n".ToCharArray());
            return purchases.Contains(orderID);
        }

        return false;
    }

    private void SavePurchaseOrder(string orderID)
    {
        var purchasesString = PlayerPrefs.GetString("DUPLICATED_PURCHASES",string.Empty);
        purchasesString += orderID + "\n";

        PlayerPrefs.SetString("DUPLICATED_PURCHASES", purchasesString);
        PlayerPrefs.Save();
    }

    private void LogRevenueToMetrix(string currencyCode, double price, string orderID, string productID)
    {
        //If inside country
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                if (BasePlatform.ActivePlatform.InsideCountry)
                {
                    if (currencyCode == "IRT")
                    {
                        // Convert toman to rial because metrix does not support toman
                        Metrix.NewRevenue("xweeu", price * 10, 0, orderID);
                    }
                    else if (currencyCode == "IRR")
                    {
                        Metrix.NewRevenue("xweeu", price, 0, orderID);
                    }
                }
                else
                {
                    var productPrice =
                        this._revenueTrackingConfiguration.Prices.FirstOrDefault(p => p.ProductID == productID);
                    if (productPrice != null)
                    {
                        //Find usd price of product because in some Third-Party Tracker like Metrix , We can only report usd unit for revenue.
                        price = productPrice.CADPrice * 0.78; //Convert CAD To USD
                        currencyCode = "USD";
                        Metrix.NewRevenue("xweeu", price, 1, orderID);
                    }
                }

                UnityEngine.Debug.Log("Logging revenue to metrix : currency - " +
                                      currencyCode +
                                      " , price - " + price);

                Metrix.NewEvent("elhmr");//has_purchased event to track paid users

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error on revenue log : " + e.Message);
            }
        }
    }

    private void LogRevenueToFlurry(string productID,double price,string currencyCode,string transactionID)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                Flurry.LogPayment(productID, productID, 1, price, currencyCode, transactionID,
                    new Dictionary<string, string>());
            }
            catch (Exception e)
            {

            }
        }
    }

    private void LogRevenueToGameAnalytics(string currency,double price,string orderID,string productID,string signature)
    {
        GameAnalytics.NewBusinessEventGooglePlay(currency, (int) price, "consumable", productID,
            "shop",
            orderID,
            signature);
    }
    
    private void LogRevenueToUnityAnalytics(string currency,double price,string orderID,string productID,string signature, string market, string receipt)
    {
        return;
        //revenue will be logged to unity analytics automatically
        
        string type = "";
        if (productID.ToLower().Contains("cash")) {
            type = "Cash";
        } else if (productID.ToLower().Contains("gold")) {
            type = "Gold";
        } else if (productID.ToLower().Contains("starter")) {
            type = "Starter Pack";
        } else if (productID.ToLower().Contains("gas")) {
            type = "Gas Tank";
        } else if (productID.ToLower().Contains("bundleoffer")) {
            type = "Bundle Offer Pack";
        } else if (productID.ToLower().Contains("megaoffer")) {
            type = "Mega Offer Pack";
        } else if (productID.ToLower().Contains("megaoffer")) {
            type = "Mega Offer Pack";
        } else {
            type = productID;
        }
        /*var productReceivedVirtualCurrencies = new List<Unity.Services.Analytics.Events.VirtualCurrency>()
        {
            new Unity.Services.Analytics.Events.VirtualCurrency()
            {
                virtualCurrencyName = "Goal", virtualCurrencyType = "PREMIUM", virtualCurrencyAmount = 100,
            }
        };*/

        var productReceivedItems = new List<Unity.Services.Analytics.Events.Item>()
        {
            new Unity.Services.Analytics.Events.Item()
            {
                itemName = productID, itemType = type, itemAmount = 1
            }
        };

        var productReceived = new Unity.Services.Analytics.Events.Product
        {
            items = productReceivedItems
            //,virtualCurrencies = productReceivedVirtualCurrencies
        };

        var productsSpent = new Unity.Services.Analytics.Events.Product()
        {
            // Transaction is in cents. This means the user spent $5.
            realCurrency = new Unity.Services.Analytics.Events.RealCurrency() {realCurrencyType = currency, realCurrencyAmount = Mathf.FloorToInt((float)(price*100))}
        };

        Unity.Services.Analytics.Events.Transaction( new Unity.Services.Analytics.Events.TransactionParameters()
            {
                productsReceived = productReceived,
                productsSpent = productsSpent,
                transactionID = orderID,
                transactionName = "IAP - " + productID,
                transactionType = Unity.Services.Analytics.Events.TransactionType.PURCHASE,
                transactionServer = Unity.Services.Analytics.Events.TransactionServer.GOOGLE,
                transactionReceipt = receipt,
                storeItemID =  productID,
                transactorID = UserManager.Instance.currentAccount != null? UserManager.Instance.currentAccount.UserID.ToString():"",
                storeID = market,
                productID = productID,
                transactionReceiptSignature = signature
            }
        );
    }

    /*private void LogRevenueToAppsFlyer(string currencyCode,double price)
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            try
            {
                Dictionary<string, string> purchaseEvent = new
                    Dictionary<string, string>();
                purchaseEvent.Add(AFInAppEvents.CURRENCY, currencyCode);
                purchaseEvent.Add(AFInAppEvents.REVENUE, price.ToString());
                purchaseEvent.Add(AFInAppEvents.QUANTITY, "1");
                //purchaseEvent.Add(AFInAppEvents.CONTENT_TYPE, "category_a");
                AppsFlyer.trackRichEvent("af_purchase", purchaseEvent);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Error sending revenue to appsflyer : " + e.Message);
            }
        }
    }*/
    
    private void LogRevenueToAppsFlyer(PurchaseResult.PurchaseReport report)
    {
        return;
        // if (Application.platform == RuntimePlatform.Android ||
        // Application.platform == RuntimePlatform.IPhonePlayer)
        // {
        //     try
        //     {
        //         #if UNITY_IOS
        //         MetricsIntegration.Instance.appsFlyerCallbacks.Report = report;
        //             AppsFlyer.validateReceipt(report.prodID, report.price, report.currency, report.transactionID, null);
        //         #elif UNITY_ANDROID
        //             Log.AnEvent(Events.AppsFlyerValidationStart, new Dictionary<Parameters, string>()
        //             {
        //                 {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
        //                 {Parameters.ProductID, report.prodID},
        //                 {Parameters.TransactionID, report.transactionID},
        //                 {Parameters.Signature, report.signature},
        //                 {Parameters.ErrorText, ""}
        //             });
        //             MetricsIntegration.Instance.ValidateReceiptWithAppsFlyer(report.prodID, report.price, report.currency, report.receipt);
        //             /*MetricsIntegration.Instance.appsFlyerCallbacks.Report = report;
        //             AppsFlyer.validateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiar/ADjUuPaprSw0rMQgqXyNepBWngSc23ycAJkbmcxYsNiC5LggFJhIjlY21Qb2cMG8rJcQRYnKIEWp7TqYchodULnYtqwCIGv+Wn+qJOwRm/ug7FHmocq4LgCoN6TvBSWD4zzOquWE0Yz0RC16b5X7Z9oxXzjy6miBCvVbARFNtcxVU86NtQ4i/onkjsKFVLSagPCpas29AcddEKxuCbO3dRhZsdE4KeW8IfSZ+LjqYbaK6Zy2P3T98ll1FeyjMq3GOaMH6l/TdAdfUmKHO7yV0qdKE3oy1PXAYnPfl/hpjmqGJ+EVPxz2+7OoExxo72+Uxtr0mUItmXAROW5Q2QIDAQAB",
        //                 report.purchaseData, report.signature, report.price, report.currency, null);
        //             AppsFlyer.createValidateInAppListener ("AppsFlyerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");*/
        //         #endif
        //     }
        //     catch (Exception e)
        //     {
        //         UnityEngine.Debug.Log("Error sending revenue to appsflyer : " + e.Message);
        //         Log.AnEvent(Events.AppsFlyerValidationFail, new Dictionary<Parameters, string>()
        //         {
        //             {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
        //             {Parameters.ProductID, report.prodID},
        //             {Parameters.TransactionID, report.transactionID},
        //             {Parameters.Signature, report.signature},
        //             {Parameters.ErrorText, e.Message}
        //         });
        //     }
        // }
    }
    
    private void LogRevenueToAppsMetrica(string productID, double price, string currencyCode, string orderID, string signature)
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            try
            {
                YandexAppMetricaRevenue rev = new YandexAppMetricaRevenue(price, currencyCode);
                rev.ProductID = productID;
                YandexAppMetricaReceipt receipt = new YandexAppMetricaReceipt();
                receipt.TransactionID = orderID;
                receipt.Signature = signature;
                rev.Quantity = 1;
                rev.Receipt = receipt;
                AppMetrica.Instance.ReportRevenue(rev);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Error sending revenue to appsmetrica : " + e.Message);
            }
        }
    }
}
