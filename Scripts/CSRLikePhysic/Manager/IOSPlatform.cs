using System.Collections;
using System.Collections.Generic;
//using KingKodeStudio.IAB;
using UnityEngine;


#if UNITY_IOS
public class IOSPlatform : BasePlatform 
{
    public override void Initialise()
    {
        base.Initialise();
        IOSSpecific.Initialize();
    }

    public override GTAppStore GetTargetAppStore()
    {
        return InsideCountry ? GTAppStore.Zarinpal : GTAppStore.iOS;
    }

    public override bool ShareImage(string contents, string url, string image)
    {
        
        // Create share sheet
        ShareSheet _shareSheet     = new ShareSheet();    
        _shareSheet.Text        = contents;
        _shareSheet.AttachScreenShot();

        // Show composer
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
#if USES_SHARING
        NPBinding.Sharing.ShowView(_shareSheet, null);
#endif
        return true;
    }

    public override bool ShareText(string caption, string contents, string url, string image)
    {
        // Create share sheet
        ShareSheet _shareSheet     = new ShareSheet();    
        _shareSheet.Text        = contents;

        // Set this list if you want to exclude any service/application type. Else, ignore.
//        _shareSheet.ExcludedShareOptions    = m_excludedOptions;

        // Show composer
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
#if USES_SHARING
        NPBinding.Sharing.ShowView(_shareSheet, null);
#endif
        return true;
    }


    public override string GetTimeZone()
    {
        return IOSSpecific.GetTimeZone();
    }

    public override string GetCity()
    {
        return IOSSpecific.GetCity();
    }

    public override void ShowFreshChat(string userid, string name, string restoreID)
    {
            IOSSpecific.ShowFreshChatConversation(userid, restoreID);
    }

    public override bool IsSupportedGooglePlayStore()
    {
        return false;
    }

    public override void InitializeFreshChat()
    {
            IOSSpecific.InitializeFreshChat();
    }
#endif
