using System;
using System.Collections.Generic;

public static class CarTierHelper
{
	public static List<string> TierToString = new List<string>
	{
		"1",
		"2",
		"3",
		"4",
		"5",
		"X"
	};

	public static List<eCarTier> Tier1To5 = new List<eCarTier>
	{
		eCarTier.TIER_1,
		eCarTier.TIER_2,
		eCarTier.TIER_3,
		eCarTier.TIER_4,
		eCarTier.TIER_5
	};

    public static List<string> TierToNameString = new List<string>
	{
		"E",
		"D",
		"C",
		"B",
		"A",
		"X"
	};
}
