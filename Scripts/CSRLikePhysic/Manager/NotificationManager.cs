using System;
using I2.Loc;
using UnityEngine;

public class NotificationManager
{
    private static string m_uniqueIdentifier;

    public const int GROUP_ID_DELIVERY = 1;

    public const int GROUP_ID_CHALLENGE = 2;

    public const int GROUP_ID_INACTIVITY = 3;

    public const int GROUP_ID_TRASHTALK = 4;

    public const int GROUP_ID_REMINDER = 5;

    public const int GROUP_ID_FUEL = 6;

    public const int GROUP_ID_CARE_PACAKGE = 7;

    public const string TAG_KEY = "NotificationTag";

    public const string TAG_CARE_PACKAGE = "CarePackage";

    private const string DailyBattlesTodayReminderTag = "DailyBattlesTodayReminder";

    private const string DailyBattlesTomorrowReminderTag = "DailyBattlesTomorrowReminder";

    public static NotificationManager Active
    {
        get;
        set;
    }

    protected bool IsEnabled
    {
        get
        {
            return PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null && PlayerProfileManager.Instance.ActiveProfile.OptionNotifications;
        }
        private set
        {
        }
    }

    public static void CreateActiveManager()
    {
        m_uniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        GTDebug.Log(GTLogChannel.Notification,"Device Token : "+ m_uniqueIdentifier);
        if (NotificationManager.Active != null)
        {
        }
        if (GTPlatform.Runtime == GTPlatforms.iOS )//|| GTPlatform.Runtime == GTPlatforms.OSX)
        {
#if UNITY_IOS
            NotificationManager.Active = new IOSNotificationManager();
#endif
        }
        else if (GTPlatform.Runtime == GTPlatforms.ANDROID)
        {
#if UNITY_ANDROID
            NotificationManager.Active = new Z2HAndroidNotificationManager();
#endif
        }
        else if (GTPlatform.Runtime == GTPlatforms.WP8)
        {
            NotificationManager.Active = new NotificationManager();
        }
        else if (GTPlatform.Runtime == GTPlatforms.METRO)
        {
            NotificationManager.Active = new NotificationManager();
        }
        else
        {
            NotificationManager.Active = new NotificationManager();
        }

        ApplicationManager.DidBecomeActiveEvent += Active.DidBecomeActive;
    }

    public int FindMatchUpdate(int secondsAfterNow, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        if (this.IsEnabled)
        {
            return this.findMatchUpdate(secondsAfterNow, bodyText, buttonText, groupId, largeIcon);
        }
        return -1;
    }

