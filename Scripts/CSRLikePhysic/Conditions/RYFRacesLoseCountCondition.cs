using DataSerialization;

public class RYFRacesLoseCountCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		return base.IsInRange(gameState.GetPlayerProfileInteger("FriendRacesLost"), details);
	}
}
