using System;
using I2.Loc;
using UnityEngine;

public class CarPhysicsSetupCreator
{
    public CarPhysics CarPhysics
    {
        get;
        private set;
    }

    public int PriorToUpgradeHP
    {
        get;
        private set;
    }

    public int NewPeakHP
    {
        get;
        private set;
    }

    public float PriorToUpgradeWeight
    {
        get;
        private set;
    }

    public float NewWeight
    {
        get;
        private set;
    }

    public float PriorToUpgradeGrip
    {
        get;
        private set;
    }

    public float NewGrip
    {
        get;
        private set;
    }

    public float PriorToUpgradeEngineRevRate
    {
        get;
        private set;
    }

    public float NewEngineRevRate
    {
        get;
        private set;
    }

    public float PriorToUpgradeAeroCoeff
    {
        get;
        private set;
    }

    public float NewAeroCoeff
    {
        get;
        private set;
    }

    public float PriorToUpgradeGearShiftTime
    {
        get;
        private set;
    }

    public float NewGearShiftTime
    {
        get;
        private set;
    }

    public float PriorToUpgradeNitrousDuration
    {
        get;
        private set;
    }

    public float NewNitrousDuration
    {
        get;
        private set;
    }

    public float PriorToUpgradeNitrousHPBoost
    {
        get;
        private set;
    }

    public float NewNitrousHPBoost
    {
        get;
        private set;
    }

    public int BasePerformanceIndex
    {
        get;
        private set;
    }

    public eCarTier BaseCarTier
    {
        get;
        private set;
    }

    public string BaseCarTierString
    {
        get;
        private set;
    }

    public eDriveType NewDriveType
    {
        get;
        private set;
    }

    public eCarTier PriorToUpgradeCarTierEnum
    {
        get;
        set;
    }

    public string PriorToUpgradeCarTierString
    {
        get;
        private set;
    }

    public int PriorToUpgradePerformanceIndex
    {
        get;
        set;
    }

    public int NewPerformanceIndex
    {
        get;
        private set;
    }

    public string CarModel
    {
        get;
        private set;
    }

    public int TheoreticalTopSpeedMPH
    {
        get;
        private set;
    }

    public bool HaveAnyUpgradesBeenFitted
    {
        get;
        private set;
    }

    public bool HasAnyNitrousUpgradeBeenFitted
    {
        get;
        private set;
    }

    public bool HasAnyTurboUpgradeBeenFitted
    {
        get;
        private set;
    }

    public bool HasAnyTyreUpgradeBeenFitted
    {
        get;
        private set;
    }

    public bool isMechanicFettled
    {
        get;
        private set;
    }

    public CarInfo carInfo
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator(CarInfo inCarInfo, CarPhysics carPhysics)
    {
        this.CarPhysics = carPhysics;
        this.carInfo = inCarInfo;
    }

    public void CalculateStockSetup()
    {
        CarUpgradeSetup nullCarSetup = CarUpgradeSetup.NullCarSetup;
        this.InitialiseCarPhysics(nullCarSetup);
        this.SetStatsPriorToUpgrade();
        this.SetStatsAfterUpgrade();
    }

