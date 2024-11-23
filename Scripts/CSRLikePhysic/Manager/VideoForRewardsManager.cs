using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdSystem.Enums;
using I2.Loc;
using Metrics;
using UnityEngine;

public class VideoForRewardsManager : MonoBehaviour
{
    const float WaitForBest = 2;
    const float AdTimeOut = 12;
    public enum eResult
    {
        Invalid,
        RewardGiven,
        FailedToPrecache,
        FailedToDisplay,
        UserFailedToFinish,
        NotWatching,
        VideoCapHit
    }

    private enum VideoState
    {
        Idle,
        Precaching,
        Showing
    }

    public delegate void OnCompletedCallback(eResult result);

    private VideoState m_State;

    private string m_AdProvider = string.Empty;

    private VideoForRewardConfiguration m_Cfg;

    private int m_PrecacheRetries;
    private bool m_waiting;

    private ExtraRewardResult m_ExtraRewardResult;


    public static VideoForRewardsManager Instance
    {
        get;
        private set;
    }

    public void Awake()
    {
        Instance = this;
    }

    public int GetVideoForFuel24HCap(VideoForRewardConfiguration cfg)
    {
        if (cfg == null || !cfg.Enabled)
        {
            return 0;
        }
        return cfg.Video24HCap;
    }

    public bool BloggerOrAgentForVideoAdPopup(VideoForRewardConfiguration.eRewardID idenum)
    {
        return idenum == VideoForRewardConfiguration.eRewardID.WatchForStreakRescue;
    }

    public string GraphicForVideoAdPopup(VideoForRewardConfiguration.eRewardID idenum)
    {
        return (!this.BloggerOrAgentForVideoAdPopup(idenum)) ? PopUpManager.Instance.graphics_agentPrefab : PopUpManager.Instance.graphics_bloggerPrefab;
    }

    public string CaptionForVideoAdPopup(VideoForRewardConfiguration.eRewardID idenum)
    {
        return (!this.BloggerOrAgentForVideoAdPopup(idenum)) ? "TEXT_NAME_AGENT" : "TEXT_NAME_CONSUMABLES_PRAGENT";
    }

    public PopUp GetPreAdPromptPopup(VideoForRewardConfiguration.eRewardID rewardID)
    {
        VideoForRewardConfiguration cfg = this.GetConfiguration(rewardID);
        string bundledGraphicPath = this.GraphicForVideoAdPopup(cfg.RewardID);
        string imageCaption = this.CaptionForVideoAdPopup(cfg.RewardID);
        return new PopUp
        {
            ID = PopUpID.PreAdPromptPopup,
            Title = cfg.PreAdPromptTitleTextID,
            BodyText = this.GetTranslatedBodyText(cfg, cfg.PreAdPromptBodyTextID, false),
            IsBig = true,
            BodyAlreadyTranslated = true,
            CancelAction = delegate
            {
                this.OnCancelWatchAction(cfg);
            },
            ConfirmAction = delegate
            {
                this.OnConfirmWatchAction(cfg);
            },
            CancelText = this.GetTranslatedCancelButtonText(cfg, cfg.PreAdPromptCancelButtonTextID),
            CancelTextAlreadyTranslated = true,
            ConfirmText = this.GetTranslatedConfirmButtonText(cfg, cfg.PreAdPromptOkButtonTextID),
            ConfirmTextAlreadyTranslated = true,
            GraphicPath = bundledGraphicPath,
            ImageCaption = imageCaption
        };
    }

