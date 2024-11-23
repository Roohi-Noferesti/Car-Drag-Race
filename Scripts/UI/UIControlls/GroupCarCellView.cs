using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroupCarCellView : SelectableScrollerCellView
{
    [SerializeField] private UICarItem[] m_carItemCellViews;

    public new Action<EnhancedScrollerCellView, int> SelectedCallback;

    public override void Awake()
    {

    }

    public void OnSelected(int index)
    {
        if (SelectedCallback != null)
            SelectedCallback(this, index);
    }

    public UICarItem this[int index]
    {
        get { return m_carItemCellViews[index]; }
        set { m_carItemCellViews[index] = value; }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    public void SetActiveAll(bool value)
    {
        foreach (var carItemCellView in m_carItemCellViews)
        {
            carItemCellView.SetActive(value);
        }
    }
}
