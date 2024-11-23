using System;
using UnityEngine;

[Serializable]
public class TireData
{
    public float WheelRadius;

    public float RollingFrictionCoefficient;

    public float RoadFrictionCoefficient;

    public float TireGripMax;

    public bool FrontAxleDriven;

    public bool RearAxleDriven;

    public EditableWheelSpinVsTyreGripCurve WheelSpinGripCurve;

    public EditableRPMVsExtraWheelSpinCurve RPMVsExtraWheelSpinCurve;

    public TireData()
    {
        this.SetDefaultData();
    }

    public void SetDefaultData()
    {
        this.WheelRadius = 0.3186f;
        this.RollingFrictionCoefficient = 0.015f;
        this.RoadFrictionCoefficient = 1f;
        this.TireGripMax = 4000f;
        this.FrontAxleDriven = false;
        this.RearAxleDriven = true;
        //this.SetDefaultWheelSpinGripCurve();
        //this.SetDefaultRPMVsExtraWheelSpinCurve();
    }

    public void SetDefaultWheelSpinGripCurve()
    {
        this.WheelSpinGripCurve = ScriptableObject.CreateInstance<EditableWheelSpinVsTyreGripCurve>();
        this.WheelSpinGripCurve.SetDefault();
    }

    public void SetDefaultRPMVsExtraWheelSpinCurve()
    {
        this.RPMVsExtraWheelSpinCurve = ScriptableObject.CreateInstance<EditableRPMVsExtraWheelSpinCurve>();
        this.RPMVsExtraWheelSpinCurve.SetDefault();
    }

    public TireData Clone()
    {
        //TireData tireData = new TireData();
        return base.MemberwiseClone() as TireData;
    }

    public void AssignFrom(TireData data)
    {
        this.WheelRadius = data.WheelRadius;
        this.RollingFrictionCoefficient = data.RollingFrictionCoefficient;
        this.RoadFrictionCoefficient = data.RoadFrictionCoefficient;
        this.TireGripMax = data.TireGripMax;
        this.RoadFrictionCoefficient = data.RoadFrictionCoefficient;
        this.FrontAxleDriven = data.FrontAxleDriven;
        this.RearAxleDriven = data.RearAxleDriven;
        this.WheelSpinGripCurve = data.WheelSpinGripCurve;
        this.RPMVsExtraWheelSpinCurve = data.RPMVsExtraWheelSpinCurve;
    }

    public override string ToString()
    {
        string text = "-----TYRE DATA-----\n\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Wheel Radius : ",
            this.WheelRadius*39.37008f,
            " inches\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "RollingFrictionCoefficient : ",
            this.RollingFrictionCoefficient,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "RoadFrictionCoefficient : ",
            this.RoadFrictionCoefficient,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "TireGripMax : ",
            this.TireGripMax,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "FrontAxleDriven : ",
            this.FrontAxleDriven,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "RearAxleDriven : ",
            this.RearAxleDriven,
            "\n"
        });
        text = text + "WheelSpin vs Tyre Grip Curve : " + this.WheelSpinGripCurve.ToString() + "\n";
        return text + "RPM vs Extra Wheel Spin : " + this.RPMVsExtraWheelSpinCurve.ToString() + "\n";
    }
}
