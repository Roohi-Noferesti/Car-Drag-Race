using System;

public static class NativeEvents
{
    public static event NativeEvents_Delegate ApplicationDidRecieveMemoryWarningEvent;

    public static event NativeEvents_Delegate ApplicationDidFinishCopyingSnapshotToCameraRollEvent;

    public static event NativeEvents_Delegate ApplicationDidFailCopyingSnapshotToCameraRollEvent;

    public static event NativeEvents_Delegate ApplicationDidFinishWithTweetComposerEvent;

    public static event NativeEvents_DelegateToken fbDidLoginEvent;

    public static event NativeEvents_Delegate fbDidLogoutEvent;

    public static event NativeEvents_DelegateInt fbDidNotLoginEvent;

    public static event NativeEvents_DelegateString fbGotFriendsEvent;

    public static event NativeEvents_DelegateString fbGotFriendInfoEvent;

    public static event NativeEvents_DelegateString fbGotUserInfoEvent;

    public static event NativeEvents_DelegateString fbGotTokenForBusinessEvent;

    public static event NativeEvents_DelegateString2 fbGotFriendProfilePicEvent;

    public static event NativeEvents_DelegateString fbGotFriendProfilePicFailedEvent;

    public static event NativeEvents_DelegateString fbGotUserPermissionsEvent;

    public static event NativeEvents_DelegateInt fbRequestFailedEvent;

    public static event NativeEvents_DelegateInt fbAppRequestDialogueFailedEvent;

    public static event NativeEvents_Delegate fbAppRequestDialogueCancelledEvent;

    public static event NativeEvents_Delegate fbAppRequestDialogueSucceededEvent;

    public static event NativeEvents_DelegateInt fbPostDialogueFailedEvent;

    public static event NativeEvents_Delegate fbPostDialogueCancelledEvent;

    public static event NativeEvents_Delegate fbPostDialogueSucceededEvent;

    public static event NativeEvents_DelegateInt fbInviteFriendsSucceededEvent;

    public static event NativeEvents_DelegateBool fbRequestFriendsPermissionEvent;

    public static event NativeEvents_DelegateBool fbRequestPermissionEvent;

    public static event NativeEvents_DelegateBool twitterUserIsFollowingUsEvent;

    public static event NativeEvents_Delegate ChartBoostAdDismissedCallbackEvent;

    public static event NativeEvents_Delegate PlayHavenAdDismissedCallbackEvent;

    public static event NativeEvents_DelegateString FlurryAdSucceededCallbackEvent;

    public static event NativeEvents_DelegateString FlurryAdFailedCallbackEvent;
    public static event NativeEvents_DelegateUri urlOpenedEvent;

	public static void urlOpened(string url)
	{
		if (urlOpenedEvent != null)
		{
			urlOpenedEvent(new Uri(url));
		}
	}

