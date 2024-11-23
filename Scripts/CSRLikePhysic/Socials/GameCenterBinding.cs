using System;
using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenterBinding
{
	private const string DLL_ID = "__Internal";

	public static void SetGameCenterDisabled(bool zState)
	{
	}

	private static bool OnDeviceAndEnabled()
	{
		return false;
	}

	public static bool isGameCenterAvailable()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return true;
#endif
		return false;
	}

	public static bool areGameCenterPicsAvailable()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return Social.Active.localUser.image != null;
#endif
		return false;
	}

	public static void authenticateLocalPlayer()
	{
#if UNITY_IOS && !UNITY_EDITOR
		 Social.Active.localUser.Authenticate(status =>
		 {
			 
		 });
#endif
	}

	public static bool isPlayerAuthenticated()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return Social.Active.localUser.authenticated;
#endif
		return false;
	}

	public static string playerAlias()
	{
#if UNITY_IOS && !UNITY_EDITOR
		var alias =  Social.Active.localUser.userName;
		if (string.IsNullOrEmpty(alias))
		{
			return PlayerPrefs.GetString("ios_player_username");
		}
		PlayerPrefs.SetString("ios_player_username", alias);
		Debug.Log("Ios alias : "+alias);
		return alias;
#endif
		return string.Empty;
	}

	public static string playerIdentifier()
	{
		string playerID = null;
		
#if UNITY_ANDROID
		playerID = GooglePlayGamesController.Instance.GetPlayerID();
        if (string.IsNullOrEmpty(playerID))
		{
			return AndroidSpecific.GetLastUsedGPid();
		}
		AndroidSpecific.SetLastUsedGPid(playerID);
#elif UNITY_IOS
		playerID =  Social.Active.localUser.id;
		if (playerID == "0")
		{
			playerID = string.Empty;
		}
		if (string.IsNullOrEmpty(playerID))
		{
			return PlayerPrefs.GetString("ios_playerid");
		}

		PlayerPrefs.SetString("ios_playerid",playerID);
#endif
		return playerID;
	}

	public static bool isUnderage()
	{
#if UNITY_IOS
		return Social.Active.localUser.underage;
#endif
		return false;
	}

	public static void retrieveFriends()
	{
	}

	public static void loadPlayerData(string[] playerIdArray)
	{
	}

	public static void loadLeaderboardCategoryTitles()
	{
	}

	public static void reportScore(long score, string leaderboardId)
	{
	}

	public static void showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope timeScope)
	{
	}

	public static void showLeaderboardWithTimeScopeAndCategory(GameCenterLeaderboardTimeScope timeScope, string categoryId)
	{
	}

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end)
	{
	}

	public static void retrieveScores(bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end, string category)
	{
	}

	public static void retrieveScoresForPlayerId(string playerId)
	{
	}

	public static void retrieveScoresForPlayerId(string playerId, string category)
	{
	}

	public static void reportAchievement(string identifier, float percent)
	{
	}

	public static void getAchievements()
	{
	}

	public static void resetAchievements()
	{
	}

	public static void showAchievements()
	{
	}

	public static void retrieveAchievementMetadata()
	{
	}
}
