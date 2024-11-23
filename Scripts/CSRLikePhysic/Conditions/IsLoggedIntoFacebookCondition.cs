using DataSerialization;

public class IsLoggedIntoFacebookCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//SocialController.Instance.isLoggedIntoFacebook;
	}
}