    public bool VideoAdCapHit(VideoForRewardConfiguration.eRewardID rewardID)
    {
        VideoForRewardConfiguration configuration = this.GetConfiguration(rewardID);
        return !PlayerProfileManager.Instance.ActiveProfile.CanWatchVideoForFuel(this.GetVideoForFuel24HCap(configuration)) && configuration.EnableVideo24HCap;
    }
    public static bool DisableLimitForAd { get; set; }
    public void StartFlow(VideoForRewardConfiguration.eRewardID rewardID)
    {
        VideoForRewardConfiguration cfg = this.GetConfiguration(rewardID);

        if (VideoAdCapHit(rewardID) && m_ExtraRewardResult != null &&
            (rewardID == VideoForRewardConfiguration.eRewardID.VideoForDoubledPrize ||
            rewardID == VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize))
        {
            m_ExtraRewardResult.VideoFailed(cfg.RewardType);
            return;
        }
        if (!DisableLimitForAd)
        {
            if (cfg.EnableVideo24HCap && !PlayerProfileManager.Instance.ActiveProfile.CanWatchVideoForFuel(this.GetVideoForFuel24HCap(cfg)))
            {

                PopUp popup = new PopUp
                {
                    ID = PopUpID.AdResult,
                    Title = cfg.VideoCapHitPromptTitleTextID,
                    BodyText = string.Format(LocalizationManager.GetTranslation(cfg.VideoCapHitPromptBodyTextID), cfg.Video24HCap),// this.GetTranslatedBodyText(cfg, cfg.VideoCapHitPromptBodyTextID),
                    IsBig = true,
                    BodyAlreadyTranslated = true,
                    ConfirmAction = delegate
                    {
                        this.OnVideoCapHitAction(cfg);
                    },
                    ConfirmText = cfg.VideoCapHitPromptOkButtonTextID,
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                    ImageCaption = "TEXT_NAME_AGENT"
                };
                PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
                return;
            }
        }

        if (cfg.EnablePreAdPrompt)
        {
            PopUpManager.Instance.TryShowPopUp(this.GetPreAdPromptPopup(rewardID), PopUpManager.ePriority.Default, null);
        }
        else
        {
            this.OnConfirmWatchAction(cfg);
        }
    }

    private void OnCancelWatchAction(VideoForRewardConfiguration cfg)
    {
        this.OnResultConfirmed(cfg, eResult.NotWatching);
    }

    private void OnVideoCapHitAction(VideoForRewardConfiguration cfg)
    {
        this.OnResultConfirmed(cfg, eResult.VideoCapHit);
    }

    private void OnConfirmWatchAction(VideoForRewardConfiguration cfg)
    {
        Debug.Log("OnConfirmWatchAction VideoForRewardConfiguration");
        this.m_PrecacheRetries = 0;
        if (cfg.GetAdSpace() == AdSpace.None)
        {
            this.DisplayResultPopup(cfg, eResult.FailedToDisplay);
            return;
        }
        this.m_Cfg = cfg;

        Invoke(nameof(AttemptPrecache), 0.2f);
    }


    private async void AttemptPrecache()
    {

        AdSpace adSpace = this.m_Cfg.GetAdSpace();
        m_State = VideoState.Idle;
        if (adSpace != AdSpace.None)
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUP_TITLE_PLEASE_WAIT_SPINNER",
                IsWaitSpinner = true,
                ID = PopUpID.WaitSpinner
            };
            var pupres = PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            Debug.Log("TryShowPopUp WAIT_SPINNER" + "//" + adSpace + "//" + pupres);

            m_State = VideoState.Precaching;

           await Task.Delay(200);


