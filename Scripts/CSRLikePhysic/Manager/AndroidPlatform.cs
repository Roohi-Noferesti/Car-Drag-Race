
//using KingKodeStudio.IAB;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UDP;

#if UNITY_ANDROID
public class AndroidPlatform : BasePlatform
{
    //private static AndroidJavaObject mCompress;

    private bool mIsEmailSent;

    private string m_CountryCode;
    private string m_City;

    //public static AndroidJavaObject mFacebook;
    
    public static AndroidJavaObject mShare;

   

    public override void Initialise()
    {
        base.Initialise();
        AndroidSpecific.Initialise();
    }

    

    public override int GetOSUpTime()
    {
        return AndroidSpecific.mActivityJavaObject.Call<int>("getOSUpTime", new object[0]);
    }

    
    public override string GetApplicationName()
    {
        return "com.kingkodestudio.z2h";
    }

    public override void Alert(string message)
    {
        AndroidSpecific.mActivityJavaObject.Call("alert", new object[]
        {
            message
        });
    }

    public override string GetDeviceOS()
    {
        return "Android";
    }

    public override string GetDeviceType()
    {
        return "Android";
    }

    public override GTAppStore GetTargetAppStore()
    {
        var config = PurchasingModuleSelection.Config;
        if (InsideCountry && config.IsAndroidZarinpal)
        {
            return GTAppStore.Zarinpal;
        }

        if (!InsideCountry && config.UseUnityIAPSetting)
        {
            var standardPurchasingModule = PurchasingModuleSelection.PurchasingModue as StandardPurchasingModule;
            if (standardPurchasingModule == null)
            {
                return GTAppStore.GooglePlay;
            }
            if (standardPurchasingModule.appStore == UnityEngine.Purchasing.AppStore.GooglePlay)
            {
                return GTAppStore.GooglePlay;
            }
            return GTAppStore.UDP;
        }

        if (config.IsBazaar) return GTAppStore.Bazaar;
        if (config.IsMyket) return GTAppStore.Myket;

        return GTAppStore.None;
    }

    public override string GetTargetAppStoreFriendlyName()
    {
        return "Android";
    }

    public override string GetDeviceOSVersion()
    {
        if (!AndroidSpecific.Initialized)
        {
            Initialise();
        }
        return AndroidSpecific.mActivityJavaObject.Call<string>("getDeviceOSVersion", new object[0]);
    }


    public override string GetDeviceModel()
    {
        return AndroidSpecific.mActivityJavaObject.Call<string>("getBuildModel", new object[0]);
    }



    public override void PrepareRenderTexture(RenderTexture rt)
    {
        if (SystemInfo.graphicsDeviceName.Contains("PowerVR"))
        {
            if (rt.IsCreated())
            {
                rt.Release();
            }
            rt.Create();
        }
    }

    public override bool CanSendEmail()
    {
        return AndroidSpecific.mActivityJavaObject.Call<bool>("CanSendEmail", new object[0]);
    }

    public override bool SendEmail(string contents, string subject)
    {
        this.mIsEmailSent = AndroidSpecific.mActivityJavaObject.Call<bool>("SendEmail", new object[]
        {
            contents,
            subject
        });
        return true;
    }

    public override bool GetSendEmailSuccessful()
    {
        bool result = this.mIsEmailSent;
        this.mIsEmailSent = false;
        return result;
    }
    
    
    public override bool IsSupportedGooglePlayStore()
    {
        if (!BuildType.CanShowGooglePlay())
            return false;
        
        var targetAppStore = GetTargetAppStore();
        switch (targetAppStore)
        {
            case GTAppStore.GooglePlay:
            case GTAppStore.Zarinpal:
            case GTAppStore.Bazaar:
            case GTAppStore.Myket:
            case GTAppStore.Iraqapps:
                return true;
            default:
                return false;
        }
    }


