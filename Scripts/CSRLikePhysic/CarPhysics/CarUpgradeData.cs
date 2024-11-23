using System;
using UnityEngine;

[Serializable]
public class CarUpgradeData
{
    public static string[] UpgradeTypeStrings = new string[]
    {
        "ENGINE",
        "TURBO",
        "INTAKE",
        "NITROUS",
        "BODY",
        "TYRES",
        "TRANS",
        "ERROR",
        "NOTSET"
    };

    public static string[] UpgradeTypeStringsShort = new string[]
    {
        "eng",
        "tur",
        "int",
        "nit",
        "bod",
        "tir",
        "tra",
        "ERROR",
        "NOT_SET"
    };

    public static byte NUM_UPGRADE_LEVELS = 5;

    public string AssetDatabaseID;

    public float EngineRevRateDifference;

    public float NitrousTime;

    public string UpgradeLocalisationTextID;

    public float NitrousHPBoost;

    public bool IsNitrousContinuous;

    public int ExtraRPMAvailable;

    [HideInInspector] public InGameTorqueCurve TorqueCurveDifference;

    public EditableTorqueCurve AssignedFromDataTorqueCurve;

    public float DragCoeffDifference;

    public float WeightDifference;

    public float GripDifference;

    public float GearShiftTimeDifference;

    public eUpgradeType UpgradeType;

    public byte UpgradeLevel;

    public int CostInCash
    {
        get
        {
            return GameDatabase.Instance.Prices.GetUpgradeCashPrice(this.AssetDatabaseID);
        }
    }

    public int GoldPrice
    {
        get
        {
            return GameDatabase.Instance.Prices.GetUpgradeGoldPrice(this.AssetDatabaseID);
        }
    }

    public bool IsImportPart
    {
        get
        {
            return Application.isPlaying && this.UpgradeLevel > 3;
            //    && !CarDatabase.Instance.GetCar(this.AssetDatabaseID.Split(new char[]
            //{
            //    '_'
            //})[0]).UsesEvoUpgrades() ;
        }
    }

    public void Reset()
    {
        this.EngineRevRateDifference = 0f;
        this.NitrousTime = 0f;
        this.NitrousHPBoost = 0f;
        this.AssignedFromDataTorqueCurve = ScriptableObject.CreateInstance<EditableTorqueCurve>();
        this.AssignedFromDataTorqueCurve.SetDefault();
        this.DragCoeffDifference = 0f;
        this.WeightDifference = 0f;
        this.GripDifference = 0f;
        this.GearShiftTimeDifference = 0f;
        this.UpgradeType = eUpgradeType.INVALID;
        this.UpgradeLevel = 0;
    }

    public CarUpgradeData Clone()
    {
        //CarUpgradeData carUpgradeData = new CarUpgradeData();
        return base.MemberwiseClone() as CarUpgradeData;
    }

    public void AssignSimpleData(CarUpgradeData data)
    {
        this.AssetDatabaseID = data.AssetDatabaseID;
        this.EngineRevRateDifference = data.EngineRevRateDifference;
        this.UpgradeLocalisationTextID = data.UpgradeLocalisationTextID;
        this.NitrousHPBoost = data.NitrousHPBoost;
        this.NitrousTime = data.NitrousTime;
        this.DragCoeffDifference = data.DragCoeffDifference;
        this.WeightDifference = data.WeightDifference;
        this.GripDifference = data.GripDifference;
        this.GearShiftTimeDifference = data.GearShiftTimeDifference;
        this.UpgradeType = data.UpgradeType;
        this.UpgradeLevel = data.UpgradeLevel;
        this.ExtraRPMAvailable = data.ExtraRPMAvailable;
        this.IsNitrousContinuous = data.IsNitrousContinuous;
    }

    public void AssignFrom(CarUpgradeData data)
    {
        this.AssignSimpleData(data);
        this.TorqueCurveDifference = data.TorqueCurveDifference;
        this.AssignedFromDataTorqueCurve = data.AssignedFromDataTorqueCurve;
    }

    public void ConvertTorqueCurve()
    {
        if (this.AssignedFromDataTorqueCurve != null)
        {
            this.TorqueCurveDifference = new InGameTorqueCurve();
            this.TorqueCurveDifference.SetFromAnimationCurve(this.AssignedFromDataTorqueCurve.animationCurve, 1f);
        }
    }

    public override string ToString()
    {
        string text = "-----UPGRADES DATA-----\n\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            CarUpgrades.UpgradeNamesShort[this.UpgradeType],
            " Stage ",
            this.UpgradeLevel,
            "\n\n"
        });
        text = text + "Localisation Text ID : " + this.UpgradeLocalisationTextID + "\n";
        text = text + "AssetDatabaseID : " + this.AssetDatabaseID + "\n";
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "EngineRevRateDifference : ",
            this.EngineRevRateDifference,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "NitrousTime : ",
            this.NitrousTime,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "NitrousHPBoost: ",
            this.NitrousHPBoost,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "IsNitrousContinuous: ",
            this.IsNitrousContinuous,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "ExtraRPMAvailable: ",
            this.ExtraRPMAvailable,
            "\n"
        });
        if (this.AssignedFromDataTorqueCurve != null)
        {
            text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "Extra Torque Curve: ",
                this.AssignedFromDataTorqueCurve,
                "\n"
            });
        }
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "DragCoeffDifference: ",
            this.DragCoeffDifference,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "WeightDifference: ",
            this.WeightDifference,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "GripDifference: ",
            this.GripDifference,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "GearShiftTimeDifference: ",
            this.GearShiftTimeDifference,
            "\n"
        });
    }

    public void Process()
    {
        //InternTracker.Intern(ref this.UpgradeLocalisationTextID);
    }
}
