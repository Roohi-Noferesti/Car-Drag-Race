using EnhancedScrollerDemos.SelectionDemo;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DummyCellView : EnhancedScrollerCellView,IPointerClickHandler
{
	[SerializeField] private bool m_canDarg;
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (m_canDarg)
			GetComponentInParent<ScrollRect> ().OnInitializePotentialDrag (eventData);
	}
}
