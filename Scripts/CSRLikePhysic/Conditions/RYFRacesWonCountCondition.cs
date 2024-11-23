using DataSerialization;

public class RYFRacesWonCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("FriendRacesWon"), details);
	}
}
