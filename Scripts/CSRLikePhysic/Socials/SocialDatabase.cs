using System;
using UnityEngine;

public class SocialDatabase : ConfigurationAssetLoader
{
	public SocialConfiguration Configuration
	{
		get;
		private set;
	}

	public SocialDatabase() : base(GTAssetTypes.configuration_file, "SocialConfiguration")
	{
		this.Configuration = null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<SocialConfiguration>(assetDataString);
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (SocialConfiguration) scriptableObject;
    }

	public int GetCashRewardForTwitter()
	{
		switch (RaceEventQuery.Instance.getHighestUnlockedClass())
		{
		case eCarTier.TIER_2:
			return this.Configuration.TwitterCashRewardT2;
		case eCarTier.TIER_3:
			return this.Configuration.TwitterCashRewardT3;
		case eCarTier.TIER_4:
			return this.Configuration.TwitterCashRewardT4;
		case eCarTier.TIER_5:
			return this.Configuration.TwitterCashRewardT5;
		default:
			return this.Configuration.TwitterCashRewardT1;
		}
	}


}
