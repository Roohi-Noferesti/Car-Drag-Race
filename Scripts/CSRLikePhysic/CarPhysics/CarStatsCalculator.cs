using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarStatsCalculator : MonoBehaviour
{
    public delegate void FinishedCalculatingNewPPIndex(string name, eCarTier tier, int PPIndex, float QMTime);

    private CarPhysics carPhysics;

    private eUpgradeType cachedUpgradeType;

    private int cachedCurrentLevel;

    private int cachedToLevel;

    private bool hasCachedCalculationToPerform;

    private eUpgradeType completedCalcUpgradeType;

    private int completeCalcCurrentLevel;

    private int completeCalcWantedLevel;

    public bool isCalculatingPlayerCurrentSetupStats;

    private Dictionary<CarStatKey, UpgradeScreenCarStats> statsDict = new Dictionary<CarStatKey, UpgradeScreenCarStats>();

    public static int MAX_FRAMES_FOR_QUARTER_MILE_CALCULATION = 1000;

    public event CarStatsCalculator.FinishedCalculatingNewPPIndex NewPPIndexCalculated;

    public static CarStatsCalculator Instance
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator playerCarPhysicsSetup
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator stockCarPhysicsSetup
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator playerCarUpgradeScreenSetup
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator sportsPackStockCarSetup
    {
        get;
        private set;
    }

    public CarPhysicsSetupCreator sportsPackUpgradedCarSetup
    {
        get;
        private set;
    }

    public CarPerformanceIndexCalculator piCalculator
    {
        get;
        private set;
    }

    public bool IsCalculatingPerformance
    {
        get;
        private set;
    }

    public bool hasFinishedCalculatingPerformance
    {
        get;
        private set;
    }

    public int HorsePower
    {
        get;
        private set;
    }

    public int Weight
    {
        get;
        private set;
    }

    public int TyreGrip
    {
        get;
        private set;
    }

    public int GearShiftTime
    {
        get;
        private set;
    }

    private void Awake()
    {
        CarStatsCalculator.Instance = this;
    }

    public void ApplyUpgradeSetup()
    {
        this.playerCarPhysicsSetup = this.playerCarUpgradeScreenSetup;
    }

    public void ResetStatCache()
    {
        this.statsDict.Clear();
    }

    private void CreatePhysicsIfNeeded()
    {
        if (this.carPhysics == null)
        {
            this.carPhysics = base.gameObject.AddComponent<CarPhysics>();
            this.carPhysics.FrontendMode = true;
        }
    }

    public void DestroyFrontendPhysics()
    {
        this.Stop();
        this.carPhysics.OnDestroy();
        UnityEngine.Object.Destroy(this.carPhysics);
    }

    public void Stop()
    {
        if (this.piCalculator != null)
        {
            this.piCalculator.Stop();
        }
        this.IsCalculatingPerformance = false;
        this.hasFinishedCalculatingPerformance = false;
    }

    public void CalculateStatsForHumanCarWithUpgradeSetup(CarUpgradeSetup zCarSetup)
    {
        this.CreatePhysicsIfNeeded();
        CarInfo car = CarDatabase.Instance.GetCar(zCarSetup.CarDBKey);
        this.playerCarPhysicsSetup = new CarPhysicsSetupCreator(car, this.carPhysics);
        zCarSetup.IsFettled = false;
        this.playerCarPhysicsSetup.InitialiseCarPhysics(zCarSetup);
        this.playerCarPhysicsSetup.SetStatsPriorToUpgrade();
        this.playerCarPhysicsSetup.ApplyCarUpgrades(zCarSetup);
        this.playerCarPhysicsSetup.SetStatsAfterUpgrade();
        OptimalGearChangeSpeedCalculator optimalGearChangeSpeedCalculator = new OptimalGearChangeSpeedCalculator(this.carPhysics);
        optimalGearChangeSpeedCalculator.CalculateGearChangeSpeeds();
        this._CalculateStatsForHumanCarWithUpgradeSetup(0f);
    }

    private void _CalculateStatsForHumanCarWithUpgradeSetup(float launchRPMVariation)
    {
        this.piCalculator = new CarPerformanceIndexCalculator(true);
        this.piCalculator.SetupCalculation(this.carPhysics, -1, launchRPMVariation);
        this.piCalculator.DoInstantCalculation();
        this.playerCarPhysicsSetup.SetPerformancePotentialStats(this.piCalculator, true);
    }

    private void CalculateNewPerformanceIndex(bool zBlock, CarPhysics inCarPhysics, float launchVariation, bool ignoreTierChange = false)
    {
        this.IsCalculatingPerformance = true;
        this.hasFinishedCalculatingPerformance = false;
        this.piCalculator = new CarPerformanceIndexCalculator(ignoreTierChange);
        int numContinousFramesToCalculate = 60;
        if (zBlock)
        {
            numContinousFramesToCalculate = -1;
        }
        this.piCalculator.SetupCalculation(inCarPhysics, numContinousFramesToCalculate, launchVariation);
    }

    public void CalculateUpgradeScreenPerformanceIndex()
    {
        this.CalculatePerformanceIndexWorder(this.completeCalcCurrentLevel, this.completeCalcWantedLevel);
    }

    public void CalculateSportsPackScreenPerformanceIndex()
    {
        this.CalculatePerformanceIndexWorder(0, 0);
    }

    private void CalculatePerformanceIndexWorder(int completeCalcCurrentLevel, int completeCalcWantedLevel)
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        this.PerformCalculateStatsForUpgradeScreen(activeProfile.GetCurrentCarUpgradeSetup(), this.completedCalcUpgradeType, completeCalcCurrentLevel, completeCalcWantedLevel);
        OptimalGearChangeSpeedCalculator optimalGearChangeSpeedCalculator = new OptimalGearChangeSpeedCalculator(this.carPhysics);
        optimalGearChangeSpeedCalculator.CalculateGearChangeSpeeds();
        this.statsDict.Clear();
        this.CalculateNewPerformanceIndex(false, this.carPhysics, 0f, true);
    }

    private void Update()
    {
        if (this.IsCalculatingPerformance)
        {
            CarPerformanceIndexCalculator.eState eState = this.piCalculator.ContinueCalculation();
            if (eState == CarPerformanceIndexCalculator.eState.FINISHED)
            {
                this.IsCalculatingPerformance = false;
                this.hasFinishedCalculatingPerformance = true;
                this.piCalculator.CalculatePerformanceIndex();
                if (this.isCalculatingPlayerCurrentSetupStats)
                {
                    PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup = CarStatsCalculator.Instance.playerCarPhysicsSetup;
                    CarStatsCalculator.Instance.playerCarPhysicsSetup.SetPerformancePotentialStats(this.piCalculator, true);
                    CarStatsCalculator.Instance.SetOutStats(eCarStatsType.PLAYER_SETUP_CAR);
                }
                this.isCalculatingPlayerCurrentSetupStats = false;
                if (this.NewPPIndexCalculated != null)
                {
                    var performanceIndex = this.piCalculator.PerformanceIndex;//this.piCalculator.CarTierEnum >= eCarTier.TIER_4
//                        ? this.piCalculator.PerformanceIndexHalfMile
//                        : this.piCalculator.PerformanceIndex;
                    this.NewPPIndexCalculated(this.playerCarUpgradeScreenSetup.carInfo.ShortName,
                        this.piCalculator.CarTierEnum, performanceIndex
                        , this.piCalculator.QuarterMileTime);
                    PlayerProfileManager.CurrentCarNewPP(this.playerCarUpgradeScreenSetup.carInfo.ShortName,
                        this.piCalculator.CarTierEnum, performanceIndex,
                        this.piCalculator.QuarterMileTime);
                }
                if (this.hasCachedCalculationToPerform)
                {
                    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                    this.CalculateStatsForUpgradeScreen(activeProfile.GetCurrentCarUpgradeSetup(), this.cachedUpgradeType, this.cachedCurrentLevel, this.cachedToLevel);
                    this.hasCachedCalculationToPerform = false;
                }
            }
            else if (eState == CarPerformanceIndexCalculator.eState.CANCELLED)
            {
                this.IsCalculatingPerformance = false;
                this.hasFinishedCalculatingPerformance = true;
            }
        }
    }

    public void CalculateStatsForUpgradeScreen(CarUpgradeSetup zCurrentSetup, eUpgradeType zUpgradeType, int zCurrentUpgradeLevel, int zToUpgradeLevel)
    {
        this.CreatePhysicsIfNeeded();
        if (this.IsCalculatingPerformance)
        {
            this.cachedUpgradeType = zUpgradeType;
            this.cachedCurrentLevel = zCurrentUpgradeLevel;
            this.cachedToLevel = zToUpgradeLevel;
            this.hasCachedCalculationToPerform = true;
            return;
        }
        CarStatKey carStatKey = new CarStatKey((int)zCurrentSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted, (int)zCurrentSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted, zToUpgradeLevel, zUpgradeType, zToUpgradeLevel > zCurrentUpgradeLevel);
        this.completedCalcUpgradeType = zUpgradeType;
        this.completeCalcCurrentLevel = zCurrentUpgradeLevel;
        this.completeCalcWantedLevel = zToUpgradeLevel;
        UpgradeScreenCarStats stats;
        bool flag = this.statsDict.TryGetValue(carStatKey, out stats);
        if (flag)
        {
            CarInfoUI.Instance.ResetCarDataWithUpgrades(stats);
            return;
        }
        this.PerformCalculateStatsForUpgradeScreen(zCurrentSetup, zUpgradeType, zCurrentUpgradeLevel, zToUpgradeLevel);
        this.UpdateUIAndStoreInCache(carStatKey);
    }

    private void PerformCalculateStatsForUpgradeScreen(CarUpgradeSetup zCurrentSetup, eUpgradeType zUpgradeType, int zCurrentUpgradeLevel, int zToUpgradeLevel)
    {
        CarInfo car = CarDatabase.Instance.GetCar(zCurrentSetup.CarDBKey);
        this.playerCarUpgradeScreenSetup = new CarPhysicsSetupCreator(car, this.carPhysics);
        this.isCalculatingPlayerCurrentSetupStats = false;
        zCurrentSetup.IsFettled = false;
        this.playerCarUpgradeScreenSetup.InitialiseCarPhysics(zCurrentSetup);
        this.playerCarUpgradeScreenSetup.ApplyCarUpgrades(zCurrentSetup);
        this.playerCarUpgradeScreenSetup.SetStatsPriorToUpgrade();
        if (zToUpgradeLevel > zCurrentUpgradeLevel)
        {
            for (int i = zCurrentUpgradeLevel + 1; i <= zToUpgradeLevel; i++)
            {
                this.playerCarUpgradeScreenSetup.ApplyDeltaCarUpgrade(zCurrentSetup.CarDBKey, zUpgradeType, i);
            }
        }
        else if (zToUpgradeLevel < zCurrentUpgradeLevel)
        {
            for (int j = zCurrentUpgradeLevel; j >= zToUpgradeLevel + 1; j--)
            {
                this.playerCarUpgradeScreenSetup.ApplyNegativeDeltaCarUpgrade(zCurrentSetup.CarDBKey, zUpgradeType, j);
            }
        }
        this.playerCarUpgradeScreenSetup.SetStatsAfterUpgrade();
    }

    private void UpdateUIAndStoreInCache(CarStatKey carStatHash)
    {
        UpgradeScreenCarStats upgradeScreenCarStats;
        upgradeScreenCarStats.CurrentHP = Mathf.RoundToInt((float)this.playerCarUpgradeScreenSetup.PriorToUpgradeHP);
        upgradeScreenCarStats.CurrentGrip = Mathf.RoundToInt(this.playerCarUpgradeScreenSetup.PriorToUpgradeGrip);
        upgradeScreenCarStats.CurrentGearShiftTime = Mathf.RoundToInt(this.playerCarUpgradeScreenSetup.PriorToUpgradeGearShiftTime * 1000f);
        upgradeScreenCarStats.CurrentWeight = Mathf.RoundToInt(this.playerCarUpgradeScreenSetup.PriorToUpgradeWeight * 2.20462251f);
        upgradeScreenCarStats.DeltaHP = Mathf.RoundToInt((float)(this.playerCarUpgradeScreenSetup.NewPeakHP - upgradeScreenCarStats.CurrentHP));
        upgradeScreenCarStats.DeltaWeight = Mathf.RoundToInt((this.playerCarUpgradeScreenSetup.NewWeight - this.playerCarUpgradeScreenSetup.PriorToUpgradeWeight) * 2.20462251f);
        upgradeScreenCarStats.DeltaGrip = Mathf.RoundToInt(this.playerCarUpgradeScreenSetup.NewGrip - (float)upgradeScreenCarStats.CurrentGrip);
        upgradeScreenCarStats.DeltaGearShiftTime = Mathf.RoundToInt((this.playerCarUpgradeScreenSetup.NewGearShiftTime - this.playerCarUpgradeScreenSetup.PriorToUpgradeGearShiftTime) * 1000f);
        this.statsDict.Add(carStatHash, upgradeScreenCarStats);
        CarInfoUI.Instance.ResetCarDataWithUpgrades(upgradeScreenCarStats);
    }

    public void CalculateStatsForStockCar(string zCarName)
    {
        this.CreatePhysicsIfNeeded();
        CarInfo car = CarDatabase.Instance.GetCar(zCarName);
        this.isCalculatingPlayerCurrentSetupStats = false;
        CarUpgradeSetup nullCarSetup = CarUpgradeSetup.NullCarSetup;
        this.stockCarPhysicsSetup = new CarPhysicsSetupCreator(car, this.carPhysics);
        this.stockCarPhysicsSetup.InitialiseCarPhysics(nullCarSetup);
        this.stockCarPhysicsSetup.SetStatsPriorToUpgrade();
        this.stockCarPhysicsSetup.SetStatsAfterUpgrade();
    }

    public CarUpgradeSetup CalculateStatsForSportsPackCars(CarInfo carInfo, int basePerformanceIndex, int targetPP, CarInfoUI standardCar, CarInfoUI sportsPackCar, out int sportsPackNewPP)
    {
        this.CreatePhysicsIfNeeded();
        CarUpgradeSetup carUpgradeSetup = PredefinedUpgradeSetsData.InitialiseFromRequestedPPIndex(carInfo, basePerformanceIndex, targetPP, out sportsPackNewPP, true, false);
        if (carUpgradeSetup == null)
        {
            carUpgradeSetup = CarUpgradeSetup.NullCarSetup;
        }
        this.sportsPackUpgradedCarSetup = new CarPhysicsSetupCreator(carInfo, this.carPhysics);
        this.sportsPackUpgradedCarSetup.InitialiseCarPhysics(carUpgradeSetup);
        this.sportsPackUpgradedCarSetup.SetStatsPriorToUpgrade();
        this.sportsPackUpgradedCarSetup.ApplyCarUpgrades(carUpgradeSetup);
        this.sportsPackUpgradedCarSetup.SetStatsAfterUpgrade();
        this.SetStatsForSportsPackCars(this.sportsPackUpgradedCarSetup, standardCar, sportsPackCar);
        return carUpgradeSetup;
    }

    public void SetStatsForSportsPackCars(CarPhysicsSetupCreator sportsPackUpgradedCarSetup, CarInfoUI standardCar, CarInfoUI sportsPackCar)
    {
        UpgradeScreenCarStats stats;
        stats.CurrentHP = Mathf.RoundToInt((float)sportsPackUpgradedCarSetup.PriorToUpgradeHP);
        stats.CurrentWeight = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeWeight * 2.20462251f);
        stats.CurrentGrip = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeGrip);
        stats.CurrentGearShiftTime = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeGearShiftTime * 1000f);
        stats.DeltaHP = 0;
        stats.DeltaWeight = 0;
        stats.DeltaGrip = 0;
        stats.DeltaGearShiftTime = 0;
        standardCar.ResetDeltaStatCache();
        standardCar.ResetCarDataWithUpgrades(stats);
        stats.CurrentHP = Mathf.RoundToInt((float)sportsPackUpgradedCarSetup.PriorToUpgradeHP);
        stats.CurrentWeight = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeWeight * 2.20462251f);
        stats.CurrentGrip = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeGrip);
        stats.CurrentGearShiftTime = Mathf.RoundToInt(sportsPackUpgradedCarSetup.PriorToUpgradeGearShiftTime * 1000f);
        stats.DeltaHP = Mathf.RoundToInt((float)(sportsPackUpgradedCarSetup.NewPeakHP - sportsPackUpgradedCarSetup.PriorToUpgradeHP));
        stats.DeltaWeight = Mathf.RoundToInt((sportsPackUpgradedCarSetup.NewWeight - sportsPackUpgradedCarSetup.PriorToUpgradeWeight) * 2.20462251f);
        stats.DeltaGrip = Mathf.RoundToInt(sportsPackUpgradedCarSetup.NewGrip - sportsPackUpgradedCarSetup.PriorToUpgradeGrip);
        stats.DeltaGearShiftTime = Mathf.RoundToInt((sportsPackUpgradedCarSetup.NewGearShiftTime - sportsPackUpgradedCarSetup.PriorToUpgradeGearShiftTime) * 1000f);
        sportsPackCar.ResetDeltaStatCache();
        sportsPackCar.ResetCarDataWithUpgrades(stats);
    }

    public void OverrideStatsForSportsPackScreen(CarInfoUI standardCar, CarInfoUI sportsPackCar, UpgradeScreenCarStats baseStats)
    {
        UpgradeScreenCarStats stats;
        stats.CurrentHP = baseStats.CurrentHP;
        stats.CurrentGrip = baseStats.CurrentGrip;
        stats.CurrentGearShiftTime = baseStats.CurrentGearShiftTime;
        stats.CurrentWeight = baseStats.CurrentWeight;
        stats.DeltaHP = 0;
        stats.DeltaWeight = 0;
        stats.DeltaGrip = 0;
        stats.DeltaGearShiftTime = 0;
        standardCar.ResetDeltaStatCache();
        standardCar.ResetCarDataWithUpgrades(stats);
        stats.CurrentHP = baseStats.CurrentHP;
        stats.CurrentGrip = baseStats.CurrentGrip;
        stats.CurrentGearShiftTime = baseStats.CurrentGearShiftTime;
        stats.CurrentWeight = baseStats.CurrentWeight;
        stats.DeltaHP = this.sportsPackUpgradedCarSetup.NewPeakHP - baseStats.CurrentHP;
        stats.DeltaWeight = Mathf.RoundToInt(this.sportsPackUpgradedCarSetup.NewWeight * 2.20462251f) - baseStats.CurrentWeight;
        stats.DeltaGrip = Mathf.RoundToInt(this.sportsPackUpgradedCarSetup.NewGrip - (float)baseStats.CurrentGrip);
        stats.DeltaGearShiftTime = Mathf.RoundToInt(this.sportsPackUpgradedCarSetup.NewGearShiftTime * 1000f) - baseStats.CurrentGearShiftTime;
        sportsPackCar.ResetDeltaStatCache();
        sportsPackCar.ResetCarDataWithUpgrades(stats);
    }

    public void SetOutStats(eCarStatsType zCarStatsType)
    {
        CarPhysicsSetupCreator carPhysicsSetupCreator = null;
        if (zCarStatsType == eCarStatsType.PLAYER_SETUP_CAR)
        {
            carPhysicsSetupCreator = this.playerCarPhysicsSetup;
        }
        else if (zCarStatsType == eCarStatsType.STOCK_CAR)
        {
            carPhysicsSetupCreator = this.stockCarPhysicsSetup;
        }
        else if (zCarStatsType == eCarStatsType.PLAYER_NEXT_UPGRADE_CAR)
        {
            carPhysicsSetupCreator = this.playerCarUpgradeScreenSetup;
        }
        if (carPhysicsSetupCreator == null)
        {
            return;
        }
        this.HorsePower = Mathf.CeilToInt((float)carPhysicsSetupCreator.NewPeakHP);
        this.Weight = Mathf.CeilToInt(carPhysicsSetupCreator.NewWeight * 2.20462251f);
        this.TyreGrip = Mathf.CeilToInt(carPhysicsSetupCreator.NewGrip);
        this.GearShiftTime = Mathf.CeilToInt(carPhysicsSetupCreator.NewGearShiftTime * 1000f);
    }
}
