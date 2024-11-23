using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

#if UNITY_ANDROID
public class Z2HAndroidNotificationManager : NotificationManager
{
    private List<Notification> m_notifications;
    public Z2HAndroidNotificationManager()
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

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);


        //NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        //NotificationService.DidReceiveLocalNotificationEvent += DidReceiveLocalNotificationEvent;
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

    /*
    private void DidReceiveLocalNotificationEvent(CrossPlatformNotification _notification)
    {

    }
    */

    protected override int findMatchUpdate(int secondsAfterNow, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        GTDebug.Log(GTLogChannel.Notification,"here notify after now " + secondsAfterNow + "  " + bodyText + "   " + groupId);

        foreach (var notification in m_notifications.Where(n => n.GroupID == groupId))
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.GroupID == groupId);

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText,string.Empty,groupId, largeIcon);
        SaveNotifications();
        return ret;
    }



    protected override int findMatchUpdateTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        foreach (var notification in m_notifications.Where(n => n.Tag == tag))
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.Tag == tag);

        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText,tag, groupId);
        SaveNotifications();
        return ret;
    }

    protected override int addWithTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText, tag, groupId);
        SaveNotifications();
        return ret;
    }

    protected override int add(int secondsAfterNow, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        var ret = CreateAndAddNotification(secondsAfterNow, bodyText, buttonText, string.Empty, groupId);
        SaveNotifications();
        return ret;
    }

    public override int FindMatchRemove(string bodyText)
    {
        foreach (var notification in m_notifications.Where(n => n.BodyText == bodyText))
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.BodyText == bodyText);

        SaveNotifications();
        return 0;
    }

    public override int ClearNotificationsWithTag(string tag)
    {
        if (m_notifications == null)
        {
            return 0;
        }
        foreach (var notification in m_notifications.Where(n => n.Tag == tag))
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.Tag == tag);

        SaveNotifications();

        return 0;
    }

    public override int Cancel(int which)
    {

        foreach (var notification in m_notifications.Where(n => n.IntID == which))
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
        }

        m_notifications.RemoveAll(n => n.IntID == which);

        SaveNotifications();

        return 0;
    }

    public override int ClearAllNotifications()
    {
        m_notifications.Clear();
        AndroidNotificationCenter.CancelAllNotifications();
        //NPBinding.NotificationService.CancelAllLocalNotification();

        SaveNotifications();

        return 0;
    }

    public override int FindMatchReplace(string original, string replaceText)
    {
        var notification = m_notifications.FirstOrDefault(n => n.BodyText == original);
        if (notification != null)
        {
            AndroidNotificationCenter.CancelNotification(notification.IntID);
            //NPBinding.NotificationService.CancelLocalNotification(notification.ID);
            notification.BodyText = replaceText;
            notification.ID = string.Empty;
            notification.IntID = CreateNotification(notification.Seconds, replaceText, notification.ButtonText, notification.LargeIconName);
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
    }

    public override string GetScheduledNotificationDebugLog()
    {
        //return AndroidSpecific.mActivityJavaObject.Call<string>("_nativeNotificationGetScheduledNotificationDebugLog", new object[0]);
        return null;
    }


    private int CreateAndAddNotification(int secondsAfterNow, string bodyText, string buttonText, string tag, int groupId, string largeIcon = null)
    {
        if(m_notifications==null)
            m_notifications = new List<Notification>();
        
        var notificationID = CreateNotification(secondsAfterNow, bodyText, buttonText, largeIcon);
        var newNotification = new Notification();
        newNotification.ID = string.Empty;//cpNotification.GetNotificationID();
        newNotification.GroupID = groupId;
        newNotification.Tag = tag;
        newNotification.BodyText = bodyText;
        newNotification.IntID = notificationID;
        newNotification.Seconds = secondsAfterNow;
        m_notifications.Add(newNotification);
        return newNotification.IntID;
    }

    private int CreateNotification(long fireAfterSec,string body, string contentTitle, string largeIcon = null)
    {
        //// User info - Is used to set custom data. Create a dictionary and set your data if any.
        ////IDictionary _userInfo = new Dictionary<string, string>();
        ////_userInfo["data"] = "add what is required";

        //// Set iOS specific properties
        ////CrossPlatformNotification.iOSSpecificProperties _iosProperties = new CrossPlatformNotification.iOSSpecificProperties();
        ////_iosProperties.HasAction = true;
        ////_iosProperties.AlertAction = "alert action";

        //// Set Android specific properties
        //CrossPlatformNotification.AndroidSpecificProperties _androidProperties = new CrossPlatformNotification.AndroidSpecificProperties();
        //_androidProperties.ContentTitle = contentTitle;
        ////_androidProperties.TickerText = "ticker ticks over here";
        //_androidProperties.LargeIcon = "icon.png"; //Keep the files in Assets/PluginResources/VoxelBusters/NativePlugins/Android folder.

        //// Create CrossPlatformNotification instance
        //CrossPlatformNotification _notification = new CrossPlatformNotification();
        //_notification.AlertBody = body; //On Android, this is considered as ContentText
        //_notification.FireDate = System.DateTime.Now.AddSeconds(fireAfterSec);
        //_notification.RepeatInterval = eNotificationRepeatInterval.NONE;
        ////_notification.UserInfo = _userInfo;
        ////_notification.SoundName = "Notification.mp3"; //Keep the files in Assets/PluginResources/NativePlugins/Android or iOS or Common folder.

        ////_notification.iOSProperties = _iosProperties;
        //_notification.AndroidProperties = _androidProperties;

        //NPBinding.NotificationService.ScheduleLocalNotification(_notification);

        //return _notification;

        GTDebug.Log(GTLogChannel.Notification,"Scheduling notification : "+ body+" , after : "+ fireAfterSec);
        var notification = new AndroidNotification();
        notification.Title = contentTitle;
        notification.Text = body;
        notification.FireTime = System.DateTime.Now.AddSeconds(fireAfterSec);

        return AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
#endif


public class Notification
{
    public string ID;
    public int GroupID;
    public string BodyText;
    public string Tag;
    public string ButtonText;
    public int Seconds;
    public int IntID;
    #region UI buttons

    public string LargeIconName;

    #endregion
}
