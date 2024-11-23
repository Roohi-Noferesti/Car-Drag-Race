using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditScreen : ZHUDScreen
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Scrollbar scrollBar;

    private bool canScroll = true;

    public override ScreenID ID
    {
        get { return ScreenID.Credits; }
    }

    public void OnPointerDown()
    {
        canScroll = false;
    }

    public void OnPointerUp()
    {
        canScroll = true;
    }

    void Update()
    {
        if(canScroll)
        {
            scrollBar.value -= scrollSpeed * Time.deltaTime;
        }
        
        if(scrollBar.value < 0.1) {
            canScroll = false;
            OnHardwareBackButton();
        }
    }
}
