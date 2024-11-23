using System;
using System.Collections.Generic;

[Serializable]
public class ArrivalQueue
{
	public List<Arrival> theQueue
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.ArrivalQueue;
		}
		set
		{
			PlayerProfileManager.Instance.ActiveProfile.ArrivalQueue = value;
		}
	}

	public Arrival At(int i)
	{
		if (i < 0 || i > this.theQueue.Count)
		{
			return null;
		}
		return this.theQueue[i];
	}

	public void Add(Arrival a)
	{
		if (this.theQueue.Count == 0)
		{
			this.theQueue.Add(a);
			return;
		}
		for (int i = this.theQueue.Count - 1; i >= 0; i--)
		{
			if (a.dueTime >= this.theQueue[i].dueTime)
			{
				this.theQueue.Insert(i + 1, a);
				return;
			}
		}
		this.theQueue.Insert(0, a);
	}

	public List<Arrival> GetAllDeliverableArrivals()
	{
        return this.theQueue.FindAll((Arrival i) => i.dueTime.CompareTo(GTDateTime.Now) <= 0);
	}

	public Arrival Pop()
	{
		Arrival result = this.theQueue[0];
		this.theQueue.RemoveAt(0);
		return result;
	}

	public int FindMatchIndex(Arrival a)
	{
		return this.theQueue.FindIndex((Arrival q) => a.Equals(q));
	}

	public List<Arrival> getAllNotArrived()
	{
        return this.theQueue.FindAll((Arrival i) => i.dueTime.CompareTo(GTDateTime.Now) > 0);
	}

	public Arrival Remove(Arrival a)
	{
		int num = this.FindMatchIndex(a);
		if (num < 0)
		{
			return null;
		}
		Arrival result = this.theQueue[num];
		this.theQueue.RemoveAt(num);
		return result;
	}

	public void Clear()
	{
		this.theQueue.Clear();
	}
}
