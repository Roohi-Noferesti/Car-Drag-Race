using System;

public abstract class AwardSizedPrize : AwardPrizeBase
{
	protected RewardSize prizeSize;

	public AwardSizedPrize(RewardSize size)
	{
		this.prizeSize = size;
	}
}
