using System;
using System.Collections;
using GameAnalyticsSDK;
using UnityEngine;

public static class ConsumablesManager
{
	public static ShopScreen.PurchaseType PurchaseTypeForShop;

	public static void SetupRaceTeamConsumable(eCarConsumables consumableType, ConsumableValueData consumableData,
		int cashCost, int goldCost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (consumableData.MinutesActive > 0)
		{
			activeProfile.SetConsumableExpireTime(consumableType, consumableData.MinutesActive);
		}
		else
		{
			activeProfile.SetConsumableRacesLeft(consumableType, consumableData.RacesActive);
		}
		
		activeProfile.SpendCash(cashCost,"consumable", "CarConsumables");
		activeProfile.SpendGold(goldCost,"consumable","CarConsumables");

		ConsumablesManager.ApplyConsumablesAndSave();
	}

	public static void SetupRaceTeamConsumablePrize(eCarConsumables ConsumableType, int duration)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.ExtendConsumableDuration(ConsumableType, duration);
		ConsumablesManager.ApplyConsumablesAndSave();
	}

	public static void SetupWholeTeamConsumable(ConsumableValueData consumableData = null)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (consumableData == null)
		{
			consumableData = new ConsumableValueData();
			consumableData.MinutesActive = ((ConsumablesManager.PurchaseTypeForShop != ShopScreen.PurchaseType.Renew) ? GameDatabase.Instance.IAPs.WholeTeamConsumableLengthMinutes : GameDatabase.Instance.IAPs.WholeTeamConsumableRenewalLengthMinutes);
		}
		int minutesActive = ConsumablesManager.ValidateWholeTeamRenewalMinutes(consumableData.MinutesActive);
		IEnumerator enumerator = Enum.GetValues(typeof(eCarConsumables)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				eCarConsumables type = (eCarConsumables)((int)enumerator.Current);
				if (activeProfile.IsConsumableActive(type) && activeProfile.GetConsumableRacesLeft(type) == 0)
				{
					activeProfile.IncrementConsumableExpireTime(type, minutesActive);
				}
				else
				{
					activeProfile.SetConsumableExpireTime(type, consumableData.MinutesActive);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		activeProfile.HasSeenRenewWholeTeamIAPCondition = false;
		ConsumablesManager.ApplyConsumablesAndSave();
		ConsumablesManager.QueueLocalNotification();
	}

	public static void RevokeWholeTeamConsumable()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		IEnumerator enumerator = Enum.GetValues(typeof(eCarConsumables)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				eCarConsumables type = (eCarConsumables)((int)enumerator.Current);
				activeProfile.ResetConsumable(type);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		ConsumablesManager.CancelLocalNotification();
	}

	private static int ValidateWholeTeamRenewalMinutes(int minutesToAdd)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		int consumableExpireTimeTotalMinutes = activeProfile.GetConsumableExpireTimeTotalMinutes(eCarConsumables.WholeTeam);
		int b = iAPs.WholeTeamConsumableRenewalLengthMinutes + iAPs.WholeTeamRenewalAvailableMinutes;
		int num = Mathf.Min(consumableExpireTimeTotalMinutes + minutesToAdd, b);
		return Mathf.Clamp(num - consumableExpireTimeTotalMinutes, 0, num);
	}

	private static void QueueLocalNotification()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		DateTime d = activeProfile.GetConsumableExpireTime(eCarConsumables.WholeTeam);
		DateTime consumableExpireTimeSync = activeProfile.GetConsumableExpireTimeSync(eCarConsumables.WholeTeam);
		if (ServerSynchronisedTime.Instance.ServerTimeValid && consumableExpireTimeSync != DateTime.MinValue)
		{
			d = consumableExpireTimeSync;
		}
		IAPDatabase iAPs = GameDatabase.Instance.IAPs;
		DateTime whenToShow = d - TimeSpan.FromMinutes((double)iAPs.WholeTeamRenewalNotificationMinutes);
		NotificationManager.Active.UpdateRaceTeamNotification(whenToShow);
	}

	private static void CancelLocalNotification()
	{
		NotificationManager.Active.UpdateRaceTeamNotification(DateTime.MinValue);
	}

	private static void OnServerTimeRequestComplete(bool requestSuccessfull, DateTime serverTime)
	{
		if (requestSuccessfull && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.UpdateConsumablesFromNetworkTime();
			PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
		}
	}

	private static void ApplyConsumablesAndSave()
	{
		if (CompetitorManager.Instance.LocalCompetitor != null)
		{
			LocalPlayerInfo localPlayerInfo = CompetitorManager.Instance.LocalCompetitor.PlayerInfo as LocalPlayerInfo;
			localPlayerInfo.PopulatePhysicsCarSetup(true);
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		if (!ServerSynchronisedTime.Instance.ServerTimeValid && !ServerSynchronisedTime.Instance.RequestInProgress)
		{
			ServerSynchronisedTime.Instance.RequestServerTime(new ServerSynchronisedTime.RequestCallback(ConsumablesManager.OnServerTimeRequestComplete));
		}
	}
}
