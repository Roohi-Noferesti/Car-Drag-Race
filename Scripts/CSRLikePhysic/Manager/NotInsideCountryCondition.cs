using DataSerialization;

public class NotInsideCountryCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return !BasePlatform.ActivePlatform.InsideCountry;
	}
}
