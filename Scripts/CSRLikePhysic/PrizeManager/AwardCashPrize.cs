using System;
using System.Collections.Generic;

public class AwardCashPrize : AwardSizedPrize
{
	private const string partialCashMetricsType = " cash";

	private Dictionary<RewardSize, Action> prizeRemoval;

	private int cashToReward;

    public AwardCashPrize(RewardSize size)
        : base(size)
	{
		Dictionary<RewardSize, Action> dictionary = new Dictionary<RewardSize, Action>();
		dictionary.Add(RewardSize.Tiny, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyCashRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Small, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallCashRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Medium, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumCashRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Large, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeCashRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Huge, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeCashRewardsRemaining--;
		});
		this.prizeRemoval = dictionary;
        //base..ctor(size);
		this.cashToReward = PrizeOMaticRewardsManager.CalculatePrizeomaticCashReward(this.prizeSize);
	}

	public override void AwardPrize()
	{
		PlayerProfileManager.Instance.ActiveProfile.AddCash(this.cashToReward,"reward", "AwardCashPrize");
	}

	public override string GetMetricsTypeString()
	{
		return this.cashToReward + " cash";
	}

    public override string GetPrizeString()
    {
        return CurrencyUtils.GetCashString(this.cashToReward);
    }

    public override void TakePrizeAwayFromProfile()
	{
		this.prizeRemoval[this.prizeSize]();
	}
}
