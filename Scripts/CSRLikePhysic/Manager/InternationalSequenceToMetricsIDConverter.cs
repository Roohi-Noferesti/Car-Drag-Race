using System;
using System.Linq;
using System.Text.RegularExpressions;

public class InternationalSequenceToMetricsIDConverter : ISequenceToMetricsIDConverter
{
	private const string Match = "Int_Branch_[1-4]";

	public string ConvertToMetricsID(string sequenceID)
	{
		if (!Regex.IsMatch(sequenceID, "Int_Branch_[1-4]"))
		{
			return null;
		}
		string text = sequenceID.Substring("Int_Branch_".Length);
		string[] array = text.Split(new char[]
		{
			'_'
		});
		switch (array.Length)
		{
		case 1:
			return array[0] + "Q";
		case 2:
			return array[0] + array[1].First<char>();
		case 3:
			return array[0] + array[2].First<char>();
		default:
			return null;
		}
	}
}
