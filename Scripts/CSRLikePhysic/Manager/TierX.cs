using System;

[Serializable]
public class TierX : BaseCarTierEvents
{
	public override string ToString()
	{
		string str = "Car Tier X Events\n";
		return str + base.ToString();
	}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_X;
	}
}
