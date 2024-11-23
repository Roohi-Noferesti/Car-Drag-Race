using System;
using System.Collections.Generic;

public static class GameCenterManager
{
    public static event GameCenterErrorEventHandler loadPlayerDataFailed;

    public static event PlayerDataLoadedEventHandler playerDataLoaded;

    public static event GameCenterEventHandler playerAuthenticated;

    public static event GameCenter2StringEventHandler playerPhotoLoaded;

    public static event GameCenter2StringEventHandler playerPhotoFailed;

    public static event GameCenterErrorEventHandler playerFailedToAuthenticate;

    public static event GameCenterEventHandler playerLoggedOut;

    public static event GameCenterErrorEventHandler loadCategoryTitlesFailed;

    public static event CategoriesLoadedEventHandler categoriesLoaded;

    public static event GameCenterErrorEventHandler reportScoreFailed;

    public static event GameCenterStringEventHandler reportScoreFinished;

    public static event GameCenterStringEventHandler retrieveScoresFailed;

    public static event ScoresLoadedEventHandler scoresLoaded;

    public static event GameCenterErrorEventHandler retrieveScoresForPlayerIdFailed;

    public static event ScoresLoadedEventHandler scoresForPlayerIdLoaded;

    public static event GameCenterErrorEventHandler reportAchievementFailed;

    public static event GameCenterStringEventHandler reportAchievementFinished;

    public static event GameCenterErrorEventHandler loadAchievementsFailed;

    public static event AchievementLoadedEventHandler achievementsLoaded;

    public static event GameCenterErrorEventHandler resetAchievementsFailed;

    public static event GameCenterEventHandler resetAchievementsFinished;

    public static event GameCenterErrorEventHandler retrieveAchievementMetadataFailed;

    public static event AchievementMetadataLoadedEventHandler achievementMetadataLoaded;

	public static void loadPlayerDataDidFail(string error)
	{
		if (GameCenterManager.loadPlayerDataFailed != null)
		{
			GameCenterManager.loadPlayerDataFailed(error);
		}
	}

	public static void loadPlayerDataDidLoad(string jsonFriendList)
	{
		if (jsonFriendList == "[{}]")
		{
		}
	}

	public static void playerDidLogOut()
	{
		if (GameCenterManager.playerLoggedOut != null)
		{
			GameCenterManager.playerLoggedOut();
		}
	}

	public static void playerDidAuthenticate()
	{
		if (GameCenterManager.playerAuthenticated != null)
		{
			GameCenterManager.playerAuthenticated();
		}
	}

	public static void playerAuthenticationFailed(string error)
	{
		if (GameCenterManager.playerFailedToAuthenticate != null)
		{
			GameCenterManager.playerFailedToAuthenticate(error);
		}
	}

	public static void loadPlayerPhotoDidLoad(string userID, string filename)
	{
		if (GameCenterManager.playerPhotoLoaded != null)
		{
			GameCenterManager.playerPhotoLoaded(userID, filename);
		}
	}

	public static void loadPlayerPhotoDidFail(string userID, string error)
	{
		if (GameCenterManager.playerPhotoFailed != null)
		{
			GameCenterManager.playerPhotoFailed(userID, error);
		}
	}

	public static void loadCategoryTitlesDidFail(string error)
	{
		if (GameCenterManager.loadCategoryTitlesFailed != null)
		{
			GameCenterManager.loadCategoryTitlesFailed(error);
		}
	}

	public static void categoriesDidLoad(string jsonCategoryList)
	{
		List<GameCenterLeaderboard> leaderboards = GameCenterLeaderboard.fromJSON(jsonCategoryList);
		if (GameCenterManager.categoriesLoaded != null)
		{
			GameCenterManager.categoriesLoaded(leaderboards);
		}
	}

	public static void reportScoreDidFail(string error)
	{
		if (GameCenterManager.reportScoreFailed != null)
		{
			GameCenterManager.reportScoreFailed(error);
		}
	}

	public static void reportScoreDidFinish(string category)
	{
		if (GameCenterManager.reportScoreFinished != null)
		{
			GameCenterManager.reportScoreFinished(category);
		}
	}

	public static void retrieveScoresDidFail(string category)
	{
		if (GameCenterManager.retrieveScoresFailed != null)
		{
			GameCenterManager.retrieveScoresFailed(category);
		}
	}

	public static void retrieveScoresDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresLoaded != null)
		{
			GameCenterManager.scoresLoaded(scores);
		}
	}

	public static void retrieveScoresForPlayerIdDidFail(string error)
	{
		if (GameCenterManager.retrieveScoresForPlayerIdFailed != null)
		{
			GameCenterManager.retrieveScoresForPlayerIdFailed(error);
		}
	}

	public static void retrieveScoresForPlayerIdDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresForPlayerIdLoaded != null)
		{
			GameCenterManager.scoresForPlayerIdLoaded(scores);
		}
	}

	public static void reportAchievementDidFail(string error)
	{
		if (GameCenterManager.reportAchievementFailed != null)
		{
			GameCenterManager.reportAchievementFailed(error);
		}
	}

	public static void reportAchievementDidFinish(string identifier)
	{
		if (GameCenterManager.reportAchievementFinished != null)
		{
			GameCenterManager.reportAchievementFinished(identifier);
		}
	}

	public static void loadAchievementsDidFail(string error)
	{
		if (GameCenterManager.loadAchievementsFailed != null)
		{
			GameCenterManager.loadAchievementsFailed(error);
		}
	}

	public static void achievementsDidLoad(string jsonAchievmentList)
	{
		List<GameCenterAchievement> achievements = GameCenterAchievement.fromJSON(jsonAchievmentList);
		if (GameCenterManager.achievementsLoaded != null)
		{
			GameCenterManager.achievementsLoaded(achievements);
		}
	}

	public static void resetAchievementsDidFail(string error)
	{
		if (GameCenterManager.resetAchievementsFailed != null)
		{
			GameCenterManager.resetAchievementsFailed(error);
		}
	}

	public static void resetAchievementsDidFinish(string emptyString)
	{
		if (GameCenterManager.resetAchievementsFinished != null)
		{
			GameCenterManager.resetAchievementsFinished();
		}
	}

	public static void retrieveAchievementsMetadataDidFail(string error)
	{
		if (GameCenterManager.retrieveAchievementMetadataFailed != null)
		{
			GameCenterManager.retrieveAchievementMetadataFailed(error);
		}
	}

	public static void achievementMetadataDidLoad(string jsonAchievementDescriptionList)
	{
		List<GameCenterAchievementMetadata> achievementMetadata = GameCenterAchievementMetadata.fromJSON(jsonAchievementDescriptionList);
		if (GameCenterManager.achievementMetadataLoaded != null)
		{
			GameCenterManager.achievementMetadataLoaded(achievementMetadata);
		}
	}

	public static void retrieveMatchesBestScoresDidFail(string error)
	{
	}
}