            GTAdManager.Instance.AutoShowAd(adSpace,
            TryToLoad:3,
            OnFinish: () =>
            {
                Debug.Log("ShowAD OnFinish" + adSpace);
                m_State = VideoState.Idle;
                PopUpManager.Instance.KillPopUp();
                this.DisplayResultPopup(this.m_Cfg, eResult.RewardGiven);
                Log.AnEvent(Events.RewardedAdWatched);
                if (!PlayerProfileManager.Instance.ActiveProfile.FirstRewardedAdWatched)
                {
                    Log.AnEvent(Events.FirstRewardedAdWatched);
                    PlayerProfileManager.Instance.ActiveProfile.FirstRewardedAdWatched = true;
                    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
                }
            },
            OnShowError: (mes) =>
            {
                Debug.Log("ShowAD OnError" + adSpace+"//"+ mes);
                m_State = VideoState.Idle;
                PopUpManager.Instance.KillPopUp();
                GTDebug.Log(GTLogChannel.Adverts, "can't show video for reward");
                this.DisplayResultPopup(this.m_Cfg, eResult.FailedToDisplay);
                GTAdManager.Instance.Cancel(adSpace);
            },
            OnLoadError: (mes) =>
            {
                Debug.Log("OnError" + mes);
                m_State = VideoState.Idle;
                PopUpManager.Instance.KillPopUp();
                this.SendV4RMetricsEvent(this.m_Cfg, Events.AdFailedPrecacheV4R, this.m_AdProvider);
                this.DisplayResultPopup(this.m_Cfg, eResult.FailedToPrecache);
            },
            OnClose: () =>
            {
                Debug.Log("ShowAD OnClose" + adSpace);
                PopUpManager.Instance.KillPopUp();
                m_State = VideoState.Idle;
                this.DisplayResultPopup(this.m_Cfg, eResult.UserFailedToFinish);
            }, WaitForBest, AdTimeOut);

        }
    }






    //private void Update()
    //{
    //    if (m_waiting)
    //        return;
    //    if (this.m_State == VideoState.Precaching)
    //    {
    //        AdSpace adSpace = this.m_Cfg.GetAdSpace();
    //        if (GTAdManager.Instance.isPrepared(adSpace))
    //        {
    //            GTDebug.Log(GTLogChannel.Adverts, "trying to show video");
    //            MenuAudio.Instance.MuteAudio();
    //            PopUpManager.Instance.KillPopUp();
    //            this.SendV4RMetricsEvent(this.m_Cfg, Events.AdUserStartV4R, this.m_AdProvider);
    //            this.m_State = VideoState.Showing;
    //        }
    //        else if (!GTAdManager.Instance.isPreparing(adSpace))
    //        {
    //            if (this.m_PrecacheRetries < 1)
    //            {
    //                this.m_PrecacheRetries++;
    //                this.AttemptPrecache();
    //                if (this.m_State == VideoState.Precaching)
    //                {
    //                    return;
    //                }
    //            }
    //            PopUpManager.Instance.KillPopUp();
    //            this.m_State = VideoState.Idle;
    //            this.SendV4RMetricsEvent(this.m_Cfg, Events.AdFailedPrecacheV4R, this.m_AdProvider);
    //            this.DisplayResultPopup(this.m_Cfg, eResult.FailedToDisplay);
    //        }
    //    }
    //}

    //private void OnVideoFinished(AdFinishedEventArgs args, object userData)
    //{
    //	MenuAudio.Instance.UnMuteAudio();
    //	this.m_State = VideoState.Idle;
    //	VideoForRewardConfiguration videoForRewardConfiguration = userData as VideoForRewardConfiguration;
    //	if (videoForRewardConfiguration == null)
    //	{
    //		this.DisplayResultPopup(videoForRewardConfiguration, eResult.FailedToDisplay);
    //		return;
    //	}
    //	if (args.AdStatus == AdFinishedEventArgs.Status.Succeeded)
    //	{
    //		this.DisplayResultPopup(videoForRewardConfiguration, eResult.RewardGiven);
    //		Log.AnEvent(Events.RewardedAdWatched);
    //		if (!PlayerProfileManager.Instance.ActiveProfile.FirstRewardedAdWatched)
    //           {
    //			Log.AnEvent(Events.FirstRewardedAdWatched);
    //			PlayerProfileManager.Instance.ActiveProfile.FirstRewardedAdWatched = true;
    //			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    //		}
    //	}
    //	else if (args.AdStatus == AdFinishedEventArgs.Status.Dismissed)
    //	{
    //		this.DisplayResultPopup(videoForRewardConfiguration, eResult.UserFailedToFinish);
    //	}
    //	else if (args.AdStatus == AdFinishedEventArgs.Status.Failed)
    //	{
    //		this.DisplayResultPopup(videoForRewardConfiguration, eResult.FailedToDisplay);
    //	}
    //	else
    //	{
    //		this.DisplayResultPopup(videoForRewardConfiguration, eResult.FailedToDisplay);
    //	}
    //}

    private void DisplayResultPopup(VideoForRewardConfiguration cfg, eResult result)
    {
        string bundledGraphicPath = null;
        string imageCaption = string.Empty;
        string title = string.Empty;
        string bodyText = string.Empty;
        string confirmText = string.Empty;
        switch (result)
        {
            case eResult.RewardGiven:
                {
                    VideoForRewardConfiguration.eRewardID rewardIDEnum = cfg.RewardID;
                    bundledGraphicPath = this.GraphicForVideoAdPopup(rewardIDEnum);
                    imageCaption = this.CaptionForVideoAdPopup(rewardIDEnum);
                    title = cfg.PostAdPromptTitleTextID;
                    bodyText = this.GetTranslatedBodyText(cfg, cfg.PostAdPromptBodyTextID, true);
                    confirmText = cfg.PostAdPromptOkButtonTextID;
                    break;
                }
            case eResult.FailedToPrecache:
            case eResult.FailedToDisplay:
                if (cfg.RewardID == VideoForRewardConfiguration.eRewardID.PressForFuel)
                {
                    Log.AnEvent(Events.ErrorToShowPressFuelVideo);
                }
                title = "TEXT_POPUP_TITLE_VIDEO_AD_FAILED_TITLE";
                bodyText = LocalizationManager.GetTranslation("TEXT_POPUP_TITLE_VIDEO_AD_FAILED_BODY");
                confirmText = "TEXT_BUTTON_OK";
                bundledGraphicPath = PopUpManager.Instance.graphics_agentPrefab;
                break;
            default:
                VideoForRewardConfiguration.eRewardID rewardIDEnum2 = cfg.RewardID;
                bundledGraphicPath = this.GraphicForVideoAdPopup(rewardIDEnum2);
                imageCaption = this.CaptionForVideoAdPopup(rewardIDEnum2);
                title = "TEXT_POPUP_TITLE_VIDEO_AD_FAILED_USER_DIDNT_FINISH_TITLE";
                bodyText = LocalizationManager.GetTranslation("TEXT_POPUP_TITLE_VIDEO_AD_FAILED_USER_DIDNT_FINISH_BODY");
                confirmText = "TEXT_BUTTON_OK";
                break;
        }

        if (result != eResult.RewardGiven || cfg.EnablePostAdPrompt)
        {
            PopUp popup = new PopUp
            {
                ID = PopUpID.AdResult,
                Title = title,
                BodyText = bodyText,
                IsBig = true,
                BodyAlreadyTranslated = true,
                ConfirmAction = delegate
                {
                    this.OnResultConfirmed(cfg, result);
                },
                ConfirmText = confirmText,
                GraphicPath = bundledGraphicPath,
                ImageCaption = imageCaption
            };

            if (PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.WaitSpinner))
                PopUpManager.Instance.KillPopUp();
            if (!PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null))
            {
                this.OnResultConfirmed(cfg, result);
            }
        }
        else
        {
            this.OnResultConfirmed(cfg, result);
        }
    }

    public void ApplyRPBonus()
    {
        //DateTime dateTime = ServerSynchronisedTime.Now;
        //DateTime inFinish = dateTime.Add(GameDatabase.Instance.RPBonusConfiguration.VideoAdMultiplierDuration);
        //RPBonusAd rPBonusAd = RPBonusManager.GetRPBonusAd();
        //if (rPBonusAd == null)
        //{
        //    rPBonusAd = new RPBonusAd(GameDatabase.Instance.RPBonusConfiguration.VideoAdMultiplier);
        //    RPBonusManager.AddUniqueBonusObject(rPBonusAd);
        //}
        //rPBonusAd.BonusWindow = new RPBonusWindow(dateTime, inFinish);
        //RPBonusManager.RefreshUI();
    }

    private void OnResultConfirmed(VideoForRewardConfiguration cfg, eResult result)
    {
        if (result == eResult.RewardGiven)
        {
            this.SendV4RMetricsEvent(cfg, Events.AdUserV4RComplete, this.m_AdProvider);
            if (cfg.RewardID == VideoForRewardConfiguration.eRewardID.PressForFuel)
            {
                Log.AnEvent(Events.PressForFuelAddWatchCompleted);
            }
            switch (cfg.RewardType)
            {
                case VideoForRewardConfiguration.eRewardType.Fuel:
                    FuelManager.Instance.AddFuel(GetRewardAmount(cfg), FuelReplenishTimeUpdateAction.KEEP, FuelAnimationLockAction.OBEY);
                    if (cfg.EnableVideo24HCap)
                    {
                        PlayerProfileManager.Instance.ActiveProfile.VideoForFuelTimestamp();
                    }
                    break;
                case VideoForRewardConfiguration.eRewardType.Cash:
                    PlayerProfileManager.Instance.ActiveProfile.AddCash(GetRewardAmount(cfg), "reward", "video");

                    break;
                case VideoForRewardConfiguration.eRewardType.Gold:
                    PlayerProfileManager.Instance.ActiveProfile.AddGold(GetRewardAmount(cfg), "reward", "video");
                    break;
                case VideoForRewardConfiguration.eRewardType.StreakRescue:
                    //StreakScreen streakScreen = ScreenManager.Instance.ActiveCSRScreen as StreakScreen;
                    //if (streakScreen != null)
                    //{
                    //    streakScreen.OnBrokenStreakRetry(0, StreakRescueCostData.StreakRescueCostType.COST_AD);
                    //}
                    break;
                case VideoForRewardConfiguration.eRewardType.RPBonus:
                    this.ApplyRPBonus();
                    break;
                case VideoForRewardConfiguration.eRewardType.ReduceDeliveryTime:
                    ArrivalManager.Instance.ReduceDeliveryTime(GetRewardAmount(cfg));
                    break;
                case VideoForRewardConfiguration.eRewardType.AppTuttiTimedReward:
                    PlayerProfileManager.Instance.ActiveProfile.AddCash(AppTuttiTimedRewardManager.Instance.GetCashRewardAmount(), "reward", "apptuttitimedvideo");
                    PlayerProfileManager.Instance.ActiveProfile.AddGold(AppTuttiTimedRewardManager.Instance.GetGoldRewardAmount(), "reward", "apptuttitimedvideo");
                    AppTuttiTimedRewardManager.Instance.OnRewardGiven();
                    break;
                case VideoForRewardConfiguration.eRewardType.DoubledPrize:
                case VideoForRewardConfiguration.eRewardType.ExtraCash:
                    if (m_ExtraRewardResult != null)
                        m_ExtraRewardResult.VideoWatched(cfg.RewardType);
                    break;
            }

            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
        else if (result == eResult.UserFailedToFinish)
        {
            this.SendV4RMetricsEvent(cfg, Events.AdUserV4RStopEarly, this.m_AdProvider);
            if (cfg.RewardID == VideoForRewardConfiguration.eRewardID.PressForFuel)
                Log.AnEvent(Events.PressForFuelAddWatchCanceled);
            if (m_ExtraRewardResult != null)
                m_ExtraRewardResult.VideoFailed(cfg.RewardType);
        }
        else if (result == eResult.NotWatching)
        {
            this.SendV4RMetricsEvent(cfg, Events.AdUserCancelV4R, this.m_AdProvider);

            if (m_ExtraRewardResult != null)
                m_ExtraRewardResult.OfferRejected(cfg.RewardType);
        }
        else
        {
            if (m_ExtraRewardResult != null)
                m_ExtraRewardResult.VideoFailed(cfg.RewardType);
        }
        if (result != eResult.RewardGiven && cfg.RewardType == VideoForRewardConfiguration.eRewardType.StreakRescue)
        {
            //StreakScreen streakScreen2 = ScreenManager.Instance.ActiveCSRScreen as StreakScreen;
            //if (streakScreen2 != null)
            //{
            //    if (result == VideoForRewardsManager.eResult.NotWatching)
            //    {
            //        streakScreen2.OnBrokenStreakCancel();
            //    }
            //    else
            //    {
            //        streakScreen2.DisplayStreakRetryPopup(false);
            //    }
            //}
        }
    }

    private VideoForRewardConfiguration GetConfiguration(VideoForRewardConfiguration.eRewardID rewardID)
    {
        return GameDatabase.Instance.Ad.GetConfiguration(rewardID);
    }

    private string GetTranslatedBodyText(VideoForRewardConfiguration configuration, string bodyTextID, bool rewardGiven = true)
    {
        string text = LocalizationManager.GetTranslation(bodyTextID);
        switch (configuration.RewardType)
        {
            case VideoForRewardConfiguration.eRewardType.Fuel:
                text = string.Format(text, GetRewardAmount(configuration));
                break;
            case VideoForRewardConfiguration.eRewardType.Cash:
                text = string.Format(text, CurrencyUtils.GetCashString(GetRewardAmount(configuration)));
                break;
            case VideoForRewardConfiguration.eRewardType.Gold:
                text = string.Format(text, CurrencyUtils.GetGoldStringWithIcon(GetRewardAmount(configuration)));
                break;
            case VideoForRewardConfiguration.eRewardType.RPBonus:
                //text = string.Format(text, GameDatabase.Instance.RPBonusConfiguration.VideoAdMultiplierDuration.TotalMinutes);
                break;
            case VideoForRewardConfiguration.eRewardType.ReduceDeliveryTime:
                text = string.Format(text, GetRewardAmount(configuration));
                break;
            case VideoForRewardConfiguration.eRewardType.AppTuttiTimedReward:
                if (rewardGiven)
                {
                    text = string.Format(text,
                        CurrencyUtils.GetCashString(AppTuttiTimedRewardManager.Instance.GetCashRewardAmount()) + " " + LocalizationManager.GetTranslation("TEXT_AND_SIGN") + " " +
                        CurrencyUtils.GetGoldStringWithIcon(AppTuttiTimedRewardManager.Instance.GetGoldRewardAmount()));
                }
                else
                {
                    text = string.Format(text,
                        CurrencyUtils.GetCashString(AppTuttiTimedRewardManager.Instance.GetCashRewardAmount()),
                        CurrencyUtils.GetGoldStringWithIcon(AppTuttiTimedRewardManager.Instance.GetGoldRewardAmount()),
                        GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward.minutesPerReward);
                }
                break;
            case VideoForRewardConfiguration.eRewardType.ExtraCash:
                if (rewardGiven)
                {
                    text = string.Format(text, m_ExtraRewardResult.VideoSuccessRewardText);
                }
                else
                {
                    if (string.IsNullOrEmpty(m_ExtraRewardResult.VideoFailRewardText))
                        text = string.Format(LocalizationManager.GetTranslation(bodyTextID + "_2"), m_ExtraRewardResult.VideoSuccessRewardText);
                    else
                        text = string.Format(text, m_ExtraRewardResult.VideoSuccessRewardText, m_ExtraRewardResult.VideoFailRewardText);
                }
                break;
            case VideoForRewardConfiguration.eRewardType.DoubledPrize:
                text = string.Format(text, m_ExtraRewardResult.VideoSuccessRewardText, m_ExtraRewardResult.VideoFailRewardText);
                break;

        }
        return text;
    }

    private string GetTranslatedCancelButtonText(VideoForRewardConfiguration configuration, string cancelButtonTextID)
    {
        string text = LocalizationManager.GetTranslation(cancelButtonTextID);
        switch (configuration.RewardType)
        {
            case VideoForRewardConfiguration.eRewardType.ExtraCash:
                if (string.IsNullOrEmpty(m_ExtraRewardResult.VideoFailRewardText))
                    text = LocalizationManager.GetTranslation(cancelButtonTextID + "_2");
                else
                    text = m_ExtraRewardResult.VideoFailRewardText;
                break;
            case VideoForRewardConfiguration.eRewardType.DoubledPrize:
                text = m_ExtraRewardResult.VideoFailRewardText;
                break;
        }
        return text;
    }

    private string GetTranslatedConfirmButtonText(VideoForRewardConfiguration configuration, string confirmButtonTextID)
    {
        string text = LocalizationManager.GetTranslation(confirmButtonTextID);
        switch (configuration.RewardType)
        {
            case VideoForRewardConfiguration.eRewardType.ExtraCash:
                if (string.IsNullOrEmpty(m_ExtraRewardResult.VideoFailRewardText))
                    text = "<size=25>" + m_ExtraRewardResult.VideoSuccessRewardText + "<br>" + LocalizationManager.GetTranslation("TEXT_BUTTON_WATCH_AD");
                else
                    text = "<size=25>" + m_ExtraRewardResult.VideoFailRewardText + " " + LocalizationManager.GetTranslation("TEXT_AND_SIGN") + " " + m_ExtraRewardResult.VideoSuccessRewardText + "<br>" + LocalizationManager.GetTranslation("TEXT_BUTTON_WATCH_AD");
                break;
            case VideoForRewardConfiguration.eRewardType.DoubledPrize:
                text = "<size=25>" + m_ExtraRewardResult.VideoSuccessRewardText + "<br>" + LocalizationManager.GetTranslation("TEXT_BUTTON_WATCH_AD");
                break;
        }
        return text;
    }

    private string GetItmClss(VideoForRewardConfiguration cfg)
    {
        string result = "unknown";
        VideoForRewardConfiguration.eRewardType rewardTypeEnum = cfg.RewardType;
        if (rewardTypeEnum != VideoForRewardConfiguration.eRewardType.ReduceDeliveryTime)
        {
            result = cfg.RewardType.ToString().ToLower();
        }
        else
        {
            Arrival priorityDelivery = ArrivalManager.Instance.GetPriorityDelivery();
            if (priorityDelivery != null)
            {
                result = priorityDelivery.GetItmClss();
            }
        }
        return result;
    }

    private string GetItm(VideoForRewardConfiguration cfg)
    {
        string result = "unknown";
        VideoForRewardConfiguration.eRewardType rewardTypeEnum = cfg.RewardType;
        if (rewardTypeEnum != VideoForRewardConfiguration.eRewardType.ReduceDeliveryTime)
        {
            result = "NA";
        }
        else
        {
            Arrival priorityDelivery = ArrivalManager.Instance.GetPriorityDelivery();
            if (priorityDelivery != null)
            {
                result = priorityDelivery.GetItm();
            }
        }
        return result;
    }

    private void SendV4RMetricsEvent(VideoForRewardConfiguration cfg, Events eventID, string adProviderName)
    {
        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        {
            {
                Parameters.AdCfg,
                cfg.RewardID.ToString()
            },
            {
                Parameters.AdNwk,
                adProviderName
            },
            {
                Parameters.AdLoc,
                cfg.RewardType.ToString()
            },
            {
                Parameters.AdRwType,
                cfg.RewardType.ToString()
            },
            {
                Parameters.AdRwAmt,
                GetRewardAmount(cfg).ToString()
            },
            {
                Parameters.ItmClss,
                this.GetItmClss(cfg)
            },
            {
                Parameters.Itm,
                this.GetItm(cfg)
            }
        };
        Log.AnEvent(eventID, data);
    }

    public static int GetRewardAmount(VideoForRewardConfiguration configuration)
    {
        switch (RaceEventQuery.Instance.getHighestUnlockedClass())
        {
            case eCarTier.TIER_1:
                return configuration.RewardAmountT1;
            case eCarTier.TIER_2:
                return configuration.RewardAmountT2;
            case eCarTier.TIER_3:
                return configuration.RewardAmountT3;
            case eCarTier.TIER_4:
                return configuration.RewardAmountT4;
            case eCarTier.TIER_5:
                return configuration.RewardAmountT5;
            case eCarTier.TIER_X:
                return configuration.RewardAmountTX;
            default:
                return configuration.RewardAmountT1;
        }
    }

    public static int GetRewardAmount(VideoForRewardConfiguration configuration, eCarTier tier)
    {
        switch (tier)
        {
            case eCarTier.TIER_1:
                return configuration.RewardAmountT1;
            case eCarTier.TIER_2:
                return configuration.RewardAmountT2;
            case eCarTier.TIER_3:
                return configuration.RewardAmountT3;
            case eCarTier.TIER_4:
                return configuration.RewardAmountT4;
            case eCarTier.TIER_5:
                return configuration.RewardAmountT5;
            case eCarTier.TIER_X:
                return configuration.RewardAmountTX;
            default:
                return configuration.RewardAmountT1;
        }
    }

    public void SetExtraRewardResult(ExtraRewardResult extraRewardResult)
    {
        m_ExtraRewardResult = extraRewardResult;
    }

    [System.Serializable]
    public class ExtraRewardResult
    {
        public System.Action ActionOnVideoFail;
        public System.Action ActionOnVideoOfferReject;
        public System.Action ActionOnVideoSuccess;
        public string VideoFailRewardText;
        public string VideoSuccessRewardText;

        public void VideoWatched(VideoForRewardConfiguration.eRewardType rewardType)
        {
            if (!ShouldExecute(rewardType))
                return;

            if (ActionOnVideoSuccess != null)
                ActionOnVideoSuccess.Invoke();
        }

        public void VideoFailed(VideoForRewardConfiguration.eRewardType rewardType)
        {
            if (!ShouldExecute(rewardType))
                return;

            if (ActionOnVideoFail != null)
                ActionOnVideoFail.Invoke();
        }

        public void OfferRejected(VideoForRewardConfiguration.eRewardType rewardType)
        {
            if (!ShouldExecute(rewardType))
                return;

            if (ActionOnVideoSuccess != null)
                ActionOnVideoOfferReject.Invoke();
        }

        private bool ShouldExecute(VideoForRewardConfiguration.eRewardType rewardType)
        {
            return (rewardType == VideoForRewardConfiguration.eRewardType.ExtraCash ||
                    rewardType == VideoForRewardConfiguration.eRewardType.DoubledPrize);
        }
    }
}
