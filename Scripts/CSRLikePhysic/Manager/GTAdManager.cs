using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdSystem;
using AdSystem.Enums;
using KingKodeStudio;
using Metrics;
using UnityEngine;
using Random = UnityEngine.Random;


public class GTAdManager : AdSystem.AdManager
{
    private const int SecondsPerSession = 86400;

    private const int NoOfHoursBetweenAdResets = 24;

    //private CrossPromoOverrider crossPromoAdOverrider = new CrossPromoOverrider();

    private int RacesSinceLastAd;

    private int RacesSinceLastInterstitialBannerAd;

    private DateTime SessionStartTime = DateTime.MinValue;

    public int AdsShownThisSession;

    public int InterstitialBannerAfterRaceShownThisSession;
    public int InterstitialBannerAfterPauseShownThisSession;

    public bool TriggerAdInWorkshop { get;set; }
    private bool TriggerAdInWorkshop2;

    public static GTAdManager Instance
    {
        get;
        private set;
    }
    public override bool IsIslamic => BasePlatform.ActivePlatform.InsideIslamicCountry;
    public override bool InsideCountry => BasePlatform.ActivePlatform.InsideCountry;
    public override bool IsAppTuttiBuild => BuildType.IsAppTuttiBuild;
    public override bool IsUnder13 => AgeVerificationManager.Instance.IsUnder13;

    public override bool ShouldHideAdInterface
    {
        get
        {

            if (BuildType.IsVasBuild)
            {
                return true;
            }
            else if (BuildType.IsAppTuttiBuild)
            {
#if APPTUTTI_ON
				    var tuttiProvider = (TuttiAdProvider) Instance.GetAdProviderByName("TuttiAds").GetProvider();
				    return !tuttiProvider.IsAdAllowed();
#else
                return true;
#endif
            }
            else
            {
                return false;
            }
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
    }

    public bool CanShowAdForExtraReward()
    {
        Debug.Log("CanShowAdForExtraReward");
        return GameDatabase.Instance.AdConfiguration.EnableBonusAdRewards && !ShouldHideAdInterface;
    }

    public override void Initialise()
    {
        var a = Resources.Load<AdConfig>("Ad Config");
        base.Config = a;
        base.Initialise();
        ApplicationManager.DidBecomeActiveEvent += this.AddASession;
    }

    public bool CheckShouldShowAdsBasedOnSessions()
    {
        Debug.Log("CheckShouldShowAdsBasedOnSessions");
        AdConfiguration adConfiguration = GameDatabase.Instance.AdConfiguration;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile.SessionCounter >= adConfiguration.MaxAdsPerSession)
        {
            return false;
        }
        if (activeProfile.LastAdvertDate.AddHours(24.0) < GTDateTime.Now)
        {
            activeProfile.AdCount = 0;
            activeProfile.LastAdvertDate = GTDateTime.Now;
        }
        if (activeProfile.AdCount > adConfiguration.MaxAdvertsPerDay)
        {
            return false;
        }
        activeProfile.AdCount++;
        return true;
    }

