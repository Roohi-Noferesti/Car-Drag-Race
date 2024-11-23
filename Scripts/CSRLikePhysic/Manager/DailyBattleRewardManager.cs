using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DailyBattleRewardManager : MonoBehaviour
{
    [Serializable]
    public class TierReward
	{
		public List<DailyBattleReward> WinRewardList;

		public List<DailyBattleReward> LoseRewardList;

		public TierReward(List<DailyBattleReward> winRewardList, List<DailyBattleReward> loseRewardList)
		{
			this.WinRewardList = winRewardList;
			this.LoseRewardList = loseRewardList;
		}
	}

    [Serializable]
    public class RewardContainer
	{
        public string Name = string.Empty;

        public string NextDaysRewardName;

        public string FarRewardName;

        public int FarRewardLimit;

		public TierReward[] TierRewards = new TierReward[6];
	}

	private const int m_MaxRewardCacheSize = 3;

	private RewardContainer m_FirstReward;

	private Dictionary<int, RewardContainer> m_RewardCache = new Dictionary<int, RewardContainer>();

	public static DailyBattleRewardManager Instance
	{
		get;
		private set;
	}

	public bool CheatsBattleOncePerDay
	{
		get;
		private set;
	}

	public bool AllowSubZeroPrizeDeductions
	{
		get;
		private set;
	}

	public TimeSpan TimeBeforeOneRacePerDay
	{
		get;
		private set;
	}

	public TimeSpan TimeBeforeTomorrowNotification
	{
		get;
		private set;
	}

	public int SessionRacesReminderThreshold
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	public void StartUp()
	{
        //this.InitFromJson();
	    m_FirstReward = GameDatabase.Instance.DailyBattle.Configuration.RewardContainer[0];
	    this.CheatsBattleOncePerDay = GameDatabase.Instance.DailyBattle.Configuration.CheatsBattleOncePerDay;
	    this.AllowSubZeroPrizeDeductions = GameDatabase.Instance.DailyBattle.Configuration.AllowSubZeroPrizeDeductions;
	    this.TimeBeforeOneRacePerDay = GameDatabase.Instance.DailyBattle.Configuration.TimeBeforeOneRacePerDay.TimeSpan;
	    this.TimeBeforeTomorrowNotification =
	        GameDatabase.Instance.DailyBattle.Configuration.TimeBeforeTomorrowNotification.TimeSpan;
	    this.SessionRacesReminderThreshold = GameDatabase.Instance.DailyBattle.Configuration.SessionRacesReminderThreshold;
	}

    //private void InitFromJson()
    //{
        //JsonDict jsonData = GameDatabase.Instance.DailyBattle.JsonData;
        //JsonDict jsonDict = jsonData.GetJsonDict("DailyBattleData");
        //this.CheatsBattleOncePerDay = jsonDict.GetBool("CheatsBattleOncePerDay");
        //this.AllowSubZeroPrizeDeductions = jsonDict.GetBool("AllowSubZeroPrizeDeductions");
        //this.TimeBeforeOneRacePerDay = TimeSpan.Parse(jsonDict.GetString("TimeBeforeOneRacePerDay"));
        //this.TimeBeforeTomorrowNotification = TimeSpan.Parse(jsonDict.GetString("TimeBeforeTomorrowNotification"));
        //this.SessionRacesReminderThreshold = jsonDict.GetInt("SessionRacesReminderThreshold");
        //JsonList jsonList = jsonData.GetJsonList("DailyRewardCollection");
        //List<DailyBattleReward> list = new List<DailyBattleReward>();
        //for (int i = 0; i < jsonList.Count; i++)
        //{
        //    JsonDict jsonDict2 = jsonList.GetJsonDict(i);
        //    DailyBattleReward item = new DailyBattleReward(jsonDict2.GetInt("PrizeValue"), jsonDict2.GetString("PrizeId"), jsonDict2.GetString("PrizeType"), TimeSpan.Parse(jsonDict2.GetString("CooldownTime")), jsonDict2.GetInt("IconId"));
        //    list.Add(item);
        //}
        //Dictionary<string, DailyBattleRewardManager.RewardContainer> dictionary = new Dictionary<string, DailyBattleRewardManager.RewardContainer>();
        //List<DailyBattleRewardManager.RewardContainer> list2 = new List<DailyBattleRewardManager.RewardContainer>();
        //JsonList jsonList2 = jsonData.GetJsonList("DailyBattleList");
        //for (int j = 0; j < jsonList2.Count; j++)
        //{
        //    JsonDict jsonDict3 = jsonList2.GetJsonDict(j);
        //    DailyBattleRewardManager.RewardContainer rewardContainer = this.ReadRewardContainer(jsonDict3, list);
        //    if (rewardContainer.Name != string.Empty && !dictionary.ContainsKey(rewardContainer.Name))
        //    {
        //        dictionary.Add(rewardContainer.Name, rewardContainer);
        //    }
        //    list2.Add(rewardContainer);
        //}
        //for (int k = 0; k < list2.Count; k++)
        //{
        //    DailyBattleRewardManager.RewardContainer rewardContainer2 = list2[k];
        //    JsonDict jsonDict4 = jsonList2.GetJsonDict(k);
        //    if (jsonDict4.ContainsKey("NextReward"))
        //    {
        //        string @string = jsonDict4.GetString("NextReward");
        //        if (dictionary.ContainsKey(@string))
        //        {
        //            rewardContainer2.NextDaysReward = dictionary[@string];
        //        }
        //    }
        //    else if (k < list2.Count - 1)
        //    {
        //        rewardContainer2.NextDaysReward = list2[k + 1];
        //    }
        //    if (jsonDict4.ContainsKey("FarReward"))
        //    {
        //        string string2 = jsonDict4.GetString("FarReward");
        //        if (dictionary.ContainsKey(string2))
        //        {
        //            rewardContainer2.FarReward = dictionary[string2];
        //        }
        //    }
        //}
        //this.m_FirstReward = list2[0];
    //}

    //private List<DailyBattleReward> ReadAwardList(JsonList awardList, List<DailyBattleReward> dailyBattleRewardList)
    //{
    //    List<DailyBattleReward> list = new List<DailyBattleReward>();
    //    for (int i = 0; i < awardList.Count; i++)
    //    {
    //        int @int = awardList.GetInt(i);
    //        if (@int < dailyBattleRewardList.Count)
    //        {
    //            list.Add(dailyBattleRewardList[@int]);
    //        }
    //    }
    //    return list;
    //}

    //private RewardContainer ReadRewardContainer(JsonDict dayDict, List<DailyBattleReward> dailyBattleRewardList)
    //{
    //    RewardContainer rewardContainer = new DailyBattleRewardManager.RewardContainer();
    //    if (dayDict.ContainsKey("Name"))
    //    {
    //        rewardContainer.Name = dayDict.GetString("Name");
    //    }
    //    if (dayDict.ContainsKey("FarRewardLimit"))
    //    {
    //        rewardContainer.FarRewardLimit = dayDict.GetInt("FarRewardLimit");
    //    }
    //    JsonList jsonList = dayDict.GetJsonList("TierList");
    //    for (int i = 0; i < jsonList.Count; i++)
    //    {
    //        JsonDict jsonDict = jsonList.GetJsonDict(i);
    //        JsonList jsonList2 = jsonDict.GetJsonList("WinRewardList");
    //        JsonList jsonList3 = jsonDict.GetJsonList("LoseRewardList");
    //        List<DailyBattleReward> winRewardList = this.ReadAwardList(jsonList2, dailyBattleRewardList);
    //        List<DailyBattleReward> loseRewardList = this.ReadAwardList(jsonList3, dailyBattleRewardList);
    //        rewardContainer.TierRewards[i] = new DailyBattleRewardManager.TierReward(winRewardList, loseRewardList);
    //    }
    //    return rewardContainer;
    //}

    private RewardContainer GetNextReward(RewardContainer rc, int consecutiveDays)
    {
        if (!string.IsNullOrEmpty(rc.FarRewardName) && consecutiveDays > rc.FarRewardLimit)
        {
            var reward =  GetRewardByName(rc.FarRewardName);
            //Debug.Log("far reward " + reward.Name + "  at day " + consecutiveDays);
            return reward;
        }

        var reward1 =  GetRewardByName(rc.NextDaysRewardName);
        //Debug.Log("near reward " + reward1.Name + "  at day " + consecutiveDays);
        return reward1;
    }

    private RewardContainer GetRewardByName(string rewardName)
    {
        return GameDatabase.Instance.DailyBattle.Configuration.RewardContainer.FirstOrDefault(r => r.Name == rewardName);
    }

    private DailyBattleReward PickReward(List<DailyBattleReward> rewardList)
	{
		foreach (DailyBattleReward current in rewardList)
		{
		    switch (current.RewardType)
		    {
		        case DailyBattleRewardType.Cash:
		        case DailyBattleRewardType.Gold:
		            return current;
		    }
		}
		return null;
	}

	private RewardContainer GetRewardContainer(int consecutiveDays)
	{
        if (this.m_RewardCache.ContainsKey(consecutiveDays))
        {
            return this.m_RewardCache[consecutiveDays];
        }
        RewardContainer rewardContainer = this.m_FirstReward;
        DateTime now = GTDateTime.Now;
        for (int i = 0; i < consecutiveDays; i++)
        {
            rewardContainer = this.GetNextReward(rewardContainer, consecutiveDays);
        }
        this.m_RewardCache.Add(consecutiveDays, rewardContainer);
        if (this.m_RewardCache.Count > 3)
        {
            var key = this.m_RewardCache.Keys.Min();
            this.m_RewardCache.Remove(key);
        }
        return rewardContainer;
	}

	private DailyBattleReward GetReward(RewardContainer rewardContainer, eCarTier tier, bool playerWon)
	{
		var tierReward = rewardContainer.TierRewards[(int)tier];
		return this.PickReward((!playerWon) ? tierReward.LoseRewardList : tierReward.WinRewardList);
	}

	public DailyBattleReward GetReward(int consecutiveDays, eCarTier tier, bool playerWon)
	{
		consecutiveDays--;
		var rewardContainer = this.GetRewardContainer(consecutiveDays);
		return this.GetReward(rewardContainer, tier, playerWon);
	}

	public IEnumerable<DailyBattleReward> Sequence(int consecutiveDays, eCarTier tier, bool playerWon)
	{
	    consecutiveDays--;
	    var casheContainer = GetRewardContainer(consecutiveDays);

        while (true)
        {
            yield return GetReward(casheContainer, tier, playerWon);
            casheContainer = GetNextReward(casheContainer, consecutiveDays);
            consecutiveDays++;
        }
	}
}
