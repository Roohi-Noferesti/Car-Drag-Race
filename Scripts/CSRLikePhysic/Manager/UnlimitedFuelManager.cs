using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class UnlimitedFuelManager
{
	public static bool IsActive
	{
		get
		{
			return TimeRemaining > TimeSpan.Zero;
		}
	}

	public static TimeSpan TimeRemaining
	{
		get
		{
			PlayerProfile activeProfile = ActiveProfile;
			TimeSpan timeSpan = TimeSpan.Zero;
			DateTime d = activeProfile.UnlimitedFuelExpires;
			DateTime unlimitedFuelExpiresSync = activeProfile.UnlimitedFuelExpiresSync;
            DateTime d2 = ServerSynchronisedTime.Instance.GetDateTime();
			if (ServerSynchronisedTime.Instance.ServerTimeValid && unlimitedFuelExpiresSync != DateTime.MinValue)
			{
				d = unlimitedFuelExpiresSync;
				d2 = ServerSynchronisedTime.Instance.GetDateTime();
			}
			timeSpan = d - d2;
			if (timeSpan <= TimeSpan.Zero)
			{
				return TimeSpan.Zero;
			}
			return timeSpan;
		}
	}

    public static DateTime ExpireTime
    {
        get { return ActiveProfile.UnlimitedFuelExpires; }
    }

	private static PlayerProfile ActiveProfile
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile;
		}
	}

	public static void Apply()
	{
		int unlimitedFuelMinutes = GameDatabase.Instance.IAPs.UnlimitedFuelMinutes;
		GiveMinutes(unlimitedFuelMinutes);
	}

	private static void GiveMinutes(int minutesToAdd)
	{
		minutesToAdd = ValidateRenewalMinutes(minutesToAdd);
		if (IsActive)
		{
			IncrementExpireTime(minutesToAdd);
		}
		else
		{
			SetExpireTime(minutesToAdd);
		}
		ActiveProfile.HasSeenUnlimitedFuelRenewalPopup = false;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		QueueLocalNotification();
	}

	public static void Revoke()
	{
		PlayerProfile activeProfile = ActiveProfile;
		activeProfile.UnlimitedFuelExpires = DateTime.MinValue;
		activeProfile.UnlimitedFuelExpiresSync = DateTime.MinValue;
		activeProfile.UnlimitedFuelMinutesPurchased = 0;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		CancelLocalNotification();
	}

	public static bool IsPlayerClockCheating()
	{
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		return TimeRemaining.TotalMinutes > (double)(iAPs.UnlimitedFuelMinutes + iAPs.UnlimitedFuelRenewalAvailableMinutes + 2);
	}

	private static void SetExpireTime(int minutes)
	{
		PlayerProfile activeProfile = ActiveProfile;
        DateTime unlimitedFuelExpires = ServerSynchronisedTime.Instance.GetDateTime().AddMinutes((double)minutes);
		DateTime unlimitedFuelExpiresSync = DateTime.MinValue;
		if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			unlimitedFuelExpiresSync = ServerSynchronisedTime.Instance.GetDateTime().AddMinutes((double)minutes);
		}
		activeProfile.UnlimitedFuelExpires = unlimitedFuelExpires;
		activeProfile.UnlimitedFuelExpiresSync = unlimitedFuelExpiresSync;
		activeProfile.UnlimitedFuelMinutesPurchased = minutes;
	}

	private static void IncrementExpireTime(int minutesToAdd)
	{
		PlayerProfile activeProfile = ActiveProfile;
		DateTime unlimitedFuelExpires = activeProfile.UnlimitedFuelExpires.AddMinutes((double)minutesToAdd);
		int unlimitedFuelMinutesPurchased = TimeRemaining.Minutes + minutesToAdd;
		DateTime unlimitedFuelExpiresSync = DateTime.MinValue;
		DateTime unlimitedFuelExpiresSync2 = activeProfile.UnlimitedFuelExpiresSync;
		if (ServerSynchronisedTime.Instance.ServerTimeValid && unlimitedFuelExpiresSync2 != DateTime.MinValue)
		{
			unlimitedFuelExpiresSync = unlimitedFuelExpiresSync2.AddMinutes((double)minutesToAdd);
		}
		activeProfile.UnlimitedFuelExpires = unlimitedFuelExpires;
		activeProfile.UnlimitedFuelExpiresSync = unlimitedFuelExpiresSync;
		activeProfile.UnlimitedFuelMinutesPurchased = unlimitedFuelMinutesPurchased;
	}

	private static int ValidateRenewalMinutes(int minutesToAdd)
	{
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		int num = (int)TimeRemaining.TotalMinutes;
		int b = iAPs.UnlimitedFuelMinutes + iAPs.UnlimitedFuelRenewalAvailableMinutes;
		int num2 = Mathf.Min(num + minutesToAdd, b);
		return Mathf.Clamp(num2 - num, 0, num2);
	}

	private static void QueueLocalNotification()
	{
		PlayerProfile activeProfile = ActiveProfile;
		DateTime d = activeProfile.UnlimitedFuelExpires;
		DateTime unlimitedFuelExpiresSync = activeProfile.UnlimitedFuelExpiresSync;
		if (ServerSynchronisedTime.Instance.ServerTimeValid && unlimitedFuelExpiresSync != DateTime.MinValue)
		{
			d = unlimitedFuelExpiresSync;
		}
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		DateTime whenToShow = d - TimeSpan.FromMinutes((double)iAPs.UnlimitedFuelRenewalNotificationMinutes);
		NotificationManager.Active.UpdateUnlimitedFuelNotification(whenToShow);
	}

	private static void CancelLocalNotification()
	{
		NotificationManager.Active.UpdateUnlimitedFuelNotification(DateTime.MinValue);
	}
}
