using System;
using System.Diagnostics;
using UnityEngine;

public class SocialGamePlatformSelector : MonoBehaviour
{
	public class AchievementData
	{
		public Achievement achievement;

		public AchievementStatus status;
	}

	public enum AchievementStatus
	{
		locked,
		requested,
		unlocked
	}

	public enum ScoreStatus
	{
		local,
		posted
	}

	public class ScoreData
	{
		public int score;

		public Leaderboard leaderboard;

		public ScoreStatus status;
	}

	private const int numberOfRaceCategories = 10;

	public const int MAXCANCELLATIONS = 1;

	public int baseTimeScore;

	public static SocialGamePlatformSelector Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		#if UNITY_ANDROID
if (AndroidSpecific.GetGPGSignInCount() < 1)
		{
			AuthenticateIfConnectionAvailable();
		}
#elif UNITY_IOS
	    AuthenticateIfConnectionAvailable();
#endif
		baseTimeScore = ConvertTimeForGamePlatform(360f);
		AssetSystemManager.JustKicked += StartGamePlatformWhenSafe;
	}

	private void OnDestroy()
	{
		AssetSystemManager.JustKicked -= StartGamePlatformWhenSafe;
	}

	public void StartGamePlatformWhenSafe(AssetSystemManager.Reason reason)
	{
		if (PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations < 1)
		{
			AuthenticateIfConnectionAvailable();
		}
	}

	private void AuthenticateIfConnectionAvailable()
	{


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }
#if UNITY_ANDROID
        if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
        {
            return;
        }

        //Disable automatic login when privacy policy not accepted yet
        if (!AgeVerificationManager.Instance.HasPrivacyPolicyVerified)
        {
            return;
        }

        GooglePlayGamesController.Instance.AuthenticatePlayer(null, false);
#elif UNITY_IOS
	    GameCenterBinding.authenticateLocalPlayer();
#endif
	}

	public void AuthenticatePlayer()
	{
#if UNITY_ANDROID
        GooglePlayGamesController.Instance.AuthenticatePlayer(null, false);
#elif UNITY_IOS
	    GameCenterBinding.authenticateLocalPlayer();
#endif
    }

    public void ShowAchievements()
	{
#if UNITY_ANDROID
        GooglePlayGamesController.Instance.ShowAchievements();
#endif

    }

    public void ReportAchievement(Achievement achievement)
	{
		ReportAchievement(achievement, true);
	}

	public void ReportAchievement(Achievement achievement, bool display)
	{
#if UNITY_ANDROID
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
        GooglePlayGamesController.Instance.ReportAchievement(achievement);
#endif
	}

	public void ResetAchievements()
	{
	    if (PlayerProfileManager.Instance.ActiveProfile == null)
	        return;
		PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Clear();
		foreach (Achievement current in Achievements.All)
		{
			AchievementData achievementData = new AchievementData();
			achievementData.achievement = current;
			achievementData.status = AchievementStatus.locked;
			PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Add(achievementData);
		}
	}

	public void ShowLeaderboard(Leaderboard leaderboard = null)
	{
		if (leaderboard != null)
		{
#if UNITY_ANDROID
            GooglePlayGamesController.Instance.ShowLeaderboard(leaderboard);
#endif
		}
	}

	public void ReportScore(int score, Leaderboard leaderboard)
	{
#if UNITY_ANDROID
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
        GooglePlayGamesController.Instance.ReportScore(score, leaderboard);
#endif
	}

	public void ResetScores()
	{
	    if (PlayerProfileManager.Instance.ActiveProfile == null)
	        return;
		PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Clear();
		foreach (Leaderboard current in Leaderboards.All)
		{
			ScoreData scoreData = new ScoreData();
			if (current.IsStandardRace)
			{
				scoreData.score = baseTimeScore;
			}
			else if (current == Leaderboards.OverallTime)
			{
				scoreData.score = baseTimeScore * 10;
			}
			else
			{
				scoreData.score = 0;
			}
			scoreData.leaderboard = current;
			scoreData.status = ScoreStatus.local;
			PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Add(scoreData);
		}
	}

	public void SetGamePlatformDisabled(bool zState)
	{
#if UNITY_ANDROID
        GooglePlayGamesController.Instance.SetGooglePlayGamesDisabled(zState);
#endif
	}

	public bool IsGamePlatformDisabled()
	{
#if UNITY_ANDROID
        return GooglePlayGamesController.Instance.IsGooglePlayGamesDisabled();
#endif
	    return false;
	}

    public string GetCurrentAlias()
    {
#if UNITY_ANDROID
        return GooglePlayGamesController.Instance.GetCurrentAlias();
#elif UNITY_IOS
	    return GameCenterBinding.playerAlias();
#endif
        return null;
    }

    public string GetPlayerID()
    {
#if UNITY_ANDROID
        return GooglePlayGamesController.Instance.GetPlayerID();
#elif UNITY_IOS
        return GameCenterBinding.playerIdentifier();
#endif
        return null;
    }

    public bool IsPlayerLoggedIn()
    {
#if UNITY_ANDROID
        return GooglePlayGamesController.Instance.IsPlayerAuthenticated();
#elif UNITY_IOS
	    return GameCenterBinding.isPlayerAuthenticated();
#endif
        return false;
    }

    public bool IsPlayerLoggedInAndSocialGamePlatformPicsAvailable()
    {
#if UNITY_ANDROID
        return GooglePlayGamesController.Instance.IsPlayerAuthenticated();
#elif UNITY_IOS
	    return GameCenterBinding.isPlayerAuthenticated();
#endif
        return false;
    }

    public int GetNewOverallTime()
	{
		float num = 0f;
		foreach (ScoreData current in PlayerProfileManager.Instance.ActiveProfile.PlayerScores)
		{
			if (current.leaderboard.IsStandardRace)
			{
				num += ConvertTimeFromGamePlatform(current.score);
			}
		}
		return ConvertTimeForGooglePlayGames(num);
	}

	public int ConvertTimeForGamePlatform(float raceTime)
	{
		return ConvertTimeForGooglePlayGames(raceTime);
	}

	public float ConvertTimeFromGamePlatform(int raceTime)
	{
		return ConvertTimeFromGooglePlayGames(raceTime);
	}

	public int ConvertRaceTime_ToHundredths(float raceTime)
	{
		return ConvertTimeForGameCenter(raceTime);
	}

	private int ConvertTimeForGooglePlayGames(float raceTime)
	{
		return (int)TimeSpan.FromSeconds(Math.Round(raceTime, 3)).TotalMilliseconds;
	}

	private float ConvertTimeFromGooglePlayGames(int raceTime)
	{
		int num = raceTime / 60000;
		int num2 = (raceTime - num * 60000) / 1000;
		int num3 = (raceTime - num * 60000 - num2 * 1000) / 10;
		return num * 60 + num2 + num3 / 100f;
	}

	public int ConvertTimeForGameCenter(float RaceTime)
	{
		return (int)TimeSpan.FromSeconds(Math.Round(RaceTime, 2)).TotalMilliseconds / 10;
	}

	private float ConvertTimeFromGameCenter(int RaceTime)
	{
		int num = RaceTime / 6000;
		int num2 = (RaceTime - num * 6000) / 100;
		int num3 = RaceTime - num * 6000 - num2 * 100;
		return num * 60 + num2 + num3 / 100f;
	}

	private void ShowPopup(string info)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_GAMECENTER_TITLE",
			BodyText = info,
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	private void Log(string message)
	{
	}
}
