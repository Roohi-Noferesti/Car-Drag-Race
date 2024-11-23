using System.Collections.Generic;
using UnityEngine;

public static class AchievementChecks
{
	private static Dictionary<string, Achievement> manufacturerAchievements = new Dictionary<string, Achievement>
	{
		{
			"id_bmw",
			Achievements.BuyBMW
		},
		{
			"id_chevrolet",
			Achievements.BuyChevrolet
		},
		{
			"id_nissan",
			Achievements.BuyNissan
		},
		{
			"id_audi",
			Achievements.BuyAudi
		},
		{
			"id_mini",
			Achievements.BuyMini
		},
		{
			"id_ford",
			Achievements.BuyFord
		}
	};

	private static Dictionary<int, Achievement> crewLeaderAchievements = new Dictionary<int, Achievement>
	{
		{
			4105,
			Achievements.BeatTier1CrewLeader
		},
		{
			4205,
			Achievements.BeatTier2CrewLeader
		},
		{
			4305,
			Achievements.BeatTier3CrewLeader
		},
		{
			4405,
			Achievements.BeatTier4CrewLeader
		},
		{
			4505,
			Achievements.BeatAllCrewLeaders
		}
	};

	public static void AchievementsCheck()
	{
		CheckForNumberOfCarsOwnedAchievements();
	}

