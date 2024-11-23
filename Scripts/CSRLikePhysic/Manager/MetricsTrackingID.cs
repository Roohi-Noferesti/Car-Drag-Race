using System;

public class MetricsTrackingID
{
	private long ID = MetricsTrackingID.Generate();

	private static long Generate()
	{
		return DateTime.UtcNow.Subtract(new DateTime(2015, 1, 1)).Ticks;
	}

	public override string ToString()
	{
		return this.ID.ToString();
	}
}
