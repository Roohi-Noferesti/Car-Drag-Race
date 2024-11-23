using System;
using System.Collections;
using com.apptutti.sdk;
using UnityEngine;

public class TuttiAdProvider : MonoBehaviour,IAdProvider
{
    public event AdFinishedEventHandler AdFinished;
    
    private bool initialized = false;
    private UserInfo userInfo;
    private bool adsAllowed;
    
    private AdPrecachingState _rewardedAdPrecachingState;
    
    public string Name
    {
        get
        {
            return "TuttiAds";
        }
    }

    public bool IsInitialized
    {
        get
        {
            return initialized;
        }
    }

    public bool IsEnabled
    {
        get
        {
            return BasePlatform.ActivePlatform.IsAppTuttiBuild;
#if UNITY_IOS
            return false;
#endif
        }
    }
    
    public AdPrecachingState NativeBannerPrechachingState { get; private set; }

    public void Init()
    {
        if (!BasePlatform.ActivePlatform.IsAppTuttiBuild || IsInitialized)
            return;
        
        Debug.Log("tutti ads is going to be initialized.");

        ApptuttiSDK.GetInstance().Init(new IInitListener(
            (userInfo) => {
                //Save the userInfo, we need it after
                this.userInfo = userInfo;
                AfterInitialized();
            },
            (message) => Debug.Log(message)));
    }
    
    private void AfterInitialized()
    {
        //Save state which indicates ad is enabled/disabled,
        // we need it later.
        this.adsAllowed = ApptuttiSDK.GetInstance().IsAdsEnabled();
        //Query inventory after initialized ATSDK, lookup inventory
        //  and try to deliver items which hasn't been delivered last time.
        /*ApptuttiSDK.GetInstance().QueryInventory(userInfo.userId, new IQueryListener(
            (purchaseInfos) =>
            {
                    //Invoke consumePurchase method for all purchaseInfos
                    //  which contains parameters need to be pass to consumePurchase method.
                    foreach (PurchaseInfo purchaseInfo in purchaseInfos)
                {
                    ApptuttiSDK.GetInstance().ConsumePurchase(purchaseInfo, new IConsumeListener(
                        () =>
                        {
                                //So we marked this item as deliverd at AT Server, now we need to
                                //  deliver items to player in game.
                                if (purchaseInfo.productId.Equals("100golds"))
                            {
                                    //Increate number of golds to simulate delivering.
                                    IncreaseGolds(100, "query");
                                Debug.Log("[AfterQuery] Consume success, deliver one gold to player!");
                            }
                            else
                            {
                                Debug.Log("[AfterQuery] Consume success, deliver " + purchaseInfo.productId + " to player!");
                            }
                        },
                        (message) => Debug.Log(message)));
                }
            },
            (message) => Debug.Log(message)));*/

        HandleAds();
        this.initialized = true;
    }
    
    public void HandleAds()
    {
        if (adsAllowed)
        {
            // If ads is enabled, show banner ad
            //ApptuttiSDK.GetInstance().BannerAd();
        }
        else
        {
            // If ads is disabled, hide ads related buttons.
            Debug.Log("AppTutti: Ads is not allowed");
        }
    }
    
    protected virtual void OnAdFinished(AdFinishedEventArgs e)
    {
        var handler = AdFinished;
        if (handler != null) handler(this, e);
    }

    public void Show(string adunit, AdManager.AdSpace adSpace)
    {
        if (adSpace == AdManager.AdSpace.NativeBanner) {
            BannerAd();
        } else if (adSpace == AdManager.AdSpace.Default) {
            InterstitialAd();
        } else if (adSpace != AdManager.AdSpace.InterstitialBannerAfterPause && adSpace != AdManager.AdSpace.InterstitialBannerAfterRace) {
            //VideoAd();
        }
    }

    public void OnNativeBannerShown()
    {
    }

    public void Precache(string adunit,AdManager.AdSpace adSpace)
    {
        if (adSpace == AdManager.AdSpace.Default)
        {
            Show(adunit, adSpace);
        }
        else if(adSpace!= AdManager.AdSpace.InterstitialBannerAfterPause && adSpace!= AdManager.AdSpace.InterstitialBannerAfterRace)
        {
            RequestRewarded(adunit);
        }
    }

    public AdPrecachingState PrecachingStatus(string adunit, AdManager.AdSpace adSpace)
    {
        if (adSpace == AdManager.AdSpace.Default) {
            //return _interstitialAdPrecachingState;
        } else if (adSpace != AdManager.AdSpace.InterstitialBannerAfterPause &&
                 adSpace != AdManager.AdSpace.InterstitialBannerAfterRace) {
            return _rewardedAdPrecachingState;
        }

        return AdPrecachingState.NotPrecached;
    }

    public string GetRuntimeAdSpaceAppends()
    {
        return String.Empty;
    }

    public void Reset()
    {
    }
    
    private void Update()
    {
        //Invoke exit method before actually exit game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed.");

            if(ApptuttiSDK.GetInstance().ShouldUseSDKExit())
            {
                //Should use SDK.exit method to show exit window of SDK instead
                //  of Game.
                ApptuttiSDK.GetInstance().Exit();
            } else
            {
                //SDK doesn't required invocation of SDK.
                // Use exit logic of Game.
                Debug.Log("Game exit");
            }
        }
    }
    
    public void BannerAd()
    {
        ApptuttiSDK.GetInstance().BannerAd();
    }
    
    public void InterstitialAd()
    {
        // If ads is enabled, show interstitial ad
        ApptuttiSDK.GetInstance().InterstitialAd();
    }
    
    private void RequestRewarded(string adUnitId)
    {
        GTAdManager.Instance.ForceChangeAdProviderStatus("TuttiAds", AdManager.AdProviderStatus.Idle);
        _rewardedAdPrecachingState = AdPrecachingState.Unknown;
        //if (_rewardedAdPrecachingState== AdPrecachingState.Precaching || _rewardedAdPrecachingState == AdPrecachingState.Precached)
        //    return;
        
        // If ads is enabled, show rewarded video ad
        ApptuttiSDK.GetInstance().RewardedVideoAd(new IAdsListener(
            () => RewardedAd_OnAdLoaded(),
            () => HandleUserEarnedReward()));
        
        _rewardedAdPrecachingState = AdPrecachingState.Precaching;
        //Invoke("ResetPrecachingState", 1f);
    }
    
    private void RewardedAd_OnAdLoaded()
    {
        _rewardedAdPrecachingState = AdPrecachingState.Precached;
        UnityEngine.Debug.Log("AppTutti: rewarded video loaded");
    }
    
    public void HandleUserEarnedReward()
    {
        _rewardedAdPrecachingState = AdPrecachingState.NotPrecached;
        if (AdFinished != null)
        {
            AdFinished(this, new AdFinishedEventArgs(AdFinishedEventArgs.Status.Succeeded));
        }
    }

    private void ResetPrecachingState()
    {
        _rewardedAdPrecachingState = AdPrecachingState.NotPrecached;
    }

    public void RequestNativeBannerAd(string zoneID, Action OnNativeBannerFilled = null)
    {
        BannerAd();
    }

    public NativeBannerBase GetPrecachedNativeBanner()
    {
        return null;
    }

    public bool IsAdAllowed()
    {
        //return false; //TODO
        return adsAllowed;
    }
}
