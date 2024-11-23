using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PricesDatabase : ConfigurationAssetLoader
{
    public Dictionary<string, PriceInfo> Cars;

    public Dictionary<string, PriceInfo> Upgrades;

    public Dictionary<string, PriceInfo> Liveries;


	public PricesConfiguration Configuration
	{
		get;
		private set;
	}

	public PricesDatabase() : base(GTAssetTypes.configuration_file, "PricesConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (PricesConfiguration) scriptableObject;//JsonConverter.DeserializeObject<PricesConfiguration>(assetDataString);
        this.Configuration.Initialize();
        Cars = this.Configuration.Cars;
        Upgrades = this.Configuration.Upgrades;
        Liveries = this.Configuration.Liveries;
        Resources.UnloadAsset(this.Configuration);
        this.Configuration = null;
	}

	private bool GetCarPriceInfo(string carDBKey, out PriceInfo info)
	{
        return this.Cars.TryGetValue(carDBKey, out info);
	}

	private bool GetUpgradePriceInfo(string upgradeID, out PriceInfo info)
	{
        return this.Upgrades.TryGetValue(upgradeID, out info);
	}

	private bool GetLiveryPriceInfo(string liveryID, out PriceInfo info)
	{
        return this.Liveries.TryGetValue(liveryID, out info);
	}

	public int GetCarCashPrice(string carDBKey)
	{
		PriceInfo priceInfo = null;
		return (!this.GetCarPriceInfo(carDBKey, out priceInfo)) ? 0 : priceInfo.Cash;
	}

	public int GetCarGoldPrice(string carDBKey)
	{
		PriceInfo priceInfo = null;
		return (!this.GetCarPriceInfo(carDBKey, out priceInfo)) ? 0 : priceInfo.Gold;
	}

	public int GetUpgradeCashPrice(string upgradeID)
	{
		PriceInfo priceInfo = null;
		return (!this.GetUpgradePriceInfo(upgradeID, out priceInfo)) ? 0 : priceInfo.Cash;
	}

	public int GetUpgradeGoldPrice(string upgradeID)
	{
		PriceInfo priceInfo = null;
		return (!this.GetUpgradePriceInfo(upgradeID, out priceInfo)) ? 0 : priceInfo.Gold;
	}

	public int GetLiveryCashPrice(string liveryID)
	{
		PriceInfo priceInfo = null;
		if (!this.GetLiveryPriceInfo(liveryID, out priceInfo))
			return 0;
		else
			return priceInfo.Cash;
	}

	public int GetLiveryGoldPrice(string liveryID)
	{
		PriceInfo priceInfo = null;
		return (!this.GetLiveryPriceInfo(liveryID, out priceInfo)) ? 0 : priceInfo.Gold;
	}
}
