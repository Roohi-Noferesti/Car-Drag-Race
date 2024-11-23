using PurchasableItems;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAPDatabase : ConfigurationAssetLoader
{
    public IAPConfiguration Configuration
    {
        get;
        private set;
    }

    public bool ShouldShowWholeTeamConsumable
    {
        get
        {
            return this.Configuration.WholeTeamConsumableActive && this.Configuration.WholeTeamConsumableMinutes > 0;
        }
    }

    public int WholeTeamConsumableLengthMinutes
    {
        get
        {
            return this.Configuration.WholeTeamConsumableMinutes;
        }
    }

    public int WholeTeamConsumableRenewalLengthMinutes
    {
        get
        {
            return this.Configuration.WholeTeamRenewalMinutes;
        }
    }

    public int WholeTeamRenewalReminderMinutes
    {
        get
        {
            return this.Configuration.WholeTeamRenewalReminderMinutes;
        }
    }

    public int WholeTeamRenewalAvailableMinutes
    {
        get
        {
            return this.Configuration.WholeTeamRenewalAvailableMinutes;
        }
    }

    public int WholeTeamRenewalNotificationMinutes
    {
        get
        {
            return this.Configuration.WholeTeamRenewalNotificationMinutes;
        }
    }

    public string WholeTeamConsumableProductCode
    {
        get
        {
            return this.Configuration.WholeTeamConsumableProductCode;
        }
    }

    public string WholeTeamRenewalProductCode
    {
        get
        {
            return this.Configuration.WholeTeamRenewalProductCode;
        }
    }

    public string WholeTeamConsumableDescription
    {
        get
        {
            return this.Configuration.WholeTeamConsumableDescription;
        }
    }

    public string WholeTeamRenewalDescription
    {
        get
        {
            return this.Configuration.WholeTeamRenewalDescription;
        }
    }

    public bool IsUnlimitedFuelAvailable
    {
        get
        {
            return !string.IsNullOrEmpty(this.Configuration.UnlimitedFuelProductCode) && this.Configuration.UnlimitedFuelActive;
        }
    }

    public int UnlimitedFuelMinutes
    {
        get
        {
            return this.Configuration.UnlimitedFuelMinutes;
        }
    }

    public int UnlimitedFuelRenewalReminderMinutes
    {
        get
        {
            return this.Configuration.UnlimitedFuelRenewalReminderMinutes;
        }
    }

    public int UnlimitedFuelRenewalAvailableMinutes
    {
        get
        {
            return this.Configuration.UnlimitedFuelRenewalAvailableMinutes;
        }
    }

    public int UnlimitedFuelRenewalNotificationMinutes
    {
        get
        {
            return this.Configuration.UnlimitedFuelRenewalNotificationMinutes;
        }
    }

    public string UnlimitedFuelProductCode
    {
        get
        {
            return this.Configuration.UnlimitedFuelProductCode;
        }
    }

    public IAPDatabase()
        : base(GTAssetTypes.configuration_file, "IAPConfiguration")
    {
        this.Configuration = null;
    }

    public bool IsGasTankUpgradeAvailable()
    {
        return this.Configuration.UpgradedGasTankAvailable;
    }

    public int UpgradedGasTankSize()
    {
        return this.Configuration != null ? this.Configuration.UpgradedGasTankSize : 10;
    }

    public int GetGasTankReminderLowFuelAmount()
    {
        return this.Configuration.GasTankReminderLowFuelAmount;
    }

    public int GetGasTankReminderFrequencyInMinutes()
    {
        return this.Configuration.GasTankReminderFrequencyInMinutes;
    }

    public int GetGasTankReminderRepeatCount()
    {
        return this.Configuration.GasTankReminderRepeatCount;
    }

    public string GetGasTankProductCode()
    {
        PurchasableItem purchasableItem = this.GetPurchasableItems().FirstOrDefault((PurchasableItem q) => q.IAPCode.Contains("gastank"));
        return (purchasableItem == null) ? null : purchasableItem.IAPCode;
    }

    public IEnumerable<PurchasableItem> GetPurchasableItems()
    {
        return this.Configuration.PurchasableItems;
    }

    public IEnumerable<string> GetRestorableProductCodes()
    {
        return from q in this.Configuration.PurchasableItems
               where !q.IsOwned
               select q.IAPCode;
    }

    public PurchasableItem GetPurchasableItemForCar(string carDBKey)
    {
        return this.Configuration.PurchasableItems.Find((PurchasableItem item) => item.Type == PurchasableItem.ProductType.Car && item.Value.Contains(carDBKey));
    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (IAPConfiguration) scriptableObject;//JsonConverter.DeserializeObject<IAPConfiguration>(assetDataString);
        foreach (PurchasableItem current in this.Configuration.PurchasableItems)
        {
            current.Initialise();
        }
    }
}
