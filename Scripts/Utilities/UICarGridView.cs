using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UICarGridView :BasicGridView
{
    [SerializeField]
    private UICarItem m_carItemTemplate;
    [SerializeField]
    private int m_itemCount = 40;

    //private ScrollRect m_scrollRect;

    public void Populate(ICarSimpleSpec[] carList,Texture2D[] thumbnails ,string selectedCarID)
    {
        m_carItemTemplate.gameObject.SetActive(false);
        if (m_items == null)
        {
            m_items = new List<UIBasicItem>();
        }
        else
        {
            Clear();
        }


        for (int i = 0; i < (m_itemCount <= 0 ? carList.Length: m_itemCount); i++)
        {
            var item = (GameObject)Instantiate(m_carItemTemplate.gameObject, Vector3.zero, Quaternion.identity);
            item.SetActive(true);
            item.transform.SetParent(m_grid.transform, false);
            var uiItem = item.GetComponent<UICarItem>();
            //m_items.Add(uiItem);
            if (i < carList.Length)
            {
                uiItem.SetActive(true);
                uiItem.Name = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCarShortName(carList[i].ID));
                uiItem.Rate = carList[i].PPIndex;
                //uiItem.ID = carList[i].ID;
                if (thumbnails != null)
                    uiItem.Icon = thumbnails[i];

                if (carList[i].ID == selectedCarID)
                {
                    //uiItem.IsSelected = true;
                }
            }
            else
            {
                uiItem.SetActive(false);
            }
        }

    }

    private void Clear()
    {
        foreach (var uiCarItem in m_items)
        {
            Destroy(uiCarItem.gameObject);
        }
        m_items.Clear();
    }

    public override void Init()
    {
        m_carItemTemplate.gameObject.SetActive(false);
        //m_scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void SetThumbnails(Texture2D[] thumbnails)
    {
        for (int i = 0; i < m_itemCount; i++)
        {
            //if (i < thumbnails.Length)
            //    (m_items[i] as UICarItem).Icon = thumbnails[i];
        }
    }
}
