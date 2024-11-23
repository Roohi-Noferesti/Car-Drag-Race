using System;
using UnityEngine;

[Serializable]
public class LevelBonus
{
    [SerializeField] private LevelBonusMultipliers[] m_levelRewards;

    public LevelBonusMultipliers GetLevelBonusRewardMultiplier(int level)
    {
        var levelIndex = level - 1;

        if (levelIndex < 0)
            levelIndex = 0;

        if ( m_levelRewards.Length <= levelIndex)
        {
            return m_levelRewards[m_levelRewards.Length - 1];
        }

        if (m_levelRewards.Length == 0)
        {
            return new LevelBonusMultipliers();
        }

        return m_levelRewards[levelIndex];
    }

    public bool HasLevelBonusRewardMultiplier(int level)
    {
        var levelIndex = level - 1;

        if (levelIndex <= 0)
        {
            return false;
        }

        if (m_levelRewards.Length <= levelIndex)
        {
            return false;
        }

        var lastLevelReward = m_levelRewards[levelIndex - 1];
        var thisLevelReward = m_levelRewards[levelIndex];

        return thisLevelReward.CashPrizeMultiplier - lastLevelReward.CashPrizeMultiplier > 0
               || thisLevelReward.GoldPrizeMultiplier - lastLevelReward.GoldPrizeMultiplier > 0;
    }
}