    public static AndroidJavaObject GetShareObject()
    {
        if (AndroidPlatform.mShare == null)
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kingkodestudio.z2hUtilities.GTSharing"))
            {
                AndroidPlatform.mShare = androidJavaClass.CallStatic<AndroidJavaObject>("Instance", new object[0]);
            }
        }
        return AndroidPlatform.mShare;
    }

    public override bool UsesNativeSharing()
    {
        return true;
    }

    public override bool ShareImage(string contents, string url, string image, string message = null)
    {
        var intentJavaClass = new AndroidJavaClass("android.content.Intent");
        var intentJavaObject = new AndroidJavaObject("android.content.Intent", new object[0]);
        intentJavaObject.Call<AndroidJavaObject>("setAction", new object[]
        {
            intentJavaClass.GetStatic<string>("ACTION_SEND")
        });
        var uriJavaClass = new AndroidJavaClass("android.net.Uri");
        var uriParseResult = uriJavaClass.CallStatic<AndroidJavaObject>("parse", new object[]
        {
            "content://" + image
        });
        intentJavaObject.Call<AndroidJavaObject>("putExtra", new object[]
        {
            intentJavaClass.GetStatic<string>("EXTRA_STREAM"),
            uriParseResult
        });
        intentJavaObject.Call<AndroidJavaObject>("setType", new object[]
        {
            "image/png"
        });
        intentJavaObject.Call<AndroidJavaObject>("putExtra", new object[]
        {
            intentJavaClass.GetStatic<string>("EXTRA_TEXT"),
            message
        });
        var unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = unityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        var chooserActivityJavaObject = intentJavaClass.CallStatic<AndroidJavaObject>("createChooser", new object[]
        {
            intentJavaObject,
            "Share"
        });
        currentActivity.Call("startActivity", new object[]
        {
            chooserActivityJavaObject
        });
        return AndroidPlatform.GetShareObject().Call<bool>("ShareImage", new object[]
        {
            contents,
            image
        });
    }

    public override bool ShareText(string caption, string contents, string url, string image)
    {
        return AndroidPlatform.GetShareObject().Call<bool>("ShareText", new object[]
        {
            caption,
            contents,
            url,
            image
        });

    }

    public override bool CanSendTweet()
    {
        return true;
    }

    public override bool CopyImageToCameraRoll(string image)
    {
        //bool flag = true;AndroidSpecific.mActivityJavaObject.Call<bool>("CopyImageToCameraRoll", new object[]
        //{
        //    image
        //});
        if (true)//flag)
        {
            NativeEvents.ApplicationDidFinishCopyingSnapshotToCameraRoll();
        }
        else
        {
            NativeEvents.ApplicationDidFailCopyingSnapshotToCameraRoll();
        }
        return true;
    }

    public override string GetDefaultBranchName()
    {
        if (this.GetTargetAppStore() == GTAppStore.Amazon)
        {
            return "Default_Amazon";
        }
        return "Default_Android";
    }


    public override string GetTimeZone()
    {
        return AndroidSpecific.GetTimeZone();
    }

    public override string GetCountryCode()
    {
        if (string.IsNullOrEmpty(m_CountryCode))
        {
            m_CountryCode =  AndroidSpecific.GetCountryCode();
            GTDebug.Log(GTLogChannel.Android, "Country Code is : " + m_CountryCode);
        }

        return m_CountryCode;
    }
    
    public override string GetCity()
    {
        if (string.IsNullOrEmpty(m_City))
        {
            m_City =  AndroidSpecific.GetCity();
            GTDebug.Log(GTLogChannel.Android, "City is : " + m_City);
        }

        return m_City;
    }

    public override void InitializeFreshChat()
    {
        AndroidSpecific.InitializeFreshChat();
    }


    public override void ShowFreshChat(string userid, string name, string restoreID)
    {
        AndroidSpecific.ShowFreshChatConversation(userid, name, restoreID);
    }
    
    public override void ShowFreshChatFAQ(string userid, string name, string restoreID)
    {
        AndroidSpecific.ShowFreshChatFAQ(userid, name, restoreID);
    }
}
#endif
