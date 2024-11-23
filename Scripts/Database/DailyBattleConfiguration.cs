using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBattleConfiguration : ScriptableObject
{
    public DailyBattleData DailyBattleData;
    public DailyReward[] DailyRewardCollection;
    public DailyBattle[] DailyBattleList;

}

[System.Serializable]
public class DailyBattleData
{
    public bool CheatsBattleOncePerDay;
    public bool AllowSubZeroPrizeDeductions;
    public string TimeBeforeOneRacePerDay;
    public string TimeBeforeTomorrowNotification;
    public int SessionRacesReminderThreshold;
}

[System.Serializable]
public class DailyReward
{
    public string PrizeType;
    public int PrizeValue;
    public string PrizeId;
    public string CooldownTime;
    public int IconId;
}

[System.Serializable]
public class DailyBattle
{
    public TierReward[] TierList;
}

[System.Serializable]
public class TierReward
{
    public int[] WinRewardList;
    public int[] LoseRewardList;
}


