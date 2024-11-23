using System.Linq;
using UnityEngine;

public class AdDatabase : ConfigurationAssetLoader
{
	public AdConfiguration Configuration
	{
		get;
		private set;
	}

	public AdDatabase() : base(GTAssetTypes.configuration_file, "AdConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (AdConfiguration) scriptableObject;//JsonConverter.DeserializeObject<AdConfiguration>(assetDataString);
		this.Configuration.Initialise();
	}

	public bool IsEnabled(VideoForRewardConfiguration.eRewardID rewardID)
	{
		VideoForRewardConfiguration configuration = this.GetConfiguration(rewardID);
		return configuration != null && configuration.Enabled;
	}

	public VideoForRewardConfiguration GetConfiguration(VideoForRewardConfiguration.eRewardID rewardID)
	{
		if (this.Configuration == null)
		{
			return null;
		}
	    return
	        this.Configuration.VideoForRewardConfiguration.Find(p => p.RewardID == rewardID);
	}

	public OfferWallConfiguration.eProvider GetOfferWallProvider()
	{
		OfferWallConfiguration offerWallConfiguration = this.GetOfferWallConfiguration();
		if (offerWallConfiguration == null)
		{
			return OfferWallConfiguration.eProvider.None;
		}
		return offerWallConfiguration.ProviderEnum;
	}

	public OfferWallConfiguration GetOfferWallConfiguration()
	{
		if (this.Configuration == null)
		{
			return null;
		}
		return this.Configuration.OfferWallConfiguration.Find((OfferWallConfiguration p) => p.AppStoreEnum == BasePlatform.ActivePlatform.GetTargetAppStore());
	}

	public PPEAppStoreConfiguration GetPPEConfiguration(PayPerEngagementConfiguration.eEngagement engagement)
	{
		if (this.Configuration == null)
		{
			return null;
		}
		PayPerEngagementConfiguration payPerEngagementConfiguration = this.Configuration.PayPerEngagementConfiguration.Find((PayPerEngagementConfiguration p) => p.AppStoreEnum == BasePlatform.ActivePlatform.GetTargetAppStore());
		if (payPerEngagementConfiguration == null)
		{
			return null;
		}
		return payPerEngagementConfiguration.Engagements.Find((PPEAppStoreConfiguration p) => p.EngagementEnum == engagement);
	}

	public PayPerEngagementConfiguration.eProvider GetPPEProvider(PayPerEngagementConfiguration.eEngagement engagement)
	{
		PPEAppStoreConfiguration pPEConfiguration = this.GetPPEConfiguration(engagement);
		if (pPEConfiguration == null)
		{
			return PayPerEngagementConfiguration.eProvider.None;
		}
		return pPEConfiguration.ProviderEnum;
	}

	public string GetPPEEngagementID(PayPerEngagementConfiguration.eEngagement engagement)
	{
		PPEAppStoreConfiguration pPEConfiguration = this.GetPPEConfiguration(engagement);
		if (pPEConfiguration == null)
		{
			return string.Empty;
		}
		return pPEConfiguration.ID;
	}

	public string GetAdSpace(string adProvider, string adSpace)
	{
	    var cfg = Configuration.AdProviderConfiguration.Get(adProvider);
        if (cfg==null)
		{
			return string.Empty;
		}
        string adUnitIDForAdSpace = cfg.GetAdUnitIDForAdSpace(adSpace);
		if (string.IsNullOrEmpty(adUnitIDForAdSpace))
		{
			return string.Empty;
		}
		return adUnitIDForAdSpace;
	}

    public int GetRewardAmount(VideoForRewardConfiguration.eRewardID watchToSkipDeliveryTime, eCarTier arrivalTier)
    {
        var videoRewardConfig = GameDatabase.Instance.AdConfiguration.VideoForRewardConfiguration.FirstOrDefault(
            i => i.RewardID ==watchToSkipDeliveryTime);

        return videoRewardConfig != null ? videoRewardConfig.RewardAmountT1 : 0;
    }
}
