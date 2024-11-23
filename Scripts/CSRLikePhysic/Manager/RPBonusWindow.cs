using System;

public class RPBonusWindow
{
	public DateTime StartTime
	{
		get;
		private set;
	}

	public DateTime EndTime
	{
		get;
		private set;
	}

	public RPBonusWindow()
	{
		this.StartTime = DateTime.MinValue;
		this.EndTime = DateTime.MinValue;
	}

	public RPBonusWindow(DateTime inStart, DateTime inFinish)
	{
		this.StartTime = inStart;
		this.EndTime = inFinish;
	}

	public bool IsInsideWindow(DateTime NowTime)
	{
		return NowTime >= this.StartTime && NowTime < this.EndTime;
	}

	public TimeSpan TimeRemaining()
	{
		return this.TimeUntil(this.EndTime);
	}

	public TimeSpan TimeUntilStart()
	{
		return this.TimeUntil(this.StartTime);
	}

	public bool IsAbleToAwardBonus()
	{
		return ServerSynchronisedTime.Instance.ServerTimeValid && this.IsInsideWindow(ServerSynchronisedTime.Instance.GetDateTime());
	}

	public bool IsPermanent()
	{
		return this.EndTime == DateTime.MaxValue;
	}

	public bool IsInvalid()
	{
		return this.StartTime == DateTime.MinValue && this.EndTime == DateTime.MinValue;
	}

	private TimeSpan TimeUntil(DateTime time)
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return TimeSpan.Zero;
		}
		return RPBonusWindow.Max(time - ServerSynchronisedTime.Instance.GetDateTime(), TimeSpan.Zero);
	}

	private static TimeSpan Max(TimeSpan t1, TimeSpan t2)
	{
		return (!(t1 > t2)) ? t2 : t1;
	}

	public string GetTimeRemainingString()
	{
		if (!this.IsAbleToAwardBonus())
		{
			return string.Empty;
		}
	    return this.TimeRemaining().ToString();//LocalisationManager.GetNiceTimeSpan(this.TimeRemaining(), false);
	}
}
