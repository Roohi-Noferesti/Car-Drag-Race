using LitJson;
using System;

public class ApplyableCarReward
{
	private eCarSource carSource;

	private int paintIndex;

	//private int interiorID = -2147483648;

	//private int calliperID = -2147483648;

	//private int liveryZonesSetID = -2147483648;

	//private int liveryID = -2147483648;

	//private string rimAssetName = string.Empty;

	//private bool pendingArrival;

    //private CarVisualsGachaConfig gachaConfig = new CarVisualsGachaConfig();

	public ApplyableCarReward()
	{
	}

	public ApplyableCarReward(CarGarageInstance carInstance)
	{
        //this.carSource = carInstance.CarSource;
        //this.paintIndex = carInstance.OriginalPaintIndex;
        //this.interiorID = carInstance.OriginalInteriorID;
        //this.calliperID = carInstance.OriginalCalliperID;
        //this.liveryZonesSetID = carInstance.OriginalLiveryZonesSetID;
        //this.liveryID = carInstance.OriginalLiveryID;
        //this.rimAssetName = carInstance.AppliedCustomisationNames[0];
        //this.gachaConfig.Clone(carInstance.GachaConfig);
	}

	public void GiveCar(string carDBKey)
	{
        //int carUID = PlayerProfileManager.Instance.ActiveProfile.GiveCar(carDBKey, this.carSource, this.paintIndex, this.interiorID, this.calliperID, this.rimAssetName, this.pendingArrival, this.liveryZonesSetID, this.liveryID, this.gachaConfig, -2147483648);
        //if (this.carSource == eCarSource.High_Stakes)
        //{
        //    CarGarageInstance carFromUID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromUID(carUID);
        //    if (carFromUID == null)
        //    {
        //    }
        //    foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
        //    {
        //        CarUpgradeStatus carUpgradeStatus = carFromUID.UpgradeStatus[current];
        //        carUpgradeStatus.levelFitted = 6;
        //        carUpgradeStatus.levelOwned = 6;
        //    }
        //}
	}

	public void SerialiseToJson(out JsonDict dict)
	{
	    //dict = new JsonDict();
        //dict.SetEnum<eCarSource>("carSource", this.carSource);
        //dict.Set("paintIndex", this.paintIndex);
        //dict.Set("interiorID", this.interiorID);
        //dict.Set("calliperID", this.calliperID);
        //dict.Set("rimAssetName", this.rimAssetName);
        //dict.Set("liveryZonesSetID", this.liveryZonesSetID);
        //dict.Set("liveryID", this.liveryID);
        //dict.Set("pendingArrival", this.pendingArrival);
        //this.gachaConfig.SerialiseToJson(dict);
	    dict = null;
	}

    public void ParseFromJSon(JsonDict dict)
	{
        //this.carSource = dict.GetEnum<eCarSource>("carSource");
        //this.paintIndex = dict.GetInt("paintIndex");
        //this.interiorID = dict.GetInt("interiorID", -2147483648);
        //this.calliperID = dict.GetInt("calliperID", -2147483648);
        //this.rimAssetName = dict.GetString("rimAssetName");
        //this.liveryZonesSetID = dict.GetInt("liveryZonesSetID", -2147483648);
        //this.liveryID = dict.GetInt("liveryID", -2147483648);
        //this.pendingArrival = dict.GetBool("pendingArrival", false);
        //this.gachaConfig.SerialiseFromJson(ref dict);
	}
}
