using System.Globalization;
using I2.Loc;
using UnityEngine;

public class RaceRewards_BonusBlockDailyBattle : RaceRewards_BonusBlockDefault
{
	private const int m_GoldToCashMultiplier = 550;

    public override void Setup(RaceResultsTrackerState resultsData)
    {
        base.Setup(resultsData);
        RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        bool isWinner = resultsData.You.IsWinner;
        bool greatLaunch = resultsData.You.GreatLaunch;
        this.SetPlayerWon(resultsData.You.IsWinner);
        int numberOfOptimalChanges = resultsData.You.NumberOfOptimalChanges;
        int numberOfGoodChanges = resultsData.You.NumberOfGoodChanges;
        DailyBattleReward reward =
            DailyBattleRewardManager.Instance.GetReward(
                PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount,
                RaceEventQuery.Instance.getHighestUnlockedClass(), resultsData.You.IsWinner);
        int rewardAmount = (reward.RewardType != DailyBattleRewardType.Cash) ? 0 : reward.RewardValue;
        int goldPrize = (reward.RewardType != DailyBattleRewardType.Gold) ? 0 : reward.RewardValue;
        int cashPrize = (reward.RewardType != DailyBattleRewardType.Cash) ? (reward.RewardValue * 550) : reward.RewardValue;
        RaceEventTypeMultipliers raceEventTypeRewardMultipliers =
            GameDatabase.Instance.Currencies.getRaceEventTypeRewardMultipliers(currentEvent);
        if (greatLaunch)
        {
            this.Bonus_2_PrizeVal =
                (int)(cashPrize * raceEventTypeRewardMultipliers.PerfectStartCashMultiplier);
        }
        //this.Bonus_2_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_PERFECT_START");
        //Level Reward
        this.Bonus_2_Prize.text = CurrencyUtils.GetCashString(this.Bonus_2_PrizeVal).ToNativeNumber();
        this.TotalCash += this.Bonus_2_PrizeVal;

        this.Bonus_3_PrizeVal +=
            (int)((float)cashPrize * raceEventTypeRewardMultipliers.OptimalShiftsCashMultiplier) *
            numberOfOptimalChanges;
        this.Bonus_3_PrizeVal += (int)((float)cashPrize * raceEventTypeRewardMultipliers.GoodShiftsCashMultiplier) *
                                 numberOfGoodChanges;
        //this.Bonus_3_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_PERFECT_SHIFTS");
        this.Bonus_3_Prize.text = CurrencyUtils.GetCashString(this.Bonus_3_PrizeVal).ToNativeNumber();
        this.TotalCash += this.Bonus_3_PrizeVal;

        //this.Bonus_4_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_GOOD_SHIFTS");
        //this.Bonus_4_Prize.text = CurrencyUtils.GetShortCashString(this.Bonus_4_PrizeVal);
        //this.TotalCash += this.Bonus_4_PrizeVal;
        //CarGarageInstance humanCarGarageInstance = RaceEventInfo.Instance.HumanCarGarageInstance;
        //bool flag = humanCarGarageInstance.AppliedLiveryName != null && humanCarGarageInstance.AppliedLiveryName.Length > 0;
        //if (resultsData.You.IsWinner && flag)
        //{
        //    this.Bonus_5_PrizeVal = this.DetermineLiveryCashRewardForDailyBattles(currentEvent, humanCarGarageInstance.AppliedLiveryName);
        //    this.TotalCash += this.Bonus_5_PrizeVal;
        //}
        carGarageInstance = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        if (RemoteConfigABTest.CheckRemoteConfigValue())
        {
            FindSumOfCarBonuses(carGarageInstance, out SumBonus);
            Bonus_5_PrizeVal = SumBonus;
            this.Bonus_5_Name.text = LocalizationManager.GetTranslation("TEXT_REWARD_SCREEN_BONUS_REWARD");
            Bonus_5_Prize.text = CurrencyUtils.GetCashString(Bonus_5_PrizeVal).ToNativeNumber();
        }
        //this.Bonus_1_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_RACE_PRIZE");
        this.Bonus_1_Prize.text = CurrencyUtils.GetCashString(rewardAmount).ToNativeNumber();
        this.Bonus_1_PrizeVal = rewardAmount;
        this.TotalCash += this.Bonus_1_PrizeVal;
        if (RemoteConfigABTest.CheckRemoteConfigValue())
        {
            if (resultsData.You.IsWinner)
            {
                TotalCash += Bonus_5_PrizeVal;
            }
        }
        this.TotalGold += goldPrize;
        this.TotalXP = GameDatabase.Instance.XPEvents.GetXPPrizeForRaceComplete(isWinner);
        var leagueName = GameDatabase.Instance.StarDatabase.GetPlayerLeague();
        var starReward = currentEvent.RaceReward.RaceStarReward.GetLeagueRewardByLeagueName(leagueName);
        this.TotalStar = resultsData.You.IsWinner
            ? starReward.WinStar
            : starReward.LoseStar;

        base.SetTotalCash(this.TotalCash);
        base.SetBonusGold(this.TotalGold);
        var dailyBattlesLastEventAt = GTDateTime.Now;
        PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt = dailyBattlesLastEventAt;
    }
    
}
