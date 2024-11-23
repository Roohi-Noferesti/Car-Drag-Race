using UnityEngine;

public class DifficultyDatabase : ConfigurationAssetLoader
{
	public DifficultyConfiguration Configuration
	{
		get;
		private set;
	}

	public DifficultyDatabase() : base(GTAssetTypes.configuration_file, "DifficultyConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (DifficultyConfiguration) scriptableObject;//JsonConverter.DeserializeObject<DifficultyConfiguration>(assetDataString);
	}
}
