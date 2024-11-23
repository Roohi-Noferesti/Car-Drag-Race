using System;
using UnityEngine;

public class NmgServices
{
	public static bool Initialise(string portalUrl, string preSharedKey, string productKey, string productDesc)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.NewStringUTF(portalUrl);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(preSharedKey);
		IntPtr intPtr3 = AndroidJNI.NewStringUTF(productKey);
		IntPtr intPtr4 = AndroidJNI.NewStringUTF(productDesc);
		jvalue[] array = new jvalue[4];
		array[0].l = intPtr;
		array[1].l = intPtr2;
		array[2].l = intPtr3;
		array[3].l = intPtr4;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_Initialise != IntPtr.Zero);
		bool result = AndroidJNI.CallStaticBooleanMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_Initialise, array);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
		AndroidJNI.DeleteLocalRef(intPtr3);
		AndroidJNI.DeleteLocalRef(intPtr4);
		NmgJNI.CheckExceptions();
		return result;
	}

	public static bool Deinitialise()
	{
		AndroidJNI.AttachCurrentThread();
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_Deinitialise != IntPtr.Zero);
		bool result = AndroidJNI.CallStaticBooleanMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_Deinitialise, NmgJNI.CreateNullJNIParams());
		NmgJNI.CheckExceptions();
		return result;
	}

	public static int Update()
	{
		AndroidJNI.AttachCurrentThread();
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_Update != IntPtr.Zero);
		int result = AndroidJNI.CallStaticIntMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_Update, NmgJNI.CreateNullJNIParams());
		NmgJNI.CheckExceptions();
		return result;
	}

	public static void SetGameCenterID(string gameCenterId)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.NewStringUTF(gameCenterId);
		jvalue[] array = new jvalue[1];
		array[0].l = intPtr;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_SetGameCenterID != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_SetGameCenterID, array);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr);
		NmgJNI.CheckExceptions();
	}

	public static void AddSocialNotificationData(string network, string userId)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.NewStringUTF(network);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(userId);
		jvalue[] array = new jvalue[2];
		array[0].l = intPtr;
		array[1].l = intPtr2;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_AddSocialNetworkData != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_AddSocialNetworkData, array);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
		NmgJNI.CheckExceptions();
	}

	public static void AddSocialNotificationDatas(string network, string userId)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.NewStringUTF(network);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(userId);
		jvalue[] array = new jvalue[2];
		array[0].l = intPtr;
		array[1].l = intPtr2;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_AddSocialNetworkData != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_AddSocialNetworkDatas, array);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
		NmgJNI.CheckExceptions();
	}

	public static void SetPushNotificationData(byte[] token, bool badges, bool alerts, bool sounds)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.ToByteArray(token);
		jvalue[] array = new jvalue[4];
		array[0].l = intPtr;
		array[1].z = badges;
		array[2].z = alerts;
		array[3].z = sounds;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_SetPushNotificationData != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_SetPushNotificationData, array);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr);
		NmgJNI.CheckExceptions();
	}

	public static string GetCoreID()
	{
		AndroidJNI.AttachCurrentThread();
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_GetCoreID != IntPtr.Zero);
		IntPtr intPtr = AndroidJNI.CallStaticObjectMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_GetCoreID, NmgJNI.CreateNullJNIParams());
		NmgJNI.CheckExceptions();
		string stringUTFChars = AndroidJNI.GetStringUTFChars(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr);
		NmgJNI.CheckExceptions();
		return stringUTFChars;
	}

	public static string GetDeviceID()
	{
		AndroidJNI.AttachCurrentThread();
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_GetDeviceID != IntPtr.Zero);
		IntPtr intPtr = AndroidJNI.CallStaticObjectMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_GetDeviceID, NmgJNI.CreateNullJNIParams());
		NmgJNI.CheckExceptions();
		string stringUTFChars = AndroidJNI.GetStringUTFChars(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr);
		NmgJNI.CheckExceptions();
		return stringUTFChars;
	}

	public static string GetSwitchValue(string switchId)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = AndroidJNI.NewStringUTF(switchId);
		jvalue[] array = new jvalue[1];
		array[0].l = intPtr;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_GetSwitchValue != IntPtr.Zero);
		IntPtr intPtr2 = AndroidJNI.CallStaticObjectMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_GetSwitchValue, array);
		string stringUTFChars = AndroidJNI.GetStringUTFChars(intPtr2);
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
		NmgJNI.CheckExceptions();
		return stringUTFChars;
	}

	public static void LogMetricsEvent(string eventName, string[] paramNames, string[] paramValues, int paramListSize)
	{
		AndroidJNI.AttachCurrentThread();
		IntPtr intPtr = NmgJNI.FindClass("android/os/Bundle");
		IntPtr methodID = NmgJNI.GetMethodID(intPtr, "<init>", "(Ljava/lang/ClassLoader;)V");
		jvalue[] array = new jvalue[1];
		array[0].l = NmgUnityPlugin.g_activityClassLoader;
		IntPtr intPtr2 = AndroidJNI.NewObject(intPtr, methodID, array);
		if (paramListSize != 0)
		{
			IntPtr methodID2 = NmgJNI.GetMethodID(intPtr, "putString", "(Ljava/lang/String;Ljava/lang/String;)V");
			for (int i = 0; i < paramListSize; i++)
			{
				IntPtr intPtr3 = AndroidJNI.NewStringUTF(paramNames[i]);
				IntPtr intPtr4 = AndroidJNI.NewStringUTF(paramValues[i]);
				jvalue[] array2 = new jvalue[2];
				array2[0].l = intPtr3;
				array2[1].l = intPtr4;
				AndroidJNI.CallVoidMethod(intPtr2, methodID2, array2);
				NmgJNI.CheckExceptions();
				AndroidJNI.DeleteLocalRef(intPtr3);
				AndroidJNI.DeleteLocalRef(intPtr4);
				NmgJNI.CheckExceptions();
			}
		}
		IntPtr intPtr5 = AndroidJNI.NewStringUTF(eventName);
		jvalue[] array3 = new jvalue[2];
		array3[0].l = intPtr5;
		array3[1].l = intPtr2;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_LogMetricsEvent != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_LogMetricsEvent, array3);
		NmgJNI.CheckExceptions();
		AndroidJNI.DeleteLocalRef(intPtr5);
		AndroidJNI.DeleteLocalRef(intPtr2);
		AndroidJNI.DeleteLocalRef(intPtr);
		NmgJNI.CheckExceptions();
	}

	public static void FlushMetricsEvents(int flushType)
	{
		AndroidJNI.AttachCurrentThread();
		jvalue[] array = new jvalue[1];
		array[0].i = flushType;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_FlushMetricsEvents != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_FlushMetricsEvents, array);
		NmgJNI.CheckExceptions();
	}

	public static void SetFlushMetricsEventsOnAppBackground(bool flushOnBackground)
	{
		AndroidJNI.AttachCurrentThread();
		jvalue[] array = new jvalue[1];
		array[0].z = flushOnBackground;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_SetFlushMetricsEventsOnAppBackground != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_SetFlushMetricsEventsOnAppBackground, array);
		NmgJNI.CheckExceptions();
	}

	public static void SetFlushMetricsEventsOnAppTerminate(bool flushOnTerminate)
	{
		AndroidJNI.AttachCurrentThread();
		jvalue[] array = new jvalue[1];
		array[0].z = flushOnTerminate;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgServices_SetFlushMetricsEventsOnAppTerminate != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgServicesClass, NmgUnityPlugin.g_nmgServices_SetFlushMetricsEventsOnAppTerminate, array);
		NmgJNI.CheckExceptions();
	}

	public static void RegisterLogErrorCallback(NmgUnityPlugin.LogErrorCallback callback)
	{
		NmgUnityPlugin.g_nmgPortal_LogErrorCallback = callback;
		NmgUnityPlugin.g_nmgMetrics_LogErrorCallback = callback;
	}

	public static void SetAppBackgroundTimeout(int timeout)
	{
		AndroidJNI.AttachCurrentThread();
		jvalue[] array = new jvalue[1];
		array[0].i = timeout;
		NmgUnityPlugin.ASSERT(NmgUnityPlugin.g_nmgMetrics_SetAppBackgroundTimeout != IntPtr.Zero);
		AndroidJNI.CallStaticVoidMethod(NmgUnityPlugin.g_nmgMetricsClass, NmgUnityPlugin.g_nmgMetrics_SetAppBackgroundTimeout, array);
		NmgJNI.CheckExceptions();
	}
}
