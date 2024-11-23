using DataSerialization;

public class InsideCountryCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return BasePlatform.ActivePlatform.InsideCountry;
	}
}
