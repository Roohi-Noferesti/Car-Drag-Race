using System;

[Serializable]
public class StreakRescueCostData
{
	public enum StreakRescueCostType
	{
		COST_CASH,
		COST_GOLD,
		COST_AD
	}

	public int StreakChainLength;

	public int StreakRaceIndex;

	public string CostTypeID;

	public int CashCost;

	public int GoldCost;

	public int CashCostWithBlogger;

	public int GoldCostWithBlogger;

	public StreakRescueCostType CostTypeEnum
	{
		get
		{
			return EnumHelper.FromString<StreakRescueCostType>(this.CostTypeID);
		}
	}
}
