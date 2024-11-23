using DataSerialization;

public class RYFStarTotalCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;// base.IsInRange(StarsManager.GetStarStatsForAllTiers().TotalStars, details);
	}
}
