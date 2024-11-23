using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class GameCenterController : MonoBehaviour
{
	public enum AchievementStatus
	{
		locked,
		requested,
		unlocked
	}

	public class AchievementData
	{
		public Achievement achievement;

		public GameCenterController.AchievementStatus status;
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

		public GameCenterController.ScoreStatus status;
	}

	public delegate void PostGameCentreStartedDelegate(AssetSystemManager.Reason reason);

	private const int numberOfRaceCategories = 10;

	public static bool GameCenterIsDisabled;

	private int baseTimeScore;

	public List<GameCenterAchievementMetadata> achievementMetadataMain;

	private List<GameCenterAchievement> achievementsMain;

	private List<GameCenterScore> scoresMain;

	public GameCenterControllerPlayAuthenticatedDelegate PlayAuthenticatedEvent;

	public GameCenterControllerFailedAuthenticatedDelegate FailedAuthenticatedEvent;

	public GameCenterControllerLoggedOutDelegate LoggedOutEvent;

	public GameCenterController.PostGameCentreStartedDelegate PostGameCentreStarted;

	private bool playerLoggedIn;

	private bool showLeaderboardDialogue;

	private bool showAchievementDialogue;

	private bool shownGCChangedDialogue;

	private bool deferredIDChange;

	public static GameCenterController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (GameCenterController.Instance != null)
		{
			return;
		}
		GameCenterController.Instance = this;
		this.baseTimeScore = GameCenterController.Instance.ConvertTimeForGameCenter(360f);
		this.playerLoggedIn = false;
		this.showLeaderboardDialogue = false;
		this.showAchievementDialogue = false;
		this.shownGCChangedDialogue = false;
		this.deferredIDChange = false;

		AssetSystemManager.JustKicked += new AssetSystemManager.JustKickedDelegate(this.StartGameCenterWhenSafe);
		if (BasePlatform.TargetPlatform == GTPlatforms.ANDROID)
		{
			ApplicationManager.DidBecomeActiveEvent += new ApplicationEvent_Delegate(this.CheckGCIdChanges);
		}
	}

	private void OnDestroy()
	{
		AssetSystemManager.JustKicked -= new AssetSystemManager.JustKickedDelegate(this.StartGameCenterWhenSafe);
		if (BasePlatform.TargetPlatform == GTPlatforms.ANDROID)
		{
			ApplicationManager.DidBecomeActiveEvent -= new ApplicationEvent_Delegate(this.CheckGCIdChanges);
		}
	}

	public void SetGameCenterDisabled(bool zState)
	{
		GameCenterController.GameCenterIsDisabled = zState;
		GameCenterBinding.SetGameCenterDisabled(zState);
	}

	public bool IsGameCenterDisabled()
	{
		return GameCenterController.GameCenterIsDisabled;
	}

	public void OnProfileChanged()
	{
		bool flag = false;
		this.shownGCChangedDialogue = false;
		if (PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Count == 0)
		{
			SocialGamePlatformSelector.Instance.ResetAchievements();
			flag = true;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Count != Leaderboards.Count)
		{
			SocialGamePlatformSelector.Instance.ResetScores();
			flag = true;
		}
		if (flag)
		{
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public void StartGameCenterWhenSafe(AssetSystemManager.Reason reason)
	{
		this.shownGCChangedDialogue = false;
		if (GameCenterBinding.isGameCenterAvailable())
		{
			if (!GameCenterBinding.isPlayerAuthenticated())
			{
				GameCenterBinding.authenticateLocalPlayer();
			}
			else
			{
				this.playerLoggedIn = true;
				GameCenterController.Instance.pullGameCenterInfo();
				GameCenterController.Instance.callBack_playAuthenticated();
			}
		}
		if (this.PostGameCentreStarted != null)
		{
			this.PostGameCentreStarted(reason);
		}
	}

	private void Update()
	{
		if (Kickback.IsSafePlaceToKickback() && this.deferredIDChange)
		{
			this.DoDeferredIDChange();
		}
	}

	public bool isGameCenterAvailable()
	{
		return GameCenterBinding.isGameCenterAvailable();
	}

	public void authenticatePlayer()
	{
		GameCenterBinding.authenticateLocalPlayer();
	}

	public bool isPlayerLoggedIn()
	{
		bool flag = this.playerLoggedIn;
		return false;
	}

	public bool isPlayerLoggedInAndGameCenterPicsAvailable()
	{
		return this.isPlayerLoggedIn() && GameCenterBinding.areGameCenterPicsAvailable();
	}

	public bool isPlayerAuthenticated()
	{
		return GameCenterBinding.isPlayerAuthenticated();
	}

	public string currentID()
	{
		return GameCenterBinding.playerIdentifier();
	}

	public string currentAlias()
	{
#if UNITY_IOS
		return GameCenterBinding.playerAlias();
#endif
		return string.Empty;
	}

	public bool currentAliasCanBeDisplayed()
	{
		return true;
	}

    public void DoDeferredIDChange()
    {
        string gCid = UserManager.Instance.currentAccount.GCid;
        //NmgBinding.SetGameCenterID(gCid);
        switch (BasePlatform.ActivePlatform.GetTargetAppStore())
        {
            case GTAppStore.iOS:
                this.ShowGCChangedPopup("TEXT_POPUPS_GAMECENTER_TITLE", "TEXT_POPUPS_GAMECENTER_BODY_CHANGED");
                break;
            case GTAppStore.GooglePlay:
#if UNITY_ANDROID
            case GTAppStore.Bazaar:
            case GTAppStore.Iraqapps:
            case GTAppStore.Myket:
#endif
            case GTAppStore.None:
                if (gCid.Contains("@") && !this.currentID().Contains("@"))
                {
                    this.ShowGCChangedPopup("TEXT_POPUPS_GOOGLE_TITLE_CHANGED", "TEXT_POPUPS_GOOGLE_BODY_MIGRATION");
                }
                else
                {
                    this.ShowGCChangedPopup("TEXT_POPUPS_GOOGLE_TITLE_CHANGED", "TEXT_POPUPS_GOOGLE_BODY_CHANGED");
                }
                break;
            case GTAppStore.Amazon:
                this.ShowGCChangedPopup("TEXT_POPUPS_AMAZON_TITLE_CHANGED", "TEXT_POPUPS_AMAZON_BODY_CHANGED");
                break;
        }
        this.deferredIDChange = false;
    }

    private bool GCIdUndefined(string a)
	{
		return string.IsNullOrEmpty(a);
	}

	private bool GCIdEquivilant(string a, string b)
	{
		return (this.GCIdUndefined(a) && this.GCIdUndefined(b)) || (!this.GCIdUndefined(a) && !this.GCIdUndefined(b) && string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
	}

	private void CheckGCIdChanges()
	{
		string gCid = UserManager.Instance.currentAccount.GCid;
		//Debug.Log("GCID : "+gCid+" , "+this.currentID());
		if (this.GCIdUndefined(gCid) && !this.GCIdUndefined(this.currentID()))
		{
			UserManager.Instance.currentAccount.GCid = this.currentID();
			UserManager.Instance.SaveCurrentAccount();
			UserManager.Instance.StartConnect();
		}
		else if (!this.GCIdUndefined(gCid) && !this.GCIdEquivilant(gCid, this.currentID()) && !this.GCIdUndefined(this.currentID()))
		{
			//Debug.Log("deferredIDChange : "+gCid+" , "+this.currentID());
			this.deferredIDChange = true;
		}
	}

	public void callBack_playAuthenticated()
	{
		this.playerLoggedIn = true;
		if (this.PlayAuthenticatedEvent != null)
		{
			this.PlayAuthenticatedEvent();
		}
		if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
		{
			this.CheckGCIdChanges();
		}
		if (BasePlatform.ActivePlatform.GetTargetAppStore() == GTAppStore.Amazon && UserManager.Instance != null)
		{
			UserManager.Instance.StartConnect();
		}
	}

	public void ForceDeferredIDChange()
	{
		this.deferredIDChange = true;
	}

	private void ShowGCChangedPopup(string title, string info)
	{
		if (!this.shownGCChangedDialogue)
		{
			this.shownGCChangedDialogue = true;
			PopUp popup = new PopUp
			{
				Title = title,
				BodyText = info,
				ConfirmAction = new PopUpButtonAction(this.onGCOkButtonPressed),
				ConfirmText = "TEXT_BUTTON_OK"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
		}
	}

	public void callBack_playerFailedToAuthenticate(string error)
	{
		if (BasePlatform.ActivePlatform.GetTargetAppStore() == GTAppStore.Amazon && UserManager.Instance != null)
		{
			UserManager.Instance.StartConnect();
		}
		if (this.FailedAuthenticatedEvent != null)
		{
			this.FailedAuthenticatedEvent(error);
		}
	}

	public void callBack_playerLoggedOut()
	{
		this.playerLoggedIn = false;
		if (this.showLeaderboardDialogue)
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_GAME_CENTER_LEADERBOARD_LOGIN"));
			this.showLeaderboardDialogue = false;
		}
		if (this.showAchievementDialogue)
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_GAME_CENTER_ACHIEVEMENT_LOGIN"));
			this.showAchievementDialogue = false;
		}
		if (this.LoggedOutEvent != null)
		{
			this.LoggedOutEvent();
		}
	}

	public void ShowLeaderboard()
	{
		this.showLeaderboardDialogue = true;
		this.ShowPopup("TEXT_POPUPS_GAMECENTER_LEADERBOARD_DEVICE_ONLY");
	}

	public void ShowAchievements()
	{
		this.showAchievementDialogue = true;
		this.ShowPopup("TEXT_POPUPS_GAMECENTER_ACHIEVEMENTS_DEVICE_ONLY");
	}

	public float getBestScoreForEvent()
	{
		int num = 0;
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (!RaceEventInfo.Instance.IsDailyBattleEvent && currentEvent != null && !currentEvent.IsTutorial())
		{
			CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
			eCarTier currentTier = currentCar.CurrentTier;
			Leaderboard leaderboardForRace = Leaderboards.GetLeaderboardForRace(currentTier, currentEvent.IsHalfMile);
			num = leaderboardForRace.BestScore();
		}
		return (float)num / 100f;
	}

	public bool reportScore(int score, Leaderboard leaderboard)
	{
		if (leaderboard.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Count)
		{
			return false;
		}
		SocialGamePlatformSelector.ScoreData scoreData = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[leaderboard.Idx];
		bool flag = (!leaderboard.HigherIsBetter) ? (score < scoreData.score) : (score > scoreData.score);
		if (flag)
		{
			scoreData.score = score;
			scoreData.status = SocialGamePlatformSelector.ScoreStatus.local;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			GameCenterBinding.reportScore((long)score, leaderboard.CategoryIDName);
			if (leaderboard.IsStandardRace)
			{
				int score2 = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[Leaderboards.OverallTime.Idx].score;
				int newOverallTime = this.getNewOverallTime();
				if (newOverallTime < score2)
				{
					this.reportScore(newOverallTime, Leaderboards.OverallTime);
				}
			}
			this.displayScore(leaderboard);
			return true;
		}
		return false;
	}

	public bool reportAchievement(Achievement achievement)
	{
		return this.reportAchievement(achievement, true);
	}

	public bool reportAchievement(Achievement achievement, bool display)
	{
		return this.reportAchievement(achievement, display, false);
	}

	public bool reportAchievement(Achievement achievement, bool display, bool syncOverride)
	{
		if (achievement.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Count)
		{
			return false;
		}
		SocialGamePlatformSelector.AchievementData achievementData = PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements[achievement.Idx];
		if (achievementData.status == SocialGamePlatformSelector.AchievementStatus.requested)
		{
			display = false;
		}
		if (achievementData.status == SocialGamePlatformSelector.AchievementStatus.locked || achievementData.status == SocialGamePlatformSelector.AchievementStatus.requested)
		{
			achievementData.status = SocialGamePlatformSelector.AchievementStatus.requested;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			if (display)
			{
				this.displayAchievement(achievement);
			}
			return true;
		}
		return syncOverride;
	}

	public void pullGameCenterInfo()
	{
		GameCenterBinding.retrieveAchievementMetadata();
		GameCenterBinding.loadLeaderboardCategoryTitles();
		GameCenterBinding.getAchievements();
		GameCenterBinding.retrieveScoresForPlayerId(UserManager.Instance.currentAccount.GCid);
	}

	public void pullGameCenterFriends()
	{
		GameCenterBinding.retrieveFriends();
	}

	public void updateLocalAchievements(List<GameCenterAchievement> achievements)
	{
		this.achievementsMain = achievements;
	}

	public void updateLocalAchievementsMeta(List<GameCenterAchievementMetadata> achievementMetadata)
	{
		this.achievementMetadataMain = achievementMetadata;
	}

	public void updateLocalScores(List<GameCenterScore> scores)
	{
		this.scoresMain = scores;
	}

	public string GetAchievementStringComparison()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (this.achievementMetadataMain == null)
		{
			return "NO_CHIEVOS";
		}
		foreach (GameCenterAchievementMetadata current in this.achievementMetadataMain)
		{
			Achievement byCategoryIDName = Achievements.GetByCategoryIDName(current.identifier);
			if (byCategoryIDName != null)
			{
				string text3 = LocalizationManager.GetTranslation(string.Format("TEXT_ACHIEVEMENT_NAME_{0}", byCategoryIDName.Idx));
				string text4 = text;
				text = string.Concat(new string[]
				{
					text4,
					"Localisation: ",
					text3,
					", Connect: ",
					current.title,
					"\n"
				});
				if (text3 != current.title)
				{
					text4 = text2;
					text2 = string.Concat(new string[]
					{
						text4,
						"Localisation: ",
						text3,
						", Connect: ",
						current.title,
						"\n"
					});
				}
			}
		}
		return text + "DIFFERENT: \n" + text2;
	}

	public void registerAchievement(Achievement achievement)
	{
		if (achievement.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements.Count)
		{
			return;
		}
		SocialGamePlatformSelector.AchievementData achievementData = PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements[achievement.Idx];
		if (achievementData.status == SocialGamePlatformSelector.AchievementStatus.requested)
		{
			achievementData.status = SocialGamePlatformSelector.AchievementStatus.unlocked;
		}
	}

	public void displayAchievement(Achievement achievement)
	{
		string text = LocalizationManager.GetTranslation(string.Format("TEXT_ACHIEVEMENT_NAME_{0}", achievement.Idx));
		bool flag = false;
		bool flag2 = true;
		if (this.achievementMetadataMain != null)
		{
			foreach (GameCenterAchievementMetadata current in this.achievementMetadataMain)
			{
				if (current.identifier == achievement.CategoryIDName)
				{
					if (flag)
					{
						text = string.Concat(new string[]
						{
							current.title
						});
					}
					if (current.isHidden)
					{
						flag2 = false;
					}
					break;
				}
			}
		}
		if (flag2)
		{
			AchievementsController.Instance.IntroduceAchievement(text);
		}
	}

	public void displayScore(Leaderboard leaderboard)
	{
		if (leaderboard.Idx > PlayerProfileManager.Instance.ActiveProfile.PlayerScores.Count)
		{
			return;
		}
		SocialGamePlatformSelector.ScoreData scoreData = PlayerProfileManager.Instance.ActiveProfile.PlayerScores[leaderboard.Idx];
		if (scoreData.status == SocialGamePlatformSelector.ScoreStatus.local)
		{
			scoreData.status = SocialGamePlatformSelector.ScoreStatus.posted;
		}
	}

	public void onGCOkButtonPressed()
	{
		if (SocialController.Instance.isLoggedIntoFacebook)
		{
			BasePlatform.ActivePlatform.FacebookLogout();
		}
		UserManager.Instance.StartConnect();
		AssetSystemManager.Instance.KickBackToSafePlaceAndReload(AssetSystemManager.Reason.OnGameCenterChanged);
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

	public void ResetGameCenterAchievements()
	{
	}

	public void SyncAchievementStatusWithGameCenter()
	{
		foreach (SocialGamePlatformSelector.AchievementData current in PlayerProfileManager.Instance.ActiveProfile.PlayerAchievements)
		{
			Achievement achievement = current.achievement;
			bool flag = false;
			if (this.achievementsMain != null)
			{
				foreach (GameCenterAchievement current2 in this.achievementsMain)
				{
					if (achievement.Matches(current2))
					{
						flag = true;
						if (current.status != SocialGamePlatformSelector.AchievementStatus.unlocked)
						{
							current.status = SocialGamePlatformSelector.AchievementStatus.unlocked;
						}
					}
				}
				if (current.status == SocialGamePlatformSelector.AchievementStatus.requested && !flag)
				{
					this.reportAchievement(achievement, false);
				}
				if (current.status == SocialGamePlatformSelector.AchievementStatus.unlocked && !flag)
				{
					this.reportAchievement(achievement, false, true);
				}
			}
		}
	}

	public void SyncScoreStatusWithGameCenter()
	{
		foreach (SocialGamePlatformSelector.ScoreData current in PlayerProfileManager.Instance.ActiveProfile.PlayerScores)
		{
			if (current != null)
			{
				if (current.status == SocialGamePlatformSelector.ScoreStatus.local)
				{
					bool flag = false;
					string b = this.currentAlias();
					if (this.scoresMain != null)
					{
						foreach (GameCenterScore current2 in this.scoresMain)
						{
							if (current2.alias == b && current.leaderboard.CategoryIDName == current2.category)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							if (current.score != this.baseTimeScore && current.score != 0)
							{
								GameCenterBinding.reportScore((long)current.score, current.leaderboard.CategoryIDName);
							}
						}
						else
						{
							current.status = SocialGamePlatformSelector.ScoreStatus.posted;
						}
					}
				}
			}
		}
	}

	private int getNewOverallTime()
	{
		float num = 0f;
		foreach (SocialGamePlatformSelector.ScoreData current in PlayerProfileManager.Instance.ActiveProfile.PlayerScores)
		{
			if (current.leaderboard.IsStandardRace)
			{
				num += this.ConvertTimeFromGameCenter(current.score);
			}
		}
		return this.ConvertTimeForGameCenter(num);
	}

	public int ConvertTimeForGameCenter(float RaceTime)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)Math.Round((decimal)RaceTime, 2));
		return timeSpan.Minutes * 6000 + timeSpan.Seconds * 100 + timeSpan.Milliseconds / 10;
	}

	public float ConvertTimeFromGameCenter(int RaceTime)
	{
		int num = RaceTime / 6000;
		int num2 = (RaceTime - num * 6000) / 100;
		int num3 = RaceTime - num * 6000 - num2 * 100;
		return (float)(num * 60 + num2) + (float)num3 / 100f;
	}
}
