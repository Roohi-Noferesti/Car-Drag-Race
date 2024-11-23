using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio.IAB;
using KingKodeStudio.IAB.Zarinpal;
//using KingKodeStudio.IAB;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class GTQueryProvider : IZarinpalQueryProvider
{
    private Action<List<Purchase>> _queryPurchasesSucceedAction;
    private Action<List<SkuInfo>> querySkuDetailsSucceedAction;
    private Action<string> _failedAction;
    private Action<string> querySkuDetailsFailedAction;

    public void QueryPurchases(Action<List<Purchase>> succeedAction, Action<string> failedAction)
    {
        if (UserManager.Instance == null || UserManager.Instance.currentAccount == null ||
            string.IsNullOrEmpty(UserManager.Instance.currentAccount.Username))
        {
            return;
        }
        _queryPurchasesSucceedAction = succeedAction;
        _failedAction = failedAction;
        JsonDict parameters = new JsonDict();
        parameters.Set("username", UserManager.Instance.currentAccount.Username);
        WebRequestQueue.Instance.StartCall("acc_zarinpal_query_purchases", "query zarinpal purchase", parameters,
            QueryPurchaseResponse,
            null, ProduceHashSource(parameters));
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

    private void QueryPurchaseResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            var error = "error on zarinpal query purchases : " + zerror;

            if (zstatus != 0)
            {
                _failedAction?.Invoke(error);
            }
            else
            {
                _queryPurchasesSucceedAction?.Invoke(new List<Purchase>());
            }


            GTDebug.LogError(GTLogChannel.Account, error);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            
            var error = "error on zarinpal query purchases : server send malformed json in response";
            _failedAction?.Invoke(error);

            GTDebug.LogError(GTLogChannel.Account, error);
            return;
        }

        var records = parameters.GetObjectList<Purchase>("purchases", GetPurchasesRecord);

        _queryPurchasesSucceedAction?.Invoke(records);
    }

    private void GetPurchasesRecord(JsonDict jsondict, ref Purchase purchase)
    {
        purchase.SetProductID(jsondict.GetString("ProductID"));
        purchase.SetOrderID(jsondict.GetString("Authority"));
    }


    public void QuerySkuDetail(string[] skus, Action<List<SkuInfo>> succeedAction, Action<string> failedAction)
    {
        querySkuDetailsSucceedAction = succeedAction;
        querySkuDetailsFailedAction = failedAction;
        JsonDict parameters = new JsonDict();
        parameters.Set("username", "dummy_username");
        JsonDict productJson = new JsonDict();
        productJson.Set("expected_products", skus);
        parameters.Set("expected_products_json", productJson.ToString());
        WebRequestQueue.Instance.StartCall("rtw_get_products", "get all iap products", parameters,
            OnGetProductsResponse);
    }

    private void OnGetProductsResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            var error = "error getting products : " + zerror;
            GTDebug.LogError(GTLogChannel.RPBonus, error);
            //zstatus = 0 means Error due on reset queue and its not very important
            if (zstatus != 0)
            {
                querySkuDetailsFailedAction?.Invoke(error);
            }
            else
            {
                querySkuDetailsSucceedAction?.Invoke(new List<SkuInfo>());
            }
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(zhttpcontent))
        {
            var error = "error getting products : server send malformed json in response";
            GTDebug.LogError(GTLogChannel.RPBonus, error);
            querySkuDetailsFailedAction?.Invoke(error);
            return;
        }

        var products = parameters.GetObjectList<IapProduct>("products", GetIapProduct);

        var skuInfos = new List<SkuInfo>();

        foreach (var marketItem in products)
        {
            var price = marketItem.Value.ToString();
            var localisedPrice = GetLocalizedPrice(marketItem.Value);
//#if UNITY_EDITOR
//            var product =
//                GameDatabase.Instance.RevenueTrackingConfiguration.Prices.FirstOrDefault(r =>
//                    r.ProductID == marketItem.ID);
//            if (product != null)
//            {
//                price = string.Format("{0:C}$", product.CADPrice);
//                localisedPrice = price;
//            }
//#endif

            skuInfos.Add(new SkuInfo(GetProductLocalizedTitle(marketItem.ID), localisedPrice,
                GetProductLocalizedTitle(marketItem.ID), null, marketItem.ID, "IRT"));

        }

        querySkuDetailsSucceedAction?.Invoke(skuInfos);
    }

    private void GetIapProduct(JsonDict jsondict, ref IapProduct product)
    {
        product.ID = jsondict.GetString("id");
        product.Name = jsondict.GetString("name");
        product.Value = jsondict.GetInt("value");//Convert it to rials
        product.Available = jsondict.GetBool("avlb");
    }


    public string GetLocalizedPrice(int price)
    {
        return string.Format("تومان {0:n0}", price);
    }

    private string GetProductLocalizedTitle(string productID)
    {
        var count = productID.Split('_').Length;
        var codeElements = productID.Split('_');
        int result = 0;
        var isNumber = int.TryParse(codeElements[count-1], out result);
        if(count > 1 && !isNumber)
        {
            productID = productID.Remove(productID.LastIndexOf('_'));
        }
        return LocalizationManager.GetTranslation("TEXT_PRODUCT_" + productID);
    }
}
