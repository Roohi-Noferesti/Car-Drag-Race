using System;

[Serializable]
public class Tier5 : BaseCarTierEvents
{
	public override string ToString()
	{
		string str = "Car Tier 5 Events\n";
		return str + base.ToString();
	}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_5;
	}
}
