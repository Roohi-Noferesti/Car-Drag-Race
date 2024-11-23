using System;
using UnityEngine;

[Serializable]
public class DailyBattleReward
{
    public int RewardValue;

    public string RewardId;

    public DailyBattleRewardType RewardType;

    //public DailyBattleRewardType RewardType
    //{
    //    get { return EnumHelper.FromString<DailyBattleRewardType>(this.RewardTypeID); }
    //    private set { this.RewardType = value; }
    //}

    public SerializedTimeSpan CooldownTime;

    public int RewardIcon;

    public DailyBattleReward(int rewardValue, string rewardId, string rewardTypeId, TimeSpan cooldownTime,
        int rewardIcon)
    {
        this.RewardValue = rewardValue;
        this.RewardId = rewardId;
        //this.RewardTypeID = rewardTypeId;
        //this.GetCoolDownTime = cooldownTime;
        this.RewardIcon = rewardIcon;
    }

    public void ApplyToProfile(PlayerProfile profile)
    {
        DailyBattleRewardType rewardType = this.RewardType;
        switch (rewardType)
        {
            case DailyBattleRewardType.Cash:
                profile.AddCash(this.RewardValue,"reward","DailyBattleReward");
                break;
            case DailyBattleRewardType.Gold:
                profile.AddGold(this.RewardValue,"reward","DailyBattleReward");
                break;
            //case DailyBattleRewardType.Key:
            //    profile.AddKey(this.RewardValue);
            //    break;
        }
    }

    public void ConfirmInProfile(PlayerProfile profile)
    {
        DailyBattleRewardType rewardType = this.RewardType;
        if (rewardType != DailyBattleRewardType.Cash)
        {
            if (rewardType == DailyBattleRewardType.Gold)
            {
                profile.ConfirmDailyBattleGoldAwarded(this.RewardValue);
            }
        }
        else
        {
            profile.ConfirmDailyBattleCashAwarded(this.RewardValue);
        }
    }

    public void RemoveFromProfile(PlayerProfile profile, bool allowSubZeroDeduction)
    {
        Func<int, int> func = delegate(int playerAmount)
        {
            int num = (!allowSubZeroDeduction) ? Math.Min(playerAmount, this.RewardValue) : this.RewardValue;
            return -num;
        };
        DailyBattleRewardType rewardType = this.RewardType;
        if (rewardType != DailyBattleRewardType.Cash)
        {
            if (rewardType == DailyBattleRewardType.Gold)
            {
                profile.AddGold(func(profile.GetCurrentGold()),"reward","DailyBattleReward");
            }
        }
        else
        {
            profile.AddCash(func(profile.GetCurrentCash()),"reward","DailyBattleReward");
        }
    }

    public override string ToString()
    {
        return string.Concat(new string[]
        {
            this.RewardType.ToString(),
            " - ",
            this.RewardValue.ToString(),
            " - ",
            this.RewardId.ToString(),
            " - ",
            this.CooldownTime.TimeSpan.ToString()
        });
    }

    public string GetRewardText()
    {
        switch (this.RewardType)
        {
            case DailyBattleRewardType.Cash:
                return CurrencyUtils.GetCashString(this.RewardValue);
            case DailyBattleRewardType.Gold:
                return CurrencyUtils.GetCostStringBrief(0,this.RewardValue);
            case DailyBattleRewardType.Car:
                return CarDatabase.Instance.GetCarNiceName(this.RewardId);
            default:
                return string.Empty;
        }
    }
}


[Serializable]
public class SerializedTimeSpan
{
    [SerializeField] private int m_days;
    [SerializeField,Range(0,24)] private int m_hours;
    [SerializeField, Range(0, 60)] private int m_minutes;
    [SerializeField, Range(0, 60)] private int m_seconds;

    public TimeSpan TimeSpan
    {
        get { return new TimeSpan(m_days, m_hours, m_minutes, m_seconds); }
    }

    public SerializedTimeSpan()
    {
    }

    public SerializedTimeSpan(int day,int hour,int minutes,int seconds)
    {
        m_days = day;
        m_hours = hour;
        m_minutes = minutes;
        m_seconds = seconds;
    }
}
