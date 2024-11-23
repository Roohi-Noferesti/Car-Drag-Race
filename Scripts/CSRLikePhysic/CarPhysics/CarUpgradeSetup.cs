using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarUpgradeSetup
{
    public string CarDBKey;

    public Dictionary<eUpgradeType, CarUpgradeStatus> UpgradeStatus;

    public float FettleEnginePercent;

    public int FettleTyreAdd;

    public float EngConsumableEnginePercent;

    public int EngConsumableTyreAdd;

    public float TyreConsumableEnginePercent;

    public int TyreConsumableTyreAdd;

    public float NitConsumableBodyAdd;

    public bool IsFettled;

    public bool EngineConsumableActive;

    public bool TyreConsumableActive;

    public bool NitrousConsumableActive;

    public float ModifiedCarMass;

    public static CarUpgradeSetup NullCarSetup
    {
        get
        {
            return new CarUpgradeSetup
            {
                IsFettled = false,
                FettleEnginePercent = 0f,
                FettleTyreAdd = 0
            };
        }
    }

    public CarUpgradeSetup()
    {
        this.UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>();
        SetDefaultUpgradeStatus(this.UpgradeStatus);
    }

    public static void SetDefaultUpgradeStatus(Dictionary<eUpgradeType, CarUpgradeStatus> status)
    {
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            status[current] = new CarUpgradeStatus();
        }
    }

    public void SetupConsumableParams(CarInfo carInfo, int ppIndex)
    {
        this.EngConsumableEnginePercent = carInfo.EngineConsumableEngineGraphSlope*(float) ppIndex +
                                          carInfo.EngineConsumableEngineGraphIntercept;
        this.EngConsumableTyreAdd =
            Mathf.RoundToInt(carInfo.EngineConsumableTyreGraphSlope*(float) ppIndex +
                             carInfo.EngineConsumableTyreGraphIntercept);
        this.TyreConsumableEnginePercent = carInfo.TyreConsumableEngineGraphSlope*(float) ppIndex +
                                           carInfo.TyreConsumableEngineGraphIntercept;
        this.TyreConsumableTyreAdd =
            Mathf.RoundToInt(carInfo.TyreConsumableTyreGraphSlope*(float) ppIndex +
                             carInfo.TyreConsumableTyreGraphIntercept);
        this.NitConsumableBodyAdd = carInfo.NitrousConsumableBodyGraphSlope*(float) ppIndex +
                                    carInfo.NitrousConsumableBodyGraphIntercept;
    }

    public void SetFullyUpgraded()
    {
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            CarUpgradeStatus carUpgradeStatus = this.UpgradeStatus[current];
            carUpgradeStatus.levelFitted = CarUpgradeData.NUM_UPGRADE_LEVELS;
        }
    }

    public void PickRandomUpgrades()
    {
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            CarUpgradeStatus carUpgradeStatus = this.UpgradeStatus[current];
            carUpgradeStatus.levelFitted =
                CarUpgradeStatus.Convert(Random.Range(0, (int) CarUpgradeData.NUM_UPGRADE_LEVELS));
        }
        this.UpgradeStatus[eUpgradeType.TYRES].levelFitted =
            CarUpgradeStatus.Convert(Random.Range(3, (int) CarUpgradeData.NUM_UPGRADE_LEVELS));
    }

    public void CalculateFettle(CarInfo carInfo, int pp)
    {
        float mechanicEngineFettleGraphSlope = carInfo.MechanicEngineFettleGraphSlope;
        float mechanicEngineFettleGraphIntercept = carInfo.MechanicEngineFettleGraphIntercept;
        float mechanicTyreGripFettleGraphSlope = carInfo.MechanicTyreGripFettleGraphSlope;
        float mechanicTyreGripFettleGraphIntercept = carInfo.MechanicTyreGripFettleGraphIntercept;
        this.FettleEnginePercent = mechanicEngineFettleGraphSlope*(float) pp + mechanicEngineFettleGraphIntercept;
        this.FettleTyreAdd = (int) (mechanicTyreGripFettleGraphSlope*(float) pp + mechanicTyreGripFettleGraphIntercept);
    }

    public override string ToString()
    {
        string text = "CarDBKey : " + this.CarDBKey + "\n";
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "TurboLevel : ",
            this.UpgradeStatus[eUpgradeType.TURBO].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "EngineLevel : ",
            this.UpgradeStatus[eUpgradeType.ENGINE].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "TyreLevel : ",
            this.UpgradeStatus[eUpgradeType.TYRES].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "TransmissionLevel : ",
            this.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "IntakeLevel: ",
            this.UpgradeStatus[eUpgradeType.INTAKE].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "NitrousLevel: ",
            this.UpgradeStatus[eUpgradeType.NITROUS].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "BodyLevel: ",
            this.UpgradeStatus[eUpgradeType.BODY].levelFitted,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "IsFettled: ",
            this.IsFettled,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "FettleTyreAdd: ",
            this.FettleTyreAdd,
            "\n"
        });
        text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "FettleEnginePercent: ",
            this.FettleEnginePercent,
            "\n"
        });
        text2 = text;
        return string.Concat(new object[]
        {
            text2,
            "ModifiedCarMass: ",
            this.ModifiedCarMass,
            "\n"
        });
    }

    public bool IsFullyUpgraded()
    {
        return this.UpgradeStatus.Values.All((CarUpgradeStatus q) => q.levelFitted >= CarUpgradeData.NUM_UPGRADE_LEVELS);
    }
}
