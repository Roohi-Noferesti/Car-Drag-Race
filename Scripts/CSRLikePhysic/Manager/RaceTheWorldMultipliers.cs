using System;

[Serializable]
public class RaceTheWorldMultipliers
{
	public int BaseRewardT1 = 100;

	public int BaseRewardT2 = 350;

	public int BaseRewardT3 = 800;

	public int BaseRewardT4 = 1200;

	public int BaseRewardT5 = 2200;

	public float ScaleForPP = 0.05f;

	public float LoseMultiplier = 0.01f;

	public float CashMultiplier = 0.08f;

	public float XPMultiplier = 0.08f;

	public float PerfectStartScale = 0.03f;

	public float PerfectShiftScale = 0.03f;

	public float RaceStage_Opponent = 1f;

	public float RaceStage_Grudge = 1.5f;

	public float RaceStage_Bounty = 1.4f;

	public float RaceStage_Rival = 1.1f;

	public float LeadTimeBonusPerSecond = 20f;

	public float EliteBonus = 0.8f;

	public float PRAgentBonus = 0.5f;
}
