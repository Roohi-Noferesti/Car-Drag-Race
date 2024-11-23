using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("GT/CarPhysics/Engine")]
public class Engine : MonoBehaviour
{
    private float mThrottle;

    private CarPhysics mCarPhysics;

    private float mTargetRPM;

    private float mCurrentRPM;

    private EngineData mEngineData;

    private bool bustingTheEngine;

    private InGameTorqueCurve torqueCurve;

    private float mRevLimiterTimer;

    private float mLaunchThrottle;

    private int mCachedPeakHorsePower;

    private bool flipVisibleEngineWheelSpinRPMDirection;

    private float maxRPMWheelSpinVariation = 165f;

    private int RPMWheelSpinVariationPeriod = 4;

    private float currentRPMWheelSpinVariation;

    private float mLaunchWheelSpin;

    public float Throttle
    {
        get
        {
            return this.mThrottle;
        }
        set
        {
            this.mThrottle = value;
        }
    }

    public bool NitrousInput
    {
        private get;
        set;
    }

    public CarPhysics CarPhysics
    {
        set
        {
            this.mCarPhysics = value;
        }
    }

    public float TargetRPM
    {
        get
        {
            return this.mTargetRPM;
        }
    }

    public float CurrentRPM
    {
        get
        {
            return this.mCurrentRPM;
        }
    }

    public float LastFrameRPM
    {
        get;
        private set;
    }

    public float LaunchRPM
    {
        get;
        private set;
    }

    public EngineData EngineData
    {
        set
        {
            this.mEngineData = value;
        }
    }

    public float EngineForce
    {
        get;
        private set;
    }

    public float CurrentHorsePower
    {
        get;
        private set;
    }

    public float GaugeVisibleRPM
    {
        get;
        private set;
    }

    public float NitrousTimeLeft
    {
        get;
        private set;
    }

    public float NitrousConsumableExtraTime
    {
        get;
        private set;
    }

    public float NitrousTimeLeftAtStart
    {
        get;
        private set;
    }

    public float NitrousHorsePowerIncrease
    {
        get;
        private set;
    }

    public bool BustingTheEngine
    {
        get
        {
            return this.bustingTheEngine;
        }
        set
        {
            this.bustingTheEngine = value;
        }
    }

    public float AutoLauchRPM
    {
        private get;
        set;
    }

    public InGameTorqueCurve TorqueCurve
    {
        get
        {
            return this.torqueCurve;
        }
        set
        {
            this.torqueCurve = value;
        }
    }

    public float TrueRedLineRPM
    {
        get;
        set;
    }

    public bool HasLaunched
    {
        get;
        private set;
    }

    public float RedLineRPM
    {
        get;
        set;
    }

    public float IdleRPM
    {
        get;
        private set;
    }

    public bool SuperNitrousAvailable
    {
        get;
        set;
    }

    public float SuperNitrousFactor
    {
        get;
        set;
    }

    private bool NitrousInputAtAnyPointInRace
    {
        get;
        set;
    }

    public float NitrousTuneTimePercentChange
    {
        get;
        set;
    }

    public float NitrousTunePowerPercentChange
    {
        get;
        set;
    }

    public float LaunchWheelSpin
    {
        get
        {
            return this.mLaunchWheelSpin;
        }
    }

    public void Initialise(float fettledEngineFactor)
    {
        this.torqueCurve = new InGameTorqueCurve();
        this.torqueCurve.SetFromAnimationCurve(this.mEngineData.BaseTorqueCurve.animationCurve, fettledEngineFactor);
        this.TrueRedLineRPM = this.torqueCurve.MaxRPMValue;
        this.RedLineRPM = this.TrueRedLineRPM;
        this.IdleRPM = this.torqueCurve.MinRPMValue;
        this.SuperNitrousAvailable = false;
        this.SuperNitrousFactor = 0f;
        this.NitrousTuneTimePercentChange = 0f;
        this.NitrousTunePowerPercentChange = 0f;
    }

    private void Start()
    {
        this.ResetEngine();
    }

