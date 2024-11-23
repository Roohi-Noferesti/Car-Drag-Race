using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

[Serializable]
public class CarInfo : ScriptableObject,ICarSimpleSpec
{
    public static string[] DriveWheelsString = new string[]
    {
        "TEXT_FRONT_WHEEL_DRIVE",
        "TEXT_REAR_WHEEL_DRIVE",
        "TEXT_ALL_WHEEL_DRIVE"
    };

    //[HideInInspector]
    public string Key;

    public bool Available = true;


    [HideInInspector] public List<string> EvolutionSequences;

    public string ManufacturerID = "None";

    //[HideInInspector]
    public eAudioEngineType CarEngineSound = eAudioEngineType.ENGINE_4_CYLINDER;

    [HideInInspector] public int CarNumberID;

    [HideInInspector] public string ManufacturerName = "FillIn";

    [HideInInspector] public string DefaultLiveryBundleName = string.Empty;

    [HideInInspector] public string EliteOverrideCarAssetID = string.Empty;

    //[HideInInspector]
    public eDriveType DriveType;

    public string ModelPrefabString;

    [HideInInspector]
    public eCarTier BaseCarTier;

    [HideInInspector]
    public int BasePerformanceIndex;

    [HideInInspector, SerializeField] 
    public int FlyWheelPower = 1;

    public bool IsAvailableInShowroomAndUpgrade = true;
    public bool IsAvailableInShowroomOnlyInsideCountry = false;

    public int OrderInShowroom;

    [HideInInspector] public int CarUnlocksInSeason;

    [HideInInspector] public int IsPrizeInSeason;

    [HideInInspector] public int AppearsInMultiplayerAtSeason;

    [HideInInspector] public List<string> UnlockDependencies;

    [HideInInspector] public List<string> UnlockEvents;

    public List<int> UnlockEventIds;

    [HideInInspector] public float MechanicEngineFettleGraphSlope;

    [HideInInspector] public float MechanicEngineFettleGraphIntercept;

    [HideInInspector] public float MechanicTyreGripFettleGraphSlope;

    [HideInInspector] public float MechanicTyreGripFettleGraphIntercept;

    [HideInInspector] public float EngineConsumableEngineGraphSlope;

    [HideInInspector] public float EngineConsumableEngineGraphIntercept;

    [HideInInspector] public float EngineConsumableTyreGraphSlope;

    [HideInInspector] public float EngineConsumableTyreGraphIntercept;

    [HideInInspector] public float NitrousConsumableBodyGraphSlope;

    [HideInInspector] public float NitrousConsumableBodyGraphIntercept;

    [HideInInspector] public float TyreConsumableEngineGraphSlope;

    [HideInInspector] public float TyreConsumableEngineGraphIntercept;

    [HideInInspector] public float TyreConsumableTyreGraphSlope;

    [HideInInspector] public float TyreConsumableTyreGraphIntercept;

    //[HideInInspector]
    public int BaseRedlineRPM;

    //[HideInInspector]
    public int OptimalLaunchRPM;

    [HideInInspector] public float TransmissionLoss;

    //[HideInInspector]
    public EngineData BaseEngineData;

    //[HideInInspector]
    public ChassisData BaseChassisData;

    //[HideInInspector]
    public GearBoxData BaseGearBoxData;

    //[HideInInspector]
    public TireData BaseTyreData;

    //[HideInInspector]
    public NitrousData BaseNitrousData;

    //[HideInInspector]
    public CarUpgradeData[] AvailableUpgradeData;

    //[HideInInspector]
    public PredefinedUpgradeSetsData[] PredefinedUpgradeSets;

    public int[] Stickers;

    public string PrizeInfoText = string.Empty;

    public string PrizeInfoButton = string.Empty;

    public bool IsWorldTourWinnableCar;

    public bool IsWorldTour;

    public bool IsInternationalCar;

    public bool IsBossCarOverride;

    public string BodyShaderOverride;
    public string HeadlightShaderOverride;
    public string RingShaderOverride;
    public string StickerOverride;
    public string SpoilerOverride;


