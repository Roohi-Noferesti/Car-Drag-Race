using System;
using UnityEngine;
using UnityEngine.Android;

#if UNITY_ANDROID
public class AndroidSpecific
{
    private static AndroidJavaClass EnvironmentClass = new AndroidJavaClass("android.os.Environment");
    private const string Environment_MediaMounted = "mounted";
    public static AndroidJavaObject mActivityJavaObject;

    private static AndroidJavaClass freshChatClass;

    public static bool Initialized { get; private set; }


    public static void Quit()
    {
        AndroidDevice.mIsQuitting = true;
    }

    // public static bool GetFullscreenMode()
    // {
    //     return mActivityJavaObject.Call<bool>("getFullscreenMode", new object[0]);
    // }

    public static bool GetFullscreenMode()
    {
        return mActivityJavaObject.Call<bool>("getFullscreenMode", new object[0]);
    }



    public static string GetTimeZone()
    {
        if(!Initialized)
            Initialise();
        AndroidJNI.PushLocalFrame(100);
        var result = mActivityJavaObject.Call<string>("getTimeZone");
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }

    public static string GetCountryCode()
    {
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kingkodestudio.z2h.LocationUtility"))
        {
            return androidJavaClass.CallStatic<string>("getDeviceCountryCode");
        }
    }
    
    public static string GetCity()
    {
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kingkodestudio.z2h.TimeZonePlugin"))
        {
            string city = androidJavaClass.CallStatic<string>("getTimeZone");
            try {
                city = city.Split('/')[1];
            } catch(Exception e) {
                city = "London";
            }
            return city.ToLower();
        }
    }

    public static void SetGPGSignInCount(int count)
    {
 #if UNITY_EDITOR
        PlayerPrefs.SetInt("getGPGSignInCount", count);
 #else
 		mActivityJavaObject.Call("setGPGSignInCount", new object[]
 		{
 			count
 		});
 #endif
    }
    
    public static string GetObbFullPath(bool isMain)
    {
        var versionCode = mActivityJavaObject.CallStatic<int>("getApplicationVersionCode");
        var mainOrPatch = isMain ? "main" : "patch";
        return $"{GetObbRoot()}/{mainOrPatch}.{versionCode}.{ObbDownloader.ApplicationIdentifier}.obb";
        
        //return mActivityJavaObject.Call<string>("getObbFullName", isMain);
    }
    
    public static string GetObbRoot()
    {
        string expansionFilePath=null;
        if (EnvironmentClass.CallStatic<string>("getExternalStorageState") != Environment_MediaMounted)
        {
            return null;
        }

        const string _obbPath = "Android/obb";
        using (var externalStorageDirectory =
            EnvironmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            var externalRoot = externalStorageDirectory.Call<string>("getPath");
            expansionFilePath = $"{externalRoot}/{_obbPath}/{ObbDownloader.ApplicationIdentifier}";
        }

        return expansionFilePath;
        //return mActivityJavaObject.Call<string>("getObbRoot");
    }

    
    public static int GetGPGSignInCount()
    {
#if UNITY_EDITOR
        return 1;//PlayerPrefs.GetInt("getGPGSignInCount");
#else
        //return PlayerPrefs.GetInt("getGPGSignInCount");

         return mActivityJavaObject.Call<int>("getGPGSignInCount", new object[0]);
#endif
    }

    public static void Initialise()
    {
        if (Initialized)
            return;
        //NmgUnityPlugin.Initialise();
        // callbackScheduler = AndroidCallbackScheduler.Create();
        //var androidJavaClass = new AndroidJavaClass("com.kingkodestudio.z2hUtilities.MainUtilities");
        //mActivityJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        mActivityJavaObject = new AndroidJavaObject("com.kingkodestudio.z2hUtilities.MainUtilities");
        mActivityJavaObject.Call("initialise");
        ObbMount();

        InitializeFreshChat();
        Initialized = true;

    }

