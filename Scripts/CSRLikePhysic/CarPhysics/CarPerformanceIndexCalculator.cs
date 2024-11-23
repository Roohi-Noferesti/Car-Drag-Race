using I2.Loc;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarPerformanceIndexCalculator
{
    public enum eState
    {
        NOT_STARTED,
        CALCULATING,
        FINISHED,
        CANCELLED
    }

    private int numFramesToCalculate;

    private bool ignoreTierChange;

    private BaseRunCarPhysicsInTightLoop runCarPhysicsInLoop;

    private CarPhysics carPhysics;

    private eState mState;

    public int PerformanceIndex
    {
        get;
        private set;
    }

    public int PerformanceIndexHalfMile
    {
        get;
        private set;
    }

    public float QuarterMileTime
    {
        get;
        private set;
    }

    public float HalfMileTime
    {
        get;
        private set;
    }

    public string CarTier
    {
        get;
        private set;
    }

    public eCarTier CarTierEnum
    {
        get;
        private set;
    }

    public CarPerformanceIndexCalculator(bool ignoreTier = false)
    {
        ignoreTierChange = ignoreTier;
    }

    public void SetupCalculation(CarPhysics zCarPhysics, int numContinousFramesToCalculate, float launchRPMVariation)
    {
        carPhysics = zCarPhysics;
        numFramesToCalculate = numContinousFramesToCalculate;
        mState = eState.CALCULATING;
        runCarPhysicsInLoop = new BaseRunCarPhysicsInTightLoop(carPhysics);
        //runCarPhysicsInLoop.distanceOfTest = PhysicsConstants.QUARTER_MILE_DISTANCE;
        runCarPhysicsInLoop.distanceOfTest = PhysicsConstants.HALF_MILE_DISTANCE;
        runCarPhysicsInLoop.startRevRPM = carPhysics.OptimalLaunchRPM + launchRPMVariation;
        runCarPhysicsInLoop.nitrousUseMPH = 60f;
        runCarPhysicsInLoop.StartTightLoopRun();
    }

    public void DoInstantCalculation()
    {
        runCarPhysicsInLoop.Execute(3000);
    }

    public eState ContinueCalculation()
    {
        if (mState == eState.CANCELLED)
        {
            return eState.CANCELLED;
        }
        if (runCarPhysicsInLoop.Execute(numFramesToCalculate))
        {
            return eState.FINISHED;
        }
        return eState.CALCULATING;
    }

    public void Stop()
    {
        mState = eState.CANCELLED;
    }

    public void CalculatePerformanceIndex()
    {
        QuarterMileTime = carPhysics.SpeedMileStoneTimer.mQuarterMileTime;
        HalfMileTime = carPhysics.SpeedMileStoneTimer.mHalfMileTime;
        PerformanceIndex = CalculatePerformanceIndexWorker(QuarterMileTime);
        PerformanceIndexHalfMile = CalculatePerformanceIndexWorkerHalfMile(HalfMileTime);
        //Debug.Log(PerformanceIndex + "    " + PerformanceIndexHalfMile);
        int num = 0;
        int num2 = 0;
        CarTierEnum = GetCarTierFromPPIndex(PerformanceIndex, out num, out num2);
        if (ignoreTierChange)
        {
            CarTierEnum = carPhysics.CarTier;
        }

        CarTier = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(CarTierEnum));
    }

    public static eCarTier GetCarTierFromPPIndex(int PPindex, out int lowerPPForThisTier, out int upperPPForThisTier)
    {
        CarPerformanceIndexData carPPData;
#if UNITY_EDITOR
        carPPData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarPPData;
#else
        carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
#endif
        eCarTier result;
        if (PPindex >= carPPData.Tier1LowerLimit && PPindex <= carPPData.Tier1HigherLimit)
        {
            lowerPPForThisTier = carPPData.Tier1LowerLimit;
            upperPPForThisTier = carPPData.Tier1HigherLimit;
            result = eCarTier.TIER_1;
        }
        else if (PPindex >= carPPData.Tier2LowerLimit && PPindex <= carPPData.Tier2HigherLimit)
        {
            lowerPPForThisTier = carPPData.Tier2LowerLimit;
            upperPPForThisTier = carPPData.Tier2HigherLimit;
            result = eCarTier.TIER_2;
        }
        else if (PPindex >= carPPData.Tier3LowerLimit && PPindex <= carPPData.Tier3HigherLimit)
        {
            lowerPPForThisTier = carPPData.Tier3LowerLimit;
            upperPPForThisTier = carPPData.Tier3HigherLimit;
            result = eCarTier.TIER_3;
        }
        else if (PPindex >= carPPData.Tier4LowerLimit && PPindex <= carPPData.Tier4HigherLimit)
        {
            lowerPPForThisTier = carPPData.Tier4LowerLimit;
            upperPPForThisTier = carPPData.Tier4HigherLimit;
            result = eCarTier.TIER_4;
        }
        else if (PPindex >= carPPData.Tier5LowerLimit && PPindex <= carPPData.Tier5HigherLimit)
        {
            lowerPPForThisTier = carPPData.Tier5LowerLimit;
            upperPPForThisTier = carPPData.Tier5HigherLimit;
            result = eCarTier.TIER_5;
        }
        else
        {
            lowerPPForThisTier = 100;
            upperPPForThisTier = 200;
            result = eCarTier.TIER_1;
        }
        return result;
    }

    public static void GetPPRangeForCarTier(eCarTier carTier, out int lowerPPIndex, out int upperPPIndex)
    {
        lowerPPIndex = -1;
        upperPPIndex = -1;
        CarPerformanceIndexData carPPData;
#if UNITY_EDITOR
        carPPData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarPPData;
#else
        carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
#endif
        switch (carTier)
        {
            case eCarTier.TIER_1:
                lowerPPIndex = carPPData.Tier1LowerLimit;
                upperPPIndex = carPPData.Tier1HigherLimit;
                break;
            case eCarTier.TIER_2:
                lowerPPIndex = carPPData.Tier2LowerLimit;
                upperPPIndex = carPPData.Tier2HigherLimit;
                break;
            case eCarTier.TIER_3:
                lowerPPIndex = carPPData.Tier3LowerLimit;
                upperPPIndex = carPPData.Tier3HigherLimit;
                break;
            case eCarTier.TIER_4:
                lowerPPIndex = carPPData.Tier4LowerLimit;
                upperPPIndex = carPPData.Tier4HigherLimit;
                break;
            case eCarTier.TIER_5:
                lowerPPIndex = carPPData.Tier5LowerLimit;
                upperPPIndex = carPPData.Tier5HigherLimit;
                break;
        }
    }

    public static float GetPPIndexRatioWithinTier(int zPPIndex)
    {
        int num = 0;
        int num2 = 0;
        GetCarTierFromPPIndex(zPPIndex, out num, out num2);
        return (zPPIndex - num) / (float)(num2 - num);
    }

    public static float GetPPIndexRatioWithinTierIgnoreTierBoundaries(int zPPIndex, eCarTier baseCarTier)
    {
        int num = 0;
        int num2 = 0;
        int carTierFromPPIndex = (int)GetCarTierFromPPIndex(zPPIndex, out num, out num2);
        if (carTierFromPPIndex > (int)baseCarTier)
        {
            GetCarTierFromPPIndex(num - 1, out num, out num2);
        }
        float value = (zPPIndex - num) / (float)(num2 - num);
        return Mathf.Clamp01(value);
    }

    public static int CalculatePerformanceIndexWorker(float quarterMileTime)
    {
        CarPerformanceIndexData carPPData;
#if UNITY_EDITOR
        carPPData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarPPData;
#else
        carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
#endif
        int value;
        if (quarterMileTime <= carPPData.PP700Time)
        {
            value = InterpolateIndex(carPPData.PP700Time, carPPData.PP800Time, quarterMileTime, 800);
        }
        else if (quarterMileTime <= carPPData.PP600Time)
        {
            value = InterpolateIndex(carPPData.PP600Time, carPPData.PP700Time, quarterMileTime, 700);
        }
        else if (quarterMileTime <= carPPData.PP500Time)
        {
            value = InterpolateIndex(carPPData.PP500Time, carPPData.PP600Time, quarterMileTime, 600);
        }
        else if (quarterMileTime <= carPPData.PP400Time)
        {
            value = InterpolateIndex(carPPData.PP400Time, carPPData.PP500Time, quarterMileTime, 500);
        }
        else if (quarterMileTime <= carPPData.PP300Time)
        {
            value = InterpolateIndex(carPPData.PP300Time, carPPData.PP400Time, quarterMileTime, 400);
        }
        else if (quarterMileTime <= carPPData.PP200Time)
        {
            value = InterpolateIndex(carPPData.PP200Time, carPPData.PP300Time, quarterMileTime, 300);
        }
        else if (quarterMileTime <= carPPData.PP100Time)
        {
            value = InterpolateIndex(carPPData.PP100Time, carPPData.PP200Time, quarterMileTime, 200);
        }
        else
        {
            value = InterpolateIndex(20f, carPPData.PP100Time, quarterMileTime, 100);
        }
        return Mathf.Clamp(value, 0, 1000);
    }


    public static int CalculatePerformanceIndexWorkerHalfMile(float halfMileTime)
    {
        CarPerformanceIndexData carPPData;
#if UNITY_EDITOR
        carPPData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarPPData;
#else
        carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
#endif
        int value;
        if (halfMileTime <= carPPData.PP700HMTime)
        {
            value = InterpolateIndex(carPPData.PP700HMTime, carPPData.PP800HMTime, halfMileTime, 800);
        }
        else if (halfMileTime <= carPPData.PP600HMTime)
        {
            value = InterpolateIndex(carPPData.PP600HMTime, carPPData.PP700HMTime, halfMileTime, 700);
        }
        else if (halfMileTime <= carPPData.PP500HMTime)
        {
            value = InterpolateIndex(carPPData.PP500HMTime, carPPData.PP600HMTime, halfMileTime, 600);
        }
        else if (halfMileTime <= carPPData.PP400HMTime)
        {
            value = InterpolateIndex(carPPData.PP400HMTime, carPPData.PP500HMTime, halfMileTime, 500);
        }
        else if (halfMileTime <= carPPData.PP300HMTime)
        {
            value = InterpolateIndex(carPPData.PP300HMTime, carPPData.PP400HMTime, halfMileTime, 400);
        }
        else if (halfMileTime <= carPPData.PP200HMTime)
        {
            value = InterpolateIndex(carPPData.PP200HMTime, carPPData.PP300HMTime, halfMileTime, 300);
        }
        else if (halfMileTime <= carPPData.PP100HMTime)
        {
            value = InterpolateIndex(carPPData.PP100HMTime, carPPData.PP200HMTime, halfMileTime, 200);
        }
        else
        {
            value = InterpolateIndex(20f, carPPData.PP100HMTime, halfMileTime, 100);
        }
        return Mathf.Clamp(value, 0, 1000);
    }

    private static int InterpolateIndex(float zHigherTime, float zLowerTime, float zActualTime, int zHigherPerfIndex)
    {
        float num = (zHigherTime - zLowerTime) / 100f;
        var minus = Mathf.RoundToInt((zActualTime - zLowerTime)/num);
        //Debug.Log("zHigherTime:" + zHigherTime + " , zLowerTime:" + zLowerTime
        //          + " , zActualTime:" + zActualTime + " , zHigherPerfIndex:" + zHigherPerfIndex
        //          + " , minus:" + minus + " , ppIndex:" + (zHigherPerfIndex - minus));
        return zHigherPerfIndex - minus;
    }

    private static float InterpolateQMTimeFromPPIndexRatio(float slowEndTime, float fastEndLimitTime, int lowerPPBoundary, int upperPPBoundary, int incomingPP)
    {
        int num = upperPPBoundary - lowerPPBoundary;
        float num2 = (incomingPP - lowerPPBoundary) / (float)num;
        num2 = Mathf.Clamp(num2, 0f, 1f);
        return slowEndTime - (slowEndTime - fastEndLimitTime) * num2;
    }

    public static float GetQMTimeForPPIndex(int PPindex)
    {
        CarPerformanceIndexData carPPData;
        if (Application.isPlaying)
        {
            carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
        }
        else
        {
            carPPData =
                ResourceManager.GetSharedAsset<CarsConfiguration>("CarsConfiguration",
                    ServerItemBase.AssetType.configuration).CarPPData;
        }
        if (PPindex > carPPData.Tier5LowerLimit)
        {
            return InterpolateQMTimeFromPPIndexRatio(carPPData.PP600Time, carPPData.PP800Time, carPPData.Tier5LowerLimit, carPPData.Tier5HigherLimit, PPindex);
        }
        if (PPindex > carPPData.Tier4LowerLimit)
        {
            return InterpolateQMTimeFromPPIndexRatio(carPPData.PP500Time, carPPData.PP600Time, carPPData.Tier4LowerLimit, carPPData.Tier4HigherLimit, PPindex);
        }
        if (PPindex > carPPData.Tier3LowerLimit)
        {
            return InterpolateQMTimeFromPPIndexRatio(carPPData.PP400Time, carPPData.PP500Time, carPPData.Tier3LowerLimit, carPPData.Tier3HigherLimit, PPindex);
        }
        if (PPindex > carPPData.Tier2LowerLimit)
        {
            return InterpolateQMTimeFromPPIndexRatio(carPPData.PP300Time, carPPData.PP400Time, carPPData.Tier2LowerLimit, carPPData.Tier2HigherLimit, PPindex);
        }
        return InterpolateQMTimeFromPPIndexRatio(carPPData.PP100Time, carPPData.PP300Time, carPPData.Tier1LowerLimit, carPPData.Tier1HigherLimit, PPindex);
    }
}
