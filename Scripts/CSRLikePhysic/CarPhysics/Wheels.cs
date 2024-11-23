using System;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("GT/CarPhysics/Wheels")]
public class Wheels : MonoBehaviour
{
    private TireData mTireData;

    private InGameWheelSpinVsTyreGripCurve wheelSpinVsGripCurve;

    private InGameWheelSpinVsAccelPenaltyCurve wheelSpinVsAccelPenaltyCurve;

    private CarPhysics mCarPhysics;

    private bool useFakeWheelSpinVsRPMGraph;

    private int ExtraFettleGrip;

    public TireData TireData
    {
        set
        {
            this.mTireData = value;
        }
    }

    public float WheelsRPM
    {
        get;
        private set;
    }

    public float WheelSpin
    {
        get;
        private set;
    }

    public bool SuperNitrousExtraGripAvailable
    {
        get;
        set;
    }

    public CarPhysics CarPhysics
    {
        set
        {
            this.mCarPhysics = value;
        }
    }

    public int NumPoweredWheels
    {
        get
        {
            int num = 0;
            if (this.mTireData.RearAxleDriven)
            {
                num += 2;
            }
            if (this.mTireData.FrontAxleDriven)
            {
                num += 2;
            }
            return num;
        }
    }

    private void Start()
    {
    }

    public void Initialise(int fettleTyreGrip)
    {
        this.wheelSpinVsGripCurve = new InGameWheelSpinVsTyreGripCurve();
        this.wheelSpinVsAccelPenaltyCurve = new InGameWheelSpinVsAccelPenaltyCurve();
        this.wheelSpinVsGripCurve.SetFromAnimationCurve(this.mTireData.WheelSpinGripCurve.animationCurve, 1f);
        this.wheelSpinVsAccelPenaltyCurve.SetFromAnimationCurve(this.mTireData.RPMVsExtraWheelSpinCurve.animationCurve, 1f);
        this.ExtraFettleGrip = fettleTyreGrip;
    }

    public void ResetWheels()
    {
        this.WheelsRPM = 0f;
        this.WheelSpin = 0f;
        this.useFakeWheelSpinVsRPMGraph = true;
    }

    public void OrderedUpdate()
    {
        this.CalculateWheelRPM();
        this.UpdateWheelSpin();
    }

    public void CalculateWheelSpin()
    {
        this.WheelSpin = 0f;
        float totalPoweredTyreForce = this.GetTotalPoweredTyreForce();
        if (this.mCarPhysics.Engine.EngineForce > totalPoweredTyreForce)
        {
            this.WheelSpin = this.mCarPhysics.Engine.EngineForce / totalPoweredTyreForce;
            this.WheelSpin -= 1f;
            this.WheelSpin = Mathf.Clamp(this.WheelSpin, 0f, 99999.9f);
        }
    }

    public void ResetWheelSpin()
    {
        this.WheelSpin = 0f;
    }

    public float EvaluateTyreGripFractionAtWheelSpin()
    {
        float zNormalisedWheelSpin = (this.WheelSpin - this.wheelSpinVsGripCurve.MinWheelSpinValue) / (this.wheelSpinVsGripCurve.MaxWheelSpinValue - this.wheelSpinVsGripCurve.MinWheelSpinValue);
        return this.wheelSpinVsGripCurve.EvaluateTyreGripAtWheelSpin(zNormalisedWheelSpin);
    }

    private void UpdateWheelSpin()
    {
        this.WheelSpin = this.mCarPhysics.Engine.EngineForce / this.GetTotalPoweredTyreForce();
        this.WheelSpin -= 1f;
        if (!this.mCarPhysics.GearBox.IsInNeutral)
        {
            float zNormalisedWheelSpin = (this.mCarPhysics.Engine.LaunchRPM - this.mCarPhysics.Engine.IdleRPM) / (this.mCarPhysics.Engine.TrueRedLineRPM - this.mCarPhysics.Engine.IdleRPM);
            if (this.wheelSpinVsAccelPenaltyCurve.EvaluateAccelPenaltyAtWheelSpin(zNormalisedWheelSpin) == 0f)
            {
                this.useFakeWheelSpinVsRPMGraph = false;
            }
        }
        if ((this.mCarPhysics.GearBox.CurrentGear == 1 && this.useFakeWheelSpinVsRPMGraph) || (this.mCarPhysics.GearBox.CurrentGear == 2 && this.useFakeWheelSpinVsRPMGraph && this.mCarPhysics.SpeedMPH < this.mCarPhysics.FirstGearTheoreticalSpeedMPH))
        {
            float zNormalisedWheelSpin2 = (this.mCarPhysics.Engine.LaunchRPM - this.wheelSpinVsAccelPenaltyCurve.MinWheelSpinValue) / (this.wheelSpinVsAccelPenaltyCurve.MaxWheelSpinValue - this.wheelSpinVsAccelPenaltyCurve.MinWheelSpinValue);
            this.WheelSpin += this.wheelSpinVsAccelPenaltyCurve.EvaluateAccelPenaltyAtWheelSpin(zNormalisedWheelSpin2);
        }
        this.WheelSpin = Mathf.Clamp(this.WheelSpin, 0f, 99999.9f);
    }

    public float GetTotalPoweredTyreForce()
    {
        int numPoweredWheels = this.NumPoweredWheels;
        float num = this.mTireData.TireGripMax;
        num += (float)this.mCarPhysics.ConsumableTyreFactor;
        if (this.mCarPhysics.IsUsingNitrous && this.SuperNitrousExtraGripAvailable)
        {
            num += this.mCarPhysics.NitrousData.SuperNitrousExtraTyreGrip;
        }
        num += (float)this.ExtraFettleGrip;
        return num * this.mTireData.RoadFrictionCoefficient * (float)numPoweredWheels;
    }

    private void CalculateWheelRPM()
    {
        this.WheelsRPM = PhysicsConstants.WheelRPMFromLinearSpeed(this.mCarPhysics.TireData.WheelRadius, this.mCarPhysics.SpeedMS);
    }

    [Conditional("CSR_DEBUG_LOGGING")]
    private void WheelLog(string output)
    {
    }

    public float GetNormalisedWheelSpin()
    {
        return (this.WheelSpin - this.wheelSpinVsGripCurve.MinWheelSpinValue) / (this.wheelSpinVsGripCurve.MaxWheelSpinValue - this.wheelSpinVsGripCurve.MinWheelSpinValue);
    }
}
