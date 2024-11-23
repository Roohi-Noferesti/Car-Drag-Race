using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class OnlineConfiguration:ScriptableObject
{
	public int PerformanceIncreasePercent = 20;

	public int SportsPackLiveryOnlyCost;

	public List<int> SportsPackTierBaseGoldCosts = new List<int>
	{
		5,
		10,
		15,
		20,
		25
	};

    public float LeaderboardTimeout = 500;

	public PrizeOMaticData PrizeoMatic;
	public AppTuttiTimedRewardData AppTuttiTimedReward;
	public VasTimedRewardData VasTimedReward;

	public bool ExpressUpgradeGoldOnly = true;

	public float ExpressUpgradeCashToGold = 0.01f;

	public float ExpressGoldMultiplier = 1.5f;

	public RaceTeamPrizeData RaceTeamPrizeData;

	public StreakData StreakData;

	public string NetworkReplayVersion = "1.4";

    public Dictionary<string, ColourSwatch> Swatches = new Dictionary<string, ColourSwatch>();

	public List<EligibilityCondition> IsMultiplayerDisabled = new List<EligibilityCondition>();

    public int ServerTimeMaxHourDifference = 24;

    public int StreakCount = 15;

    public float MinSearchTime = 3;

    public float MaxSearchTime = 6;

    public OnlineRace[] StakeRaces;

    public AnimationCurve WinLoseDifficultyModifier;

    public SMPWinStreakConfiguration SMPWinStreak;

    public AnimationCurve FakeCountdownDelayCurve;

    public int SMPSessionHoursToReset = 6;

    public bool MultiplayerEnabled
	{
		get
		{
			IGameState gameState = new GameStateFacade();
			foreach (EligibilityCondition current in this.IsMultiplayerDisabled.Where(c=>c.IsActive))
			{
				if (!current.IsValid(gameState))
				{
					return true;
				}
			}
			return false;
		}
	}

	public void Initialise()
	{
		this.StreakData.Initialise();
	}

	public bool IsStreakRescueActive()
	{
		return this.StreakData.StreakRescue.IsActive;
	}
}
