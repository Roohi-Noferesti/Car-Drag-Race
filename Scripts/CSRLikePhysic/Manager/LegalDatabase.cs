using System;
using UnityEngine;

public class LegalDatabase : ConfigurationAssetLoader
{
	public LegalConfiguration Configuration
	{
		get;
		private set;
	}

	public LegalDatabase() : base(GTAssetTypes.configuration_file, "LegalConfiguration")
	{
		this.Configuration = null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<LegalConfiguration>(assetDataString);
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (LegalConfiguration) scriptableObject;
    }
}
