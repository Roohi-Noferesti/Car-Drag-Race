using System;

public class LadderSequenceToMetricsIDConverter : ISequenceToMetricsIDConverter
{
	private const string Match = "Ladder_";

	public string ConvertToMetricsID(string sequenceID)
	{
		if (sequenceID.EndsWith("_intro"))
		{
			return null;
		}
		if (!sequenceID.StartsWith("Ladder_"))
		{
			return null;
		}
		return sequenceID.Substring("Ladder_".Length);
	}
}
