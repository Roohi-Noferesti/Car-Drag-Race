using GooglePlayGames;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using I2.Loc;
using UnityEngine;

#if UNITY_ANDROID
public class GooglePlayGamesController : MonoBehaviour
{
	private Dictionary<string, int> validFinishTimes;

	private bool googlePlayGamesIsDisabled;

	private bool achievementIDsMapped;

	public bool playerInitiatedSignIn;

    public static event GameCenter2StringEventHandler PlayerPhotoLoadFailed;

    public static event GameCenter2StringEventHandler PlayerPhotoLoaded;

	public static GooglePlayGamesController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (GooglePlayGamesController.Instance != null)
		{
			return;
		}
		GooglePlayGamesController.Instance = this;
        //PlayGamesPlatform.DebugLogEnabled = true;
        
        if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
	        return;
        
        PlayGamesPlatform.Activate();
		this.validFinishTimes = new Dictionary<string, int>();
		this.validFinishTimes["grp.CSR_LDR_CLASS_D_QUARTER"] = 12500;
		this.validFinishTimes["grp.CSR_LDR_CLASS_D_HALF"] = 20500;
		this.validFinishTimes["grp.CSR_LDR_CLASS_C_QUARTER"] = 11000;
		this.validFinishTimes["grp.CSR_LDR_CLASS_C_HALF"] = 19500;
		this.validFinishTimes["grp.CSR_LDR_CLASS_B_QUARTER"] = 9500;
		this.validFinishTimes["grp.CSR_LDR_CLASS_B_HALF"] = 16800;
		this.validFinishTimes["grp.CSR_LDR_CLASS_A_QUARTER"] = 8000;
		this.validFinishTimes["grp.CSR_LDR_CLASS_A_HALF"] = 14400;
		this.validFinishTimes["grp.CSR_LDR_CLASS_S_QUARTER"] = 6500;
		this.validFinishTimes["grp.CSR_LDR_CLASS_S_HALF"] = 11500;
	}

	public void AuthenticatePlayer(Action<bool> callback = null, bool silent = false)
	{
		if (callback == null)
		{
			callback = new Action<bool>(this.Callback_AuthenticatePlayer);
		}
		if (this.IsPlayerAuthenticated())
		{
			return;
		}
		if (silent)
		{
			PlayGamesPlatform.Instance.Authenticate(callback, true);
		}
		else
		{
			PlayGamesPlatform.Instance.Authenticate(callback);
		}
	}

	private void Callback_AuthenticatePlayer(bool success)
	{
		if (UserManager.Instance == null && Z2HInitialisers.managerGo.GetComponent<UserManager>() == null)
		{
			Z2HInitialisers.managerGo.AddComponent<UserManager>();
		}
		UserManager.Instance.StartConnect(true);
		if (!success)
		{
			PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations++;
		}
		else
		{
			this.InitialLogin();
		}
	}

	public void InitialLogin()
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		if (!this.achievementIDsMapped)
		{
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.FirstAchievement.CategoryIDName, GPGSIds.achievement_lets_go);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.HalfTier1.CategoryIDName, GPGSIds.achievement_halfway_class_1);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.AllTier1.CategoryIDName, GPGSIds.achievement_class_1_passed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.HalfTier2.CategoryIDName, GPGSIds.achievement_halfway_class_2);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.AllTier2.CategoryIDName, GPGSIds.achievement_class_2_passed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.HalfTier3.CategoryIDName, GPGSIds.achievement_halfway_class_3);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.AllTier3.CategoryIDName, GPGSIds.achievement_class_3_passed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.HalfTier4.CategoryIDName, GPGSIds.achievement_halfway_class_4);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.AllTier4.CategoryIDName, GPGSIds.achievement_class_4_passed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.HalfTier5.CategoryIDName, GPGSIds.achievement_halfway_class_5);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.AllTier5.CategoryIDName, GPGSIds.achievement_class_5_passed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BeatTier1CrewLeader.CategoryIDName, GPGSIds.achievement_beat_charlie);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BeatTier2CrewLeader.CategoryIDName, GPGSIds.achievement_beat_olivia);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BeatTier3CrewLeader.CategoryIDName, GPGSIds.achievement_beat_damian);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BeatTier4CrewLeader.CategoryIDName, GPGSIds.achievement_beat_ethan);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BeatAllCrewLeaders.CategoryIDName, GPGSIds.achievement_beat_all_the_leaders);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.Wheelspin500.CategoryIDName, GPGSIds.achievement_great_wheel_spin);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.TenSecQuarter.CategoryIDName, GPGSIds.achievement_10_seconds_win);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.FifteenSecHalf.CategoryIDName, GPGSIds.achievement_15_seconds_win);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.Reach180.CategoryIDName, GPGSIds.achievement_180_speed);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.WinByASecond.CategoryIDName, GPGSIds.achievement_1_second_win);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.WinByPoint01.CategoryIDName, GPGSIds.achievement_001_second_winner);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.LoseByPoint01.CategoryIDName, GPGSIds.achievement_001_second_loser);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.PerfectShift.CategoryIDName, GPGSIds.achievement_gear_shifter);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.FullyUpgradeACar.CategoryIDName, GPGSIds.achievement_full_upgrade);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyACarFrom4Tiers.CategoryIDName, GPGSIds.achievement_buy_a_car_from_each_class);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.Own18Cars.CategoryIDName, GPGSIds.achievement_buy_18_cars);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BigSpender.CategoryIDName, GPGSIds.achievement_spend_5_milion_cash);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.UseMechanic.CategoryIDName, GPGSIds.achievement_tune_your_car);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.WinInFirst.CategoryIDName, GPGSIds.achievement_tuner);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.Spend250Gold.CategoryIDName, GPGSIds.achievement_gold_spender);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyBMW.CategoryIDName, GPGSIds.achievement_get_a_bmw);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyChevrolet.CategoryIDName, GPGSIds.achievement_get_a_cheverolet);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyNissan.CategoryIDName, GPGSIds.achievement_get_a_nissan);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyAudi.CategoryIDName, GPGSIds.achievement_get_a_audi);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyMini.CategoryIDName, GPGSIds.achievement_get_a_minicooper);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.BuyFord.CategoryIDName, GPGSIds.achievement_get_a_ford);
			PlayGamesPlatform.Instance.AddIdMapping(Achievements.Own40Cars.CategoryIDName, GPGSIds.achievement_buy_40_cars);
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_OVERALL_EARNINGS")).CategoryIDName, "CgkIn__M29MbEAIQAQ");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_OVERALL_TIME")).CategoryIDName, "CgkIn__M29MbEAIQAA");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_D_QUARTER")).CategoryIDName, "CgkIn__M29MbEAIQAg");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_D_HALF")).CategoryIDName, "CgkIn__M29MbEAIQAw");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_C_QUARTER")).CategoryIDName, "CgkIn__M29MbEAIQBA");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_C_HALF")).CategoryIDName, "CgkIn__M29MbEAIQBQ");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_B_QUARTER")).CategoryIDName, "CgkIn__M29MbEAIQBg");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_B_HALF")).CategoryIDName, "CgkIn__M29MbEAIQBw");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_A_QUARTER")).CategoryIDName, "CgkIn__M29MbEAIQCA");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_A_HALF")).CategoryIDName, "CgkIn__M29MbEAIQCQ");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_S_QUARTER")).CategoryIDName, "CgkIn__M29MbEAIQCg");
			PlayGamesPlatform.Instance.AddIdMapping(Leaderboards.GetByIDName(GameCenterCategoryIDs.CategoryIDName("CSR_LDR_CLASS_S_HALF")).CategoryIDName, "CgkIn__M29MbEAIQCw");
			this.achievementIDsMapped = true;
		}
		foreach (SocialGamePlatformSelector.AchievementData current in PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements)
		{
			if (current.status == SocialGamePlatformSelector.AchievementStatus.requested)
			{
				this.ReportAchievement(current.achievement);
				current.status = SocialGamePlatformSelector.AchievementStatus.unlocked;
			}
		}
		foreach (SocialGamePlatformSelector.ScoreData current2 in PlayerProfileManager.Instance.ActiveProfile.PlayerScores)
		{
			if (current2.status == SocialGamePlatformSelector.ScoreStatus.local)
			{
				this.ReportScore(current2.score, current2.leaderboard);
			}
		}
		if (PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations > 0)
		{
			PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations = 0;
		}
		if (!PlayerProfileManager.Instance.ActiveProfile.HasSignedIntoGooglePlayGamesBefore)
		{
			PlayerProfileManager.Instance.ActiveProfile.HasSignedIntoGooglePlayGamesBefore = true;
		}
        //NmgBinding.AddSocialNetworkData("googleid", PlayGamesPlatform.Instance.GetUserId());
        //NmgBinding.AddSocialNetworkDatas("googleEmails", string.Join(" ", AndroidSpecific.GetGMailAddresses()));
	}

	public void SignOut()
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		if (this.IsPlayerAuthenticated())
		{
			PlayerProfileManager.Instance.ActiveProfile.GPGSignInCancellations++;
			PlayGamesPlatform.Instance.SignOut();
		}
	}

	public void ShowAchievements()
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		if (this.IsPlayerAuthenticated())
		{
			Social.ShowAchievementsUI();
		}
		else if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			this.playerInitiatedSignIn = true;
			this.AuthenticatePlayer(new Action<bool>(this.Callback_DisplayAchievementsWindowAfterLogin), false);
		}
		else
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_NO_INTERNET"));
		}
	}

	private void Callback_DisplayAchievementsWindowAfterLogin(bool success)
	{
		if (success)
		{
			this.InitialLogin();
			Social.ShowAchievementsUI();
		}
	}

	public void ReportAchievement(Achievement achievement)
	{
		this.ReportAchievement(achievement, true);
	}

	public void ReportAchievement(Achievement achievement, bool display)
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		try {
			if (achievement.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Count)
			{
				return;
			}

			SocialGamePlatformSelector.AchievementData playerAchievement =
				PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements[achievement.Idx];
			if (playerAchievement.status == SocialGamePlatformSelector.AchievementStatus.requested)
			{
				display = false;
			}

			playerAchievement.status = SocialGamePlatformSelector.AchievementStatus.requested;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			bool readyToSubmit = false;
			if (Social.localUser.authenticated)
			{
				readyToSubmit = true;
			}
			else
			{
				this.AuthenticatePlayer(delegate(bool success)
				{
					if (!success)
					{
					}

					readyToSubmit = success;
				}, true);
			}

			if (readyToSubmit)
			{
				Social.ReportProgress(achievement.CategoryIDName, 100.0, delegate(bool success)
				{
					if (success)
					{
						playerAchievement.status = SocialGamePlatformSelector.AchievementStatus.unlocked;
					}
				});
			}
			else
			{
				string text =
					LocalizationManager.GetTranslation(string.Format("TEXT_ACHIEVEMENT_NAME_{0}", achievement.Idx));
				List<GameCenterAchievementMetadata> achievementMetadataMain =
					GameCenterController.Instance.achievementMetadataMain;
				if (achievementMetadataMain != null)
				{
					foreach (GameCenterAchievementMetadata current in achievementMetadataMain)
					{
						if (current.identifier == achievement.CategoryIDName)
						{
							text = string.Concat(new string[]
							{
								current.title
							});
							break;
						}
					}
				}

				if (display)
				{
					AchievementsController.Instance.IntroduceAchievement(text);
				}
			}
		} catch {
			Debug.Log("Could not report achievement.");
		}
	}

	public void ShowLeaderboard(Leaderboard leaderboard = null)
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		if (leaderboard != null)
		{
		}
		if (Social.localUser.authenticated)
		{
			if (leaderboard == null)
			{
				Social.ShowLeaderboardUI();
			}
			else
			{
				((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(leaderboard.CategoryIDName);
			}
		}
		else if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			this.playerInitiatedSignIn = true;
			this.AuthenticatePlayer(new Action<bool>(this.Callback_DisplayLeaderboardsWindowAfterLogin), false);
		}
		else
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_NO_INTERNET"));
		}
	}

	private void Callback_DisplayLeaderboardsWindowAfterLogin(bool success)
	{
		if (success)
		{
			this.InitialLogin();
			Social.ShowLeaderboardUI();
		}
	}

	public bool ReportScore(int score, Leaderboard leaderboard)
	{
		if (leaderboard.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Count)
		{
			return false;
		}
		SocialGamePlatformSelector.ScoreData scoreData = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[leaderboard.Idx];
		bool flag = (!leaderboard.HigherIsBetter) ? (score < scoreData.score) : (score > scoreData.score);
		if (leaderboard.CategoryIDName.Contains("CSR_LDR_CLASS") && !this.ScoreLooksValid(score, leaderboard.CategoryIDName))
		{
			return false;
		}
		if (flag || scoreData.status == SocialGamePlatformSelector.ScoreStatus.local)
		{
			scoreData.score = score;
			scoreData.status = SocialGamePlatformSelector.ScoreStatus.local;
			this.AuthenticateAndPostScore(score, leaderboard);
			if (leaderboard.IsStandardRace)
			{
				SocialGamePlatformSelector.ScoreData scoreData2 = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[Leaderboards.OverallTime.Idx];
				int newOverallTime = SocialGamePlatformSelector.Instance.GetNewOverallTime();
				if (newOverallTime < scoreData2.score)
				{
					scoreData2.score = newOverallTime;
					scoreData2.status = SocialGamePlatformSelector.ScoreStatus.local;
					this.ReportScore(newOverallTime, Leaderboards.OverallTime);
				}
			}
			return true;
		}
		return false;
	}

	private bool ScoreLooksValid(int score, string leaderboard)
	{
		return this.validFinishTimes.ContainsKey(leaderboard) && score > this.validFinishTimes[leaderboard];
	}

	private void AuthenticateAndPostScore(int score, Leaderboard leaderboard)
	{
		if (!BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
			return;
		
		if (Social.localUser.authenticated)
		{
			Social.ReportScore((long)score, leaderboard.CategoryIDName, delegate(bool success)
			{
				if (success)
				{
					SocialGamePlatformSelector.ScoreData scoreData = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[leaderboard.Idx];
					scoreData.status = SocialGamePlatformSelector.ScoreStatus.posted;
					PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
				}
			});
		}
		else
		{
			PlayGamesPlatform.Instance.Authenticate(delegate(bool success)
			{
				if (success)
				{
					this.AuthenticateAndPostScore(score, leaderboard);
				}
			}, true);
		}
	}

	public void SetGooglePlayGamesDisabled(bool zState)
	{
		this.googlePlayGamesIsDisabled = zState;
	}

	public bool IsGooglePlayGamesDisabled()
	{
		return this.googlePlayGamesIsDisabled;
	}

	public bool IsPlayerAuthenticated()
	{
		return Social.localUser.authenticated;
	}

	public string GetPlayerID()
	{
		return Social.localUser.id;
	}

	public string GetCurrentAlias()
	{
		if (this.IsPlayerAuthenticated())
		{
			return NameValidater.ReplaceUnsupportedCharacters(Social.localUser.userName);
		}
		return NameValidater.ReplaceUnsupportedCharacters("ba-gc: " + Environment.UserName);
	}

	public static void PlayerPhotoLoadComplete(string userID, string filename)
	{
		if (GooglePlayGamesController.PlayerPhotoLoaded != null)
		{
			GooglePlayGamesController.PlayerPhotoLoaded(userID, filename);
		}
	}

	public static void PlayerPhotoLoadError(string userID, string error)
	{
		if (GooglePlayGamesController.PlayerPhotoLoaded != null)
		{
			GooglePlayGamesController.PlayerPhotoLoadFailed(userID, error);
		}
	}
}
#endif
