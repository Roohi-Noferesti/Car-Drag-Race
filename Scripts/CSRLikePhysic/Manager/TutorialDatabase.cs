using UnityEngine;

public class TutorialDatabase : ConfigurationAssetLoader
{
	public TutorialConfiguration Configuration
	{
		get;
		private set;
	}

	public TutorialDatabase() : base(GTAssetTypes.configuration_file, "TutorialConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (TutorialConfiguration) scriptableObject;//JsonConverter.DeserializeObject<TutorialConfiguration>(assetDataString);
	}
}
