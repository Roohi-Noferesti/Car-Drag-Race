public class StreakDifficultyInput : DifficultyModifier
{
	private int numWins;

	private bool hasLost;

	private float streakDifficultyDelta;

	private float difficultyAtStreakStart;

	private int numRaces;

	public StreakDifficultyInput(DifficultySettings settings) : base(settings)
	{
	}

	public override void OnMultiplayerFinishedRace(ref float difficulty, float raceTime, bool wonRace)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		this.hasLost = !wonRace;
		this.numRaces++;
		if (this.numRaces == 1)
		{
			this.difficultyAtStreakStart = difficulty;
		}
		if (wonRace)
		{
			this.numWins++;
		}
		this.numWins = this.SanityCheckData(this.numWins, this.hasLost);
		this.streakDifficultyDelta = ((!this.hasLost) ? base.Settings.StreakWinDeltas[this.numWins - 1] : base.Settings.StreakLossDeltas[this.numWins]);
		if (activeProfile.BestEverMultiplayerWinStreakBanked >= 4)
		{
			if (this.numWins <= base.Settings.LowStreakNumber && wonRace)
			{
				activeProfile.ConsecutiveRacesWonAtLowDifficulty++;
			}
			else
			{
				activeProfile.ConsecutiveRacesWonAtLowDifficulty = 0;
			}
		}
		else
		{
			activeProfile.ConsecutiveRacesWonAtLowDifficulty = 0;
		}
		if (activeProfile.ConsecutiveRacesWonAtLowDifficulty > base.Settings.LowStreakNumberRaceCountLimit)
		{
			this.streakDifficultyDelta = base.Settings.DifficultyDeltaSurpassingLowStreakNumber;
		}
		difficulty = this.difficultyAtStreakStart + this.streakDifficultyDelta;
		CarGarageInstance currentCar = activeProfile.GetCurrentCar();
		if (!RaceEventInfo.Instance.CurrentEvent.IsHalfMile)
		{
			if (raceTime < currentCar.TightLoopQuarterMileTime)
			{
				currentCar.TightLoopQuarterMileTimeAdjust = (raceTime - currentCar.TightLoopQuarterMileTime) * base.Settings.PerfectTimeAdjustmentDifferenceFactor;
			}
			else
			{
				currentCar.TightLoopQuarterMileTimeAdjust = 0f;
			}
		}
		if (!wonRace)
		{
			currentCar.TightLoopQuarterMileTimeAdjust = 0f;
		}
	}

	private int SanityCheckData(int numWins, bool hasLost)
	{
		if (!hasLost && numWins > base.Settings.StreakWinDeltas.Count)
		{
			numWins = base.Settings.StreakWinDeltas.Count;
		}
		else if (hasLost && numWins >= base.Settings.StreakLossDeltas.Count)
		{
			numWins = base.Settings.StreakLossDeltas.Count - 1;
		}
		else if (!hasLost && numWins < 1)
		{
			numWins = 1;
		}
		return numWins;
	}

	public override void OnStreakStarted(ref float difficulty)
	{
		this.streakDifficultyDelta = 0f;
		this.numRaces = 0;
	}

	public override void OnStreakFinished(ref float difficulty)
	{
		if (this.hasLost && difficulty >= base.Settings.AdvancedDifficultyThreshold)
		{
			this.streakDifficultyDelta = base.Settings.StreakLossDeltasAdvanced[this.numWins];
		}
		difficulty = this.difficultyAtStreakStart + this.streakDifficultyDelta;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile.ConsecutiveRacesWonAtLowDifficulty > base.Settings.LowStreakNumberRaceCountLimit)
		{
			activeProfile.ConsecutiveRacesWonAtLowDifficulty = 0;
		}
		this.numWins = 0;
		this.hasLost = false;
	}
}
