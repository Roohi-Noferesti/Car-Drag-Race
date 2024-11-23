using System;
using System.Collections.Generic;

[Serializable]
public class MultiplayerModeTheme
{
	public string Description = string.Empty;

	public string Swatch = string.Empty;

	public bool Premium;

	public string Icon = "Icon";

	public string ThemeTexturePack = string.Empty;

	public string Branding = "Branding";

	public string FeatureImage = "Feature";

	public bool UsesSnapshot;

	public string CarDBkey = "DummyCar";

	public int ColourIndex;

	public string CarLivery = string.Empty;

	public ColourSwatch GetSwatch()
	{
		Dictionary<string, ColourSwatch> swatches = GameDatabase.Instance.OnlineConfiguration.Swatches;
		if (swatches.ContainsKey(this.Swatch))
		{
			return swatches[this.Swatch];
		}
		return ColourSwatch.Default;
	}
}
