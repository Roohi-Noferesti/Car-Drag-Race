using System;
using UnityEngine;

[Serializable]
public class OnlineRaceTierSetting
{
    //public int EnteranceCashFee;

    //public int EnteranceGoldFee;

    //public CSR2Reward WinReward;

    //public int RewardAmount;

    public int Stake;
    //public int EasyMinPPDelta;
    //public int EasyMaxPPDelta;
    //public int HardMinPPDelta;
    //public int HardMaxPPDelta;

    public AnimationCurve ProbabilityCurve;


    //public int GetInterpolatedPPDelta(float winPercentage)
    //{
    //    var minPP = Mathf.Lerp(EasyMinPPDelta, HardMinPPDelta, winPercentage);
    //    var maxPP = Mathf.Lerp(EasyMaxPPDelta, HardMaxPPDelta, winPercentage);
    //    return (int) UnityEngine.Random.Range(minPP, maxPP);
    //}

    public string GetRewardText()
    {
        return CurrencyUtils.GetCashString(Stake*2);

        //switch (WinReward.rewardType)
        //{
        //    case ERewardType.Cash:
        //        return CurrencyUtils.GetCashString(RewardAmount);
        //    case ERewardType.Gold:
        //        return CurrencyUtils.GetGoldStringWithIcon(RewardAmount);
        //    case ERewardType.FreeUpgrade:
        //        return LocalizationManager.GetTranslation("TEXT_FREE_UPGRADE");
        //    case ERewardType.FuelPip:
        //        return string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_PIPS"), RewardAmount);
        //    case ERewardType.Car:
        //        return WinReward.GetRewardTitleText(RewardAmount);
        //}
        //return string.Empty;
    }

    //public override string ToString()
    //{
    //    return String.Format(
    //        "TierSetting : EasyMinPPDelta:{0},EasyMaxPPDelta:{1},HardMinPPDelta:{2},HardMaxPPDelta:{3}", EasyMinPPDelta,
    //        EasyMaxPPDelta, HardMinPPDelta, HardMaxPPDelta);
    //}
}