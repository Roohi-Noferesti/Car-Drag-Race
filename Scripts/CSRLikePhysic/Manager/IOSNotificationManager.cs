using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;



#if UNITY_IPHONE
public class IOSNotificationManager : NotificationManager 
{
    private List<Notification> m_notifications;
    public IOSNotificationManager()
    {
        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        //OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.VERBOSE);
        /*try
        {
            OneSignal.StartInit("1c509159-18ff-4f99-a5c2-804393f7ba1d")
                .EndInit();

            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
        }
        catch (Exception e)
        {

        }
        */
        NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        NotificationService.DidReceiveLocalNotificationEvent += DidReceiveLocalNotificationEvent;
        LoadNotifications();
    }

    private void LoadNotifications()
    {
        var file = Application.persistentDataPath + "/ntfy.ny";
        if (File.Exists(file))
        {
            try
            {
                var txt = File.ReadAllText(file);
                m_notifications = JsonConverter.DeserializeObject<List<Notification>>(txt);
            }
            catch (Exception e)
            {
                m_notifications = new List<Notification>();
            }

        }
        else
        {
            m_notifications = new List<Notification>();
        }
    }

    private void SaveNotifications()
    {
        var file = Application.persistentDataPath + "/ntfy.ny";
        if (m_notifications == null)
        {
            m_notifications = new List<Notification>();
        }
        var txt = JsonConverter.SerializeObject(m_notifications);
        File.WriteAllText(file, txt);
    }

    private void DidReceiveLocalNotificationEvent(CrossPlatformNotification _notification)
    {

    }

