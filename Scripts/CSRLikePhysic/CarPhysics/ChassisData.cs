using System;

[Serializable]
public class ChassisData
{
    public float Mass;

    public float CosmeticMass;

    public float DragCoefficient;

    public float Width;

    public float Height;

    public float LongitudinalBodyRollScaleFactor;

    public float LongitudinalBodyRollDamping;

    public float LongitudinalBodyRollScaleDownWithSpeed;

    public ChassisData()
    {
        this.SetDefaultData();
    }

    public void SetDefaultData()
    {
        this.Mass = 1500f;
        this.CosmeticMass = 0f;
        this.DragCoefficient = 0.31f;
        this.Width = 1.78f;
        this.Height = 1.28f;
        this.LongitudinalBodyRollScaleFactor = 0.34f;
        this.LongitudinalBodyRollDamping = 0.94f;
        this.LongitudinalBodyRollScaleDownWithSpeed = 150f;
    }

    public ChassisData Clone()
    {
        //ChassisData chassisData = new ChassisData();
        return base.MemberwiseClone() as ChassisData;
    }

    public void AssignFrom(ChassisData data)
    {
        this.Mass = data.Mass;
        this.DragCoefficient = data.DragCoefficient;
        this.Width = data.Width;
        this.Height = data.Height;
        this.LongitudinalBodyRollScaleFactor = data.LongitudinalBodyRollScaleFactor;
        this.LongitudinalBodyRollDamping = data.LongitudinalBodyRollDamping;
        this.LongitudinalBodyRollScaleDownWithSpeed = data.LongitudinalBodyRollScaleDownWithSpeed;
        this.CosmeticMass = data.CosmeticMass;
    }

    public override string ToString()
    {
        string text = "-----CHASSIS DATA-----\n\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Mass : ",
            this.Mass*2.20462251f,
            " pounds\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Cosmetic mass : ",
            this.CosmeticMass*2.20462251f,
            " pounds\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "DragCoefficient : ",
            this.DragCoefficient,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Width : ",
            this.Width,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Height : ",
            this.Height,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "LongitudinalBodyRollScaleFactor : ",
            this.LongitudinalBodyRollScaleFactor,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "LongitudinalBodyRollDamping : ",
            this.LongitudinalBodyRollDamping,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "LongitudinalBodyRollScaleDownWithSpeed : ",
            this.LongitudinalBodyRollScaleDownWithSpeed,
            "\n"
        });
    }
}
