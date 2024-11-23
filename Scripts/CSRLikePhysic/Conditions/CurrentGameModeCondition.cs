using DataSerialization;

public class CurrentGameModeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        return true;//gameState.GetCurrentGameMode().Equals(details.GameMode);
	}
}
