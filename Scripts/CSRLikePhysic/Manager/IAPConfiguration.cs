using System;
using System.Collections.Generic;
using PurchasableItems;
using UnityEngine;

[Serializable]
public class IAPConfiguration:ScriptableObject
{
	public string WholeTeamConsumableProductCode;

	public string WholeTeamRenewalProductCode;

	public bool WholeTeamConsumableActive;

	public string WholeTeamConsumableDescription;

	public string WholeTeamRenewalDescription;

	public int WholeTeamConsumableMinutes;

	public int WholeTeamRenewalMinutes;

	public int WholeTeamRenewalReminderMinutes;

	public int WholeTeamRenewalNotificationMinutes;

	public int WholeTeamRenewalAvailableMinutes;

	public int UpgradedGasTankSize;

	public bool UpgradedGasTankAvailable;

	public int GasTankReminderLowFuelAmount;

	public int GasTankReminderFrequencyInMinutes;

	public int GasTankReminderRepeatCount;

	public bool UnlimitedFuelActive;

	public string UnlimitedFuelProductCode;

	public int UnlimitedFuelMinutes;

	public int UnlimitedFuelRenewalReminderMinutes;

	public int UnlimitedFuelRenewalNotificationMinutes;

	public int UnlimitedFuelRenewalAvailableMinutes;

	[Header("A/B Test Properties")]
	public bool InSideCountryABTst;

	public string[] Markets;

	[Header("Purchase Items")]
	public List<PurchasableItem> PurchasableItems = new List<PurchasableItem>();
	
}

[Serializable]
public class MarketsName
{
	public bool Zarinpal;
	public bool Bazaar;
	public bool Myket;
}
