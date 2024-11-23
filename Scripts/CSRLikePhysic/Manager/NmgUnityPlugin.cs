using System;
using UnityEngine;

public class NmgUnityPlugin
{
    public delegate void LogErrorCallback(string mesg);

    public static bool DEBUG = false;

    public static bool DEBUG_ASSERT = DEBUG;

    public static bool DEBUG_LOG_ENABLED = DEBUG;

    public static bool DEBUG_UNITY_JNI = false;

    public static IntPtr g_activityObj = IntPtr.Zero;

    public static IntPtr g_activityClassLoader = IntPtr.Zero;

    public static string g_nmgJNIPluginClassName = "org/naturalmotion/NmgJNIPlugin";

    public static string g_nmgPortalClassName = "org/naturalmotion/NmgPortal";

    public static string g_nmgMetricsClassName = "org/naturalmotion/NmgMetrics";

    public static string g_nmgServicesClassName = "org/naturalmotion/NmgServices";

    public static IntPtr g_nmgPortalClass = IntPtr.Zero;

    public static IntPtr g_nmgMetricsClass = IntPtr.Zero;

    public static IntPtr g_nmgServicesClass = IntPtr.Zero;

    public static IntPtr g_nmgPortal_Connect = IntPtr.Zero;

    public static IntPtr g_nmgPortal_SetHandledUpdatedResponse = IntPtr.Zero;

    public static IntPtr g_nmgPortal_SetPushNotificationData = IntPtr.Zero;

    public static IntPtr g_nmgPortal_SetGameCenterID = IntPtr.Zero;

    public static IntPtr g_nmgPortal_Update = IntPtr.Zero;

    public static IntPtr g_nmgPortal_GetCoreID = IntPtr.Zero;

    public static IntPtr g_nmgPortal_GetServiceURL = IntPtr.Zero;

    public static IntPtr g_nmgPortal_GetSwitchValue = IntPtr.Zero;

    public static IntPtr g_nmgPortal_GetServerTimestamp = IntPtr.Zero;

    public static IntPtr g_nmgPortal_Reconnect = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetSystemParams = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_Update = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_StartSession = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetConnectionParams = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_LogEvent = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_FlushEvents = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_GetCurrentNumberOfEventsInMemory = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_GetCurrentMemoryUsage = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_GetCurrentNumberOfFilesStored = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetAutoFlushEnabled = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetHardMemoryLimitEnabled = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetFlushEventsOnAppBackground = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetFlushEventsOnAppTerminate = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetAllowOverwriteStorageFiles = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetHTTPConnectionTimeout = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetMinimumNumberOfStorageFilesBeforeServerFlush = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_SetAppBackgroundTimeout = IntPtr.Zero;

    public static IntPtr g_nmgMetrics_ResetSession = IntPtr.Zero;

    public static IntPtr g_nmgServices_Initialise = IntPtr.Zero;

    public static IntPtr g_nmgServices_Deinitialise = IntPtr.Zero;

    public static IntPtr g_nmgServices_Update = IntPtr.Zero;

    public static IntPtr g_nmgServices_SetGameCenterID = IntPtr.Zero;

    public static IntPtr g_nmgServices_AddSocialNetworkData = IntPtr.Zero;

    public static IntPtr g_nmgServices_AddSocialNetworkDatas = IntPtr.Zero;

    public static IntPtr g_nmgServices_SetPushNotificationData = IntPtr.Zero;

    public static IntPtr g_nmgServices_GetCoreID = IntPtr.Zero;

    public static IntPtr g_nmgServices_GetDeviceID = IntPtr.Zero;

    public static IntPtr g_nmgServices_GetSwitchValue = IntPtr.Zero;

    public static IntPtr g_nmgServices_LogMetricsEvent = IntPtr.Zero;

    public static IntPtr g_nmgServices_FlushMetricsEvents = IntPtr.Zero;

    public static IntPtr g_nmgServices_SetFlushMetricsEventsOnAppBackground = IntPtr.Zero;

    public static IntPtr g_nmgServices_SetFlushMetricsEventsOnAppTerminate = IntPtr.Zero;

    public static LogErrorCallback g_nmgPortal_LogErrorCallback = null;

    public static LogErrorCallback g_nmgMetrics_LogErrorCallback = null;

    public static void ASSERT(bool EXPR)
    {
        if (DEBUG_ASSERT && !EXPR)
        {
            throw new Exception();
        }
    }

