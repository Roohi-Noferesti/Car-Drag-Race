using DataSerialization;
using System;
using System.Collections.Generic;

public static class PinLockExtensions
{
	private delegate bool IsLockedDelegate(PinDetail pin, PinLockDetails lockDetails);

	private static Dictionary<string, PinLockExtensions.IsLockedDelegate> LockLookup = new Dictionary<string, PinLockExtensions.IsLockedDelegate>
	{
		{
			"Never",
			new PinLockExtensions.IsLockedDelegate(PinLockExtensions.IsNeverLocked)
		},
		{
			"Season",
			new PinLockExtensions.IsLockedDelegate(PinLockExtensions.IsSeasonLocked)
		},
		{
			"Always",
			new PinLockExtensions.IsLockedDelegate(PinLockExtensions.IsAlwaysLocked)
		}
	};

	public static bool IsLocked(this PinLock pl, PinDetail pin)
	{
		return PinLockExtensions.LockLookup.ContainsKey(pl.Type) && PinLockExtensions.LockLookup[pl.Type](pin, pl.Details);
	}

	private static bool IsSeasonLocked(PinDetail pin, PinLockDetails lockDetails)
	{
		LoadTierAction clickAction = pin.ClickAction;
		string text = null;
		if (clickAction != null && !string.IsNullOrEmpty(clickAction.themeToLoad))
		{
			text = ((clickAction.themeToLoad + clickAction.themeOption) ?? string.Empty);
		}
		bool flag = lockDetails.IntValue <= 0 ;//|| SeasonUtilities.IsPinUnlockedByCurrentSeason(lockDetails.IntValue);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (flag && text != null)
		{
			activeProfile.SetHasSeenSeasonUnlockableTheme(text);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
		flag |= (text != null && activeProfile.HasSeenSeasonUnlockableTheme(text));
		return !flag;
	}

	private static bool IsNeverLocked(PinDetail pin, PinLockDetails lockDetails)
	{
		return false;
	}

	private static bool IsAlwaysLocked(PinDetail pin, PinLockDetails lockDetails)
	{
		return true;
	}
}
