using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RTLTMPro;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIBasicItemIDAssociator : Editor 
{
    [MenuItem("Tools/Associate IDs")]
    public static void UpdateIDs()
    {
        if (!EditorUtility.DisplayDialog("ID Association","Are you sure you want to associate IDS","Yes","No"))
        {
            return;
        }
        var obj = Selection.activeGameObject;
        int i = 0;
        foreach (var item in obj.GetComponentsInChildren<UIBasicItem>())
        {
            i++;
            item.ID = item.name;
        }
        EditorUtility.DisplayDialog("Operation done", i + " items renamed", "OK");
    }

    [MenuItem("Tools/Print Gameobjects Count")]
    public static void PrintGameObjectsCount()
    {
        var count = Resources.FindObjectsOfTypeAll<GameObject>().Count();
        //var count = FindObjectsOfType<GameObject>().Count();
        EditorUtility.DisplayDialog("Operation done", count + " items found", "OK");
    }


    [MenuItem("Tools/Font/Replace TextMeshPro")]
    public static void ReplaceTextMeshProAll()
    {
        if (!EditorUtility.DisplayDialog("Replace TextMeshPro", "Are you sure you want to Replace TextMeshPro", "Yes", "No"))
        {
            return;
        }

        var textsObjects = Selection.activeGameObject.GetComponent<TextMeshProUGUI>();

        ReplaceTextMeshPro(textsObjects);
    }


    [MenuItem("Tools/Fix CarParts Name")]
    public static void FixCarPartName()
    {
        if (Selection.gameObjects.Count() == 0)
            return;

        foreach (var obj in Selection.gameObjects)
        {
            foreach (Transform trans in obj.transform)
            {
                if (trans.name.ToLower().Contains("backlight"))
                {
                    trans.name = "BackLight_Projection";
                }
                else if (trans.name.ToLower().Contains("headlight"))
                {
                    trans.name = "HeadLight_Projection";
                }
                else if (trans.name.ToLower().Contains("shadow"))
                {
                    trans.name = "Shadow_Projection";
                }




                else if (trans.name.ToLower().Contains("body"))
                {
                    foreach (Transform t in trans)
                    {
                        if (t.name.ToLower().Contains("black"))
                            t.name = "Black_Shade";
                        else if (t.name.ToLower() == "glass")
                            t.name = "Glass";
                        else if (t.name.ToLower().Contains("chrome"))
                            t.name = "HeadLight_Chrome";
                        else if (t.name.ToLower()=="headlight_glass")
                            t.name = "HeadLight_Glass";
                        else if (t.name.ToLower().Contains("interior"))
                            t.name = "Interior";
                        else if (t.name.ToLower().Contains("texture"))
                            t.name = "Texture_Only";
                        else if (t.name.ToLower().Contains("glow"))
                            t.name = "Glow_Headlight";
                        else if (t.name.ToLower().Contains("spoiler"))
                            t.name = "Spoiler";
                        else if (t.name.ToLower().Contains("name"))
                            t.name = "Player_Name";
                    }
                }
            }

            //PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
            //if (prefabType == PrefabType.Prefab)
            //{
            //    var prefab = PrefabUtility.GetPrefabParent(obj);
            //    PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
            //}
        }

        
    }

    private static void ReplaceTextMeshPro(TextMeshProUGUI textMeshPro)
    {
        var obj = textMeshPro.gameObject;
        var fontSize = textMeshPro.fontSize;
        var fontColor = textMeshPro.color;
        var alightment = textMeshPro.alignment;
        var textContent = textMeshPro.text;
        var font = textMeshPro.font;
        var autoSize = textMeshPro.autoSizeTextContainer;
        var fontSizeMin = textMeshPro.fontSizeMin;
        var fontSizeMax = textMeshPro.fontSizeMax;
        DestroyImmediate(textMeshPro);
        var rtlTextMeshPro = obj.AddComponent<RTLTextMeshPro>();
        rtlTextMeshPro.fontSize = fontSize;
        rtlTextMeshPro.alignment = alightment;
        rtlTextMeshPro.color = fontColor;
        rtlTextMeshPro.text = textContent;
        rtlTextMeshPro.font = font;
        rtlTextMeshPro.autoSizeTextContainer = autoSize;
        rtlTextMeshPro.fontSizeMin = fontSizeMin;
        rtlTextMeshPro.fontSizeMax = fontSizeMax;
        EditorUtility.DisplayDialog("Operation done", " text replaced", "OK");
    }

    private static TextAlignmentOptions GetTextAlighmentOption(TextAnchor textAnchor)
    {
        switch (textAnchor)
        {
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
                case TextAnchor.MiddleCenter:
                return TextAlignmentOptions.Midline;
                case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.MidlineLeft;
                case TextAnchor.MiddleRight:
                return TextAlignmentOptions.MidlineRight;
                case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
                case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
        }
        return TextAlignmentOptions.Baseline;
    }


    [MenuItem("Assets/Create/CreateCarFolders")]
    public static void CreateCarFolders()
    {
        if (!EditorUtility.DisplayDialog("CreateCarFolders", "Are you sure you want to Create Car Folders", "Yes", "No"))
        {
            return;
        }
        CreateFolderIfNotExist("garage_model");
        CreateFolderIfNotExist("lightmap_garage");
        CreateFolderIfNotExist("lightmap_extra");
        //CreateFolderIfNotExist("logo");
        CreateFolderIfNotExist("spec");
        CreateFolderIfNotExist("spoiler");
        CreateFolderIfNotExist("sticker");
        CreateFolderIfNotExist("thumbnail");
    }

    [MenuItem("Tools/FixAllGearsError")]
    public static void FixAllGearsError()
    {
        if (!EditorUtility.DisplayDialog("FixAllGearsError", "Are you sure you want to FixAllGearsError", "Yes", "No"))
        {
            return;
        }
        EditorUtility.DisplayProgressBar("Fixing gears error", "please wait...", 0);
        float i = 0;
        var cars = Resources.LoadAll<CarInfo>("");
        foreach (var carInfo in cars)
        {
            SerializedObject so = new SerializedObject(carInfo);
            var tempCar = so.targetObject as CarInfo;
            tempCar.BaseGearBoxData.DebugChangeGearAtMPH = new float[tempCar.BaseGearBoxData.GearRatios.Length];
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(carInfo);
            i++;
            EditorUtility.DisplayProgressBar("Fixing gears error", "please wait...", i/cars.Length);
        }
        EditorUtility.ClearProgressBar();
    }


    [MenuItem("Tools/PrintBuggyCarList")]
    public static void PrintBuggyCarList()
    {
        EditorUtility.DisplayProgressBar("Loading cars", "please wait...", 0);
        var cars = Resources.LoadAll<CarInfo>("");
        var buggyCars = cars.Where(c => c.BaseEngineData.BaseTorqueCurve == null
                                || c.BaseTyreData.RPMVsExtraWheelSpinCurve == null
                                || c.BaseTyreData.WheelSpinGripCurve == null
                                || string.IsNullOrEmpty(c.Key));
        string list = "Buggy cars : " + buggyCars.Count();

        foreach (var carInfo in buggyCars)
        {
            list += "\n" + carInfo.name;
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Buggy cars", list, "Ok");
    }


	[MenuItem("Tools/FixBodyRolling")]
	public static void FixBodyRolling()
	{
        if (!EditorUtility.DisplayDialog("FixBodyRolling", "Are you sure you want to FixBodyRolling", "Yes", "No"))
        {
            return;
        }
		var transforms = Selection.transforms;

		foreach (Transform t in transforms) {
			var body = t.Find ("Body");

			var obj1 = t.Find ("Chrome");
			SetParent (obj1,body);

			var obj2 = t.Find ("Black_Shade");
			SetParent (obj2,body);

			var obj3 = t.Find ("Glass");
			SetParent (obj3,body);


			var obj4 = t.Find ("HeadLight_Chrome");
			SetParent (obj4,body);


			var obj5 = t.Find ("HeadLight_Glass");
			SetParent (obj5,body);


			var obj6 = t.Find ("Interior");
			SetParent (obj6,body);


			var obj7 = t.Find ("Texture_Only");
			SetParent (obj7,body);


			var obj8 = t.Find ("Glow_headlight (7)");
			SetParent (obj8,body);


			var obj9 = t.Find ("Glow_headlight (8)");
			SetParent (obj9,body);


			EditorUtility.SetDirty (t);
		}
	}

	private static void  SetParent(Transform child,Transform parent)
	{
		if (child == null || parent == null)
			return;

		child.SetParent (parent);
	}


    [MenuItem("Tools/AdjustRedLineRpm")]
    public static void AdjustRedLineRpm()
    {
        if (!EditorUtility.DisplayDialog("AdjustRedLineRpm", "Are you sure you want to AdjustRedLineRpm", "Yes", "No"))
        {
            return;
        }
        EditorUtility.DisplayProgressBar("Adjusting red line rpm", "please wait...", 0);
        float i = 0;
        var cars = Resources.LoadAll<CarInfo>("");
        foreach (var carInfo in cars)
        {
            SerializedObject so = new SerializedObject(carInfo);
            var tempCar = so.targetObject as CarInfo;
            if (tempCar.BaseEngineData.BaseTorqueCurve.animationCurve != null)
            {
                tempCar.BaseRedlineRPM = (int) tempCar.BaseEngineData.BaseTorqueCurve.animationCurve.keys.Last().time;
            }

            if (tempCar.BaseRedlineRPM < 1)
            {
                tempCar.BaseRedlineRPM = 6000;
            }

            if (tempCar.OptimalLaunchRPM < 1)
            {
                tempCar.OptimalLaunchRPM = Random.Range(3700, tempCar.BaseRedlineRPM - 700);
            }
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(carInfo);
            i++;
            EditorUtility.DisplayProgressBar("Fixing gears error", "please wait...", i / cars.Length);
        }
        EditorUtility.ClearProgressBar();
    }


    private static string GetSelectedPath()
    {

        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        return path;
    }


    private static string[] GetSelectedPathes()
    {
        var list = new List<string>();
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            list.Add(path);
        }
        return list.ToArray();
    }

    private static void CreateFolderIfNotExist(string folderName)
    {
        var selectedPathes = GetSelectedPathes();

        foreach (var selectedPath in selectedPathes)
        {
            var assetPath = Application.dataPath;
            assetPath = Directory.GetParent(assetPath).FullName;
            var finalPath = (assetPath + "/" + selectedPath + "/" + folderName).Replace("\\", "/");

            if (Directory.Exists(finalPath))
                return;

            AssetDatabase.CreateFolder(selectedPath, folderName);
        }
    }

    [MenuItem("Tools/Update Race Event Star Rewards")]
    private static void UpdateRaceEventStarRewards()
    {
        if (!EditorUtility.DisplayDialog("Update Race Event Star Rewards", "Are you sure you want to Update Race Event Star Rewards", "Yes", "No"))
        {
            return;
        }
        var cc = Resources.Load<CareerConfiguration>("shared_assets/configuration/CareerConfiguration");
        SetTierEventStarReward(cc.CareerRaceEvents.Tier1);
        SetTierEventStarReward(cc.CareerRaceEvents.Tier2);
        SetTierEventStarReward(cc.CareerRaceEvents.Tier3);
        SetTierEventStarReward(cc.CareerRaceEvents.Tier4);
        SetTierEventStarReward(cc.CareerRaceEvents.Tier5);

        EditorUtility.SetDirty(cc);
    }

    private static void SetTierEventStarReward(BaseCarTierEvents tierEvents)
    {
        SetTopLevelCategoryStarRewards(tierEvents.RegulationRaceEvents);
        SetTopLevelCategoryStarRewards(tierEvents.DailyBattleEvents);
        SetTopLevelCategoryStarRewards(tierEvents.CrewBattleEvents);
        SetTopLevelCategoryStarRewards(tierEvents.LadderEvents);
        SetTopLevelCategoryStarRewards(tierEvents.CarSpecificEvents);
        SetTopLevelCategoryStarRewards(tierEvents.ManufacturerSpecificEvents);
        SetTopLevelCategoryStarRewards(tierEvents.RestrictionEvents);
    }

    private static void SetTopLevelCategoryStarRewards(RaceEventTopLevelCategory levelCategory)
    {
        foreach (var g in levelCategory.RaceEventGroups)
        {
            foreach (var raceEventData in g.RaceEvents)
            {
                SetReward(raceEventData.RaceReward.RaceStarReward.RegularLeagueReward, 30, -8);
                SetReward(raceEventData.RaceReward.RaceStarReward.BronzeLeagueReward, 30, -11);
                SetReward(raceEventData.RaceReward.RaceStarReward.SilverLeagueReward, 30, -14);
                SetReward(raceEventData.RaceReward.RaceStarReward.GoldenLeagueReward, 30, -18);
                SetReward(raceEventData.RaceReward.RaceStarReward.DiamondLeagueReward, 30, -22);
            }
        }
    }

    private static void SetReward(LeagueStarReward reward, int win, int lose)
    {
        reward.WinStar = win;
        reward.LoseStar = lose;
    }


    public static void ResetAllPopups()
    {
        //var cc =
        //    Resources.Load<ProgressionPopupsConfiguration>("shared_assets/configuration/ProgressionPopupsConfiguration");

        //foreach (var popupStatusData in cc.PopupsData)
        //{

        //}

        //EditorUtility.SetDirty(cc);
    }


    [MenuItem("Tools/Reset All Popups")]
    private static void ResetAllPopupsByDialog()
    {
        if (!EditorUtility.DisplayDialog("ResetAllPopups", "Are you sure you want to ResetAllPopups", "Yes", "No"))
        {
            return;
        }
        ResetAllPopups();
        EditorUtility.DisplayDialog("Popup Reset", "All Popup has been Reset", "Ok");
    }


    [MenuItem("Tools/Log Zero Prices Cars")]
    private static void LogZeroPricesCar()
    {
        var pricesConfiguration = Resources.Load<PricesConfiguration>("shared_assets/configuration/PricesConfiguration");
        Debug.Log("Zero Prices car");
        foreach (var priceInfoDictionary in pricesConfiguration.CarsData)
        {
            var cash = priceInfoDictionary.PriceInfo.Cash;
            var gold = priceInfoDictionary.PriceInfo.Gold;
            if (cash == 0 || gold == 0)
            {
                Debug.Log(priceInfoDictionary.Name + " : gold " + gold + ",cash " + cash);
            }
        }


        Debug.Log("Zero Prices Upgrades");
        foreach (var priceInfoDictionary in pricesConfiguration.UpgradesData)
        {
            var cash = priceInfoDictionary.PriceInfo.Cash;
            var gold = priceInfoDictionary.PriceInfo.Gold;
            if (cash == 0 || gold == 0)
            {
                Debug.Log(priceInfoDictionary.Name + " : gold " + gold + ",cash " + cash);
            }
        }
    }


    [MenuItem("Tools/Reset All Cars Unlock Conditions")]
    private static void ResetAllCarsUnlockConditions()
    {
        if (!EditorUtility.DisplayDialog("ResetAllPopups", "Are you sure you want to Reset All Cars Unlock Conditions", "Yes", "No"))
        {
            return;
        }
        EditorUtility.DisplayProgressBar("Reseting conditions",
            "Reseting cars unlock conditions.Please wait cause we are gonna do a few things", 0.1F);
        ResetAllCarsUnlock();
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Popup Reset", "All Cars Unlock Conditions has been Reset", "Ok");
    }


    private static void ResetAllCarsUnlock()
    {
        var carinfos = Resources.LoadAll<CarInfo>("");
        var careerCnf = Resources.Load<CareerConfiguration>("shared_assets/configuration/CareerConfiguration");

        foreach (var carinfo in carinfos)
        {
            carinfo.UnlockEventIds.Clear();
            BaseCarTierEvents tierEvents = null;
            switch (carinfo.BaseCarTier)
            {
                case eCarTier.TIER_1:
                    break;
                case eCarTier.TIER_2:
                    tierEvents = careerCnf.CareerRaceEvents.Tier1;
                    break;
                case eCarTier.TIER_3:
                    tierEvents = careerCnf.CareerRaceEvents.Tier2;
                    break;
                case eCarTier.TIER_4:
                    tierEvents = careerCnf.CareerRaceEvents.Tier3;
                    break;
                case eCarTier.TIER_5:
                    tierEvents = careerCnf.CareerRaceEvents.Tier4;
                    break;
                //case eCarTier.TIER_6:
                //    tierEvents = careerCnf.CareerRaceEvents.Tier5;
                //    break;
            }

            if (tierEvents != null)
            {
                var lastCrewrace = tierEvents.CrewBattleEvents.RaceEventGroups[0].RaceEvents.Last();
                if (lastCrewrace != null)
                {
                    carinfo.UnlockEventIds.Add(lastCrewrace.EventID);
                }
            }
            EditorUtility.SetDirty(carinfo);
        }
    }


    [MenuItem("Tools/Update Star Rewards")]
    private static void UpdateStarRewardForAllEvents()
    {
        if (!EditorUtility.DisplayDialog("Update Star Rewards", "Are you sure you want to Update Star Rewards", "Yes", "No"))
        {
            return;
        }
        var careerConfig = AssetDatabase.LoadAssetAtPath<CareerConfiguration>("Assets/configuration/CareerConfiguration.asset");
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier1);
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier2);
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier3);
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier4);
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier5);
        UpdateStarRewardsForTierEvents(careerConfig.CareerRaceEvents.TierX);
        //careerConfig.

        EditorUtility.SetDirty(careerConfig);
        EditorUtility.DisplayDialog("Update Star Rewards", "All star rewards updated", "Ok");
    }


    [MenuItem("Tools/Reset XP Rewards")]
    private static void ResetXPRewardForAllEvents()
    {
        if (!EditorUtility.DisplayDialog("Reset XP Rewards", "Are you sure you want to Reset all event xp rewards to -1", "Yes", "No"))
        {
            return;
        }
        var careerConfig = AssetDatabase.LoadAssetAtPath<CareerConfiguration>("Assets/configuration/CareerConfiguration.asset");
        ResetXPRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier1);
        ResetXPRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier2);
        ResetXPRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier3);
        ResetXPRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier4);
        ResetXPRewardsForTierEvents(careerConfig.CareerRaceEvents.Tier5);
        //careerConfig.

        EditorUtility.SetDirty(careerConfig);
        EditorUtility.DisplayDialog("Reset XP Rewards", "All xp rewards updated to -1", "Ok");
    }


    [MenuItem("Tools/Update Price Configurations")]
    private static void UpdatePriceConfiguration()
    {
        if (!EditorUtility.DisplayDialog("Update Price Configurtion", "Are you sure you want to Update Price Configuration", "Yes", "No"))
        {
            return;
        }

        EditorUtility.DisplayProgressBar("Updating Prices...", "Loading configurations ", 0);
        var pricesConfiguration = Resources.Load<PricesConfiguration>("shared_assets/configuration/PricesConfiguration");

        var allCars = Resources.LoadAll<CarInfo>("");

        ServerItemDatabase serverItemDatabase = new ServerItemDatabase(true);
        serverItemDatabase.Initialise();

        pricesConfiguration.Clear();

        for (int i = 0; i < allCars.Length; i++)
        {
            var carInfo = allCars[i];
            var carPriceInfo = GetPriceInfo(serverItemDatabase, carInfo.Key);
            pricesConfiguration.CarsData.Add(carPriceInfo);

            foreach (var upg in carInfo.AvailableUpgradeData)
            {
                EditorUtility.DisplayProgressBar("Updaeting Prices...",
                    "Updaeting Price for " + carInfo.Key + " : " + upg.AssetDatabaseID
                    , (float) i/allCars.Length);
                var upgradePriceInfo = GetPriceInfo(serverItemDatabase, upg.AssetDatabaseID);
                pricesConfiguration.UpgradesData.Add(upgradePriceInfo);
            }

            var longPhraseDbKey = "_" + carInfo.Key;
            AddPaintItemPriceInfos(serverItemDatabase, pricesConfiguration, CarPropertyItemIDs.BodyShaderIDs, longPhraseDbKey, carInfo,
                allCars.Length, i);
            AddPaintItemPriceInfos(serverItemDatabase, pricesConfiguration, CarPropertyItemIDs.HeadlightShaderIDs, longPhraseDbKey, carInfo,
                allCars.Length, i);
            AddPaintItemPriceInfos(serverItemDatabase, pricesConfiguration, CarPropertyItemIDs.RingShaderIDs, longPhraseDbKey, carInfo,
                allCars.Length, i);
            AddPaintItemPriceInfos(serverItemDatabase, pricesConfiguration, CarPropertyItemIDs.SpoilerIDs, longPhraseDbKey, carInfo,
                allCars.Length, i);
            AddPaintItemPriceInfos(serverItemDatabase, pricesConfiguration, CarPropertyItemIDs.StickerIDs, longPhraseDbKey, carInfo,
                allCars.Length, i);
        }


        EditorUtility.SetDirty(pricesConfiguration);
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Update Price Completed", "All Price Info updated", "Ok");
    }
    
    [MenuItem("Tools/Copy Price Configurations")]
    private static void UpdatePriceConfigurationLocally()
    {
        if (!EditorUtility.DisplayDialog("Copy Price Configurtion", "Are you sure you want to Copy Price Configuration", "Yes", "No"))
        {
            return;
        }

        try
        {
            string fromCarID = "BentleyContinentalGTV8";
            string toCarID = "MercedesAMGG63";

            EditorUtility.DisplayProgressBar("Updating Prices...", "Loading configurations ", 0);
            var pricesConfiguration = Resources.Load<PricesConfiguration>("Temp/PricesConfiguration");

            //var allCars = Resources.LoadAll<CarInfo>("");
            for (int i=0 ; i<pricesConfiguration.LiveriesData.Count ; i++)
            {
                var liveryData = pricesConfiguration.LiveriesData[i];
                if (!liveryData.Name.Contains(fromCarID))
                    continue;

                var newLiveryData = liveryData;
                newLiveryData.Name = liveryData.Name.Replace(fromCarID, toCarID);
                
                pricesConfiguration.LiveriesData.Add(newLiveryData);
            }

            EditorUtility.SetDirty(pricesConfiguration);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Update Price Completed", "All Price Info updated", "Ok");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Update Price Failed", e.ToString(), "Ok");
        }
    }

    private static PriceInfoDictionary GetPriceInfo(ServerItemDatabase serverItemDatabase,string itemID)
    {
        var goldPirce = serverItemDatabase.GetGoldPrice(itemID);
        var cashPrice = serverItemDatabase.GetCashPrice(itemID);
        return new PriceInfoDictionary()
        {
            Name = itemID,
            PriceInfo = new PriceInfo()
            {
                Gold = goldPirce,
                Cash = cashPrice
            }
        };
    }

    private static void AddPaintItemPriceInfos(ServerItemDatabase serverItemDatabase,
        PricesConfiguration pricesConfiguration,
        string[] itemIDs, string longPhraseDbKey, CarInfo carInfo,
        int length, int i)
    {
        foreach (var bodyShaderID in itemIDs)
        {
            var currectID = bodyShaderID + longPhraseDbKey;
            EditorUtility.DisplayProgressBar("Updaeting Prices...",
                "Updaeting Price for " + carInfo.Key + " : " + currectID
                , (float) i/length);
            var paintPriceInfo = GetPriceInfo(serverItemDatabase, currectID);
            pricesConfiguration.LiveriesData.Add(paintPriceInfo);
        }
    }


    private static void ResetXPRewardsForTierEvents(BaseCarTierEvents tierEvent)
    {
        ResetXPRewardsForTopLevelcategory(tierEvent.CrewBattleEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.DailyBattleEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.LadderEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.RegulationRaceEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.WorldTourRaceEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.ManufacturerSpecificEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.RestrictionEvents);
        ResetXPRewardsForTopLevelcategory(tierEvent.CarSpecificEvents);
    }


    private static void ResetXPRewardsForTopLevelcategory(RaceEventTopLevelCategory topLevelCategory)
    {
        foreach (var raceEventGroup in topLevelCategory.RaceEventGroups)
        {
            foreach (var raceEventData in raceEventGroup.RaceEvents)
            {
                raceEventData.RaceReward.XPPrize = -1;
            }
        }
    }

    private static void UpdateStarRewardsForTierEvents(BaseCarTierEvents tierEvent)
    {
        UpdateStarRewardsForTopLevelcategory(tierEvent.CrewBattleEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.DailyBattleEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.LadderEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.RegulationRaceEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.WorldTourRaceEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.ManufacturerSpecificEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.RestrictionEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.CarSpecificEvents);
        UpdateStarRewardsForTopLevelcategory(tierEvent.WorldTourRaceEvents);
    }

    private static void UpdateStarRewardsForTopLevelcategory(RaceEventTopLevelCategory topLevelCategory)
    {
        //var isDaily = topLevelCategory is DailyBattleEvents;
        //var isLadder = topLevelCategory is LadderEvents;
        //var isCrew = topLevelCategory is CrewBattleEvents;
        //var isTournament = topLevelCategory is TournamentRaceEvents;
        foreach (var raceEventGroup in topLevelCategory.RaceEventGroups)
        {
            int i = 0;
            foreach (var raceEventData in raceEventGroup.RaceEvents)
            {
                if (true)//isRegular || isDaily || isLadder || isCrew)
                {
                    var isRegular = topLevelCategory is RegulationRaceEvents || raceEventGroup.RaceEvents.Count==3;

                    var winReward = isRegular ? i == 0 ? 20 : i == 1 ? 30 : 40 : 30;
                    //regular
                    raceEventData.RaceReward.RaceStarReward.RegularLeagueReward.LoseStar = -15;
                    raceEventData.RaceReward.RaceStarReward.RegularLeagueReward.WinStar = winReward;

                    //Bronze
                    raceEventData.RaceReward.RaceStarReward.BronzeLeagueReward.LoseStar = -18;
                    raceEventData.RaceReward.RaceStarReward.BronzeLeagueReward.WinStar = winReward;

                    //Silver
                    raceEventData.RaceReward.RaceStarReward.SilverLeagueReward.LoseStar = -21;
                    raceEventData.RaceReward.RaceStarReward.SilverLeagueReward.WinStar = winReward;

                    //Gold
                    raceEventData.RaceReward.RaceStarReward.GoldenLeagueReward.LoseStar = -24;
                    raceEventData.RaceReward.RaceStarReward.GoldenLeagueReward.WinStar = winReward;

                    //Diamond
                    raceEventData.RaceReward.RaceStarReward.DiamondLeagueReward.LoseStar = -27;
                    raceEventData.RaceReward.RaceStarReward.DiamondLeagueReward.WinStar = winReward;
                }
                //else if(isLadder)
                //{
                //    //regular
                //    raceEventData.RaceReward.RaceStarReward.RegularLeagueReward.LoseStar = -15;
                //    raceEventData.RaceReward.RaceStarReward.RegularLeagueReward.WinStar = 30;

                //    //Bronze
                //    raceEventData.RaceReward.RaceStarReward.BronzeLeagueReward.LoseStar = -18;
                //    raceEventData.RaceReward.RaceStarReward.BronzeLeagueReward.WinStar = 30;

                //    //Silver
                //    raceEventData.RaceReward.RaceStarReward.SilverLeagueReward.LoseStar = -21;
                //    raceEventData.RaceReward.RaceStarReward.SilverLeagueReward.WinStar = 30;

                //    //Gold
                //    raceEventData.RaceReward.RaceStarReward.GoldenLeagueReward.LoseStar = -24;
                //    raceEventData.RaceReward.RaceStarReward.GoldenLeagueReward.WinStar = 30;

                //    //Diamond
                //    raceEventData.RaceReward.RaceStarReward.DiamondLeagueReward.LoseStar = -27;
                //    raceEventData.RaceReward.RaceStarReward.DiamondLeagueReward.WinStar = 30;
                //}
                //else
                //{
                    
                //}
                i++;
            }
        }
    }
}