    public void ResetEngine()
    {
        float num = this.mCurrentRPM = (this.mTargetRPM = this.IdleRPM);
        this.GaugeVisibleRPM = num;
        this.LastFrameRPM = num;
        this.HasLaunched = false;
        this.mLaunchWheelSpin = 0f;
        this.AutoLauchRPM = -1f;
        this.mRevLimiterTimer = -1f;
        this.NitrousInputAtAnyPointInRace = false;
        this.NitrousInput = false;
        this.mThrottle = 0f;
        this.bustingTheEngine = false;
        this.NitrousTimeLeft = this.mCarPhysics.NitrousData.Duration;
        this.NitrousHorsePowerIncrease = this.mCarPhysics.NitrousData.HorsePowerIncrease;
        this.NitrousConsumableExtraTime = 0f;
        this.NitrousHorsePowerIncrease *= 1f + this.NitrousTunePowerPercentChange / 100f;
        this.NitrousTimeLeft *= 1f + this.NitrousTuneTimePercentChange / 100f;
        this.NitrousTimeLeftAtStart = this.NitrousTimeLeft;
        this.LaunchRPM = 0f;
        this.flipVisibleEngineWheelSpinRPMDirection = false;
        this.currentRPMWheelSpinVariation = 0f;
        this.ResetSuperNitrous();
        this.mCachedPeakHorsePower = this.CalculatePeakHorsePower();
    }

    public void ResetSuperNitrous()
    {
        if (!this.SuperNitrousAvailable)
        {
            return;
        }
        this.NitrousTimeLeft = this.mCarPhysics.NitrousData.SuperNitrousDuration * this.SuperNitrousFactor;
        this.NitrousHorsePowerIncrease = this.mCarPhysics.NitrousData.SuperNitrousHorsePowerIncrease * this.SuperNitrousFactor;
        this.NitrousTimeLeftAtStart = this.NitrousTimeLeft;
    }

    public void CalculateEngineForce()
    {
        float num = this.CalculateAcceleration();
        this.EngineForce = num * this.mCarPhysics.EffectiveMass;
    }

    public void OrderedUpdate()
    {
        this.LastFrameRPM = this.mCurrentRPM;
        if (this.mRevLimiterTimer > 0f && this.mCarPhysics.GearBox.IsInNeutral)
        {
            this.mThrottle = 0f;
        }
        this.mThrottle = Mathf.Clamp(this.mThrottle, 0f, 1f);
        this.mRevLimiterTimer -= Time.fixedDeltaTime;
        float num = PhysicsConstants.WheelRPMFromLinearSpeed(this.mCarPhysics.TireData.WheelRadius, this.mCarPhysics.SpeedMS) * this.mCarPhysics.GearBox.CombinedGearRatio;
        float num2 = this.RedLineRPM * this.mThrottle;
        this.mTargetRPM = num2;
        this.mTargetRPM = Mathf.Clamp(this.mTargetRPM, this.IdleRPM, this.RedLineRPM);
        this.mCarPhysics.IsUsingNitrous = false;
        if ((this.NitrousInput || (!this.mCarPhysics.NitrousData.IsContinuous && this.NitrousInputAtAnyPointInRace)) && this.NitrousTimeLeft > 0f)
        {
            this.NitrousInputAtAnyPointInRace = true;
            this.NitrousTimeLeft -= Time.fixedDeltaTime;
            this.mCarPhysics.IsUsingNitrous = true;
        }
        this.CalculateEngineForce();
        if (this.mCarPhysics.GearBox.IsInNeutral)
        {
            if (this.mTargetRPM > this.mCurrentRPM)
            {
                this.mCurrentRPM += this.mEngineData.EngineRevRate * 1.5f * Time.fixedDeltaTime;
            }
            else
            {
                float value = (this.mCurrentRPM - this.IdleRPM) / (this.RedLineRPM - this.IdleRPM);
                float num3 = Mathf.Clamp(value, 0.6f, 1f);
                this.mCurrentRPM -= this.mEngineData.EngineRevRate * num3 * num3 * Time.fixedDeltaTime;
            }
            if (this.AutoLauchRPM != -1f)
            {
                this.mCurrentRPM = this.AutoLauchRPM;
            }
            this.mCurrentRPM = Mathf.Clamp(this.mCurrentRPM, this.IdleRPM, this.RedLineRPM);
            this.GaugeVisibleRPM = this.mCurrentRPM;
        }
        else
        {
            if (!this.HasLaunched)
            {
                this.HasLaunched = true;
                this.mLaunchThrottle = 1f;
                this.mThrottle = this.mLaunchThrottle;
                this.CalculateEngineForce();
                this.mCarPhysics.Wheels.CalculateWheelSpin();
                this.mLaunchWheelSpin = this.mCarPhysics.Wheels.WheelSpin;
                this.LaunchRPM = this.mCurrentRPM;
            }
            if (this.mCarPhysics.Wheels.WheelSpin > 0f && num < this.mCurrentRPM)
            {
                this.mCurrentRPM += 1f * this.mCarPhysics.Wheels.WheelSpin + 1f;
                this.VaryVisibleRPMForWheelSpin();
            }
            else
            {
                float num4 = (this.mCurrentRPM - num) / 2f + num;
                float clutch = this.mCarPhysics.GearBox.Clutch;
                this.mCurrentRPM = num4 * clutch + this.mCurrentRPM * (1f - clutch);
                this.mCurrentRPM = Mathf.Clamp(this.mCurrentRPM, this.IdleRPM, this.RedLineRPM);
                this.GaugeVisibleRPM = this.mCurrentRPM;
            }
        }
        this.CurrentHorsePower = this.CurrentEnginePowerOutput();
    }

