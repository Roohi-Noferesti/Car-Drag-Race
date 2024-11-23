using System;
using UnityEngine;

[Serializable]
public class XPEventsConfiguration:ScriptableObject
{
#if UNITY_EDITOR
    public bool DebugLevels;
#endif

	public int XPForRaceEvent;

	public int XPForCrewMember;

	public int XPForCrewBoss;

	public int XPForPurchase;

	public int XPForEngineUpgrade;

	public int XPStageEngineModifier;

	public int XPForTurboUpgrade;

	public int XPStageTurboModifier;

	public int XPForIntakeUpgrade;

	public int XPStageIntakeModifier;

	public int XPForNitrousUpgrade;

	public int XPStageNitrousModifier;

	public int XPForBodyUpgrade;

	public int XPStageBodyModifier;

	public int XPForTyresUpgrade;

	public int XPStageTyresModifier;

	public int XPForTransUpgrade;

	public int XPStageTransModifier;

	public int XPForAchievement;

	public int XPForEvent;

	public int XPLossModifier;

	public int XPTierModifier;

	public int LevelUpBaseXP;

	public int LevelUpIncrementPercent;

	public int LevelUpBaseGold;

	public int LevelUpIncrementPercentGold;
}
