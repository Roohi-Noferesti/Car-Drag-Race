using System.Collections.Generic;
using System.Linq;
using Metrics;
using UnityEngine;

public static class StreakManager
{
    public enum eStreakResult
    {
        None,
        Won,
        Lost
    }

    public static StreakData StreakData;

    public static StreakChainManager Chain = new StreakChainManager();

    private static List<CachedOpponentInfo> cachedOpponents = new List<CachedOpponentInfo>();

    private static List<MetricsRaceStreakData> metricsRaceStreakData = new List<MetricsRaceStreakData>();

    private static int numberOfOpponents = 15;

    private static int currentStreak;

    private static int lastStreak;

    private static bool hasSentEndOfStreakEvent;

    private static int cashBank;

    private static int cardsBank;

    private static bool hasBanked;

    private static int refreshCount;

    private static int currentStreakRPDelta;

    private static float currentStreakDifficulty;

    private static int streakRescueCount;

    private static eStreakResult lastStreakResult = eStreakResult.None;

    public static float CurrentStreakDifficulty
    {
        get { return currentStreakDifficulty; }
        set { currentStreakDifficulty = value; }
    }

    public static void ResetStreak()
    {
        lastStreak = currentStreak;
        currentStreak = 0;
    }

    public static void ResetRefreshCost()
    {
        refreshCount = 0;
    }

    public static ModeInfo GetModeCostInfo(RaceEventData raceEvent)
    {
        if (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.EVENT)
        {
            return StreakData.EventInfo;
        }
        if (raceEvent.IsOnlineClubRacingEvent())
        {
            return StreakData.EliteClubInfo;
        }
        if (raceEvent.IsRaceTheWorldEvent())
        {
            return StreakData.RaceTheWorldInfo;
        }
        return null;
    }

    public static int CalculateEntryFeeCash(RaceEventData raceEvent, PlayerProfile activeProfile)
    {
        if (raceEvent == null)
        {
            return 0;
        }
        int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
        float num = GetMultiplierForPP(currentPPIndex);
        ModeInfo modeCostInfo = GetModeCostInfo(raceEvent);
        if (modeCostInfo != null)
        {
            num *= modeCostInfo.CashEntryMultiplier;
        }
        return ((int)num * StreakData.PlayerListRefreshCashCosts[refreshCount]).RoundTo(10);
    }

    public static int CalculateEntryFeeGold(RaceEventData raceEvent, PlayerProfile activeProfile)
    {
        if (raceEvent == null)
        {
            return 0;
        }
        ModeInfo modeCostInfo = GetModeCostInfo(raceEvent);
        if (modeCostInfo != null)
        {
            return modeCostInfo.GoldEntryFee;
        }
        return 0;
    }

    public static int CurrentEntryFeeCash()
    {
        return CalculateEntryFeeCash(RaceEventInfo.Instance.CurrentEvent, PlayerProfileManager.Instance.ActiveProfile);
    }

    public static int CurrentEntryFeeGold()
    {
        return CalculateEntryFeeGold(RaceEventInfo.Instance.CurrentEvent, PlayerProfileManager.Instance.ActiveProfile);
    }

    public static int RefreshCostCash()
    {
        if (MultiplayerUtils.IsMultiplayerObjectiveActive())
        {
            return 0;
        }
        if (StreakData.PlayerListRefreshCashCosts.Count == 0)
        {
            return 0;
        }
        return CurrentEntryFeeCash();
    }

    public static void UpdateRefreshCost()
    {
        if (StreakData.PlayerListRefreshCashCosts.Count == 0)
        {
            refreshCount = 0;
        }
        else
        {
            int count = StreakData.PlayerListRefreshCashCosts.Count;
            refreshCount++;
            if (refreshCount >= count)
            {
                refreshCount = count - 1;
            }
        }
    }

    public static void ResetBank()
    {
        ResetStreak();
        currentStreakRPDelta = 0;
        cashBank = 0;
        cardsBank = 0;
        hasBanked = false;
    }

    public static void Reset()
    {
        ResetStreak();
        ResetBank();
        ResetOpponentsList();
        ResetRefreshCost();
        Chain.Reset();
    }