    private void VaryVisibleRPMForWheelSpin()
    {
        this.GaugeVisibleRPM = this.mCurrentRPM + this.currentRPMWheelSpinVariation;
        this.currentRPMWheelSpinVariation += this.maxRPMWheelSpinVariation * 0.5f / (float)this.RPMWheelSpinVariationPeriod * ((!this.flipVisibleEngineWheelSpinRPMDirection) ? -1f : 1f);
        if (this.currentRPMWheelSpinVariation > this.maxRPMWheelSpinVariation * 0.5f)
        {
            this.currentRPMWheelSpinVariation = this.maxRPMWheelSpinVariation * 0.5f - 0.001f;
            this.flipVisibleEngineWheelSpinRPMDirection = !this.flipVisibleEngineWheelSpinRPMDirection;
        }
        if (this.currentRPMWheelSpinVariation < -this.maxRPMWheelSpinVariation * 0.5f)
        {
            this.currentRPMWheelSpinVariation = -this.maxRPMWheelSpinVariation * 0.5f + 0.001f;
            this.flipVisibleEngineWheelSpinRPMDirection = !this.flipVisibleEngineWheelSpinRPMDirection;
        }
        this.GaugeVisibleRPM = Mathf.Clamp(this.GaugeVisibleRPM, this.IdleRPM, this.RedLineRPM);
    }

    public float CalculateAcceleration()
    {
        if (this.mCurrentRPM >= this.RedLineRPM && this.mRevLimiterTimer < 0f && this.mCarPhysics.GearBox.IsInNeutral)
        {
            this.mRevLimiterTimer = this.mEngineData.RevLimiterTime * 0.25f;
        }
        float num = PhysicsConstants.WheelRPMFromLinearSpeed(this.mCarPhysics.TireData.WheelRadius, this.mCarPhysics.SpeedMS) * this.mCarPhysics.GearBox.CombinedGearRatio;
        float num2 = num - this.RedLineRPM;
        if (num2 > 0f)
        {
            float num3 = (num2 - 50f) / 500f;
            num3 = Mathf.Clamp01(num3);
            if (!this.mCarPhysics.GearBox.IsInNeutral && !this.mCarPhysics.GearBox.IsGearShifting)
            {
                this.BustingTheEngine = true;
            }
            return -num3 * (float)this.mCachedPeakHorsePower * 0.5f;
        }
        float num4 = this.CalculateAccelerationAtRPMAndGear(this.mCurrentRPM, this.mCarPhysics.GearBox.CurrentGear);
        return num4 * this.mThrottle;
    }

    private float CalculateNitrousEffect()
    {
        float result = 0f;
        if (this.mCarPhysics.IsUsingNitrous && this.NitrousTimeLeft > 0f)
        {
            result = this.NitrousHorsePowerIncrease * 5252f / this.mCurrentRPM;
        }
        return result;
    }

    private float CurrentEngineRawPowerOutput()
    {
        float torque = this.CurrentEngineRawEngineTorque();
        return this.PowerFromTorque(torque);
    }

    private float CurrentEngineRawEngineTorque()
    {
        float normlisedRPM = this.GetNormlisedRPM(this.mCurrentRPM);
        return this.torqueCurve.EvaluateTorqueAtNormalisedRPM(normlisedRPM);
    }

    private float CurrentEnginePowerOutput()
    {
        float num = this.CurrentEngineRawEngineTorque();
        num += this.CalculateNitrousEffect();
        return this.PowerFromTorque(num);
    }

    private float PowerFromTorque(float torque)
    {
        float num = torque * this.mCurrentRPM / 5252f;
        return num * this.mCarPhysics.HorsePowerDisplayRatio;
    }

