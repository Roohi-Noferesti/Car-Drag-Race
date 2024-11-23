using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PurchasableItems;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(BundleOffersPopUpConfiguration))]
public class BundleOffersPopUpConfigurationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load From json"))
        {
            var filePath = EditorUtility.OpenFilePanel("BundleOffers Json", Application.persistentDataPath, "txt");

            if (!string.IsNullOrEmpty(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loadedBundleOffer = JsonConverter.DeserializeObject<BundleOffersPopUpConfiguration>(json);
                if (loadedBundleOffer != null)
                {
                    var bundleOfferConfig = target as BundleOffersPopUpConfiguration;

                    bundleOfferConfig.NumberOfSessionsBetweenOfferShown = loadedBundleOffer.NumberOfSessionsBetweenOfferShown;
                    bundleOfferConfig.OneTimeOffers = loadedBundleOffer.OneTimeOffers;
                    bundleOfferConfig.RepeatOfferSettings = loadedBundleOffer.RepeatOfferSettings;
                    bundleOfferConfig.RepeatableOffers = loadedBundleOffer.RepeatableOffers;

                    EditorUtility.SetDirty(target);
                }
            }
        }



        if (GUILayout.Button("validate currency pack"))
        {
            var bundleOfferConfig = target as BundleOffersPopUpConfiguration;

            VlidateCurrencypacks(bundleOfferConfig.OneTimeOffers);
            VlidateCurrencypacks(bundleOfferConfig.RepeatableOffers);

            EditorUtility.SetDirty(target);
        }


        if (GUILayout.Button("Sync with IAP"))
        {

            SyncwithIAP();

            EditorUtility.SetDirty(target);
        }


        base.OnInspectorGUI();
    }

    private void VlidateCurrencypacks(List<BundleOfferData>  offers)
    {
        foreach (var bundleOfferData in offers)
        {
            foreach (var bundleOfferWidgetInfo in bundleOfferData.PopupData.WidgetInfo)
            {
                if (!bundleOfferWidgetInfo.ShopItem.Contains("pack"))
                {
                    bundleOfferWidgetInfo.ShopItem = bundleOfferWidgetInfo.ShopItem.Replace("cash", "cash_pack_");
                    bundleOfferWidgetInfo.ShopItem = bundleOfferWidgetInfo.ShopItem.Replace("gold", "gold_pack_");
                }
            }
        }
    }



    private void SyncwithIAP()
    {
        var bundleOfferConfig = target as BundleOffersPopUpConfiguration;
        var iapConfig = AssetDatabase.LoadAssetAtPath<IAPConfiguration>("Assets/configuration/IAPConfiguration.asset");
        var inAppPurchases = AssetDatabase.LoadAssetAtPath<InAppPurchases>("Assets/configuration/InAppPurchases.asset");

        foreach (var bundleOfferData in bundleOfferConfig.OneTimeOffers)
        {
            var bundleOfferItem = bundleOfferData.PopupData.BundleOfferItem;
            var starterPackItem1 = bundleOfferData.PopupData.StarterPackItem1;
            var starterPackItem2 = bundleOfferData.PopupData.StarterPackItem2;

            GTProduct product = inAppPurchases.RetrieveFirstPurchaseItem(bundleOfferItem);
            var widgets = bundleOfferData.PopupData.WidgetInfo.Where(w => w.OfferType != "CAR").ToArray();
            GTProduct[] packProducts = new GTProduct[2+widgets.Length];
            packProducts[0] = inAppPurchases.RetrieveFirstPurchaseItem(starterPackItem1);
            packProducts[1] = inAppPurchases.RetrieveFirstPurchaseItem(starterPackItem2);

            for (int i = 0; i < widgets.Length; i++)
            {
                packProducts[i+2] =
                    inAppPurchases.RetrieveFirstPurchaseItem(widgets[i].ShopItem);
            }
            if (product == null)
            {
                var newLength = inAppPurchases.GetProductsListLength() + 1;
                var abTestReadyGtProducts = inAppPurchases.ABTestReadyGtProducts;
                Array.Resize(ref abTestReadyGtProducts, newLength);
                product = new GTProduct()
                {
                    Code = bundleOfferItem
                };

                inAppPurchases.ABTestReadyGtProducts[newLength-1] = product;
            }

            product.Gold = 0;
            product.Cash = 0;
            product.BonusCash = 0;
            product.BonusGold = 0;
            product.NumSuperNitros = 0;
            product.NumRaceCrew = 0;
            foreach (var packProduct in packProducts)
            {
                if (packProduct != null)
                {
                    product.Gold += packProduct.Gold;
                    product.Cash += packProduct.Cash;
                    product.BonusCash += packProduct.BonusCash;
                    product.BonusGold += packProduct.BonusGold;
                    product.NumSuperNitros += packProduct.NumSuperNitros;
                    product.NumRaceCrew += packProduct.NumRaceCrew;
                }
            }


            foreach (var bundleOfferWidgetInfo in bundleOfferData.PopupData.WidgetInfo)
            {
                if (bundleOfferWidgetInfo.OfferType == "CAR")
                {
                    //GTProduct carProduct =
                    //    inAppPurchases.GTProducts.FirstOrDefault(
                    //        gtProduct => gtProduct.Code == bundleOfferWidgetInfo.ShopItem);

                    //if (carProduct == null)
                    //{
                    //    carProduct = new GTProduct()
                    //    {
                    //        Code = bundleOfferWidgetInfo.CarDBKey
                    //    };
                    //    var newLength = inAppPurchases.GTProducts.Length + 1;
                    //    Array.Resize(ref inAppPurchases.GTProducts, newLength);
                    //    inAppPurchases.GTProducts[newLength - 1] = carProduct;
                    //}

                    if (string.IsNullOrEmpty(bundleOfferWidgetInfo.CarDBKey))
                    {
                        GTDebug.Log(GTLogChannel.CarDatabase,"CarDBKey of widget car for '" + bundleOfferData.PopupData.BundleOfferItem +
                                                                 "' is null.ignore it");
                        continue;
                    }
                    var purchasableItem =
                        iapConfig.PurchasableItems.FirstOrDefault(i => i.Value == bundleOfferWidgetInfo.CarDBKey);

                    if (purchasableItem == null)
                    {
                        purchasableItem = new PurchasableItem()
                        {
                            IAPCode = bundleOfferData.PopupData.BundleOfferItem,
                            Value = bundleOfferWidgetInfo.CarDBKey,
                            Type = PurchasableItem.ProductType.BundleCar,
                            IsAvailable = true
                        };
                        iapConfig.PurchasableItems.Add(purchasableItem);
                    }
                    else
                    {
                        purchasableItem.IAPCode = bundleOfferData.PopupData.BundleOfferItem;
                        purchasableItem.Value = bundleOfferWidgetInfo.CarDBKey;
                        purchasableItem.Type = PurchasableItem.ProductType.BundleCar;
                        purchasableItem.IsAvailable = true;
                    }
                    break;
                }
            }
        }
    }
}
