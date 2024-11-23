using System;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class CarPropertyScrollerController : CarouselScrollerController
{
    [SerializeField] protected Sprite[] m_sprites;
    [SerializeField] protected Color[] m_colors;
    [SerializeField] private float m_cellSize = 196;

    public string[] IDs { get; set; }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        if (IDs != null)
            return IDs.Length + DummyObjectCount;
        return 0;
    }


    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return m_cellSize;
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        var imageCellView = cellView as ImageButtonCellView;
        imageCellView.name = IDs[dataIndex];
        char[] separator = {Convert.ToChar("_")};
        String[] strlist = IDs[dataIndex].Split(separator, 2);
        if (strlist[0] == "Sticker" || strlist[0] == "Spoiler")
        {
            SetSprite(imageCellView, dataIndex);
            SetColor(imageCellView, dataIndex);
        }
        else
        {
            int index = Convert.ToInt32(strlist[1]);
            index = index - 1;
            SetSprite(imageCellView, index);
            SetColor(imageCellView, index);
            
        }

    }

    protected virtual void SetSprite(ImageButtonCellView cellView,int dataIndex)
    {
        if (m_sprites.Length > 0)
        {
            cellView.Icon = m_sprites[dataIndex];
        }
    }


    protected virtual void SetColor(ImageButtonCellView cellView, int dataIndex)
    {
        if (m_colors.Length > 0)
            cellView.Color = m_colors[dataIndex];
    }

    public string SelectedID
    {
        get
        {
            if (IDs != null && SelectedIndex > m_startDummyObjectCount - 1 && SelectedIndex < IDs.Length + m_startDummyObjectCount)
                return IDs [SelectedIndex - m_startDummyObjectCount];

            //if (SelectedIndex > -1)
            //    return IDs[SelectedIndex];
            return null;
        }
        set
        {
            if (IDs != null)
            {
                var index = Array.IndexOf(IDs, value);
                SelectedIndex = Math.Max(index, 0)+ m_startDummyObjectCount;
            }
        }
    }
}
