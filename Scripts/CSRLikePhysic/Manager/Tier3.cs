using System;

[Serializable]
public class Tier3 : BaseCarTierEvents
{
	public override string ToString()
	{
		string str = "Car Tier 3 Events\n";
		return str + base.ToString();
	}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_3;
	}
}
