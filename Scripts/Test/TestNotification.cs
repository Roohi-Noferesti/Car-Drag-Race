using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNotification : MonoBehaviour 
{
#if UNITY_ANDROID

	// Use this for initialization
	void Start () {
		AndroidSpecific.Initialise();
	}

    public void Notify()
    {
        var returnValue =  AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationAddWithTag", new object[]
        {
            10,
            "mytag",
            "Test text for debug",
            "Some button",
            2
        });

        Debug.Log("return value is "+returnValue);
    }


    public void NotifyByObject()
    {
        DateTime dateTime = DateTime.Now.AddSeconds(20);
        Z2HAndroidNotificationManager notificationManager = new Z2HAndroidNotificationManager();
        notificationManager.UpdateFuelNotification(dateTime);
    }


    public void NotifyAdd()
    {
        Z2HAndroidNotificationManager notificationManager = new Z2HAndroidNotificationManager();
        notificationManager.AddWithTag(10, "MyTag", "this is add tag test", "add tag", 265);
    }
#endif
}