    public void InitialiseCarPhysics(CarUpgradeSetup zUpgradeSetup)
    {
        this.CarModel = this.carInfo.Key;
        this.CarPhysics.GearBoxData = this.carInfo.BaseGearBoxData.Clone();
        this.CarPhysics.EngineData = this.carInfo.BaseEngineData.Clone();
        this.CarPhysics.TireData = this.carInfo.BaseTyreData.Clone();
        this.CarPhysics.ChassisData = this.carInfo.BaseChassisData.Clone();
        this.CarPhysics.NitrousData = this.carInfo.BaseNitrousData.Clone();
        if (this.CarPhysics.ChassisData.CosmeticMass == 0f)
        {
            this.CarPhysics.ChassisData.CosmeticMass = this.CarPhysics.ChassisData.Mass;
        }
        if (zUpgradeSetup.ModifiedCarMass > 0f)
        {
            this.CarPhysics.ChassisData.Mass = zUpgradeSetup.ModifiedCarMass;
        }
        this.PriorToUpgradeCarTierString = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(this.carInfo.BaseCarTier));
        this.PriorToUpgradeCarTierEnum = this.carInfo.BaseCarTier;
        this.PriorToUpgradePerformanceIndex = this.carInfo.BasePerformanceIndex;
        this.BasePerformanceIndex = this.carInfo.BasePerformanceIndex;
        this.BaseCarTier = this.carInfo.BaseCarTier;
        this.BaseCarTierString = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(this.BaseCarTier));
        this.isMechanicFettled = zUpgradeSetup.IsFettled;
        this.NewPerformanceIndex = this.PriorToUpgradePerformanceIndex;
        this.HaveAnyUpgradesBeenFitted = false;
        this.HasAnyNitrousUpgradeBeenFitted = false;
        this.HasAnyTurboUpgradeBeenFitted = false;
        this.HasAnyTyreUpgradeBeenFitted = false;
        this.NewDriveType = this.carInfo.DriveType;
        this.CarPhysics.FettledEngineFactor = ((!this.isMechanicFettled) ? 1f : (zUpgradeSetup.FettleEnginePercent / 100f + 1f));
        this.CarPhysics.FettledTyreFactor = ((!this.isMechanicFettled) ? 0 : zUpgradeSetup.FettleTyreAdd);
        if (zUpgradeSetup.EngineConsumableActive)
        {
            this.CarPhysics.FettledEngineFactor += zUpgradeSetup.EngConsumableEnginePercent / 100f;
            this.CarPhysics.ConsumableTyreFactor = zUpgradeSetup.EngConsumableTyreAdd;
        }
        if (zUpgradeSetup.TyreConsumableActive)
        {
            this.CarPhysics.FettledEngineFactor += zUpgradeSetup.TyreConsumableEnginePercent / 100f;
            this.CarPhysics.ConsumableTyreFactor += zUpgradeSetup.TyreConsumableTyreAdd;
        }
        this.CarPhysics.ConsumableNitrousBodyFactor = 0f;
        if (zUpgradeSetup.NitrousConsumableActive)
        {
            this.CarPhysics.ConsumableNitrousBodyFactor = zUpgradeSetup.NitConsumableBodyAdd;
        }
        this.CarPhysics.CarTier = this.carInfo.BaseCarTier;
        this.CarPhysics.Initialise();
        this.CarPhysics.TransmissionPowerLossMultiplier = (100f - this.carInfo.TransmissionLoss) / 100f;
        this.CarPhysics.RedLineRPM = (float)this.carInfo.BaseRedlineRPM;
        this.CarPhysics.OptimalLaunchRPM = (float)this.carInfo.OptimalLaunchRPM;
        this.CarPhysics.GearBox.SetOptimalGearChangeMPHArray(this.carInfo.BaseGearBoxData.DebugChangeGearAtMPH);
        this.CarPhysics.HorsePowerDisplayRatio = (float)this.carInfo.FlyWheelPower / (float)this.CarPhysics.Engine.TruePeakHorsePower();
    }

    public void ApplySuperNitrous(float superNitrousFraction)
    {
        if (superNitrousFraction == 0f)
        {
            return;
        }
        this.CarPhysics.Engine.SuperNitrousAvailable = true;
        this.CarPhysics.Engine.SuperNitrousFactor = superNitrousFraction;
        this.CarPhysics.Wheels.SuperNitrousExtraGripAvailable = true;
    }

    public void SetStatsPriorToUpgrade()
    {
        this.PriorToUpgradeWeight = this.CarPhysics.ChassisData.CosmeticMass;
        this.PriorToUpgradeGrip = this.CarPhysics.TireData.TireGripMax;
        this.PriorToUpgradeEngineRevRate = this.CarPhysics.EngineData.EngineRevRate;
        this.PriorToUpgradeAeroCoeff = this.CarPhysics.ChassisData.DragCoefficient;
        this.PriorToUpgradeGearShiftTime = this.CarPhysics.GearBoxData.ClutchDelay;
        this.PriorToUpgradeNitrousDuration = this.CarPhysics.NitrousData.Duration;
        this.PriorToUpgradeNitrousHPBoost = this.CarPhysics.NitrousData.HorsePowerIncrease;
        this.PriorToUpgradeHP = Mathf.RoundToInt((float)this.CarPhysics.Engine.CalculatePeakHorsePower() * this.CarPhysics.HorsePowerDisplayRatio);
    }

    public void ApplyCarUpgrades(CarUpgradeSetup zUpgradeSetup)
    {
        if (zUpgradeSetup == null)
        {
            return;
        }
        string carDBKey = zUpgradeSetup.CarDBKey;
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            CarUpgradeStatus carUpgradeStatus = zUpgradeSetup.UpgradeStatus[current];
            this.ProcessUpgrades(carDBKey, current, 1, (int)carUpgradeStatus.levelFitted);
        }
    }

    public void ApplyDeltaCarUpgrade(string zCarDBKey, eUpgradeType zUpgradeType, int zCurrentLevel)
    {
        CarUpgradeData carUpgrade = CarDatabase.Instance.GetCarUpgrade(zCarDBKey, zUpgradeType, zCurrentLevel);
        if (carUpgrade == null)
        {
            return;
        }
        bool zNegateDifference = false;
        this.DoProcessUpgrade(carUpgrade, zUpgradeType, zNegateDifference);
    }

    public void ApplyNegativeDeltaCarUpgrade(string zCarDBKey, eUpgradeType zUpgradeType, int zCurrentLevel)
    {
        CarUpgradeData carUpgrade = CarDatabase.Instance.GetCarUpgrade(zCarDBKey, zUpgradeType, zCurrentLevel);
        if (carUpgrade == null)
        {
            return;
        }
        bool zNegateDifference = true;
        this.DoProcessUpgrade(carUpgrade, zUpgradeType, zNegateDifference);
    }

    public void SetStatsAfterUpgrade()
    {
        this.NewWeight = this.CarPhysics.ChassisData.CosmeticMass;
        this.NewGrip = this.CarPhysics.TireData.TireGripMax;
        this.NewEngineRevRate = this.CarPhysics.EngineData.EngineRevRate;
        this.NewAeroCoeff = this.CarPhysics.ChassisData.DragCoefficient;
        this.NewGearShiftTime = this.CarPhysics.GearBoxData.ClutchDelay;
        this.NewNitrousDuration = this.CarPhysics.NitrousData.Duration;
        this.NewNitrousHPBoost = this.CarPhysics.NitrousData.HorsePowerIncrease;
        this.NewPeakHP = Mathf.RoundToInt((float)this.CarPhysics.Engine.CalculatePeakHorsePower() * this.CarPhysics.HorsePowerDisplayRatio);
    }

    public void CalculatePerformanceIndex(bool isHumanCar)
    {
        CarPerformanceIndexCalculator carPerformanceIndexCalculator = new CarPerformanceIndexCalculator(true);
        carPerformanceIndexCalculator.SetupCalculation(this.CarPhysics, -1, 0f);
        carPerformanceIndexCalculator.DoInstantCalculation();
        carPerformanceIndexCalculator.CalculatePerformanceIndex();
        this.SetPerformancePotentialStats(carPerformanceIndexCalculator, isHumanCar);
    }

    public void SetPerformancePotentialStats(CarPerformanceIndexCalculator piCalculator, bool isHumanCar)
    {
        piCalculator.CalculatePerformanceIndex();
        this.CarPhysics.CarTier = this.BaseCarTier;

//        if (BaseCarTier>=eCarTier.TIER_4)
//            this.NewPerformanceIndex = piCalculator.PerformanceIndexHalfMile;
//        else
//        {
            this.NewPerformanceIndex = piCalculator.PerformanceIndex;
//        }
        if (isHumanCar && PlayerProfileManager.Instance != null)
        {
            CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            currentCar.TightLoopQuarterMileTime = piCalculator.QuarterMileTime;
            currentCar.CurrentPPIndex = this.NewPerformanceIndex;
        }
    }

    public void SetCachedPerformancePotentialStats(eCarTier carTier, int perfIndex)
    {
        this.NewPerformanceIndex = perfIndex;
        this.CarPhysics.CarTier = this.BaseCarTier;
    }

    private void ProcessUpgrades(string zCarDBID, eUpgradeType zUpgradeType, int zFromLevel, int zToLevel)
    {
        for (int i = zFromLevel; i <= zToLevel; i++)
        {
            CarUpgradeData carUpgrade = CarDatabase.Instance.GetCarUpgrade(zCarDBID, zUpgradeType, i);
            if (carUpgrade != null)
            {
                this.DoProcessUpgrade(carUpgrade, zUpgradeType, false);
            }
        }
        float zCombinedGearRatio = this.CarPhysics.GearBox.GearRatioFinal * this.CarPhysics.GearBox.GearRatio(this.CarPhysics.GearBox.NumGears);
        this.TheoreticalTopSpeedMPH = Mathf.CeilToInt(CarPhysicsCalculations.CalculateTheoreticalTopSpeedForThisGear(this.CarPhysics.Engine.RedLineRPM, zCombinedGearRatio, this.CarPhysics.TireData.WheelRadius) * 2.236f);
    }

    private void DoProcessUpgrade(CarUpgradeData zUpgrade, eUpgradeType zUpgradeType, bool zNegateDifference)
    {
        zUpgrade.ConvertTorqueCurve();
        this.ApplyToCarPhysics(zUpgrade, zNegateDifference);
        this.HaveAnyUpgradesBeenFitted = true;
        if (zUpgradeType == eUpgradeType.TURBO)
        {
            this.HasAnyTurboUpgradeBeenFitted = true;
        }
        if (zUpgradeType == eUpgradeType.NITROUS)
        {
            this.HasAnyNitrousUpgradeBeenFitted = true;
        }
        if (zUpgradeType == eUpgradeType.TYRES)
        {
            this.HasAnyTyreUpgradeBeenFitted = true;
        }
    }

    private void ApplyToCarPhysics(CarUpgradeData zUpgrade, bool zNegateDifference)
    {
        if (zNegateDifference)
        {
            if (zUpgrade.TorqueCurveDifference != null)
            {
                this.CarPhysics.Engine.TorqueCurve.TakeAwayTorqueCurve(zUpgrade.TorqueCurveDifference);
            }
            this.CarPhysics.ChassisData.Mass -= zUpgrade.WeightDifference;
            this.CarPhysics.ChassisData.CosmeticMass -= zUpgrade.WeightDifference;
            this.CarPhysics.TireData.TireGripMax -= zUpgrade.GripDifference;
            this.CarPhysics.EngineData.EngineRevRate -= zUpgrade.EngineRevRateDifference;
            this.CarPhysics.ChassisData.DragCoefficient -= zUpgrade.DragCoeffDifference;
            this.CarPhysics.NitrousData.Duration -= zUpgrade.NitrousTime;
            this.CarPhysics.NitrousData.HorsePowerIncrease -= zUpgrade.NitrousHPBoost;
            this.CarPhysics.GearBoxData.ClutchDelay -= zUpgrade.GearShiftTimeDifference;
            if (this.CarPhysics.GearBoxData.ClutchDelay <= 0)
            {
                GTDebug.LogWarning(GTLogChannel.PhysicsCalculations, "Gear shift time is equal or less than zrto for car '" + carInfo.Key +
                                 "', clamping to 0.05");
                this.CarPhysics.GearBoxData.ClutchDelay = 0.05F;
            }
            this.CarPhysics.RedLineRPM -= (float)zUpgrade.ExtraRPMAvailable;
            this.CarPhysics.NitrousData.IsContinuous = zUpgrade.IsNitrousContinuous;
        }
        else
        {
            if (zUpgrade.TorqueCurveDifference != null)
            {
                this.CarPhysics.Engine.TorqueCurve.AddTorqueCurve(zUpgrade.TorqueCurveDifference);
            }
            this.CarPhysics.ChassisData.Mass += zUpgrade.WeightDifference;
            this.CarPhysics.ChassisData.CosmeticMass += zUpgrade.WeightDifference;
            this.CarPhysics.TireData.TireGripMax += zUpgrade.GripDifference;
            this.CarPhysics.EngineData.EngineRevRate += zUpgrade.EngineRevRateDifference;
            this.CarPhysics.ChassisData.DragCoefficient += zUpgrade.DragCoeffDifference;
            this.CarPhysics.NitrousData.Duration += zUpgrade.NitrousTime;
            this.CarPhysics.NitrousData.HorsePowerIncrease += zUpgrade.NitrousHPBoost;
            this.CarPhysics.GearBoxData.ClutchDelay += zUpgrade.GearShiftTimeDifference;
            if (this.CarPhysics.GearBoxData.ClutchDelay <= 0.03)
            {
                GTDebug.LogWarning(GTLogChannel.PhysicsCalculations, "Gear shift time is '" + this.CarPhysics.GearBoxData.ClutchDelay + "' for car '" + carInfo.Key +
                                 "', clamping to 0.05");
                this.CarPhysics.GearBoxData.ClutchDelay = 0.04F;
            }
            this.CarPhysics.RedLineRPM += (float)zUpgrade.ExtraRPMAvailable;
            this.CarPhysics.NitrousData.IsContinuous = zUpgrade.IsNitrousContinuous;
        }
    }
}
