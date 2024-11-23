using System;
using I2.Loc;
using UnityEngine;

public class VasTimedRewardManager : MonoBehaviour
{
    public static VasTimedRewardManager Instance;

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
        var lastPrizeTime = activeProfile.VasTimedRewardLastEventAt;
        var vasTimedRewardTimeSpan = now - lastPrizeTime;
        if ((vasTimedRewardTimeSpan.TotalMinutes > GameDatabase.Instance.OnlineConfiguration.VasTimedReward.minutesPerReward) && activeProfile.HasCompletedSecondTutorialRace())
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
        return Mathf.Clamp(GameDatabase.Instance.OnlineConfiguration.VasTimedReward.minutesPerReward*60 - num, 0, GameDatabase.Instance.OnlineConfiguration.VasTimedReward.minutesPerReward*60);
    }
    
    public int TimeSinceLastReward()
    {
        var value = PlayerProfileManager.Instance.ActiveProfile.LastVasRewardTime();
        return (int)GTDateTime.Now.Subtract(value).TotalSeconds;
    }

    public void StartFlow()
    {
        PlayerProfileManager.Instance.ActiveProfile.AddCash(GetCashRewardAmount(),"reward", "vastimedvideo");
        PlayerProfileManager.Instance.ActiveProfile.AddGold(GetGoldRewardAmount(),"reward", "vastimedvideo");
        PlayerProfileManager.Instance.ActiveProfile.VasTimedRewardLastEventAt = GTDateTime.Now;
        ShowPostPromotPopup();
    }

    public void ShowPostPromotPopup()
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            ID = PopUpID.PreAdPromptPopup,
            Title = "TEXT_POPUPS_CASH_AD_POST_BODY_TITLE",
            BodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_CASH_AD_POST_BODY"), 
                CurrencyUtils.GetCashString(GetCashRewardAmount()) + " " + 
                LocalizationManager.GetTranslation("TEXT_AND_SIGN") + " " +
                CurrencyUtils.GetGoldStringWithIcon(GetGoldRewardAmount())),
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        }, 0, null);
    }
    
    public void ShowTimeRemainingpopup()
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "TEXT_POPUPS_CASH_AD_PRE_BODY_TITLE",
            BodyText =
                string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_VAS_PRIZE_NOTREADY_BODY"), GameDatabase.Instance.OnlineConfiguration.VasTimedReward.minutesPerReward),
            BodyAlreadyTranslated = true,
            ConfirmText = "TEXT_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        }, 0, null);
    }
    
    public int GetCashRewardAmount()
    {
        VasTimedRewardData data = GameDatabase.Instance.OnlineConfiguration.VasTimedReward;
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
        VasTimedRewardData data = GameDatabase.Instance.OnlineConfiguration.VasTimedReward;
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
