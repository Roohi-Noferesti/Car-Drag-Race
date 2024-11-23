using UnityEngine;

public class DailyBattleRewardDatabase : ConfigurationAssetLoader
{
    //public JsonDict JsonData
    //{
    //    get;
    //    private set;
    //}

    public DailyBattleRewardConfiguration Configuration;

    public DailyBattleRewardDatabase()
        : base(GTAssetTypes.configuration_file, "DailyBattleRewardConfiguration")
	{
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    Configuration = (DailyBattleRewardConfiguration) scriptableObject;
	    //JsonDict jsonDict = new JsonDict();
	    //if (!jsonDict.Read(assetDataString))
	    //{
	    //    return;
	    //}
	    //this.JsonData = jsonDict;
	}
}
