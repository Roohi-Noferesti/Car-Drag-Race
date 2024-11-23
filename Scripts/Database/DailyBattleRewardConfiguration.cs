using System;
using UnityEngine;

public class DailyBattleRewardConfiguration : ScriptableObject
{
    public bool CheatsBattleOncePerDay;
    public bool AllowSubZeroPrizeDeductions;
    public SerializedTimeSpan TimeBeforeOneRacePerDay;
    public SerializedTimeSpan TimeBeforeTomorrowNotification;
    public int SessionRacesReminderThreshold;
    public DailyBattleRewardManager.RewardContainer[] RewardContainer;
}
