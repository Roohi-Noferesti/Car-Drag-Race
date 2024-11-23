using DataSerialization;

public class RacesEnteredCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("RacesEntered"), details);
	}
}
