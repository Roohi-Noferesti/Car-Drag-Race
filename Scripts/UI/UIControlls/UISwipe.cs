using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class UISwipe : MonoBehaviour
{
    //private bool m_touchDown;
    //private float m_touchPos
    [SerializeField]
    private bool m_toggle = true;

    [SerializeField] private UnityEvent m_onToggleOn;
    [SerializeField] private UnityEvent m_onToggleOff;
    [SerializeField] private DragEvent m_onSwipe;
    [SerializeField] private DragEvent m_onSwipeEnd;
    [SerializeField] private bool m_sendInputAxis;
    [SerializeField] private string m_horizontalAxisName;
    [SerializeField] private string m_verticalAxisName;
    [SerializeField] private string m_pinchAxisName;
    [SerializeField] private float m_swipeMuliplier = 1;
    [SerializeField] private float m_pinchMultiplier = 1;

    void Awake()
    {
        EasyTouch.On_Swipe += EasyTouch_On_Swipe;
        EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;
        EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
        if (!string.IsNullOrEmpty(m_pinchAxisName))
        {
            EasyTouch.On_Pinch += EasyTouch_On_Pinch;
            EasyTouch.On_PinchEnd += EasyTouch_On_PinchEnd;
        }
        //m_touchDown = value;
    }

    void OnDestroy()
    {
        EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
        EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;
        EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        if (!string.IsNullOrEmpty(m_pinchAxisName))
        {
            EasyTouch.On_Pinch -= EasyTouch_On_Pinch;
            EasyTouch.On_PinchEnd -= EasyTouch_On_PinchEnd;
        }
    }

    void EasyTouch_On_SwipeEnd(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject)
        {
            if (m_sendInputAxis)
            {
                CrossPlatformInputManager.SetAxis(m_verticalAxisName, 0);
                CrossPlatformInputManager.SetAxis(m_horizontalAxisName, 0);
            }
            m_onSwipeEnd.Invoke(Vector2.zero);
        }
    }

    void EasyTouch_On_SimpleTap(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject)
        {
            m_toggle = !m_toggle;
            if(m_toggle)
            m_onToggleOn.Invoke();
            else
            {
                m_onToggleOff.Invoke();
            }
        }
    }

    private void EasyTouch_On_Swipe(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject)
        {
            var swipe = gesture.swipeVector*m_swipeMuliplier;
            if (m_sendInputAxis)
            {
                //Debug.Log(swipe);
                CrossPlatformInputManager.SetAxis(m_verticalAxisName, swipe.y);
                CrossPlatformInputManager.SetAxis(m_horizontalAxisName, swipe.x);
            }
            m_onSwipe.Invoke(swipe);
        }
    }

    void EasyTouch_On_PinchEnd(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject)
        {
            if (m_sendInputAxis)
            {
                CrossPlatformInputManager.SetAxis(m_pinchAxisName, 0);
            }
        }
    }

    void EasyTouch_On_Pinch(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject)
        {
            var pinch = gesture.deltaPinch * m_pinchMultiplier;
            if (m_sendInputAxis)
            {
                //Debug.Log(swipe);
                CrossPlatformInputManager.SetAxis(m_pinchAxisName, pinch);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class DragEvent : UnityEvent<Vector2>
{
    
}
