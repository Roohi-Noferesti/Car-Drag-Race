using DataSerialization;

public class RYFStarTypeCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//base.IsInRange(StarsManager.GetStarStatsForAllTiers().NumStars[(StarType)details.IntValue], details);
	}
}
