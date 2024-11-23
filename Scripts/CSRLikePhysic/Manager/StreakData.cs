using System;
using System.Collections.Generic;

[Serializable]
public class StreakData
{
	public List<int> PlayerListRefreshCashCosts = new List<int>();

    //public List<TierBonusMultiplier> TierBonusMultipliers = new List<TierBonusMultiplier>();

	public List<StreakPrize> StreakPrizes = new List<StreakPrize>();

    public ModeInfo RaceTheWorldInfo;

    public ModeInfo EliteClubInfo;

    public ModeInfo EventInfo;

	public StreakRescueData StreakRescue;

	public void Initialise()
	{
        //this.RaceTheWorldInfo.Initialise();
        //this.EliteClubInfo.Initialise();
        //this.EventInfo.Initialise();
	}
}