    public bool CheckSessionShouldShowAd()
    {
        Debug.Log("CheckSessionShouldShowAd");
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //Prevent ad to be shown for new users until HoursBeforeFirstAdvert passed ( normally 24 hours )
        if (GTDateTime.Now < activeProfile.UserStartedPlaying.AddHours((double)GameDatabase.Instance.AdConfiguration.HoursBeforeFirstAdvert))
        {
            Debug.Log("Baddd1=>");
            return false;
        }

        //some time we need to disable ads for new users after some period of time ( Just for test , HoursBeforeLastAdvert  is usually -1 )
        if (GameDatabase.Instance.AdConfiguration.HoursBeforeLastAdvert > 0 && GTDateTime.Now > activeProfile.UserStartedPlaying.AddHours((double)GameDatabase.Instance.AdConfiguration.HoursBeforeLastAdvert))
        {
            Debug.Log("Baddd2=>");
            return false;
        }
        //if player is playing the game more than a day in one session , reset all counters
        if (this.SessionStartTime.AddSeconds(86400.0) < GTDateTime.Now)
        {
            this.SessionStartTime = GTDateTime.Now;
            this.AdsShownThisSession = 0;
            this.RacesSinceLastAd = GameDatabase.Instance.AdConfiguration.RacesPerAdSessionStartCounter;
            this.RacesSinceLastInterstitialBannerAd = GameDatabase.Instance.AdConfiguration.RacesPerAdSessionStartCounter;
        }
        bool allowAdToBeShownForThisSession = this.AdsShownThisSession < GameDatabase.Instance.AdConfiguration.MaxAdvertsPerDay;

        int racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAd;
        if (BasePlatform.ActivePlatform.InsideIAPFocusedCountry)
            racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAdInIAPFocusedCountries;
        if (BasePlatform.ActivePlatform.InsideAdFocusedCountry)
            racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAdInAdFocusedCountries;
        //Debug.LogError("racesPerAd: " + racesPerAd.ToString() + " | " + BasePlatform.ActivePlatform.InsideIAPFocusedCountry + "," + BasePlatform.ActivePlatform.InsideAdFocusedCountry);

        bool allowAdToBeShownForRacesPlayed = this.RacesSinceLastAd >= racesPerAd;
        Debug.Log("Baddd3=>"+ allowAdToBeShownForThisSession+"//"+ allowAdToBeShownForRacesPlayed);
        return allowAdToBeShownForThisSession && allowAdToBeShownForRacesPlayed;
    }
    //Note this method is dependent on calling CheckSessionShouldShowAd Method because SessionStartTime will be changed after calling CheckSessionShouldShowAd
    private bool CheckSessionShouldShowInterstitialBanner(AdSpace adSpace)
    {

        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //Prevent ad to be shown for new users until HoursBeforeFirstAdvert passed ( normally 24 hours )
        if (GTDateTime.Now < activeProfile.UserStartedPlaying.AddHours((double)GameDatabase.Instance.AdConfiguration.HoursBeforeFirstAdvert))
        {
            return false;
        }

        //some time we need to disable ads for new users after some period of time ( Just for test , HoursBeforeLastAdvert  is usually -1 )
        if (GameDatabase.Instance.AdConfiguration.HoursBeforeLastAdvert > 0 && GTDateTime.Now > activeProfile.UserStartedPlaying.AddHours((double)GameDatabase.Instance.AdConfiguration.HoursBeforeLastAdvert))
        {
            return false;
        }

        int racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAd;
        if (BasePlatform.ActivePlatform.InsideIAPFocusedCountry)
            racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAdInIAPFocusedCountries;
        if (BasePlatform.ActivePlatform.InsideAdFocusedCountry)
            racesPerAd = GameDatabase.Instance.AdConfiguration.RacesPerAdInAdFocusedCountries;
        //Debug.LogError("racesPerAd: " + racesPerAd.ToString() + " | " + BasePlatform.ActivePlatform.InsideIAPFocusedCountry + "," + BasePlatform.ActivePlatform.InsideAdFocusedCountry);

        bool allowAdToBeShownForRacesPlayed = adSpace == AdSpace.InterstitialBannerAfterPause || this.RacesSinceLastInterstitialBannerAd >= racesPerAd;
        if (allowAdToBeShownForRacesPlayed)
        {
            if (adSpace == AdSpace.InterstitialBannerAfterRace)
            {
                return this.InterstitialBannerAfterRaceShownThisSession < GameDatabase.Instance.AdConfiguration.MaxInterstitialBannerAfterRacePerDay;
            }
            else if (adSpace == AdSpace.InterstitialBannerAfterPause)
            {
                return this.InterstitialBannerAfterPauseShownThisSession < GameDatabase.Instance.AdConfiguration.MaxInterstitialBannerAfterPausePerDay;
            }
        }
        else
        {
            return false;
        }

        return false;
    }
    public bool UserHasPaidSomethingToDisableAds()
    {
        return PlayerProfileManager.Instance.ActiveProfile.HasPaidForSomething;
    }
    public bool isShowing { get; private set; }
    private void AddASession()
    {
        PlayerProfileManager.Instance.ActiveProfile.SessionCounter++;
        //    Debug.Log("AddASession" + PlayerProfileManager.Instance.ActiveProfile.SessionCounter);
        var adspace = AdSpace.InterstitialBannerAfterPause;
        if (ScreenManager.Instance.CurrentScreen != ScreenID.Shop &&
            SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Race)
            AutoShowAd(adspace,3);
    }