    [HideInInspector] public string CarVersionNumber = "1.0";

    [HideInInspector] public int GarageWheelRotation = 22;

    [HideInInspector]
    public int BuyPrice
    {
        get
        {
            return GameDatabase.Instance.Prices.GetCarCashPrice(this.Key);
        }
    }

    [HideInInspector]
    public int GoldPrice
    {
        get
        {
            return GameDatabase.Instance.Prices.GetCarGoldPrice(this.Key);
        }
    }

    public string LongName
    {
        get
        {
            try
            {
                return "TEXT_CAR_" + this.Key + GameDatabase.Instance.CarsConfiguration.postfix + "_LONG";
            }
            catch
            {
                return "TEXT_CAR_" + this.Key + "_LONG";
            }
        }
        private set { }
    }

    public string MediumName
    {
        get
        {
            try
            {
                return "TEXT_CAR_" + this.Key + GameDatabase.Instance.CarsConfiguration.postfix + "_MEDIUM";
            }
            catch
            {
                return "TEXT_CAR_" + this.Key + "_MEDIUM";
            }
        }
        private set { }
    }

    public string ShortName
    {
        get
        {
            try
            {
                return "TEXT_CAR_" + this.Key + GameDatabase.Instance.CarsConfiguration.postfix + "_SHORT";
            }
            catch
            {
                return "TEXT_CAR_" + this.Key + "_SHORT";
            }
        }
        private set { }
    }

    public CarInfo()
    {
        this.BaseEngineData = new EngineData();
        this.BaseChassisData = new ChassisData();
        this.BaseGearBoxData = new GearBoxData();
        this.BaseTyreData = new TireData();
        this.BaseNitrousData = new NitrousData();
        this.AvailableUpgradeData =
            new CarUpgradeData[CarUpgrades.ValidUpgrades.Count*(int) CarUpgradeData.NUM_UPGRADE_LEVELS];

        ResetUpgrades();
    }

    public void ResetUpgrades()
    {
        if (string.IsNullOrEmpty(Key))
            return;
        if (AvailableUpgradeData == null ||
            AvailableUpgradeData.Length < CarUpgrades.ValidUpgrades.Count*CarUpgradeData.NUM_UPGRADE_LEVELS)
            AvailableUpgradeData = new CarUpgradeData[CarUpgrades.ValidUpgrades.Count*CarUpgradeData.NUM_UPGRADE_LEVELS];
        for (int i = 0, k = 0; i < CarUpgrades.ValidUpgrades.Count; i++)
        {
            for (int j = 0; j < CarUpgradeData.NUM_UPGRADE_LEVELS; j++, k++)
            {
                if (AvailableUpgradeData[k] == null)
                    this.AvailableUpgradeData[k] = new CarUpgradeData();

                var assetID = Key.Replace("car_", "_");
                switch (CarUpgrades.ValidUpgrades[i])
                {
                    case eUpgradeType.BODY:
                        assetID = "body_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.ENGINE:
                        assetID = "engine_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.INTAKE:
                        assetID = "intake_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.NITROUS:
                        assetID = "nitrous_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.TRANSMISSION:
                        assetID = "tranmission_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.TURBO:
                        assetID = "turbo_" + (j + 1) + assetID;
                        break;
                    case eUpgradeType.TYRES:
                        assetID = "tyre_" + (j + 1) + assetID;
                        break;
                }
                AvailableUpgradeData[k].AssetDatabaseID = assetID;
                AvailableUpgradeData[k].UpgradeType = CarUpgrades.ValidUpgrades[i];
                AvailableUpgradeData[k].UpgradeLevel = (byte) (j+1);
            }
        }
    }

    public static string ConvertCarTierEnumToString(eCarTier carTier)
    {
        return "TEXT_TIER_" + CarTierHelper.TierToString[(int)carTier];
    }

    public static string ConvertCarTierEnumToShortString(eCarTier carTier)
    {
        return "TEXT_TIER_" + CarTierHelper.TierToString[(int)carTier] + "_SHORT";
    }

