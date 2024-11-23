using System;
using System.Collections.Generic;
using I2.Loc;

public class AwardGoldPrize : AwardSizedPrize
{
	private const string partialGoldMetricsType = " gold";

	private Dictionary<RewardSize, Action> prizeRemoval;

	private int goldToReward;

	public AwardGoldPrize(RewardSize size):base(size)
	{
		Dictionary<RewardSize, Action> dictionary = new Dictionary<RewardSize, Action>();
		dictionary.Add(RewardSize.Tiny, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyGoldRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Small, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallGoldRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Medium, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumGoldRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Large, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeGoldRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Huge, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeGoldRewardsRemaining--;
		});
		this.prizeRemoval = dictionary;
        //base..ctor(size);
		this.goldToReward = PrizeOMaticRewardsManager.CalculatePrizeomaticGoldReward(this.prizeSize);
	}

	public override void AwardPrize()
	{
		PlayerProfileManager.Instance.ActiveProfile.AddGold(this.goldToReward,"reward", "AwardCashPrize");
	}

	public override string GetMetricsTypeString()
	{
		return this.goldToReward + " gold";
	}

    public override string GetPrizeString()
    {
        return CurrencyUtils.GetGoldStringWithIcon(goldToReward);
    }

    public override void TakePrizeAwayFromProfile()
	{
		this.prizeRemoval[this.prizeSize]();
	}
}
