using UnityEngine;

public class CarsDatabase : ConfigurationAssetLoader
{
	public CarsConfiguration Configuration
	{
		get;
		private set;
	}

	public CarsDatabase() : base(GTAssetTypes.configuration_file, "CarsConfiguration")
	{
        this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (CarsConfiguration) scriptableObject;//JsonConverter.DeserializeObject<CarsConfiguration>(assetDataString);
	}

    public eCarTier GetTierUnlockAtLevel(int level)
    {
        if (Configuration.Tier1_UnlockLevel == level)
        {
            return eCarTier.TIER_1;
        }

        if (Configuration.Tier2_UnlockLevel == level)
        {
            return eCarTier.TIER_2;
        }

        if (Configuration.Tier3_UnlockLevel == level)
        {
            return eCarTier.TIER_3;
        }

        if (Configuration.Tier4_UnlockLevel == level)
        {
            return eCarTier.TIER_4;
        }

        if (Configuration.Tier5_UnlockLevel == level)
        {
            return eCarTier.TIER_5;
        }

        return eCarTier.BASE_EVENT_TIER;
    }
}
