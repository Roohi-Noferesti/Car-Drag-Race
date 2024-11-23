using System.Linq;
using DataSerialization;
using I2.Loc;

public static class FormatStringDataExtensions
{
	public static string GetFormatString(this FormatStringData fsd)
	{
		string text = fsd.StringFormatBase;
		if (!fsd.AlreadyTranslated)
		{
			text = LocalizationManager.GetTranslation(fsd.StringFormatBase);
		}
		if (fsd.StringFormatParameters.Count == 0)
		{
			return text;
		}
		object[] args = (from p in fsd.StringFormatParameters
		select p.GetDecoratedString()).Cast<object>().ToArray<object>();
		return string.Format(text, args);
	}
}