    public float CalculateWouldBeForceAtEachTyre()
    {
        int zGear = (!this.mCarPhysics.GearBox.IsInNeutral) ? this.mCarPhysics.GearBox.CurrentGear : 1;
        float num = this.CalculateAccelerationAtRPMAndGear(this.mCurrentRPM, zGear);
        float num2 = num * this.mThrottle * this.mCarPhysics.EffectiveMass * this.mCarPhysics.GearBox.Clutch;
        return num2 / (float)this.mCarPhysics.Wheels.NumPoweredWheels;
    }

    public float CalculateAccelerationAtRPMAndGear(float zRPM, int zGear)
    {
        float normlisedRPM = this.GetNormlisedRPM(zRPM);
        float extraTorqueFromNitrousInFootPounds = this.CalculateNitrousEffect();
        float num = CarPhysicsCalculations.EvaluateTorqueAtWheelAtThisRPM(this.torqueCurve, normlisedRPM, extraTorqueFromNitrousInFootPounds, this.mCarPhysics.GearBox.GearRatio(zGear) * this.mCarPhysics.GearBox.GearRatioFinal);
        return num / (this.mCarPhysics.TireData.WheelRadius * this.mCarPhysics.EffectiveMass);
    }

    public float GetNormlisedRPM(float zRPM)
    {
        float num = this.TrueRedLineRPM - this.IdleRPM;
        float value = (zRPM - this.IdleRPM) / num;
        return Mathf.Clamp01(value);
    }

    public int TruePeakHorsePower()
    {
        bool useTrueRedLineRPMForCalculation = true;
        float num = this.PeakPowerRPM(0.01f, useTrueRedLineRPMForCalculation);
        return Mathf.FloorToInt(num * this.torqueCurve.EvaluateTorqueAtNormalisedRPM(this.GetNormlisedRPM(num)) / 5252f);
    }

    public int HorsePowerAtIdle()
    {
        return Mathf.FloorToInt(this.IdleRPM * this.torqueCurve.EvaluateTorqueAtNormalisedRPM(this.GetNormlisedRPM(0f)) / 5252f);
    }

    public int CalculatePeakHorsePower()
    {
        bool useTrueRedLineRPMForCalculation = false;
        float num = this.PeakPowerRPM(0.01f, useTrueRedLineRPMForCalculation);
        return Mathf.FloorToInt(num * this.torqueCurve.EvaluateTorqueAtNormalisedRPM(this.GetNormlisedRPM(num)) / 5252f);
    }

    public float PeakPowerRPM(float zTorqueGraphXAxisIncrement, bool useTrueRedLineRPMForCalculation)
    {
        float num = Mathf.Clamp(zTorqueGraphXAxisIncrement, 0.01f, 0.99f);
        float result = 0f;
        float num2 = 0f;
        float num3 = this.TrueRedLineRPM - this.IdleRPM;
        float num4 = (!useTrueRedLineRPMForCalculation) ? this.RedLineRPM : this.TrueRedLineRPM;
        for (float num5 = 0f; num5 < 1f; num5 += num)
        {
            float num6 = this.IdleRPM + num3 * num5;
            float num7 = this.torqueCurve.EvaluateTorqueAtNormalisedRPM(num5) * num6 / 5252f;
            if (num7 > num2 && num6 <= num4)
            {
                num2 = num7;
                result = num6;
            }
        }
        return result;
    }

    public float MaxEngineRPMWithNoWheelSpin(float zTorqueGraphXAxisIncrement)
    {
        float num = Mathf.Clamp(zTorqueGraphXAxisIncrement, 0.01f, 0.99f);
        float num2 = this.IdleRPM;
        float num3 = 0f;
        for (float num4 = 0f; num4 < 1f; num4 += num)
        {
            float num5 = this.torqueCurve.EvaluateTorqueAtNormalisedRPM(num4);
            float num6 = this.CalculateAccelerationAtRPMAndGear(num4 * this.TrueRedLineRPM, 1) * this.mCarPhysics.EffectiveMass / this.mCarPhysics.Wheels.GetTotalPoweredTyreForce();
            if (num5 > num3 && num6 < 1f && num2 <= this.RedLineRPM)
            {
                num3 = num5;
                num2 = num4 * this.TrueRedLineRPM;
            }
        }
        return num2;
    }

    [Conditional("CSR_DEBUG_LOGGING")]
    private static void EngineLog(string output)
    {
    }
}
