using DataSerialization;

public class DeviceOSCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return details.StringValues.Contains(gameState.DeviceOS);
	}
}
