using System;
using System.Linq;
using UnityEngine;

public class Z2HDifficultyConfiguration : ScriptableObject
{
    [Serializable]
    public class DifficultyPPSetting
    {
        public AutoDifficulty.DifficultyRating DifficultyRating;
        public int Tier1MinPPDelta;
        public int Tier1MaxPPDelta;
        public int Tier2MinPPDelta;
        public int Tier2MaxPPDelta;
        public int Tier3MinPPDelta;
        public int Tier3MaxPPDelta;
        public int Tier4MinPPDelta;
        public int Tier4MaxPPDelta;
        public int Tier5MinPPDelta;
        public int Tier5MaxPPDelta;

        public int GetMinDeltaForTier(eCarTier carTier)
        {
            switch (carTier)
            {
                    case eCarTier.TIER_1:
                    return Tier1MinPPDelta;
                    case eCarTier.TIER_2:
                    return Tier2MinPPDelta;
                    case eCarTier.TIER_3:
                    return Tier3MinPPDelta;
                    case eCarTier.TIER_4:
                    return Tier4MinPPDelta;
                    case eCarTier.TIER_5:
                    return Tier5MinPPDelta;
            }
            return Tier1MinPPDelta;
        }


        public int GetMaxDeltaForTier(eCarTier carTier)
        {
            switch (carTier)
            {
                case eCarTier.TIER_1:
                    return Tier1MaxPPDelta;
                case eCarTier.TIER_2:
                    return Tier2MaxPPDelta;
                case eCarTier.TIER_3:
                    return Tier3MaxPPDelta;
                case eCarTier.TIER_4:
                    return Tier4MaxPPDelta;
                case eCarTier.TIER_5:
                    return Tier5MaxPPDelta;
            }
            return Tier1MaxPPDelta;
        }
    }

    [SerializeField] private DifficultyPPSetting[] m_difficulties;

    [SerializeField] private DifficultyPPSetting[] m_staticDifficulties;

    [SerializeField] private int m_minPPStep = 3;
    [SerializeField] private int m_maxPPStep = 3;
    [Space(20)]
    public int EasyLabelPPIndex = 25;
    public int ChallengingLabelPPIndex = 5;
    public int DifficultLabelPPIndex = -10;
    public bool UseEqualOrLowerTierForMatchMaking;
    public bool UseRelativeDifficulty;

    public int MinPPStep
    {
        get
        {
            return m_minPPStep;
        }
    }

    public int MaxPPStep
    {
        get
        {
            return m_maxPPStep;
        }
    }

    public DifficultyPPSetting this[AutoDifficulty.DifficultyRating difficultyRating]
    {
        get { return m_difficulties.FirstOrDefault(d => d.DifficultyRating == difficultyRating); }
    }

    public DifficultyPPSetting GetStaticDifficultiy(AutoDifficulty.DifficultyRating difficultyRating)
    {
        return m_staticDifficulties.FirstOrDefault(d => d.DifficultyRating == difficultyRating);
    }

    public DifficultyPPSetting GetDifficultySetting(int ppDiff,eCarTier carTier)
    {
        switch (carTier)
        {
                case eCarTier.TIER_1:
                return m_difficulties.FirstOrDefault(d => d.Tier1MaxPPDelta >= ppDiff && d.Tier1MinPPDelta <= ppDiff);
                case eCarTier.TIER_2:
                return m_difficulties.FirstOrDefault(d => d.Tier2MaxPPDelta >= ppDiff && d.Tier2MinPPDelta <= ppDiff);
                case eCarTier.TIER_3:
                return m_difficulties.FirstOrDefault(d => d.Tier3MaxPPDelta >= ppDiff && d.Tier3MinPPDelta <= ppDiff);
                case eCarTier.TIER_4:
                return m_difficulties.FirstOrDefault(d => d.Tier4MaxPPDelta >= ppDiff && d.Tier4MinPPDelta <= ppDiff);
                case eCarTier.TIER_5:
                return m_difficulties.FirstOrDefault(d => d.Tier5MaxPPDelta >= ppDiff && d.Tier5MinPPDelta <= ppDiff);
        }
        return null;
    }
}
