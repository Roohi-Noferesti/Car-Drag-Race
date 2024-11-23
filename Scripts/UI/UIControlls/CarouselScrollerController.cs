using System;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EnhancedScroller))]
public abstract class CarouselScrollerController : MonoBehaviour, IEnhancedScrollerDelegate, IEndDragHandler
{
    protected EnhancedScroller m_scroller;
	[SerializeField] protected DummyCellView m_dummyCellViewPrefab;
    [SerializeField] protected SelectableScrollerCellView m_cellViewPrefab;
    [SerializeField] protected bool m_useSnapping = true;
	[SerializeField] protected int m_startDummyObjectCount;
	[SerializeField] protected int m_endDummyObjectCount;
    protected int m_selectedIndex;
    public event Action<int> SelectedIndexChanged;
    protected DummyDataClass[] DummyDatas;
    private bool m_interactable = true;
    protected EnhancedScroller.TweenType m_originalTweenType;

    public bool Interactable
    {
        get { return m_interactable; }
        set
        {
            m_interactable = value;
            GetComponent<ScrollRect>().enabled = value;
        }
    }

    protected virtual int DummyObjectCount
	{
		get {
			return m_startDummyObjectCount + m_endDummyObjectCount;
		}
	}

    public virtual int SelectedIndex
    {
        get { return m_selectedIndex; }
        set
        {
            if (m_selectedIndex != value)
            {
                m_selectedIndex = value;
                if (gameObject.activeInHierarchy)
                {
                    JumpToIndex(m_selectedIndex);
                    //Debug.Log("carousel");
                }
                else
                {
                    OnSelectedIndexChanged(m_selectedIndex);
                }
            }
        }
    }

    protected virtual void Awake()
    {
        m_scroller = GetComponent<EnhancedScroller>();
        DummyDatas = new DummyDataClass[GetNumberOfCells(m_scroller)];
        m_originalTweenType = m_scroller.snapTweenType;
    }

    public virtual void Reload()
    {
        if (m_scroller != null)
        {
            DummyDatas = new DummyDataClass[GetNumberOfCells(m_scroller)];
            m_scroller.ReloadData();
            m_selectedIndex = -1;
        }
    }


    protected virtual void Start()
    {
        m_scroller.snapping = false;
        m_scroller.Delegate = this;
        if (m_useSnapping)
            m_scroller.scrollerSnapped += ScrollerSnapped;
        //m_scroller.ReloadData();
    }

	protected virtual int GetValidDataIndex(EnhancedScroller scroller,int dataIndex)
	{
		return Mathf.Clamp (dataIndex, m_startDummyObjectCount, GetNumberOfCells (scroller) - m_endDummyObjectCount-1);
	}

    protected virtual void ScrollerSnapped(EnhancedScroller scroller, int cellindex, int dataindex, EnhancedScrollerCellView cellView)
    {
		var validDataIndex = GetValidDataIndex (scroller, dataindex);
		m_selectedIndex = validDataIndex;
		OnSelectedIndexChanged(validDataIndex);
        m_scroller.snapping = false;
    }

    public abstract int GetNumberOfCells(EnhancedScroller scroller);

    public abstract float GetCellViewSize(EnhancedScroller scroller, int dataIndex);

    public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int
        dataIndex, int cellIndex)
    {
		if (DummyObjectCount > 0 && (dataIndex < m_startDummyObjectCount || dataIndex >= GetNumberOfCells (scroller) - m_endDummyObjectCount)) 
		{
			DummyCellView cellView = (DummyCellView)scroller.GetCellView (m_dummyCellViewPrefab);
			DummyDatas [dataIndex] = new DummyDataClass ();
			return cellView;
		} 
		else 
		{
			SelectableScrollerCellView cellView = (SelectableScrollerCellView)scroller.GetCellView (m_cellViewPrefab);
			//upgradeButtonView.IsSelected = CurrentUpgrade == upgrade;
			cellView.SelectedCallback = CellViewSelected;
			cellView.IsSelected = SelectedIndex == dataIndex;
			//Debug.Log(dataIndex + "   " + DummyDatas.Length);
			DummyDatas [dataIndex] = new DummyDataClass (cellView);
			var validDataIndex = dataIndex - m_startDummyObjectCount;
			UpdateCellData (cellView, validDataIndex, cellIndex);
			return cellView;
		}
    }

    public abstract void UpdateCellData(EnhancedScrollerCellView cellView, int
        dataIndex, int cellIndex);

    protected virtual void CellViewSelected(EnhancedScrollerCellView cellView)
    {
        if (cellView != null && Interactable)
        {
            SelectedIndex = cellView.dataIndex;
        }
    }

    protected virtual void JumpToIndex(int index)
    {
        m_scroller.JumpToDataIndex(index, m_scroller.snapWatchOffset, m_scroller.snapCellCenterOffset,
            m_scroller.snapUseCellSpacing, m_scroller.snapTweenType, m_scroller.snapTweenTime,
            () =>
            {
                OnJumpToIndexCompleted(index);
            });
    }

    protected virtual void OnJumpToIndexCompleted(int index)
    {
        OnSelectedIndexChanged(index);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (m_useSnapping)
        {
            m_scroller.snapping = true;
            //m_scroller.Snap();
        }
    }

    protected virtual void OnSelectedIndexChanged(int dataIndex)
    {
        for (int i = 0; i < DummyDatas.Length; i++)
        {
            if (DummyDatas[i] != null)
                DummyDatas[i].IsSelected = dataIndex == i;
        }

        if (gameObject.activeInHierarchy)
            StartCoroutine(_toggleSelectedDelayed());
        if (SelectedIndexChanged != null)
        {
            SelectedIndexChanged(dataIndex);
        }
    }

    protected virtual IEnumerator _toggleSelectedDelayed()
    {
        yield return new WaitForEndOfFrame();
        if (SelectedIndex > -1 && SelectedIndex < DummyDatas.Length)
        {
            if (DummyDatas[SelectedIndex] != null)
                DummyDatas[SelectedIndex].IsSelected = true;
        }
    }

    public void SetTweenTypeToImmediate()
    {
        m_scroller.snapTweenType = EnhancedScroller.TweenType.immediate;
    }

    public void ResetTweenType()
    {
        m_scroller.snapTweenType = m_originalTweenType;
    }

    public class DummyDataClass
    {
        private bool m_isSelected;
        public SelectableScrollerCellView cellView { get; private set; }
        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                if (cellView != null)
                    cellView.IsSelected = value;
            }
        }

        public DummyDataClass(SelectableScrollerCellView cellView)
        {
            this.cellView = cellView;
        }

		public DummyDataClass()
		{
			
		}
    }
}
