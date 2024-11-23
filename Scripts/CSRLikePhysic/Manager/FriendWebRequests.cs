using System;
using System.Collections.Generic;
using System.Globalization;

public static class FriendWebRequests
{
	public const string RequestPrefix = "ryf_";

	private const string syncFriendDataRequest = "ryf_sync_friend_data";

	private const string resultRequest = "ryf_result";

	private const string getRaceDataRequest = "ryf_get_race_data";

	private const string updateFriendNetworkRequest = "ryf_update_friend_network";

	private const string sendPushMessageRequest = "ryf_messaging";

	public const string TokenName = "ryft";

	public static string SyncFriendDataRequest
	{
		get
		{
			return "ryf_sync_friend_data";
		}
	}

	public static void UpdateFriendNetwork(string serviceID, List<string> friendServiceIDs, string networkPrefix, WebClientDelegate2 callback = null)
	{
		int userID = UserManager.Instance.currentAccount.UserID;
		string rYFToken = UserManager.Instance.currentAccount.RYFToken;
		bool firstTimeFriendsUser = PlayerProfileManager.Instance.ActiveProfile.FirstTimeFriendsUser;
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("uid", userID.ToString());
		jsonDict.Set("sid", serviceID);
		jsonDict.Set("friend_sids", JsonConverter.SerializeObject(friendServiceIDs, true));
		jsonDict.Set("ryft", rYFToken);
		jsonDict.Set("type", networkPrefix);
		jsonDict.Set("first_time_user", firstTimeFriendsUser.ToString());
		WebRequestQueueRTW.Instance.StartCall("ryf_update_friend_network", "ryf_update_friend_network", jsonDict, callback, userID, string.Empty, 6);
	}

	public static void Result(PlayerReplay replay, WebClientDelegate2 replayCallback)
	{
		string rYFToken = UserManager.Instance.currentAccount.RYFToken;
		JsonDict jsonDict = new JsonDict();
		RacePlayerInfoComponent component = replay.playerInfo.GetComponent<RacePlayerInfoComponent>();
		jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
		jsonDict.Set("ryft", rYFToken);
		jsonDict.Set("car_id", component.CarDBKey);
		jsonDict.Set("replay_data", replay.ToJson());
		jsonDict.Set("result_time", replay.replayData.finishTime.ToString(CultureInfo.InvariantCulture));
		WebRequestQueueRTW.Instance.StartCall("ryf_result", "ryf_result", jsonDict, replayCallback, UserManager.Instance.currentAccount.UserID, string.Empty, 5);
	}

	private static void SetMessageArgument(KeyValuePair<string, bool> kvp, ref JsonDict jsonDict)
	{
		jsonDict.Set("text", kvp.Key);
		jsonDict.Set("translate", kvp.Value);
	}

	public static void QueuePushNotification(int fromPlayer, List<int> toPlayers, string tag, List<KeyValuePair<string, bool>> args)
	{
		if (RYFStatusManager.Instance.PushMessagesAvailable && toPlayers.Count > 0)
		{
			string rYFToken = UserManager.Instance.currentAccount.RYFToken;
			JsonDict jsonDict = new JsonDict();
			jsonDict.Set("ryft", rYFToken);
			jsonDict.Set("uid", fromPlayer.ToString());
			jsonDict.Set("guid", Guid.NewGuid().ToString());
			JsonDict jsonDict2 = new JsonDict();
			jsonDict2.Set("target_user_ids", toPlayers);
			jsonDict2.Set("message", tag);
			if (args != null)
			{
				jsonDict2.SetObjectList<KeyValuePair<string, bool>>("message_args", args, new SetObjectDelegate<KeyValuePair<string, bool>>(FriendWebRequests.SetMessageArgument));
			}
			jsonDict.Set("payload", jsonDict2.ToString());
			WebRequestQueueRTW.Instance.StartCall("ryf_messaging", "ryf_messaging", jsonDict, null, null, string.Empty, 5);
		}
	}

	public static void SyncFriendData(WebClientDelegate2 callback)
	{
		string rYFToken = UserManager.Instance.currentAccount.RYFToken;
		int userID = UserManager.Instance.currentAccount.UserID;
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("uid", userID.ToString());
		jsonDict.Set("ryft", rYFToken);
		jsonDict.Set("cache_update_time", RYFStatusManager.Instance.UnixTimeStampOfCachedData);
		WebRequestQueueRTW.Instance.StartCall("ryf_sync_friend_data", "ryf_sync_friend_data", jsonDict, callback, userID, string.Empty, 5);
	}

	public static void GetRaceData(int userID, string carDbKey, WebClientDelegate2 replayCallback)
	{
		string rYFToken = UserManager.Instance.currentAccount.RYFToken;
		int userID2 = UserManager.Instance.currentAccount.UserID;
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("uid", userID2.ToString());
		jsonDict.Set("ryft", rYFToken);
		jsonDict.Set("target_car_id", carDbKey.ToString());
		jsonDict.Set("target_user_id", userID.ToString());
		WebRequestStandalone.Instance.StartCall("ryf_get_race_data", "ryf_get_race_data", jsonDict, replayCallback, userID2, string.Empty);
	}
}