    public static string ConvertCarTierEnumToLongString(eCarTier carTier)
    {
        return "TEXT_TIER_" + CarTierHelper.TierToString[(int)carTier] + "_LONG";
    }

    public CarInfo Clone()
    {
        //CarInfo carInfo = CreateInstance<CarInfo>();
        return base.MemberwiseClone() as CarInfo;
    }

    public void Process()
    {
        CarUpgradeData[] availableUpgradeData = this.AvailableUpgradeData;
        for (int i = 0; i < availableUpgradeData.Length; i++)
        {
            CarUpgradeData carUpgradeData = availableUpgradeData[i];
            carUpgradeData.Process();
        }
    }

    public void CalulateCosmeticMassRange(out float min, out float max)
    {
        float num = this.BaseChassisData.CosmeticMass;
        if (num == 0f)
        {
            num = this.BaseChassisData.Mass;
        }
        max = (min = num);
        CarUpgradeData[] availableUpgradeData = this.AvailableUpgradeData;
        for (int i = 0; i < availableUpgradeData.Length; i++)
        {
            CarUpgradeData carUpgradeData = availableUpgradeData[i];
            if (carUpgradeData.WeightDifference < 0f)
            {
                min += carUpgradeData.WeightDifference;
            }
            else
            {
                max += carUpgradeData.WeightDifference;
            }
        }
    }

