using System;
using System.Collections.Generic;

[Serializable]
public class DifficultySettings
{
	public List<DifficultyKeyframe> VarianceCurve = new List<DifficultyKeyframe>
	{
		new DifficultyKeyframe
		{
			Time = 15.3f
		},
		new DifficultyKeyframe
		{
			Time = 10.6f
		}
	};

	public List<float> StreakWinDeltas = new List<float>
	{
		0.25f,
		0.3f,
		0.35f,
		0.4f,
		0.45f,
		0.5f
	};

	public List<float> StreakLossDeltas = new List<float>
	{
		0f,
		0f,
		0f,
		0f,
		0f,
		0f
	};

	public List<float> StreakLossDeltasAdvanced = new List<float>
	{
		0f,
		0f,
		0f,
		0f,
		0f,
		0f
	};

	public List<float> RaceWindow = new List<float>
	{
		0f,
		0.2f,
		0.4f,
		0.6f,
		0.8f,
		0.98f
	};

	public float FalloffRateDifficultyPerHour = -0.25f;

	public float FalloffIdleTimeHours;

	public float FalloffMaximum = -1.5f;

	public float UpgradePurchaseDelta;

	public float ConsumableLossDelta;

	public float InitialDifficulty = -1f;

	public float SeasonResetDifficulty = -1f;

	public float DifficultyMax = 1f;

	public float DifficultyMin = -1f;

	public float DifficultyClamp = 1f;

	public float DifficultyClampRange = 0.5f;

	public float DifficultyEasyClampPostFirstStreak = -0.5f;

	public float DifficultyRatingExtreme = 0.18f;

	public float DifficultyRatingDifficult = -0.05f;

	public float DifficultyRatingChallenging = -0.22f;

	public float PerfectTimeAdjustmentDifferenceFactor;

	public float AdvancedDifficultyThreshold = 1.5f;

	public float DifficultyDeltaSurpassingLowStreakNumber = 0.3f;

	public int LowStreakNumberRaceCountLimit = 6;

	public int LowStreakNumber = 3;
}
