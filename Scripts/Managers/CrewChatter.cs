using System;
using I2.Loc;
using UnityEngine;

public class CrewChatter
{
	private static int preBossRaceMessageToUse;

	private static int crewLostRaceMessageToUse;

	private static int bossLoseRaceMessageToUse;

	private static int bossWinRaceMessageToUse;

    public static string GetCrewIntroPreRace(int zTier, int index)
    {
        var textKey = string.Concat(new object[]
	    {
	        "TEXT_CHATTER_INTRO_T_",
	        CarTierHelper.TierToString[zTier],
	        "_D_",
	        index,
	        "_PRERACE"
	    });
        return LocalizationManager.GetTranslation(textKey);
    }

	public static string GetCrewPreRace(int zTier, int zMember)
    {
	    var textKey = string.Concat(new object[]
	    {
	        "TEXT_CHATTER_T",
	        CarTierHelper.TierToString[zTier],
	        "_C",
	        zMember,
	        "_PRERACE"
	    });
        return LocalizationManager.GetTranslation(textKey);
	}


    public static string GetCrewMemberPreRace(int zTier, int zMember)
    {
        var textKey = string.Concat(new object[]
        {
            "TEXT_CHATTER_T",
            CarTierHelper.TierToString[zTier],
            "_MEMBER_",
            zMember,
            "_PRERACE"
        });
        return LocalizationManager.GetTranslation(textKey);
    }

	public static string GetCrewLostRaceTitle(int zTier)
	{
        return LocalizationManager.GetTranslation("TEXT_CHATTER_T" + CarTierHelper.TierToString[zTier] + "_LOSE_TITLE");
	}

	public static string GetCrewLostRace(int zTier)
	{
		if (CrewChatter.crewLostRaceMessageToUse == 0)
		{
			CrewChatter.crewLostRaceMessageToUse = UnityEngine.Random.Range(0, 4);
		}
		CrewChatter.crewLostRaceMessageToUse++;
		if (CrewChatter.crewLostRaceMessageToUse > 5)
		{
			CrewChatter.crewLostRaceMessageToUse = 1;
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_T",
			CarTierHelper.TierToString[zTier],
			"_LOSE_",
			CrewChatter.crewLostRaceMessageToUse
		}));
	}

	public static string GetCrewWonRace(int zTier, int zMember)
	{
	    return LocalizationManager.GetTranslation(string.Concat(new object[]
	    {
	        "TEXT_CHATTER_T",
	        CarTierHelper.TierToString[zTier],
	        "_C",
	        zMember,
	        "_WON"
	    }));
	}

	public static string GetBossPreRace(int zTier, int which)
	{
		CrewChatter.preBossRaceMessageToUse = which;
		if (CrewChatter.preBossRaceMessageToUse > 3)
		{
			CrewChatter.preBossRaceMessageToUse = 1;
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_T",
			CarTierHelper.TierToString[zTier],
			"_B",
			CrewChatter.preBossRaceMessageToUse,
			"_PRERACE"
		}));
	}

	public static string GetBossLoseRaceTitle(int zTier)
	{
        return LocalizationManager.GetTranslation("TEXT_CHATTER_T" + CarTierHelper.TierToString[zTier] + "_B_LOSE_TITLE");
	}

	public static string GetBossLoseRace(int zTier)
	{
		if (CrewChatter.bossLoseRaceMessageToUse == 0)
		{
			CrewChatter.bossLoseRaceMessageToUse = UnityEngine.Random.Range(0, 2);
		}
		CrewChatter.bossLoseRaceMessageToUse++;
		if (CrewChatter.bossLoseRaceMessageToUse > 3)
		{
			CrewChatter.bossLoseRaceMessageToUse = 1;
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_T",
			CarTierHelper.TierToString[zTier],
			"_B",
			CrewChatter.bossLoseRaceMessageToUse,
			"_LOSE"
		}));
	}

	public static string GetBossWinRace(int zTier, int which)
	{
		CrewChatter.bossWinRaceMessageToUse = which;
		if (CrewChatter.bossWinRaceMessageToUse > 3)
		{
			CrewChatter.bossWinRaceMessageToUse = 1;
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_T",
			CarTierHelper.TierToString[zTier],
			"_B",
			CrewChatter.bossWinRaceMessageToUse,
			"_WIN"
		}));
	}

	public static string GetCrewName(int zTier)
	{
		return "TEXT_CHATTER_T" + CarTierHelper.TierToString[zTier] + "_C_PRERACE_TITLE";
	}

	public static string GetTierName(int zTier)
	{
		return "TEXT_TIER_" + CarTierHelper.TierToString[zTier];
	}

	private static string getTranslationStringForBossDefeated(int zTier, int zMessage)
	{
		return string.Concat(new object[]
		{
			"TEXT_CHATTER_T",
			CarTierHelper.TierToString[zTier],
			"_END_",
			zMessage
		});
	}

	public static bool DoesBossDefeatedMessageExist(int zTier, int zMessage)
	{
        return zMessage<2;//LocalisationManager.DoesTranslationExist(CrewChatter.getTranslationStringForBossDefeated(zTier, zMessage));
	}

	public static string GetBossDefeatedMessage(int zTier, int zMessage)
	{
		string text = LocalizationManager.GetTranslation(CrewChatter.getTranslationStringForBossDefeated(zTier, zMessage));
		if (zTier == 3 && zMessage == 1)
		{
			string currentlySelectedCarDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
			CarInfo car = CarDatabase.Instance.GetCar(currentlySelectedCarDBKey);
            text = string.Format(text, LocalizationManager.GetTranslation(car.ShortName));
		}
		return text;
	}

	public static string GetMemberName(int zTier, int zMember)
    {
        if (zMember == 4)
            zMember = 3;
		return string.Concat(new object[]
		{
			"TEXT_NAME_CREWMEMBER_TIER",
			CarTierHelper.TierToString[zTier],
			"_",
			zMember
		});
	}

	public static string GetLeaderName(int zTier)
	{
		return "TEXT_NAME_CREWLEADER_TIER" + CarTierHelper.TierToString[zTier];
	}
}
