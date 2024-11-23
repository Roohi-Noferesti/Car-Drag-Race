using System.Collections;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NativeBannerScreen : ZHUDScreen
{
    public RawImage m_image;
    public RuntimeTextButton m_clickButton;
    public RuntimeButton m_altButton;
    public TextMeshProUGUI m_descText;
    public TextMeshProUGUI m_titleText;
    public Animator SizeAnimator;
    public override ScreenID ID
    {
        get { return ScreenID.NativeBanner; }
    }

    public static NativeBannerBase NativeBannerObject;



    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);

            m_image.texture = NativeBannerObject.landscapeBannerImage;
   

        m_clickButton.AddValueChangedDelegate(OnClick);
        m_altButton.AddValueChangedDelegate(OnClick);
        m_descText.text = NativeBannerObject.getDescription();
        m_titleText.text = NativeBannerObject.getTitle();
        m_clickButton.SetText(NativeBannerObject.callToActionText,true,false);
        NativeBannerObject.onShown();
    //    GTAdManager.Instance.ClearNativeBannerCache();
    }

    //public override void OnAfterActivate()
    //{
    //    base.OnAfterActivate();
    //    if (true)//NativeBannerObject.portraitBannerImage != null)
    //    {
    //        SizeAnimator.Play("NativeBanner_Portrate");
    //    }
    //    else
    //    {
    //        SizeAnimator.Play("NativeBanner_LanScape");
    //    }
    //}


    public void OnClick()
    {
        GarageScreen.ResetNativeBannerTime();
        NativeBannerObject.onClicked();
        ScreenManager.Instance.PopScreen();
    }
}