    public static List<CachedOpponentInfo> CachedOpponents()
    {
        return cachedOpponents;
    }

    public static CachedOpponentInfo CachedOpponentAtIndex(int index)
    {
        return cachedOpponents[index];
    }

    public static int CurrentStreak()
    {
        return currentStreak;
    }

    public static int LastStreak()
    {
        return lastStreak;
    }

    public static eStreakResult GetLastStreakResult()
    {
        return lastStreakResult;
    }

    public static int NumberOfOpponents()
    {
        return numberOfOpponents;
    }

    public static int CashBonusForTier(int tier)
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
        int num = (int)(StreakData.StreakPrizes[tier].Cash * GetMultiplierForPP(currentPPIndex));
        ModeInfo modeCostInfo = GetModeCostInfo(RaceEventInfo.Instance.CurrentEvent);
        if (modeCostInfo != null)
        {
            num = (int)(num * modeCostInfo.CashRewardMultiplier);
        }
        return num.RoundTo(10);
    }

    public static int CashBonus()
    {
        return StreakData.StreakPrizes[currentStreak].Cash;
    }

    public static int CashBank()
    {
        return cashBank;
    }

    public static int CardBonusForTier(int tier)
    {
        return StreakData.StreakPrizes[tier].Cards;
    }

    public static int CardBonus()
    {
        return StreakData.StreakPrizes[currentStreak].Cards;
    }

    public static int CardsBank()
    {
        return cardsBank;
    }

    public static bool NoStreak()
    {
        return currentStreak == 0;
    }

    public static bool HasBanked()
    {
        return hasBanked;
    }

    public static int CurrentStreakRPDelta()
    {
        return currentStreakRPDelta;
    }

    public static int CurrentStreakRescueCount()
    {
        return streakRescueCount;
    }

    public static void StreakWasRescued()
    {
        streakRescueCount++;
    }

    private static void SendMetricsStreakEndedEvent(string streakResult, int streakRaceCount, int cashInStreak,
        int cardsInStreak)
    {
        if (hasSentEndOfStreakEvent)
        {
            return;
        }
        hasSentEndOfStreakEvent = true;
        string carDBKey = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey;
        Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
        {
            {
                Parameters.Result,
                streakResult
            },
            {
                Parameters.RaceCount,
                streakRaceCount.ToString()
            },
            {
                Parameters.PlayerCar,
                carDBKey
            },
            {
                Parameters.RPStreakDelta,
                currentStreakRPDelta.ToString()
            },
            {
                Parameters.WinningsCash,
                cashInStreak.ToString()
            },
            {
                Parameters.WinningsCards,
                cardsInStreak.ToString()
            },
            {
                Parameters.StreakType,
                MultiplayerUtils.GetMultiplayerStreakType()
            }
        };
        if (metricsRaceStreakData.Count == NumberOfOpponents())
        {
            dictionary[Parameters.Race1PPDelta] = metricsRaceStreakData[0].PPDifference.ToString();
            dictionary[Parameters.Race1RPDelta] = metricsRaceStreakData[0].RPDelta.ToString();
            dictionary[Parameters.Race1Result] = metricsRaceStreakData[0].Result;
            dictionary[Parameters.Race2PPDelta] = metricsRaceStreakData[1].PPDifference.ToString();
            dictionary[Parameters.Race2RPDelta] = metricsRaceStreakData[1].RPDelta.ToString();
            dictionary[Parameters.Race2Result] = metricsRaceStreakData[1].Result;
            dictionary[Parameters.Race3PPDelta] = metricsRaceStreakData[2].PPDifference.ToString();
            dictionary[Parameters.Race3RPDelta] = metricsRaceStreakData[2].RPDelta.ToString();
            dictionary[Parameters.Race3Result] = metricsRaceStreakData[2].Result;
            dictionary[Parameters.Race4PPDelta] = metricsRaceStreakData[3].PPDifference.ToString();
            dictionary[Parameters.Race4RPDelta] = metricsRaceStreakData[3].RPDelta.ToString();
            dictionary[Parameters.Race4Result] = metricsRaceStreakData[3].Result;
            dictionary[Parameters.Race5PPDelta] = metricsRaceStreakData[4].PPDifference.ToString();
            dictionary[Parameters.Race5RPDelta] = metricsRaceStreakData[4].RPDelta.ToString();
            dictionary[Parameters.Race5Result] = metricsRaceStreakData[4].Result;
            dictionary[Parameters.Race6PPDelta] = metricsRaceStreakData[5].PPDifference.ToString();
            dictionary[Parameters.Race6RPDelta] = metricsRaceStreakData[5].RPDelta.ToString();
            dictionary[Parameters.Race6Result] = metricsRaceStreakData[5].Result;
        }
        Log.AnEvent(Events.WinStreakComplete, dictionary);
        streakRescueCount = 0;
    }

    public static void LoseStreak()
    {
        SendMetricsStreakEndedEvent("0", currentStreak, 0, 0);
        PlayerProfileManager.Instance.ActiveProfile.TotalMultiplayerStreaksLost++;
        Reset();
        lastStreakResult = eStreakResult.Lost;
    }

    public static void UpdateMetricsOnBankit()
    {
        if (currentStreak >= 1)
        {
            MetricsRaceStreakData value;
            value.Result = "B";
            value.RPDelta = NetworkReplayManager.Instance.Response.deltaRP;
            value.PPDifference = RaceResultsTracker.You.PerformancePotential - RaceResultsTracker.Them.PerformancePotential;
            metricsRaceStreakData[currentStreak - 1] = value;
            for (int i = currentStreak; i < metricsRaceStreakData.Count; i++)
            {
                value.Result = "NA";
                value.RPDelta = 0;
                value.PPDifference = 0;
                metricsRaceStreakData[i] = value;
            }
            SendMetricsStreakEndedEvent("1", currentStreak, cashBank, cardsBank);
        }
    }

    public static void BankIt()
    {
        if (NoStreak())
        {
            return;
        }
        hasBanked = true;
        lastStreakResult = eStreakResult.Won;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        activeProfile.TotalMultiplayerStreaksCompleted++;
        if (activeProfile.BestEverMultiplayerWinStreakBanked < activeProfile.BestEverMultiplayerWinStreak)
        {
            activeProfile.BestEverMultiplayerWinStreakBanked = activeProfile.BestEverMultiplayerWinStreak;
        }
        activeProfile.AddCash(cashBank,"Streak","bank");
        ResetOpponentsList();
        if (currentStreak == NumberOfOpponents())
        {
            PrizeProgression.AddProgress(PrizeProgressionType.StreaksCompleted, 1f);
        }
        PrizeProgression.AddProgress(PrizeProgressionType.CashWon, cashBank);
    }

    public static void UpdateStreak(bool isWinner)
    {
        if (currentStreak < metricsRaceStreakData.Count)
        {
            MetricsRaceStreakData value;
            value.Result = ((!isWinner) ? "L" : "W");
            value.RPDelta = NetworkReplayManager.Instance.Response.deltaRP;
            value.PPDifference = RaceResultsTracker.You.PerformancePotential - RaceResultsTracker.Them.PerformancePotential;
            metricsRaceStreakData[currentStreak] = value;
        }
        currentStreakRPDelta += NetworkReplayManager.Instance.Response.deltaRP;
        if (isWinner)
        {
            cashBank = CashBonusForTier(currentStreak);
            cardsBank += CardBonusForTier(currentStreak);
            currentStreak++;
            if (currentStreak == NumberOfOpponents())
            {
                SendMetricsStreakEndedEvent("2", currentStreak, cashBank, cardsBank);
                PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak = true;
                Chain.StreakWon();
            }
        }
    }

    public static void ResetOpponentsList()
    {
        DifficultyManager.OnStreakFinished();
        metricsRaceStreakData.Clear();
        cachedOpponents.Clear();
    }

    public static bool OpponentListIsEmpty()
    {
        return cachedOpponents.Count == 0;
    }

    public static bool AllOpponentsDefeated()
    {
        int num = 0;
        foreach (CachedOpponentInfo current in cachedOpponents)
        {
            if (current.Defeated)
            {
                num++;
            }
        }
        return cachedOpponents.Count == num;
    }

    public static void AddOpponent(CachedOpponentInfo opponent)
    {
        cachedOpponents.Add(opponent);
        MetricsRaceStreakData item;
        item.Result = "NA";
        item.PPDifference = 0;
        item.RPDelta = 0;
        metricsRaceStreakData.Add(item);
        hasSentEndOfStreakEvent = false;
    }

    public static void SetDefeated(int index, bool zValue)
    {
        CachedOpponentInfo value = default(CachedOpponentInfo);
        value = cachedOpponents[index];
        value.Defeated = zValue;
        cachedOpponents[index] = value;
    }

    public static void SetAnimated(int index, bool zValue)
    {
        CachedOpponentInfo value = default(CachedOpponentInfo);
        value = cachedOpponents[index];
        value.Animated = zValue;
        cachedOpponents[index] = value;
    }

    public static float GetMultiplierForPP(int pp)
    {
        int minPPForTier = 0;
        int maxPPForTier = 0;
        eCarTier carTierFromPPIndex = CarPerformanceIndexCalculator.GetCarTierFromPPIndex(pp, out minPPForTier,
            out maxPPForTier);
        //if (carTierFromPPIndex >= (eCarTier)StreakManager.StreakData.TierBonusMultipliers.Count)
        //{
        //    return 0f;
        //}
        //float lower = 0;//StreakManager.StreakData.TierBonusMultipliers[(int)carTierFromPPIndex].Lower;
        //float upper = 0;//StreakManager.StreakData.TierBonusMultipliers[(int)carTierFromPPIndex].Upper;
        //int ppDiffForTier = maxPPForTier - minPPForTier;
        //float num4 = upper - lower;
        //return num4/(float) ppDiffForTier*(float) (pp - minPPForTier); // + lower;
        switch (carTierFromPPIndex)
        {
            case eCarTier.TIER_1:
                return GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.Tier1CashMultiplier;
            case eCarTier.TIER_2:
                return GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.Tier2CashMultiplier;
            case eCarTier.TIER_3:
                return GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.Tier3CashMultiplier;
            case eCarTier.TIER_4:
                return GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.Tier4CashMultiplier;
            case eCarTier.TIER_5:
                return GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.Tier5CashMultiplier;
        }
        return 1;
    }

    public static int GetStreakRescueCost(bool isBloggerEquipped, bool withAdverts,
        out StreakRescueCostData.StreakRescueCostType costType)
    {
        int result = 0;
        bool useBloggerDiscount = StreakData.StreakRescue.UseBloggerDiscount;
        costType = StreakRescueCostData.StreakRescueCostType.COST_CASH;
        int b = 0;
        StreakData.StreakRescue.RaceRescueCost.MaxValue((StreakRescueCostData q) => q.StreakChainLength);
        int chainLength = Mathf.Min(Chain.ChainCount, b);
        StreakRescueCostData streakRescueCostData =
            StreakData.StreakRescue.RaceRescueCost.FirstOrDefault(
                (StreakRescueCostData q) => q.StreakChainLength == chainLength && q.StreakRaceIndex == currentStreak);
        if (streakRescueCostData == null)
        {
            return 0;
        }
        costType = streakRescueCostData.CostTypeEnum;
        switch (costType)
        {
            case StreakRescueCostData.StreakRescueCostType.COST_CASH:
                result = ((!isBloggerEquipped || !useBloggerDiscount)
                    ? streakRescueCostData.CashCost
                    : streakRescueCostData.CashCostWithBlogger);
                break;
            case StreakRescueCostData.StreakRescueCostType.COST_GOLD:
                result = ((!isBloggerEquipped || !useBloggerDiscount)
                    ? streakRescueCostData.GoldCost
                    : streakRescueCostData.GoldCostWithBlogger);
                break;
            case StreakRescueCostData.StreakRescueCostType.COST_AD:
                if (!withAdverts)
                {
                    result = ((!isBloggerEquipped || !useBloggerDiscount)
                        ? streakRescueCostData.CashCost
                        : streakRescueCostData.CashCostWithBlogger);
                }
                break;
        }
        return result;
    }
}
