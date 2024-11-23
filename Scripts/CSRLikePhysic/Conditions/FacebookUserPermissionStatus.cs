using DataSerialization;

public class FacebookUserPermissionStatus : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    return false;//SocialController.Instance.FacebookPermissions.Granted(details.StringValues);
	}
}
