using System;

public class GTDateTime
{
	public static TimeSpan LocalTimeDelta = new TimeSpan(0L);

	public static bool AdjustServerTime = false;

	public static DateTime UtcNow
	{
		get
		{
			return DateTime.UtcNow + GTDateTime.LocalTimeDelta;
		}
	}

	public static DateTime Now
	{
		get
		{
			return DateTime.Now + GTDateTime.LocalTimeDelta;
		}
	}

	public static DateTime Today
	{
		get
		{
			return GTDateTime.Now.Date;
		}
	}
}
