using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using UnityEngine;

public static class RelayManager
{
	private class RelayResultData
	{
		public float HumanTime;

		public float AiTime;

		public bool PerfectStart;

		public int PerfectShifts;

		public int GoodShifts;
	}

	private const float MECHANIC_EFFECT = -0.154f;

	private const float EASY_UPPERLIMIT = -0.361f;

	private const float CHALLENINGING_UPPERLIMIT = -0.072f;

	private const float DIFFICULT_UPPERLIMIT = 0.144f;

	public static int grindRaces;

	public static int difficultyOffset;

	private static List<RelayResultData> m_races = new List<RelayResultData>();

	private static bool m_seenResults = false;

	public static void SetRaceResult(int race, RaceResultsData humanResultsData, RaceResultsData aiResultsData)
	{
		m_races.Add(new RelayResultData
		{
			HumanTime = humanResultsData.RaceTime,
			AiTime = aiResultsData.RaceTime,
			PerfectStart = humanResultsData.GreatLaunch,
			PerfectShifts = humanResultsData.NumberOfOptimalChanges,
			GoodShifts = humanResultsData.NumberOfOptimalChanges
		});
		m_races.ForEach(delegate(RelayResultData d)
		{
		});
	}

	public static float GetHumanRaceTime(int race)
	{
		RelayResultData resultData = GetResultData(race);
		return resultData.HumanTime;
	}

	public static float GetAiRaceTime(int race)
	{
		RelayResultData resultData = GetResultData(race);
		return resultData.AiTime;
	}

	public static float GetHumanTotalTime()
	{
		float num = 0f;
		foreach (RelayResultData current in m_races)
		{
			num += current.HumanTime;
		}
		return num;
	}

	public static float GetAITotalTime()
	{
		float num = 0f;
		foreach (RelayResultData current in m_races)
		{
			num += current.AiTime;
		}
		return num;
	}

	public static int GetRacesDone()
	{
		return m_races.Count;
	}

