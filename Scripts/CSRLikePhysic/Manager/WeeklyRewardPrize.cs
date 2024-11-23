using System;
using System.Collections.Generic;
using System.Linq;
using Objectives;

[Serializable]
public class WeeklyRewardData
{
    public string LeagueName;

    public WeeklyReward[] WeeklyRewards;
    public WeeklyReward GetRewardByRank(int rank)
    {
        return WeeklyRewards.FirstOrDefault(r => r.HasRank(rank));
        UnityEngine.Debug.Log("league===>>>>>rank is:"+ rank);
    }
}

[Serializable]
public class WeeklyReward
{
    public string Name;
    public int MinRank;
    public int MaxRank;
    public List<ObjectiveRewardData> Rewards = new List<ObjectiveRewardData>();

    public bool HasRank(int rank)
    {
        return (rank >= MinRank && rank <= MaxRank);
    }

    public string GetRewardText()
    {
        string result = string.Empty;

        for (int i = 0; i < Rewards.Count; i++)
        {
            var objectiveRewardData = Rewards[i];
            result += objectiveRewardData.GetRewardText();

            if ((i + 1) < Rewards.Count)
            {
                result += "\n";
            }
        }

        return result;
    }
}