    public int FindMatchUpdateTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId)
    {
        if (this.IsEnabled)
        {
            return this.findMatchUpdateTag(secondsAfterNow, tag, bodyText, buttonText, groupId);
        }
        return -1;
    }

    public int AddWithTag(int secondsAfterNow, string id, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        if (this.IsEnabled)
        {
            return this.addWithTag(secondsAfterNow, id, bodyText, buttonText, groupId, largeIcon);
        }
        return -1;
    }

    public int Add(int secondsAfterNow, string bodyText, string buttonText, int groupId)
    {
        if (this.IsEnabled)
        {
            return this.add(secondsAfterNow, bodyText, buttonText, groupId);
        }
        return -1;
    }

    protected virtual int findMatchUpdate(int secondsAfterNow, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        return 0;
    }

    protected virtual int findMatchUpdateTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        return 0;
    }

    protected virtual int addWithTag(int secondsAfterNow, string tag, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        return 0;
    }

    protected virtual int add(int secondsAfterNow, string bodyText, string buttonText, int groupId, string largeIcon = null)
    {
        return 0;
    }

    public virtual int FindMatchRemove(string bodyText)
    {
        return 0;
    }

    public virtual int Cancel(int which)
    {
        return 0;
    }

    public virtual void ClearNotificationNumber()
    {
    }

    public virtual int ClearAllNotifications()
    {
        return 0;
    }

    public virtual int FindMatchReplace(string original, string replaceText)
    {
        return 0;
    }

    public virtual int ClearWrongLanguageNotifications()
    {
        return 0;
    }

    public virtual int ClearNotificationsWithTag(string tag)
    {
        return 0;
    }

    public virtual string LaunchNotificationTag()
    {
        return string.Empty;
    }

    public virtual void AskForNotificationPermission()
    {
    }

    public virtual string GetUnityDeviceToken()
    {
        return m_uniqueIdentifier;
    }

    public virtual string GetNotificationServicesDeviceToken()
    {
        return m_uniqueIdentifier;
    }

    public virtual string GetScheduledNotificationDebugLog()
    {
        return "Not implemented on this platform";
    }

    public void UpdateDailyBattlesNextRace(TimeSpan time)
    {
        string bodyText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_DAILYRACESREMINDER");
        string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_DAILYRACESREMINDER_ACTION");
        GTDebug.Log(GTLogChannel.Notification, "UpdateDailyBattlesNextRace : second after " + time.TotalSeconds + "   " + bodyText + "   " + buttonText);
        this.FindMatchUpdateTag((int)time.TotalSeconds, DailyBattlesTodayReminderTag, bodyText, buttonText, GROUP_ID_REMINDER);
    }

    public void UpdateDailyBattlesFirstRaceTomorrow()
    {
        DateTime d = PlayerProfileManager.Instance.ActiveProfile.UserStartedLastSession.Add(DailyBattleRewardManager.Instance.TimeBeforeTomorrowNotification);
        TimeSpan timeSpan = d - GTDateTime.Now;
        string bodyText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_DAILYRACESREMINDER_TOMORROW");
        string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_DAILYRACESREMINDER_ACTION");
        GTDebug.Log(GTLogChannel.Notification, "UpdateDailyBattlesFirstRaceTomorrow : second after " + timeSpan.TotalSeconds + "   " + bodyText + "   " + buttonText);
        this.FindMatchUpdateTag((int)timeSpan.TotalSeconds, DailyBattlesTomorrowReminderTag, bodyText, buttonText, GROUP_ID_REMINDER);
    }

    public void UpdateFuelNotification(DateTime WhenFill)
    {
        string bodText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_FUELFILLED");
        int secondsAfterNow = (int)(WhenFill - GTDateTime.Now).TotalSeconds;
        if (secondsAfterNow < 10)
        {
            if (!string.IsNullOrEmpty(bodText))
            {
                this.FindMatchRemove(bodText);
            }
            return;
        }
        string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_DAILYRACESREMINDER_ACTION");
        string icon = null;
        icon = BasePlatform.ActivePlatform.InsideCountry ? "icon_0" : "icon_1";
        if (!string.IsNullOrEmpty(bodText) && !string.IsNullOrEmpty(buttonText))
        {
            this.FindMatchUpdate(secondsAfterNow, bodText, buttonText, GROUP_ID_FUEL, icon);
        }
    }

    public void UpdateUnlimitedFuelNotification(DateTime WhenToShow)
    {
        string bodyText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UNLIMITED_FUEL_REMINDER");
        string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UNLIMITED_FUEL_REMINDER_ACTION");
        int num = (int)(WhenToShow - GTDateTime.Now).TotalSeconds;
        string icon = null;
        if (num < 10)
        {
            this.FindMatchRemove(bodyText);
        }
        else
        {
            icon = BasePlatform.ActivePlatform.InsideCountry ? "icon_0" : "icon_1";
            this.FindMatchUpdate(num, bodyText, buttonText, GROUP_ID_REMINDER, icon);
        }
    }

    public void UpdateRaceTeamNotification(DateTime WhenToShow)
    {
        string bodyText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_RACE_TEAM_REMINDER");
        string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_RACE_TEAM_REMINDER_ACTION");
        int num = (int)(WhenToShow - GTDateTime.Now).TotalSeconds;
        string icon = null;
        if (num < 10)
        {
            this.FindMatchRemove(bodyText);
        }
        else
        {
            icon = BasePlatform.ActivePlatform.InsideCountry ? "icon_0" : "icon_1";
            this.FindMatchUpdate(num, bodyText, buttonText, GROUP_ID_REMINDER, icon);
        }
    }

    public void ClearCarePackageNotifications()
    {
        this.ClearNotificationsWithTag(TAG_CARE_PACKAGE);
    }

    public bool LaunchedViaNotification(string tag)
    {
        return tag == this.LaunchNotificationTag();
    }

    public void DidBecomeActive()
    {
        this.ClearNotificationNumber();
    }

    public void RemoveOldLocalNotifications()
    {
        this.ClearWrongLanguageNotifications();
        this.FindMatchRemove(LocalizationManager.GetTranslation("TEXT_7DAY_CREW_LEAVE_NOTIFICATION"));
        this.FindMatchRemove(LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_NOTPLAYEDFORAWHILE"));
    }
}
