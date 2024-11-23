using System.Collections;
using System.Collections.Generic;
using Objectives;
using UnityEngine;

public class ObjectiveDatabase : ConfigurationAssetLoader
{
    public ObjectiveConfiguration Configuration;
    public ObjectiveDatabase(GTAssetTypes assetType, string assetID) : base(assetType, assetID)
    {
    }

    public ObjectiveDatabase() : base(GTAssetTypes.configuration_file, "ObjectiveConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        Configuration = (ObjectiveConfiguration) scriptableObject;
        ObjectiveManager.Instance.SetConfigDataV2(Configuration);
    }
}
