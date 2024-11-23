using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class ItemAssets : ScriptableObject
{
    [SerializeField] private ServerItemBase[] m_items;
    [SerializeField] private RaceRewardCollection m_raceRewards;
    //[SerializeField] private RaceDetailsCollection m_raceDetailsList;
    //[SerializeField] private DifficultyDetailsCollection m_difficultyDetailsList;
    //[SerializeField] private CarDivisionsCollection m_carDivisionsList;
    //[SerializeField] private XPLevelsCollection m_xpLevels;
    [SerializeField] private CarCollection m_carsList;

    public ServerItemBase this[int index]
    {
        get { return m_items[index]; }
    }

    public int ItemLength
    {
        get { return m_items.Length; }
    }

    public ServerItemBase GetItemByID(string itemID)
    {
        return m_items.FirstOrDefault(i => i.Item.ItemID == itemID);
    }

    public List<VirtualItem> GetTradeItemRelation(string tradeItemID)
    {
        var collection = new List<VirtualItem>();

        foreach (var serverItemBase in m_items)
        {
            foreach (var itemRelation in serverItemBase.Item.ItemRelations)
            {
                if (itemRelation.VirtualTradeItemID == tradeItemID)
                {
                    collection.Add(serverItemBase.Item);
                    break;
                }
            }
        }
        return collection;
    }

    public void UpdateAssets(VirtualItemCollection items)
    {
        List<ServerItemBase> preItems = new List<ServerItemBase>(m_items);

        for (int i = 0; i < items.Count; i++)
        {
            var sameItem = preItems.FirstOrDefault(m => m.Item.ItemID == items[i].ItemID);

            if (sameItem != null)
            {
                sameItem.Item = items[i];
            }
            else
            {
                preItems.Add(new ServerItemBase()
                {
                    Item = items[i]
                });
            }
        }

        m_items = preItems.ToArray();
    }


    public void UpdateRaceRewards(RaceRewardCollection raceRewards)
    {
        m_raceRewards = raceRewards;
    }

    public void UpdateRaceDetails(RaceDetailsCollection racedetails)
    {
        //m_raceDetailsList = racedetails;
    }

    public void UpdateDifficultyDetails(DifficultyDetailsCollection difficultyDetails)
    {
        //m_difficultyDetailsList = difficultyDetails;
    }

    public void UpdateCarDivisions(CarDivisionsCollection carDivisionList)
    {
        //m_carDivisionsList = carDivisionList;
    }

    public void UpdateXpLevels(XPLevelsCollection xpLevels)
    {
        //m_xpLevels = xpLevels;
    }

    public void UpdateCarsList(CarCollection carList)
    {
        m_carsList = carList;
    }

    public RaceRewardCollection GetRaceRewards()
    {
        return m_raceRewards;
    }

    public CarCollection GetCarsList()
    {
        return m_carsList;
    }

    public CarInfo GetCar(string carID)
    {
        return m_carsList.FirstOrDefault(c => c.Key == carID);
    }
}
