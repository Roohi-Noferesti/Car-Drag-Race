using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplayerEventsDatabase : ConfigurationAssetLoader
{
	private Dictionary<int, MultiplayerEventData> events;

	public MultiplayerEventsConfiguration Configuration
	{
		get;
		private set;
	}

	public MultiplayerEventsDatabase() : base(GTAssetTypes.configuration_file, "MultiplayerEventsConfiguration")
	{
        this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (MultiplayerEventsConfiguration) scriptableObject;//JsonConverter.DeserializeObject<MultiplayerEventsConfiguration>(assetDataString);
        this.Configuration.Initialise();
        this.events = this.Configuration.Events.ToDictionary((MultiplayerEventData x) => x.ID, (MultiplayerEventData x) => x);
    }

	public MultiplayerEventData GetActiveEvent()
	{
		return this.Configuration.GetActiveEvent();
	}

	public MultiplayerEventData GetNextEvent()
	{
		return this.Configuration.GetNextEvent();
	}

	public MultiplayerEventData GetPreviousEvent()
	{
		return this.Configuration.GetPreviousEvent();
	}

	public List<MultiplayerEventData> GetAllEvents()
	{
		return this.Configuration.Events;
	}

	public MultiplayerEventData GetEventByID(int ID)
	{
		MultiplayerEventData multiplayerEventData;
		return (!this.events.TryGetValue(ID, out multiplayerEventData)) ? null : multiplayerEventData;
	}
}
