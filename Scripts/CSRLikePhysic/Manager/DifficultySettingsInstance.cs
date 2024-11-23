using System;
using UnityEngine;

public class DifficultySettingsInstance
{
	private DifficultyEventType eventType;

	private bool hasCompletedRaceInStreak;

	private DifficultyModifier[] inputs;

	public DifficultySettings Settings
	{
		get;
		private set;
	}

	public float Difficulty
	{
		get
		{
			switch (this.eventType)
			{
			case DifficultyEventType.RaceTheWorld:
				return PlayerProfileManager.Instance.ActiveProfile.MultiplayerDifficulty;
			case DifficultyEventType.EliteClub:
				return PlayerProfileManager.Instance.ActiveProfile.EliteMultiplayerDifficulty;
			case DifficultyEventType.MultiplayerEvent:
				return PlayerProfileManager.Instance.ActiveProfile.MultiplayerEventDifficulty;
			default:
				return -1f;
			}
		}
		set
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			float min = this.Settings.DifficultyMin;
			if (activeProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
			{
				min = this.Settings.DifficultyEasyClampPostFirstStreak;
			}
			float num = Mathf.Clamp(value, min, this.Settings.DifficultyMax);
			switch (this.eventType)
			{
			case DifficultyEventType.RaceTheWorld:
				activeProfile.MultiplayerDifficulty = num;
				break;
			case DifficultyEventType.EliteClub:
				activeProfile.EliteMultiplayerDifficulty = num;
				break;
			case DifficultyEventType.MultiplayerEvent:
				activeProfile.MultiplayerEventDifficulty = num;
				break;
			}
		}
	}

	public DifficultySettingsInstance(DifficultySettings settings, DifficultyEventType zEventType)
	{
		this.Settings = settings;
		this.eventType = zEventType;
		switch (this.eventType)
		{
		case DifficultyEventType.RaceTheWorld:
			this.inputs = new DifficultyModifier[]
			{
				new StreakDifficultyInput(settings),
				new FalloffDifficultyInput(settings, this.eventType),
				new PurchaseUpgradeInput(settings),
				new TutorialInput(settings),
				new ConsumableExpiredInput(settings)
			};
			break;
		case DifficultyEventType.EliteClub:
			this.inputs = new DifficultyModifier[]
			{
				new StreakDifficultyInput(settings),
				new FalloffDifficultyInput(settings, this.eventType),
				new PurchaseUpgradeInput(settings),
				new ConsumableExpiredInput(settings)
			};
			break;
		case DifficultyEventType.MultiplayerEvent:
			this.inputs = new DifficultyModifier[]
			{
				new StreakDifficultyInput(settings),
				new FalloffDifficultyInput(settings, this.eventType),
				new PurchaseUpgradeInput(settings),
				new ConsumableExpiredInput(settings)
			};
			break;
		}
	}

	public void OnMultiplayerFinishedRace(float raceTime, bool wonRace)
	{
		this.hasCompletedRaceInStreak = true;
		float difficulty = this.Difficulty;
		DifficultyModifier[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyModifier difficultyModifier = array[i];
			difficultyModifier.OnMultiplayerFinishedRace(ref difficulty, raceTime, wonRace);
		}
		this.Difficulty = difficulty;
	}

	public void OnStreakFinished()
	{
		if (this.hasCompletedRaceInStreak)
		{
			this.hasCompletedRaceInStreak = false;
			float difficulty = this.Difficulty;
			DifficultyModifier[] array = this.inputs;
			for (int i = 0; i < array.Length; i++)
			{
				DifficultyModifier difficultyModifier = array[i];
				difficultyModifier.OnStreakFinished(ref difficulty);
			}
			this.Difficulty = difficulty;
		}
	}

	public void OnStreakStarted()
	{
		float difficulty = this.Difficulty;
		DifficultyModifier[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyModifier difficultyModifier = array[i];
			difficultyModifier.OnStreakStarted(ref difficulty);
		}
		this.Difficulty = difficulty;
	}

	public void OnProfileChanged()
	{
		float difficulty = this.Difficulty;
		DifficultyModifier[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyModifier difficultyModifier = array[i];
			difficultyModifier.OnProfileChanged(ref difficulty);
		}
		this.Difficulty = difficulty;
	}

	public void OnUpgradePurchased()
	{
		float difficulty = this.Difficulty;
		DifficultyModifier[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			DifficultyModifier difficultyModifier = array[i];
			difficultyModifier.OnUpgradePurchased(ref difficulty);
		}
		this.Difficulty = difficulty;
	}

	public void OnNewSeasonStarted()
	{
		this.Difficulty = Mathf.Min(this.Difficulty, this.Settings.SeasonResetDifficulty);
	}

	public void OnNewEventStarted()
	{
		this.Difficulty = this.Settings.SeasonResetDifficulty;
	}
}
