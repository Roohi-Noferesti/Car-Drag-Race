using DataSerialization;
using System;

public static class TextureDetailExtensions
{
	public static string GetName(this TextureDetail td)
	{
		return (!td.AppendThemeChoiceToName) ? td.Name : (td.Name + TierXManager.Instance.CurrentThemeOption);
	}
}
