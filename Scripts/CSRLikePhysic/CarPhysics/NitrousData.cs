using System;

[Serializable]
public class NitrousData
{
    public float Duration;

    public float HorsePowerIncrease;

    public float SuperNitrousDuration;

    public float SuperNitrousHorsePowerIncrease;

    public float SuperNitrousExtraTyreGrip;

    public bool IsContinuous { get; set; }

    public NitrousData()
    {
        this.SetDefaultData();
    }

    public void SetDefaultData()
    {
        this.Duration = 0.2f;
        this.HorsePowerIncrease = 50f;
    }

    public NitrousData Clone()
    {
        //NitrousData nitrousData = new NitrousData();
        return base.MemberwiseClone() as NitrousData;
    }

    public void AssignFrom(NitrousData data)
    {
        this.Duration = data.Duration;
        this.HorsePowerIncrease = data.HorsePowerIncrease;
        this.IsContinuous = data.IsContinuous;
        this.SuperNitrousDuration = data.SuperNitrousDuration;
        this.SuperNitrousHorsePowerIncrease = data.SuperNitrousHorsePowerIncrease;
        this.SuperNitrousExtraTyreGrip = data.SuperNitrousExtraTyreGrip;
    }

    public override string ToString()
    {
        string text = "-----NITROUS DATA-----\n\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Duration : ",
            this.Duration,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "HorsePowerIncrease : ",
            this.HorsePowerIncrease,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "IsContinuous : ",
            this.IsContinuous,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "SuperNitrousDuration : ",
            this.SuperNitrousDuration,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "SuperNitrousHorsePowerIncrease : ",
            this.SuperNitrousHorsePowerIncrease,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "SuperNitrousExtraTyreGrip : ",
            this.SuperNitrousExtraTyreGrip,
            "\n"
        });
    }
}
