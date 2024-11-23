using System;

[Serializable]
public class RaceEventTypeMultipliers
{
	public float PerfectStartCashMultiplier;

	public float GoodShiftsCashMultiplier;

	public float OptimalShiftsCashMultiplier;

	public float RaceLoseCashMultiplier;


	public override string ToString()
	{
		return string.Format("PerfectStart: {0:0.00000}, GoodShifts: {1:0.00000}, OptimalShifts: {2:0.00000}, RaceLose: {3:0.00000}", new object[]
		{
			this.PerfectStartCashMultiplier,
			this.GoodShiftsCashMultiplier,
			this.OptimalShiftsCashMultiplier,
			this.RaceLoseCashMultiplier
		});
	}
}

[Serializable]
public class RegulationRewards : RaceEventTypeMultipliers
{
    [Serializable]
    public class RegulationDifficultyReward
    {
        public int EasyReward;
        public int NormalReward;
        public int HardReward;
    }

    public RegulationDifficultyReward Tier1;
    public RegulationDifficultyReward Tier2;
    public RegulationDifficultyReward Tier3;
    public RegulationDifficultyReward Tier4;
    public RegulationDifficultyReward Tier5;
    public RegulationDifficultyReward TierX;
}
