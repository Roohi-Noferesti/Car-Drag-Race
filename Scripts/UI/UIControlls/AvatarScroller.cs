using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using I2.Loc;
using UnityEngine;

public class AvatarScroller : CarouselScrollerController
{
    private string[] m_avatars;
    [SerializeField]
    private float m_cellViewSize = 190;


    public string SelectedID
    {
        get
        {
            if (m_avatars != null && SelectedIndex > m_startDummyObjectCount - 1 && SelectedIndex < m_avatars.Length + m_startDummyObjectCount)
                return m_avatars[SelectedIndex - m_startDummyObjectCount];
            return null;
        }
        set
        {
            if (m_avatars != null)
            {
                SelectedIndex = Array.IndexOf(m_avatars, value)+m_startDummyObjectCount;
            }
        }
    }

    public void SetAvatars(IEnumerable<string> avatars)
    {
        m_avatars = avatars.ToArray();
        Reload();
    }
    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        if (m_avatars != null)
            return m_avatars.Length + DummyObjectCount;
        return 0;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return m_cellViewSize;
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        var avatarCellView = (cellView as AvatarCellView);
        avatarCellView.AvatarID = m_avatars[dataIndex];
        avatarCellView.SetActive(true);
        avatarCellView.name = m_avatars[dataIndex]; //LocalizationManager.GetTranslation(CarDatabase.Instance.GetCarShortName(car.ID));
        avatarCellView.ReloadImage();
        avatarCellView.IsSelected = SelectedIndex == dataIndex;
    }
}
