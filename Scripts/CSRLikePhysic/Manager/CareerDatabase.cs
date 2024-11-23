using System;
using System.Collections.Generic;
using UnityEngine;

public class CareerDatabase : ConfigurationAssetLoader
{
	public Dictionary<int, RaceEventData> RaceEventsById;

	public CareerConfiguration Configuration
	{
		get;
		private set;
	}

	public bool Ready
	{
		get
		{
			return this.Configuration != null && this.RaceEventsById.Count != 0;
		}
	}

    public CareerDatabase()
        : base(GTAssetTypes.career_metadata, "CareerConfiguration")
	{
		this.Configuration = null;
		this.RaceEventsById = new Dictionary<int, RaceEventData>();
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (CareerConfiguration) scriptableObject;//JsonConverter.DeserializeObject<CareerConfiguration>(assetDataString);
		CarDatabase.Instance.WhenReady(new Action(this.ProcessLoadedEventsNow));
	}

	public void RegisterEvent(RaceEventData eventData)
	{
		if (this.RaceEventsById.ContainsKey(eventData.EventID))
		{
		}
		this.RaceEventsById[eventData.EventID] = eventData;
	}

	public RaceEventData GetEventByEventIndex(int zEventIndex)
	{
		if (!this.RaceEventsById.ContainsKey(zEventIndex))
		{
			return null;
		}
		return this.RaceEventsById[zEventIndex];
	}

	public bool IsWorldTourUnlocked()
	{
		return PlayerProfileManager.Instance.ActiveProfile.EventsCompleted.Contains(this.Configuration.EventIDToUnlockWorldTour);
	}

	private void ProcessLoadedEventsNow()
	{
		this.RaceEventsById.Clear();
		this.Configuration.CareerRaceEvents.ProcessEvents();
        RaceEventDatabase.instance.SetAndProcessCareerRaceEventsData(this.Configuration.CareerRaceEvents);
	}
}
