using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR || UNITY_IOS
public static class IOSSpecific
{
    private static bool _initialized;

    public static void Initialize()
    {
    }

    public static void InitializeFreshChat()
    {
        _initilizeFreshchat();
    }

    public static string GetTimeZone()
    {
        return _GetTimeZone();
    }
    
    public static string GetCity()
    {
        string city = _GetTimeZone();
        try {
            city = city.Split('/')[1];
        } catch(Exception e) {
            city = "London";
        }
        return city.ToLower();
    }
    
    public static void RequestReview()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            _RequestReview();
    }
    
    public static void ShowFreshChatConversation(string userID,string restoreID)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            _showConversation(userID,restoreID);
    }
    
    
    [DllImport("__Internal")]
    private static extern void _initilizeFreshchat();

    [DllImport("__Internal")]
    private static extern string _GetTimeZone();
    
    [DllImport("__Internal")]
    private static extern void _RequestReview();
    
    [DllImport("__Internal")]
    private static extern void _showConversation(string userID,string restoreID);
}
#endif
