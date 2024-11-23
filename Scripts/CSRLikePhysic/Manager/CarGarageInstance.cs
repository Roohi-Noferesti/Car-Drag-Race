using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

#if UNITY_EDITOR
[Serializable]
#endif
public class CarGarageInstance : ICarSimpleSpec,ISerializationCallbackReceiver
{
    [Serializable]
    public class CarUpgradeStatusDict
    {
        public eUpgradeType UpgradeType;
        public CarUpgradeStatus Status;
    }


    public string CarDBKey;

    [SerializeField]
    private CarUpgradeStatusDict[] m_upgradeStatusDict;

    public Dictionary<eUpgradeType, CarUpgradeStatus> UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>();

    public List<string> OwnedLiveries;

    public List<string> NewLiveries;

    public List<string> UnlockedLiveries;

    public float[] GearRatioSetup;

    public float FinalDriveRatio;

    public float NitrousLengthPowerRatio;

    public int AppliedColourIndex;

    public Color ColorOverride;

    public bool UseColorOverride;

    public string AppliedLiveryName;

    public VirtualEquipItemCollection EquipItemCollection = new VirtualEquipItemCollection();

    private string LongPhraseDbKey
    {
        get { return "_" + CarDBKey; }
    }

    public string AppliedBodyPaintItemID
    {
        get { return GetEquipedVirtualItemID(VirtualItemType.BodyShader); }
    }

    public string AppliedStickerItemID
    {
        get { return GetEquipedVirtualItemID(VirtualItemType.CarSticker); }

    }
    public string AppliedRingColorItemID
    {
        get { return GetEquipedVirtualItemID(VirtualItemType.RingShader); }

    }
    public string AppliedFrontlightColorItemID
    {
        get { return GetEquipedVirtualItemID(VirtualItemType.HeadLighShader); }

    }
    public string AppliedSpoilerItemID
    {
        get { return GetEquipedVirtualItemID(VirtualItemType.CarSpoiler); }
    }

    public float AppliedBodyHeight
    {
        get { return BodyHeight; }
    }

    public string GetEquipedVirtualItemID(VirtualItemType itemType, bool useShortPhrase = true,string defaultValue = null)
    {
        var item = EquipItemCollection.GetEquipedItem(itemType,CarDBKey);
        return item != null ? GetCorrectItemID(item.VirtualItemID, useShortPhrase) : defaultValue;
    }

    public string GetCorrectItemID(string itemID, bool useShortPhrase)
    {
        //if(itemID.ToLower()=="sticker_no" || itemID.ToLower() == "spoiler_no")
        //    return itemID;
        return useShortPhrase ? GetShortEquipedItemID(itemID) : GetLongEquipedItemID(itemID);
    }

    private string GetShortEquipedItemID(string itemID)
    {
        return itemID.Replace(LongPhraseDbKey, string.Empty);
    }