    public static void Initialise()
    {
        AndroidJNI.AttachCurrentThread();
        AndroidJNIHelper.debug = DEBUG_UNITY_JNI;
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        g_activityObj = @static.GetRawObject();
        g_activityObj = AndroidJNI.NewGlobalRef(g_activityObj);
        NmgJNI.CheckExceptions();
        ASSERT(g_activityObj != IntPtr.Zero);
        g_activityClassLoader = NmgJNI.GetObjectClassLoader(g_activityObj);
        g_activityClassLoader = AndroidJNI.NewGlobalRef(g_activityClassLoader);
        NmgJNI.CheckExceptions();
        g_nmgPortalClass = NmgJNI.FindClass(g_activityClassLoader, g_nmgPortalClassName);
        g_nmgPortalClass = AndroidJNI.NewGlobalRef(g_nmgPortalClass);
        g_nmgPortal_Connect = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "Connect", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)Z");
        g_nmgPortal_SetHandledUpdatedResponse = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "SetHandledUpdatedResponse", "(Z)V");
        g_nmgPortal_SetPushNotificationData = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "SetPushNotificationData", "([BZZZ)V");
        g_nmgPortal_SetGameCenterID = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "SetGameCenterID", "(Ljava/lang/String;)V");
        g_nmgPortal_Update = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "Update", "()I");
        g_nmgPortal_GetCoreID = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "GetCoreID", "()Ljava/lang/String;");
        g_nmgPortal_GetServiceURL = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "GetServiceURL", "(Ljava/lang/String;)Ljava/lang/String;");
        g_nmgPortal_GetSwitchValue = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "GetSwitchValue", "(Ljava/lang/String;)Ljava/lang/String;");
        g_nmgPortal_GetServerTimestamp = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "GetServerTimestamp", "()Ljava/lang/String;");
        g_nmgPortal_Reconnect = NmgJNI.GetStaticMethodID(g_nmgPortalClass, "Reconnect", "()Z");
        ASSERT(g_nmgPortal_Connect != IntPtr.Zero);
        ASSERT(g_nmgPortal_SetHandledUpdatedResponse != IntPtr.Zero);
        ASSERT(g_nmgPortal_SetPushNotificationData != IntPtr.Zero);
        ASSERT(g_nmgPortal_SetGameCenterID != IntPtr.Zero);
        ASSERT(g_nmgPortal_Update != IntPtr.Zero);
        ASSERT(g_nmgPortal_GetCoreID != IntPtr.Zero);
        ASSERT(g_nmgPortal_GetServiceURL != IntPtr.Zero);
        ASSERT(g_nmgPortal_GetSwitchValue != IntPtr.Zero);
        ASSERT(g_nmgPortal_GetServerTimestamp != IntPtr.Zero);
        ASSERT(g_nmgPortal_Reconnect != IntPtr.Zero);
        g_nmgMetricsClass = NmgJNI.FindClass(g_activityClassLoader, g_nmgMetricsClassName);
        g_nmgMetricsClass = AndroidJNI.NewGlobalRef(g_nmgMetricsClass);
        g_nmgMetrics_SetSystemParams = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetSystemParams", "(IIII)V");
        g_nmgMetrics_Update = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "Update", "()I");
        g_nmgMetrics_StartSession = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "StartSession", "(Ljava/lang/String;Ljava/lang/String;)Z");
        g_nmgMetrics_SetConnectionParams = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetConnectionParams", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
        g_nmgMetrics_LogEvent = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "LogEvent", "(Ljava/lang/String;Landroid/os/Bundle;)V");
        g_nmgMetrics_FlushEvents = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "FlushEvents", "(I)V");
        g_nmgMetrics_GetCurrentNumberOfEventsInMemory = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "GetNumberOfEventsInMemory", "()I");
        g_nmgMetrics_GetCurrentMemoryUsage = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "GetMemoryUsage", "()I");
        g_nmgMetrics_GetCurrentNumberOfFilesStored = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "GetNumberOfStorageFiles", "()I");
        g_nmgMetrics_SetAutoFlushEnabled = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetAutoFlushEnabled", "(Z)V");
        g_nmgMetrics_SetHardMemoryLimitEnabled = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetHardMemoryLimitEnabled", "(Z)V");
        g_nmgMetrics_SetFlushEventsOnAppBackground = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetFlushEventsOnAppBackground", "(Z)V");
        g_nmgMetrics_SetFlushEventsOnAppTerminate = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetFlushEventsOnAppTerminate", "(Z)V");
        g_nmgMetrics_SetAllowOverwriteStorageFiles = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetAllowOverwriteStorageFile", "(Z)V");
        g_nmgMetrics_SetHTTPConnectionTimeout = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetHTTPConnectionTimeout", "(I)V");
        g_nmgMetrics_SetMinimumNumberOfStorageFilesBeforeServerFlush = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetMinimumNumberOfStorageFilesBeforeServerFlush", "(II)V");
        g_nmgMetrics_SetAppBackgroundTimeout = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "SetAppBackgroundTimeout", "(I)V");
        g_nmgMetrics_ResetSession = NmgJNI.GetStaticMethodID(g_nmgMetricsClass, "ResetSession", "()V");
        ASSERT(g_nmgMetrics_SetSystemParams != IntPtr.Zero);
        ASSERT(g_nmgMetrics_Update != IntPtr.Zero);
        ASSERT(g_nmgMetrics_StartSession != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetConnectionParams != IntPtr.Zero);
        ASSERT(g_nmgMetrics_LogEvent != IntPtr.Zero);
        ASSERT(g_nmgMetrics_FlushEvents != IntPtr.Zero);
        ASSERT(g_nmgMetrics_GetCurrentNumberOfEventsInMemory != IntPtr.Zero);
        ASSERT(g_nmgMetrics_GetCurrentMemoryUsage != IntPtr.Zero);
        ASSERT(g_nmgMetrics_GetCurrentNumberOfFilesStored != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetAutoFlushEnabled != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetHardMemoryLimitEnabled != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetFlushEventsOnAppBackground != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetFlushEventsOnAppTerminate != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetAllowOverwriteStorageFiles != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetHTTPConnectionTimeout != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetMinimumNumberOfStorageFilesBeforeServerFlush != IntPtr.Zero);
        ASSERT(g_nmgMetrics_SetAppBackgroundTimeout != IntPtr.Zero);
        ASSERT(g_nmgMetrics_ResetSession != IntPtr.Zero);
        g_nmgServicesClass = NmgJNI.FindClass(g_activityClassLoader, g_nmgServicesClassName);
        g_nmgServicesClass = AndroidJNI.NewGlobalRef(g_nmgServicesClass);
        g_nmgServices_Initialise = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "Initialise", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)Z");
        g_nmgServices_Deinitialise = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "Deinitialise", "()Z");
        g_nmgServices_Update = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "Update", "()I");
        g_nmgServices_SetGameCenterID = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "SetGameCenterId", "(Ljava/lang/String;)V");
        g_nmgServices_AddSocialNetworkData = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "AddSocialNetworkData", "(Ljava/lang/String;Ljava/lang/String;)V");
        g_nmgServices_AddSocialNetworkDatas = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "AddSocialNetworkDatas", "(Ljava/lang/String;Ljava/lang/String;)V");
        g_nmgServices_SetPushNotificationData = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "SetPushNotificationData", "([BZZZ)V");
        g_nmgServices_GetCoreID = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "GetCoreID", "()Ljava/lang/String;");
        g_nmgServices_GetDeviceID = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "GetDeviceID", "()Ljava/lang/String;");
        g_nmgServices_GetSwitchValue = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "GetSwitchValue", "(Ljava/lang/String;)Ljava/lang/String;");
        g_nmgServices_LogMetricsEvent = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "LogMetricsEvent", "(Ljava/lang/String;Landroid/os/Bundle;)V");
        g_nmgServices_FlushMetricsEvents = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "FlushMetricsEvents", "(I)V");
        g_nmgServices_SetFlushMetricsEventsOnAppBackground = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "SetFlushMetricsEventsOnAppBackground", "(Z)V");
        g_nmgServices_SetFlushMetricsEventsOnAppTerminate = NmgJNI.GetStaticMethodID(g_nmgServicesClass, "SetFlushMetricsEventsOnAppTerminate", "(Z)V");
        ASSERT(g_nmgServices_Initialise != IntPtr.Zero);
        ASSERT(g_nmgServices_Deinitialise != IntPtr.Zero);
        ASSERT(g_nmgServices_Update != IntPtr.Zero);
        ASSERT(g_nmgServices_SetGameCenterID != IntPtr.Zero);
        ASSERT(g_nmgServices_AddSocialNetworkData != IntPtr.Zero);
        ASSERT(g_nmgServices_SetPushNotificationData != IntPtr.Zero);
        ASSERT(g_nmgServices_GetCoreID != IntPtr.Zero);
        ASSERT(g_nmgServices_GetDeviceID != IntPtr.Zero);
        ASSERT(g_nmgServices_GetSwitchValue != IntPtr.Zero);
        ASSERT(g_nmgServices_LogMetricsEvent != IntPtr.Zero);
        ASSERT(g_nmgServices_FlushMetricsEvents != IntPtr.Zero);
        ASSERT(g_nmgServices_SetFlushMetricsEventsOnAppBackground != IntPtr.Zero);
        ASSERT(g_nmgServices_SetFlushMetricsEventsOnAppTerminate != IntPtr.Zero);
    }
}
