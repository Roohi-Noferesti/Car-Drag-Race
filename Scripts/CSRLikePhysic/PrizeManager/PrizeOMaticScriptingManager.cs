using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class PrizeOMaticScriptingManager
{
	public static PrizeomaticScriptingData ScriptingData;

	private static Dictionary<string, Reward> rewardsMap = new Dictionary<string, Reward>
	{
		{
			"FreeUpgrade",
			Reward.FreeUpgrade
		},
		{
			"CommonCarPart1",
			Reward.CommonCarPart1
		},
		{
			"CommonCarPart2",
			Reward.CommonCarPart2
		},
		{
			"CommonCarPart3",
			Reward.CommonCarPart3
		},
		{
			"DesirableCarPart1",
			Reward.DesiribleCarPart1
		},
		{
			"DesirableCarPart2",
			Reward.DesiribleCarPart2
		},
		{
			"SportsCarPart",
			Reward.SportCarPart
		},
		{
			"GoldTiny",
			Reward.GoldTiny
		},
		{
			"GoldSmall",
			Reward.GoldSmall
		},
		{
			"GoldMedium",
			Reward.GoldMedium
		},
		{
			"GoldLarge",
			Reward.GoldLarge
		},
		{
			"GoldHuge",
			Reward.GoldHuge
		},
		{
			"CashTiny",
			Reward.CashTiny
		},
		{
			"CashSmall",
			Reward.CashSmall
		},
		{
			"CashMedium",
			Reward.CashMedium
		},
		{
			"CashLarge",
			Reward.CashLarge
		},
		{
			"CashHuge",
			Reward.CashHuge
		},
		{
			"FuelPips",
			Reward.PipsOfFuel
		},
		{
			"FuelRefill",
			Reward.FuelRefill
		}
	};

	public static bool IsScriptedMoment(int numberofMoments)
	{
		foreach (PrizeomaticMoment current in PrizeOMaticScriptingManager.ScriptingData.ScriptedMoments)
		{
			if (current.NumberOfMomentsNeeded == numberofMoments)
			{
				return true;
			}
		}
		return false;
	}

	public static Reward GetRewardForScriptedMoment(int numberofMoments)
	{
		int index = PrizeOMaticScriptingManager.FindIndexOfPrizeomaticMoment(numberofMoments);
		return PrizeOMaticScriptingManager.GetRewardFromString(PrizeOMaticScriptingManager.ScriptingData.ScriptedMoments[index].Reward);
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public static void ListAllScriptedMoment()
	{
		foreach (PrizeomaticMoment current in PrizeOMaticScriptingManager.ScriptingData.ScriptedMoments)
		{
		}
	}

	private static int FindIndexOfPrizeomaticMoment(int numberOfMoments)
	{
		for (int i = 0; i < PrizeOMaticScriptingManager.ScriptingData.ScriptedMoments.Count; i++)
		{
			if (PrizeOMaticScriptingManager.ScriptingData.ScriptedMoments[i].NumberOfMomentsNeeded == numberOfMoments)
			{
				return i;
			}
		}
		return 0;
	}

	private static Reward GetRewardFromString(string prizeString)
	{
		if (PrizeOMaticScriptingManager.rewardsMap.ContainsKey(prizeString))
		{
			return PrizeOMaticScriptingManager.rewardsMap[prizeString];
		}
		return Reward.CashMedium;
	}
}
