using System.Collections;
using EnhancedScrollerDemos.SelectionDemo;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableScrollerCellView : EnhancedScrollerCellView,IPointerClickHandler
{
    public SelectedDelegate SelectedCallback;
    private Button m_button;
    private Animator m_animator;
    private bool m_isSelected;

    public virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_button = GetComponentInChildren<Button>();
        if (m_button != null)
            m_button.onClick.AddListener(OnSelect);
        //ToggleChanged(m_isSelected);
    }


    protected void ToggleChanged(bool value)
    {
        if (m_animator != null && m_animator.isInitialized)
        {
            m_animator.SetBool("HighLighted", value);
        }
    }


    public bool IsSelected
    {
        get { return m_isSelected; }
        set
        {
            m_isSelected = value;
            ToggleChanged(m_isSelected);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (m_button == null)
        {
            OnSelect();
            GetComponentInParent<ScrollRect>().OnInitializePotentialDrag(eventData);
        }
    }

    protected virtual void OnSelect()
    {
        if (SelectedCallback != null) SelectedCallback(this);
    }
}
