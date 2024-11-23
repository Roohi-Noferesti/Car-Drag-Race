using System;

[Serializable]
public class GearBoxData
{
    public float[] GearRatios;

    public float[] DebugChangeGearAtMPH;

    public float FinalGearRatio;

    public float RPMToShiftAutomatic;

    public float ClutchDelay;

    public float ClutchDelayFirstGear;

    public GearBoxData()
    {
        this.SetDefaultData();
    }

    public void SetDefaultData()
    {
        this.GearRatios = new float[6];
        this.GearRatios[0] = 3.82f;
        this.GearRatios[1] = 2.2f;
        this.GearRatios[2] = 1.52f;
        this.GearRatios[3] = 1.22f;
        this.GearRatios[4] = 1.02f;
        this.GearRatios[5] = 0.84f;
        this.FinalGearRatio = 3.44f;
        this.DebugChangeGearAtMPH = new float[5];
        this.DebugChangeGearAtMPH[0] = 0f;
        this.DebugChangeGearAtMPH[1] = 0f;
        this.DebugChangeGearAtMPH[2] = 0f;
        this.DebugChangeGearAtMPH[3] = 0f;
        this.DebugChangeGearAtMPH[4] = 0f;
        this.RPMToShiftAutomatic = 6500f;
        this.ClutchDelay = 0.2f;
        this.ClutchDelayFirstGear = 0.1f;
    }

    public GearBoxData Clone()
    {
        GearBoxData gearBoxData = new GearBoxData();
        gearBoxData = (base.MemberwiseClone() as GearBoxData);
        gearBoxData.GearRatios = new float[this.GearRatios.Length];
        gearBoxData.DebugChangeGearAtMPH = new float[this.GearRatios.Length];
        this.GearRatios.CopyTo(gearBoxData.GearRatios, 0);
        this.DebugChangeGearAtMPH.CopyTo(gearBoxData.DebugChangeGearAtMPH, 0);
        return gearBoxData;
    }

    public void AssignFrom(GearBoxData data)
    {
        this.GearRatios = new float[data.GearRatios.Length];
        data.GearRatios.CopyTo(this.GearRatios, 0);
        this.DebugChangeGearAtMPH = new float[data.DebugChangeGearAtMPH.Length];
        data.DebugChangeGearAtMPH.CopyTo(this.DebugChangeGearAtMPH, 0);
        this.FinalGearRatio = data.FinalGearRatio;
        this.RPMToShiftAutomatic = data.RPMToShiftAutomatic;
        this.ClutchDelay = data.ClutchDelay;
        this.ClutchDelayFirstGear = data.ClutchDelayFirstGear;
    }

    public override string ToString()
    {
        string text = "-----GEARBOX DATA-----\n\n";
        text = text + "GearRatios : " + this.PrintFloatArray(this.GearRatios) + "\n";
        text = text + "DebugChangeGearAtMPH : " + this.PrintFloatArray(this.DebugChangeGearAtMPH) + "\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "FinalGearRatio : ",
            this.FinalGearRatio,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "RPMToShiftAutomatic : ",
            this.RPMToShiftAutomatic,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "ClutchDelay : ",
            this.ClutchDelay,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "ClutchDelayFirstGear : ",
            this.ClutchDelayFirstGear,
            "\n"
        });
    }

    private string PrintFloatArray(float[] floatArray)
    {
        string text = string.Empty;
        for (int i = 0; i < floatArray.Length; i++)
        {
            string text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "[",
                i,
                "] : ",
                floatArray[i],
                ", "
            });
        }
        return text;
    }
}