    public string GetLongEquipedItemID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return null;
        return itemID.Contains(LongPhraseDbKey) ? itemID : itemID + LongPhraseDbKey;
    }

    public bool IsOwned(string itemID,bool useShortPhrase=true)
    {
        if (string.IsNullOrEmpty(itemID))
            return false;
        return EquipItemCollection.Exists(GetCorrectItemID(itemID, useShortPhrase));
    }

    public float BestQuarterMileTime;

    public float BestHalfMileTime;

    public float DistanceTravelled;

    public int RacesAttempted;

    public int RacesWon;

    public int MoneySpentOnUpgrades;

    public int MoneySpentOnPropertyItem;

    public int NumUpgradesBought;

    public int NumPropertyBought;

    public float TopSpeedAttained;

    public int CustomCarNags;

    public NitrousData BaseNitrousData;

    public NumberPlate NumberPlate;

    public eCarTier CurrentTier;

    public int CurrentPPIndex;

    public float TightLoopQuarterMileTime;

    public float TightLoopQuarterMileTimeAdjust;

    public bool EliteCar;

    public bool SportsUpgrade;

    public float BodyHeight;

    public bool IsEliteLiveryApplied
    {
        get
        {
            return !string.IsNullOrEmpty(this.AppliedLiveryName) && this.AppliedLiveryName.ToLower().EndsWith("elite");
        }
    }

    public bool IsEliteSportsLiveryApplied
    {
        get
        {
            return !string.IsNullOrEmpty(this.AppliedLiveryName) &&
                   this.AppliedLiveryName.ToLower().EndsWith("elite_sports");
        }
    }

    [Obsolete("Use CarDatabase.Instance.GetCar(string carID).BaseCarTier instead", true)]
    public eCarTier GetBaseCarTier
    {
        get { return eCarTier.TIER_1; }
    }

    public void SetupNewGarageInstance(CarInfo car)
    {
        this.CarDBKey = car.Key;
        this.UpgradeStatus.Clear();
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            this.UpgradeStatus[current] = new CarUpgradeStatus();
            this.UpgradeStatus[current].levelOwned = 0;
            this.UpgradeStatus[current].levelFitted = 0;
        }
        this.OwnedLiveries = new List<string>();
        this.NewLiveries = new List<string>();
        this.UnlockedLiveries = new List<string>();
        this.CurrentTier = car.BaseCarTier;
        this.CurrentPPIndex = car.BasePerformanceIndex;
        this.TightLoopQuarterMileTime = 0f;
        this.TightLoopQuarterMileTimeAdjust = 0f;
        this.AppliedLiveryName = car.DefaultLiveryBundleName;
        this.GearRatioSetup = new float[car.BaseGearBoxData.GearRatios.Length];
        car.BaseGearBoxData.GearRatios.CopyTo(this.GearRatioSetup, 0);
        this.FinalDriveRatio = car.BaseGearBoxData.FinalGearRatio;
        this.NitrousLengthPowerRatio = 0.5f;
        this.NumberPlate = new NumberPlate();
        this.NumberPlate.SetToInactiveColours();
        this.EliteCar = false;
        this.SportsUpgrade = false;
        this.ColorOverride = Color.magenta;
        this.UseColorOverride = false;
    }

    public IEnumerable<string> GetUnlockedLiverySuffixes()
    {
        return from name in this.UnlockedLiveries
            select string.Format("_Unlockable_{0}", name);
    }

    [Obsolete("Use CarDataDefaults.IsBossCar instead.", true)]
    public bool IsBossCar()
    {
        return false; //CarDataDefaults.IsBossCar(this.CarDBKey);
    }

    public bool GetIsFullyUpgraded()
    {
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            if (this.UpgradeStatus[current].levelOwned < CarUpgradeData.NUM_UPGRADE_LEVELS)
            {
                return false;
            }
        }
        return true;
    }

    public bool GetIsFullyFitted()
    {
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            if (this.UpgradeStatus[current].levelFitted < CarUpgradeData.NUM_UPGRADE_LEVELS)
            {
                return false;
            }
        }
        return true;
    }

    public bool GetIsAsFittedAsPossible(CarInfo car)
    {
        if (!car.UsesEvoUpgrades())
        {
            return this.GetIsFullyFitted();
        }
        int num = 0;
        for (int i = 0; i < (int) CarUpgradeData.NUM_UPGRADE_LEVELS; i++)
        {
            num += Mathf.Min(car.EvolutionUpgradesEarned(i), CarUpgrades.ValidUpgrades.Count);
        }
        int num2 = 0;
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            num2 += (int) this.UpgradeStatus[current].levelFitted;
        }
        return num2 >= num;
    }

    public void SetAndApplyUpgradeLevelToAllUpgrades(int l)
    {
        byte b = CarUpgradeStatus.Convert(l);
        foreach (CarUpgradeStatus current in this.UpgradeStatus.Values)
        {
            current.levelOwned = b;
            current.levelFitted = b;
        }
    }

    public void ApplyLivery(string liveryName)
    {
        //CarInfo car = CarDatabase.Instance.GetCar(this.CarDBKey);
        //if (AsyncSwitching.IsLiveryName(liveryName))
        //{
        //    this.AppliedLiveryName = liveryName;
        //}
        //else
        //{
        //    this.AppliedLiveryName = car.DefaultLiveryBundleName;
        //}
    }

    public string ID
    {
        get { return CarDBKey; }
    }

    public int PPIndex
    {
        get { return CurrentPPIndex; }
    }

    public eCarTier Tier
    {
        get { return CarDatabase.Instance.GetCar(CarDBKey).BaseCarTier; }
    }

    public int GarageBaseCashValue
    {
        get
        {
            CarInfo car = CarDatabase.Instance.GetCar(this.CarDBKey);
            //if (this.IsUncommonCar())
            //{
            //    return car.GachaGarageCashValue;
            //}
            return car.BuyPrice;
        }
    }

    public bool SetAppliedItem(string id,bool useShortPhrase = true)
    {
        var correcItemID = GetCorrectItemID(id, useShortPhrase);
        return EquipItemCollection.EquipItem(correcItemID,CarDBKey);
    }

    public void GivePropertyItem(string selectedItemID, bool useShortPhrase, VirtualItemType itemType)
    {
        var itemID = GetCorrectItemID(selectedItemID, useShortPhrase);
        if (EquipItemCollection.Exists(itemID))
        {
            GTDebug.Log(GTLogChannel.CarVisuals, String.Format("'{0}' already purchased!Ignore it...",itemID));
            return;
        }

        EquipItemCollection.Add(new VirtualEquipItem()
        {
            VirtualItemID = itemID,
            CarID = CarDBKey,
            Equiped = false,
            ItemType = itemType
        });
    }

    public void SetupRandomVisual()
    {
        var body = CarCustomiseUtility.GetRandomEquipItem(VirtualItemType.BodyShader);
        var ring = CarCustomiseUtility.GetRandomEquipItem(VirtualItemType.RingShader);
        var headLight = CarCustomiseUtility.GetRandomEquipItem(VirtualItemType.HeadLighShader);
        var spoiler = CarCustomiseUtility.GetRandomEquipItem(VirtualItemType.CarSpoiler);
        var sticker = CarCustomiseUtility.GetRandomEquipItem(VirtualItemType.CarSticker);
        EquipItemCollection.Add(body);
        EquipItemCollection.Add(ring);
        EquipItemCollection.Add(headLight);
        if (spoiler!=null && !string.IsNullOrEmpty(spoiler.VirtualItemID))
            EquipItemCollection.Add(spoiler);
        if (sticker!=null && !string.IsNullOrEmpty(sticker.VirtualItemID))
            EquipItemCollection.Add(sticker);
    }

    public int CalculateCashValueUpgradeBonus()
    {
        int num = 0;
        int num2 = 0;
        int num3 = (this.Tier < eCarTier.TIER_2) ? 5 : 6;
        foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        {
            num += this.UpgradeStatus[current].levelOwned;
            num2 += num3;
        }
        float num4 = (float)num / (float)num2 * 0.25f;
        return (int)(num4 * (float)this.GarageBaseCashValue);
    }

    public int CalculateCashValueFusionBonus()
    {
        //int num = 0;
        //int num2 = 0;
        //int num3 = (this.Tier < eCarTier.TIER_2) ? 5 : 6;
        //foreach (KeyValuePair<eUpgradeType, CarUpgradeStatus> current in this.UpgradeStatus)
        //{
        //    for (int i = 0; i <= num3; i++)
        //    {
        //        num += current.Value.levels[i].CalculateNumFusedSlots();
        //        num2 += CarUpgrades.GetNumFusingSlots(this.FusingSlotProfileIndex, current.Key, this.Tier, i);
        //    }
        //}
        //float num4 = (float)this.GarageBaseCashValue * (0.05f * (float)this.CarTier.ToTierIndex());
        //float num5 = num4 * ((float)num / (float)num2);
        //return (int)num5;
        return 0;
    }

    public void OnBeforeSerialize()
    {
        m_upgradeStatusDict = new CarUpgradeStatusDict[UpgradeStatus.Count];
        int i = 0;
        foreach (var carUpgradeStatuse in UpgradeStatus)
        {
            m_upgradeStatusDict[i] = new CarUpgradeStatusDict()
            {
                UpgradeType = carUpgradeStatuse.Key,
                Status = carUpgradeStatuse.Value
            };
            i++;
        }
    }

    public void OnAfterDeserialize()
    {
        UpgradeStatus.Clear();
        foreach (var carUpgradeStatuse in m_upgradeStatusDict)
        {
            UpgradeStatus.Add(carUpgradeStatuse.UpgradeType, carUpgradeStatuse.Status);
        }
    }
}
