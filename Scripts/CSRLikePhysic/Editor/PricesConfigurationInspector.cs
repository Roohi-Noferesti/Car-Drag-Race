using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(PricesConfiguration))]
public class PricesConfigurationInspector : Editor
{
    
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<PricesConfiguration>(target, (obj) =>
            {
                (obj as PricesConfiguration).OnAfterDeserialization();
                EditorUtility.SetDirty(obj);
            });
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<PricesConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((PricesConfiguration) target);
        }
        
        
        if (GUILayout.Button("Copy Bugatti Chiron"))
        {
            Undo.RegisterCompleteObjectUndo(target, "Bugatti Chiron Upgrade Prices");

            var prices = (PricesConfiguration) target;
            var bugattiUpgrades = prices.UpgradesData.Where(u => u.Name.Contains("Bugatti")).ToArray();
            foreach (var bugattiUpg in bugattiUpgrades)
            {
                var newUpgrade = new PriceInfo()
                {
                    Gold = (int) (bugattiUpg.PriceInfo.Gold * 1.1F),
                    Cash = (int) (bugattiUpg.PriceInfo.Cash * 1.1F)
                };
                var key = bugattiUpg.Name.Replace("BugattiVeyronVitesse","Bugatti_ChironReward_2016");
                prices.UpgradesData.Add(new PriceInfoDictionary() {Name = key, PriceInfo = newUpgrade});
            }

            EditorUtility.SetDirty(target);
        }
        
        
        if (GUILayout.Button("Make Goldx10"))
        {
            var prices = (PricesConfiguration)target;
            
            Undo.RegisterCompleteObjectUndo(target,"Goldx10");
            for (var i = 0; i < prices.CarsData.Count; i++)
            {
                var cars = prices.CarsData[i];
                cars.PriceInfo.Gold *= 10;
            }

            foreach (var upgrade in prices.UpgradesData)
            {
                upgrade.PriceInfo.Gold *= 10;
            }
            
            foreach (var livery in prices.LiveriesData)
            {
                livery.PriceInfo.Gold *= 10;
            }
            
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Add old liveries"))
        {
            var prices = CopyConfigUtils.LoadFromFile<PricesConfiguration>();

            if (prices != null)
            {
                Undo.RegisterCompleteObjectUndo(target,"Add liveries");
                var destination = target as PricesConfiguration;
                destination.LiveriesData.AddRange(prices.LiveriesData);
                EditorUtility.SetDirty(destination);
            }
        }


        if (GUILayout.Button("Copy CSR 5 Upgrades"))
        {
            var prices = CopyConfigUtils.LoadFromFile<PricesConfiguration>();

            if (prices != null)
            {
                Undo.RegisterCompleteObjectUndo(target, "Copy Upgrades");
                var destination = target as PricesConfiguration;
                foreach (var pricesUpgrade in prices.Upgrades)
                {
                    if (!destination.UpgradesData.Any(u => u.Name == pricesUpgrade.Key))
                    {
                        destination.UpgradesData.Add(new PriceInfoDictionary()
                        {
                            Name = pricesUpgrade.Key,
                            PriceInfo = pricesUpgrade.Value
                        });
                    }

                }
                EditorUtility.SetDirty(destination);
            }
        }


        //        if (GUILayout.Button("AddOrUpdateCarItemsPrices"))
        //        {
        //            EditorUtility.DisplayProgressBar("Updating prices", "Loading cars...", 0);
        //            var carInfos = Resources.LoadAll<CarInfo>("");
        //            var prices = target as PricesConfiguration;
        //            float i = 0;
        //            foreach (var carInfo in carInfos)
        //            {
        //                EditorUtility.DisplayProgressBar("Updating prices", "Updating car : " + carInfo.Key, i / carInfos.Length);
        //                foreach (var carUpgradeData in carInfo.AvailableUpgradeData)
        //                {
        //
        //                    var cashPrice = 0F;
        //                    if (carInfo.PPIndex > 0)
        //                    {
        //                        cashPrice = Mathf.Pow(carInfo.PPIndex, 1.2F);
        //                    }
        //                    switch (carUpgradeData.UpgradeType)
        //                    {
        //                        case eUpgradeType.BODY:
        //                            cashPrice *= 1.3F;
        //                            break;
        //                        case eUpgradeType.ENGINE:
        //                            cashPrice *= 2.25F;
        //                            break;
        //                        case eUpgradeType.INTAKE:
        //                            cashPrice *= 2.25F;
        //                            break;
        //                        case eUpgradeType.NITROUS:
        //                            cashPrice *= 2F;
        //                            break;
        //                        case eUpgradeType.TRANSMISSION:
        //                            cashPrice *= 2F;
        //                            break;
        //                        case eUpgradeType.TURBO:
        //                            cashPrice *= 2.5F;
        //                            break;
        //                        case eUpgradeType.TYRES:
        //                            cashPrice *= 1.15F;
        //                            break;
        //                    }
        //
        //                    switch (carInfo.BaseCarTier)
        //                    {
        //                        case eCarTier.TIER_1:
        //                            cashPrice *= 1.2F;
        //                            break;
        //                        case eCarTier.TIER_2:
        //                            cashPrice *= 1.3F;
        //                            break;
        //                        case eCarTier.TIER_3:
        //                            cashPrice *= 1.9F;
        //                            break;
        //                        case eCarTier.TIER_4:
        //                            cashPrice *= 3F;
        //                            break;
        //                        case eCarTier.TIER_5:
        //
        //                            if (carInfo.Key == "car_lamborghini_veneno"
        //                            || carInfo.Key == "car_lamborghini_aventador" ||
        //                                carInfo.Key == "car_mclaren_650s_gt3")
        //                            {
        //                                cashPrice *= 9.4F;
        //                            }
        //                            else if (carInfo.PPIndex > 700 && carInfo.Key != "car_ferrari_laferrari")
        //                            {
        //                                cashPrice *= 8.5F;
        //                            }
        //                            else if (carInfo.Key == "car_ferrari_laferrari") //This car is final car. So it should be hard to upgrade
        //                            {
        //                                cashPrice *= 22.8f;
        //                            }
        //                            else
        //                                cashPrice *= 5F;
        //
        //                            break;
        //
        //                    }
        //
        //                    switch (carUpgradeData.UpgradeLevel)
        //                    {
        //                        case 1:
        //                            cashPrice *= 1F;
        //                            break;
        //                        case 2:
        //                            cashPrice *= 2F;
        //                            break;
        //                        case 3:
        //                            cashPrice *= 3F;
        //                            break;
        //                        case 4:
        //                            cashPrice *= 5F;
        //                            break;
        //                        case 5:
        //                            cashPrice *= 7F;
        //                            break;
        //                    }
        //
        //                    var goldPrice = 0F;
        //                    switch (carInfo.Tier)
        //                    {
        //                        case eCarTier.TIER_1:
        //                            goldPrice = cashPrice * 0.004F;
        //                            break;
        //                        case eCarTier.TIER_2:
        //                            goldPrice = cashPrice * 0.0037F;
        //                            break;
        //                        case eCarTier.TIER_3:
        //                            goldPrice = cashPrice * 0.0035F;
        //                            break;
        //                        case eCarTier.TIER_4:
        //                            goldPrice = cashPrice * 0.0033F;
        //                            break;
        //                        case eCarTier.TIER_5:
        //
        //                            if (carInfo.Key == "car_lamborghini_veneno" || carInfo.Key == "car_lamborghini_aventador" || carInfo.Key == "car_mclaren_650s_gt3")
        //                            {
        //                                goldPrice = cashPrice * 0.0028F;
        //                                // Amin should change this one
        //                            }
        //                            else if (carInfo.PPIndex > 700)
        //                            {
        //                                goldPrice = cashPrice * 0.0028F;
        //                                // Amin should change this one
        //                            }
        //                            else
        //                                goldPrice = cashPrice * 0.003F;
        //
        //                            break;
        //                    }
        //
        //                    AddOrUpdatePrice(ref prices.UpgradesData, carUpgradeData.AssetDatabaseID, (int)cashPrice,
        //                        (int)goldPrice);
        //                }
        //
        //                var gold = 0;
        //                var cash = 0;
        //
        //                for (int j = 0; j < 18; j++)
        //                {
        //                    var itemID = "CarBody_Custom_" + j + "_" + carInfo.Key;
        //                    if (j == 0 || j == 2 || j == 7)
        //                    {
        //                        cash = 0;
        //                        gold = 100;
        //                    }
        //                    else if (j == 4)
        //                    {
        //                        cash = 0;
        //                        gold = 80;
        //                    }
        //                    else if (j == 5 || j == 8 || j == 14 || j == 16)
        //                    {
        //                        cash = 4000;
        //                        gold = 60;
        //                    }
        //                    else
        //                    {
        //                        cash = 3000;
        //                        gold = 50;
        //                    }
        //
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //                }
        //
        //
        //                for (int j = 1; j < 23; j++)
        //                {
        //                    var itemID = "CarBody_Matte_" + j + "_" + carInfo.Key;
        //                    cash = 700;
        //                    gold = 25;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //                for (int j = 1; j < 22; j++)
        //                {
        //                    var itemID = "CarBody_Simple_" + j + "_" + carInfo.Key;
        //                    cash = 600;
        //                    gold = 20;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //                for (int j = 1; j <= 8; j++)
        //                {
        //                    var itemID = "HeadLight_" + j + "_" + carInfo.Key;
        //                    cash = 500;
        //                    gold = 20;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //                for (int j = 1; j <= 10; j++)
        //                {
        //                    var itemID = "Ring_" + j + "_" + carInfo.Key;
        //                    cash = 350;
        //                    gold = 15;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //                for (int j = 0; j < 58; j++)
        //                {
        //                    var itemID = "Sticker_" + j + "_" + carInfo.Key;
        //                    if (j == 19)
        //                    {
        //                        j = 40;
        //                    }
        //                    cash = 600;
        //                    gold = 20;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //
        //                for (int j = 1; j <= 7; j++)
        //                {
        //                    var itemID = "Spoiler_" + j + "_" + carInfo.Key;
        //                    cash = 850;
        //                    gold = 30;
        //                    AddOrUpdatePrice(ref prices.LiveriesData, itemID, cash, gold);
        //
        //                }
        //
        //                i++;
        //            }
        //
        //            EditorUtility.SetDirty(target);
        //            EditorUtility.ClearProgressBar();
        //            EditorUtility.DisplayDialog("Updating done", "Updating prices completed", "Ok");
        //        }
        //
        //        if (GUILayout.Button("Zero gold prices"))
        //        {
        //            if (EditorUtility.DisplayDialog("Reset gold prices"
        //                , "Do you want to set all gold prices to zero for upgrades from 1 to 3", "Yes", "No"))
        //            {
        //                var prices = target as PricesConfiguration;
        //
        //                foreach (var upgrade in prices.UpgradesData)
        //                {
        //                    if (upgrade.Name.Contains("_1_") || upgrade.Name.Contains("_2_")
        //                        || upgrade.Name.Contains("_3_"))
        //                    {
        //                        upgrade.PriceInfo.Gold = 0;
        //                    }
        //                }
        //
        //                EditorUtility.SetDirty(prices);
        //
        //                EditorUtility.DisplayDialog("Done", "Reseting gold prices for upgrades completed", "Ok");
        //            }
        //        }

        base.OnInspectorGUI();

    }

    private void AddOrUpdatePrice(ref List<PriceInfoDictionary> priceInfo, string itemID, int cash, int gold)
    {

        var priceDic = priceInfo.FirstOrDefault(i => i.Name == itemID);
        if (priceDic == null)
        {
            priceDic = new PriceInfoDictionary();
            priceInfo.Add(priceDic);
        }

        priceDic.Name = itemID;
        priceDic.PriceInfo = new PriceInfo()
        {
            Cash = cash,
            Gold = gold
        };
    }
}
