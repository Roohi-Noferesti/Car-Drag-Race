using UnityEngine;

public class MiniStoreDatabase : ConfigurationAssetLoader
{
	public MiniStoreConfiguration Configuration
	{
		get;
		private set;
	}

	public MiniStoreDatabase() : base(GTAssetTypes.configuration_file, "MiniStoreConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (MiniStoreConfiguration) scriptableObject;//JsonConverter.DeserializeObject<MiniStoreConfiguration>(assetDataString);
	}
}
