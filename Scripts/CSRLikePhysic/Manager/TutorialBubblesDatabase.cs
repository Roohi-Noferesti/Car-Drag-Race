using UnityEngine;

public class TutorialBubblesDatabase : ConfigurationAssetLoader
{
	public TutorialBubblesConfiguration Configuration
	{
		get;
		private set;
	}

	public TutorialBubblesDatabase() : base(GTAssetTypes.configuration_file, "TutorialBubblesConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (TutorialBubblesConfiguration) scriptableObject;
            //JsonConverter.DeserializeObject<TutorialBubblesConfiguration>(assetDataString);
        Configuration.Process();
    }

    protected override void OnShutdown()
    {
        base.OnShutdown();
        Configuration.Clear();
    }
}
