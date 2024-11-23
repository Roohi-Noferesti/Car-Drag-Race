using System;

[Serializable]
public class EngineData
{
    public EditableTorqueCurve BaseTorqueCurve;

    public float EngineRevRate;

    public float RevLimiterTime = 0.15f;

    public EngineData()
    {
        this.SetDefaultData();
    }

    public void SetDefaultData()
    {
        //this.BaseTorqueCurve = ScriptableObject.CreateInstance<EditableTorqueCurve>();
        //this.BaseTorqueCurve.SetDefault();
        this.EngineRevRate = 4000f;
        this.RevLimiterTime = 0.15f;
    }

    public EngineData Clone()
    {
        //EngineData engineData = new EngineData();
        return base.MemberwiseClone() as EngineData;
    }

    public void AssignFrom(EngineData data)
    {
        this.EngineRevRate = data.EngineRevRate;
        this.RevLimiterTime = data.RevLimiterTime;
        this.BaseTorqueCurve = data.BaseTorqueCurve;
    }

    public override string ToString()
    {
        string text = "-----ENGINE DATA-----\n\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "Main Torque Curve : ",
            this.BaseTorqueCurve,
            "\n"
        });
        text += "....\n";
        text = text + this.BaseTorqueCurve.ToString() + "\n";
        text += "....\n";
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "EngineRevRate : ",
            this.EngineRevRate,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "RevLimiterTime : ",
            this.RevLimiterTime,
            "\n"
        });
    }
}
