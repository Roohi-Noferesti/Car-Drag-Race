using System;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class MyCarScrollerController : CarouselScrollerController
{
    private ICarSimpleSpec[] m_myCarIDs;
    [SerializeField] private bool m_customThumbnail;
    private ToggleGroup m_group;
    [SerializeField] private float m_cellSize = 223;

    public event Action<string> SelectedItemClicked;

    protected override void Awake()
    {
        base.Awake();
        m_group = GetComponent<ToggleGroup>();
    }

    //protected override IEnumerator _toggleSelectedDelayed()
    //{
    //    yield return new WaitForEndOfFrame();
    //}

    public void SetData(ICarSimpleSpec[] cars)
    {
        m_myCarIDs = cars;
        //var ids = cars.Select(i => i.ID).ToArray();
        //ResourceUtility.GetCarsThumbnails(ids, thumbnailsLoadCompleted, true);
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        if (m_myCarIDs != null)
        {
            return Mathf.Min(m_myCarIDs.Length/3 + m_endDummyObjectCount, 61);
        }
        return 0;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return m_cellSize;
    }

    public override int SelectedIndex
    {
        set
        {
            var oldRow = m_selectedIndex / 3;
            var newRow = value / 3;
            m_selectedIndex = value;
            if (gameObject.activeInHierarchy && oldRow!=newRow)
            {
                JumpToIndex(m_selectedIndex/3);
            }
            else
            {
                OnSelectedIndexChanged(m_selectedIndex);
            }
        }
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int
    dataIndex, int cellIndex)
    {
        GroupCarCellView cellView = (GroupCarCellView)scroller.GetCellView(m_cellViewPrefab);
        //upgradeButtonView.IsSelected = CurrentUpgrade == upgrade;
        cellView.SelectedCallback = CellViewSelected;
        UpdateCellData(cellView, dataIndex, cellIndex);
        return cellView;
    }

    private void CellViewSelected(EnhancedScrollerCellView cellView, int cellIndex)
    {
        if (cellView != null)
        {
            var newSelectedIndex = (cellView.dataIndex*3) + cellIndex;

            if (newSelectedIndex == SelectedIndex)
            {
                OnSelectedItemClicked(SelectedID);
            }
            else
            {
                SelectedIndex = newSelectedIndex;
            }
        }
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        var carView = cellView as GroupCarCellView;

        carView.SetActiveAll(false);
        for (var subGroupIndex = 0; subGroupIndex < 3 && subGroupIndex < m_myCarIDs.Length; subGroupIndex++)
        {
            var subGroupDataIndex = (dataIndex*3) + subGroupIndex;
            //Debug.Log(dataIndex+"   "+subGroupIndex+"    "+subGroupDataIndex);
            if (subGroupDataIndex >= m_myCarIDs.Length)
                return;
            // do something with your subgroup cell here
            var car = m_myCarIDs[subGroupDataIndex];
            //m_items.Add(uiItem);
            if (!string.IsNullOrEmpty(car.ID))
            {
                carView[subGroupIndex].SetActive(true);
                carView[subGroupIndex].Name = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCarShortName(car.ID));
                carView[subGroupIndex].Rate = car.PPIndex;
                var carTier = ((int) car.Tier)+1;
                carView[subGroupIndex].Tier = LocalizationManager.GetTranslation("TEXT_TIER_" + carTier);
                carView[subGroupIndex].CarID = car.ID;
                carView[subGroupIndex].ReloadImage(m_customThumbnail);
                carView[subGroupIndex].IsSelected = SelectedIndex == subGroupDataIndex;
                carView[subGroupIndex].ToggleGroup = m_group;

                if (car.ID == SelectedID)
                {
                    //carView = true;
                }
            }
            else
            {
                carView[subGroupIndex].SetActive(false);
            }
        }
    }

    public string SelectedID
    {
        get
        {
            if (SelectedIndex > -1 && SelectedIndex < m_myCarIDs.Length)
                return m_myCarIDs[SelectedIndex].ID;
            return null;
        }
        set
        {
            var ids = m_myCarIDs.Select(i => i.ID).ToArray();
            SelectedIndex = Array.IndexOf(ids, value);
        }
    }

    protected virtual void OnSelectedItemClicked(string obj)
    {
        var handler = SelectedItemClicked;
        if (handler != null) handler(obj);
    }
}
