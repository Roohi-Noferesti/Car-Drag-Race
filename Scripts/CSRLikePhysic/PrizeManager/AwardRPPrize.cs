using System;
using System.Collections.Generic;

public class AwardRPPrize : AwardSizedPrize
{
	//private Dictionary<RewardSize, Reward> sizeToRPReward = new Dictionary<RewardSize, Reward>
	//{
	//	{
	//		RewardSize.Tiny,
	//		Reward.RPTiny
	//	},
	//	{
	//		RewardSize.Small,
	//		Reward.RPSmall
	//	},
	//	{
	//		RewardSize.Medium,
	//		Reward.RPMedium
	//	},
	//	{
	//		RewardSize.Large,
	//		Reward.RPLarge
	//	},
	//	{
	//		RewardSize.Huge,
	//		Reward.RPHuge
	//	}
	//};

	private Dictionary<RewardSize, Action> prizeRemoval;

    public AwardRPPrize(RewardSize size)
        : base(size)
	{
		Dictionary<RewardSize, Action> dictionary = new Dictionary<RewardSize, Action>();
		dictionary.Add(RewardSize.Tiny, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyRPRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Small, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallRPRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Medium, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumRPRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Large, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeRPRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Huge, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeRPRewardsReamining--;
		});
		this.prizeRemoval = dictionary;
        //base..ctor(size);
	}

	public override void AwardPrize()
	{
        //RPBonusCard rPBonusCard = new RPBonusCard();
        //rPBonusCard.RewardSize = this.prizeSize;
        //rPBonusCard.Populate(null);
        //RPBonusManager.AddBonus(rPBonusCard);
	}

	public override void TakePrizeAwayFromProfile()
	{
		this.prizeRemoval[this.prizeSize]();
	}

	public override string GetMetricsTypeString()
	{
		return "RP Multiplayer";
	}

    public override string GetPrizeString()
    {
        throw new NotImplementedException();
    }
}
