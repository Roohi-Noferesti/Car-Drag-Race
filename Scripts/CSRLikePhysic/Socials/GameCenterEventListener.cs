using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("BossAlien/GameCenter/BAEventListener")]
public class GameCenterEventListener : MonoBehaviour
{
	public List<GameCenterAchievementMetadata> achievementMetadataMain;

	public List<GameCenterAchievement> achievementsMain;

	private void Start()
	{
		GameCenterManager.loadPlayerDataFailed += new GameCenterErrorEventHandler(this.loadPlayerDataFailed);
		GameCenterManager.playerDataLoaded += new PlayerDataLoadedEventHandler(this.playerDataLoaded);
		GameCenterManager.playerAuthenticated += new GameCenterEventHandler(this.playerAuthenticated);
		GameCenterManager.playerFailedToAuthenticate += new GameCenterErrorEventHandler(this.playerFailedToAuthenticate);
		GameCenterManager.playerLoggedOut += new GameCenterEventHandler(this.playerLoggedOut);
		GameCenterManager.loadCategoryTitlesFailed += new GameCenterErrorEventHandler(this.loadCategoryTitlesFailed);
		GameCenterManager.categoriesLoaded += new CategoriesLoadedEventHandler(this.categoriesLoaded);
		GameCenterManager.reportScoreFailed += new GameCenterErrorEventHandler(this.reportScoreFailed);
		GameCenterManager.reportScoreFinished += new GameCenterStringEventHandler(this.reportScoreFinished);
		GameCenterManager.retrieveScoresFailed += new GameCenterStringEventHandler(this.retrieveScoresFailed);
		GameCenterManager.scoresLoaded += new ScoresLoadedEventHandler(this.scoresLoaded);
		GameCenterManager.retrieveScoresForPlayerIdFailed += new GameCenterErrorEventHandler(this.retrieveScoresForPlayerIdFailed);
		GameCenterManager.scoresForPlayerIdLoaded += new ScoresLoadedEventHandler(this.scoresForPlayerIdLoaded);
		GameCenterManager.reportAchievementFailed += new GameCenterErrorEventHandler(this.reportAchievementFailed);
		GameCenterManager.reportAchievementFinished += new GameCenterStringEventHandler(this.reportAchievementFinished);
		GameCenterManager.loadAchievementsFailed += new GameCenterErrorEventHandler(this.loadAchievementsFailed);
		GameCenterManager.achievementsLoaded += new AchievementLoadedEventHandler(this.achievementsLoaded);
		GameCenterManager.resetAchievementsFailed += new GameCenterErrorEventHandler(this.resetAchievementsFailed);
		GameCenterManager.resetAchievementsFinished += new GameCenterEventHandler(this.resetAchievementsFinished);
		GameCenterManager.retrieveAchievementMetadataFailed += new GameCenterErrorEventHandler(this.retrieveAchievementMetadataFailed);
		GameCenterManager.achievementMetadataLoaded += new AchievementMetadataLoadedEventHandler(this.achievementMetadataLoaded);
	}

	private void playerAuthenticated()
	{
		GameCenterController.Instance.pullGameCenterInfo();
		GameCenterController.Instance.callBack_playAuthenticated();
	}

	private void playerFailedToAuthenticate(string error)
	{
		GameCenterController.Instance.callBack_playerFailedToAuthenticate(error);
	}

	private void playerLoggedOut()
	{
		GameCenterController.Instance.callBack_playerLoggedOut();
		PauseGame.PauseIfInRace();
	}

	private void playerDataLoaded(List<GameCenterPlayer> players)
	{
	}

	private void loadPlayerDataFailed(string error)
	{
	}

	private void categoriesLoaded(List<GameCenterLeaderboard> leaderboards)
	{
	}

	private void loadCategoryTitlesFailed(string error)
	{
	}

	private void scoresLoaded(List<GameCenterScore> scores)
	{
		GameCenterController.Instance.updateLocalScores(scores);
		GameCenterController.Instance.SyncScoreStatusWithGameCenter();
	}

	private void retrieveScoresFailed(string error)
	{
	}

	private void retrieveScoresForPlayerIdFailed(string error)
	{
	}

	private void scoresForPlayerIdLoaded(List<GameCenterScore> scores)
	{
		GameCenterController.Instance.updateLocalScores(scores);
		GameCenterController.Instance.SyncScoreStatusWithGameCenter();
	}

	private void reportScoreFinished(string gcCategoryIDName)
	{
		Leaderboard byIDName = Leaderboards.GetByIDName(gcCategoryIDName);
		GameCenterController.Instance.displayScore(byIDName);
	}

	private void reportScoreFailed(string error)
	{
	}

	private void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementMetadata)
	{
		GameCenterController.Instance.updateLocalAchievementsMeta(achievementMetadata);
	}

	private void retrieveAchievementMetadataFailed(string error)
	{
	}

	private void resetAchievementsFinished()
	{
	}

	private void resetAchievementsFailed(string error)
	{
	}

	private void achievementsLoaded(List<GameCenterAchievement> achievements)
	{
		GameCenterController.Instance.updateLocalAchievements(achievements);
		GameCenterController.Instance.SyncAchievementStatusWithGameCenter();
	}

	private void loadAchievementsFailed(string error)
	{
	}

	private void reportAchievementFinished(string gcCategoryIDName)
	{
		Achievement byCategoryIDName = Achievements.GetByCategoryIDName(gcCategoryIDName);
		GameCenterController.Instance.registerAchievement(byCategoryIDName);
		GameCenterController.Instance.pullGameCenterInfo();
	}

	private void reportAchievementFailed(string error)
	{
	}
}
