using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public static class CarCustomiseUtility 
{
    public static void SetupRandomFurthestShaders(CarVisuals carVisual, out string bodyShader, out string headlightShader, out string ringShader, out string sticker, out string spoiler, out float bodyHeight)
    {
        bodyShader = GetRandomItemID(VirtualItemType.BodyShader);
        headlightShader = GetRandomItemID(VirtualItemType.HeadLighShader);
        ringShader = GetRandomItemID(VirtualItemType.RingShader);
        sticker = GetRandomItemID(VirtualItemType.CarSticker);
        spoiler = GetRandomItemID(VirtualItemType.CarSpoiler);
        bodyHeight = Random.Range(carVisual.MinBodyHeight, carVisual.MaxBodyHeight);
    }


    public static string GetRandomItemID(VirtualItemType itemType)
    {
        var items = CarPropertyItemIDs.GetItemsByType(itemType);

        if (items != null && items.Length > 0)
        {
            return items[Random.Range(0, items.Length)];
        }
        return null;
    }

    public static VirtualEquipItem GetRandomEquipItem(VirtualItemType itemType)
    {
        VirtualEquipItem v = new VirtualEquipItem();
        var items = CarPropertyItemIDs.GetItemsByType(itemType);

        if (items != null && items.Length > 0)
        {
            var item = items[Random.Range(0, items.Length)];
            v.Equiped = true;
            v.ItemType = itemType;
            v.VirtualItemID = item;
            return v;
        }
        return null;
    }

    public static VirtualEquipItem GetEquipItemByID(VirtualItemType typ, string itemID, string carID)
    {
        VirtualEquipItem v = new VirtualEquipItem();
        v.Equiped = true;
        v.ItemType = typ;
        v.VirtualItemID = itemID;
        v.CarID = carID;
        return v;
    }
}
