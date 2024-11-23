using System;
using System.Collections.Generic;

public class RaceEvents
{
	private Dictionary<string, Action> events = new Dictionary<string, Action>();

    private event Action RaceStartEvent;

    private event Action RaceEndEvent;

    private event Action RaceReset;

    private event Action RaceCountdownStarted;

    private event Action RaceEnteredCloseRaceSlowMo;

	public RaceEvents()
	{
		this.events.Add("RaceStart", this.RaceStartEvent);
		this.events.Add("RaceEnd", this.RaceEndEvent);
		this.events.Add("RaceReset", this.RaceReset);
		this.events.Add("RaceCountdownStarted", this.RaceCountdownStarted);
		this.events.Add("RaceEnteredCloseRaceSlowMo", this.RaceEnteredCloseRaceSlowMo);
	}

	public void HandleEvent(string name, Action handler)
	{
		Dictionary<string, Action> obj = this.events;
		lock (obj)
		{
			this.events[name] += (Action)Delegate.Combine(this.events[name], handler);
		}
	}

	public void StopHandlingEvent(string name, Action handler)
	{
		Dictionary<string, Action> obj = this.events;
		lock (obj)
		{
			this.events[name] = (Action)Delegate.Remove(this.events[name], handler);
		}
	}

	public void FireEvent(string name)
	{
		Action action = this.events[name];
		if (action != null)
		{
			action();
		}
	}
}
