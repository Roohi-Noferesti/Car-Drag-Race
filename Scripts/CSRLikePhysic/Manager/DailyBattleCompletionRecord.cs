using System;

[Serializable]
public class DailyBattleCompletionRecord
{
    public int RewardId;

    public DateTime When;

    public eCarTier Tier;

    public bool PlayerWon;

    [Obsolete("Use 'DailyBattleRewardManager.Instance.GetReward()' instead", true)]
    public DailyBattleReward GetReward()
    {
        return DailyBattleRewardManager.Instance.GetReward(this.RewardId, this.Tier, this.PlayerWon);
    }

    public static void ReadFromJson(JsonDict jsonDict, ref DailyBattleCompletionRecord result)
    {
        jsonDict.TryGetValue("i", out result.RewardId);
        jsonDict.TryGetValue("w", out result.When);
        jsonDict.TryGetValue("l", out result.PlayerWon);
        int num = 1;
        jsonDict.TryGetValue("t", out num);
        if (num < 0)
        {
            num = 0;
        }
        if (num >= 6)
        {
            num = 5;
        }
        result.Tier = (eCarTier)num;
    }

    public static void WriteToJson(DailyBattleCompletionRecord record, ref JsonDict jsonDict)
    {
        jsonDict.Set("i", record.RewardId);
        jsonDict.Set("w", record.When);
        jsonDict.Set("t", (int)record.Tier);
        jsonDict.Set("l", record.PlayerWon);
    }
}
