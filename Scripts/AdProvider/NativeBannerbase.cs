using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeBannerBase
{
    public string Title;
    public string Description;
    public Action OnShown;
    public Action OnClick;
    public Texture Icon;
    public Texture landscapeBannerImage;
    public string callToActionText;

    public string getDescription()
    {
        return Description;
    }

    public string getTitle()
    {
        return Title;
    }

    public void onShown()
    {
        if (OnShown != null)
        {
            OnShown();
        }
    }

    public void onClicked()
    {
        if (OnClick != null)
        {
            OnClick();
        }
    }
}