    protected override int findMatchUpdate(int secondsAfterNow, string bodyText, string buttonText, int groupId)
    {
        GTDebug.Log(GTLogChannel.Notification,"here notify after now " + secondsAfterNow + "  " + bodyText + "   " + groupId);
  //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationFindMatchUpdate", new object[]
		//{
		//	secondsAfterNow,
		//	bodyText,
		//	buttonText,
		//	groupId
		//});

        foreach (var notification in m_notifications.Where(n => n.GroupID == groupId))
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.GroupID == groupId);

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText,string.Empty,groupId);
        SaveNotifications();
        return ret;
    }



    protected override int findMatchUpdateTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationFindMatchUpdateTag", new object[]
        //{
        //	secondsAfterNow,
        //	tag,
        //	bodyText,
        //	buttonText,
        //	groupId
        //});

        foreach (var notification in m_notifications.Where(n => n.Tag == tag))
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.Tag == tag);

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText,tag, groupId);
        SaveNotifications();
        return ret;
    }

    protected override int addWithTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId)
    {
  //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationAddWithTag", new object[]
		//{
		//	secondsAfterNow,
		//	tag,
		//	bodyText,
		//	buttonText,
		//	groupId
		//});

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText, tag, groupId);
        SaveNotifications();
        return ret;
    }

    protected override int add(int secondsAfterNow, string bodyText, string buttonText, int groupId)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationAdd", new object[]
        //{
        //	secondsAfterNow,
        //	bodyText,
        //	buttonText,
        //	groupId
        //});

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText, string.Empty, groupId);
        SaveNotifications();
        return ret;
    }

    public override int FindMatchRemove(string bodyText)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationFindMatchRemove", new object[]
        //{
        //	bodyText
        //});

        foreach (var notification in m_notifications.Where(n => n.BodyText == bodyText))
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.BodyText == bodyText);

        SaveNotifications();
        return 0;
    }

    public override int ClearNotificationsWithTag(string tag)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationClearNotificationsWithTag", new object[]
        //{
        //	tag
        //});

        foreach (var notification in m_notifications.Where(n => n.Tag == tag))
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.Tag == tag);

        SaveNotifications();

        return 0;
    }

    public override int Cancel(int which)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationCancel", new object[]
        //{
        //	which
        //});

        foreach (var notification in m_notifications.Where(n => n.IntID == which))
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.IntID == which);

        SaveNotifications();

        return 0;
    }

    public override int ClearAllNotifications()
    {
        //return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationClearAllNotifications", new object[0]);

        m_notifications.Clear();
        NPBinding.NotificationService.CancelAllLocalNotification();

        SaveNotifications();

        return 0;
    }

    public override int FindMatchReplace(string original, string replaceText)
    {
        //      return AndroidSpecific.mActivityJavaObject.Call<int>("_nativeNotificationFindMatchReplace", new object[]
        //{
        //	original,
        //	replaceText
        //});

        var notification = m_notifications.FirstOrDefault(n => n.BodyText == original);
        if (notification != null)
        {
            NPBinding.NotificationService.CancelLocalNotification(notification.ID);
            notification.BodyText = replaceText;
            notification.ID = CreateNotification(notification.Seconds, replaceText, notification.ButtonText).GetNotificationID();
            SaveNotifications();
            return notification.IntID;
        }

        return -1;
    }

    public override string GetNotificationServicesDeviceToken()
    {
        try
        {
	        return string.Empty;//OneSignal.GetPermissionSubscriptionState().subscriptionStatus.userId;
            //return "fake_one_signal_device_id";
        }
        catch (Exception e)
        {
            return null;
        }
        //return AndroidSpecific.mActivityJavaObject.Call<string>("_nativeNotificationGetDeviceToken", new object[0]);
    }

    public override string GetScheduledNotificationDebugLog()
    {
        //return AndroidSpecific.mActivityJavaObject.Call<string>("_nativeNotificationGetScheduledNotificationDebugLog", new object[0]);
        return null;
    }


    private int CreateAndAddNotification(int secondsAfterNow, string bodyText, string buttonText, string tag, int groupId)
    {
        var cpNotification = CreateNotification(secondsAfterNow, bodyText, buttonText);
        var newNotification = new Notification();
        newNotification.ID = cpNotification.GetNotificationID();
        newNotification.GroupID = groupId;
        newNotification.Tag = tag;
        newNotification.BodyText = bodyText;
        newNotification.IntID = newNotification.ID.GetHashCode();
        newNotification.Seconds = secondsAfterNow;
        m_notifications.Add(newNotification);
        return newNotification.IntID;
    }

    private CrossPlatformNotification CreateNotification(long fireAfterSec,string body, string contentTitle)
    {
        // User info - Is used to set custom data. Create a dictionary and set your data if any.
        //IDictionary _userInfo = new Dictionary<string, string>();
        //_userInfo["data"] = "add what is required";

        // Set iOS specific properties
        CrossPlatformNotification.iOSSpecificProperties _iosProperties = new CrossPlatformNotification.iOSSpecificProperties();
        _iosProperties.HasAction = false;
        //_iosProperties.AlertAction = "Play";

        // Set Android specific properties
//        CrossPlatformNotification.AndroidSpecificProperties _androidProperties = new CrossPlatformNotification.AndroidSpecificProperties();
//        _androidProperties.ContentTitle = contentTitle;
//        //_androidProperties.TickerText = "ticker ticks over here";
//        _androidProperties.LargeIcon = "icon.png"; //Keep the files in Assets/PluginResources/VoxelBusters/NativePlugins/Android folder.

        // Create CrossPlatformNotification instance
        CrossPlatformNotification _notification = new CrossPlatformNotification();
        _notification.AlertBody = body; //On Android, this is considered as ContentText
        _notification.FireDate = System.DateTime.Now.AddSeconds(fireAfterSec);
        _notification.RepeatInterval = eNotificationRepeatInterval.NONE;
        //_notification.UserInfo = _userInfo;
        //_notification.SoundName = "Notification.mp3"; //Keep the files in Assets/PluginResources/NativePlugins/Android or iOS or Common folder.

        _notification.iOSProperties = _iosProperties;

        NPBinding.NotificationService.ScheduleLocalNotification(_notification);

        return _notification;
    }
}
#endif
