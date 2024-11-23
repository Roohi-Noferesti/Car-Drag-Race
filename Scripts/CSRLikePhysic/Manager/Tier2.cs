using System;

[Serializable]
public class Tier2 : BaseCarTierEvents
{
    //public override string ToString()
    //{
    //    string str = "Car Tier 2 Events\n";
    //    return str + base.ToString();
    //}

	public override eCarTier GetCarTier()
	{
		return eCarTier.TIER_2;
	}
}
