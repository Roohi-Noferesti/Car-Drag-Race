using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class DifficultyManager
{
	private static Dictionary<DifficultyEventType, DifficultySettingsInstance> difficultyInstanceSettings = new Dictionary<DifficultyEventType, DifficultySettingsInstance>
	{
		{
			DifficultyEventType.RaceTheWorld,
			null
		},
		{
			DifficultyEventType.EliteClub,
			null
		},
		{
			DifficultyEventType.MultiplayerEvent,
			null
		}
	};

	public static DifficultyEventType CurrentEventType
	{
		get
		{
			switch (MultiplayerUtils.SelectedMultiplayerMode)
			{
			case MultiplayerMode.PRO_CLUB:
				return DifficultyEventType.EliteClub;
			case MultiplayerMode.EVENT:
				return DifficultyEventType.MultiplayerEvent;
			}
			return DifficultyEventType.RaceTheWorld;
		}
	}

	public static float RTWDifficulty
	{
		get
		{
			return GetDifficultySettingsInstance(DifficultyEventType.RaceTheWorld).Difficulty;
		}
		set
		{
			GetDifficultySettingsInstance(DifficultyEventType.RaceTheWorld).Difficulty = value;
		}
	}

	public static float EliteDifficulty
	{
		get
		{
			return GetDifficultySettingsInstance(DifficultyEventType.EliteClub).Difficulty;
		}
		set
		{
			GetDifficultySettingsInstance(DifficultyEventType.EliteClub).Difficulty = value;
		}
	}

	public static float MultiplayerEventDifficulty
	{
		get
		{
			return GetDifficultySettingsInstance(DifficultyEventType.MultiplayerEvent).Difficulty;
		}
		set
		{
			GetDifficultySettingsInstance(DifficultyEventType.MultiplayerEvent).Difficulty = value;
		}
	}

	public static DifficultySettings GetDifficultySettings(DifficultyEventType eventType)
	{
		switch (eventType)
		{
		case DifficultyEventType.RaceTheWorld:
			return GameDatabase.Instance.DifficultyConfiguration.Settings;
		case DifficultyEventType.EliteClub:
			return GameDatabase.Instance.DifficultyConfiguration.EliteSettings;
		case DifficultyEventType.MultiplayerEvent:
		{
			MultiplayerEventData data = MultiplayerEvent.Saved.Data;
			return (data == null) ? new DifficultySettings() : data.EventDifficultySettings;
		}
		default:
			return null;
		}
	}

	private static DifficultySettingsInstance GetDifficultySettingsInstance(DifficultyEventType eventType)
	{
		DifficultySettingsInstance difficultySettingsInstance = difficultyInstanceSettings[eventType];
		if (difficultySettingsInstance == null)
		{
			DifficultySettings difficultySettings = GetDifficultySettings(eventType);
			if (difficultySettings != null)
			{
				if (difficultySettings.RaceWindow.Count == 0 || difficultySettings.StreakLossDeltas.Count == 0 || difficultySettings.StreakWinDeltas.Count == 0)
				{
					difficultySettings = null;
				}
				else if (difficultySettings.StreakWinDeltas.Count != difficultySettings.StreakLossDeltas.Count || difficultySettings.StreakWinDeltas.Count != difficultySettings.RaceWindow.Count)
				{
					difficultySettings = null;
				}
			}
			if (difficultySettings == null)
			{
				difficultySettings = new DifficultySettings();
				difficultySettings.StreakLossDeltas = new List<float>
				{
					0f,
					0f,
					0f,
					0f,
					0f,
					0f
				};
				difficultySettings.StreakWinDeltas = new List<float>
				{
					0f,
					0f,
					0f,
					0f,
					0f,
					0f
				};
				difficultySettings.RaceWindow = new List<float>
				{
					0.1f,
					0.2f,
					0.3f,
					0.4f,
					0.5f,
					0.6f
				};
				difficultySettings.StreakLossDeltasAdvanced = new List<float>
				{
					-0.3f,
					-0.3f,
					-0.3f,
					-0.1f,
					-0.1f,
					-0.1f
				};
			}
			difficultySettingsInstance = new DifficultySettingsInstance(difficultySettings, eventType);
			difficultyInstanceSettings[eventType] = difficultySettingsInstance;
		}
		return difficultySettingsInstance;
	}

	public static void OnMultiplayerFinishedRace(float raceTime, bool wonRace)
	{
		GetDifficultySettingsInstance(CurrentEventType).OnMultiplayerFinishedRace(raceTime, wonRace);
	}

	public static void OnStreakFinished()
	{
		GetDifficultySettingsInstance(CurrentEventType).OnStreakFinished();
	}

	public static void OnStreakStarted()
	{
		GetDifficultySettingsInstance(CurrentEventType).OnStreakStarted();
		StreakManager.CurrentStreakDifficulty = GetDifficultySettingsInstance(CurrentEventType).Difficulty;
	}

	public static void OnProfileChanged()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile.DifficultyNeedsReset)
		{
			activeProfile.DifficultyNeedsReset = false;
			activeProfile.MultiplayerDifficulty = GetDifficultySettingsInstance(DifficultyEventType.RaceTheWorld).Settings.InitialDifficulty;
			activeProfile.EliteMultiplayerDifficulty = GetDifficultySettingsInstance(DifficultyEventType.EliteClub).Settings.InitialDifficulty;
			activeProfile.MultiplayerEventDifficulty = GetDifficultySettingsInstance(DifficultyEventType.MultiplayerEvent).Settings.InitialDifficulty;
		}
		DifficultyEventType[] array = (DifficultyEventType[])Enum.GetValues(typeof(DifficultyEventType));
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyEventType eventType = array[i];
			GetDifficultySettingsInstance(eventType).OnProfileChanged();
		}
	}

	public static void OnUpgradePurchased()
	{
		DifficultyEventType[] array = (DifficultyEventType[])Enum.GetValues(typeof(DifficultyEventType));
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyEventType eventType = array[i];
			GetDifficultySettingsInstance(eventType).OnUpgradePurchased();
		}
	}

	public static void OnNewSeasonStarted()
	{
		DifficultyEventType[] array = (DifficultyEventType[])Enum.GetValues(typeof(DifficultyEventType));
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyEventType difficultyEventType = array[i];
			if (difficultyEventType != DifficultyEventType.MultiplayerEvent)
			{
				GetDifficultySettingsInstance(difficultyEventType).OnNewSeasonStarted();
			}
		}
	}

	public static void OnNewEventStarted()
	{
		GetDifficultySettingsInstance(DifficultyEventType.MultiplayerEvent).OnNewEventStarted();
	}

	public static void GetDifficulty(DifficultyEventType eventType, out float difficulty, out float perfectTime, out float minTime, out float maxTime, out float perfectTimeAdjust)
	{
		DifficultySettingsInstance difficultySettingsInstance = GetDifficultySettingsInstance(eventType);
		difficulty = difficultySettingsInstance.Difficulty;
		perfectTime = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().TightLoopQuarterMileTime;
		perfectTimeAdjust = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().TightLoopQuarterMileTimeAdjust;
		perfectTimeAdjust = Mathf.Clamp(perfectTimeAdjust, -0.3f, 0f);
		perfectTime += perfectTimeAdjust;
		if (perfectTime == 0f)
		{
			perfectTime = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CurrentPPIndex);
		}
		float num = difficulty;
		if (difficulty > difficultySettingsInstance.Settings.DifficultyClamp)
		{
			difficulty = difficultySettingsInstance.Settings.DifficultyClamp;
			if (difficulty > difficultySettingsInstance.Settings.DifficultyClamp + difficultySettingsInstance.Settings.DifficultyClampRange)
			{
				difficulty -= difficultySettingsInstance.Settings.DifficultyClampRange;
			}
		}
		float num2 = EvaluateVarianceCurve(difficultySettingsInstance.Settings, perfectTime);
		minTime = perfectTime - num * num2;
		maxTime = minTime + num2;
	}

	public static List<float> GetMatches(DifficultyEventType eventType)
	{
		float num;
		float num2;
		float num3;
		float num4;
		float num5;
		GetDifficulty(eventType, out num, out num2, out num3, out num4, out num5);
		DifficultySettings settings = GetDifficultySettingsInstance(eventType).Settings;
		int count = settings.StreakWinDeltas.Count;
		float num6 = Mathf.Abs(num3 - num4);
		float num7 = num3;
		List<float> list = new List<float>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(num7 + num6 * settings.RaceWindow[i]);
		}
		list.Shuffle();
		return list;
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public static void AppendLog(string line, params object[] args)
	{
	}

	private static float EvaluateVarianceCurve(DifficultySettings settings, float time)
	{
		Keyframe[] array = new Keyframe[settings.VarianceCurve.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Keyframe(settings.VarianceCurve[i].Time, settings.VarianceCurve[i].Variance);
		}
		AnimationCurve animationCurve = new AnimationCurve(array);
		return animationCurve.Evaluate(time);
	}
}
