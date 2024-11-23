using UnityEngine;

public class FlowConditionsDatabase : ConfigurationAssetLoader
{
	public FlowConditionsConfiguration Configuration
	{
		get;
		private set;
	}

	public FlowConditionsDatabase() : base(GTAssetTypes.configuration_file, "FlowConditionsConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (FlowConditionsConfiguration) scriptableObject;//JsonConverter.DeserializeObject<FlowConditionsConfiguration>(assetDataString);
	    Configuration.Init();
    }
}
