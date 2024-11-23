using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerInfo : PlayerInfo
{
	public LocalPlayerInfo() : base(new LocalPersona())
	{
		StatsPlayerInfoComponent statsPlayerInfoComponent = base.AddComponent<StatsPlayerInfoComponent>();
		RTWPlayerInfoComponent rTWPlayerInfoComponent = base.AddComponent<RTWPlayerInfoComponent>();
		RacePlayerInfoComponent racePlayerInfoComponent = base.AddComponent<RacePlayerInfoComponent>();
		RWFPlayerInfoComponent rWFPlayerInfoComponent = base.AddComponent<RWFPlayerInfoComponent>();
		base.AddComponent<ConsumablePlayerInfoComponent>();
		base.AddComponent<MechanicPlayerInfoComponent>();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		this._csrUserName = activeProfile.Name;
		rTWPlayerInfoComponent._rankPoints = Mathf.Max(activeProfile.GetPlayerRP(), 0);
		rTWPlayerInfoComponent._worldRank = activeProfile.PlayerWorldRank;
		CarGarageInstance carGarageInstance;
		if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
		{
			carGarageInstance = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance;
		}
		else
		{
			carGarageInstance = activeProfile.GetCurrentCar();
		}
		if (RaceEventInfo.Instance.IsDailyBattleEvent)
		{
			racePlayerInfoComponent._isEliteCar = false;
		}
		else
		{
			racePlayerInfoComponent._isEliteCar = carGarageInstance.EliteCar;
		}
		racePlayerInfoComponent._hasSportsUpgrade = carGarageInstance.SportsUpgrade;
		statsPlayerInfoComponent._onlineRacesWon = activeProfile.OnlineRacesWon;
		statsPlayerInfoComponent._onlineRacesLost = activeProfile.OnlineRacesLost;
		this.PopulateTop10CarsFromProfile(activeProfile);
		this.PopulatePhysicsCarSetup(false);
		statsPlayerInfoComponent._totalCash = activeProfile.CashEarnedDisplayCapped;
		statsPlayerInfoComponent._totalGold = activeProfile.GoldEarned;
		statsPlayerInfoComponent._level = activeProfile.GetPlayerLevel();
		statsPlayerInfoComponent._carsOwned = activeProfile.CarsOwned.Count;
		statsPlayerInfoComponent._totalGaragePP = activeProfile.GetTotalGaragePP();
		if (activeProfile.BestCarTimes.ContainsKey(racePlayerInfoComponent.CarDBKey))
		{
			rWFPlayerInfoComponent._bestTimeInCurrentCar = activeProfile.BestCarTimes[racePlayerInfoComponent.CarDBKey];
		}
		statsPlayerInfoComponent._totalPlayTime = activeProfile.TotalPlayTime;
		statsPlayerInfoComponent._totalDistanceTravelled = activeProfile.TotalDistanceTravelled;
		statsPlayerInfoComponent._halfMile = activeProfile.BestOverallHalfMileTime;
		statsPlayerInfoComponent._quarterMile = activeProfile.BestOverallQuarterMileTime;
        //statsPlayerInfoComponent._starCount = StarsManager.GetMyStarStats().TotalStars;
		statsPlayerInfoComponent._bossesBeaten = this.GetBossesBeatenFromProfile(activeProfile);
        //racePlayerInfoComponent._colourIndex = carGarageInstance.AppliedColourIndex;
		racePlayerInfoComponent.AppliedLivery = carGarageInstance.AppliedLiveryName;
	}

	private int GetBossesBeatenFromProfile(PlayerProfile profile)
	{
		int num = 0;
        //foreach (int current in GameDatabase.Instance.SocialConfiguration.BossEventsIDs)
        //{
        //    if (profile.EventsCompleted.Contains(current))
        //    {
        //        num++;
        //    }
        //}
		return num;
	}

	private void PopulateTop10CarsFromProfile(PlayerProfile profile)
	{
		List<CarGarageInstance> list = new List<CarGarageInstance>(profile.CarsOwned);
		list.Sort(new Comparison<CarGarageInstance>(CompareCarsByPPIndexDescending));
		if (list.Count > 10)
		{
			list.RemoveRange(10, list.Count - 10);
		}
		StatsPlayerInfoComponent component = base.GetComponent<StatsPlayerInfoComponent>();
		component._top10OwnedCarsDBKey.Clear();
		component._top10OwnedCarsPPIndex.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			component._top10OwnedCarsDBKey.Add(list[i].CarDBKey);
			component._top10OwnedCarsPPIndex.Add(list[i].CurrentPPIndex);
		}
	}

	public void PopulatePhysicsCarSetup(bool afterConsumableOrUpgrade = true)
	{
		RacePlayerInfoComponent component = base.GetComponent<RacePlayerInfoComponent>();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CarGarageInstance carGarageInstance;
		if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace() && !afterConsumableOrUpgrade)
		{
			carGarageInstance = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance;
		}
		else
		{
			carGarageInstance = activeProfile.GetCurrentCar();
		}
		component.CarDBKey = carGarageInstance.CarDBKey;
		component._carTier = carGarageInstance.CurrentTier;
		component._ppIndex = carGarageInstance.CurrentPPIndex;
		component.CarUpgradeStatus = carGarageInstance.UpgradeStatus;
		ConsumablePlayerInfoComponent component2 = base.GetComponent<ConsumablePlayerInfoComponent>();
	    component2.ConsumablePRAgent = !activeProfile.IsConsumableActive(eCarConsumables.PRAgent) ? 0 : 1;
	    component2.ConsumableEngine =  !activeProfile.IsConsumableActive(eCarConsumables.EngineTune) ? 0 : 1;
	    component2.ConsumableTyre =  !activeProfile.IsConsumableActive(eCarConsumables.Tyre) ? 0 : 1;
	    component2.ConsumableN2O =  !activeProfile.IsConsumableActive(eCarConsumables.Nitrous) ? 0 : 1;
		MechanicPlayerInfoComponent component3 = base.GetComponent<MechanicPlayerInfoComponent>();
		component3.MechanicEnabled = (activeProfile.MechanicTuningRacesRemaining > 0 && RaceEventInfo.Instance.ShouldCurrentEventUseMechanic());
		if (RaceEventInfo.Instance.LocalPlayerCarUpgradeSetup != null)
		{
			RaceEventInfo.Instance.LocalPlayerCarUpgradeSetup.UpgradeStatus = component.CarUpgradeStatus;
		}
	}
}