    public void AutoShowAd(AdSpace adSpace, int TryToLoad, Action OnFinish = null, Action<string> OnShowError = null, Action<string> OnLoadError = null, Action OnClose = null, float WaitForBest = 3, float TimeOut = 10)
    {
        TryToLoad = Mathf.Clamp(TryToLoad, 2,10);
        Debug.Log("AutoShowAd " + adSpace);
        try
        {
            isShowing=true; 
            Show2(adSpace,
                OnFinish: (space) =>
                {
                    Debug.Log("AutoShowAd OnFinish " + adSpace);
                    OnFinish?.Invoke(); 
                    isShowing = false;
                    Prepare(adSpace, false, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
                },
                OnClose: (space) =>
                {
                    Debug.Log("AutoShowAd OnClose " + adSpace);
                    OnClose?.Invoke();
                    isShowing = false;
                    Prepare(adSpace, false, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
                },
                OnShowError: (space, message) =>
                {
                    Debug.Log("AutoShowAd OnShowError " + adSpace+"//"+ message);
                    OnShowError?.Invoke(message);
                    isShowing = false;
                    Prepare(adSpace, false, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
                },
                OnNotLoaded: (space,mes) =>
                {
                    Debug.Log("AutoShowAd OnNotLoaded " + adSpace+"//"+ mes);
                    isShowing = false;
                    Prepare(adSpace, true, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
                });
        }
        catch (Exception ex)
        {
            OnShowError?.Invoke(ex.Message);
            isShowing = false;
            Prepare(adSpace, false, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
        }
    }

    void Prepare(AdSpace adSpace,bool ShowOnLoad, int TryToLoad, Action OnFinish = null, Action<string> OnShowError = null, Action<string> OnLoadError = null, Action OnClose = null, float WaitForBest = 3, float TimeOut = 10)
    {
        TryToLoad--;
        Debug.Log("Prepare"+ adSpace+ "//TryToLoad:" + TryToLoad);
        
        Prepare(adSpace,
        OnLoadBest: (_space, message) =>
        {
            Debug.Log("Prepare OnLoadBest" + _space + "//message:" + message);
            if (ShowOnLoad)
                AutoShowAd(_space, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
        },
        OnError: (_space, message) =>
        {
            Debug.Log("Prepare OnError" + _space + "//message:" + message+ "//TryToLoad" + TryToLoad);
            if (TryToLoad<=0)
            {
                OnLoadError?.Invoke(message);
            }
            else
            {
                Prepare(adSpace, ShowOnLoad, TryToLoad, OnFinish, OnShowError, OnLoadError, OnClose, WaitForBest, TimeOut);
            }
        }, WaitForBest, TimeOut);
    }

    public bool ShouldTriggerAdInWorkshop()
    {
        //Debug.Log(TriggerAdInWorkshop);
        return this.TriggerAdInWorkshop;//We comment this line because we show first advert after one day later player start playing the game\\ && PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(4102);
    }

    public bool ShouldTriggerInterstitialBannerInWorkshop()
    {
        //Debug.Log(TriggerAdInWorkshop);
        return this.TriggerAdInWorkshop2 && CanShowInterstitialBannerAdvert(AdSpace.InterstitialBannerAfterRace);//We comment this line because we show first advert after one day later player start playing the game\\ && PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(4102);
    }

    private bool ShouldShowVideoForReward()
    {
        return GameDatabase.Instance.AdConfiguration != null &&
               GameDatabase.Instance.Ad.IsEnabled(VideoForRewardConfiguration.eRewardID.InterstitialForCash) &&
               BasePlatform.ActivePlatform.GetReachability() != BasePlatform.eReachability.OFFLINE &&
               Random.value <= GameDatabase.Instance.AdConfiguration.VideoForRewardInterstitialFrequency &&
               !ShouldHideAdInterface;
    }

    private bool CanShowAdvert()
    {
        this.TriggerAdInWorkshop = false;

        if (this.UserHasPaidSomethingToDisableAds())
        {
            return false;
        }
        if (GameDatabase.Instance.AdConfiguration.UseHoursInsteadOfSessions)
        {
            if (!this.CheckSessionShouldShowAd())
            {
                this.RacesSinceLastAd++;
                return false;
            }

            return true;

        }
        else if (!this.CheckShouldShowAdsBasedOnSessions())
        {
            return false;
        }
        return true;
    }


    private bool CanShowInterstitialBannerAdvert(AdSpace adSpace)
    {
        if (m_AdProviderDict.Values.Count(a => a.IsEnabled && !string.IsNullOrEmpty(a.GetAdUnitFromAdSpace(adSpace))) == 0)
        {
            return false;
        }

        this.TriggerAdInWorkshop2 = false;
        if (this.UserHasPaidSomethingToDisableAds())
        {
            return false;
        }

        if (!this.CheckSessionShouldShowInterstitialBanner(adSpace))
        {
            RacesSinceLastInterstitialBannerAd++;
            return false;
        }

        return true;
    }

    //public void TriggerAdvert(AdSpace adSpace)
    //{
    //    if (adSpace != AdSpace.Default && adSpace != AdSpace.InterstitialBannerAfterRace && adSpace != AdSpace.InterstitialBannerAfterPause)
    //    {
    //        ShowAd(adSpace);
    //        return;
    //    }

    //    if (adSpace == AdSpace.InterstitialBannerAfterRace || adSpace == AdSpace.InterstitialBannerAfterPause)
    //    {
    //        if (!CanShowInterstitialBannerAdvert(adSpace))
    //        {
    //            return;
    //        }
    //    }
    //    if (adSpace == AdSpace.Default)
    //    {
    //        if (!CanShowAdvert())
    //        {
    //            return;
    //        }
    //    }

    //    if (this.ShouldShowVideoForReward())
    //    {
    //        VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.InterstitialForCash);
    //        return;
    //    }

    //    ShowAd(adSpace);
    //}

    //protected void OnAdvertTriggered(string providerName, AdSpace adSpace)
    //{
    //    Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
    //    {
    //        {
    //            Parameters.AdNwk,
    //            providerName
    //        },
    //        {
    //            Parameters.Result,
    //            "SUCCESS"
    //        },
    //        {
    //            Parameters.AdSpace,
    //            adSpace.ToString()
    //        },
    //        {
    //            Parameters.AdRaces,
    //            this.RacesSinceLastAd.ToString()
    //        },
    //        {
    //            Parameters.AdsPerSession,
    //            (this.AdsShownThisSession + 1).ToString()
    //        }
    //    };
    //    Log.AnEvent(Events.AdStaticInterstitialDisplayed, data);
    //    //bool flag = this.crossPromoAdOverrider.OnAdvertTriggered(providerName, adSpace);
    //    if (adSpace == AdSpace.Default)// || flag)
    //    {
    //        this.RacesSinceLastAd = 0;
    //    }

    //    if (adSpace == AdSpace.InterstitialBannerAfterRace)
    //    {
    //        RacesSinceLastInterstitialBannerAd = 0;
    //        this.InterstitialBannerAfterRaceShownThisSession++;
    //    }
    //    else if (adSpace == AdSpace.InterstitialBannerAfterPause)
    //    {
    //        this.InterstitialBannerAfterPauseShownThisSession++;
    //    }
    //    this.AdsShownThisSession++;
    //    this.TriggerAdInWorkshop = false;
    //}

    //protected void OnTriggerAdvertFailed(List<string> providerNames, string failureCase, AdSpace adSpace)
    //{
    //    Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
    //    {
    //        {
    //            Parameters.AdNwk,
    //            string.Join(":", providerNames.ToArray())
    //        },
    //        {
    //            Parameters.Result,
    //            failureCase
    //        },
    //        {
    //            Parameters.AdSpace,
    //            adSpace.ToString()
    //        },
    //        {
    //            Parameters.AdRaces,
    //            this.RacesSinceLastAd.ToString()
    //        },
    //        {
    //            Parameters.AdsPerSession,
    //            this.AdsShownThisSession.ToString()
    //        }
    //    };
    //    Log.AnEvent(Events.AdStaticInterstitialDisplayed, data);
    //}

    //protected  List<AdProvider> GetTriggerableProvidersInPriorityOrder(AdSpace adSpace)
    //{
    //    return base.GetTriggerableProvidersInPriorityOrder(adSpace);
    //}

    //public bool IsAdProviderAvailable(string providerName)
    //{
    //    var provideList = from p in this.m_AdProviderDict.Values
    //                      where p.Name == providerName
    //                      select p into x
    //                      orderby x.Priority
    //                      select x;

    //    return (provideList.Count() > 0);
    //}

    //private AdProvider GetAdProviderByName(string providerName)
    //{
    //    var provideList = from p in this.m_AdProviderDict.Values
    //                      where p.Name == providerName
    //                      select p into x
    //                      orderby x.Priority
    //                      select x;
    //    return provideList.FirstOrDefault();
    //}
}
