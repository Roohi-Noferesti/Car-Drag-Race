using System.Collections.Generic;

public class CurrencyStringInfo
{
	public string originalString;

	public string digits;

	public List<string> symbols = new List<string>();

	public List<CurrencySymbolClass> symbolClasses = new List<CurrencySymbolClass>();

	public List<int> symbolDigitOffsets = new List<int>();

	public double currencyValue;

	public string thousandsSeperator = string.Empty;

	public string decimalSeperator = string.Empty;

	public string prefix = string.Empty;

	public string suffix = string.Empty;
}
