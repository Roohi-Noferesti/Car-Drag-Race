using UnityEngine;

public class DealDatabase : ConfigurationAssetLoader
{
	public DealConfiguration Configuration
	{
		get;
		private set;
	}

    public DealDatabase()
        : base(GTAssetTypes.configuration_file, "DealConfiguration")
    {
        this.Configuration = null;
    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (DealConfiguration) scriptableObject;
    }
}
