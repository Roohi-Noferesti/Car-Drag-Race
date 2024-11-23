using System;
using I2.Loc;
//using KingKodeStudio.IAB;
using Metrics;
using UnityEngine;

public class RateTheAppNagger
{
	private const string DLL_ID = "__Internal";

	public static DateTime LastTryShowPrompt;

	private static void _nativeSetAppID(int appID)
	{
	}

	private static void _nativePromptIfShould()
	{
		if (!BuildType.CanShowRate())
			return;
        //AndroidSpecific.NativePromptIfShould(title, body, rate, decline);


        //        var popup =  new PopUp
        //        {
        //            Title = "TEXT_RATE_THE_APP_TITLE",
        //            BodyText = "TEXT_RATE_THE_APP",
        //            IsBig = true,
        //            ConfirmAction = () =>
        //            {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //                _nativeTriggerRateAppPage();
        //                //CafeIntent.Like();
        //                Log.AnEvent(Events.ConfirmedRateApp);
        //#endif
        //            },
        //            CancelAction = () =>
        //            {
        //                Log.AnEvent(Events.DismissedRateApp);
        //            },
        //            CancelText = "TEXT_RATE_THE_APP_DECLINE",
        //            ConfirmText = "TEXT_RATE_THE_APP_RATE",
        //            ImageCaption = "TEXT_NAME_AGENT",
        //            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        //        };
        //	    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);

        //KingKodeStudio.ScreenManager.Instance.PushScreen(ScreenID.UserRatingGame);
        PopUp popup = new PopUp
        {
	        Title = "TEXT_RATE_THE_APP_RATE",
	        BodyText = "TEXT_POPUPS_ARE_YOU_SURE",
	        ConfirmText = "TEXT_USERRATE_SUBMIT",
	        CancelText = "TEXT_BUTTON_CANCEL",
	        IsRatePopup = true,
	        CancelAction = () =>
	        {
		        PopUpManager.Instance.KillPopUp();
	        },
	        ID = PopUpID.UserRate,
	        
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        
	}


    private static string GetStorePackage()
    {
#if UNITY_ANDROID
        //if (KingIAB.Setting.IsGooglePlay)
        //{
        //    return null;
        //}
        //else if (KingIAB.Setting.IsBazaar)
        //{
        //    return "com.farsitel.bazaar";
        //}
        //else if (KingIAB.Setting.IsIraqApps)
        //{
        //    return "ir.tgbs.android.iranapp";
        //}
        //else if (KingIAB.Setting.IsMyket)
        //{
        //    return null;
        //}
#endif

        return null;
    }



    private static string GetStoreLikeURI()
    {
        string packageName = "";
#if UNITY_EDITOR
        packageName = "";
        
#elif UNITY_ANDROID
        packageName = Application.identifier;
        var config = PurchasingModuleSelection.Config;

        if (config.UseUnityIAPSetting)
        {
            return "market://details?id="+ packageName;
        }
        else if (config.IsBazaar)
        {
            return "bazaar://details?id="+ packageName;
        }
        else if (config.IsMyket)
        {
            return "myket://comment?id="+packageName;
        }
#elif UNITY_IPHONE
	    packageName = "";
#endif

        return null;
    }

    private static int _nativePromptResult()
	{
        //return AndroidSpecific.NativePromptResult();
	    return 0;
	}

	private static void _nativeTriggerRateAppPage()
	{
	    var uri = GetStoreLikeURI();
	    var package = GetStorePackage();
	    int actionType = 0;
        if (PurchasingModuleSelection.Config.IsBazaar)
        {
            actionType = 1;
        }
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidSpecific.NativeTriggerRateAppPage(uri,package, actionType);
#elif UNITY_IPHONE
		IOSSpecific.RequestReview();
#endif
    }


    public static void initNagger()
	{
        LastTryShowPrompt = GTDateTime.Now.AddMinutes(-177.20001220703125);
	}

	public static void TryShowPrompt(RateTheAppTrigger trigger)
	{
		if (ShouldShowPrompt(trigger))
		{
			DoShowPrompt();
		}
	}

	private static bool ShouldShowPrompt(RateTheAppTrigger trigger)
	{
        //if (LastTryShowPrompt.AddHours(3.0) > GTDateTime.Now)
        //{
        //    return false;
        //}
        switch (trigger)
        {
            case RateTheAppTrigger.BUYACAR:
                if (PlayerProfileManager.Instance.ActiveProfile.DoneRateAppTriggerBuyCar)
                {
                    return false;
                }
                PlayerProfileManager.Instance.ActiveProfile.DoneRateAppTriggerBuyCar = true;
                break;
            case RateTheAppTrigger.BEATCREWMEMBER:
                if (PlayerProfileManager.Instance.ActiveProfile.DoneRateAppTriggerCrewMember)
                {
                    return false;
                }
                PlayerProfileManager.Instance.ActiveProfile.DoneRateAppTriggerCrewMember = true;
                break;
        }
        return true;
	}

	private static void DoShowPrompt()
	{
        //string body = LocalizationManager.GetTranslation("TEXT_RATE_THE_APP");
        //string title = LocalizationManager.GetTranslation("TEXT_RATE_THE_APP_TITLE");
        //string rate = LocalizationManager.GetTranslation("TEXT_RATE_THE_APP_RATE");
        //string decline = LocalizationManager.GetTranslation("TEXT_RATE_THE_APP_DECLINE");
		_nativePromptIfShould();
        //GameObject gameObject = new GameObject("NaggerWaiter");
        //gameObject.AddComponent<RateTheAppWaiter>();
        LastTryShowPrompt = GTDateTime.Now;
	}

	public static void TriggerRateAppPage()
	{
		_nativeTriggerRateAppPage();
	}

	public static int GetNativePromptResult()
	{
		return _nativePromptResult();
	}
}