	public static void CheckForAllUpgradesAchievement()
	{
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		if (IsCarInstanceFullyUpgraded(currentCar))
		{
			SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.FullyUpgradeACar);
		}
	}

	public static bool IsCarInstanceFullyUpgraded(CarGarageInstance carInstance)
	{
		foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
		{
			byte b = carInstance.UpgradeStatus[current].levelOwned;
			if (ArrivalManager.Instance.isUpgradeOnOrder(carInstance.CarDBKey, current))
			{
				b += 1;
			}
			if (b < CarUpgradeData.NUM_UPGRADE_LEVELS)
			{
				return false;
			}
		}
		return true;
	}

	public static void CheckForCarsOwnedAchievements()
	{
		CheckForTierAchievements();
		CheckForNumberOfCarsOwnedAchievements();
	}

	public static void CheckForTierAchievements()
	{
		bool[] array = new bool[6];
		foreach (CarGarageInstance current in PlayerProfileManager.Instance.ActiveProfile.CarsOwned)
		{
			CarInfo car = CarDatabase.Instance.GetCar(current.CarDBKey);
			array[(int)car.BaseCarTier] = true;
		}
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (array[i])
			{
				num++;
			}
		}
		if (num >= 4)
		{
			SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.BuyACarFrom4Tiers);
		}
	}

	public static void CheckForNumberOfCarsOwnedAchievements()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Count + ArrivalManager.Instance.HowManyCarsAreOnOrder() >= 18)
		{
            SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.Own18Cars);
		}
		if (PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Count + ArrivalManager.Instance.HowManyCarsAreOnOrder() >= 40)
		{
            SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.Own40Cars);
		}
	}

	public static string CheckForManufacturerAchievement(string zManufacturerID)
	{
        //return null;
		if (!manufacturerAchievements.ContainsKey(zManufacturerID))
		{
			return string.Empty;
		}
		Achievement achievement = manufacturerAchievements[zManufacturerID];
		SocialGamePlatformSelector.Instance.ReportAchievement(achievement);
		return achievement.CategoryIDName;
	}

	public static void CheckForRaceEndAchievementsAndLeaderboards()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<Achievement> list = new List<Achievement>();
		RaceResultsData you = RaceResultsTracker.You;
		RaceResultsData them = RaceResultsTracker.Them;
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (!currentEvent.IsHalfMile && (double)you.RaceTime < 10.0)
		{
			list.Add(Achievements.TenSecQuarter);
		}
		if (currentEvent.IsHalfMile && (double)you.RaceTime < 15.0)
		{
			list.Add(Achievements.FifteenSecHalf);
		}
		if ((double)you.SpeedWhenCrossingFinishLine >= 180.0)
		{
			list.Add(Achievements.Reach180);
		}
		float num = you.RaceTime - them.RaceTime;
		if ((double)num < -1.0)
		{
			list.Add(Achievements.WinByASecond);
		}
		if (num < 0f && (double)num > -0.01)
		{
			list.Add(Achievements.WinByPoint01);
		}
		if ((double)num > 0.0 && (double)num < 0.01)
		{
			list.Add(Achievements.LoseByPoint01);
		}
		if (you.NumberOfChanges >= 3 && you.NumberOfChanges == you.NumberOfOptimalChanges)
		{
			list.Add(Achievements.PerfectShift);
		}
		if (you.WheelSpinDistance > 152.4f)
		{
			list.Add(Achievements.Wheelspin500);
		}
		if (you.IsWinner)
		{
			if (you.NumberOfChanges == 0)
			{
				list.Add(Achievements.WinInFirst);
			}
			if (crewLeaderAchievements.ContainsKey(currentEvent.EventID))
			{
				list.Add(crewLeaderAchievements[currentEvent.EventID]);
			}
		}
		if (activeProfile != null)
		{
            //SocialGamePlatformSelector.Instance.ReportScore(activeProfile.CashEarnedDisplayCapped, Leaderboards.OverallEarnings);
			RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
			int num2 = careerRaceEvents.Tier1.NumEvents();
			int num3 = careerRaceEvents.Tier2.NumEvents();
			int num4 = careerRaceEvents.Tier3.NumEvents();
			int num5 = careerRaceEvents.Tier4.NumEvents();
			int num6 = careerRaceEvents.Tier5.NumEvents();
			if (activeProfile.EventsCompletedTier1 == num2 / 2)
			{
				list.Add(Achievements.HalfTier1);
			}
			if (activeProfile.EventsCompletedTier1 == num2)
			{
				list.Add(Achievements.AllTier1);
			}
			if (activeProfile.EventsCompletedTier2 == num3 / 2)
			{
				list.Add(Achievements.HalfTier2);
			}
			if (activeProfile.EventsCompletedTier2 == num3)
			{
				list.Add(Achievements.AllTier2);
			}
			if (activeProfile.EventsCompletedTier3 == num4 / 2)
			{
				list.Add(Achievements.HalfTier3);
			}
			if (activeProfile.EventsCompletedTier3 == num4)
			{
				list.Add(Achievements.AllTier3);
			}
			if (activeProfile.EventsCompletedTier4 == num5 / 2)
			{
				list.Add(Achievements.HalfTier4);
			}
			if (activeProfile.EventsCompletedTier4 == num5)
			{
				list.Add(Achievements.AllTier4);
			}
			if (activeProfile.EventsCompletedTier5 == num6 / 2)
			{
				list.Add(Achievements.HalfTier5);
			}
			if (activeProfile.EventsCompletedTier5 == num6)
			{
				list.Add(Achievements.AllTier5);
			}
		}
		int score = SocialGamePlatformSelector.Instance.ConvertTimeForGamePlatform(you.RaceTime);
		if (!IngameTutorial.IsInTutorial && !RaceEventInfo.Instance.IsDailyBattleEvent && !currentEvent.IsHighStakesEvent())
		{
			CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
			eCarTier currentTier = currentCar.CurrentTier;
			Leaderboard leaderboardForRace = Leaderboards.GetLeaderboardForRace(currentTier, currentEvent.IsHalfMile);
			SocialGamePlatformSelector.Instance.ReportScore(score, leaderboardForRace);
		}
		foreach (Achievement current in list)
		{
            SocialGamePlatformSelector.Instance.ReportAchievement(current);
		}
	}
	
	public static void CheckForFirstAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.FirstAchievement);
	}

	public static void CheckForGoldSpentAchievement(int GoldSpent)
	{
		if (GoldSpent >= 250)
		{
            SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.Spend250Gold);
		}
	}

	public static void CheckForCashSpentAchievement(long CashSpent)
	{
		if (CashSpent >= 5000000L)
		{
			SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.BigSpender);
		}
	}

	public static void ReportSpeedupDeliveryAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.SpeedupDelivery);
	}

	public static void ReportRollingRoadAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.UseRollingRoad);
	}

	public static void ReportTuningAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.AdjustTuning);
	}

	public static void ReportRateAchievement()
	{
	}

	public static void ReportColourChangeAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.ChangeColour);
	}

	public static void ReportUseMechanicAchievement()
	{
		SocialGamePlatformSelector.Instance.ReportAchievement(Achievements.UseMechanic);
	}
}
