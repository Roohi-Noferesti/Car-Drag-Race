using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;
using Random = UnityEngine.Random;

public class ServerItemDatabase : ConfigurationAssetLoader 
{
    public ServerItemConfiguration Configuration { get; private set; }
    public ServerItemDatabase()
        : base(GTAssetTypes.configuration_file, null)
    {

    }


    public ServerItemDatabase(bool loadConfig)
        : base(GTAssetTypes.configuration_file, "ServerItemConfiguration")
    {

    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        Configuration = (ServerItemConfiguration) scriptableObject;
    }

    public VirtualItem GetItemByID(string itemID)
    {
        return Configuration.Items.FirstOrDefault(i => i.ItemID == itemID);
    }

    public VirtualItem[] GetItemsByType(VirtualItemType itemType)
    {
        return Configuration.Items.Where(i => i.ItemType == itemType).ToArray();
    }

    public List<VirtualItem> GetTradeItemRelation(string tradeItemID)
    {
        var collection = new List<VirtualItem>();

        foreach (var virtualItem in Configuration.Items)
        {
            foreach (var itemRelation in virtualItem.ItemRelations)
            {
                if (itemRelation.VirtualTradeItemID == tradeItemID)
                {
                    collection.Add(virtualItem);
                    break;
                }
            }
        }
        return collection;
    }

    public int GetGoldPrice(string itemID)
    {
        var itemReltion = GetItemRelation(itemID, "gold");

        if (!itemReltion.IsNull)
            return itemReltion.FinalAmount;
        return 0;
    }

    public int GetCashPrice(string itemID)
    {
        var itemReltion = GetItemRelation(itemID, "cash");

        if (!itemReltion.IsNull)
            return itemReltion.FinalAmount;
        return 0;
    }

    private VirtualItemRelation GetItemRelation(string tradeItemID, string currencyItemID)
    {
        return GetTradeItemRelation(tradeItemID)
            .SelectMany(i => i.ItemRelations)
            .FirstOrDefault(i => i.VirtualTradeItemID == currencyItemID);
    }

    public VirtualItemRelation GetCostItemRelationForTradeItem(string tradeItemID, CostType costType)
    {
        var currencyItemID = "";
        switch (costType)
        {
                case CostType.CASH:
                currencyItemID = "cash";
                break;
                case CostType.GOLD:
                currencyItemID = "gold";
                break;
        }

        if (string.IsNullOrEmpty(currencyItemID))
            return new VirtualItemRelation();

        var virtualItems = GetTradeItemRelation(tradeItemID);
        var itemRelations = virtualItems.SelectMany(i => i.ItemRelations).ToArray();
        var itemRelationByCurrency = itemRelations.FirstOrDefault(i => i.VirtualTradeItemID == currencyItemID);
        return itemRelationByCurrency;
    }

    public VirtualEquipItem GetRandomEquipItem(VirtualItemType itemType)
    {
        VirtualEquipItem v = new VirtualEquipItem();
        var items = CarPropertyItemIDs.GetItemsByType(itemType);

        if (items != null && items.Length>0)
        {
            var item = items[Random.Range(0, items.Length)];
            v.Equiped = true;
            v.ItemType = itemType;
            v.VirtualItemID = item;
            return v;
        }
        return null;
    }


    public VirtualEquipItem GetEquipItemByID(VirtualItemType typ,string itemID,string carID)
    {
        VirtualEquipItem v = new VirtualEquipItem();
        v.Equiped = true;
        v.ItemType = typ;
        v.VirtualItemID = itemID;
        v.CarID = carID;
        return v;
    }


    public string GetRandomItemID(VirtualItemType itemType)
    {
        var items = CarPropertyItemIDs.GetItemsByType(itemType);

        if (items != null && items.Length > 0)
        {
            return items[Random.Range(0, items.Length)];
        }
        return null;
    }
}