    public void AssignFrom(CarInfo data)
    {
        this.Key = data.Key;
        this.LongName = "TEXT_CAR_" + data.Key + "_LONG";
        this.MediumName = "TEXT_CAR_" + data.Key + "_MEDIUM";
        this.ShortName = "TEXT_CAR_" + data.Key + "_SHORT";
        this.CarNumberID = data.CarNumberID;
        this.CarEngineSound = data.CarEngineSound;
        this.DefaultLiveryBundleName = data.DefaultLiveryBundleName;
        this.EliteOverrideCarAssetID = data.EliteOverrideCarAssetID;
        this.ManufacturerName = data.ManufacturerName;
        this.ManufacturerID = data.ManufacturerID;
        this.ModelPrefabString = data.ModelPrefabString;
        this.BaseCarTier = data.BaseCarTier;
        this.BasePerformanceIndex = data.BasePerformanceIndex;
        this.ModelPrefabString = data.ModelPrefabString;
        this.TransmissionLoss = data.TransmissionLoss;
        this.FlyWheelPower = data.FlyWheelPower;
        this.BaseRedlineRPM = data.BaseRedlineRPM;
        this.DriveType = data.DriveType;
        this.OptimalLaunchRPM = data.OptimalLaunchRPM;
        this.GarageWheelRotation = data.GarageWheelRotation;
        this.MechanicEngineFettleGraphSlope = data.MechanicEngineFettleGraphSlope;
        this.MechanicEngineFettleGraphIntercept = data.MechanicEngineFettleGraphIntercept;
        this.MechanicTyreGripFettleGraphSlope = data.MechanicTyreGripFettleGraphSlope;
        this.MechanicTyreGripFettleGraphIntercept = data.MechanicTyreGripFettleGraphIntercept;
        this.EngineConsumableEngineGraphSlope = data.EngineConsumableEngineGraphSlope;
        this.EngineConsumableEngineGraphIntercept = data.EngineConsumableEngineGraphIntercept;
        this.EngineConsumableTyreGraphSlope = data.EngineConsumableTyreGraphSlope;
        this.EngineConsumableTyreGraphIntercept = data.EngineConsumableTyreGraphIntercept;
        this.NitrousConsumableBodyGraphSlope = data.NitrousConsumableBodyGraphSlope;
        this.NitrousConsumableBodyGraphIntercept = data.NitrousConsumableBodyGraphIntercept;
        this.TyreConsumableEngineGraphSlope = data.TyreConsumableEngineGraphSlope;
        this.TyreConsumableEngineGraphIntercept = data.TyreConsumableEngineGraphIntercept;
        this.TyreConsumableTyreGraphSlope = data.TyreConsumableTyreGraphSlope;
        this.TyreConsumableTyreGraphIntercept = data.TyreConsumableTyreGraphIntercept;
        this.IsAvailableInShowroomAndUpgrade = data.IsAvailableInShowroomAndUpgrade;
        this.IsAvailableInShowroomOnlyInsideCountry = data.IsAvailableInShowroomOnlyInsideCountry;
        this.CarUnlocksInSeason = data.CarUnlocksInSeason;
        this.IsPrizeInSeason = data.IsPrizeInSeason;
        this.AppearsInMultiplayerAtSeason = data.AppearsInMultiplayerAtSeason;
        this.UnlockDependencies = data.UnlockDependencies;
        this.UnlockEvents = data.UnlockEvents;
        this.EvolutionSequences = data.EvolutionSequences;
        this.PrizeInfoText = data.PrizeInfoText;
        this.PrizeInfoButton = data.PrizeInfoButton;
        this.IsWorldTourWinnableCar = data.IsWorldTourWinnableCar;
        this.IsInternationalCar = data.IsInternationalCar;
        this.IsBossCarOverride = data.IsBossCarOverride;
        this.BodyShaderOverride = data.BodyShaderOverride;
        this.HeadlightShaderOverride = data.HeadlightShaderOverride;
        this.RingShaderOverride = data.RingShaderOverride;
        this.StickerOverride = data.StickerOverride;
        this.SpoilerOverride = data.SpoilerOverride;
        this.BaseEngineData = new EngineData();
        this.BaseEngineData.AssignFrom(data.BaseEngineData);
        this.BaseChassisData = new ChassisData();
        this.BaseChassisData.AssignFrom(data.BaseChassisData);
        this.BaseGearBoxData = new GearBoxData();
        this.BaseGearBoxData.AssignFrom(data.BaseGearBoxData);
        this.BaseTyreData = new TireData();
        this.BaseTyreData.AssignFrom(data.BaseTyreData);
        this.BaseNitrousData = new NitrousData();
        this.BaseNitrousData.AssignFrom(data.BaseNitrousData);
        this.AvailableUpgradeData = new CarUpgradeData[data.AvailableUpgradeData.Length];
        for (int i = 0; i < data.AvailableUpgradeData.Length; i++)
        {
            this.AvailableUpgradeData[i] = new CarUpgradeData();
            this.AvailableUpgradeData[i].AssignFrom(data.AvailableUpgradeData[i]);
        }
        this.PredefinedUpgradeSets = new PredefinedUpgradeSetsData[data.PredefinedUpgradeSets.Length];
        for (int j = 0; j < data.PredefinedUpgradeSets.Length; j++)
        {
            this.PredefinedUpgradeSets[j] = new PredefinedUpgradeSetsData();
            this.PredefinedUpgradeSets[j].AssignFrom(data.PredefinedUpgradeSets[j]);
        }
        if (data.CarVersionNumber != string.Empty)
        {
            this.CarVersionNumber = data.CarVersionNumber;
        }
    }

