using System;

public class AdFinishedEventArgs : EventArgs
{
	public enum Status
	{
		Succeeded,
		Failed,
		Dismissed
	}

	public Status AdStatus
	{
		get;
		private set;
	}

	public AdFinishedEventArgs(Status status)
	{
		this.AdStatus = status;
	}
}
