using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class PredefinedUpgradeSetsData
{
    private const string ModifiedCarMassSeparator = ":";

    public int PPIndex;

    public string UpgradeData;

    public bool HasModifiedCarMass
    {
        get { return this.UpgradeData.IndexOf(":") >= 0; }
    }

    public void AssignFrom(PredefinedUpgradeSetsData data)
    {
        this.PPIndex = data.PPIndex;
        this.UpgradeData = data.UpgradeData;
    }

    public void SetFromUpgradeSetup(int PPIndexIn, CarUpgradeSetup upgradeSetup)
    {
        this.PPIndex = PPIndexIn;
        this.UpgradeData = string.Empty;
        byte[] value = new byte[]
        {
            (byte)
                ((int) upgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted << 4 |
                 (int) upgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted),
            (byte)
                ((int) upgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted << 4 |
                 (int) upgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted),
            (byte)
                ((int) upgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted << 4 |
                 (int) upgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted),
            (byte) ((int) upgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted << 4 | 0)
        };
        int num = BitConverter.ToInt32(value, 0);
        this.UpgradeData += string.Format("{0:X8}", num);
        if (upgradeSetup.ModifiedCarMass > 0f)
        {
            this.UpgradeData += string.Format(":{0:0.####}", upgradeSetup.ModifiedCarMass);
        }
    }

    public void FillUpgradeSetup(CarInfo carInfo, ref CarUpgradeSetup carSetup)
    {
        carSetup.CarDBKey = carInfo.Key;
        carSetup.FettleEnginePercent = 0f;
        carSetup.IsFettled = false;
        carSetup.FettleTyreAdd = 0;
        string value = this.UpgradeData.Substring(0, 8);
        byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(value, 16));
        carSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.BODY].levelOwned = CarUpgradeStatus.Convert(bytes[0] >> 4));
        carSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.ENGINE].levelOwned = CarUpgradeStatus.Convert((int) (bytes[0] & 15)));
        carSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.INTAKE].levelOwned = CarUpgradeStatus.Convert(bytes[1] >> 4));
        carSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.NITROUS].levelOwned = CarUpgradeStatus.Convert((int) (bytes[1] & 15)));
        carSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelOwned = CarUpgradeStatus.Convert(bytes[2] >> 4));
        carSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.TURBO].levelOwned = CarUpgradeStatus.Convert((int) (bytes[2] & 15)));
        carSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted =
            (carSetup.UpgradeStatus[eUpgradeType.TYRES].levelOwned = CarUpgradeStatus.Convert(bytes[3] >> 4));
        int num = this.UpgradeData.IndexOf(":");
        if (num == -1)
        {
            carSetup.ModifiedCarMass = 0f;
        }
        else
        {
            carSetup.ModifiedCarMass = float.Parse(this.UpgradeData.Substring(num + 1),
                CultureInfo.InvariantCulture.NumberFormat);
        }
    }

    public static CarUpgradeSetup InitialiseFromRequestedPPIndex(CarInfo carInfo, int basePerformanceIndex, int targetPP,
        out int closestPP, bool skipBodyMassModifier = true, bool skipPastMaxMassModifiers = false)
    {
        if (carInfo.PredefinedUpgradeSets == null || carInfo.PredefinedUpgradeSets.Length == 0)
        {
            closestPP = carInfo.BasePerformanceIndex;
            return CarUpgradeSetup.NullCarSetup;
        }
        if (targetPP <= carInfo.BasePerformanceIndex)
        {
            closestPP = carInfo.BasePerformanceIndex;
            return CarUpgradeSetup.NullCarSetup;
        }
        if (skipPastMaxMassModifiers)
        {
            int num = MaximumPPFromUpgrades(carInfo);
            if (targetPP > num)
            {
                closestPP = MaximumPPFromUpgrades(carInfo);
                return CarUpgradeSetup.NullCarSetup;
            }
        }
        closestPP = -1;
        int num2 = -1;
        int num3 = -1;
        for (int i = 0; i < carInfo.PredefinedUpgradeSets.Length; i++)
        {
            int pPIndex = carInfo.PredefinedUpgradeSets[i].PPIndex;
            if (!skipBodyMassModifier || !carInfo.PredefinedUpgradeSets[i].HasModifiedCarMass)
            {
                int num4 = Mathf.Abs(pPIndex - targetPP);
                if (num3 == -1 || num4 < num3)
                {
                    num3 = num4;
                    num2 = i;
                }
            }
        }
        if (num2 == -1)
        {
            closestPP = carInfo.BasePerformanceIndex;
            return CarUpgradeSetup.NullCarSetup;
        }
        CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
        carUpgradeSetup.CarDBKey = carInfo.Key;
        carInfo.PredefinedUpgradeSets[num2].FillUpgradeSetup(carInfo, ref carUpgradeSetup);
        closestPP = carInfo.PredefinedUpgradeSets[num2].PPIndex;
        return carUpgradeSetup;
    }

    public static int MaximumPPFromUpgrades(CarInfo carInfo)
    {
        int num = -1;
        int num2 = 0;
        while (num2 < carInfo.PredefinedUpgradeSets.Length && num == -1)
        {
            if (carInfo.PredefinedUpgradeSets[num2].UpgradeData.Contains("50555555"))
            {
                num = carInfo.PredefinedUpgradeSets[num2].PPIndex;
            }
            num2++;
        }
        return num;
    }
}
