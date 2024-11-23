using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class FriendsDatabase : ConfigurationAssetLoader
{
	public FriendsConfiguration Configuration
	{
		get;
		private set;
	}

	public FriendsDatabase() : base(GTAssetTypes.configuration_file, "FriendsConfiguration")
	{
		this.Configuration = null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<FriendsConfiguration>(assetDataString);
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (FriendsConfiguration)scriptableObject;
    }

	public int StarsRequiredToUnlockTier(eCarTier tier)
	{
		return this.Configuration.StarsRequiredToUnlockTier[(int)tier];
	}

	public bool FriendCountHasPrize(int friendCount)
	{
		bool result = false;
		if (this.Configuration.Incentive != null)
		{
			result = this.Configuration.Incentive.Exists((FriendsConfiguration.IncentiveData p) => p.Index.Contains(friendCount));
		}
		return result;
	}

	public string GetFriendIncentiveText(int friendCount)
	{
		FriendsConfiguration.IncentiveData incentiveData = this.Configuration.Incentive.Find((FriendsConfiguration.IncentiveData p) => p.Index.Contains(friendCount));
		return string.Format(LocalizationManager.GetTranslation(incentiveData.Message), incentiveData.Amount, friendCount);
	}



	public FriendsConfiguration.IncentiveData GetFriendIncentiveData(int friendCount)
	{
		return this.Configuration.Incentive.Find((FriendsConfiguration.IncentiveData p) => p.Index.Contains(friendCount));
	}

	public int GoldAwardForGoldStarByTier(eCarTier tier)
	{
		return this.Configuration.GoldAwardForGoldStarByTier[(int)tier];
	}

	public int GetRaceReward(eCarTier tier, StarType star)
	{
		return this.Configuration.RaceEventRewards.Find((FriendsRaceEventReward q) => q.Tier == tier && q.Star == star).GetCashReward();
	}

	public int GetFirstAchievementRaceReward(eCarTier tier, StarType star)
	{
		return this.Configuration.RaceEventRewards.Find((FriendsRaceEventReward q) => q.Tier == tier && q.Star == star).FirstTimeAchievedCashPrize;
	}

	public List<string> AllRewardableCars()
	{
		foreach (string current in this.Configuration.CarAwardForGoldStar)
		{
			CarDatabase.Instance.GetCar(current);
		}
		return this.Configuration.CarAwardForGoldStar;
	}

	public string CarRewardForNStars()
	{
		return this.Configuration.CarAwardForNStars;
	}

	public int NumStarsForCarReward()
	{
		return this.Configuration.NumStarsForCarAward;
	}

	public int InviteFriendsConditionMinCount()
	{
		return this.Configuration.InviteFriendsConditionMinCount;
	}

	public int InviteFriendsConditionMaxTimes()
	{
		return this.Configuration.InviteFriendsConditionMaxTimes;
	}

	public int RaceWithFriendConditionMaxTimes()
	{
		return this.Configuration.RaceWithFriendConditionMaxTimes;
	}

	public int BeatYourBestConditionMaxTimes()
	{
		return this.Configuration.BeatYourBestConditionMaxTimes;
	}

	public int RaceTooHardConditionMaxTimes()
	{
		return this.Configuration.RaceTooHardConditionMaxTimes;
	}

	public int InviteFriendsConditionFreq()
	{
		return this.Configuration.InviteFriendsConditionFreq;
	}

	public int BuyACarConditionFreq()
	{
		return this.Configuration.BuyACarConditionFreq;
	}

	public int RaceWithFriendConditionFreq()
	{
		return this.Configuration.RaceWithFriendConditionFreq;
	}

	public int FriendsEasyRaceConditionFreq()
	{
		return this.Configuration.FriendsEasyRaceConditionFreq;
	}

	public bool AllowUseMechanic()
	{
		return this.Configuration.AllowUseMechanic;
	}
}
