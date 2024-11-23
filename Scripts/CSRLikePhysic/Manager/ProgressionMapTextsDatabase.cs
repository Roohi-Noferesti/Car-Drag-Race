using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionMapTextsDatabase : ConfigurationAssetLoader
{
	public ProgressionMapTextsConfiguration Configuration
	{
		get;
		private set;
	}

	public ProgressionMapTextsDatabase() : base(GTAssetTypes.configuration_file, "ProgressionMapTextsConfiguration")
	{
		this.Configuration = null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<ProgressionMapTextsConfiguration>(assetDataString);
    //    foreach (ProgressionMapTextData current in this.Configuration.MapTexts)
    //    {
    //        current.Initialise();
    //    }
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (ProgressionMapTextsConfiguration)scriptableObject;
        foreach (ProgressionMapTextData current in this.Configuration.MapTexts)
        {
            current.Initialise();
        }
    }

	public List<ProgressionMapTextData> GetEligibleProgressionMapTexts()
	{
		IGameState gs = new GameStateFacade();
		return this.Configuration.MapTexts.FindAll((ProgressionMapTextData mt) => mt.IsEligibile(gs));
	}

	public ProgressionMapTextData GetEligibleProgressionMapTextData()
	{
		List<ProgressionMapTextData> source = (from p in this.GetEligibleProgressionMapTexts()
		orderby p.Priority descending
		select p).ToList<ProgressionMapTextData>();
		return source.FirstOrDefault<ProgressionMapTextData>();
	}

	public string GetEligibleProgressionMapTextString()
	{
		ProgressionMapTextData eligibleProgressionMapTextData = this.GetEligibleProgressionMapTextData();
		return (eligibleProgressionMapTextData == null) ? string.Empty : eligibleProgressionMapTextData.GetTextString();
	}
}
