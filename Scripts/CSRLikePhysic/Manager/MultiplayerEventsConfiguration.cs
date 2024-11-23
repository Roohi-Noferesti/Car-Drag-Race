using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MultiplayerEventsConfiguration:ScriptableObject
{
	public List<MultiplayerEventData> Events = new List<MultiplayerEventData>();

	public void Initialise()
	{
		foreach (MultiplayerEventData current in this.Events)
		{
			current.Initialise();
		}
	}

	public MultiplayerEventData GetActiveEvent()
	{
		return this.Events.Find((MultiplayerEventData e) => e.IsActive());
	}

	public MultiplayerEventData GetPreviousEvent()
	{
		if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			DateTime currentTime = ServerSynchronisedTime.Instance.GetDateTime();
			return (from e in this.Events
			where e.EndTime < currentTime
			orderby e.EndTime descending
			select e).FirstOrDefault<MultiplayerEventData>();
		}
		return null;
	}

	public MultiplayerEventData GetNextEvent()
	{
		if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			DateTime currentTime = ServerSynchronisedTime.Instance.GetDateTime();
			return (from e in this.Events
			where e.StartTime > currentTime
			orderby e.StartTime
			select e).FirstOrDefault<MultiplayerEventData>();
		}
		return null;
	}

	public MultiplayerEventData GetEventByID(int ID)
	{
		return this.Events.Find((MultiplayerEventData e) => e.ID == ID);
	}
}
