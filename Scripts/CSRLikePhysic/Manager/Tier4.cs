using System;

[Serializable]
public class Tier4 : BaseCarTierEvents
{
	public override string ToString()
	{
		string str = "Car Tier 4 Events\n";
		return str + base.ToString();
	}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_4;
	}
}
