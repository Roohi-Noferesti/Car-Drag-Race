using DataSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OnlineDatabase : ConfigurationAssetLoader
{
	public OnlineConfiguration Configuration
	{
		get;
		private set;
	}

	public Reward UnlimitedFuelFallbackPrize_Pips
	{
		get { return this.Configuration.PrizeoMatic.FuelPipFallbackUnlimitedFuel; }
	}

	public Reward UnlimitedFuelFallbackPrize_Refill
	{
		get
		{
			return this.Configuration.PrizeoMatic.FuelRefillFallbackUnlimitedFuel;
		}
	}

	public OnlineDatabase() : base(GTAssetTypes.configuration_file, "OnlineConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (OnlineConfiguration) scriptableObject;//JsonConverter.DeserializeObject<OnlineConfiguration>(assetDataString);
		this.Configuration.Initialise();
        SMPConfigManager.WinStreak.SetConfig(Configuration.SMPWinStreak);
        SMPConfigManager.SMPSessionHoursToReset = Configuration.SMPSessionHoursToReset;
		StreakManager.StreakData = this.Configuration.StreakData;
		PrizeOMaticScriptingManager.ScriptingData = this.Configuration.PrizeoMatic.PrizeomaticScriptingData;
		foreach (EligibilityCondition current in this.Configuration.IsMultiplayerDisabled)
		{
			current.Initialise();
		}
	}

	public List<WinnableCar> GetWinnableCars()
	{
		if (this.Configuration.PrizeoMatic != null && this.Configuration.PrizeoMatic.WinnableCars != null)
		{
			return this.Configuration.PrizeoMatic.WinnableCars;
		}
		return null;
	}

	private WinnableCar GetCarForKey(string carDBKey)
	{
		WinnableCar winnableCar = null;
		if (this.Configuration.PrizeoMatic != null && this.Configuration.PrizeoMatic.WinnableCars != null)
		{
			winnableCar = this.Configuration.PrizeoMatic.WinnableCars.Find((WinnableCar x) => x.CarDBKey == carDBKey);
		}
		if (winnableCar == null)
		{
		}
		return winnableCar;
	}

	private PrizeRarityDataEntry GetRarityData(CarRarity rarity)
	{
		PrizeRarityDataEntry prizeRarityDataEntry = null;
		if (this.Configuration.PrizeoMatic != null && this.Configuration.PrizeoMatic.PrizeRarityData != null)
		{
			prizeRarityDataEntry = this.Configuration.PrizeoMatic.PrizeRarityData.Find((PrizeRarityDataEntry x) => x.Rarity == rarity);
		}
		if (prizeRarityDataEntry == null)
		{
		}
		return prizeRarityDataEntry;
	}

	public int NumTotalCarPiecesToWin(string carDBKey)
	{
		WinnableCar carForKey = this.GetCarForKey(carDBKey);
		if (carForKey != null)
		{
			PrizeRarityDataEntry rarityData = this.GetRarityData(carForKey.Rarity);
			return (int)rarityData.NumPieces;
		}
		return 0;
	}

	public int GoldCostToCompleteCar(string carDBKey, int cardsOwned)
	{
		WinnableCar carForKey = this.GetCarForKey(carDBKey);
		PrizeRarityDataEntry rarityData = this.GetRarityData(carForKey.Rarity);
		CarInfo car = CarDatabase.Instance.GetCar(carDBKey);
		if (cardsOwned >= (int)rarityData.NumPieces)
		{
			return 0;
		}
		int num = rarityData.DiscountPerCardOwned[cardsOwned];
		int goldPrice = car.GoldPrice;
		return Mathf.CeilToInt((float)goldPrice * (1f - (float)num / 100f));
	}

	public int CashCostToCompleteCar(string carDBKey, int cardsOwned)
	{
		WinnableCar carForKey = this.GetCarForKey(carDBKey);
		PrizeRarityDataEntry rarityData = this.GetRarityData(carForKey.Rarity);
		CarInfo car = CarDatabase.Instance.GetCar(carDBKey);
		if (cardsOwned >= (int)rarityData.NumPieces)
		{
			return 0;
		}
		int num = rarityData.DiscountPerCardOwned[cardsOwned];
		int buyPrice = car.BuyPrice;
		if (buyPrice <= 0)
		{
			return 0;
		}
		return Mathf.CeilToInt((float)buyPrice * (1f - (float)num / 100f));
	}


    public OnlineRace[] GetOnlineRacesMatch()
    {
        return Configuration.StakeRaces;
    }

    public int GetPPDeltaOnline(OnlineRace onlineRace, eCarTier carTier)
    {
        var consecutiveLoses = PlayerProfileManager.Instance.ActiveProfile.SMPConsecutiveLoses;
        var consecutiveWins = PlayerProfileManager.Instance.ActiveProfile.SMPConsecutiveWins;
        float loseModifier = Configuration.WinLoseDifficultyModifier.Evaluate(consecutiveWins - consecutiveLoses);
        //var smpTotalRaces = Mathf.Max(10, PlayerProfileManager.Instance.ActiveProfile.SMPTotalRacesLastSession);
        // return onlineRace.GetRandomPPDelta(carTier, loseModifier);
        //Debug.LogError(loseModifier);
        return Mathf.RoundToInt(loseModifier * RemoteConfigManager.Instance.DifficultyLevelMultiplier);
    }

    public int GetStake(short eventOrder, eCarTier carTier)
    {
        var raceData = Configuration.StakeRaces[eventOrder];
        return raceData.GetTierSetting(carTier).Stake;
    }

    public float GetRandomSearchTime()
    {
        return Random.Range(Configuration.MinSearchTime, Configuration.MaxSearchTime);
    }

    public float GetRandomFakeCountDownDelay()
    {
        return Configuration.FakeCountdownDelayCurve.Evaluate(Random.value);
    }
}
