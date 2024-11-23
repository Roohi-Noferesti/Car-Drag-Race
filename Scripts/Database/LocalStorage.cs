using System.Collections.Generic;
using System.Linq;
using Z2HSharedLibrary.DatabaseEntity;

public class LocalStorage : MonoSingleton<LocalStorage>
{
    //[SerializeField] private ItemAssets m_itemAssets;

    //private static VirtualItem? GetItemInLocalStorage(string itemID)
    //{
    //    var serverItem = GameDatabase.Instance.ServerItemDatabase.GetItemByID(itemID);
    //    return !string.IsNullOrEmpty(serverItem.ItemID) ? serverItem : (VirtualItem?) null;
    //}

    //public static bool BuyItem(string itemID)
    //{
    //    var item = GameDatabase.Instance.ServerItemDatabase.GetItemByID(itemID); 

    //    if (item == null)
    //    {
    //        OnlineRaceGameEvents.Instance.OnLocalItemNotFound(itemID);
    //        return false;
    //    }

    //    var itemRelations = item.Item.ItemRelations.ItemList;

    //    if (itemRelations == null || itemRelations.Count==0)
    //    {
    //        OnlineRaceGameEvents.Instance.OnItemIsEmpty(itemID);
    //        return false;
    //    }

    //    var currencyItems = itemRelations.Where(i => i.Relation <= 0).ToArray();

    //    if (!CanBuyItem(itemID, currencyItems))
    //        return false;

    //    foreach (var virtualItemRelation in itemRelations.Where(i => i.Relation != 0))
    //    {
    //        var itemType = GetItemType(virtualItemRelation.VirtualTradeItemID);
    //        AddItemBalance(virtualItemRelation.VirtualTradeItemID, itemType,
    //            (int)(Mathf.Sign(virtualItemRelation.Relation) * virtualItemRelation.FinalAmount));
    //    }

    //    return true;
    //}

    //public static bool CanBuyItem(string itemID,VirtualItemRelation[] currencyItems = null)
    //{
    //    if (currencyItems == null)
    //    {
    //        var item = GameDatabase.Instance.ServerItemDatabase.GetItemByID(itemID);

    //        if (string.IsNullOrEmpty(item.ItemID))
    //        {
    //            return false;
    //        }

    //        var itemRelations = item.ItemRelations.ItemList;
    //        currencyItems = itemRelations.Where(i => i.Relation <= 0).ToArray();
    //    }

    //    //foreach (var currencyItem in currencyItems)
    //    //{
    //    //    var currencyBalance =
    //    //        Instance.m_user.UserBalances.ItemBalanceCollection.ItemList.FirstOrDefault(
    //    //            i => i.ItemID == currencyItem.VirtualTradeItemID);
    //    //    if (currencyBalance == null || currencyBalance.UserBalance < currencyItem.FinalAmount)
    //    //    {
    //    //        var balance = currencyBalance == null ? 0 : currencyBalance.UserBalance;
    //    //        Instance.m_event.OnLocalInsufficientFund(currencyItem.VirtualTradeItemID, balance,
    //    //            currencyItem.FinalAmount);
    //    //        return false;
    //    //    }
    //    //}

    //    return true;
    //}

    //private static void AddItemBalance(string itemID,VirtualItemType itemType, int value)
    //{
    //    //var itemBalance =
    //    //    Instance.m_user.UserBalances.ItemBalanceCollection.ItemList.FirstOrDefault(i => i.ItemID == itemID);


    //    //if (itemBalance == null)
    //    //{
    //    //    itemBalance = new VirtualItemBalance() {ItemID = itemID, UserBalance = 0};
    //    //    Instance.m_user.UserBalances.ItemBalanceCollection.Add(itemBalance);
    //    //}

    //    //itemBalance.UserBalance += value;

    //    //var eventArgs = new ItemBalanceChangedEventArgs
    //    //    (itemID, itemType, itemBalance.UserBalance, value);

    //    //Instance.m_event.OnLocalItemBalanceChanged(eventArgs);
    //}

    //public static bool EquipItem(string itemID)
    //{
    //    //var localItem = GetItemInLocalStorage(itemID);

    //    //if (localItem == null)
    //    //    return false;


    //    //var equipedItem = Instance.m_user.ItemEquipCollection.FirstOrDefault(i => i.ItemType == localItem.Value.ItemType);


    //    //string lastEquipItemID = null;
    //    //if (equipedItem != null)
    //    //{
    //    //    if (equipedItem.VirtualItemID == itemID)
    //    //    {
    //    //        //Item already equiped
    //    //        return false;
    //    //    }
    //    //    lastEquipItemID = equipedItem.VirtualItemID;
    //    //}
    //    //else
    //    //{
    //    //    equipedItem = new VirtualEquipItem()
    //    //    {
    //    //        //GroupCode = localItem.Value.Group,
    //    //        ItemType = localItem.Value.ItemType,
    //    //        VirtualItemID = localItem.Value.ItemID
    //    //    };
    //    //    Instance.m_user.ItemEquipCollection.Add(equipedItem);
    //    //}

    //    //equipedItem.VirtualItemID = localItem.Value.ItemID;

    //    //var eventArgs = new ItemEquipChangedEventArgs(equipedItem, lastEquipItemID);
    //    //Instance.m_event.OnLocalItemEquiped(eventArgs);
    //    return true;
    //}

    //public static VirtualItemType GetItemType(string itemID)
    //{
    //    var item = GetItemInLocalStorage(itemID);
    //    if (item != null)
    //    {
    //        return item.Value.ItemType;
    //    }
    //    else
    //    {
    //        return VirtualItemType.None;
    //        //throw new Exception(string.Format("Item '{0}' not found", itemID));
    //    }
    //}

    //public static VirtualItemRelationCollection GetItemRelations(string itemID)
    //{
    //    var item = GetItemInLocalStorage(itemID);
    //    if (item != null)
    //    {
    //        return item.Value.ItemRelations;
    //    }
    //    return null;
    //}

    /// <summary>
    /// return items relations wich trade by tradeItemID
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    //public static List<VirtualItem> GetTradeItemRelations(string tradeItemID)
    //{
    //    return GameDatabase.Instance.ServerItemDatabase.GetTradeItemRelation(tradeItemID);
    //}

    public static RaceReward GetBaseReward(ICarSimpleSpec specProvider)
    {
        //var raceRewards = Instance.m_itemAssets.GetRaceRewards();
        //var reward = raceRewards.FirstOrDefault(i => i.IsWinner &&
        //                         i.Difficulty == DifficultyDetails.Difficulty.Easy
        //                         && i.LevelID == 1 && i.RaceType == RaceDetails.RaceType.OneByOne
        //                         && i.Step == 0);
        //reward.Amount *= specProvider.PPIndex;
        //return reward;
        return new RaceReward();
    }


    //public static ICarSimpleSpec[] CarList
    //{
    //    get
    //    {
    //        var cars = Instance.m_itemAssets.GetCarsList();
    //        var carProvider = new ICarSimpleSpec[cars.Count];

    //        for (int i = 0; i < carProvider.Length; i++)
    //        {
    //            carProvider[i] = new DatabaseCarSpecProvider(cars[i], 1, 1, 1, 1, 0, 1);
    //        }

    //        return carProvider;
    //    }
    //}

    public static IEnumerable<string> GetCarPropertyItems(string carID, VirtualItemType itemType)
    {
        //var car = Instance.m_itemAssets.GetCar(carID);
        //return null;//car.PaintItems.Where(p => GetItemType(p) == itemType);
        return null;
    }
}
