using DataSerialization;

public class IsMultiplayerUnlockedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//MultiplayerUtils.IsMultiplayerUnlocked();
	}
}
