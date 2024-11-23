using System;
using System.Collections.Generic;

[Serializable]
public class PrizeOMaticData
{
    public int NumOfDailyPrize = 2;

	public int CashRewardBase = 800;

	public float CashRewardMultiplierTiny = 0.5f;

	public float CashRewardMultiplierSmall = 0.75f;

	public float CashRewardMultiplierMedium = 1f;

	public float CashRewardMultiplierLarge = 1.5f;

	public float CashRewardMultiplierHuge = 2f;

	public float EliteCashRewardMultiplier = 1.5f;

	public float WorldTourCashRewardMultiplier = 3f;

	public int GoldRewardTiny = 1;

	public int GoldRewardSmall = 3;

	public int GoldRewardMedium = 5;

	public int GoldRewardLarge = 7;

	public int GoldRewardHuge = 10;

    public int KeyRewardTiny = 1;

    public int KeyRewardSmall = 3;

    public int KeyRewardMedium = 5;

    public int KeyRewardLarge = 7;

    public int KeyRewardHuge = 10;

	public int PipsOfFuelReward = 2;

	public int FuelPipsBelowFullToDiableFillTankReward = 3;

    public List<WinnableCar> WinnableCars;

	public List<PrizeRarityDataEntry> PrizeRarityData;

	public PrizeomaticRewards PrizeomaticRewards;

	public PrizeomaticScriptingData PrizeomaticScriptingData;

	public int FreeUpgradesSavedLimit = 10;

	public int FreeUpgradesFallbackFuelTheshold = 6;

	public Reward FreeUpgradesFallback = Reward.RPLarge;

    public Reward FuelPipFallbackUnlimitedFuel;

    public Reward FuelRefillFallbackUnlimitedFuel;
    public float Tier1CashMultiplier;
    public float Tier2CashMultiplier;
    public float Tier3CashMultiplier;
    public float Tier4CashMultiplier;
    public float Tier5CashMultiplier;
}
