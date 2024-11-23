using System;
using System.Collections.Generic;

[Serializable]
public class RewardsMultipliers
{
    public float RaceRewardPower = 1.2F;
	public int BaseLiveryWinBonus;

	public float LiveryWinGoldMultiplier;

	public float LiveryWinTier1Multiplier;

	public float LiveryWinTier2Multiplier;

	public float LiveryWinTier3Multiplier;

	public float LiveryWinTier4Multiplier;

    public float LiveryWinTier5Multiplier;

    public RegulationRewards RegulationRaceMultipliers;

	public RaceEventTypeMultipliers LadderRaceMultipliers;

	public RaceEventTypeMultipliers CrewBattleRaceMultipliers;

	public RaceEventTypeMultipliers RestrictionRaceMultipliers;

	public RaceEventTypeMultipliers CarSpecificRaceMultipliers;

	public RaceEventTypeMultipliers ManufacturerSpecificRaceMultipliers;

	public RaceEventTypeMultipliers RaceWithFriendsRaceMultipliers;

	public RaceEventTypeMultipliers DefaultWorldTourRaceMultipliers;

    public RaceEventTypeMultipliers TournamentRaceMultipliers;

	public Dictionary<string, RaceEventTypeMultipliers> WorldTourRaceMultipliers = new Dictionary<string, RaceEventTypeMultipliers>();

	public DailyBattleMultipliers DailyBattleMultipliers;

	public RaceEventTypeMultipliers FinalCrewBattleMultipliers;

	public RaceTheWorldMultipliers RaceTheWorldMultipliers;

	public int LiveryBonusTypeDateBoundary;

    //public LevelBonus RegulationRaceLevelBonus;

    public LeagueBonus LeagueBonus;

}
