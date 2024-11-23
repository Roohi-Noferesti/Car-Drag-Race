using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FriendsConfiguration:ScriptableObject
{
	public enum IncentiveType
	{
		Cash,
		Gold,
		Car
	}

	public struct IncentiveData
	{
		public List<int> Index;

		public FriendsConfiguration.IncentiveType Type;

		public int Amount;

		public string Message;
	}

	public List<int> StarsRequiredToUnlockTier;

	public List<FriendsConfiguration.IncentiveData> Incentive;

	public List<int> GoldAwardForGoldStarByTier;

	public List<FriendsRaceEventReward> RaceEventRewards;

	public List<string> CarAwardForGoldStar;

	public string CarAwardForNStars;

	public int NumStarsForCarAward;

	public int InviteFriendsConditionMinCount;

	public int InviteFriendsConditionMaxTimes;

	public int RaceWithFriendConditionMaxTimes;

	public int BeatYourBestConditionMaxTimes;

	public int RaceTooHardConditionMaxTimes;

	public int InviteFriendsConditionFreq;

	public int BuyACarConditionFreq;

	public int RaceWithFriendConditionFreq;

	public int FriendsEasyRaceConditionFreq;

	public bool AllowUseMechanic;
}