	public static void fbDidLogin(string accessToken, string expiryToken)
	{
		if (fbDidLoginEvent != null)
		{
			fbDidLoginEvent(accessToken, expiryToken);
		}
		int gold = 1;
        //if (GameDatabase.Instance.SocialConfiguration != null)
        //{
        //    gold = GameDatabase.Instance.SocialConfiguration.FacebookSSOReward;
        //}
		if (PlayerProfileManager.Instance.ActiveProfile.IsFacebookSSORewardAllowed)
		{
			PlayerProfileManager.Instance.ActiveProfile.AddGold(gold,"reward", "facebook");
			PlayerProfileManager.Instance.ActiveProfile.IsFacebookSSORewardAllowed = false;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public static void fbDidLogout()
	{
		if (fbDidLogoutEvent != null)
		{
			fbDidLogoutEvent();
		}
	}

	public static void fbDidNotLogin(int errorCode)
	{
		if (fbDidNotLoginEvent != null)
		{
			fbDidNotLoginEvent(errorCode);
		}
	}

	public static void fbGotFriends(string friendsJSON)
	{
		if (fbGotFriendsEvent != null)
		{
			fbGotFriendsEvent(friendsJSON);
		}
	}

	public static void fbGotFriendInfo(string friendInfoJSON)
	{
		if (fbGotFriendInfoEvent != null)
		{
			fbGotFriendInfoEvent(friendInfoJSON);
		}
	}

	public static void fbGotUserInfo(string userInfoJSON)
	{
		if (fbGotUserInfoEvent != null)
		{
			fbGotUserInfoEvent(userInfoJSON);
		}
	}

	public static void fbGotTokenForBusiness(string tfbJSON)
	{
		if (fbGotTokenForBusinessEvent != null)
		{
			fbGotTokenForBusinessEvent(tfbJSON);
		}
	}

	public static void fbGetFriendProfilePic(string friendID, string friendPicJSON)
	{
		if (fbGotFriendProfilePicEvent != null)
		{
			fbGotFriendProfilePicEvent(friendID, friendPicJSON);
		}
	}

	public static void fbGetFriendProfilePicFailed(string friendID)
	{
		if (fbGotFriendProfilePicEvent != null)
		{
			fbGotFriendProfilePicFailedEvent(friendID);
		}
	}

	public static void fbGotUserPermissions(string userPermissionsJSON)
	{
		if (fbGotUserPermissionsEvent != null)
		{
			fbGotUserPermissionsEvent(userPermissionsJSON);
		}
	}

	public static void fbRequestFailed(int errorCode)
	{
		if (fbRequestFailedEvent != null)
		{
			fbRequestFailedEvent(errorCode);
		}
	}

	public static void fbAppRequestDialogueCancelled()
	{
		if (fbAppRequestDialogueCancelledEvent != null)
		{
			fbAppRequestDialogueCancelledEvent();
		}
	}

	public static void fbAppRequestDialogueSucceeded()
	{
		if (fbAppRequestDialogueSucceededEvent != null)
		{
			fbAppRequestDialogueSucceededEvent();
		}
	}

	public static void fbAppRequestDialogueFailed(int errorCode)
	{
		if (fbAppRequestDialogueFailedEvent != null)
		{
			fbAppRequestDialogueFailedEvent(errorCode);
		}
	}

	public static void fbPostDialogueCancelled()
	{
		if (fbPostDialogueCancelledEvent != null)
		{
			fbPostDialogueCancelledEvent();
		}
	}

	public static void fbPostDialogueSucceeded()
	{
		if (fbPostDialogueSucceededEvent != null)
		{
			fbPostDialogueSucceededEvent();
		}
	}

	public static void fbPostDialogueFailed(int errorCode)
	{
		if (fbPostDialogueFailedEvent != null)
		{
			fbPostDialogueFailedEvent(errorCode);
		}
	}

	public static void fbInviteFriendsSucceeded(int count)
	{
		if (fbInviteFriendsSucceededEvent != null)
		{
			fbInviteFriendsSucceededEvent(count);
		}
	}

	public static void fbRequestFriendsPermission(bool approved)
	{
		if (fbRequestFriendsPermissionEvent != null)
		{
			fbRequestFriendsPermissionEvent(approved);
		}
	}

	public static void fbRequestPermission(bool approved)
	{
		if (fbRequestPermissionEvent != null)
		{
			fbRequestPermissionEvent(approved);
		}
	}

	public static void twitterUserIsFollowingUs(bool isFollowing)
	{
		if (twitterUserIsFollowingUsEvent != null)
		{
			twitterUserIsFollowingUsEvent(isFollowing);
		}
	}

	public static void ApplicationDidRecieveMemoryWarning()
	{
		if (ApplicationDidRecieveMemoryWarningEvent != null)
		{
			ApplicationDidRecieveMemoryWarningEvent();
		}
	}

	public static void ApplicationDidFinishCopyingSnapshotToCameraRoll()
	{
		if (ApplicationDidFinishCopyingSnapshotToCameraRollEvent != null)
		{
			ApplicationDidFinishCopyingSnapshotToCameraRollEvent();
		}
	}

	public static void ApplicationDidFailCopyingSnapshotToCameraRoll()
	{
		if (ApplicationDidFailCopyingSnapshotToCameraRollEvent != null)
		{
			ApplicationDidFailCopyingSnapshotToCameraRollEvent();
		}
	}

	public static void ApplicationDidFinishWithTweetComposer()
	{
		if (ApplicationDidFinishWithTweetComposerEvent != null)
		{
			ApplicationDidFinishWithTweetComposerEvent();
		}
	}

	public static void ChartBoostAdDismissedCallback()
	{
		if (ChartBoostAdDismissedCallbackEvent != null)
		{
			ChartBoostAdDismissedCallbackEvent();
		}
	}

	public static void PlayHavenAdDismissedCallback()
	{
		if (PlayHavenAdDismissedCallbackEvent != null)
		{
			PlayHavenAdDismissedCallbackEvent();
		}
	}

	public static void FlurryAdSucceededCallback(string adSpace)
	{
		if (FlurryAdSucceededCallbackEvent != null)
		{
			FlurryAdSucceededCallbackEvent(adSpace);
		}
	}

	public static void FlurryAdFailedCallback(string adSpace)
	{
		if (FlurryAdFailedCallbackEvent != null)
		{
			FlurryAdFailedCallbackEvent(adSpace);
		}
	}
}
