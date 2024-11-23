using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class LeagueBonus 
{
    [SerializeField]
    private LevelBonusMultipliers[] m_leagueRewards;
    public LevelBonusMultipliers GetLeagueBonusRewardMultiplier(LeagueData.LeagueName league)
    {
        var leagueIndex = (int)league;

        if (leagueIndex < 0)
            leagueIndex = 0;

        if (m_leagueRewards.Length <= leagueIndex)
        {
            return m_leagueRewards[m_leagueRewards.Length - 1];
        }

        if (m_leagueRewards.Length == 0)
        {
            return new LevelBonusMultipliers();
        }

        return m_leagueRewards[leagueIndex];
    }

    public bool HasLevelBonusRewardMultiplier(int level)
    {
        var levelIndex = level - 1;

        if (levelIndex <= 0)
        {
            return false;
        }

        if (m_leagueRewards.Length <= levelIndex)
        {
            return false;
        }

        var lastLevelReward = m_leagueRewards[levelIndex - 1];
        var thisLevelReward = m_leagueRewards[levelIndex];

        return thisLevelReward.CashPrizeMultiplier - lastLevelReward.CashPrizeMultiplier > 0
               || thisLevelReward.GoldPrizeMultiplier - lastLevelReward.GoldPrizeMultiplier > 0;
    }
}
