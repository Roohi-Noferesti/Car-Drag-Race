using System;
using System.Collections.Generic;

public static class WorldTourAnimSeenMapper
{
	private static string THEME_ID_PROFILE_KEY = "tifa";

	private static string ANIM_SEEN_LIST_PROFILE_KEY = "loas";

	public static void ReadFromJson(JsonDict jsonDict, ref WorldTourAnimationsCompleted result)
	{
		jsonDict.TryGetValue(WorldTourAnimSeenMapper.THEME_ID_PROFILE_KEY, out result.ThemeID);
		jsonDict.TryGetValue(WorldTourAnimSeenMapper.ANIM_SEEN_LIST_PROFILE_KEY, out result.AmimCompletedForTheme);
	}

	public static void WriteToJson(WorldTourAnimationsCompleted animationsCompleted, ref JsonDict jsonDict)
	{
		jsonDict.Set(WorldTourAnimSeenMapper.THEME_ID_PROFILE_KEY, animationsCompleted.ThemeID);
		jsonDict.Set(WorldTourAnimSeenMapper.ANIM_SEEN_LIST_PROFILE_KEY, animationsCompleted.AmimCompletedForTheme);
	}

	public static void RunTierItaliaFixMigration(ref List<WorldTourAnimationsCompleted> animations)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			if (animations[i].ThemeID == "TIERXITALIA" && animations[i].AmimCompletedForTheme.Count >= 5)
			{
				for (int j = 0; j < animations[i].AmimCompletedForTheme.Count; j++)
				{
					if (animations[i].AmimCompletedForTheme[j] == string.Empty)
					{
						animations[i].AmimCompletedForTheme[j] = "ItaliaCrewDefeat";
						return;
					}
				}
			}
		}
	}
}
