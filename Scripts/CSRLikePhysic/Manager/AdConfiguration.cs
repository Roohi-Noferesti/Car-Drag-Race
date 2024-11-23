using AdSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AdConfiguration:ScriptableObject
{
	public float VideoForRewardInterstitialFrequency;

	public string FlurryInterstitialAdSpaceNameiOS;

	public string FlurryInterstitialAdSpaceNameAndroid;

	public List<VideoForRewardConfiguration> VideoForRewardConfiguration = new List<VideoForRewardConfiguration>();

    public List<OfferWallConfiguration> OfferWallConfiguration = new List<OfferWallConfiguration>();

    public List<PayPerEngagementConfiguration> PayPerEngagementConfiguration = new List<PayPerEngagementConfiguration>();

    public AdProviderConfigurationDictionary[] AdProviderConfiguration;


#if AdConfig
	[SerializeField] bool Copy;

	    private void OnValidate()
    {
		if (!Copy)
			return;
		Copy = false;


		adConfig.Configurations.Clear();

        foreach (var item in AdProviderConfiguration)
        {
	
			var cc = new AdSystem.AdProviderConfiguration
			{
				IsEnable = true,
				Name=item.Name,
			};
			cc.Android.Priority = item.Configuration.Android.Priority;
			cc.Android.Priority_Islamic = item.Configuration.Android.PriorityInIslamicCountries;
			cc.Apple.Priority = item.Configuration.Apple.Priority;
			cc.Apple.Priority_Islamic = item.Configuration.Apple.PriorityInIslamicCountries;

			cc.Android.Spaces = new List<AdSpaceID>();

            foreach (var sp in item.Configuration.Android.AdSpaces)
            {
				cc.Android.Spaces.Add(new AdSpaceID
                {
					PlacementID = sp.ID,
					
					AdSpace= sp.AdSpace,
				});
			}


			foreach (var sp in item.Configuration.Apple.AdSpaces)
			{
				cc.Apple.Spaces.Add(new AdSpaceID
				{
					PlacementID = sp.ID,
					AdSpace = sp.AdSpace,
				});
			}
			adConfig.Configurations.Add(cc);
		}

	}


#endif
	public AdConfig adConfig;




    public int HoursBeforeLastAdvert;

	public int HoursBeforeFirstAdvert;

	public int MaxAdvertsPerDay;

	public int MaxInterstitialBannerAfterRacePerDay;

	public int MaxInterstitialBannerAfterPausePerDay;

	public int RacesPerAd;
	public int RacesPerAdInIAPFocusedCountries;
	public int RacesPerAdInAdFocusedCountries;

	public int RacesPerAdSessionStartCounter;

	public int SessionsUntilAd;

	public int MaxAdsPerSession;

	public bool EnableBonusAdRewards;
	public bool UseHoursInsteadOfSessions;

	public List<int> FuelAdPromptShowThreshold = new List<int>
	{
		2,
		6
	};

	public List<int> FuelAdPromptResetThreshold = new List<int>
	{
		9,
		9
	};

	public int FuelAdPromptInterstitialRewardAmount = 1;

	public float MultiplayerCrossPromoFrequency = 0.1f;

	public int TapjoySuccessTimeout;
    public float NativeBannerFreqTime = 60;

    public void Initialise()
	{
		this.OfferWallConfiguration.ForEach(delegate(OfferWallConfiguration config)
		{
			config.Initialise();
	
		});
	}
}

[Serializable]
public class AdProviderConfigurationDictionary
{
    public string Name;
    public AdProviderConfiguration Configuration;
}

public static class AdProviderConfigurationDictionaryExtension
{
    public static AdProviderConfiguration Get(this AdProviderConfigurationDictionary[] dic, string name)
    {
        var keyVal =  dic.FirstOrDefault(d => d.Name == name);
        return keyVal != null ? keyVal.Configuration : null;
    }
}
