using System;
using I2.Loc;
using UnityEngine;

public class AppTuttiTimedRewardManager : MonoBehaviour
{
    public static AppTuttiTimedRewardManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public bool IsTimeForReward()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var now = GTDateTime.Now;
        var lastPrizeTime = activeProfile.AppTuttiTimedRewardLastEventAt;
        var appTuttiTimedRewardTimeSpan = now - lastPrizeTime;
        if ((appTuttiTimedRewardTimeSpan.TotalMinutes > GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward.minutesPerReward) && activeProfile.HasCompletedSecondTutorialRace())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public int TimeUntilNextReward()
    {
        var num = this.TimeSinceLastReward();
        return Mathf.Clamp(GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward.minutesPerReward*60 - num, 0, GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward.minutesPerReward*60);
    }
    
    public int TimeSinceLastReward()
    {
        var value = PlayerProfileManager.Instance.ActiveProfile.LastAppTuttiRewardTime();
        return (int)GTDateTime.Now.Subtract(value).TotalSeconds;
    }

    public void StartFlow()
    {
        VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.AppTuttiTimedReward);
    }
    
    public void OnRewardGiven()
    {
        PlayerProfileManager.Instance.ActiveProfile.AppTuttiTimedRewardLastEventAt = GTDateTime.Now;
    }
    
    public void ShowTimeRemainingpopup()
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "TEXT_POPUPS_CASH_AD_PRE_BODY_TITLE",
            BodyText =
                string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_APPTUTTI_PRIZE_NOTREADY_BODY"), GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward.minutesPerReward),
            BodyAlreadyTranslated = true,
            ConfirmText = "TEXT_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        }, 0, null);
    }
    
    public int GetCashRewardAmount()
    {
        AppTuttiTimedRewardData data = GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward;
        switch (RaceEventQuery.Instance.getHighestUnlockedClass())
        {
            case eCarTier.TIER_1:
                return data.cashRewardAmountT1;
            case eCarTier.TIER_2:
                return data.cashRewardAmountT2;
            case eCarTier.TIER_3:
                return data.cashRewardAmountT3;
            case eCarTier.TIER_4:
                return data.cashRewardAmountT4;
            case eCarTier.TIER_5:
                return data.cashRewardAmountT5;
            case eCarTier.TIER_X:
                return data.cashRewardAmountTX;
            default:
                return data.cashRewardAmountT1;
        }
    }
    
    public int GetGoldRewardAmount()
    {
        AppTuttiTimedRewardData data = GameDatabase.Instance.OnlineConfiguration.AppTuttiTimedReward;
        switch (RaceEventQuery.Instance.getHighestUnlockedClass())
        {
            case eCarTier.TIER_1:
                return data.goldRewardAmountT1;
            case eCarTier.TIER_2:
                return data.goldRewardAmountT2;
            case eCarTier.TIER_3:
                return data.goldRewardAmountT3;
            case eCarTier.TIER_4:
                return data.goldRewardAmountT4;
            case eCarTier.TIER_5:
                return data.goldRewardAmountT5;
            case eCarTier.TIER_X:
                return data.goldRewardAmountTX;
            default:
                return data.goldRewardAmountT1;
        }
    }
}
