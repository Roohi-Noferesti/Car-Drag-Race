using UnityEngine;

public class RevenueTrackingDatabase : ConfigurationAssetLoader
{
    public RevenueTrackingConfiguration Configuration
    {
        get;
        private set;
    }

    public RevenueTrackingDatabase()
        : base(GTAssetTypes.configuration_file, "RevenueTrackingConfiguration")
    {
        this.Configuration = null;
    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (RevenueTrackingConfiguration) scriptableObject;//JsonConverter.DeserializeObject<RevenueTrackingConfiguration>(assetDataString);
	}
}
