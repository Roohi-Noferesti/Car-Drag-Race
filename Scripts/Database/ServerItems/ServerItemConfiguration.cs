using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class ServerItemConfiguration : ScriptableObject
{
    [SerializeField]
    private VirtualItem[] m_virtualItems;

    public VirtualItem[] Items
    {
        get
        {
            return m_virtualItems;
        }
    }

    public int ItemLength
    {
        get { return Items != null ? Items.Length : 0; }
    }

    public void UpdateItems(VirtualItemCollection items)
    {
        m_virtualItems = items.ToArray();
        //List<VirtualItem> preItems = new List<VirtualItem>(m_virtualItems);

        //for (int i = 0; i < items.Count; i++)
        //{
        //    var sameItem = preItems.FirstOrDefault(m => m.ItemID == items[i].ItemID);

        //    if (!string.IsNullOrEmpty(sameItem.ItemID))
        //    {
        //        sameItem.Item = items[i];
        //    }
        //    else
        //    {
        //        preItems.Add(new ServerItemBase()
        //        {
        //            Item = items[i]
        //        });
        //    }
        //}

        //m_virtualItems = preItems.ToArray();
    }

    public void AddItems(VirtualItemCollection items)
    {
        var list = new VirtualItemCollection(m_virtualItems);
        list.ItemList.AddRange(items);
        m_virtualItems = list.ToArray();
    }
}
