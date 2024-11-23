using System;
using UnityEngine;

public class RPBonusDatabase : ConfigurationAssetLoader
{
	public RPBonusConfiguration Configuration
	{
		get;
		private set;
	}

	public RPBonusDatabase() : base(GTAssetTypes.configuration_file, "RPBonusConfiguration")
	{
		this.Configuration = null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<RPBonusConfiguration>(assetDataString);
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (RPBonusConfiguration) scriptableObject;
    }
}
