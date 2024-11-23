using DataSerialization;

public class WorldTourBundlesEligibleCount : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    //int count = GameDatabase.Instance.OfferPacks.GetEligibleOfferPacks().Count;
        //return base.IsInRange(count, details);
	    return false;
	}
}
