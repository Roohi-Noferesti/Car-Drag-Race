using System.Linq;
using KingKodeStudio;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

[RequireComponent(typeof(HUDScreen))]
public class CarPropertyItemAssociator : MonoBehaviour
{
    [SerializeField] private VirtualItemType m_itemType;
    public void Populate()
    {
        var items = GetComponent<ShopScreenBase>().Items;
        //var car = ZGameManager.DataManager.localPlayerInfo.UserSelectedCar;
        var carDbKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;

        var propertyItems = LocalStorage.GetCarPropertyItems(carDbKey, m_itemType).ToArray();

        for (int i = 0; i < items.Length; i++)
        {
            items[i].ID = propertyItems[i];
            GTDebug.Log(GTLogChannel.Other,propertyItems[i]);
        }
    }
}
