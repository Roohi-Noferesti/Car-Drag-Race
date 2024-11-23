using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnlineRaceSelectButton : RuntimeTextButton
{
    public TextMeshProUGUI TitleText;

    public TextMeshProUGUI EnterFeeText;

    public TextMeshProUGUI RewardText;

    public Image OverlaySprite;

    public void Setup(string title, OnlineRace onlineRaceMatch,eCarTier tier,UnityAction onSelect,Sprite sprite)
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var isUnlocked = true;//activeProfile.IsEventCompleted(onlineRaceMatch.UnlockEventID);

        CurrentState = isUnlocked ? State.Active : State.Disabled;

        var cashCost = onlineRaceMatch.GetTierSetting(tier).Stake;
        //var goldCost = onlineRaceMatch.GetTierSetting(tier).EnteranceGoldFee;

        OverlaySprite.sprite = sprite;
        EnterFeeText.text = string.Format(LocalizationManager.GetTranslation("TEXT_SMP_LOBBY_BUTTON_ENTERY_FEE"), CurrencyUtils.GetCashString(cashCost));
        //else if (goldCost > 0)
        //{
        //    EnterFeeText.text = CurrencyUtils.GetGoldStringWithIcon(goldCost);
        //}

        RewardText.text = onlineRaceMatch.GetRewardText(tier);
        TitleText.text = title;
        AddValueChangedDelegate(onSelect);
    }
}
