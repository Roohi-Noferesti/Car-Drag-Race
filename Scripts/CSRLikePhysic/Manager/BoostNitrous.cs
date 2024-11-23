using System.Collections.Generic;
using System.Linq;

public static class BoostNitrous
{
	private static Dictionary<eCarTier, int> FinalBossEvents = new Dictionary<eCarTier, int>
	{
		{
			eCarTier.TIER_1,
			5383
		},
		{
			eCarTier.TIER_2,
			4273
		},
		{
			eCarTier.TIER_3,
			10133
		},
		{
			eCarTier.TIER_4,
			20133
		},
		{
			eCarTier.TIER_5,
			30133
		}
	};

	private static Dictionary<eCarTier, int> ChallengeBossEvents = new Dictionary<eCarTier, int>
	{
		{
			eCarTier.TIER_1,
			5384
		},
		{
			eCarTier.TIER_2,
			4274
		},
		{
			eCarTier.TIER_3,
			10134
		},
		{
			eCarTier.TIER_4,
			20134
		},
		{
			eCarTier.TIER_5,
			30134
		}
	};

	public static bool HaveBoostNitrous()
	{
	    return false;
		//PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //return UserManager.Instance.currentAccount.SuperNitrous > activeProfile.BoostNitrousUsed;
	}

	public static int GetChallengeRaceIDForTier(eCarTier carTier)
	{
		return ChallengeBossEvents[carTier];
	}

	public static bool IsLegacyBoostNitrousEvent(int eventID)
	{
		return ChallengeBossEvents.ContainsValue(eventID);
	}

	public static eCarTier GetBestBoostNitrousTierForUser()
	{
		List<int> eventsCompleted = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted;
		eCarTier result = eCarTier.TIER_1;
		bool flag = true;
		foreach (KeyValuePair<eCarTier, int> current in FinalBossEvents)
		{
			if (!eventsCompleted.Contains(current.Value))
			{
				flag = false;
				break;
			}
			result = current.Key;
		}
		if (flag)
		{
			return eCarTier.TIER_5;
		}
		return result;
	}

	public static bool TierBossChallengeFinished(eCarTier tier)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
	    return activeProfile.GetBossChallengeState(tier) == BossChallengeStateEnum.FINISHED;
	}

	public static bool TryGetBossChallenge()
	{
		eCarTier ecarTier = eCarTier.TIER_1;
        return TryGetBossChallenge(ref ecarTier);
	}

	private static bool TryGetBossChallenge(ref eCarTier challengeTier)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		foreach (KeyValuePair<eCarTier, int> current in FinalBossEvents)
		{
			eCarTier key = current.Key;
			int value = current.Value;
            //if (activeProfile.GetBossChallengeState(key) != BossChallengeStateEnum.FINISHED)
            //{
            //    if (activeProfile.GetBossChallengeState(key) == BossChallengeStateEnum.INPROGRESS && UserManager.Instance.currentAccount.SuperNitrous <= activeProfile.BoostNitrousUsed)
            //    {
            //        activeProfile.BoostNitrousUsed = UserManager.Instance.currentAccount.SuperNitrous;
            //        activeProfile.SetBossChallengeState(key, BossChallengeStateEnum.FINISHED);
            //    }
            //    else if (activeProfile.EventsCompleted.Contains(value))
            //    {
            //        challengeTier = key;
            //        return true;
            //    }
            //}
		}
		return false;
	}

	public static bool CheckAndPushBossChallenge()
	{
		eCarTier tierForChallenge = eCarTier.TIER_1;
		if (TryGetBossChallenge(ref tierForChallenge))
		{
            //HighStakesScreenBase.TierForChallenge = tierForChallenge;
            //ScreenManager.Instance.PushScreen(ScreenID.HighStakesChallenge);
			return true;
		}
		return false;
	}

	public static bool AwardBossCar(CarGarageInstance CarToGive, CarUpgradeSetup UpgradeSetup, bool awardAsElite)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<CarGarageInstance> carsOwned = activeProfile.CarsOwned;
		if (carsOwned.Any((CarGarageInstance car) => car.CarDBKey == CarToGive.CarDBKey))
		{
			return false;
		}
		activeProfile.GiveCar(CarToGive.CarDBKey, 0/*CarToGive.AppliedColourIndex*/, false);
		CarGarageInstance carGarageInstance = carsOwned.FirstOrDefault((CarGarageInstance x) => x.CarDBKey == CarToGive.CarDBKey);
		if (carGarageInstance == null)
		{
			return false;
		}
		carGarageInstance.EliteCar |= awardAsElite;
		carGarageInstance.AppliedLiveryName = CarToGive.AppliedLiveryName;
		foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
		{
			CarUpgradeStatus carUpgradeStatus = UpgradeSetup.UpgradeStatus[current];
			CarUpgradeStatus carUpgradeStatus2 = carGarageInstance.UpgradeStatus[current];
			carUpgradeStatus2.levelFitted = carUpgradeStatus.levelFitted;
			carUpgradeStatus2.levelOwned = carUpgradeStatus.levelFitted;
		}
		return true;
	}

	public static RaceEventData GetChallengeRaceForTier(eCarTier carTier)
	{
		switch (carTier)
		{
		case eCarTier.TIER_1:
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.WinACarEvent;
		case eCarTier.TIER_2:
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier2.WinACarEvent;
		case eCarTier.TIER_3:
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier3.WinACarEvent;
		case eCarTier.TIER_4:
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier4.WinACarEvent;
		case eCarTier.TIER_5:
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier5.WinACarEvent;
		default:
			return null;
		}
	}

	public static void DoRaceReward(bool didWin, RaceEventData eventData)
	{
        //PlayerProfileManager.Instance.ActiveProfile.SetBossChallengeState(HighStakesScreenBase.TierForChallenge, BossChallengeStateEnum.FINISHED);
        //if (UserManager.Instance.currentAccount.SuperNitrous > PlayerProfileManager.Instance.ActiveProfile.BoostNitrousUsed)
        //{
        //    PlayerProfileManager.Instance.ActiveProfile.BoostNitrousUsed++;
        //}
		if (didWin)
		{
			bool awardAsElite = eventData.IsWorldTourHighStakesEvent();// && !CarDataDefaults.NonEliteBossCars.Contains(RaceEventInfo.Instance.AICarGarageInstance.CarDBKey);
			AwardBossCar(RaceEventInfo.Instance.AICarGarageInstance, RaceEventInfo.Instance.OpponentCarUpgradeSetup, awardAsElite);
			PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = RaceEventInfo.Instance.AICarGarageInstance.CarDBKey;
		}
		if (eventData.IsWorldTourRace())
		{
            //WorldTourBoostNitrous worldTourBoostNitrous = PlayerProfileManager.Instance.ActiveProfile.WorldTourBoostNitrous;
            //worldTourBoostNitrous.SetRaceFinished(eventData.EventID, eventData.GetWorldTourPinPinDetail().PinID);
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public static int GetGoldPunishmentFromTierBossRace(eCarTier tier)
	{
		if (!FinalBossEvents.ContainsKey(tier))
		{
			return 0;
		}
		int zEventIndex = FinalBossEvents[tier];
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(zEventIndex);
		if (eventByEventIndex == null)
		{
			return 0;
		}
		return (int) eventByEventIndex.RaceReward.GoldPrize;
	}
}