    public static void InitializeFreshChat()
    {
        freshChatClass = new AndroidJavaClass("com.kingkodestudio.z2h.FreshChatHelper");
        freshChatClass.CallStatic("initialize");
    }


    public static string GetObbFilePathInPatchOrMain(string inPath)
    {
        
        return Application.streamingAssetsPath + "/AppDataRoot/" + inPath;
    }

    public static bool ObbMount()
    {
        return mActivityJavaObject.Call<bool>("obbMount", new object[0]);
        
    }
    /*

    public static string GetObbRootPath()
    {
        var javaClass = new AndroidJavaClass("com.kingkodestudio.z2h.ObbUtility");
        return javaClass.CallStatic<string>("getObbRootPath");
    }
    */

    public static string GetLastUsedGPid()
    {
#if UNITY_EDITOR
        return PlayerPrefs.GetString("getLastUsedGPid");
#else
		return mActivityJavaObject.Call<string>("getLastUsedGPid", new object[0]);
#endif
    }

    public static void SetLastUsedGPid(string id)
    {
#if UNITY_EDITOR
        PlayerPrefs.SetString("getLastUsedGPid", id);
#else
		mActivityJavaObject.Call("setLastUsedGPid", new object[]
		{
			id
		});
#endif
    }

    public static void ShowDialog(string title, string message)
    {
        AndroidJNI.PushLocalFrame(100);
        mActivityJavaObject.Call("showDialog", title, message);
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
    }


    public static void ShowPermissionDialog(string permission, string title, string message)
    {
        AndroidJNI.PushLocalFrame(100);
        mActivityJavaObject.Call("showPermissionDialog", permission, title, message);
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
    }

    public static void ShowFreshChatFAQ(string userid, string name,string restoreID)
    {
        freshChatClass.CallStatic("showFAQ", userid, name, restoreID);
    }
  
    public static void ShowFreshChatConversation(string userid, string name,string restoreID)
    {
        freshChatClass.CallStatic("showConversation", userid, name, restoreID);
    }

    
    public static void NativeTriggerRateAppPage(string uri, string package, int actionType)
    {
        mActivityJavaObject.Call("nativeTriggerRateAppPage", new object[] { uri, package, actionType });
    }
    
    
    public static bool OBBFileExists(string filename, bool patch)
    {
        AndroidJNI.PushLocalFrame(5);
        bool result = mActivityJavaObject.Call<bool>("OBBFileExists", new object[]
        {
            filename,
            patch
        });
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }
    
    public static byte[] LoadOBBFile(string filename, bool patch)
    {
        AndroidJNI.PushLocalFrame(5);
        byte[] result = mActivityJavaObject.Call<byte[]>("LoadOBBFile", new object[]
        {
            filename,
            patch
        });
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }
    
    private static AndroidJavaClass m_fileUtilityClass;

    private static void EnsureFileUtiltiyExists()
    {
        if (m_fileUtilityClass == null)
        {
            m_fileUtilityClass = new AndroidJavaClass("com.kingkodestudio.z2h.FileUtility");
        }
    }
    
    public static bool APKFileExists(string rootPath, string filename)
    {
        AndroidJNI.PushLocalFrame(5);
        EnsureFileUtiltiyExists();

        bool result = m_fileUtilityClass.CallStatic<bool>("fileExists", new object[]
        {
            rootPath,
            filename
        });
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }

    public static bool APKFileExists(string filename)
    {
        AndroidJNI.PushLocalFrame(5);
        EnsureFileUtiltiyExists();

        bool result = m_fileUtilityClass.CallStatic<bool>("fileExists", new object[]
        {
            filename
        });
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }
    
    public static byte[] LoadAPKFile(string filename)
    {
        GTDebug.Log(GTLogChannel.Android, "Loading From Obb: " + filename);
        AndroidJNI.PushLocalFrame(5);
        EnsureFileUtiltiyExists();

        var result = m_fileUtilityClass.CallStatic<byte[]>("openFile", new object[]
        {
            filename
        });
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
        return result;
    }
}
#endif
