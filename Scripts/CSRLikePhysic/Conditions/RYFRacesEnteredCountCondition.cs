using DataSerialization;

public class RYFRacesEnteredCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("FriendRacesPlayed"), details);
	}
}
