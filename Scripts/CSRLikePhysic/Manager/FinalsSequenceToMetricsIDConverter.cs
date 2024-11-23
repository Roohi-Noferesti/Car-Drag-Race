using System;

public class FinalsSequenceToMetricsIDConverter : ISequenceToMetricsIDConverter
{
	private const string Match = "Int_Branch_5_";

	public string ConvertToMetricsID(string sequenceID)
	{
		if (!sequenceID.StartsWith("Int_Branch_5_"))
		{
			return null;
		}
		if (sequenceID.EndsWith("_Future"))
		{
			return null;
		}
		string text = sequenceID.Substring("Int_Branch_5_".Length);
		if (text.StartsWith("Finals"))
		{
			return "Fin";
		}
		if (text.StartsWith("Evolution"))
		{
			string[] array = text.Split(new char[]
			{
				'_'
			});
			if (array.Length == 2)
			{
				return "Ev" + array[1];
			}
			if (array.Length == 3)
			{
				return "Ev" + array[1] + "F";
			}
		}
		return null;
	}
}