    public override string ToString()
    {
        string text = this.Key + "\n";
        text = text + "AssetDBID : " + this.Key + "\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Car ID : ",
            this.CarNumberID,
            "\n"
        });
        text = text + "Car Version: " + this.CarVersionNumber + "\n";
        text = text + "Long Name : " + this.LongName + "\n";
        text = text + "Medium Name : " + this.MediumName + "\n";
        text = text + "Short Name : " + this.ShortName + "\n";
        text = text + "Manufacturer ID : " + this.ManufacturerID + "\n";
        text = text + "Manufacturer Name : " + this.ManufacturerName + "\n";
        text = text + "Default Livery Bundle Name : " + this.DefaultLiveryBundleName + "\n";
        text = text + "Elite Sports Override Car Asset ID : " + this.EliteOverrideCarAssetID + "\n";
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Drive Type : ",
            this.DriveType,
            "\n"
        });
        text = text + "Model Prefab Name : " + this.ModelPrefabString + "\n";
        text = text + "Base Car Tier : " + ConvertCarTierEnumToString(this.BaseCarTier) + "\n";
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Base Performance Index : ",
            this.BasePerformanceIndex,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Power At FlyWheel : ",
            this.FlyWheelPower,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Mechanic Engine Fettle line graph equation... y = ",
            this.MechanicEngineFettleGraphSlope,
            "x ",
            this.MechanicEngineFettleGraphIntercept,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Mechanic Tyre Grip line graph equation... y = ",
            this.MechanicTyreGripFettleGraphSlope,
            "x ",
            this.MechanicTyreGripFettleGraphIntercept,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Base Redline RPM: ",
            this.BaseRedlineRPM,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Optimal Launch RPM: ",
            this.OptimalLaunchRPM,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Transmission Loss Percent : ",
            this.TransmissionLoss,
            "\n"
        });
        text += "\n";
        text = text + this.BaseEngineData.ToString() + "\n";
        text = text + this.BaseChassisData.ToString() + "\n";
        text = text + this.BaseGearBoxData.ToString() + "\n";
        text = text + this.BaseTyreData.ToString() + "\n";
        text = text + this.BaseNitrousData.ToString() + "\n";
        string text3 = text + "\n";
        CarUpgradeData[] availableUpgradeData = this.AvailableUpgradeData;
        for (int i = 0; i < availableUpgradeData.Length; i++)
        {
            CarUpgradeData carUpgradeData = availableUpgradeData[i];
            text3 += carUpgradeData.ToString();
        }
        return text3;
    }

    public int ComparableCashPrice()
    {
        if (this.BuyPrice > 0)
        {
            return this.BuyPrice;
        }
        if (this.GoldPrice > 0)
        {
            return 2147483647;
        }
        return 0;
    }

    public bool IsAvailableToBuyInShowroom()
    {
        if (BasePlatform.ActivePlatform.InsideCountry) {
            return this.IsAvailableInShowroomAndUpgrade;
        } else {
            return this.IsAvailableInShowroomOnlyInsideCountry ? false : this.IsAvailableInShowroomAndUpgrade;
        }
        //return this.IsAvailableInShowroomAndUpgrade; // && SeasonUtilities.CanCarBeUnlockedBySeason(this);
    }

    public bool IsUnlockedToBuy()
    {
        if (this.UnlockEventIds.Count == 0)
        {
            return true;
        }
        IGameState gameState = new GameStateFacade();
        for (int i = 0; i < this.UnlockEventIds.Count; i++)
        {
            if (!gameState.IsEventCompleted(this.UnlockEventIds[i]))
            {
                return false;
            }
        }
        return true;
    }

    public bool HasBeenAvailableInShowroomForNSeasons(int seasons)
    {
        return false;
        //return this.IsAvailableInShowroomAndUpgrade && SeasonUtilities.HasCarBeenAvailableInShowroomForNSeasons(this, seasons);
    }

    public bool IsComingSoonToShowroom()
    {
        return this.HasBeenAvailableInShowroomForNSeasons(-1);
    }

    public bool IsPrizeInCurrentSeason()
    {
        return false;
        //return this.IsPrizeInSeason > 0 && GameDatabase.Instance.SeasonEvents.IsSeasonCurrentEvent(this.IsPrizeInSeason);
    }

    public bool IsPrizeInFutureSeason()
    {
        return false;
        //return this.IsPrizeInSeason > 0 && GameDatabase.Instance.SeasonEvents.IsSeasonFutureEvent(this.IsPrizeInSeason);
    }

    public bool IsAvailableInMultiplayer()
    {
        return false;
        //bool flag = this.AppearsInMultiplayerAtSeason > 0 && GameDatabase.Instance.SeasonEvents.IsSeasonFutureEvent(this.AppearsInMultiplayerAtSeason);
        //if (flag)
        //{
        //    return false;
        //}
        //bool flag2 = SeasonUtilities.CanCarBeUnlockedBySeason(this) || SeasonUtilities.IsCarPreviousSeasonPrize(this);
        //return flag2 && !this.IsClassicsCrossPromo();
    }

    public bool HasEliteLivery()
    {
        return false;
        //string value = this.Key + "_Livery";
        //List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(CSRAssetTypes.livery);
        //foreach (AssetDatabaseAsset current in assetsOfType)
        //{
        //    string code = current.code;
        //    if (code.StartsWith(value))
        //    {
        //        if (code.ToLower().Contains("elite"))
        //        {
        //            return true;
        //        }
        //    }
        //}
        //return false;
    }

    public bool SupportsElite()
    {
        return false;
        //return this.HasEliteLivery() || !string.IsNullOrEmpty(this.EliteOverrideCarAssetID) || (this.IsBossCarOverride && !CarDataDefaults.NonEliteBossCars.Contains(this.Key));
    }

    public bool IsClassicsCrossPromo()
    {
        return !this.IsAvailableToBuyInShowroom() && !string.IsNullOrEmpty(this.PrizeInfoButton) &&
               this.PrizeInfoButton.Split(new char[]
               {
                   ','
               })[1].Contains("OnGoDownloadClassics");
    }

    public int EvolutionUpgradesEarned(int level)
    {
        return 0;
        //IGameState gameState = new GameStateFacade();
        //return gameState.LastWonEventSequenceLevel("TierX_International_Finals", this.EvolutionSequences[level]) + 1;
    }

    public bool UsesEvoUpgrades()
    {
        return GameDatabase.Instance.CarsConfiguration.CarsWithEvoUpgrades != null && GameDatabase.Instance.CarsConfiguration.CarsWithEvoUpgrades.Contains(this.Key);
    }

    public PredefinedUpgradeSetsData GetFullyUpgradedSet()
    {
        PredefinedUpgradeSetsData predefinedUpgradeSetsData =
            this.PredefinedUpgradeSets.LastOrDefault((PredefinedUpgradeSetsData x) => !x.UpgradeData.Contains(":"));
        return predefinedUpgradeSetsData ?? this.PredefinedUpgradeSets.Last<PredefinedUpgradeSetsData>();
    }

    public Dictionary<byte, object> ToDictionary()
    {
        var parameters = new Dictionary<byte,object>();

        parameters.Add((byte)ParameterCode.CarID, Key);
        parameters.Add((byte)ParameterCode.ManufacturerID, ManufacturerID);
        parameters.Add((byte)ParameterCode.DriveType, (byte)DriveType);
        parameters.Add((byte)ParameterCode.PPIndex, BasePerformanceIndex);
        parameters.Add((byte)ParameterCode.CarDivisionID, (byte)BaseCarTier);
        parameters.Add((byte)ParameterCode.BaseRedlineRPM, BaseRedlineRPM);
        parameters.Add((byte)ParameterCode.OptimalLaunchRPM, OptimalLaunchRPM);
        parameters.Add((byte)ParameterCode.BaseTorqueCurve, BaseEngineData.BaseTorqueCurve.animationCurve.ToXML());
        parameters.Add((byte)ParameterCode.EngineRevRate, BaseEngineData.EngineRevRate);
        parameters.Add((byte)ParameterCode.RevLimiterTime,BaseEngineData.RevLimiterTime);
        parameters.Add((byte)ParameterCode.Mass, BaseChassisData.Mass);
        parameters.Add((byte)ParameterCode.DragCoefficient, BaseChassisData.DragCoefficient);
        parameters.Add((byte)ParameterCode.Width, BaseChassisData.Width);
        parameters.Add((byte)ParameterCode.Height, BaseChassisData.Height);
        parameters.Add((byte)ParameterCode.GearsRatio, BaseGearBoxData.GearRatios);
        parameters.Add((byte)ParameterCode.DifferentialRatio, BaseGearBoxData.FinalGearRatio);
        parameters.Add((byte)ParameterCode.RPMToShiftAutomatic, BaseGearBoxData.RPMToShiftAutomatic);
        parameters.Add((byte)ParameterCode.ClutchDelay, BaseGearBoxData.ClutchDelay);
        parameters.Add((byte)ParameterCode.ClutchDelayFirstGear, BaseGearBoxData.ClutchDelayFirstGear);
        parameters.Add((byte)ParameterCode.WheelRadius, BaseTyreData.WheelRadius);
        parameters.Add((byte)ParameterCode.TyreFriction, BaseTyreData.RollingFrictionCoefficient);
        parameters.Add((byte)ParameterCode.RoadFriction, BaseTyreData.RoadFrictionCoefficient);
        parameters.Add((byte)ParameterCode.TireGripMax, BaseTyreData.TireGripMax);
        parameters.Add((byte)ParameterCode.FrontAxleDriven, BaseTyreData.FrontAxleDriven);
        parameters.Add((byte)ParameterCode.RearAxleDriven, BaseTyreData.RearAxleDriven);
        parameters.Add((byte)ParameterCode.WheelSpinGripCurve, BaseTyreData.WheelSpinGripCurve.animationCurve.ToXML());
        parameters.Add((byte)ParameterCode.RPMVsExtraWheelSpinCurve, BaseTyreData.RPMVsExtraWheelSpinCurve.animationCurve.ToXML());
        parameters.Add((byte)ParameterCode.NitrousDuration, BaseNitrousData.Duration);
        parameters.Add((byte)ParameterCode.NitrousHorsePowerIncrease, BaseNitrousData.HorsePowerIncrease);
        parameters.Add((byte)ParameterCode.SuperNitrousDuration,BaseNitrousData.SuperNitrousDuration);
        parameters.Add((byte)ParameterCode.SuperNitrousExtraTyreGrip, BaseNitrousData.SuperNitrousExtraTyreGrip);
        parameters.Add((byte)ParameterCode.SuperNitrousHorsePowerIncrease, BaseNitrousData.SuperNitrousHorsePowerIncrease);
        parameters.Add((byte)ParameterCode.uEngineRevRateDifference,AvailableUpgradeData.Select(u=>u.EngineRevRateDifference).ToArray());
        parameters.Add((byte)ParameterCode.uNitrousTime, AvailableUpgradeData.Select(u => u.NitrousTime).ToArray());
        parameters.Add((byte)ParameterCode.uNitrousHPBoost, AvailableUpgradeData.Select(u => u.NitrousHPBoost).ToArray());
        parameters.Add((byte)ParameterCode.uIsNitrousContinuous, AvailableUpgradeData.Select(u => u.IsNitrousContinuous).ToArray());
        parameters.Add((byte)ParameterCode.uExtraRPMAvailable, AvailableUpgradeData.Select(u => u.ExtraRPMAvailable).ToArray());
        parameters.Add((byte) ParameterCode.uAssignedFromDataTroqueCurve,
            AvailableUpgradeData.Select(
                u => u.AssignedFromDataTorqueCurve != null ? u.AssignedFromDataTorqueCurve.animationCurve.ToXML() : "")
                .ToArray());
        parameters.Add((byte)ParameterCode.uDragCoefficientDifference, AvailableUpgradeData.Select(u => u.DragCoeffDifference).ToArray());
        parameters.Add((byte)ParameterCode.uWeightDifference, AvailableUpgradeData.Select(u => u.WeightDifference).ToArray());
        parameters.Add((byte)ParameterCode.uGripDifference, AvailableUpgradeData.Select(u => u.GripDifference).ToArray());
        parameters.Add((byte)ParameterCode.uGearShiftTimeDifference, AvailableUpgradeData.Select(u => u.GearShiftTimeDifference).ToArray());
        parameters.Add((byte)ParameterCode.uUpgradeType, AvailableUpgradeData.Select(u => (byte)u.UpgradeType).ToArray());
        parameters.Add((byte)ParameterCode.uUpgradeLevel, AvailableUpgradeData.Select(u => u.UpgradeLevel).ToArray());

        return parameters;
    }

    public string ID
    {
        get { return Key; }
    }

    public int PPIndex
    {
        get { return BasePerformanceIndex; }
    }

    public eCarTier Tier
    {
        get { return BaseCarTier; }
    }

    public bool HasSticker
    {
        get { return Stickers.Length > 0; }
    }

    public int MaxPPIndex
    {
        get { return PredefinedUpgradeSets.Max(u => u.PPIndex); }
    }
}