	public static int GetRaceCount()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsGrindRelay())
		{
			return grindRaces;
		}
		return currentEvent.Group.RaceEvents.Count;
	}

	public static int GetRaceCount(RaceEventData relay)
	{
		if (relay.IsGrindRelay())
		{
			return grindRaces;
		}
		return relay.Group.RaceEvents.Count;
	}

	public static void SetResultsSeen()
	{
		m_seenResults = true;
	}

	public static bool HasSeenResults()
	{
		return m_seenResults;
	}

	public static int GetPerfectStarts()
	{
		return m_races.Sum((RelayResultData r) => (!r.PerfectStart) ? 0 : 1);
	}

	public static int GetPerfectShifts()
	{
		return m_races.Sum((RelayResultData r) => r.PerfectShifts);
	}

	public static int GetGoodShifts()
	{
		return m_races.Sum((RelayResultData r) => r.GoodShifts);
	}

	public static bool HumanWon()
	{
		float num = 0f;
		float num2 = 0f;
		foreach (RelayResultData current in m_races)
		{
			num += current.HumanTime;
			num2 += current.AiTime;
		}
		return RaceTimesManager.IsPlayerRaceTimeWinner(num, ref num2, RaceTimeType.SINGLEPLAYER);
	}

	private static RelayResultData GetResultData(int race)
	{
		if (m_races.Count <= race)
		{
			return new RelayResultData();
		}
		return m_races[race];
	}

	public static void ResetRelayData()
	{
		m_races.Clear();
		m_seenResults = false;
	}

	public static bool IsGrindRelayGroup(RaceEventGroup eventGroup)
	{
		return eventGroup.IsGrindRelay;
	}

	public static bool GoToNextRelayRaceIfRequired()
	{
		if (RaceEventInfo.Instance.CurrentEvent == null)
		{
			return false;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		RaceEventData nextRelayEvent = GetNextRelayEvent();
		if (nextRelayEvent != null)
		{
			int racesDone = GetRacesDone();
			if (racesDone > 0)
			{
                ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
                ScreenManager.Instance.UpdateImmediately();
				return true;
			}
		}
		else if (currentEvent != null && currentEvent.IsRelay && GetRacesDone() > 0)
		{
			if (!HasSeenResults())
			{
                ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
                ScreenManager.Instance.UpdateImmediately();
				return true;
			}
			ResetRelayData();
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
		return false;
	}

	public static void SetupGrindRelay(ref RaceEventGroup EventGroup, int difficultySelected)
	{
		CarGarageInstance carGarageInstance = new CarGarageInstance();
		RaceEventData data = EventGroup.RaceEvents[0];
		int num = EventGroup.RaceEvents.Count;
		if (num < 4)
		{
			return;
		}
		num += -2 + difficultySelected;
		grindRaces = num;
		difficultyOffset = difficultySelected;
		List<int> list = Enumerable.Range(0, AvatarPicture.NumberOfCSRAvatars).ToList<int>();
        //list.Shuffle<int>();
		for (int i = 0; i < num; i++)
		{
			ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
			chooseRandomCars.ChooseRandomOwnedCar(out carGarageInstance);
			data = EventGroup.RaceEvents[i];
			data.AICsrAvatar = list[i];
			data.HumanCar = carGarageInstance.CarDBKey;
		}
		float num2 = GetTotalEstimatedPlayerTime(EventGroup, num);
		num2 += GameDatabase.Instance.Relay.GetTimeDifference(num);
		List<float> randomAITimesForPlayerTimes = GetRandomAITimesForPlayerTimes(num, num2);
		if (randomAITimesForPlayerTimes.Count != num)
		{
			return;
		}
		for (int j = 0; j < num; j++)
		{
			data = EventGroup.RaceEvents[j];
			carGarageInstance = PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Find((CarGarageInstance x) => x.CarDBKey == data.HumanCar);
            AutoDifficulty.GetRandomOpponentForCarAtCertainTime(randomAITimesForPlayerTimes[j], ref data, carGarageInstance);
		}
	}

	public static void GenerateRelayOpponents(ref RaceEventGroup EventGroup)
	{
		CarGarageInstance carGarageInstance = new CarGarageInstance();
		RaceEventData data = EventGroup.RaceEvents[0];
		int count = EventGroup.RaceEvents.Count;
		grindRaces = count;
        List<int> list = Enumerable.Range(0, AvatarPicture.NumberOfCSRAvatars).ToList<int>();
        list.Shuffle<int>();
        for (int i = 0; i < count; i++)
        {
            data = EventGroup.RaceEvents[i];
            //data.AICsrAvatar = list[i % list.Count<int>()];
        }
		float totalEstimatedPlayerTime = GetTotalEstimatedPlayerTime(EventGroup, count);
		List<float> randomAITimesForPlayerTimes = GetRandomAITimesForPlayerTimes(count, totalEstimatedPlayerTime);
		if (randomAITimesForPlayerTimes.Count != count)
		{
			return;
		}
		for (int j = 0; j < count; j++)
		{
			data = EventGroup.RaceEvents[j];
			carGarageInstance = PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Find((CarGarageInstance x) => x.CarDBKey == data.HumanCar);
			AutoDifficulty.GetRandomOpponentForCarAtCertainTime(randomAITimesForPlayerTimes[j], ref data, carGarageInstance);
		}
	}

	public static void SetupSwitchBack(ref RaceEventGroup EventGroup)
	{
		if (EventGroup.NumOfEvents() != 2)
		{
			return;
		}
		RaceEventData raceEventData = EventGroup.RaceEvents[0];
		RaceEventData raceEventData2 = EventGroup.RaceEvents[1];
		ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
		CarGarageInstance carFromID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(raceEventData2.GetHumanCar());
		CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
		CarGarageInstance carGarageInstance = new CarGarageInstance();
	    var car = CarDatabase.Instance.GetCar(raceEventData2.AICar);
        carGarageInstance.SetupNewGarageInstance(car);
		carUpgradeSetup.SetFullyUpgraded();
		carUpgradeSetup.CarDBKey = raceEventData2.AICar;
		carGarageInstance.SetAndApplyUpgradeLevelToAllUpgrades(5);
		raceEventData.SetUpgradeStatus(carFromID.UpgradeStatus);
		chooseRandomCars.GetSwitchBackOpponentCar(ref carGarageInstance, out carUpgradeSetup);
        carGarageInstance.AppliedLiveryName = raceEventData2.AIDriverLivery;
        //carGarageInstance.ColorOverride = raceEventData2.GetAIColour();
		carGarageInstance.UseColorOverride = raceEventData2.UseCustomShader;
		carGarageInstance.NumberPlate = new NumberPlate();
		carGarageInstance.NumberPlate.Text = GameDatabase.Instance.AIPlayers.GetAIDriverData(raceEventData2.AIDriver).NumberPlateString;
		raceEventData.SetLoanCarDetails(carUpgradeSetup, carGarageInstance);
		raceEventData.AICar = raceEventData2.GetHumanCar();
		raceEventData.UseCustomShader = false;
        raceEventData.AIDriverLivery = carFromID.AppliedLiveryName;
		raceEventData.HumanCar = raceEventData2.AICar;
	}

	public static bool IsRookieUnlocked()
	{
	    return GameDatabase.Instance.Relay.IsRookieAvailable();
	}

	public static bool IsProUnlocked()
	{
	    return GameDatabase.Instance.Relay.IsProAvailable();
	}

	public static float GetTimeDifference()
	{
		float num = 0f;
		for (int i = 0; i < m_races.Count; i++)
		{
			float humanRaceTime = GetHumanRaceTime(i);
			float aiRaceTime = GetAiRaceTime(i);
			num += aiRaceTime - humanRaceTime;
		}
		return num;
	}

	private static float GetTotalEstimatedPlayerTime(RaceEventGroup relayGroup, int numberOfEvents)
	{
		float num = 0f;
		RaceEventData raceEventData = relayGroup.RaceEvents[0];
		for (int i = 0; i < GetRaceCount(raceEventData); i++)
		{
			raceEventData = relayGroup.RaceEvents[i];
			int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(raceEventData);
			num += CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex);
		}
		return num;
	}

	private static float GetAverageAITimeForPlayerTime(int numberOfEvents, float totalTime)
	{
		return totalTime / (float)numberOfEvents;
	}

	private static List<float> GetRandomAITimesForPlayerTimes(int numberOfEvents, float totalTime)
	{
		float qMTimeForPPIndex = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(161);
		float qMTimeForPPIndex2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(720);
		if (qMTimeForPPIndex * (float)numberOfEvents < totalTime)
		{
			totalTime = qMTimeForPPIndex * (float)numberOfEvents;
		}
		else if (qMTimeForPPIndex2 * (float)numberOfEvents > totalTime)
		{
			totalTime = qMTimeForPPIndex2 * (float)numberOfEvents;
		}
		float num = totalTime;
		List<float> list = new List<float>();
		for (int i = 0; i < numberOfEvents; i++)
		{
			float num2 = num - qMTimeForPPIndex * (float)(numberOfEvents - 1 - i);
			float min = (num2 >= qMTimeForPPIndex2) ? num2 : qMTimeForPPIndex2;
			num2 = num - qMTimeForPPIndex2 * (float)(numberOfEvents - 1 - i);
			float max = (num2 <= qMTimeForPPIndex) ? num2 : qMTimeForPPIndex;
			float num3 = Random.Range(min, max);
			list.Add(num3);
			num -= num3;
		}
		return list;
	}

	public static float GetRemainingTimeDifference()
	{
		int num = GetRacesDone() + 1;
		float num2 = 0f;
		RaceEventGroup group = RaceEventInfo.Instance.CurrentEvent.Group;
		for (int i = num; i < GetRaceCount(RaceEventInfo.Instance.CurrentEvent); i++)
		{
			RaceEventData raceEventData = group.RaceEvents[i];
			int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(raceEventData);
			int aIPerformancePotentialIndex = raceEventData.GetAIPerformancePotentialIndex();
			float qMTimeForPPIndex = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex);
			float qMTimeForPPIndex2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(aIPerformancePotentialIndex);
			float num3 = qMTimeForPPIndex - qMTimeForPPIndex2;
			num2 += num3;
		}
		return num2;
	}

	public static float GetPredictedRaceTimeDifference()
	{
		int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(RaceEventInfo.Instance.CurrentEvent);
		int aIPerformancePotentialIndex = RaceEventInfo.Instance.CurrentEvent.GetAIPerformancePotentialIndex();
		float qMTimeForPPIndex = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex);
		float qMTimeForPPIndex2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(aIPerformancePotentialIndex);
		return qMTimeForPPIndex - qMTimeForPPIndex2;
	}

	public static RaceEventData GetNextRelayEvent()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (!currentEvent.IsRelay)
		{
			return null;
		}
		int racesDone = GetRacesDone();
		if (racesDone < GetRaceCount())
		{
			return currentEvent.Group.RaceEvents[racesDone];
		}
		return null;
	}

	public static bool IsCurrentEventRelay()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		return currentEvent != null && currentEvent.IsRelay;
	}

	public static int GetRelayId()
	{
		RaceEventGroup group = RaceEventInfo.Instance.CurrentEvent.Group;
		return group.EventGroupID;
	}

	public static RaceEventDifficulty.Rating GetDifficultyForRelay()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		int raceCount = GetRaceCount();
		float num = 0f;
		float num2 = 0f;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int racesDone = GetRacesDone();
		for (int i = 0; i < raceCount; i++)
		{
			bool flag = racesDone > i;
			int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(currentEvent.Group.RaceEvents[i]);
			int aIPerformancePotentialIndex = currentEvent.Group.RaceEvents[i].GetAIPerformancePotentialIndex();
			float num3;
			float num4;
			if (flag)
			{
				num3 = GetHumanRaceTime(i);
				num4 = GetAiRaceTime(i);
			}
			else
			{
				num3 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex);
				num4 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(aIPerformancePotentialIndex);
			}
			num += num3;
			num2 += num4;
		}
		float num5 = num / (float)raceCount;
		float num6 = num2 / (float)raceCount;
		float timeDifference = num5 - num6;
		RaceEventDifficulty.Rating rating = GetDifficultyForTimeDifference(timeDifference);
		if (activeProfile.MechanicTuningRacesRemaining > 0 && rating != RaceEventDifficulty.Rating.Easy && currentEvent.IsMechanicAllowed())
		{
			rating--;
		}
		return rating;
	}

	public static float CalculateExpectedTimeDifference(RaceEventData relayEvent, int racesDone)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int raceCount = GetRaceCount(relayEvent);
		float num = 0f;
		for (int i = racesDone; i < raceCount; i++)
		{
			RaceEventData raceEventData = relayEvent.Group.RaceEvents[i];
			int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(raceEventData);
			int aIPerformancePotentialIndex = raceEventData.GetAIPerformancePotentialIndex();
			float num2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex - GameDatabase.Instance.Relay.GetRelayBadRunPPDifference());
			if (activeProfile.MechanicTuningRacesRemaining > i - racesDone)
			{
				num2 += -0.154f;
			}
			float qMTimeForPPIndex = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(aIPerformancePotentialIndex);
			float num3 = num2 - qMTimeForPPIndex;
			num += num3;
		}
		float num4 = 0f;
		for (int j = 0; j < racesDone; j++)
		{
			float humanRaceTime = GetHumanRaceTime(j);
			float aiRaceTime = GetAiRaceTime(j);
			num4 += humanRaceTime - aiRaceTime;
		}
		return (num4 + num) / (float)raceCount;
	}

	private static float ConvertTimeDifferenceToDifficulty(float timeDifference)
	{
		float timeDifferenceToDifficultyGradient = GameDatabase.Instance.RelayConfiguration.TimeDifferenceToDifficultyGradient;
		float timeDifferenceToDifficultyBias = GameDatabase.Instance.RelayConfiguration.TimeDifferenceToDifficultyBias;
		return Mathf.Clamp01(0.5f - 0.318309873f * Mathf.Atan(-timeDifferenceToDifficultyGradient * (timeDifference + timeDifferenceToDifficultyBias)));
	}

	public static float ConvertTimeDifferenceToPercentage(float timeDifference)
	{
		float num = ConvertTimeDifferenceToDifficulty(timeDifference);
		if (timeDifference < -0.361f)
		{
			float num2 = ConvertTimeDifferenceToDifficulty(-0.361f);
			float num3 = num / num2;
			return num3 * 0.25f;
		}
		if (timeDifference < -0.072f)
		{
			float num4 = ConvertTimeDifferenceToDifficulty(-0.072f) - ConvertTimeDifferenceToDifficulty(-0.361f);
			float num5 = (num - ConvertTimeDifferenceToDifficulty(-0.361f)) / num4;
			return 0.25f + num5 * 0.25f;
		}
		if (timeDifference < 0.144f)
		{
			float num6 = ConvertTimeDifferenceToDifficulty(0.144f) - ConvertTimeDifferenceToDifficulty(-0.072f);
			float num7 = (num - ConvertTimeDifferenceToDifficulty(-0.072f)) / num6;
			return 0.5f + num7 * 0.25f;
		}
		float num8 = 1f - ConvertTimeDifferenceToDifficulty(0.144f);
		float num9 = (num - ConvertTimeDifferenceToDifficulty(0.144f)) / num8;
		return 0.75f + num9 * 0.25f;
	}

	public static RaceEventDifficulty.Rating GetDifficultyForTimeDifference(float timeDifference)
	{
		if (timeDifference < -0.361f)
		{
			return RaceEventDifficulty.Rating.Easy;
		}
		if (timeDifference < -0.072f)
		{
			return RaceEventDifficulty.Rating.Challenging;
		}
		if (timeDifference < 0.144f)
		{
			return RaceEventDifficulty.Rating.Difficult;
		}
		return RaceEventDifficulty.Rating.Extreme;
	}

	public static string FormatRaceTime(float time)
	{
		return time.ToString("0.000s");
	}

	public static void ShowConfirmQuitPopup(PopUpButtonAction confirmAction, bool shouldCoverNavBar)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_QUIT_RELAY_TITLE",
			BodyText = "TEXT_POPUPS_QUIT_RELAY_BODY",
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_QUIT",
			ConfirmAction = confirmAction,
			ShouldCoverNavBar = shouldCoverNavBar
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}
}
