using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeasonEventDatabase : ConfigurationAssetLoader
{
	private SeasonEventsMetadata _events;

	private Dictionary<int, int> eventIDLookUp = new Dictionary<int, int>();

	public SeasonEventDatabase() : base(GTAssetTypes.configuration_file, "Seasons")
	{
		this._events = null;
		this.eventIDLookUp = null;
	}

	public SeasonEventMetadata GetEvent(int eventID)
	{
		int index = -1;
		if (this.eventIDLookUp != null && this.eventIDLookUp.TryGetValue(eventID, out index))
		{
			return this._events.Events[index];
		}
		return null;
	}

	public SeasonEventsMetadata GetAllEvents()
	{
		return this._events;
	}

	public bool IsSeasonCurrentEvent(int seasonDisplayNumber)
	{
		SeasonEventMetadata seasonEvent = this._events.Events.Find((SeasonEventMetadata p) => p.SeasonDisplayNumber == seasonDisplayNumber);
		if (seasonEvent == null)
		{
			return false;
		}
		RtwLeaderboardStatusItem rtwLeaderboardStatusItem = SeasonServerDatabase.Instance.GetAllLeaderboardStatuses().LastOrDefault((RtwLeaderboardStatusItem p) => p.event_id == seasonEvent.ID);
		return rtwLeaderboardStatusItem != null && rtwLeaderboardStatusItem.active && !rtwLeaderboardStatusItem.finished;
	}

	public bool IsSeasonFutureEvent(int seasonDisplayNumber)
	{
		foreach (RtwLeaderboardStatusItem current in SeasonServerDatabase.Instance.GetAllLeaderboardStatuses())
		{
			SeasonEventMetadata @event = this.GetEvent(current.event_id);
			if (@event != null)
			{
				if (@event.SeasonDisplayNumber == seasonDisplayNumber)
				{
					bool result = !current.active && !current.finished;
					return result;
				}
				if (@event.SeasonDisplayNumber > seasonDisplayNumber && (current.active || current.finished))
				{
					bool result = false;
					return result;
				}
			}
		}
		return true;
	}

	public bool ContainsEvent(int eventID)
	{
		SeasonEventMetadata seasonEventMetadata = this._events.Events.Find((SeasonEventMetadata p) => p.ID == eventID);
		if (seasonEventMetadata == null)
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(seasonEventMetadata.SeasonCarImageBundle);
		bool flag2 = flag && AssetDatabaseClient.Instance.Data.AssetExists(seasonEventMetadata.SeasonCarImageBundle);
		if (flag2 || flag)
		{
		}
		return flag2 || !flag;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this._events = JsonConverter.DeserializeObject<SeasonEventsMetadata>(assetDataString);
    //    this.eventIDLookUp = new Dictionary<int, int>();
    //    for (int i = 0; i < this._events.Events.Count; i++)
    //    {
    //        SeasonEventMetadata seasonEventMetadata = this._events.Events[i];
    //        this.eventIDLookUp.Add(seasonEventMetadata.ID, i);
    //    }
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this._events = (SeasonEventsMetadata) scriptableObject;
        this.eventIDLookUp = new Dictionary<int, int>();
        for (int i = 0; i < this._events.Events.Count; i++)
        {
            SeasonEventMetadata seasonEventMetadata = this._events.Events[i];
            this.eventIDLookUp.Add(seasonEventMetadata.ID, i);
        }
    }
}
