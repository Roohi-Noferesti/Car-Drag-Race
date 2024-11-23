using DataSerialization;

public class CarePackagesReceivedCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    //string stringValue = details.StringValue;
        //int value = GameDatabase.Instance.CarePackages.TotalReceivedCount(stringValue);
        //return base.IsInRange(value, details);
	    return false;
	}
}
