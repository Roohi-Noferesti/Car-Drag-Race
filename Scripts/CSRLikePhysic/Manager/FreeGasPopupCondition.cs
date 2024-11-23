using System;
using AdSystem.Enums;
using I2.Loc;

public class FreeGasPopupCondition : FlowConditionBase
{
    public override bool IsConditionActive()
    {
        return !GTAdManager.Instance.ShouldHideAdInterface;
    }

    private GasTankIAPCondition _gasTankIAPCondition;

    private int PromptIndex;

    private readonly VideoForRewardConfiguration.eRewardID rewardID = VideoForRewardConfiguration.eRewardID.PressForFuel;

    public FreeGasPopupCondition(GasTankIAPCondition gasTankIAPCondition)
    {
        this._gasTankIAPCondition = gasTankIAPCondition;
    }

    public override void EvaluateHardcodedConditions()
    {
        this.state = ConditionState.NOT_VALID;
        if (CrewProgressionSetup.CheckPostRaceCrewProgression())
        {
            return;
        }
        if (this._gasTankIAPCondition.state == ConditionState.VALID)
        {
            return;
        }
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!FuelPromptAvailable(activeProfile, out this.PromptIndex))
        {
            return;
        }
        if (activeProfile.FuelRefillsBoughtWithGold == 0)
        {
            return;
        }

        var videoEnabled = this.VideoAdEnabled();
        var videoAvailable = this.VideoAdAvailable();
        if (videoEnabled && !videoAvailable)
        {

            //var configuration = GameDatabase.Instance.Ad.GetConfiguration(this.rewardID);
            //GTAdManager.Instance.PrepareAd(configuration.GetAdSpace(), false);
        
        }
        /*if (this.InterstitialEnabled() && !this.InterstitialAvailable())
        {
            GTAdManager.Instance.Precache(AdManager.AdSpace.Default);
        }*/
        if (!this.VideoAdsAvailableAndEnabled() && !this.InterstitialAdsAvailableAndEnabled())
        {
            return;
        }
        this.state = ConditionState.VALID;
    }

    public void OnPopupShown()
    {
        PlayerProfileManager.Instance.ActiveProfile.DisableFuelAdPrompt(this.PromptIndex);
    }

    public override PopUp GetPopup()
    {
        if (this.VideoAdsAvailableAndEnabled())
        {
            PopUp preAdPromptPopup = VideoForRewardsManager.Instance.GetPreAdPromptPopup(VideoForRewardConfiguration.eRewardID.WatchForFuelPrompted);
            preAdPromptPopup.ConfirmAction += OnPopupShown;
            preAdPromptPopup.CloseAction += OnPopupShown;
            preAdPromptPopup.CancelAction += OnPopupShown;
            return preAdPromptPopup;
        }
        int pips = GameDatabase.Instance.AdConfiguration.FuelAdPromptInterstitialRewardAmount;
        return new PopUp
        {
            Title = "TEXT_VIDEO_FOR_FUEL_PRE_TITLE",
            BodyText = LocalizationManager.GetTranslation("TEXT_VIDEO_FOR_FUEL_PRE_BODY"),//string.Format(LocalizationManager.GetTranslation("TEXT_VIDEO_FOR_FUEL_PRE_BODY"), pips),
            IsBig = true,
            BodyAlreadyTranslated = true,
            CancelAction = new PopUpButtonAction(this.OnPopupShown),
            ConfirmAction = delegate
            {
                this.OnPopupShown();
                GTAdManager.Instance.AutoShowAd(AdSpace.Default,3,OnFinish:() =>
                {
                    FuelManager.Instance.AddFuel(pips, FuelReplenishTimeUpdateAction.KEEP, FuelAnimationLockAction.OBEY);
                });


            },
            CancelText = "TEXT_VIDEO_FOR_FUEL_PRE_CANCEL_BUTTON",
            ConfirmText = "TEXT_VIDEO_FOR_FUEL_PRE_OK_BUTTON",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        };
    }

    public override bool HasBubbleMessage()
    {
        return false;
    }

    public override string GetBubbleMessage()
    {
        return string.Empty;
    }

    private bool VideoAdEnabled()
    {
        if (VideoForRewardsManager.Instance.VideoAdCapHit(VideoForRewardConfiguration.eRewardID.PressForFuel))
        {
            return false;
        }
        VideoForRewardConfiguration configuration = GameDatabase.Instance.Ad.GetConfiguration(this.rewardID);
        return VideoForRewardsManager.GetRewardAmount(configuration) > 0;
    }

    private bool VideoAdAvailable()
    {
        VideoForRewardConfiguration configuration = GameDatabase.Instance.Ad.GetConfiguration(this.rewardID);
        AdSpace adSpace = configuration.GetAdSpace();
        return GTAdManager.Instance.isPrepared(adSpace);
    }

    private bool VideoAdsAvailableAndEnabled()
    {
        return this.VideoAdAvailable() && this.VideoAdEnabled();
    }

    private bool InterstitialEnabled()
    {
        return GameDatabase.Instance.AdConfiguration.FuelAdPromptInterstitialRewardAmount > 0;
    }

    private bool InterstitialAvailable()
    {
        return GTAdManager.Instance.isPrepared(AdSpace.Default);
    }

    private bool InterstitialAdsAvailableAndEnabled()
    {
        return this.InterstitialAvailable() && this.InterstitialEnabled();
    }

    private static bool FuelPromptAvailable(PlayerProfile profile, out int index)
    {
        for (int num = 0; num != GameDatabase.Instance.AdConfiguration.FuelAdPromptShowThreshold.Count; num++)
        {
            if (FuelManager.Instance.GetFuel() < GameDatabase.Instance.AdConfiguration.FuelAdPromptShowThreshold[num] && profile.FuelAdPromptEnabled(num))
            {
                index = num;
                return true;
            }
        }
        index = GameDatabase.Instance.AdConfiguration.FuelAdPromptShowThreshold.Count;
        return false;
    }


}
