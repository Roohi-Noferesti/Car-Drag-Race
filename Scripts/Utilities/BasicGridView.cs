using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class BasicGridView :MonoBehaviour
{
    [SerializeField] protected GridLayoutGroup m_grid;
    [SerializeField] protected UnityAction m_onSelectedItemChanged;
    protected List<UIBasicItem> m_items;

    public void FetchItemsComponents()
    {
        m_items = m_grid.GetComponentsInChildren<UIBasicItem>(true).ToList();
    }

    public virtual UIBasicItem SelectedItem
    {
        get
        {
            return m_items == null ? null : m_items.FirstOrDefault(i => i.IsSelected);
        }
    }

    public UIBasicItem[] Items
    {
        get
        {
            return m_items.ToArray();
        }
    }

    public string SelectedItemID
    {
        get { return SelectedItem != null ? SelectedItem.ID : null; }
        set
        {
            if (m_items != null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var basicItem = m_items.FirstOrDefault(i => i.ID == value);
                    if (basicItem != null)
                    {
                        basicItem.IsSelected = true;
                    }
                    else
                    {
                        throw new Exception(string.Format("Item '{0}' not found exception", value));
                    }
                }
                else
                {
                    if (m_items != null)
                    {
                        var item = m_items.FirstOrDefault(i => i.IsSelected);
                        if (item != null)
                        {
                            item.IsSelected = false;
                        }
                    }
                }
            }
        }

    }

    public virtual void Init()
    {
        FetchItemsComponents();
    }
}
