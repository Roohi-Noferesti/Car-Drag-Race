using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FriendsDownloadReplayManager : MonoBehaviour
{
	private static FriendsDownloadReplayManager Instance;

	private void Awake()
	{
		FriendsDownloadReplayManager.Instance = this;
	}

	public static void StartFriendDownload(string carDBKey, CachedFriendRaceData inSelectedFriend)
	{
		FriendReplayDatabase.Instance.RequestReplay(carDBKey, inSelectedFriend, new FriendReplayDatabase.ReplayDelegate(FriendsDownloadReplayManager.OnFriendReplayDownloadedSuccessfully));
        //UIManager.instance.blockInput = true;
	}

	public static void OnFriendReplayDownloadedSuccessfully(PlayerReplay replay)
	{
		FriendsDownloadReplayManager.Instance.StartCoroutine(FriendsDownloadReplayManager.Instance.Delay(replay));
	}

	private static void OnErrorPopupDismissed()
	{
	}

	private IEnumerator Delay(PlayerReplay replay)
	{
	    //FriendsDownloadReplayManager.<Delay>c__Iterator11 <Delay>c__Iterator = new FriendsDownloadReplayManager.<Delay>c__Iterator11();
        //<Delay>c__Iterator.replay = replay;
        //<Delay>c__Iterator.<$>replay = replay;
        //return <Delay>c__Iterator;
	    return null;
	}
}
