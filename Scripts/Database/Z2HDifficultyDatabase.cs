using UnityEngine;

public class Z2HDifficultyDatabase : ConfigurationAssetLoader
{
    public Z2HDifficultyConfiguration Configuration;
    public Z2HDifficultyDatabase()
        : base(GTAssetTypes.configuration_file, "Z2HDifficultyConfiguration")
	{
		this.Configuration = null;
	}

    public Z2HDifficultyDatabase(GTAssetTypes assetType, string assetID) : base(assetType, assetID)
    {
    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        Configuration = (Z2HDifficultyConfiguration) scriptableObject;
    }

    public int GetPPDelta(AutoDifficulty.DifficultyRating difficultyRating,eCarTier carTier)
    {
        var winCountAfterUpgrade = PlayerProfileManager.Instance.ActiveProfile.WinCountAfterUpgrade;
        var difficultyPpSetting = Configuration[difficultyRating];

        var t = Mathf.InverseLerp(Configuration.MinPPStep, Configuration.MaxPPStep, winCountAfterUpgrade);

        if (difficultyPpSetting != null)
        {
            return (int)Mathf.Lerp(difficultyPpSetting.GetMinDeltaForTier(carTier), difficultyPpSetting.GetMaxDeltaForTier(carTier), t);
        }
        return 0;
    }

    public int GetStaticPP(AutoDifficulty.DifficultyRating difficultyRating, eCarTier carTier)
    {
        var difficultyPpSetting = Configuration.GetStaticDifficultiy(difficultyRating);

        if (difficultyPpSetting != null)
        {
            return Random.Range(difficultyPpSetting.GetMinDeltaForTier(carTier), difficultyPpSetting.GetMaxDeltaForTier(carTier));
        }
        return 0;
    }


    public AutoDifficulty.DifficultyRating GetDifficulty(int ppDif,eCarTier carTier)
    {
        var difficultyPpSetting = Configuration.GetDifficultySetting(ppDif, carTier);

        if (difficultyPpSetting != null)
        {
            return difficultyPpSetting.DifficultyRating;
        }
        GTDebug.Log(GTLogChannel.GameDatabase,"no config found with pp : "+ppDif+" . Setting difficulty to challenging");
        return AutoDifficulty.DifficultyRating.Challenging;
    }
}
