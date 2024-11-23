using System.Linq;
using UnityEngine;

[System.Serializable]
public class OnlineRace
{
    public string Name;

    public string LocalizationNameText;

    public string IconName;

    public int EventIndex;

    public int UnlockEventID;

    public OnlineRaceTierSetting Tier1;
    public OnlineRaceTierSetting Tier2;
    public OnlineRaceTierSetting Tier3;
    public OnlineRaceTierSetting Tier4;
    public OnlineRaceTierSetting Tier5;

    public string GetRewardText(eCarTier tier)
    {
        OnlineRaceTierSetting onlineRaceTierSetting = GetTierSetting(tier);
        return onlineRaceTierSetting.GetRewardText();
    }

    public OnlineRaceTierSetting GetTierSetting(eCarTier carTier)
    {
        switch (carTier)
        {
            case eCarTier.TIER_1:
                return Tier1;
            case eCarTier.TIER_2:
                return Tier2;
            case eCarTier.TIER_3:
                return Tier3;
            case eCarTier.TIER_4:
                return Tier4;
            case eCarTier.TIER_5:
                return Tier5;
        }
        return Tier1;
    }

    public int GetRandomPPDelta(eCarTier carTier, float winLoseModifier )
    {
        OnlineRaceTierSetting onlineRaceTierSetting = GetTierSetting(carTier);

        float randomValue = onlineRaceTierSetting.ProbabilityCurve.Evaluate(Random.value + winLoseModifier);
        var ppDelta = Mathf.RoundToInt(randomValue);


        GTDebug.Log(GTLogChannel.RaceOrganiser, "Selected PP : " + ppDelta + "  By winLoseModifier : " + winLoseModifier + "/" + onlineRaceTierSetting.ProbabilityCurve);
        return ppDelta;
    }

    public int GetStake(eCarTier tier)
    {
        OnlineRaceTierSetting onlineRaceTierSetting = GetTierSetting(tier);
        return onlineRaceTierSetting.Stake;
    }
}