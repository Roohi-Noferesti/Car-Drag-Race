using System;

public class GameCenterCategoryIDs
{
	private static string groupPrefix = "grp.";

	private static string BAPrefix = "BA_";

	private static string rootPrefix = string.Empty;

	private static bool isValid = false;

	private static string Base
	{
		get
		{
			GameCenterCategoryIDs.VerifyRootPrefix();
			return GameCenterCategoryIDs.rootPrefix;
		}
	}

	public static string CategoryIDName(string therest)
	{
		return GameCenterCategoryIDs.Base + therest;
	}

	private static void VerifyRootPrefix()
	{
		if (GameCenterCategoryIDs.isValid)
		{
			return;
		}
		string bundleIdentifier = BasePlatform.ActivePlatform.GetBundleIdentifier();
		if (bundleIdentifier.ToLower().Contains("kingkodestudio"))
		{
			GameCenterCategoryIDs.rootPrefix = GameCenterCategoryIDs.groupPrefix + GameCenterCategoryIDs.BAPrefix;
		}
		else
		{
			GameCenterCategoryIDs.rootPrefix = GameCenterCategoryIDs.groupPrefix;
		}
		GameCenterCategoryIDs.isValid = true;
	}

	public static bool Matches(string firstCategory, string matchCategory)
	{
		bool result = false;
		string text = GameCenterCategoryIDs.StripPotentialPrefix(firstCategory, GameCenterCategoryIDs.rootPrefix);
		string text2 = GameCenterCategoryIDs.StripPotentialPrefix(matchCategory, GameCenterCategoryIDs.rootPrefix);
		text = GameCenterCategoryIDs.StripPotentialPrefix(text, GameCenterCategoryIDs.groupPrefix);
		text2 = GameCenterCategoryIDs.StripPotentialPrefix(text2, GameCenterCategoryIDs.groupPrefix);
		if (text == text2)
		{
			result = true;
		}
		return result;
	}

	private static string StripPotentialPrefix(string category, string prefix)
	{
		if (category.StartsWith(prefix))
		{
			category = category.Substring(prefix.Length);
		}
		return category;
	}
}
