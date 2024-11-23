using System;
using System.Collections.Generic;

public static class PrizeomaticAwarding
{
	private static Dictionary<Reward, Func<AwardPrizeBase>> PrizeFactories;

	static PrizeomaticAwarding()
	{
		// Note: this type is marked as 'beforefieldinit'.
		Dictionary<Reward, Func<AwardPrizeBase>> dictionary = new Dictionary<Reward, Func<AwardPrizeBase>>();
		dictionary.Add(Reward.SportCarPart, () => new AwardCarPrize(Reward.SportCarPart));
		dictionary.Add(Reward.DesiribleCarPart1, () => new AwardCarPrize(Reward.DesiribleCarPart1));
		dictionary.Add(Reward.DesiribleCarPart2, () => new AwardCarPrize(Reward.DesiribleCarPart2));
		dictionary.Add(Reward.CommonCarPart1, () => new AwardCarPrize(Reward.CommonCarPart1));
		dictionary.Add(Reward.CommonCarPart2, () => new AwardCarPrize(Reward.CommonCarPart2));
		dictionary.Add(Reward.CommonCarPart3, () => new AwardCarPrize(Reward.CommonCarPart3));
		dictionary.Add(Reward.CashTiny, () => new AwardCashPrize(RewardSize.Tiny));
		dictionary.Add(Reward.CashSmall, () => new AwardCashPrize(RewardSize.Small));
		dictionary.Add(Reward.CashMedium, () => new AwardCashPrize(RewardSize.Medium));
		dictionary.Add(Reward.CashLarge, () => new AwardCashPrize(RewardSize.Large));
		dictionary.Add(Reward.CashHuge, () => new AwardCashPrize(RewardSize.Huge));
		dictionary.Add(Reward.GoldTiny, () => new AwardGoldPrize(RewardSize.Tiny));
		dictionary.Add(Reward.GoldSmall, () => new AwardGoldPrize(RewardSize.Small));
		dictionary.Add(Reward.GoldMedium, () => new AwardGoldPrize(RewardSize.Medium));
		dictionary.Add(Reward.GoldLarge, () => new AwardGoldPrize(RewardSize.Large));
		dictionary.Add(Reward.GoldHuge, () => new AwardGoldPrize(RewardSize.Huge));
        dictionary.Add(Reward.KeyTiny, () => new AwardKeyPrize(RewardSize.Tiny));
        dictionary.Add(Reward.KeySmall, () => new AwardKeyPrize(RewardSize.Small));
        dictionary.Add(Reward.KeyMedium, () => new AwardKeyPrize(RewardSize.Medium));
        dictionary.Add(Reward.KeyLarge, () => new AwardKeyPrize(RewardSize.Large));
        dictionary.Add(Reward.KeyHuge, () => new AwardKeyPrize(RewardSize.Huge));
		dictionary.Add(Reward.RPTiny, () => new AwardRPPrize(RewardSize.Tiny));
		dictionary.Add(Reward.RPSmall, () => new AwardRPPrize(RewardSize.Small));
		dictionary.Add(Reward.RPMedium, () => new AwardRPPrize(RewardSize.Medium));
		dictionary.Add(Reward.RPLarge, () => new AwardRPPrize(RewardSize.Large));
		dictionary.Add(Reward.RPHuge, () => new AwardRPPrize(RewardSize.Huge));
		dictionary.Add(Reward.PipsOfFuel, () => new AwardFuelPipsPrize());
		dictionary.Add(Reward.FuelRefill, () => new AwardFuelTankPrize());
		dictionary.Add(Reward.FreeUpgrade, () => new AwardFreeUpgradePrize());
		dictionary.Add(Reward.ProTuner, () => new AwardConsumablePrize(eCarConsumables.EngineTune));
		dictionary.Add(Reward.TiresCrew, () => new AwardConsumablePrize(eCarConsumables.Tyre));
		dictionary.Add(Reward.N20Maniac, () => new AwardConsumablePrize(eCarConsumables.Nitrous));
		PrizeomaticAwarding.PrizeFactories = dictionary;
	}

	public static AwardPrizeBase CreatePrize(Reward prizeToCreate)
	{
		if (PrizeomaticAwarding.PrizeFactories.ContainsKey(prizeToCreate))
		{
			Func<AwardPrizeBase> func = PrizeomaticAwarding.PrizeFactories[prizeToCreate];
			return func();
		}
		return new AwardFreeUpgradePrize();
	}
}
