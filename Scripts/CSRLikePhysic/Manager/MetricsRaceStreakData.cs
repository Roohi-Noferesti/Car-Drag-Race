using System;

public struct MetricsRaceStreakData
{
	public string Result;

	public int PPDifference;

	public int RPDelta;

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"Res ",
			this.Result,
			" ",
			this.PPDifference.ToString(),
			" ",
			this.RPDelta.ToString(),
			"\n"
		});
	}
}
