using System;

public class FriendsFlowConditionalBase : FlowConditionalBase
{
	protected override bool IsConditionalActive()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
	    return false;// activeProfile.FriendRacesPlayed != 0 && activeProfile.FriendRacesWon != 0;
	}
}
