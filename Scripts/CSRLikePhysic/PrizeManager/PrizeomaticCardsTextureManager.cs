using System;
using System.Collections.Generic;

public static class PrizeomaticCardsTextureManager
{
	private const string texturePrefix = "PlayingCards/";

	private static Dictionary<Reward, string> cardTexture = new Dictionary<Reward, string>
	{
		{
			Reward.SportCarPart,
			"playing-card_cars"
		},
		{
			Reward.DesiribleCarPart1,
			"playing-card_cars"
		},
		{
			Reward.DesiribleCarPart2,
			"playing-card_cars"
		},
		{
			Reward.CommonCarPart1,
			"playing-card_cars"
		},
		{
			Reward.CommonCarPart2,
			"playing-card_cars"
		},
		{
			Reward.CommonCarPart3,
			"playing-card_cars"
		},
		{
			Reward.FreeUpgrade,
			"playing-card_spanner"
		},
		{
			Reward.PipsOfFuel,
			"playing-card_two_pips"
		},
		{
			Reward.FuelRefill,
			"playing-card_refuel"
		},
		{
			Reward.CashTiny,
			"playing-card_cash"
		},
		{
			Reward.CashSmall,
			"playing-card_cash"
		},
		{
			Reward.CashMedium,
			"playing-card_cash"
		},
		{
			Reward.CashLarge,
			"playing-card_cash"
		},
		{
			Reward.CashHuge,
			"playing-card_cash"
		},
		{
			Reward.GoldTiny,
			"playing-card_gold"
		},
		{
			Reward.GoldSmall,
			"playing-card_gold"
		},
		{
			Reward.GoldMedium,
			"playing-card_gold"
		},
		{
			Reward.GoldLarge,
			"playing-card_gold"
		},
		{
			Reward.GoldHuge,
			"playing-card_gold"
		},
		{
			Reward.RPTiny,
			"playing-card_rp"
		},
		{
			Reward.RPSmall,
			"playing-card_rp"
		},
		{
			Reward.RPMedium,
			"playing-card_rp"
		},
		{
			Reward.RPLarge,
			"playing-card_rp"
		},
		{
			Reward.RPHuge,
			"playing-card_rp"
		},
		{
			Reward.ProTuner,
			"playing-card_engine"
		},
		{
			Reward.N20Maniac,
			"playing-card_n2o"
		},
		{
			Reward.TiresCrew,
			"playing-card_tire"
		}
	};

	public static string GetTextureForReward(Reward prize)
	{
		if (PrizeomaticCardsTextureManager.cardTexture.ContainsKey(prize))
		{
			return "PlayingCards/" + PrizeomaticCardsTextureManager.cardTexture[prize];
		}
		return string.Empty;
	}
}
