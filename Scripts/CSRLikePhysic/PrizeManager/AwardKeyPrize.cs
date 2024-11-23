using System;
using System.Collections.Generic;
using I2.Loc;

public class AwardKeyPrize : AwardSizedPrize
{
	private Dictionary<RewardSize, Action> prizeRemoval;

	private int keyToReward;

    public AwardKeyPrize(RewardSize size)
        : base(size)
	{
		Dictionary<RewardSize, Action> dictionary = new Dictionary<RewardSize, Action>();
		dictionary.Add(RewardSize.Tiny, delegate
		{
            PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyKeyRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Small, delegate
		{
            PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallKeyRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Medium, delegate
		{
            PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumKeyRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Large, delegate
		{
            PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeKeyRewardsRemaining--;
		});
		dictionary.Add(RewardSize.Huge, delegate
		{
            PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeKeyRewardsRemaining--;
		});
		this.prizeRemoval = dictionary;
        //base..ctor(size);
		this.keyToReward = PrizeOMaticRewardsManager.CalculatePrizeomaticKeyReward(this.prizeSize);
	}

	public override void AwardPrize()
	{
        PlayerProfileManager.Instance.ActiveProfile.AddGachaKeys(this.keyToReward,GachaType.Silver, EGachaKeysEarnedReason.Unknown);
	}

	public override string GetMetricsTypeString()
	{
        return this.keyToReward + " key";
	}

    public override string GetPrizeString()
    {
        return CurrencyUtils.GetKeyString(keyToReward);
    }

    public override void TakePrizeAwayFromProfile()
	{
		this.prizeRemoval[this.prizeSize]();
	}
}
