using System;

[Serializable]
public class Tier1 : BaseCarTierEvents
{
    //public override string ToString()
    //{
    //    string str = "Car Tier 1 Events\n";
    //    return str + base.ToString();
    //}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_1;
	}
}
